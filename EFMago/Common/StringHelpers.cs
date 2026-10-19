namespace FatturazioneContratti.Common
{
    /// <summary>
    /// Helper stringhe condiviso (ex duplicato: <c>SubstringSafe</c> privato in
    /// <c>GenerazioneRigheOrdiniService</c> + ternario inline in <c>ConsuntivoService</c>).
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Tronca a <paramref name="maxLen"/> caratteri dopo <c>Trim()</c>.
        /// Restituisce <see cref="string.Empty"/> se null/vuoto (mai null).
        /// </summary>
        public static string SubstringSafe(string? val, int maxLen)
        {
            if (string.IsNullOrEmpty(val)) return string.Empty;
            string t = val.Trim();
            return t.Length <= maxLen ? t : t.Substring(0, maxLen);
        }
    }
}
