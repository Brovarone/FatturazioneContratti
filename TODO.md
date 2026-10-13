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
  (`Orders/OrderGenerationService.cs`, `Orders/OrderGenerationService.Istat.cs`,
  `Invoicing/ConsuntivoService.cs`, `Common/BulkPersistenceHelper.cs`), quindi le
  righe operative dei servizi restano solo su UI. Sul file arrivano solo i messaggi
  di `FLogin` (avvio/completato/errori): il file non è più *sempre* vuoto, ma non
  contiene il dettaglio operativo.
- `debugging`/`avvisi` ancora mai scaricati (nessun `.ToString()` in giro); errori
  dei servizi con solo `ex.Message`, senza stack. I crash globali vanno invece nel
  crash log via nuovo `Common/CrashLogger.cs` (file untracked) + handler in
  `Program.cs` (vedi punto 8).

**Decisioni residue** (UI / file / entrambi):

- [ ] Target servizi: passare `Both` (o `LogMode`) nelle `Invoke` dei servizi, oppure
  promuovere `Ui`→`Both` in `FLogin.Log` quando `LogMode` è `File`/`Both`?
- [ ] Scaricare `debugging`/`avvisi` su file a fine workflow (vedi punto 19)?
- [x] Formato/rotazione: file per sessione (`FatturazioneContratti_yyyyMMdd_HHmmss.log`)
  creato in `FLogin_Load` (anche vuoto) + `LastLogPath` salvato — `Forms/FLogin.cs:102-124`.
- [x] LinkLabel "Apri file di log": apre file → fallback Notepad → fallback cartella;
  mostrata a fine operazione — `Forms/FLogin.cs:444-499`.

Note: `LogTarget.Both` esiste già; `debugging` accumula tutto il tracciato diagnostico
([ESCLUSA] varie, dettaglio righe, messaggi storici tipo "  - Cessato").

---

## 2. Punti lasciati in sospeso (segnalati in revisione, NON applicati)

### Punto 3 — `continue` su attività fatturata salta altre tipologie
- File: `Orders/OrderGenerationService.Istat.cs:176-180` e `Orders/OrderGenerationService.cs:555` e `:594`
- `if (att.Fatturata == "1") continue;` interrompe l'analisi delle restanti
  tipologie (Annullamento/Istat) della **stessa** attività.
- DA VERIFICARE sui dati reali: un'attività può essere contemporaneamente
  Sospensione + Annullamento/Istat? Se sì è un bug (viene ignorata).
- Fix probabile: `if/else if` espliciti invece di `continue`.

### Punto 6 — causale attività ISTAT dalla UI ignorata
- File: `Orders/OrderGenerationService.Istat.cs:272` → `Attivita = "ISTAT"` hardcoded
- Il campo `TxtIstatAttivita` della dialog (`Forms/FAskFiltriOrdini.cs`) popola
  `FiltriOrdini.CausaleAttivita`, che viene loggato ma **mai usato**: se l'utente
  cambia causale non ha effetto.
- Decidere: usare `filtri.CausaleAttivita` (con fallback "ISTAT" se vuoto) oppure
  togliere il campo dalla UI.

### Punto 19 — log diagnostici mai emessi
- `debugging` e `avvisi` (StringBuilder) popolati in tutti i workflow e mai
  scaricati su log/UI. Esempi di messaggi persi:
  - `OrderGenerationService.cs`: "data prossima fatturazione antecedente...", tutte le `[ESCLUSA]`
  - `OrderGenerationService.Istat.cs`: "riga di adeguamento Istat già presente per l'anno...", "scadenza fissa. Controllare canoni!"
- Da concordare con le decisioni del punto 1 (questa sessione).

### Punto 8 — robustezza UI (COMPLETATO)
- `Forms/FLogin.cs`: helper `PostToUi` con `IsHandleCreated`/`IsDisposed` +
  `BeginInvoke` in try/catch, `_inElaborazione` volatile, `EseguiOperazione` usa
  `PostToUi`; `AggiornaStato` reso sicuro (guardie handle + try/catch sul
  `BeginInvoke`); `SetUiBusy` reso sicuro (`IsDisposed`/`IsHandleCreated`,
  `Invoke` in try/catch). Stato del testo UI non esplode più a form chiusa.
- `Program.cs`: gestori globali (`Application.ThreadException`,
  `AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException`) con
  `SetObserved()` e wrap di `Application.Run` in try/catch.
- `Common/CrashLogger.cs`: scrive `crash_yyyyMMdd_HHmmss_fff.log` (stack trace,
  contesto thread/DB/eseguibile, fallback AppData).
- Build: 0 errori / 0 avvisi. File ancora da committare aggiungere a git:
  `Common/CrashLogger.cs`.

---

## 3. Altri punti del report originale non toccati (22-25 e minimi)

### Punto 22 — `OrderGenerationService.GeneraRigheOrdine` è un metodo di ~300 righe
- Estrarre: costruzione query/filtri, elaborazione singolo contratto, creazione righe,
  calcolo statistiche. I 4 callback + 3 `ProgressCallbacks` potrebbero diventare un
  solo record opzioni.

### Punto 23 — duplicazioni
- [x] `CreaRigaDescrittiva` / `CreaRigaRiferimento` (`OrderGenerationService.cs`) unificate
  in `CreaRigaNota(..., LineType, ...)` — modifica non committata, da riverificare in build/test.
- [ ] `SubstringSafe` (`OrderGenerationService.cs`) duplicato inline in `ConsuntivoService.cs`
  (`Invoicing/ConsuntivoService.cs:545`, ternario su `c.Nota` troncata a 32) → helper comune.
- [ ] Creazione dei `ProgressCallbacks` ripetuta 4 volte in `Forms/FLogin.cs` → factory.

### Punto 24 — warning annullabili rilevanti (dal build)
- `Invoicing/ConsuntivoService.cs`: `Break(..., customer, ...)` può ricevere null
  (warning CS8604) → annotare firme.
- `MiniDistinta` (ConsuntivoService.cs): proprietà non inizializzate (CS8618).
- `OrderGenerationService.cs`: warning CS8604 su `EseguiSalvataggioTransazionale`
  (`logMessage` null-passabile).

### Punto 25 — dettagli minori
- `Forms/FAskFiltriOrdini.cs`: `using System.Windows.Forms;` duplicato (righe 6 e 12).
- `Invoicing/FiltriOrdiniConsuntivo.cs`: `SingoloOrdine`/`NrMese` non usati dal servizio.
- `Orders/CurOrdRow.cs`: `QtaAnnullata = 0;` duplicato nel costruttore; inizializzazioni
  ridondanti vs. inizializzatori di proprietà (stesso pattern in CurOrd/CurFatt/CurFattRow).
- `Common/FattureContextExtensions.cs`: cache `Initializers` legata al primo DbContext
  usato (trabocchetto latente se in-process compaiono contesti diversi).
- `Invoicing/FattureDaOrdiniService.cs`: noto, SQL raw, fuori Scope sessione.

---

## 4. Verifica manuale consigliata (prima di chiudere)

Nessun test automatico. Da provare via UI con DB di test:
1. **Genera Canoni** — timestamp unificati, include attività testa aggiunto al loader.
2. **Adeguamento ISTAT** — nuovo chunking ID-first + iterator, prototipo attività
   (`AddNew`), verificare `curLastLineAtt` (continue su attività di testa).
3. **Consuntivazione** (**la più a rischio**): raggruppamento fatture ristrutturato,
   ID documento assegnato solo ai gruppi con righe (nessuna testa vuota, nessun
   buco nel numeratore), cross-ref e ritenuta solo con righe, `LastSubId` via GroupBy,
4. **Allineamento Fatture** — ATTENZIONE: non più "non toccato": `FattureDaOrdiniService`
   ora prende `FiltriAllineamentoFatture` + nuova dialog `FAskFiltriAllineamentoFatture`
   (modifiche non committate) → da riverificare via UI come gli altri.

---

## 5. Fix già applicate in questa sessione (per riferimento)

| # | Fix | File |
|---|-----|------|
| 1 | `PathPaghe` persistito in `Save`/`Load`/`ConfigData` | `Config/AppConfig.cs` |
| 2 | Riga attività ISTAT con pattern prototipo `AddNew` | `Orders/OrderGenerationService.Istat.cs` |
| 4 | Testa fattura/cross-ref/ritenuta solo se il gruppo ha righe; ID documento assegnato alla prima riga utile | `Invoicing/ConsuntivoService.cs` |
| 5 | `MaIdnumbers.FirstOrDefault` + errore esplicito | `Invoicing/ConsuntivoService.cs` |
| 7 | `TbcreatedId/TbmodifiedId = _loginId` su righe descrittive fattura | `Invoicing/ConsuntivoService.cs` |
| 9 | `aOrdini` da lista già materializzata (no doppia query) | `Invoicing/ConsuntivoService.cs` |
| 10 | Rimosso `.Load()` duplicato su MaCustSupp | `Invoicing/ConsuntivoService.cs` |
| 11 | Join distinte via `ToLookup` + `FilialeCorrisponde` (O(N+M)) | `Invoicing/ConsuntivoService.cs` |
| 12 | `LastSubId` con `GroupBy().ToDictionary()` | `Invoicing/ConsuntivoService.cs` |
| 13 | ISTAT: ID-first + chunking con loader condiviso | `Orders/OrderGenerationService.Istat.cs`, `OrderGenerationService.cs` |
| 15 | `catch` quando-limitato a `SqlNullValueException` | `Orders/OrderGenerationService.cs` |
| 17 | `AppConfig.LastLoadError` + `Debug.WriteLine` su config illeggibile | `Config/AppConfig.cs` |
| 18 | Timestamp unici per operazione (campo `_now`) | `OrderGenerationService*.cs`, `ConsuntivoService.cs` |
| 20 | Rimosso `Common/DbDefaults.cs` (morto) | `Common/` |
| 21 | Rimossi codice morti: `PrecaricaTabelleOrdini`, `protoAllordCliAttivita`, wrapper log ridondante, campo `perc` | vari |
| 14 | Cache statica property per tipo in `CloneFromPrototype` | `Common/EntityCloner.cs` |
| 16 | `UiProgress.Advance` sicuro con Minimum == Maximum | `Forms/UiProgress.cs` |
| — | Esito finale su `lblStatus` (verde/rosso) + LinkLabel "Apri file di log" | `Forms/FLogin.cs` |
| L1 | `LogMode` default `Both` + persistenza `Load`/`Save`/`ConfigData` + combo in `FSettings` | `Config/AppConfig.cs`, `Forms/FSettings.cs` (+Designer) |
| L2 | `FLogin.Log(string)` instrada su `LogMode`; file di log creato (anche vuoto) in `FLogin_Load` | `Forms/FLogin.cs` |
| 8 | `PostToUi` + `_inElaborazione` volatile; handler globali eccezioni + `CrashLogger` (untracked) | `Forms/FLogin.cs`, `Program.cs`, `Common/CrashLogger.cs` |
| 23a | `CreaRigaDescrittiva`/`CreaRigaRiferimento` unificate in `CreaRigaNota` | `Orders/OrderGenerationService.cs` |
| A4 | Allineamento: nuova dialog `FAskFiltriAllineamentoFatture` (date da/a + radio "Tutti i numeri" / "Intervallo primo-ultimo"); firma `AdeguaFattureDaOrdini` ora riceve `FiltriAllineamentoFatture` (`AllNumbers`, `NrFirst`, `NrLast`) | `Invoicing/FattureDaOrdiniService.cs`, `Invoicing/FiltriAllineamentoFatture.cs`, `Forms/FAskFiltriAllineamentoFatture*.cs`, `Forms/FLogin.cs` |

> Le righe L1, L2, 8, 23a, A4 risultano **eseguite ma non committate** (working copy
> del 13/10/2026) — da committare dopo build + verifica manuale §4.

Build: 0 errori (warning ~1820, per lo più attesi sui model generati).
