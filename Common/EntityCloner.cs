using System;
using System.Reflection;

namespace FatturazioneContratti.Common
{
    public static class EntityCloner
    {
        /// <summary>
        /// Esegue una Shallow Copy ultra-veloce di tutte le proprietà scrivibili 
        /// da un oggetto prototipo a una nuova istanza dello stesso tipo.
        /// </summary>
        public static T CloneFromPrototype<T>(this T prototipo) where T : class, new()
        {
            if (prototipo == null) return null;

            T nuovaIstanza = new T();

            // Usiamo una micro-cache interna o un ciclo veloce sulle property della classe
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

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
