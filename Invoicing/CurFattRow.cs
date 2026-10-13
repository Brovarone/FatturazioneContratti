using System;
using EFMago.Models;
using FatturazioneContratti.Common;

namespace FatturazioneContratti.Invoicing
{
    public class CurFattRow
    {
        public int SaleDocId { get; set; }
        public bool IsOk { get; set; }
        public short Line { get; set; }
        public string Contropartita { get; set; } = string.Empty;
        public string CodIva { get; set; } = string.Empty;
        public double? PercIva { get; set; }
        public string Commessa { get; set; } = string.Empty;
        public string CdC { get; set; } = string.Empty;
        public CurFatt Parent { get; set; }
        public string ContrattoFox { get; set; } = string.Empty;
        public string CodiceIntegra { get; set; } = string.Empty;
        public double NrCanoni { get; set; }
        public DateTime CanoniDataIn { get; set; }
        public DateTime CanoniDataFin { get; set; }
        public string Impianto { get; set; } = string.Empty;

        public CurFattRow()
        {
            DateTime d = DateTime.Today;
            SaleDocId = 0;
            IsOk = true;
            Line = 0;
            NrCanoni = 0;
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
            Line = 0;
            NrCanoni = s.Qta ?? 0;
            CanoniDataIn = d;
            CanoniDataFin = d;
        }
    }
}