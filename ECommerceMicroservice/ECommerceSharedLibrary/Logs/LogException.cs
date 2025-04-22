using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace ECommerceSharedLibrary.Logs
{
    public static class LogException
    {
        public static void Loggers(Exception ex)
        {
            LogToFile(ex.Message);
            LogToConsole(ex.Message);
            LogToDebugger(ex.Message);
        }

        public static void LogToDebugger(string message)
        {
            Log.Debug(message);
        }

        public static void LogToConsole(string message)
        {
            Log.Warning(message);
        }

        public static void LogToFile(string message)
        {
            Log.Information(message);
        }
    }
}
