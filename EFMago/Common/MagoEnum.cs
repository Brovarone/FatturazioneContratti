using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EFMago.Common
{          
    /// <summary>
    /// Tipo riga documento Mago.NET (MA_SaleOrdDetails, MA_SaleDocDetail).
    /// I valori numerici corrispondono ai codici anagrafici di Mago.
    /// </summary>
    public enum LineType : int
    {
        Nota = 3538944,
        Riferimento = 3538945,
        Servizio = 3538946,
        Merce = 3538947,
        Descrittiva = 3538948
    }

    /// <summary>
    /// Tipo documento di vendita/acquisto Mago.NET.
    /// </summary>
    public enum DocumentType : int
    {
        PickingList = 3407872,
        DDT = 3407873,
        Fattura = 3407874,
        FatturaAccompagnatoria = 3407875,
        NotaCredito = 3407876,
        ResoAFornitore = 3407881,
        FatturaProForma = 3407888,
        NotaDebito = 3407889,
        AutoFattura = 3407898,
        AutoNotaCredito = 3407899,
        AcqBollaDiCarico = 9830400,
        AcqFattura = 9830401,
        AcqNotaCredito = 9830402
    }

    /// <summary>
    /// Riferimenti Incrociati Mago.NET (MA_CrossReferences).
    /// </summary>
    public enum CrossReference : int
    {
        Tutti = 27066368,
        OrdineCliente = 27066372,
        FatturaImmediata = 27066387,
        NotaDiCredito = 27066389,
        PnotaPuro = 27066418,
        PnotaEmesso = 27066419,
        MovimentoAnalitico = 27066424,
        MovimentoCespite = 27066428
    }

    /// <summary>
    /// Natura cliente/fornitore Mago.NET.
    /// </summary>
    public enum CustSuppKind : int
    {
        Nazionale = 7733248,
        CEE = 7733249,
        ExtraCEE = 7733250
    }

    /// <summary>
    /// Specie Archivio 58 Mago.NET.
    /// </summary>
    public enum IdType : int
    {
        DocVend = 3801088,
        MovMagazzino = 3801093,
        Partite = 3801094,
        PNota = 3801095,
        MovCespite = 3801097,
        OrdCli = 3801098,
        OrdFor = 3801100,
        MovAna = 3801102,
        DocAcq = 3801108,
        DicIntento = 3801122
    }

    /// <summary>
    /// Tipo Documento non fiscale 57 Mago.NET.
    /// </summary>
    public enum CodeType : int
    {         
        OrdCli = 3735554,
        PNota = 3735558,
        MovAna = 3735561,
        MovCes = 3735585    //Nr. Riferimento Movimenti Cespiti
    }

    /// <summary>
    /// Costanti Mago.NET che non sono enum ma devono essere condivise tra i servizi.
    /// </summary>
    public static class MagoConstants
    {
        /// <summary>
        /// Causali pagamento Mago.NET (MA_EI_ITDocAdditionalData / tabelle causali).
        /// Mappa la lettera della causale al relativo ID Mago per fatture elettroniche.
        /// </summary>
        public static readonly Dictionary<string, int> CausaliPagamento = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) 
        {
            { "", 32440347 }, 
            { "A", 32440320 }, 
            { "B", 32440321 }, 
            { "C", 32440322 }, 
            { "D", 32440323 }, 
            { "E", 32440324 }, 
            { "G", 32440325 }, 
            { "H", 32440326 }, 
            { "I", 32440327 }, 
            { "L", 32440328 }, 
            { "M", 32440329 }, 
            { "N", 32440330 }, 
            { "O", 32440331 }, 
            { "P", 32440332 }, 
            { "Q", 32440333 }, 
            { "R", 32440334 }, 
            { "S", 32440335 }, 
            { "T", 32440336 }, 
            { "U", 32440337 }, 
            { "V", 32440338 }, 
            { "W", 32440339 }, 
            { "X", 32440340 }, 
            { "Y", 32440341 }, 
            { "Z", 32440342 }, 
            { "L1", 32440343 }, 
            { "M1", 32440344 }, 
            { "O1", 32440345 }, 
            { "V1", 32440346 }, 
            { "M2", 32440348 }, 
            { "ZO", 32440349 } 
        };
    }

}
