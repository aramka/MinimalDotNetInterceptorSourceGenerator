using System;

namespace MinimalInterceptorSourceGenerator.SharedLibrary
{
    using System;
    using System.IO;
    using System.Net.NetworkInformation;

    public static class Log
    {
        private static string logFilePath = @"SET THIS IF YOU USE LOGGING!";
        private static readonly object _lock = new();
        static bool firstTime = true;

        static Log()
        {
            // System.IO.File.WriteAllText("GeneratorLogging.txt", "Starting logging");
        }

        public static void LogMessage(string message)
        {
            if (firstTime)
            {
                firstTime = false;
                System.IO.File.WriteAllText(logFilePath, "Starting logging");
            }
            message = $"{message}{Environment.NewLine}";
            lock (_lock)
            {
                try
                {
                    File.AppendAllLines(logFilePath, [message]);
                }
                catch (Exception ex)
                {
                    File.AppendAllLines(logFilePath, [$"[-] Exception occurred in logging: {ex.Message}"]);
                }
            }
        }
    }

}
