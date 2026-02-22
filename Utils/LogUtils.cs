// File: Utils/LogUtils.cs
// Shared version 0.3.3
// Purpose:
// - WarnOnce: prevents repeated WARN spam
// - TryLog: lazy message construction inside try/catch
// - Popup-safe: does NOT attach Exception objects at Warn level (can surface in-game popups)
// - Optional: attach Exception only at Error level

namespace CS2HonuShared
{
    using Colossal.Logging;
    using System;
    using System.Collections.Generic;

    public static class LogUtils
    {
        private static readonly object s_WarnOnceLock = new object();

        private static readonly HashSet<string> s_WarnOnceKeys =
            new HashSet<string>(StringComparer.Ordinal);

        private const int MaxWarnOnceKeys = 2048;

        public static bool WarnOnce(ILog log, string key, Func<string> messageFactory, Exception? exception = null)
        {
            if (log == null || string.IsNullOrEmpty(key) || messageFactory == null)
            {
                return false;
            }

            if (!log.isLevelEnabled(Level.Warn))
            {
                return false;
            }

            string fullKey = log.name + "|" + key;

            lock (s_WarnOnceLock)
            {
                if (s_WarnOnceKeys.Count >= MaxWarnOnceKeys)
                {
                    s_WarnOnceKeys.Clear();
                }

                if (!s_WarnOnceKeys.Add(fullKey))
                {
                    return false;
                }
            }

            TryLog(log, Level.Warn, messageFactory, exception);
            return true;
        }

        public static void TryLog(ILog log, Level level, Func<string> messageFactory, Exception? exception = null)
        {
            if (log == null || messageFactory == null)
            {
                return;
            }

            if (!log.isLevelEnabled(level))
            {
                return;
            }

            string message;

            try
            {
                message = messageFactory() ?? string.Empty;
            }
            catch (Exception ex)
            {
                SafeLogNoException(log, Level.Warn, "Log message factory threw: " + ex.GetType().Name + ": " + ex.Message);
                return;
            }

            try
            {
                Exception? attach = (exception != null && level == Level.Error) ? exception : null;
                log.Log(level, message, attach!);
            }
            catch
            {
            }
        }

        private static void SafeLogNoException(ILog log, Level level, string message)
        {
            try
            {
                if (log != null && log.isLevelEnabled(level))
                {
                    log.Log(level, message, null!);
                }
            }
            catch
            {
            }
        }
    }
}
