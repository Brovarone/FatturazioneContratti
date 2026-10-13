using System;

namespace FatturazioneContratti.Common
{
    /// <summary>
    /// Porting delle costanti sparse in Ordini.vb (decPerc/decValUnit/decTax) e in
    /// Global.vb (sDataNulla). sDataNulla nel VB è una stringa ottenuta da un DateTime
    /// per un vecchio confronto "loose" con Option Strict Off: qui è semplicemente
    /// un DateTime, confrontabile direttamente.
    /// </summary>
    public static class DomainConstants
    {
        public const int DecimaliPercentuale = 2;   // ex decPerc
        public const int DecimaliValoreUnitario = 5; // ex decValUnit
        public const int DecimaliTasse = 2;          // ex decTax

        /// <summary>Sentinella "nessuna data" usata in tutto il dominio ordini (ex sDataNulla).</summary>
        public static readonly DateTime DataNulla = new DateTime(1799, 12, 31);
    }
}
