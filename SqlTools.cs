using System;
using System.Data;
using FatturazioneContratti.Config;
using EFMago.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FatturazioneContratti.Data
{
    public static class SqlTools
    {
        public static string BuildConnectionString(string databaseName)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = AppConfig.SqlServer,
                InitialCatalog = databaseName,
                UserID = AppConfig.UserId,
                Password = AppConfig.Password,
                TrustServerCertificate = true,
                Pooling = true,
                ConnectTimeout = 30
            };
            return builder.ConnectionString;
        }

        public static string GetCurrentConnectionString() => BuildConnectionString(AppConfig.DatabaseInUse);

        /// <summary>
        /// Crea e apre una SqlConnection diretta con ARITHABORT abilitato (necessario per indici filtrati Mago).
        /// </summary>
        public static SqlConnection OpenDirectConnection()
        {
            var conn = new SqlConnection(GetCurrentConnectionString());
            conn.Open();

            using (var cmd = new SqlCommand("SET ARITHABORT ON;", conn))
            {
                cmd.ExecuteNonQuery();
            }

            return conn;
        }

        /// <summary>
        /// Crea un'istanza di FattureContext già configurata per il database attualmente attivo.
        /// </summary>
        public static FattureContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<FattureContext>();
            optionsBuilder.UseSqlServer(GetCurrentConnectionString());

            if (AppConfig.IsDebugging)
            {
                // ATTIVA LA DIAGNOSTICA AVANZATA SULLE COLONNE
                optionsBuilder.EnableDetailedErrors(); // <-- DICE IL NOME DELLA COLONNA CHE CONTIENE IL NULL!
                optionsBuilder.EnableSensitiveDataLogging(); // <-- DICE I VALORI DEI PARAMETRI
            }             

            var ctx = new FattureContext(optionsBuilder.Options);
            ctx.Database.SetCommandTimeout(180);
            return ctx;
        }
    }
}