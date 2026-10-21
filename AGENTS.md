# AGENTS.md — FatturazioneContratti

## Panoramica del Progetto
- **Tipo**: App desktop WinForms (.NET 8, solo Windows)
- **Scopo**: Automazione fatturazione contratti di servizio (canoni, adeguamenti ISTAT, fatture a consuntivo) su database Mago.net ERP (SQL Server)
- **Entry point**: `Program.cs` → form `FLogin` (orchestratore UI)
- **Workflow principali** (attivati da menu/pulsanti di FLogin):
  1. **Genera Canoni da Ordini** — costruisce righe d'ordine dagli schedulati dei contratti
  2. **Adeguamento ISTAT** — applica indicizzazione ISTAT alle righe contratto
  3. **Consuntivazione Flusso Integra** — valida interventi Integra → genera fatture a consuntivo
  4. **Allineamento Canoni Fatture** — ripopola date canoni su fatture esistenti (SQL raw)

## Build / Esecuzione
```bash
dotnet build FatturazioneContratti.sln   # 2 progetti: libreria EFMago + WinForms
dotnet run --project FatturazioneContratti.csproj
```
- Richiede istanza SQL Server con schema Mago.net
- Configurazione in `%APPDATA%\Brovarone Cristiano\FatturazioneContatti-settings.json` (gestita da `AppConfig`)

## Configurazione (AppConfig.cs)
Tutte le impostazioni runtime in `AppConfig` (classe statica, JSON-backed):
- **Connessione**: `SqlServer`, `UserId` (default `sa`), `Password`, `LoginId`
- **Database**: `DatabaseUno` / `DatabaseSpa` (produzione), `TmpDatabaseUno` / `TmpDatabaseSpa` (test)
- **DB attivo**: proprietà `DatabaseInUse` commutata da flag `IsDbUno` + `IsTestDb`
- **Percorsi**: `Path`, `FoxPath`, `PathPaghe`, `LastLogPath`
- **Debug**: `IsDebugging` abilita EF Core detailed errors + sensitive data logging

**Primo avvio**: "Configurazione Avanzata" (password `123456`) → compila connessione + nomi DB → Test → Salva.

**Disaccoppiamento dalla libreria**: `AppConfig.Load()`/`Save()` chiamano `SyncToLibrary()`, che registra su `SqlTools` una fabbrica di `EfmagoOptions` (valori riletti a ogni uso: le modifiche runtime, es. Test Connessione o cambio DB test, si riflettono subito). La libreria EFMago **non referenzia `Config/`** — un futuro sito leggerà `appsettings.json` nella stessa `EfmagoOptions`.

## Architettura e Componenti Chiave

### Libreria `EFMago` (`EFMago/`, progetto `EFMago.csproj`, `net8.0`)
Estratta dal WinForms per essere condivisa col futuro sito Blazor (scissione del 16/10/2026):
- Contiene `Data/`, `Common/` (tranne `CrashLogger.cs`), `Orders/`, `Invoicing/` — logica di dominio **senza alcuna dipendenza WinForms**
- `SqlTools.Configure(EfmagoOptions)` + `SqlTools.Options` — sorgente unica di connessione/debug/log-mode per la libreria
- `Common/LogTarget.cs` — enum `LogTarget` (Ui/File/Both), spostato qui da `AppConfig.LogTarget`
- Il WinForms la referenzia via `ProjectReference`; il sito la referenzierà dal suo repo
- Namespace invariati (`FatturazioneContratti.*`) per non toccare i consumatori

### Data Layer (`EFMago/Data/`)
- `FattureContext` — **DbContext manuale** (non generato). Contiene `OnModelCreating` con 100+ entity configurations, relazioni custom, `HasDefaultValueSql` per default Mago. **Modificare direttamente qui** quando serve.
- `Data/Models/` — **Generati** da `copia.ps1` (copia 100+ classi entità da `ALLMigrazione\EFMago\Models\`). Non modificare a mano.
- `SqlTools.CreateContext()` — factory; rispetta `EfmagoOptions` + flag `IsDebugging` (registrate dall'ospite via `Configure`).
- `SqlTools.OpenDirectConnection()` — `SqlConnection` raw con `ARITHABORT ON` (richiesto per indici filtrati Mago).

### Modelli di Dominio (`EFMago/Common/`, `Orders/`, `Invoicing/`)
| File | Scopo |
|------|-------|
| `DomainConstants.cs` | Sentinella `DataNulla = 1799-12-31`, costanti precisione decimali |
| `OrdiniEnums.cs` | Valori enum Mago (TipologiaServizio, Periodo, Cadenza, Divisione) — **valori numerici sono FK, mai cambiare** |
| `CurOrd` / `CurOrdRow` | Modelli di calcolo mutabili (portati 1:1 da VB `CurOrd.vb` / `CurOrdRow.vb`) |
| `CurOrdRowCalculations.cs` | Logica pura date/quantità (diff mesi, sospensioni, cessazioni, finestre ISTAT) |
| `FiltriOrdini` / `FiltriOrdiniConsuntivo` | DTO filtri bindati alle form UI |
| `EntityCloner.CloneFromPrototype` | Shallow copy veloce via reflection; clona entità "prototipo" EF pre-riempite con default Fluent API |
| `FattureContextExtensions.AddNew<T>()` | Crea istanza entità con tutti i `HasDefaultValueSql` applicati (replica VB `DataSet.AddNew`). Inizializzatori cachati **per `IModel`**, non per tipo |
| `StringHelpers.SubstringSafe` | Troncamento sicuro (trim + max len), mai null — usato dai servizi su campi nota |
| `MagoEnum.cs` | `LineType`, `DocumentType`, `CrossReference` — valori FK Mago. Usare gli enum, mai numeri hardcoded |
| `CrashLogger.cs` | Log crash globale (`crash_yyyyMMdd_HHmmss_fff.log`), chiamato dai handler in `Program.cs` |
| `BulkPersistenceHelper` | Wrapper `EFCore.BulkExtensions`; esegue `BulkInsert`/`BulkUpdate`/`BulkInsertOrUpdate` in transazione con `DBCC TRACEON(610)` per minimal logging |

### Servizi (`EFMago/Orders/`, `Invoicing/`)
- **GenerazioneRigheOrdiniService** (partial, file `.cs` + `.Istat.cs`) — `GeneraRigheOrdine(GeneraRigheOrdineOptions)`, `AdeguaIstatOrdine()`
  - Firma a record unico: `GeneraRigheOrdineOptions` raggruppa filtri, `DimensioneBlocco` e callback (log, stato, 2 progress)
  - Corpo scomposto: `ApplicaFiltriComuni` / `EstraiIdsOrdini` (condivisi con ISTAT) + `CaricaDefaultVenditeIva`
  - Stato mutabile in `StatoElaborazioneOrdini` (collettori + accumulatori log) e `StatoOrdine` (contatori per singolo ordine); elaborazione in `ElaboraOrdine` / `ElaboraContratto`
  - Ciclo a blocchi come **iterator locale** `CaricaOrdiniPerBlocco` (stesso pattern del workflow ISTAT)
  - Carica prima solo ID ordini, poi processa a blocchi (default 100) con `AsSplitQuery()` + predicato `OR`
  - **Modalità debug** (`AppConfig.IsDebugging`): forza scan atomico per-order per far emergere `SqlNullValueException` su righe corrotte
  - Accumula cambiamenti in memoria → singola transazione bulk finale
- **ConsuntivoService** — `ControllaFlussoIntegra()` (valida interventi), `GeneraFattureConsuntivo()` (bulk-insert fatture)
  - Usa mapping hardcoded: `FilialiMapping`, `MappingsEventoIntegra`, `CausaliPagamentoFatturaElettronica`
  - Logica raggruppamento fatture in `Break()` — rispetta flag `MaSalesOrdParameters` fulfillment break
- **AggiornamentoFattureService** — classe `static`, `UPDATE` SQL raw join `MA_SaleDocDetail` → `MA_SaleOrdDetails`, con `SqlConnection`/`SqlTransaction` ricevuti dal caller
  - Resta raw di proposito (l'`UPDATE ... FROM` su 3 tabelle è scomodo in EF), ma i valori sono presi dagli enum `MagoEnum` e il timestamp è unico per operazione
  - Unico residuo raw: i **nomi di tabella/colonna**, non tipizzati dal compilatore
- **CreazioneOrdiniService** (`Orders/CreazioneOrdiniService.cs` + `Orders/CreazioneOrdineRequest.cs`) — `CreaOrdine(CreazioneOrdineRequest, logMessage)` → `EsitoCreazioneOrdine(SaleOrdId, InternalOrdNo, NumeroRighe)` o `null` su errore (dettaglio già loggato)
  - Uso previsto: **sito web Blazor di sola scrittura ordini** (repo separato); i 4 workflow WinForms restano dove sono
  - Crea da zero: testata `MaSaleOrd` + Summary/Shipping/Notes (tabelle 1:1, sempre presenti nei dati reali) + righe `MaSaleOrdDetails`
  - Convenzioni replicate dai dati reali (ispezione 732 ordini, 10/2026): CustSuppType 3211264, AccTpl/InvRsn da `MaUserDefaultSaleOrders`, Currency EUR fallback, Presentation dall'anagrafica, TaxJournal "VEN", i 5 flag `*Auto` senza default Fluent valorizzati come Mago
  - **Contatori atomici**: `SaleOrdId` da `MA_IDNumbers` e numero `yy/NNNNN` da `MA_NonFiscalNumbers` allocati con `UPDATE ... OUTPUT` dentro `ExecuteInBulkTransaction` (sicuro per utenti multipli del sito); la riga dell'anno fiscale nuovo viene creata clonando la più recente come template
  - Testata/righe costruite su prototipi `AddNew<T>()` + `CloneFromPrototype()`; validazione input con lunghezze colonne reali (fallisce con messaggio di business, non con SqlException da troncamento)
  - **Harness di verifica**: `tools/CreazioneOrdiniSmoke` (console net8.0) — `dotnet run --project tools/CreazioneOrdiniSmoke -- <server> <db> <user> <pwd> [loginId]`; usa SOLO DB di test (es. TMPSPA), crea un ordine di prova e stampa i record scritti

### UI (`Forms/`)
- `FLogin` — form principale, orchestra i 4 workflow via `Task.Run` background
- `FSettings` — dialog configurazione (password-protetta)
- `FAskFiltriOrdini` / `FAskFiltriOrdiniConsuntivo` — dialog input filtri
- Progress reporting via `UiProgress` + delegati `ProgressCallbacks`

## Comandi Sviluppatore

### Aggiorna Modelli Entità (quando cambia schema Mago / migrazione ERP)
```powershell
# Da root repo; copia SOLO i model (classi entità) da ALLMigrazione
# Destinazione: EFMago/Data\ ( Models\ + FattureContext.cs )
.\copia.ps1
```
Poi rebuild. **Il `FattureContext.cs` NON viene toccato** — aggiornare manualmente `OnModelCreating` se servono nuove relazioni/colonne.

> **NOTA FUTURA**: `copia.ps1` è il meccanismo attuale ma **probabilmente verrà sostituito** da un tool/app separata (non in questo repo) per gestire scaffold + sync modelli + context in modo più robusto. Fino ad allora, usa lo script.

### Checklist post-migrazione ERP (DA FARE)
Quando il programma ERP Mago viene migrato/aggiornato (nuova versione, nuovi campi, nuove tabelle):
1. **Scaffold entità nuove/modificate** nel progetto `ALLMigrazione\EFMago` → `dotnet ef dbcontext scaffold ...`
2. **Esegui `copia.ps1`** (meccanismo transitorio) per riversare i model aggiornati in `Data/Models/`
3. **Aggiorna `FattureContext.cs` manualmente**:
   - Nuovi `DbSet<T>` per tabelle inedite
   - Nuove proprietà di navigazione / chiavi esterne in `OnModelCreating`
   - `HasDefaultValueSql` per campi con default Mago (es. `((0))`, `('17991231')`, `((1229717504))`)
   - Relazioni custom (es. `AllordCliAttivita` → `Allattivita`, `AllordCliContrattoDistinta` → `AllordCliContrattoDistintaServAgg`)
   - Commenti `"AGGIUNTO DA ME"` per distinguere le tue aggiunte dallo scaffold
4. **Verifica `AddNew<T>()`** funziona sulle nuove entità (legge default da Fluent API automaticamente)
5. **Testa workflow colpiti**: Generazione Canoni, ISTAT, Consuntivi
6. **Aggiorna `OrdiniEnums.cs`** se compaiono nuovi valori enum Mago (valori numerici = FK!)

### Esegui Singolo Workflow (per test)
Nessun test automatico. Verifica manuale via UI:
1. Avvia app → Configurazione Avanzata → imposta flag Test DB → Salva
2. Clicca pulsante workflow → compila dialog filtri → osserva progress/log

### Debug Crash Null-Reference in Query EF
Imposta `AppConfig.IsDebugging = true` (da Settings dialog o JSON). Abilita:
- `EnableDetailedErrors()` → rivela **nome colonna** contenente NULL
- `EnableSensitiveDataLogging()` → mostra valori parametri
- `GenerazioneRigheOrdiniService` passa auto a scan diagnostico per-order su `SqlNullValueException`

## Convenzioni Critiche & Trappole

1. **Valori enum sono FK Mago** — `TipologiaServizio.Canone = 1276116992`, ecc. Non rinumerare.
2. **Sentinella data** — `DomainConstants.DataNulla (1799-12-31)` usata ovunque invece di `null`. Confrontare con `== DataNulla` o `!= DataNulla`.
3. **Operazioni bulk** — Tutti insert/update passano per `BulkPersistenceHelper.ExecuteInBulkTransaction`. Mai `SaveChanges()` per lavoro bulk; solo per update singole entità (es. contatore `MaIdnumbers.LastId`).
4. **Connection string** — Costruita a runtime da `SqlTools` su `EfmagoOptions` (nel WinForms i valori provengono da `AppConfig` via `SyncToLibrary()`); niente `appsettings.json`. `TrustServerCertificate=true` per dev locale.
5. **ARITHABORT ON** — Obbligatorio per qualsiasi `SqlConnection` raw (indici filtrati Mago lo richiedono). `SqlTools.OpenDirectConnection()` lo gestisce.
6. **Pattern Prototype** — `context.AddNew<T>()` restituisce entità con tutti i default Fluent API materializzati. Clonarla via `.CloneFromPrototype()` per ogni nuova riga. Evita overhead change-tracker EF.
7. **Caricamento a chunk** — `GenerazioneRigheOrdiniService.LoadOrdersChunkByOrIn` costruisce `Expression<Func<MaSaleOrd,bool>>` predicato OR per batch ID. Evita esplosione cartesiana da `Include` su set grandi.
8. **Niente DI / IoC** — Servizi istanziati manualmente in `FLogin` con `SqlTools.CreateContext()` + `AppConfig.LoginId`.
9. **Commenti/logica in italiano** — Commenti business logic in italiano; preservarli modificando codice calcolo.
10. **Timestamp unico per operazione** — Non chiamare `DateTime.Now` a ogni riga: i servizi di istanza lo catturano in un campo `_now` (inizializzato nel costruttore), quelli `static` in una locale a inizio metodo. Mai `static readonly`: in un'app WinForms long-lived congelerebbe il valore al primo avvio.
11. **Valori Mago dagli enum** — Per `LineType` / `DocumentType` / `CrossReference` usare `Common/MagoEnum.cs`, mai numeri hardcoded (neppure nelle query raw).
12. **Callback inutilizzati vanno rimossi** — Le firme pubbliche espongono solo callback realmente usati: se ne resta uno morto (es. il vecchio `progress3`), eliminarlo invece di mantenerlo a `null`.
13. **Stato di elaborazione esplicito** — Quando un metodo supera ~150 righe, raggruppa collettori/accumulatori in una classe di stato (`StatoElaborazioneOrdini`) invece di moltiplicare parametri `out`/`ref`.
14. **`DataProssimaFatt`: una sola colonna viva su tre** — esiste su `ALLOrdCliContratto` (viva: unica letta/scritta dai canoni, `CurOrdRow(AllordCliContratto)` + `ElaboraContratto`), su `ALLOrdCliContratto_Distinta` e su `ALLOrdCliContratto_Distinta_ServAgg` (deprecate: popolate solo dall'import Fox `ContrattiFox.vb`, mai mantenute, quindi ferme al valore d'import — verificate 21/10/2026: 272 distinte e 535 servagg già divergenti). Non eliminarle: potrebbero servire in futuro (consuntivo per servizio, sito). Se le si adotta, riallinearle e aggiornare questa nota.
15. **`TipoRigaServizio` ridondante sulle righe figlie** — il tipo esiste su contratto, distinta e servaggi aggiuntivi, ma è **decisionale solo sul contratto** (`CurOrdRow`/`ElaboraContratto`: tipologia, periodicità, cadenza, campagna; ISTAT idem). Su distinta e servagg è ridondante e non letto da alcun flusso: il consuntivo usa `Periodicita`/`Franchigia`/`Servizio` **propri della riga servagg**, non il suo FK tipo. Le differenze osservate su ALLSPA (21/10/2026: 54 su 1.268 confronti contratto↔distinta) sono tutte campagna `ESTANT` sotto canone o canone sotto campagna, e in 40/40 casi l'ordine ha già la riga contratto `ESTANT` dedicata: **non sono errori e non vanno allineate**. Regola: non leggere il tipo delle righe figlie per decisioni di fatturazione; se un giorno servirà, definire prima quale livello possiede il tipo.

## Mappa Proprietà File
| Directory | Proprietario | Note |
|-----------|--------------|------|
| `EFMago/Data/Models/` | **Generati** — modifica solo via `copia.ps1` |
| `EFMago/Data/FattureContext.cs` | **Manuale** — DbContext custom, `OnModelCreating` mantenuto a mano |
| `EFMago/Common/` | Primitive dominio condivise (enum, costanti, estensioni) — libreria |
| `EFMago/Orders/` | Logica generazione canoni/ISTAT — libreria |
| `EFMago/Invoicing/` | Logica consuntivo + allineamento fatture — libreria |
| `Common/CrashLogger.cs` | **Resta nel WinForms** (referenzia WinForms + AppConfig) |
| `Forms/` | Solo UI WinForms; niente business logic |
| `Config/` | `AppConfig` — unica fonte verità per settings del WinForms (poi `SyncToLibrary()`) |
| `MigrazioneVb_Csharp.txt` | Log migrazione — solo riferimento |

## Task Comuni

### Aggiungere nuova tabella Mago
1. Aggiungi a progetto EF `ALLMigrazione` → scaffold → esegui `copia.ps1` (copia il model in `EFMago/Data/Models/`)
2. In `FattureContext.cs`: aggiungi `DbSet<T>` + configurazione in `OnModelCreating` (chiavi, relazioni, `HasDefaultValueSql`)
3. Se serve `AddNew<T>()` support: nessuna modifica codice (estensione legge default Fluent API automaticamente)

### Aggiungere nuovo workflow
1. Crea servizio in `Orders/` o `Invoicing/`
2. Aggiungi pulsante + handler in `FLogin` seguendo pattern esistente (`EseguiOperazione` + `ProgressCallbacks`)
3. Aggiungi form filtri se serve (pattern `FAskFiltriOrdini`)

### Modificare logica date/quantità
- **Toccare solo** `CurOrdRowCalculations.cs` o costruttori `CurOrdRow`
- Preservare commenti storici (es. "28/02/2022", "23/03/2022") — documentano decisioni edge-case
- Testare sia `Cadenza.Anticipato` (1° del mese) che `Posticipato` (fine mese)

### Cambiare dimensione batch bulk
- Parametro `DimensioneBlocco` in `GeneraRigheOrdineOptions` (default 100)
- `BulkPersistenceHelper.BulkConfig.BatchSize` auto-scala: 0 (<5000 righe) o `count/10`

## Limitazioni Note
- Nessun test automatico; tutta verifica manuale via UI
- Costruttore `CurOrdRow(AllordCliContrattoDistintaServAgg)` marcato "per ora lavora male non usare" — non usato in flussi produzione
- Parsing `IsCampagna` via `VbStyleToBool` su campo stringa — verificare valori reali dati ("-1"/"0", "S"/"N", ecc.)
- Password in `AppConfig` in plaintext JSON (solo profilo utente locale)
- Design single-user; nessuna gestione concorrenza su contatore `MaIdnumbers.LastId` oltre transazione bulk

## Riferimenti
- `MigrazioneVb_Csharp.txt` — checklist riassuntiva migrazione
- `copia.ps1` — script copia model (solo entità, NON DbContext)
- `TODO.md` — promemoria sessione di revisione, fix già applicate e verifica manuale da fare
