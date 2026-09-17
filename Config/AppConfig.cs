using System;
using System.IO;
using System.Text.Json;

namespace FatturazioneContratti.Config
{
    public static class AppConfig
    {
        private static readonly string configNameFile = "FatturazioneContatti-settings.json";
        // Proprietà usate in tutta l'applicazione
        public static string SqlServer { get; set; } = "";
        public static string UserId { get; set; } = "sa";
        public static string Password { get; set; } = "";
        public static int LoginId { get; set; } = 1;
        public static bool IsDbUno { get; set; } = true;
        public static bool IsTestDb { get; set; } = false;
        public static bool IsDebugging { get; set; } = false;

        // Nomi dei Database (ex mDATABASE, mDATABASE_SPA, ecc.)
        public static string DatabaseUno { get; set; } = "";
        public static string DatabaseSpa { get; set; } = "";
        public static string TmpDatabaseUno { get; set; } = "";
        public static string TmpDatabaseSpa { get; set; } = "";

        // Percorsi cartelle
        public static string Path { get; set; } = "";
        public static string FoxPath { get; set; } = "";
        public static string LastLogPath { get; set; } = "";
        public static string PathPaghe { get; set; } = "";

        // Proprietà dinamica che restituisce il DB corretto in base ai radio/check
        public static string DatabaseInUse
        {
            get
            {
                if (IsTestDb)
                    return IsDbUno ? TmpDatabaseUno : TmpDatabaseSpa;

                return IsDbUno ? DatabaseUno : DatabaseSpa;
            }
        }

        private static readonly string FolderPath = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Brovarone Cristiano"
        );
        private static readonly string FilePath = System.IO.Path.Combine(FolderPath, configNameFile);

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        // METODO LOAD
        public static void Load()
        {
            try
            {
                if (!File.Exists(FilePath))
                {
                    Save(); // Crea il file iniziale se non esiste
                    return;
                }

                string json = File.ReadAllText(FilePath);
                var data = JsonSerializer.Deserialize<ConfigData>(json);

                if (data != null)
                {
                    SqlServer = data.SqlServer ?? "";
                    UserId = data.UserId ?? "sa";
                    Password = data.Password ?? "";
                    LoginId = data.LoginId;
                    IsDbUno = data.IsDbUno;
                    IsTestDb = data.IsTestDb;
                    IsDebugging = data.IsDebugging;

                    DatabaseUno = data.DatabaseUno ?? DatabaseUno;
                    DatabaseSpa = data.DatabaseSpa ?? DatabaseSpa;
                    TmpDatabaseUno = data.TmpDatabaseUno ?? TmpDatabaseUno;
                    TmpDatabaseSpa = data.TmpDatabaseSpa ?? TmpDatabaseSpa;

                    Path = data.Path ?? Path;
                    FoxPath = data.FoxPath ?? FoxPath;
                    LastLogPath = data.LastLogPath ?? LastLogPath;
                }
            }
            catch
            {
                // Gestione fallback / default
            }
        }

        // METODO SAVE
        public static void Save()
        {
            try
            {
                var data = new ConfigData
                {
                    SqlServer = SqlServer,
                    UserId = UserId,
                    Password = Password,
                    LoginId = LoginId,
                    IsDbUno = IsDbUno,
                    IsTestDb = IsTestDb,
                    IsDebugging = IsDebugging,
                    DatabaseUno = DatabaseUno,
                    DatabaseSpa = DatabaseSpa,
                    TmpDatabaseUno = TmpDatabaseUno,
                    TmpDatabaseSpa = TmpDatabaseSpa,
                    Path = Path,
                    FoxPath = FoxPath,
                    LastLogPath = LastLogPath
                };

                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                string json = JsonSerializer.Serialize(data, JsonOptions);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                // Log o gestione errore scrittura
                System.Diagnostics.Debug.WriteLine($"Errore Save {configNameFile}: {ex.Message}");
            }
        }

        // DTO privato: esiste solo qui dentro per far felice JsonSerializer, nessuno fuori lo vede
        private class ConfigData
        {
            public string SqlServer { get; set; }
            public string UserId { get; set; }
            public string Password { get; set; }
            public int LoginId { get; set; }
            public bool IsDbUno { get; set; }
            public bool IsTestDb { get; set; }
            public bool IsDebugging { get; set; }
            public string DatabaseUno { get; set; }
            public string DatabaseSpa { get; set; }
            public string TmpDatabaseUno { get; set; }
            public string TmpDatabaseSpa { get; set; }
            public string Path { get; set; }
            public string FoxPath { get; set; }
            public string LastLogPath { get; set; }
        }
        // UTILITY
        public static void ApriFileJson()
        {
            if (File.Exists(FilePath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = FilePath,
                    UseShellExecute = true
                });
            }
        }

        public static void ApriCartellaImpostazioni()
        {
            if (Directory.Exists(FolderPath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = FolderPath,
                    UseShellExecute = true
                });
            }
        }

        public static void ApriCartellaLog()
        {
            if (!string.IsNullOrWhiteSpace(LastLogPath) && Directory.Exists(LastLogPath))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = LastLogPath,
                    UseShellExecute = true
                });
            }
        }
    }
}