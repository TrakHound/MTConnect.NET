using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MTConnect.Logging
{
    public static class LogReporter
    {
        #region Private Methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetPlacement(
            string sourceFilePath,
            int sourceLineNumber)
        {
            return $"[{Path.GetFileNameWithoutExtension(sourceFilePath)}, {sourceLineNumber}]";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string Now()
        {
            return DateTime.Now.ToString("hh:mm:ss.ffffff");
        }

        #endregion

        public static async Task<T> MeasureAndReport<T>(
            Func<Task<T>> operation,
            string operationName,
            ILogger logger,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await operation();
            stopwatch.Stop();

            if (logger?.IsEnabled(LogLevel.Trace) == true)
            {
                logger?.LogTrace(
                    $"{GetPlacement(sourceFilePath, sourceLineNumber)} {Now()} {operationName} in {stopwatch.ElapsedMilliseconds} ms.");
            }

            return result;
        }

        public static void MeasureAndReport(
            Action operation,
            string operationName,
            ILogger logger,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var stopwatch = Stopwatch.StartNew();
            operation();
            stopwatch.Stop();

            if (logger?.IsEnabled(LogLevel.Trace) == true)
            {
                logger?.LogTrace($"{GetPlacement(sourceFilePath, sourceLineNumber)} {Now()} {operationName} in {stopwatch.ElapsedMilliseconds} ms.");
            }
        }

        public static void Report(
            string report,
            ILogger logger,
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (logger?.IsEnabled(LogLevel.Trace) == true)
            {
                logger?.LogTrace($"{GetPlacement(sourceFilePath, sourceLineNumber)} {Now()} {report}");
            }
        }

    }
}
