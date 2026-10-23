/* ============================================================================
   Consuntivo: riferimento interventi -> riga fattura
   ----------------------------------------------------------------------------
   Da eseguire su ogni database Mago (produzione UNO/SPA e test) PRIMA di
   avviare la nuova versione dell'app.

   Meccanica:
   - Integra_Interventi.MChiaveFattura = chiave del gruppo fatturato,
     generata da ControllaFlussoIntegra (e ricalcolata da Riassegna):
        Id ordine|linea distinta|linea contratto|evento|mese
     es. 7553|1|2|INT|202609
   - La stessa chiave viene scritta sulla riga fattura in
     MA_SaleDocDetail.Packing: è un campo standard Mago, quindi viene
     copiato in entrambi i sensi nella copia fattura <-> proforma.
   - La vista ALLVInterventiFattura risolve il documento corrente per chiave,
     preferendo la Fattura (3407874) alla Proforma (3407888) e poi il
     documento più recente. Nessun SaleDocId viene memorizzato sugli
     interventi: cambierebbe a ogni copia.

   Nota: la creazione dell'indice su MA_SaleDocDetail.Packing può richiedere
   tempo su ALLSPA (tabella grande). Eseguire in finestra di manutenzione.
   ============================================================================ */

SET NOCOUNT ON;
GO

/* 1. Colonna chiave sugli interventi ---------------------------------------- */
IF COL_LENGTH('dbo.Integra_Interventi', 'MChiaveFattura') IS NULL
BEGIN
    ALTER TABLE dbo.Integra_Interventi
        ADD MChiaveFattura varchar(32) NULL;
END;
GO

/* 2. Valorizzo a stringa vuota ---------------------------------------------- */
UPDATE dbo.Integra_Interventi SET dbo.Integra_Interventi.MChiaveFattura = '' WHERE dbo.Integra_Interventi.MChiaveFattura IS NULL
GO

/* 3. Indici per il join della vista ----------------------------------------- */
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_Integra_Interventi_MChiaveFattura'
      AND object_id = OBJECT_ID('dbo.Integra_Interventi'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Integra_Interventi_MChiaveFattura
        ON dbo.Integra_Interventi (MChiaveFattura);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'IX_MA_SaleDocDetail_Packing'
      AND object_id = OBJECT_ID('dbo.MA_SaleDocDetail'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_MA_SaleDocDetail_Packing
        ON dbo.MA_SaleDocDetail (Packing)
        INCLUDE (SaleDocId, Line, SubId);
END;
GO

/* 4. Vista di lettura ------------------------------------------------------- */
IF OBJECT_ID('dbo.ALLVInterventiFattura', 'V') IS NOT NULL
    DROP VIEW dbo.ALLVInterventiFattura;
GO

CREATE VIEW dbo.ALLVInterventiFattura
AS
WITH Candidati AS
(
    SELECT
        i.ID,
        i.Contratto,
        i.Filiale,
        i.TipoEvento,
        i.InizioAllarme,
        i.MChiaveFattura,
        o.InternalOrdNo,
        d.SaleDocId,
        d.Line  AS RigaFattura,
        d.SubId AS SubRigaFattura,
        h.DocumentType,
        h.DocNo,
        h.DocumentDate,
        ROW_NUMBER() OVER (
            PARTITION BY i.ID
            ORDER BY
                CASE WHEN h.DocumentType = 3407874 THEN 0   -- Fattura
                     WHEN h.DocumentType = 3407888 THEN 1   -- Fattura proforma
                     ELSE 2 END,
                h.SaleDocId DESC,
                d.Line
        ) AS rn
    FROM dbo.Integra_Interventi i
    LEFT JOIN dbo.MA_SaleOrd o        ON o.SaleOrdId = i.MIdOrdCli
    LEFT JOIN dbo.MA_SaleDocDetail d  ON d.Packing = i.MChiaveFattura
    LEFT JOIN dbo.MA_SaleDoc h        ON h.SaleDocId = d.SaleDocId
    WHERE i.MChiaveFattura IS NOT NULL
      AND i.MChiaveFattura <> ''
)
SELECT
    ID,
    Contratto,
    Filiale,
    TipoEvento,
    InizioAllarme,
    MChiaveFattura,
    InternalOrdNo,
    SaleDocId,
    RigaFattura,
    SubRigaFattura,
    DocumentType,
    DocNo,
    DocumentDate,
    CASE
        WHEN SaleDocId IS NULL             THEN 'ORFANO'
        WHEN DocumentType = 3407874        THEN 'FATTURA'
        WHEN DocumentType = 3407888        THEN 'PROFORMA'
        ELSE 'ALTRO'
    END AS StatoFattura
FROM Candidati
WHERE rn = 1;
GO

/* Esempi:
   -- interventi di un documento
   SELECT * FROM dbo.ALLVInterventiFattura WHERE SaleDocId = 59262;

   -- dove è finito un intervento
   SELECT * FROM dbo.ALLVInterventiFattura WHERE ID = '<guid intervento>';

   -- interventi fatturati ma senza documento (fattura cancellata e non ricreata)
   SELECT * FROM dbo.ALLVInterventiFattura WHERE StatoFattura = 'ORFANO';
*/
