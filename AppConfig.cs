using System;
using System.IO;

namespace FatturazioneContratti.Config
{
    public static class AppConfig
    {
        // Connessioni & Database
        public static string SqlServer { get; set; } = @".\SQLEXPRESS";
        public static string DatabaseUno { get; set; } = "ALLSYSTEM";
        public static string DatabaseSpa { get; set; } = "ALLSYSTEM_SPA";
        public static string DatabaseTmpUno { get; set; } = "ALLSYSTEM_TEST";
        public static string DatabaseTmpSpa { get; set; } = "ALLSYSTEM_SPA_TEST";
        public static string UserId { get; set; } = "sa";
        public static string Password { get; set; } = string.Empty;
        public static int LoginId { get; set; } = 1;

        public static bool IsTestDb { get; set; } = false;
        public static bool IsDbUno { get; set; } = true;
        public static bool IsDebugging { get; set; } = false;

        public static string DatabaseInUse => IsDbUno
            ? (IsTestDb ? DatabaseTmpUno : DatabaseUno)
            : (IsTestDb ? DatabaseTmpSpa : DatabaseSpa);

        // Cartelle e Log
        public static string LogFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "FatturazioneContratti",
            "Logs");
    }


}