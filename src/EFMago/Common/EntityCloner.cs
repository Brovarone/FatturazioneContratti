using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace FatturazioneContratti.Common
{
    public static class EntityCloner
    {
        /// <summary>
        /// Cache delle property PER TIPO: la chiave è il Type di T, quindi ogni tabella
        /// clona sempre e solo con il proprio set di proprietà. Se due entità diverse
        /// hanno proprietà con lo stesso nome, i valori non finiscono mai nella tabella
        /// sbagliata: il lookup è tipizzato, non condiviso tra tipi.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> CachePropertiesPerTipo =
            new ConcurrentDictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Esegue una Shallow Copy ultra-veloce di tutte le proprietà scrivibili
        /// da un oggetto prototipo a una nuova istanza dello stesso tipo.
        /// </summary>
        public static T CloneFromPrototype<T>(this T prototipo) where T : class, new()
        {
            if (prototipo == null) return null;

            T nuovaIstanza = new T();

            // Il costo di GetProperties (reflection sui metadati) si paga una volta per tipo,
            // poi il ciclo copia solo i valori. Prima GetProperties girava a ogni riga clonata.
            PropertyInfo[] properties = CachePropertiesPerTipo.GetOrAdd(typeof(T), type =>
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance));

            foreach (PropertyInfo prop in properties)
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    object value = prop.GetValue(prototipo);
                    prop.SetValue(nuovaIstanza, value);
                }
            }

            return nuovaIstanza;
        }
    }
}
