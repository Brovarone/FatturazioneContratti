# TODO — Promemoria sessione (12/10/2026)

Riepilogo di cosa resta da fare sul progetto FatturazioneContratti dopo la sessione
di revisione codice. Le fix già applicate sono elencate in fondo per riferimento.

---

## 1. APPUNTO LOG (root della questione, da decidere prima di tutto)

**Problema**: il sistema di log va rivisto. Oggi i servizi chiamano il log con
`LogTarget.Ui` (OrderGenerationService, ConsuntivoService ecc.), quindi
`FLogin.Log` scrive solo sulla listbox `lstStato` e **il file .log resta vuoto**
(il file viene creato in `FLogin_Load` ma nessuno ci scrive, salvo le poche
chiamate con target `File` in `ControllaFlussoIntegra`). Esiste la proprietà
`AppConfig.LogMode` (default `Ui`) che però **non viene mai consultata**.

**Decisioni da prendere** (UI / file / entrambi):

- [ ] Quale target predefinito per i messaggi dei 3 servizi (OrderGenerationService, ConsuntivoService, AdeguaIstatOrdine)?
- [ ] Onorare `AppConfig.LogMode` in `FLogin.Log`? (proposta: se `destination == Ui` e `LogMode` è `File`/`Both` → promuovi a `Both`; default `Ui` invariato)
- [ ] Cosa scrivere su file tra: messaggi operativi (Passo 1/2, contatori), `debugging` e `avvisi` (vedi punto 19 — StringBuilder popolati e mai scaricati), errori con stack
- [ ] Formato/rotazione: un file per sessione (`FatturazioneContratti_yyyyMMdd_HHmmss.log`) va bene, oppure giornaliero unico?
- [ ] La nuova LinkLabel "Apri file di log" (già aggiunta a FLogin) aprirebbe un file vuoto finché non si decide quanto sopra — ricontrollarla dopo la scelta

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

### Punto 8 — robustezza UI (suggerimento dato, non applicato)
- `Forms/FLogin.cs` `EseguiOperazione`: `Invoke` sincroni nel task in background
  esplodono se la form viene chiusa durante l'elaborazione (thread pool).
- `Program.cs`: nessun gestore globale eccezioni (`Application.ThreadException`,
  `AppDomain.UnhandledException`, `TaskScheduler.UnobservedTaskException`) e
  nessun log su crash.
- Proposta formulata: helper `PostToUi` con `IsHandleCreated/IsDisposed` +
  `BeginInvoke` in try/catch, `_inElaborazione` volatile, handler in `Program.Main`.

---

## 3. Altri punti del report originale non toccati (22-25 e minimi)

### Punto 22 — `OrderGenerationService.GeneraRigheOrdine` è un metodo di ~300 righe
- Estrarre: costruzione query/filtri, elaborazione singolo contratto, creazione righe,
  calcolo statistiche. I 4 callback + 3 `ProgressCallbacks` potrebbero diventare un
  solo record opzioni.

### Punto 23 — duplicazioni
- `CreaRigaDescrittiva` / `CreaRigaRiferimento` (`OrderGenerationService.cs`): ~35 righe
  quasi identiche → base comune.
- `SubstringSafe` (`OrderGenerationService.cs`) duplicato inline in `ConsuntivoService.cs`
  (nota `c.Nota` troncata a 32) → helper comune.
- Creazione dei `ProgressCallbacks` ripetuta 3 volte in `Forms/FLogin.cs` → factory.

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
4. **Allineamento Fatture** — non toccato.

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

Build: 0 errori (warning ~1820, per lo più attesi sui model generati).
