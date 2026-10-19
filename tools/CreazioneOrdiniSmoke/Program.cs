using EFMago.Models;
using FatturazioneContratti.Common;
using FatturazioneContratti.Data;
using FatturazioneContratti.Orders;
using Microsoft.EntityFrameworkCore;

// ---------------------------------------------------------------------------
// Harness di verifica del CreazioneOrdiniService. NON usare mai su produzione:
// passare esplicitamente il database di test (es. TMPSPA).
//
// Uso:
//   dotnet run --project tools/CreazioneOrdiniSmoke -- <server> <db> <user> <pwd> [loginId]
// Esempio:
//   dotnet run --project tools/CreazioneOrdiniSmoke -- "ACERBO\SQLEXPRESS" TMPSPA sa ******** 1
// ---------------------------------------------------------------------------

if (args.Length < 4)
{
    Console.WriteLine("Uso: dotnet run --project tools/CreazioneOrdiniSmoke -- <server> <database> <user> <password> [loginId]");
    Console.WriteLine("Esempio: dotnet run --project tools/CreazioneOrdiniSmoke -- \"ACERBO\\SQLEXPRESS\" TMPSPA sa ******** 1");
    return 1;
}

string server = args[0];
string database = args[1];
string user = args[2];
string password = args[3];
int loginId = args.Length > 4 && int.TryParse(args[4], out int lid) ? lid : 1;

if (!string.Equals(database, "TMPSPA", StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine($"ATTENZIONE: il database indicato è '{database}', non TMPSPA. Continuo comunque (responsabilità dell'operatore).");
}

SqlTools.Configure(new EfmagoOptions
{
    SqlServer = server,
    UserId = user,
    Password = password,
    DatabaseInUse = database,
    LoginId = loginId,
    IsDebugging = true
});

using FattureContext context = SqlTools.CreateContext();

// Sceglie il primo cliente disponibile in anagrafica (tipologia cliente Mago = 3211264).
MaCustSupp? cliente = context.MaCustSupp.AsNoTracking()
    .Where(c => c.CustSuppType == 3211264)
    .OrderBy(c => c.CustSupp)
    .FirstOrDefault();
if (cliente == null)
{
    Console.WriteLine("Nessun cliente in anagrafica (MA_CustSupp tipo 3211264): impossibile testare.");
    return 2;
}

var request = new CreazioneOrdineRequest(
    Cliente: cliente.CustSupp,
    DataOrdine: DateTime.Today,
    Righe:
    [
        new RigaOrdineCreazione(Descrizione: "Canone mensile di prova (smoke test)", Quantita: 12, ValoreUnitario: 100, UoM: "MESE", TipoRiga: LineType.Servizio),
        new RigaOrdineCreazione(Descrizione: "Fornitura merce di prova (smoke test)", Quantita: 2, ValoreUnitario: 55.50, TipoRiga: LineType.Merce, CodIva: "22"),
        new RigaOrdineCreazione(Descrizione: "Riga di sola nota tecnica", Quantita: 0, ValoreUnitario: 0, TipoRiga: LineType.Nota)
    ],
    Note: "Ordine di prova generato dallo smoke test di CreazioneOrdiniService");

Console.WriteLine($"Cliente di test: {cliente.CustSupp} - {cliente.CompanyName}");

var service = new CreazioneOrdiniService(context, loginId);
EsitoCreazioneOrdine? esito = service.CreaOrdine(request, (msg, target) => Console.WriteLine($"  [{target}] {msg}"));

if (esito == null)
{
    Console.WriteLine("CREAZIONE FALLITA (vedi log qui sopra).");
    return 3;
}

Console.WriteLine();
Console.WriteLine($"CREATO: SaleOrdId={esito.SaleOrdId} InternalOrdNo={esito.InternalOrdNo} Righe={esito.NumeroRighe}");

// ---------------------------------------------------------------- verifica post-insserimento
MaSaleOrd ordine = context.MaSaleOrd.AsNoTracking().First(o => o.SaleOrdId == esito.SaleOrdId);
Console.WriteLine();
Console.WriteLine("--- MA_SaleOrd ---");
Console.WriteLine($"Customer={ordine.Customer} InvoicingCustomer={ordine.InvoicingCustomer} OrderDate={ordine.OrderDate:yyyy-MM-dd} " +
                  $"Payment={ordine.Payment} PriceList={ordine.PriceList} Currency={ordine.Currency} Language={ordine.Language} " +
                  $"AccTpl={ordine.AccTpl} InvRsn={ordine.InvRsn} TaxJournal={ordine.TaxJournal} OurReference={ordine.OurReference} " +
                  $"Presentation={ordine.Presentation} Divisione={ordine.Divisione} SaleType={ordine.SaleType} OpenOrder={ordine.OpenOrder} " +
                  $"InternalOrdNo={ordine.InternalOrdNo}");

Console.WriteLine("--- MA_SaleOrdDetails ---");
foreach (MaSaleOrdDetails d in context.MaSaleOrdDetails.AsNoTracking()
    .Where(d => d.SaleOrdId == esito.SaleOrdId)
    .OrderBy(d => d.Line))
{
    Console.WriteLine($"Line={d.Line} Position={d.Position} SubId={d.SubId} Type={d.LineType} Item={d.Item} UoM={d.UoM} " +
                      $"Qty={d.Qty} UnitValue={d.UnitValue} TaxCode={d.TaxCode} Taxable={d.TaxableAmount} Total={d.TotalAmount} " +
                      $"InternalOrdNo={d.InternalOrdNo} Notes={d.Notes}");
}

Console.WriteLine("--- MA_SaleOrdSummary / Shipping / Notes ---");
Console.WriteLine($"Summary={context.MaSaleOrdSummary.AsNoTracking().Any(s => s.SaleOrdId == esito.SaleOrdId)} " +
                  $"Shipping={context.MaSaleOrdShipping.AsNoTracking().Any(s => s.SaleOrdId == esito.SaleOrdId)} " +
                  $"Notes={context.MaSaleOrdNotes.AsNoTracking().Any(n => n.SaleOrdId == esito.SaleOrdId)}");

return 0;
