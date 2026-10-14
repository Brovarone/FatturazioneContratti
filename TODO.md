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

### Punto 19 — log diagnostici mai emessi
- `debugging` e `avvisi` (StringBuilder) popolati in tutti i workflow e mai
  scaricati su log/UI. Esempi di messaggi persi:
  - `OrderGenerationService.cs`: "data prossima fatturazione antecedente...", tutte le `[ESCLUSA]`
  - `OrderGenerationService.Istat.cs`: "riga di adeguamento Istat già presente per l'anno...", "scadenza fissa. Controllare canoni!"
- Da concordare con le decisioni del punto 1 (questa sessione).

---

## 3. Altri punti del report originale non toccati (22-25 e minimi)

### Punto 22 — `OrderGenerationService.GeneraRigheOrdine` era un metodo di ~300 righe (COMPLETATO)
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
- `OrderGenerationService.cs`: warning CS8604 su `EseguiSalvataggioTransazionale`
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
| 6 | Causale attività ISTAT dalla UI (`TxtIstatAttivita` → `FiltriOrdini.CausaleAttivita`) con fallback `CausaleAttivitaIstatDefault = "ISTAT"` | `Orders/OrderGenerationService.Istat.cs`, `Forms/FAskFiltriOrdini.cs`, `Orders/FiltriOrdini.cs` |
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
| 8 | `PostToUi` + `_inElaborazione` volatile; handler globali eccezioni + `CrashLogger` | `Forms/FLogin.cs`, `Program.cs`, `Common/CrashLogger.cs` |
| 23a | `CreaRigaDescrittiva`/`CreaRigaRiferimento` unificate in `CreaRigaNota` | `Orders/OrderGenerationService.cs` |
| 23c | `SubstringSafe` estratto in `Common/StringHelpers` (rimosso privato + ternario inline in consuntivo) | `Common/StringHelpers.cs`, `Orders/OrderGenerationService.cs`, `Invoicing/ConsuntivoService.cs` |
| 23b | `ProgressCallbacks` centralizzati in factory `FLogin.CreaProgress` | `Forms/FLogin.cs` |
| A4 | Allineamento: nuova dialog `FAskFiltriAllineamentoFatture` (date da/a + radio "Tutti i numeri" / "Intervallo primo-ultimo"); firma `AdeguaFattureDaOrdini` ora riceve `FiltriAllineamentoFatture` (`AllNumbers`, `NrFirst`, `NrLast`) | `Invoicing/FattureDaOrdiniService.cs`, `Invoicing/FiltriAllineamentoFatture.cs`, `Forms/FAskFiltriAllineamentoFatture*.cs`, `Forms/FLogin.cs` |
| 25 | Rimossi assegnamenti ridondanti (0/false) dai costruttori, default in inizializzatori di proprietà | `Orders/CurOrdRow.cs`, `Orders/CurOrd.cs`, `Invoicing/CurFatt.cs`, `Invoicing/CurFattRow.cs` |
| 25b | `SingoloOrdine`/`NrMese` cablati nelle query consuntivo (+ `NrOrdine` in DTO e UI) | `Invoicing/ConsuntivoService.cs`, `Invoicing/FiltriOrdiniConsuntivo.cs`, `Forms/FAskFiltriOrdiniConsuntivo*.cs` |
| 22 | `GeneraRigheOrdine` scomposto: filtri/ID/default condivisi, `StatoElaborazioneOrdini`+`StatoOrdine`, `ElaboraOrdine`/`ElaboraContratto`, iterator a blocchi, `record GeneraRigheOrdineOptions`, rimosso `progress3` | `Orders/OrderGenerationService*.cs`, `Orders/DefaultVenditeIva.cs`, `Orders/GeneraRigheOrdineOptions.cs`, `Invoicing/ConsuntivoService.cs`, `Forms/FLogin.cs` |

> Le righe L1, L2, 8, 22, 23a, 23b, 23c, A4 sono state **committate** il 13-14/10/2026
> (commit su `master` + branch `feature/refactor-genera-righe-ordine`) — resta la verifica
> manuale §4.

Build: 0 errori (warning ~1820, per lo più attesi sui model generati).
