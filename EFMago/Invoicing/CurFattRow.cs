using System;
using EFMago.Models;
using FatturazioneContratti.Common;

namespace FatturazioneContratti.Invoicing
{
    public class CurFattRow
    {
        public int SaleDocId { get; set; }
        public bool IsOk { get; set; } = true;
        public short Line { get; set; }
        public string Contropartita { get; set; } = string.Empty;
        public string CodIva { get; set; } = string.Empty;
        public double? PercIva { get; set; }
        public string Commessa { get; set; } = string.Empty;
        public string CdC { get; set; } = string.Empty;
        /// <summary>Testata di fattura a cui la riga appartiene (back-reference), assegnata alla costruzione.</summary>
        public required CurFatt Parent { get; set; }
        public string ContrattoFox { get; set; } = string.Empty;
        public string CodiceIntegra { get; set; } = string.Empty;
        public double NrCanoni { get; set; }
        public DateTime CanoniDataIn { get; set; }
        public DateTime CanoniDataFin { get; set; }
        public string Impianto { get; set; } = string.Empty;

        public CurFattRow()
        {
            // SaleDocId/Line/NrCanoni restano ai default CLR (0); PercIva è double?
            // e lo 0 è un valore intenzionale, non il default null.
            DateTime d = DateTime.Today;
            CanoniDataIn = d;
            CanoniDataFin = d;
            PercIva = 0;
        }

        public CurFattRow(AllordCliContratto c) : this()
        {
            DateTime d = DomainConstants.DataNulla;
            Line = c.Line;
            NrCanoni = c.Qta ?? 0;
            CanoniDataIn = d;
            CanoniDataFin = d;
            ContrattoFox = c.CodContratto ?? string.Empty;
            CodiceIntegra = c.CodIntegra ?? string.Empty;
        }

        public CurFattRow(AllordCliContrattoDistintaServAgg s) : this()
        {
            DateTime d = DomainConstants.DataNulla;
            NrCanoni = s.Qta ?? 0;
            CanoniDataIn = d;
            CanoniDataFin = d;
        }
    }
}