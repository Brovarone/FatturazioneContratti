using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using EFMago.Models;

namespace FatturazioneContratti.Common
{
    public static class FattureContextExtensions
    {
        // Cache per memorizzare le funzioni di inizializzazione pre-compilate per ogni tabella
        private static readonly ConcurrentDictionary<Type, Action<object>> Initializers =
            new ConcurrentDictionary<Type, Action<object>>();

        /// <summary>
        /// Crea una nuova istanza di un'entità (valida per le 116 tabelle) inizializzandola 
        /// con i default reali dichiarati nella Fluent API (HasDefaultValueSql) del contesto.
        /// Replica esattamente il comportamento del metodo AddNew dei DataSet VB.NET.
        /// </summary>
        public static TEntity AddNew<TEntity>(this DbContext context) where TEntity : class, new()
        {
            var entity = new TEntity();

            // Recupera o compila l'inizializzatore specifico per questo tipo di classe
            var initializer = Initializers.GetOrAdd(typeof(TEntity), type =>
            {
                var entityType = context.Model.FindEntityType(type);
                if (entityType == null) return _ => { };

                var propertiesWithDefaults = entityType.GetProperties()
                    .Where(p => !string.IsNullOrEmpty(p.GetDefaultValueSql()) || p.GetDefaultValue() != null)
                    .ToList();

                return obj =>
                {
                    foreach (var prop in propertiesWithDefaults)
                    {
                        var pi = prop.PropertyInfo;
                        if (pi == null || !pi.CanWrite) continue;

                        object value = prop.GetDefaultValue();

                        // Se non c'è un default nativo, analizziamo la stringa HasDefaultValueSql dell'ERP Mago
                        if (value == null)
                        {
                            var sql = prop.GetDefaultValueSql();
                            if (!string.IsNullOrWhiteSpace(sql))
                            {
                                value = EstraiValoreDaSqlLiteral(sql, pi.PropertyType);
                            }
                        }

                        if (value != null)
                        {
                            pi.SetValue(obj, value);
                        }
                    }
                };
            });

            // Applica i default sull'oggetto appena creato
            initializer(entity);
            return entity;
        }

        /// <summary>
        /// Estrattore ultra-resiliente per i letterali SQL di Mago: ((0.00)) | ('0') | ('') | ('17991231')
        /// </summary>
        private static object EstraiValoreDaSqlLiteral(string sql, Type targetType)
        {
            var s = sql.Trim();
            while (s.Length >= 2 && s[0] == '(' && s[s.Length - 1] == ')')
                s = s.Substring(1, s.Length - 2).Trim();

            if (s.Length >= 3 && (s[0] == 'N' || s[0] == 'n') && s[1] == '\'') s = s.Substring(1);
            if (s.Length >= 2 && s[0] == '\'' && s[s.Length - 1] == '\'') s = s.Substring(1, s.Length - 2).Replace("''", "'");

            // Se è un'espressione dinamica (es. GETDATE(), NEWID()), non possiamo leggerla in memoria
            if (s.Contains("(") || s.Contains(" ")) return null;

            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            try
            {
                if (underlyingType == typeof(string)) return s;
                if (underlyingType == typeof(bool)) return s == "1" || s.Equals("true", StringComparison.OrdinalIgnoreCase);
                if (underlyingType == typeof(DateTime))
                {
                    if (DateTime.TryParseExact(s, new[] { "yyyyMMdd", "yyyy-MM-dd", "yyyyMMdd HH:mm:ss", "yyyy-MM-dd HH:mm:ss" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                        return dt;
                    return null;
                }

                // Forza la conversione numerica corretta per short (smallint), int, decimal, byte
                return Convert.ChangeType(s, underlyingType, CultureInfo.InvariantCulture);
            }
            catch
            {
                return null; // Salta in modalità sicura se il dato è incompatibile
            }
        }
    }
}
