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
- **Percorsi**: `Path`, `FoxPath`, `PathPaghe`, `LastLogPath` (cartella del log di sessione)
- **Log**: `LogMode` (`Ui` / `File` / `Both`, default `Both`) — instrada gli informativi; errori e avvisi vanno sempre su entrambi (vedi §Log e diagnostica)
- **Debug**: `IsDebugging` abilita EF Core detailed errors + sensitive data logging e il tracciato `debugging` su file

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
- `SqlTools.CommandTimeoutSeconds = 600` — timeout unico dei comandi: applicato in `CreateContext()` e da passare ai `SqlCommand` raw (il default ADO di 30s e i 180s EF andavano in timeout sulle query grosse).

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
| `LogDiagnostica.cs` | Routing log centralizzato: `Informativo` → `LogMode`, `Errore`/`Avviso` → `Both`, `LogFiltri` e statistiche bulk → `File`, `Blocco`, `Scarica(...)` chiamata nei `finally` dei workflow |
| `ILogFiltri.cs` | Contratto `CostruisciFiltriLog()` / `LogFiltri` implementato dai 3 DTO filtri |
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
  - Accumula cambiamenti in memoria → singola transazione bulk finale; `EseguiSalvataggioTransazionale` ritorna `bool` e `[COMPLETATO]` esce solo senza errori
  - `CancellationToken` controllato ai confini di ordine/contratto/blocco; se arriva con transazione aperta → rollback esplicito + throw
- **ConsuntivoService** — `ControllaFlussoIntegra()` (valida interventi), `GeneraFattureConsuntivo()` (bulk-insert fatture)
  - Usa mapping hardcoded: `FilialiMapping`, `MappingsEventoIntegra`, `CausaliPagamentoFatturaElettronica`
  - Logica raggruppamento fatture in `Break()` — rispetta flag `MaSalesOrdParameters` fulfillment break
  - **Regola cessazione**: si fattura se il contratto è attivo (`DataCessazione == DataNulla`) **oppure** se la cessazione è ≥ `Filtri.DataFattura` (ultimo mese utile). Non si fattura più solo per i contratti cessati (bug ereditato da `OrdiniConsuntivi.vb`). Se un contratto **è già cessato alla data fattura** ma ha interventi nel periodo: a video solo il riepilogo `[AVVISO] Trovati X interventi su Y contratti cessati`, dettaglio per ordine solo su file
  - `FiltriOrdiniConsuntivo.ProseguiAnomalie` ("Prosegui comunque"): prosegue con anomalie **non critiche** (contratti mancanti / eventi non associati); le anomalie critiche (duplicati) bloccano sempre
  - Performance: interventi/ordini con `AsSplitQuery()` (7 Include di collection → prodotto cartesiano, 67s → 11s), `MaCustSupp` caricati solo per gli `InvoicingCustomer` degli ordini estratti (36k righe/232s → 24 righe/3,8s), `progress2` a blocchi da 100 con `Finish` finale
  - **Riferimento fattura**: `Integra_Interventi.MChiaveFattura` = `id ordine|linea servizio|linea contratto|evento|mese` (es. `7553|1|2|INT|202609`), generata in `ControllaFlussoIntegra` quando l'intervento viene associato all'ordine e **ricalcolata da `Riassegna`** (il log avvisa quante chiavi sono cambiate). La chiave è **sempre ≤32 caratteri**: se il formato leggibile sfora (id ordine molto grande o servizio non mappato e lungo) `CostruisciChiaveFattura` passa a un formato compatto base36 (max 26). La stessa stringa va sulla riga fattura in `MA_SaleDocDetail.Packing`: campo **standard** Mago, quindi Mago la copia in entrambi i sensi fattura↔proforma. Nessun `SaleDocId` viene memorizzato sugli interventi (cambierebbe a ogni copia): la vista `ALLVInterventiFattura` risolve il documento corrente per chiave, preferendo Fattura (3407874) a Proforma (3407888). Script DDL + vista: `sql/2026-10-22_Consuntivo_ChiaveFattura.sql` (da eseguire su ogni DB prima dell'avvio).
  - **Note riga fattura**: `MA_SaleDocDetail.Notes` contiene il **codice contratto Integra** della distinta (`d.CodIntegra`, fallback `IntegraInterventi.Contratto` e poi `c.CodIntegra`), non la `Nota` di testa contratto. In VB `OrdiniConsuntivi.vb` riga 306 era `r.Notes = Trim(Left(c.Nota.ToString, 32))`: porting fedele ma senza attinenza con la riga fattura (e troncato a 32).
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
  - `EseguiOperazione(nome, Func<CancellationToken,bool>)`: una `CancellationTokenSource` per run, pulsante **Interrompi** (una sola richiesta), esito "INTERROTTO" arancio su `OperationCanceledException`
  - `lblStatus` resettata a inizio run; esito finale verde/rosso; LinkLabel "Apri file di log" (file → Notepad → cartella)
  - Righe di log colorate per tag `[ERRORE]`/`[AVVISO]`; righe lunghe vanno a capo
- `FSettings` — dialog configurazione (password-protetta), include `LogMode` (UI / File / Entrambi)
- `FAskFiltriOrdini` / `FAskFiltriOrdiniConsuntivo` / `FAskFiltriAllineamentoFatture` — dialog input filtri
- Progress reporting via `UiProgress` + `ProgressCallbacks` (factory `FLogin.CreaProgress`)

### Log e diagnostica (procedure)
- **Routing centralizzato in `EFMago/Common/LogDiagnostica.cs`** — i servizi non scelgono mai il target a mano:
  - errori / anomalie / rollback → `Errore(...)` / `Avviso(...)` → `LogTarget.Both` (sempre visibili, con tag `[ERRORE]` / `[AVVISO]`)
  - eccezioni → `Errore(log, ex)`: messaggio su UI+file, stack solo su file
  - informativi (conteggi, riepiloghi di esito) → `Informativo(...)` → `AppConfig.LogMode`
  - filtri della run → `LogFiltri(...)` → **solo file**, sempre (a video inondavano il ListBox)
  - statistiche bulk (una riga per tabella) → `Scarica(...)` → **solo file**, sempre; a video basta la sintesi emessa dal servizio
  - tracciato `debugging` (lungo, una riga per ordine/attività) → **solo file**, solo se `IsDebugging`
  - accumulatori (`errori`, `avvisi`, `bulk`, `debugging`) scaricati da `LogDiagnostica.Scarica(...)` nel `finally` del workflow
- **Blocchi multi-riga**: se il target include la UI vanno emessi **riga per riga** (il ListBox non rende `\n`); se è solo file, in un colpo solo (più veloce, preserva le righe vuote)
- **File di sessione**: `FLogin_Load` crea `FatturazioneContratti_yyyyMMdd_HHmmss.log` (anche vuoto) in `AppConfig.LastLogPath` e salva il percorso; da lì si apre dal LinkLabel
- **Crash globali**: handler in `Program.cs` → `Common/CrashLogger.cs` (`crash_yyyyMMdd_HHmmss_fff.log`), separato dal log di sessione

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
16. **Log: sempre via `LogDiagnostica`** — nei servizi non si passa mai `LogTarget` a mano (né `LogToUi`/`LogToFile`, che non esistono più). Errori/anomalie/rollback devono restare visibili pure con `LogMode = File`: usa `Errore`/`Avviso`, non `Informativo`. I filtri della run e le statistiche bulk vanno **solo su file**. Ogni nuovo workflow chiude con `LogDiagnostica.Scarica(...)` nel `finally`, altrimenti gli accumulatori non arrivano a nessuno.
17. **Interruzione cooperativa** — i workflow ricevono `CancellationToken` e lo controllano ai confini naturali (ordine/contratto/intervento/chunk), mai dentro il ciclo bulk. Se il token arriva con transazione aperta: rollback esplicito e `throw` (non `return`), così l'esito resta "INTERROTTO" e non "COMPLETATO".
18. **Query: prima gli ID, poi i dati** — estrarre gli ID con i filtri e caricare i record a blocchi (`AsSplitQuery()` + predicato `OR` su ID) invece di `Include` su collection in un colpo solo; caricare le anagrafiche solo per le chiavi effettivamente usate nella run (`MaCustSupp` per gli `InvoicingCustomer` degli ordini estratti). Misure 21/10/2026 su ALLSPA: 67s → 11s e 232s → 3,8s.
19. **Mago non copia le colonne custom dei documenti** — nella duplicazione fattura↔proforma Mago si porta dietro solo i campi **standard** (verificato: `Packing` e `ReferenceDocNo`). Per un riferimento che deve sopravvivere alle copie usare un campo standard, non una colonna `ALL_*` aggiunta a mano. Il consuntivo usa `MA_SaleDocDetail.Packing` come carrier della chiave `MChiaveFattura`.

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
| `tools/` | Harness di verifica fuori app (es. `CreazioneOrdiniSmoke`) — solo DB di test |
| `sql/` | Script DDL/viste manuali da eseguire sui DB Mago (non automatici) |
| `MigrazioneVb_Csharp.txt` | Log migrazione — solo riferimento |

## Task Comuni

### Aggiungere nuova tabella Mago
1. Aggiungi a progetto EF `ALLMigrazione` → scaffold → esegui `copia.ps1` (copia il model in `EFMago/Data/Models/`)
2. In `FattureContext.cs`: aggiungi `DbSet<T>` + configurazione in `OnModelCreating` (chiavi, relazioni, `HasDefaultValueSql`)
3. Se serve `AddNew<T>()` support: nessuna modifica codice (estensione legge default Fluent API automaticamente)

### Aggiungere nuovo workflow
1. Crea servizio in `Orders/` o `Invoicing/`
2. Log e progress solo tramite `LogDiagnostica` + `LogDiagnostica.Scarica(...)` nel `finally` (convenzione 16); callback raccolti in `ProgressCallbacks`
3. Aggiungi pulsante + handler in `FLogin` seguendo il pattern `EseguiOperazione` (che passa già la `CancellationTokenSource` della run e gestisce "INTERROTTO")
4. Aggiungi form filtri se serve (pattern `FAskFiltriOrdini`, DTO che implementa `ILogFiltri`)

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
- `TODO.md` — solo lavoro aperto: verifica manuale, punti noti da decidere, fasi residue del sito
