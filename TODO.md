# TODO — Promemoria sessione (12/10/2026)

Riepilogo di cosa resta da fare sul progetto FatturazioneContratti dopo la sessione
di revisione codice. Le fix già applicate sono elencate in fondo per riferimento.

*Aggiornamento 13/10/2026: voci riverificate sul codice (working copy con modifiche
non committate). Le voci eseguite sono spuntate `[x]` e riportate in §5.*

---

## 1. APPUNTO LOG (root della questione, da decidere prima di tutto)

**Stato al 13/10/2026** (verificato sul codice; modifiche non committate): il quadro
è parzialmente cambiato rispetto alla sessione del 12/10.
- `AppConfig.LogMode` ora ha default `Both` (era `Ui`), è persistito in `Load`/`Save`
  (`Config/AppConfig.cs:40,102-103,137,171`) ed è selezionabile in `FSettings`
  (combo UI/File/Entrambi — NOTA: refuso "Entambi" in `Forms/FSettings.cs:44`).
  `FLogin.Log(string)` ora instrada su `LogMode` (`Forms/FLogin.cs:357-361`).
- MA i servizi chiamano ancora il log con target esplicito `LogTarget.Ui`
  (`Orders/GenerazioneRigheOrdiniService.cs`, `Orders/GenerazioneRigheOrdiniService.Istat.cs`,
  `Invoicing/ConsuntivoService.cs`, `Common/BulkPersistenceHelper.cs`), quindi le
  righe operative dei servizi restano solo su UI. Sul file arrivano solo i messaggi
  di `FLogin` (avvio/completato/errori): il file non è più *sempre* vuoto, ma non
  contiene il dettaglio operativo.
- `debugging`/`avvisi` ancora mai scaricati (nessun `.ToString()` in giro); errori
  dei servizi con solo `ex.Message`, senza stack. I crash globali vanno invece nel
  crash log via nuovo `Common/CrashLogger.cs` (file untracked) + handler in
  `Program.cs` (vedi punto 8).

**Decisioni residue** (UI / file / entrambi):

- [x] Target servizi: **risolto in §1b** — errori/anomalie → `Both`, informativi →
      `LogMode`, `debugging` → `File` se `IsDebugging`, routing centralizzato in
      `Common/LogDiagnostica.cs`.
- [x] Scaricare `debugging`/`avvisi` su file a fine workflow: **risolto in §1b** —
      `LogDiagnostica.Scarica(...)` chiamata in `finally` a fine workflow.
- [x] Formato/rotazione: file per sessione (`FatturazioneContratti_yyyyMMdd_HHmmss.log`)
  creato in `FLogin_Load` (anche vuoto) + `LastLogPath` salvato — `Forms/FLogin.cs:102-124`.
- [x] LinkLabel "Apri file di log": apre file → fallback Notepad → fallback cartella;
  mostrata a fine operazione — `Forms/FLogin.cs:444-499`.

Note: `LogTarget.Both` esiste già; `debugging` accumula tutto il tracciato diagnostico
([ESCLUSA] varie, dettaglio righe, messaggi storici tipo "  - Cessato").

---

## 1b. PIANO ADEGUAMENTO LOG (16/10/2026) — promemoria step

**Decisioni prese (16/10/2026)**:
- **Routing**: errori/anomalie/rollback → `LogTarget.Both`; informativi (filtri,
  conteggi, riepiloghi, statistiche bulk) → `AppConfig.LogMode`; `debugging` (tracciato
  lungo) → `LogTarget.File` **solo se** `AppConfig.IsDebugging`.
- Blocchi multi-riga: **riga per riga** quando il target include la UI (senza il
  `lstStato.Items.Add` con `\n` diventa illeggibile), altrimenti un solo `Invoke`.
- Filtri nel workflow consuntivo: loggati **due volte** (uno per servizio) — ogni step
  resta autodescrittivo.
- `progress2` in `ControllaFlussoIntegra`/`GeneraFattureConsuntivo`: **non rimossa**,
  va **abilitata con la logica a blocchi da 100 record** (progress1 = blocchi,
  progress2 = record nel blocco, come `GenerazioneRigheOrdiniService`).

**Step — TUTTI ESEGUITI il 16/10/2026** (build: 0 errori, warning invariati ~1816):

- [x] **A. `ConsuntivoService.ControllaFlussoIntegra`**
  - [x] log filtri all'avvio (`LogDiagnostica.LogFiltri` → LogMode, riga per riga)
  - [x] contatore `associati` + esito finale **sempre** emesso:
        `Controllo flusso completato: N interventi, M associati, K anomalie`
  - [x] blocco anomalie: riepilogo in **una riga → solo UI**; dettaglio completo → `File`
        una volta sola. *Nota*: con `Both` la testa finiva due volte nel file, quindi la
        riga di testa va su UI e il file riceve il blocco completo subito dopo (entrambi
        hanno comunque tutto).
  - [x] voci anomalie arricchite via `DescriviIntervento(i, nrOrdine)` →
        `Contratto X - ordine Y - evento Z - filiale F - allarme gg/mm/aaaa`
  - [x] `catch`: `LogDiagnostica.Errore(log, ex)` → messaggio `Both` + stack `File`
  - [x] `progress2` **abilitata** con blocchi da 100 (`DimensioneBlocco`):
        progress1 = blocchi, progress2 = record del blocco; via `errori` morto e via
        `Debug.WriteLine` nel loop; `TBModifiedID = _loginId`
- [x] **B. `ConsuntivoService.GeneraFattureConsuntivo`**
  - [x] log filtri all'avvio → LogMode
  - [x] contatore `MA_IdNumbers` mancante → `Both`; eccezione → `Errore(log, ex)`;
        successo → `Fatture generate con successo: N testate, M righe` (LogMode)
  - [x] `finally` → `Scarica(bulk: bulkMessage, debugging: debugging)`
  - [x] `progress2` con blocchi da 100 + `Finish()` finale (mancava del tutto)
  - [x] via `using static ...DbLoggerCategory`; via `errori` (non serviva più)
- [x] **C. `GenerazioneRigheOrdiniService.cs` + `.Istat.cs`**
  - [x] log filtri all'avvio di entrambi → LogMode
  - [x] informativi `Ui` → LogMode; errori/crash → `Both` (wrapper scansione diagnostica
        `LoadOrdersChunkByOrIn`, percentuale ISTAT = 0, catch)
  - [x] `finally` → `Scarica(errori, avvisi, bulkMessage, debugging)`: **`stato.Errori`
        finalmente compariva nell'esito ma non veniva mai stampato** ("COMPLETATO CON
        ERRORI" senza motivo)
  - [x] `EseguiSalvataggioTransazionale` ora **ritorna `bool`**: prima un rollback bulk
        veniva segnalato solo per caso (passava tramite `errori`), ora è esplicito e
        `[COMPLETATO]` viene stampato solo se `Errori` è vuoto
- [x] **D. `Common/BulkPersistenceHelper.cs` + nuovo `Common/LogDiagnostica.cs`**
  - [x] rollback → `LogDiagnostica.Errore(log, ex)` + riga rollback → `Both` (stack a file)
  - [x] **parametro `logBuilder` rimosso** da `ExecuteInBulkTransaction`: l'errore bulk
        non viene più duplicato (lo riporta il helper, il caller scarica solo gli errori
        di business) — aggiornati i 3 call site
  - [x] `Common/LogDiagnostica.cs`: `Informativo` / `Errore(string|Exception)` / `Avviso`
        / `LogFiltri` / `Scarica` + `Common/ILogFiltri.cs` implemented da i 3 DTO filtri
- [x] **E. `AggiornamentoFattureService` + `FLogin`**
  - [x] `out resultMessage` **rimosso**: il servizio logga da solo (filtri → LogMode,
        esito `Adeguate N righe` → LogMode, eccezione → `Both` + stack a file); `FLogin`
        passa solo `logMessage: Log`
  - [x] `FLogin` interruzione controllo → `Log(..., Both)` (con `LogMode = File` prima
        spariva dalla UI)
  - [x] **riga nuova** in `EseguiOperazione`: `Database: X [TEST] - Modalità log: Y`
        (nel file di sessione non compariva mai quale DB si stava usando)
- [x] **F. Pulizie**
  - [x] via `LogToUi`/`LogToFile` (mai usati — convenzione #12)
  - [x] via `CostruisciFiltriLog()` nei due form (a carico dei servizi ora)
  - [x] refuso "Entambi": **era già corretto** in `FSettings.cs:44` (nota §1 superata)
  - [x] `FiltriOrdini` / `FiltriOrdiniConsuntivo` / `FiltriAllineamentoFatture`
        implementano `ILogFiltri` (il terzo non aveva nessuna chiamata a `CostruisciFiltriLog`)
- [ ] **G. Verifica manuale (DA FARE)**: 4 workflow su DB test con LogMode = Ui / File /
      Entrambi → il file deve contenere filtri + conteggi + dettaglio anomalie + stack,
      la UI deve restare leggibile; `IsDebugging = true` per il tracciato `debugging`;
      controllare le barre progress a blocchi da 100 (consuntivo, che prima non aveva
      barra secondaria né `Finish`).

---

## 2. Punti lasciati in sospeso (segnalati in revisione, NON applicati)

### Punto 3 — `continue` su attività fatturata salta altre tipologie
- File: `Orders/GenerazioneRigheOrdiniService.Istat.cs:176-180` e `Orders/GenerazioneRigheOrdiniService.cs:555` e `:594`
- `if (att.Fatturata == "1") continue;` interrompe l'analisi delle restanti
  tipologie (Annullamento/Istat) della **stessa** attività.
- DA VERIFICARE sui dati reali: un'attività può essere contemporaneamente
  Sospensione + Annullamento/Istat? Se sì è un bug (viene ignorata).
- Fix probabile: `if/else if` espliciti invece di `continue`.

### Punto 19 — log diagnostici mai emessi ✅ RISOLTO (16/10/2026, vedi §1b)
- `debugging`, `avvisi`, `bulkMessage`, `errori` ora vengono scaricati a fine workflow
  da `LogDiagnostica.Scarica` (chiamato nei `finally` di `GeneraRigheOrdine`,
  `AdeguaIstatOrdine`, `GeneraFattureConsuntivo`).

---

## 3. Altri punti del report originale non toccati (22-25 e minimi)

### Punto 22 — `GenerazioneRigheOrdiniService.GeneraRigheOrdine` era un metodo di ~300 righe (COMPLETATO)
- [x] Fase A — estrazioni a basso rischio: `ApplicaFiltriComuni` (riuso anche da
  `AdeguaIstatOrdine`), `EstraiIdsOrdini`, `record DefaultVenditeIva` +
  `CaricaDefaultVenditeIva`.
- [x] Fase B — stato di elaborazione: `StatoElaborazioneOrdini` (collettori,
  accumulatori di log, default, prototipo riga) e `StatoOrdine` (contatori per singolo
  ordine); estratti `ElaboraOrdine` ed `ElaboraContratto`; il ciclo a blocchi è diventato
  l'iterator locale `CaricaOrdiniPerBlocco`, speculare al workflow ISTAT.
  Rimosso anche `iNrRigheNota` (contatore incrementato e mai letto).
- [x] Fase C — `record GeneraRigheOrdineOptions` con firma `GeneraRigheOrdine(options)`;
  rimosso il parametro morto `progress3` (mai usato) da `GeneraRigheOrdine`,
  `AdeguaIstatOrdine`, `ControllaFlussoIntegra`, `GeneraFattureConsuntivo`.
- Metodo da ~309 a ~114 righe. Nessun cambio di comportamento: try/catch, ordine delle
  progress (blocchi/ordini) e ritorno `Errori.Length == 0` invariati.
- **Da riverificare via UI su DB test** (casi in §4: ordine con più contratti, riga bianca
  singola, attività da rifatturare, scadenza fissa/cessazione, >100 ordini per le progress).

### Punto 24 — warning annullabili rilevanti (dal build)
- `Invoicing/ConsuntivoService.cs`: `Break(..., customer, ...)` può ricevere null
  (warning CS8604) → annotare firme.
- `MiniDistinta` (ConsuntivoService.cs): proprietà non inizializzate (CS8618).
- `GenerazioneRigheOrdiniService.cs`: warning CS8604 su `EseguiSalvataggioTransazionale`
  (`logMessage` null-passabile).

### Punto 25 — dettagli minori
- `Invoicing/FiltriOrdiniConsuntivo.cs`: [x] RISOLTO 14/10/2026 — `SingoloOrdine`/`NrMese`
  ora usati nelle query di estrazione (`Invoicing/ConsuntivoService.cs`):
  nuovo `NrOrdine` nel DTO + checkbox/casella in `FAskFiltriOrdiniConsuntivo`
  (con validazione); controllo-flusso filtra le distinte per `InternalOrdNo`,
  fatturazione filtra `ordiniList` (flag `soloOrdine` valutato in C# per restare
  traducibile in SQL); `NrMese` aggiunge predicato `Month` in modalità `MeseAnno`.
- `Orders/CurOrdRow.cs`: [x] RISOLTO 14/10/2026 — inizializzazioni ridondanti
  rimosse, default costanti spostati in inizializzatori di proprietà
  (`IsOk = true`, `string.Empty`, `TipologiaServizio = Canone`); nei costruttori
  restano solo le date e i valori letti da riga. Stesso pattern applicato a
  `Orders/CurOrd.cs`, `Invoicing/CurFatt.cs` (`SaleDocId`/`Cliente` ai default CLR),
  `Invoicing/CurFattRow.cs` (`IsOk = true` in inizializzatore). `QtaAnnullata = 0`
  duplicato eliminato. Nessun cambio di comportamento (solo 0/false già default).
- [x] `Common/FattureContextExtensions.cs`: cache `Initializers` ora a due livelli
  `IModel -> Type` (prima solo `Type`, quindi legata al primo DbContext visto:
  applicava i default del primo modello a contesti con modello diverso).
- [x] `Invoicing/AggiornamentoFattureService.cs`: resta SQL raw di proposito (UPDATE...FROM con
  join su 3 tabelle, scomoda da esprimere con EF), ma allineato al resto del progetto:
  valori `LineType`/`DocumentType` presi dagli enum `Common/MagoEnum.cs` invece dei numeri
  hardcoded, e timestamp unico per operazione (`now` a inizio metodo, come il campo `_now`
  degli altri servizi). Restano fuori portata i nomi di tabella/colonna, non tipizzati.

---

## 4. Verifica manuale consigliata (prima di chiudere)

Nessun test automatico. Da provare via UI con DB di test:
1. **Genera Canoni** — timestamp unificati, include attività testa aggiunto al loader.
2. **Adeguamento ISTAT** — nuovo chunking ID-first + iterator, prototipo attività
   (`AddNew`), verificare `curLastLineAtt` (continue su attività di testa).
3. **Consuntivazione** (**la più a rischio**): raggruppamento fatture ristrutturato,
   ID documento assegnato solo ai gruppi con righe (nessuna testa vuota, nessun
   buco nel numeratore), cross-ref e ritenuta solo con righe, `LastSubId` via GroupBy,
4. **Allineamento Fatture** — ATTENZIONE: non più "non toccato": `AggiornamentoFattureService`
   ora prende `FiltriAllineamentoFatture` + nuova dialog `FAskFiltriAllineamentoFatture`
   (modifiche non committate) → da riverificare via UI come gli altri.
5. **Log (§1b, 16/10/2026)** — per ogni workflow verificare su DB test:
   - con `LogMode = Entrambi`: il file contiene i **filtri della run**, i conteggi,
     il dettaglio anomalie (una sola volta) e gli errori con stack;
   - con `LogMode = File`: la UI resta leggibile ma gli **errori** (anomalie controllo,
     rollback bulk, eccezioni) compaiono **comunque** sulla UI;
   - con `LogMode = UI`: i messaggi informativi non finiscono a file, ma errori e
     dettaglio anomalie sì;
   - `IsDebugging = true`: a fine workflow nel file compare il tracciato `debugging`
     (una riga per ordine/attività);
   - barre di avanzamento del **consuntivo**: progress1 = blocchi da 100,
     progress2 = record del blocco, chiusura `Finish` a fine passo.

---

## 5. Fix già applicate in questa sessione (per riferimento)

| # | Fix | File |
|---|-----|------|
| 1 | `PathPaghe` persistito in `Save`/`Load`/`ConfigData` | `Config/AppConfig.cs` |
| 2 | Riga attività ISTAT con pattern prototipo `AddNew` | `Orders/GenerazioneRigheOrdiniService.Istat.cs` |
| 4 | Testa fattura/cross-ref/ritenuta solo se il gruppo ha righe; ID documento assegnato alla prima riga utile | `Invoicing/ConsuntivoService.cs` |
| 5 | `MaIdnumbers.FirstOrDefault` + errore esplicito | `Invoicing/ConsuntivoService.cs` |
| 6 | Causale attività ISTAT dalla UI (`TxtIstatAttivita` → `FiltriOrdini.CausaleAttivita`) con fallback `CausaleAttivitaIstatDefault = "ISTAT"` | `Orders/GenerazioneRigheOrdiniService.Istat.cs`, `Forms/FAskFiltriOrdini.cs`, `Orders/FiltriOrdini.cs` |
| 7 | `TbcreatedId/TbmodifiedId = _loginId` su righe descrittive fattura | `Invoicing/ConsuntivoService.cs` |
| 9 | `aOrdini` da lista già materializzata (no doppia query) | `Invoicing/ConsuntivoService.cs` |
| 10 | Rimosso `.Load()` duplicato su MaCustSupp | `Invoicing/ConsuntivoService.cs` |
| 11 | Join distinte via `ToLookup` + `FilialeCorrisponde` (O(N+M)) | `Invoicing/ConsuntivoService.cs` |
| 12 | `LastSubId` con `GroupBy().ToDictionary()` | `Invoicing/ConsuntivoService.cs` |
| 13 | ISTAT: ID-first + chunking con loader condiviso | `Orders/GenerazioneRigheOrdiniService.Istat.cs`, `GenerazioneRigheOrdiniService.cs` |
| 15 | `catch` quando-limitato a `SqlNullValueException` | `Orders/GenerazioneRigheOrdiniService.cs` |
| 17 | `AppConfig.LastLoadError` + `Debug.WriteLine` su config illeggibile | `Config/AppConfig.cs` |
| 18 | Timestamp unici per operazione (campo `_now`) | `GenerazioneRigheOrdiniService*.cs`, `ConsuntivoService.cs` |
| 20 | Rimosso `Common/DbDefaults.cs` (morto) | `Common/` |
| 21 | Rimossi codice morti: `PrecaricaTabelleOrdini`, `protoAllordCliAttivita`, wrapper log ridondante, campo `perc` | vari |
| 14 | Cache statica property per tipo in `CloneFromPrototype` | `Common/EntityCloner.cs` |
| 16 | `UiProgress.Advance` sicuro con Minimum == Maximum | `Forms/UiProgress.cs` |
| — | Esito finale su `lblStatus` (verde/rosso) + LinkLabel "Apri file di log" | `Forms/FLogin.cs` |
| L1 | `LogMode` default `Both` + persistenza `Load`/`Save`/`ConfigData` + combo in `FSettings` | `Config/AppConfig.cs`, `Forms/FSettings.cs` (+Designer) |
| L2 | `FLogin.Log(string)` instrada su `LogMode`; file di log creato (anche vuoto) in `FLogin_Load` | `Forms/FLogin.cs` |
| 8 | `PostToUi` + `_inElaborazione` volatile; handler globali eccezioni + `CrashLogger` | `Forms/FLogin.cs`, `Program.cs`, `Common/CrashLogger.cs` |
| 23a | `CreaRigaDescrittiva`/`CreaRigaRiferimento` unificate in `CreaRigaNota` | `Orders/GenerazioneRigheOrdiniService.cs` |
| 23c | `SubstringSafe` estratto in `Common/StringHelpers` (rimosso privato + ternario inline in consuntivo) | `Common/StringHelpers.cs`, `Orders/GenerazioneRigheOrdiniService.cs`, `Invoicing/ConsuntivoService.cs` |
| 23b | `ProgressCallbacks` centralizzati in factory `FLogin.CreaProgress` | `Forms/FLogin.cs` |
| A4 | Allineamento: nuova dialog `FAskFiltriAllineamentoFatture` (date da/a + radio "Tutti i numeri" / "Intervallo primo-ultimo"); firma `AdeguaFattureDaOrdini` ora riceve `FiltriAllineamentoFatture` (`AllNumbers`, `NrFirst`, `NrLast`) | `Invoicing/AggiornamentoFattureService.cs`, `Invoicing/FiltriAllineamentoFatture.cs`, `Forms/FAskFiltriAllineamentoFatture*.cs`, `Forms/FLogin.cs` |
| 25 | Rimossi assegnamenti ridondanti (0/false) dai costruttori, default in inizializzatori di proprietà | `Orders/CurOrdRow.cs`, `Orders/CurOrd.cs`, `Invoicing/CurFatt.cs`, `Invoicing/CurFattRow.cs` |
| 25b | `SingoloOrdine`/`NrMese` cablati nelle query consuntivo (+ `NrOrdine` in DTO e UI) | `Invoicing/ConsuntivoService.cs`, `Invoicing/FiltriOrdiniConsuntivo.cs`, `Forms/FAskFiltriOrdiniConsuntivo*.cs` |
| 22 | `GeneraRigheOrdine` scomposto: filtri/ID/default condivisi, `StatoElaborazioneOrdini`+`StatoOrdine`, `ElaboraOrdine`/`ElaboraContratto`, iterator a blocchi, `record GeneraRigheOrdineOptions`, rimosso `progress3` | `Orders/GenerazioneRigheOrdiniService*.cs`, `Orders/DefaultVenditeIva.cs`, `Orders/GeneraRigheOrdineOptions.cs`, `Invoicing/ConsuntivoService.cs`, `Forms/FLogin.cs` |

> Le righe L1, L2, 8, 22, 23a, 23b, 23c, A4 sono state **committate** il 13-14/10/2026
> (commit su `master` + branch `feature/refactor-genera-righe-ordine`) — resta la verifica
> manuale §4.

---

## 6. Adeguamento LOG — 16/10/2026 (dettaglio modifiche)

| # | Modifica | File |
|---|----------|------|
| L3 | **`Common/LogDiagnostica.cs` (nuovo)**: `Informativo` (→ `LogMode`), `Errore(string)` (→ `Both`), `Errore(Exception)` (messaggio → `Both`, stack → `File`), `Avviso` (→ `Both`), `LogFiltri` (filtri → `LogMode`, riga per riga), `Scarica(errori, avvisi, bulk, debugging)`; blocchi multi-riga emessi **riga per riga** quando il target include la UI | `Common/LogDiagnostica.cs` |
| L4 | **`Common/ILogFiltri.cs` (nuovo)**: contratto `LogFiltri`/`CostruisciFiltriLog` implementato dai 3 DTO | `Common/ILogFiltri.cs`, `Orders/FiltriOrdini.cs`, `Invoicing/FiltriOrdiniConsuntivo.cs`, `Invoicing/FiltriAllineamentoFatture.cs` |
| L5 | Rollback bulk → `Errore(log, ex)` + riga rollback `Both`; rimosso il parametro `logBuilder` (l'errore non viene più duplicato negli accumulatori del caller) | `Common/BulkPersistenceHelper.cs` + 3 call site |
| L6 | `ControllaFlussoIntegra`: filtri in log, contatore `associati`, esito sempre emesso, anomalia con **riga contatori su UI + dettaglio su file una volta sola**, voci anomalia con evento/filiale/data, `catch` con stack, `progress2` a blocchi da 100, via `errori` morto e `Debug.WriteLine`, `TBModifiedID = _loginId` | `Invoicing/ConsuntivoService.cs` |
| L7 | `GeneraFattureConsuntivo`: filtri in log, `MA_IdNumbers` mancante → `Both`, successo con testate **e righe**, `catch` con stack, `finally` che scarica `bulkMessage`/`debugging`, `progress2` a blocchi da 100 + `Finish` (mancava), via `errori` e `using` inutile | `Invoicing/ConsuntivoService.cs` |
| L8 | `GeneraRigheOrdine`/`AdeguaIstatOrdine`: filtri in log, informativi `Ui`→`LogMode`, errori `Ui`→`Both`, `finally` con `Scarica` (finalmente `stato.Errori` viene stampato), `EseguiSalvataggioTransazionale` **ritorna `bool`**, `[COMPLETATO]` solo senza errori | `Orders/GenerazioneRigheOrdiniService.cs`, `Orders/GenerazioneRigheOrdiniService.Istat.cs` |
| L9 | Allineamento: rimosso `out resultMessage` → il servizio logga filtri, esito e eccezione (con stack); `FLogin` passa `logMessage: Log`. Interruzione controllo → `Both`; nuova riga `Database: X [TEST] - Modalità log: Y` in apertura | `Invoicing/AggiornamentoFattureService.cs`, `Forms/FLogin.cs` |
| L10 | Pulizie: via `LogToUi`/`LogToFile`, via `CostruisciFiltriLog()` nei form (a carico dei servizi) | `Forms/FLogin.cs`, `Forms/FAskFiltriOrdini.cs`, `Forms/FAskFiltriOrdiniConsuntivo.cs` |

Modifiche non committate. Refuso "Entambi" in `FSettings`: **già corretto** in origine.

Build: **0 errori** (warning ~1816, invariati rispetto a prima della sessione di log).

---

## 7. Sito web ordini / libreria EFMago (17-19/10/2026)

Piano: sito Blazor Server (.NET 8) sul server di Biella per la **sola scrittura di
ordini**; repo separato; il codice condiviso (context, modelli, helper, servizi) vive
nella class library `src/EFMago` (net8.0) referenziata da entrambi i programmi.
Branch: `feature/libreria-efmago`.

- [x] **A0** — rename servizi (`GeneraRigheOrdineService` → `GenerazioneRigheOrdiniService`,
      `AdeguaFattureService` → `AggiornamentoFattureService`) — commit "Refactor Nomi Service".
- [x] **A** — estrazione libreria `EFMago` (`Data/`, `Common/` meno CrashLogger, `Orders/`,
      `Invoicing/`; namespace invariati). Config disaccoppiata: `SqlTools.Configure(EfmagoOptions)`
      + `AppConfig.SyncToLibrary()` per il WinForms; `LogTarget` spostato in `Common/LogTarget.cs`.
      `copia.ps1` ora copia in `src\EFMago\Data`. AGENTS.md aggiornato.
- [x] **B** — nuovo `Orders/CreazioneOrdiniService` (`CreaOrdine(CreazioneOrdineRequest)`):
      testata `MaSaleOrd` + Summary/Shipping/Notes + righe da zero, contatori `SaleOrdId`
      (`MA_IDNumbers`) e numero `yy/NNNNN` (`MA_NonFiscalNumbers`, riga anno nuovo creata
      da template) allocati con `UPDATE ... OUTPUT` in transazione. Verificato su TMPSPA
      con `tools/CreazioneOrdiniSmoke`: ordine `26/00001` (SaleOrdId 1002), contatori e
      satelliti OK — l'ordine di prova è rimasto in TMPSPA, aprirlo da Mago per il
      confronto visivo.
- [ ] **C** — nuovo repo sito Blazor Server (login applicativo, form ordine); referenziare
      `EFMago` via path/git; `appsettings.json` → `SqlTools.Configure`. Workload ASP.NET in
      VS 2022 da installare (~2 GB).
- [ ] **D** — form ordine (cliente, righe, note) sul sito.
- [ ] **E** — deploy IIS su server Biella (HTTPS).

Note per il sito: niente health/schema check a runtime (`ModelManifest` differito);
istanziazione manuale dei servizi (niente DI nella libreria); stesso pattern contatori/audit.
