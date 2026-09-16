using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;

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
        /// <summary>
        /// Esegue un blocco di operazioni bulk all'interno di una transazione dedicata
        /// con ottimizzazione SQL Server DBCC TRACEON(610) e TRACEOFF garantito in finally.
        /// </summary>
        public static bool ExecuteInBulkTransaction(
            DbContext context,
            Action executeActions,
            Action<string> logMessage = null,
            StringBuilder logBuilder = null)
        {
            using var bulkTrans = context.Database.BeginTransaction();
            bool traceOnExecuted = false;

            try
            {
                context.Database.ExecuteSqlRaw("DBCC TRACEON(610)");
                traceOnExecuted = true;

                // Esecuzione delle operazioni bulk passate dal chiamante
                executeActions();

                bulkTrans.Commit();
                return true;
            }
            catch (Exception ex)
            {
                try
                {
                    bulkTrans.Rollback();
                }
                catch (Exception rbEx)
                {
                    Debug.WriteLine($"[Rollback Fallito]: {rbEx.Message}");
                }

                logBuilder?.AppendLine($"[Bulk Transaction Error]: {ex.Message}");
                logBuilder?.AppendLine(ex.StackTrace);
                logMessage?.Invoke($"Rollback eseguito a causa di un errore: {ex.Message}");
                return false;
            }
            finally
            {
                if (traceOnExecuted)
                {
                    try
                    {
                        // Evita di sollevare eccezioni che maschererebbero l'errore originario del blocco try
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
        /// Esegue il Bulk Insert/Update/InsertOrUpdate configurando batching e tracking statistiche standard.
        /// </summary>
        public static void BulkExecuteList<T>(
            DbContext context,
            IList<T> entityList,
            string tableName,
            BulkOperation operation,
            string stepDescription = "",
            StringBuilder logBuilder = null) where T : class
        {
            if (entityList == null || !entityList.Any()) return;

            int count = entityList.Count;
            var bulkConfig = new BulkConfig
            {
                SqlBulkCopyOptions = SqlBulkCopyOptions.KeepNulls,
                BulkCopyTimeout = 0,
                CalculateStats = true,
                BatchSize = count < 5000 ? 0 : count / 10,
                NotifyAfter = count / 10
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

            string msg = $"{tableName} ({stepDescription}) Ins:{bulkConfig.StatsInfo?.StatsNumberInserted} Agg:{bulkConfig.StatsInfo?.StatsNumberUpdated}";
            logBuilder?.AppendLine(msg);
            Debug.WriteLine(msg);
        }
    }
}