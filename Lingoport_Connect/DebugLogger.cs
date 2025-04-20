using System;
using System.IO;

namespace Lingoport.LocalyzerConnect
{
    public static class DebugLogger
    {
        private static readonly string logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Trados",
            "Trados Studio",
            "Studio18",
            "logs"
        );

        private static readonly string logPath = Path.Combine(logDir, "LocalyzerPreview.log");

        public static void Log(string message)
        {
            try
            {
                Directory.CreateDirectory(logDir);
                File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
            catch
            {
                // Silently fail to avoid interrupting Studio
            }
        }
    }
}
