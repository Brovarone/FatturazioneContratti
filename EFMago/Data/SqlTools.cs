using System;
using EFMago.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FatturazioneContratti.Data
{
    public static class SqlTools
    {
        // Provider delle opzioni: l'ospite (WinForms/AppConfig, sito/appsettings) registra una
        // fabbrica che legge i valori CORRENTI a ogni chiamata, così le modifiche in runtime
        // (es. cambio DB di test da FSettings) si riflettono senza re-Configurare.
        private static Func<EfmagoOptions>? _provider;

        /// <summary>
        /// Registra la sorgente delle opzioni di connessione/behavior. Da chiamare all'avvio
        /// dell'applicazione (per il WinForms: dentro AppConfig.Load/Save).
        /// </summary>
        public static void Configure(Func<EfmagoOptions> provider)
            => _provider = provider ?? throw new ArgumentNullException(nameof(provider));

        /// <summary>Sovraccarico con opzioni fisse (utile nei test).</summary>
        public static void Configure(EfmagoOptions options)
            => Configure(() => options ?? throw new ArgumentNullException(nameof(options)));

        /// <summary>
        /// Opzioni correnti. Solleva se Configure non è stato chiamato: meglio un errore chiaro
        /// all'avvio che una connection string vuota al primo accesso al DB.
        /// </summary>
        public static EfmagoOptions Options
            => _provider?.Invoke()
               ?? throw new InvalidOperationException(
                   "SqlTools.Configure() non chiamato: l'applicazione ospitante deve registrare " +
                   "le EfmagoOptions prima di usare la libreria EFMago.");

        public static string BuildConnectionString(string databaseName)
        {
            var opts = Options;
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = opts.SqlServer,
                InitialCatalog = databaseName,
                UserID = opts.UserId,
                Password = opts.Password,
                TrustServerCertificate = true,
                Pooling = true,
                ConnectTimeout = 30
            };
            return builder.ConnectionString;
        }

        public static string GetCurrentConnectionString() => BuildConnectionString(Options.DatabaseInUse);

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
            var options = Options;
            var optionsBuilder = new DbContextOptionsBuilder<FattureContext>();
            optionsBuilder.UseSqlServer(GetCurrentConnectionString());

            if (options.IsDebugging)
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
