using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
// NON LA USO PIU' NON LAVORAVA BENE SU SMALLINT
namespace FatturazioneContratti.Common
{
    /// <summary>
    /// Replica in memoria i DEFAULT del modello EF (HasDefaultValue / HasDefaultValueSql).
    ///
    /// Regola identica a quella di SaveChanges: una proprietà con DEFAULT il cui valore è
    /// uguale al default CLR (0, null, DateTime.MinValue...) è considerata "non passata"
    /// e riceve il valore di default del DB.
    ///
    /// EFCore.BulkExtensions NON lo fa: scrive esattamente quello che c'è nell'entità,
    /// quindi va applicato PRIMA di BulkInsert.
    /// </summary>
    public static class DbDefaults
    {
        private sealed class Slot
        {
            public PropertyInfo Property;
            public object ClrDefault;   // default(T) della proprietà (null per reference/nullable)
            public object DbDefault;    // valore del DEFAULT del DB già convertito nel tipo CLR
        }

        private static readonly ConcurrentDictionary<Type, Slot[]> Cache =
            new ConcurrentDictionary<Type, Slot[]>();

        private static readonly string[] DateFormats =
        {
            "yyyyMMdd", "yyyy-MM-dd", "yyyyMMdd HH:mm:ss", "yyyy-MM-dd HH:mm:ss", "yyyy-MM-ddTHH:mm:ss"
        };

        /// <summary>
        /// Imposta il default del DB su ogni proprietà "non passata". Restituisce la stessa entità
        /// così si può usare anche a valle di un new: DbDefaults.Apply(ctx, new X { ... }).
        /// </summary>
        public static T Apply<T>(DbContext context, T entity) where T : class
        {
            var slots = Cache.GetOrAdd(typeof(T), t => Build(context, t));

            foreach (var s in slots)
            {
                var current = s.Property.GetValue(entity);
                if (Equals(current, s.ClrDefault))
                    s.Property.SetValue(entity, s.DbDefault);
            }

            return entity;
        }

        private static Slot[] Build(DbContext context, Type type)
        {
            var result = new List<Slot>();
            var entityType = context.Model.FindEntityType(type);
            if (entityType == null) return result.ToArray();

            foreach (var p in entityType.GetProperties())
            {
                var pi = p.PropertyInfo;
                if (pi == null || !pi.CanRead || !pi.CanWrite) continue;
                if (p.IsPrimaryKey()) continue;
                if (p.ValueGenerated == ValueGenerated.OnAddOrUpdate) continue; // rowversion / colonne calcolate

                // 1) HasDefaultValue(...)
                object value = p.GetDefaultValue();
                if (value == DBNull.Value) value = null;

                // 2) HasDefaultValueSql("...") -> solo se è un letterale semplice
                if (value == null)
                {
                    var sql = p.GetDefaultValueSql();
                    if (string.IsNullOrWhiteSpace(sql)) continue;
                    if (!TryParseSqlLiteral(sql, pi.PropertyType, out value)) continue; // GETDATE(), NEWID()...
                }

                if (value == null) continue;

                var propType = pi.PropertyType;

                // CORREZIONE STRUTTURALE DELLA NULLABILITÀ:
                // Se la proprietà C# accetta il NULL (stringhe o tipi Nullable come short?), il ClrDefault deve essere null.
                // Se è un tipo valore puro (short, int, decimal), il ClrDefault deve essere lo zero nativo (0).
                object clrDefault = null;
                if (propType.IsValueType && Nullable.GetUnderlyingType(propType) == null)
                {
                    clrDefault = Activator.CreateInstance(propType);
                }

                // COERCIZIONE DEL TIPO (Cast protetto per evitare eccezioni di boxing):
                // Ci assicuriamo che il valore convertito da TryParseSqlLiteral corrisponda ESATTAMENTE
                // al tipo CLR della proprietà (es. forza l'oggetto a essere uno 'short' anziché un 'int' o 'decimal')
                object dbDefaultConvertito = value;
                var tipoSottostante = Nullable.GetUnderlyingType(propType) ?? propType;

                if (dbDefaultConvertito.GetType() != tipoSottostante)
                {
                    try
                    {
                        dbDefaultConvertito = Convert.ChangeType(dbDefaultConvertito, tipoSottostante, CultureInfo.InvariantCulture);
                    }
                    catch { continue; } // Salta se il cast fallisce per disallineamento grave
                }

                result.Add(new Slot { Property = pi, ClrDefault = clrDefault, DbDefault = dbDefaultConvertito });
            }

            return result.ToArray();
        }

        /// <summary>
        /// Converte i default SQL "letterali" generati dallo scaffolding:
        /// ((0.00)) | ('0') | ('') | ('17991231') | ((3538947))
        /// Le espressioni (GETDATE(), NEWID(), ...) restituiscono false e vengono ignorate.
        /// </summary>
        private static bool TryParseSqlLiteral(string sql, Type propertyType, out object result)
        {
            result = null;
            var s = sql.Trim();

            // toglie le parentesi esterne: ((0.00)) -> 0.00 ; ('0') -> '0'
            while (s.Length >= 2 && s[0] == '(' && s[s.Length - 1] == ')')
                s = s.Substring(1, s.Length - 2).Trim();

            if (s.Length == 0) return false;

            if (s.Length >= 3 && (s[0] == 'N' || s[0] == 'n') && s[1] == '\'')
                s = s.Substring(1); // N'abc' -> 'abc'

            if (s.Length >= 2 && s[0] == '\'' && s[s.Length - 1] == '\'')
            {
                s = s.Substring(1, s.Length - 2).Replace("''", "'");
            }
            else if (s.IndexOf('(') >= 0 || s.IndexOf(' ') >= 0)
            {
                return false; // funzione o espressione, non un letterale
            }

            var t = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

            try
            {
                if (t == typeof(string)) { result = s; return true; }

                if (t == typeof(char))
                {
                    if (s.Length != 1) return false;
                    result = s[0];
                    return true;
                }

                if (t == typeof(bool))
                {
                    result = s == "1" || s.Equals("true", StringComparison.OrdinalIgnoreCase);
                    return true;
                }

                if (t == typeof(Guid))
                {
                    if (!Guid.TryParse(s, out var g)) return false;
                    result = g;
                    return true;
                }

                if (t == typeof(DateTime))
                {
                    if (!DateTime.TryParseExact(s, DateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                        return false;
                    result = dt;
                    return true;
                }

                if (t.IsEnum)
                {
                    result = Enum.ToObject(t, Convert.ChangeType(s, Enum.GetUnderlyingType(t), CultureInfo.InvariantCulture));
                    return true;
                }

                if (t.IsPrimitive || t == typeof(decimal))
                {
                    result = Convert.ChangeType(s, t, CultureInfo.InvariantCulture);
                    return true;
                }
            }
            catch (FormatException) { }
            catch (OverflowException) { }
            catch (InvalidCastException) { }

            return false;
        }
    }
}
