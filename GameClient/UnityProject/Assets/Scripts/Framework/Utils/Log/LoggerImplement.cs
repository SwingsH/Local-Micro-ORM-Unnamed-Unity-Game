using System;
using UnityEngine;
using UberLogger;
using TIZSoft.Utils;

namespace TIZSoft.Utils.Log
{
    /// <summary>
    /// Logger implemet，using UberLogger 
    /// 1. Editor console
    /// 2. In-Game Console
    /// 3. Local FIles
    /// 4. Remote Log
    /// </summary>
    static class LoggerImplement
    {
        const bool ACTIVE_INGAME_CONSOLE = true;
        const string NAME_PREFAB_INGAME_CONSOLE = "UberAppConsole";

        static private GameObject consoleGameObject = null;
        static readonly object[] EmptyArgs = new object[0];

        public static void AddFileTarget(string fullFilePath)
        {
            UberLogger.Logger.AddLogger(new FileTarget(fullFilePath), false);
        }

        [StackTraceIgnore]
        public static void Log(LogEventInfo logEventInfo)
        {
            CheckInGameConsole();

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

        public static void CheckInGameConsole()
        {
            if (ACTIVE_INGAME_CONSOLE && consoleGameObject == null)
            {
                UberLoggerAppWindow[] consoles = UnityUtils.FindObjectsOfType<UberLoggerAppWindow>();
                if (consoles.Length > 1)
                {
                    Debug.LogError("Duplicated UberLoggerAppWindow.");
                }
                if (consoles.Length > 0)
                {
                    consoleGameObject = consoles[0].gameObject;
                    return;
                }

                consoleGameObject = new GameObject(typeof(UberLoggerAppWindow).Name);
                consoleGameObject = GameObject.Instantiate(Resources.Load(NAME_PREFAB_INGAME_CONSOLE) as GameObject);
                consoleGameObject.name = consoleGameObject.name.Replace("(Clone)", string.Empty);
                consoleGameObject.hideFlags = HideFlags.DontSave;
            }
        }
    }
}
 