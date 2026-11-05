# TODO — FatturazioneContratti (agg. 22/10/2026)

Solo **lavoro aperto**. Le fix già applicate non sono più elencate qui: la storia è
in git, le regole di progetto (log, consuntivo, performance) sono in `AGENTS.md`.

---

## 0. Riferimento fattura consuntivo (`MChiaveFattura` / `Packing`) — da eseguire e verificare

- [ ] Eseguire `sql/2026-10-22_Consuntivo_ChiaveFattura.sql` su TMPSPA, poi su ALLSPA/DEMON
      (colonna `Integra_Interventi.MChiaveFattura`, indici, vista `ALLVInterventiFattura`).
      Senza la colonna l'app non può interrogare `IntegraInterventi`.
- [ ] Run consuntivo su TMPSPA con Controllo + Fatturazione: ogni riga servizio deve avere
      `Packing = <chiave>`, e gli interventi del periodo la stessa `MChiaveFattura`.
- [ ] Giro proforma in Mago (fattura → proforma → fattura): la chiave deve restare su `Packing`
      e la vista `ALLVInterventiFattura` deve passare da FATTURA a PROFORMA e ritorno.
- [ ] `Riassegna`: verificare che ricalcoli la chiave e che il log segnali quante chiavi sono cambiate.
- [ ] Verificare che `Packing` non venga azzerato da Mago al salvataggio/riapertura del documento.

---

## 1. Verifica manuale su DB test (unico debito di verifica aperto)

Nessun test automatico: da provare via UI con flag Test DB attivo.

**Workflow**
1. **Genera Canoni** — timestamp unificati, include attività di testa nel loader,
   ordine con più contratti, riga bianca singola, attività da rifatturare,
   scadenza fissa/cessazione, >100 ordini (barre a blocchi).
2. **Adeguamento ISTAT** — chunking ID-first + iterator, prototipo attività
   (`AddNew`), `curLastLineAtt` (sospensioni/attività di testa).
3. **Consuntivazione** (la più a rischio) — raggruppamento fatture, ID documento
   assegnato solo ai gruppi con righe, cross-ref/ritenuta solo con righe,
   `LastSubId` via `GroupBy`, regola cessazione, checkbox "Prosegui comunque".

**Log** (per ogni workflow)
- `LogMode = Entrambi`: il file contiene filtri della run, conteggi, dettaglio
  anomalie (una sola volta) ed errori con stack.
- `LogMode = File`: UI leggibile, ma errori/anomalie/rollback compaiono comunque a video.
- `LogMode = UI`: informativi non vanno a file; errori e dettaglio anomalie sì.
- `IsDebugging = true`: a fine workflow nel file compare il tracciato `debugging`.
- Barre consuntivo: progress1 = blocchi da 100, progress2 = record del blocco, `Finish` finale.
- Pulsante **Interrompi**: esito "INTERROTTO" (arancio) e, se il token arriva con
  transazione bulk aperta, rollback esplicito.

---

## 2. `continue` su attività già fatturata salta i controlli successivi

- `EFMago/Orders/GenerazioneRigheOrdiniService.cs:602` (blocco Sospensione) e
  `:642` (blocco ISTAT)
- `EFMago/Orders/GenerazioneRigheOrdiniService.Istat.cs:222` (blocco Sospensione)

Il `continue` esce dal `foreach` delle attività invece di chiudere solo il ramo:
un'attività **Sospensione + Annullamento/Istat insieme** e già fatturata non viene
valutata sul secondo ramo (il `:642` è l'ultimo blocco, quindi lì è innocuo).

- Da verificare sui dati reali se l'accostamento esiste davvero.
- Fix probabile: `if/else if` espliciti (o `continue` solo dentro il ramo interessato)
  al posto del `continue` sul flusso.

---

## 3. Warning nullable rilevanti (dal build)

- `EFMago/Invoicing/ConsuntivoService.cs` — `Break(..., MaCustSupp customer, ...)`
  può ricevere null (CS8604) → annotare le firme.
- `MiniDistinta` (stesso file) — proprietà non inizializzate (CS8618).
- `EFMago/Orders/GenerazioneRigheOrdiniService.cs` — CS8604 su
  `EseguiSalvataggioTransazionale` (`logMessage` null-passabile).

---

## 4. Sito web ordini (Blazor Server) — fasi residue

Libreria `EFMago` e `CreazioneOrdiniService` sono pronti e verificati su TMPSPA
(harness `tools/CreazioneOrdiniSmoke`); l'ordine di prova è rimasto in TMPSPA.

- [ ] **C** — nuovo repo sito Blazor Server (login applicativo, form ordine);
      referenziare `EFMago` via path/git; `appsettings.json` → `SqlTools.Configure`.
      Workload ASP.NET in VS 2022 da installare (~2 GB).
- [ ] **E** — deploy IIS su server Biella (HTTPS).

Note: niente health/schema check a runtime (`ModelManifest` differito);
istanziazione manuale dei servizi (niente DI nella libreria); stesso pattern
contatori/audit.
