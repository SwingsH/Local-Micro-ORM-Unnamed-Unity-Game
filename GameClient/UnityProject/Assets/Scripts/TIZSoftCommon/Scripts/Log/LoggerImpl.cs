using System;
using UberLogger;
using UnityEngine;

namespace TIZSoft.Log
{
    /// <summary>
    /// Logger 功能實作類別，使用 UberLogger 實作。
    /// </summary>
    static class LoggerImpl
    {
        static readonly object[] EmptyArgs = new object[0];

        public static void AddFileTarget(string fullFilePath)
        {
            UberLogger.Logger.AddLogger(new FileTarget(fullFilePath), false);
        }

        [StackTraceIgnore]
        public static void Log(LogEventInfo logEventInfo)
        {
            // (channel, messageOrFormat, args)
            Action<string, string, object[]> log;

            // (context, channel, messageOrFormat, args)
            Action<UnityEngine.Object, string, string, object[]> logWithContext;

            switch (logEventInfo.LogLevel)
            {
                case LogLevel.Warn:
                    {
                        log = UberDebug.LogWarningChannel;
                        logWithContext = UberDebug.LogWarningChannel;
                        break;
                    }
                case LogLevel.Error:
                case LogLevel.Fatal:
                    {
                        log = UberDebug.LogErrorChannel;
                        logWithContext = UberDebug.LogErrorChannel;
                        break;
                    }
                default:
                    {
                        log = UberDebug.LogChannel;
                        logWithContext = UberDebug.LogChannel;
                        break;
                    }
            }

            try
            {
                var context = logEventInfo.CustomData as UnityEngine.Object;
                var channel = logEventInfo.LogLevel.ToString();
                var message = logEventInfo.ToString();

                if (context != null)
                {
                    logWithContext(context, channel, message, EmptyArgs);
                }
                else
                {
                    log(channel, message, EmptyArgs);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}
 