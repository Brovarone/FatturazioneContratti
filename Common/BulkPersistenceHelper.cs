using EFCore.BulkExtensions;
using FatturazioneContratti.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FatturazioneContratti.Common
{
    public enum BulkOperation
    {
        Insert,
        Update,
        InsertOrUpdate
    }

    public static class BulkPersistenceHelper
    {
        public static bool ExecuteInBulkTransaction(
            DbContext context,
            Action executeActions,
            Action<string, AppConfig.LogTarget> logMessage = null,
            StringBuilder logBuilder = null)
        {
            using var bulkTrans = context.Database.BeginTransaction();
            bool traceOnExecuted = false;

            try
            {
                context.Database.ExecuteSqlRaw("DBCC TRACEON(610)");
                traceOnExecuted = true;

                executeActions();

                bulkTrans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                try { bulkTrans.Rollback(); } catch (Exception rbEx) { Debug.WriteLine($"[Rollback Fallito]: {rbEx.Message}"); }

                logBuilder?.AppendLine($"[Bulk Transaction Error]: {ex.Message}");
                logBuilder?.AppendLine(ex.StackTrace);  
                logMessage?.Invoke($"Rollback eseguito a causa di un errore: {ex.Message}", AppConfig.LogTarget.Ui);
                return false;
            }
            finally
            {
                if (traceOnExecuted)
                {
                    try
                    {
                        if (context.Database.GetDbConnection().State == ConnectionState.Open)
                        {
                            context.Database.ExecuteSqlRaw("DBCC TRACEOFF(610)");
                        }
                    }
                    catch (Exception exTrace)
                    {
                        Debug.WriteLine($"[TRACEOFF Fallito]: {exTrace.Message}");
                    }
                }
            }
        }

        /// <summary>
        /// Esegue la bulk operation in UNA sola chiamata.
        ///
        /// - Insert: prima di scrivere, ogni proprietà "non passata" (valore = default CLR) che nel
        ///   modello ha un DEFAULT riceve quel default (vedi FattureContextExtensions.AddNew). Nessuna esclusione di colonne,
        ///   nessun raggruppamento: tutte le righe hanno sempre tutte le colonne valorizzate.
        /// - Update / InsertOrUpdate: le entità vengono scritte così come sono. I default NON vengono
        ///   applicati, perché sovrascriverebbero valori reali che per caso valgono 0 / null.
        ///   Se hai righe nuove, passale con BulkOperation.Insert.
        /// </summary>
        public static void BulkExecuteList<T>(
            DbContext context,
            IList<T> entityList,
            string tableName,
            BulkOperation operation,
            string stepDescription = "",
            StringBuilder logBuilder = null) where T : class
        {
            if (entityList == null || entityList.Count == 0) return;

            int count = entityList.Count;                       

            var bulkConfig = new BulkConfig
            {
                SqlBulkCopyOptions = (SqlBulkCopyOptions)0,
                BulkCopyTimeout = 0,
                CalculateStats = true,
                BatchSize = count < 5000 ? 0 : count / 10,
                NotifyAfter = Math.Max(1, count / 10)
            };

            switch (operation)
            {
                case BulkOperation.Insert:
                    context.BulkInsert(entityList, bulkConfig);
                    break;
                case BulkOperation.Update:
                    context.BulkUpdate(entityList, bulkConfig);
                    break;
                case BulkOperation.InsertOrUpdate:
                    context.BulkInsertOrUpdate(entityList, bulkConfig);
                    break;
            }

            string msg = $"{tableName} ({stepDescription}) Righe:{count} Ins:{bulkConfig.StatsInfo?.StatsNumberInserted} Agg:{bulkConfig.StatsInfo?.StatsNumberUpdated}";
            logBuilder?.AppendLine(msg);
            Debug.WriteLine(msg);
        }
    }
}