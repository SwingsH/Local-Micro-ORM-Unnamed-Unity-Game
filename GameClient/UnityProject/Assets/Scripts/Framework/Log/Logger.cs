using System;
using System.Collections.Generic;
using UberLogger;

namespace Tizsoft.Log
{
    /// <summary>
    /// Log 工具方法集合，方便 log 用。
    /// </summary>
    public class Logger
    {
        readonly Dictionary<LogLevel, bool> logSwitches = new Dictionary<LogLevel, bool>
        {
            { LogLevel.Trace, true },
            { LogLevel.Debug, true },
            { LogLevel.Info, true },
            { LogLevel.Warn, true },
            { LogLevel.Error, true },
            { LogLevel.Fatal, true }
        };

        public string Name { get; private set; }

        public Logger(string name)
        {
            Name = name;
        }


        #region Enable/Disable

        public bool IsTraceEnabled
        {
            get
            {
                return IsEnabled(LogLevel.Trace);
            }
            set
            {
                SetEnabled(LogLevel.Trace, value);
            }
        }

        public bool IsDebugEnabled
        {
            get
            {
                return IsEnabled(LogLevel.Debug);
            }
            set
            {
                SetEnabled(LogLevel.Debug, value);
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return IsEnabled(LogLevel.Info);
            }
            set
            {
                SetEnabled(LogLevel.Info, value);
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return IsEnabled(LogLevel.Warn);
            }
            set
            {
                SetEnabled(LogLevel.Warn, value);
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return IsEnabled(LogLevel.Error);
            }
            set
            {
                SetEnabled(LogLevel.Error, value);
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return IsEnabled(LogLevel.Fatal);
            }
            set
            {
                SetEnabled(LogLevel.Fatal, value);
            }
        }

        /// <summary>
        /// 取得 <paramref name="level"/> 是否停用此 logger 的 log。
        /// 如果 <see cref="LogManager"/> 已經設定停用，則以 <see cref="LogManager"/> 為基準。
        /// </summary>
        /// <returns><c>true</c>, if enabled, <c>false</c> otherwise.</returns>
        /// <param name="level">Log level.</param>
        public bool IsEnabled(LogLevel level)
        {
            bool isEnabeld;
            logSwitches.TryGetValue(level, out isEnabeld);
            return isEnabeld;
        }

        /// <summary>
        /// 設定 <paramref name="level"/> 是否要停用此 logger 的 log。
        /// 如果 <see cref="LogManager"/> 已經設定停用，則以 <see cref="LogManager"/> 為基準。
        /// </summary>
        /// <param name="level">Log level.</param>
        /// <param name="isEnabled">If set to <c>true</c> is enabled.</param>
        public void SetEnabled(LogLevel level, bool isEnabled)
        {
            if (logSwitches.ContainsKey(level))
            {
                logSwitches[level] = isEnabled;
            }
        }

        #endregion Enable/Disable


        #region Log

        [StackTraceIgnore]
        public void Log<T>(LogLevel level, T value)
        {
            Log(level, null, null, "{0}", value);
        }

        [StackTraceIgnore]
        public void Log(LogLevel level, string message)
        {
            Log(level, null, null, message, null);
        }

        [StackTraceIgnore]
        public void Log(LogLevel level, string format, params object[] args)
        {
            Log(level, null, null, format, args);
        }

        [StackTraceIgnore]
        public void Log(LogLevel level, Exception exception, string message)
        {
            Log(level, null, exception, message, null);
        }

        [StackTraceIgnore]
        public void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            Log(level, null, exception, format, args);
        }

        [StackTraceIgnore]
        public void Log(LogLevel level, UnityEngine.Object context, string message)
        {
            Log(level, context, null, message, null);
        }

        [StackTraceIgnore]
        public void Log(LogLevel level, UnityEngine.Object context, string format, params object[] args)
        {
            Log(level, context, null, format, args);
        }

        [StackTraceIgnore]
        public void Log(LogLevel level, UnityEngine.Object context, Exception exception, string message)
        {
            Log(level, context, exception, message, null);
        }

        [StackTraceIgnore]
        public void Log(LogLevel level, UnityEngine.Object context, Exception exception, string format, params object[] args)
        {
            Log(new LogEventInfo
            {
                Name = Name,
                LogLevel = level,
                CustomData = context,
                Exception = exception,
                MessageOrFormat = format,
                FormatArguments = args
            });
        }

        [StackTraceIgnore]
        public void Log(LogEventInfo logEventInfo)
        {
            if (LogManager.Default.IsEnabled(logEventInfo.LogLevel) && IsEnabled(logEventInfo.LogLevel))
            {
                LoggerImpl.Log(logEventInfo);
            }
        }

        #endregion Log


        #region Trace

        [StackTraceIgnore]
        public void Trace<T>(T value)
        {
            Log(LogLevel.Trace, value);
        }

        [StackTraceIgnore]
        public void Trace(string message)
        {
            Log(LogLevel.Trace, message);
        }

        [StackTraceIgnore]
        public void Trace(string format, params object[] args)
        {
            Log(LogLevel.Trace, format, args);
        }

        [StackTraceIgnore]
        public void Trace(Exception exception, string message)
        {
            Log(LogLevel.Trace, exception, message);
        }

        [StackTraceIgnore]
        public void Trace(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Trace, exception, format, args);
        }


        [StackTraceIgnore]
        public void Trace(UnityEngine.Object context, string message)
        {
            Log(LogLevel.Trace, context, message);
        }

        [StackTraceIgnore]
        public void Trace(UnityEngine.Object context, string format, params object[] args)
        {
            Log(LogLevel.Trace, context, format, args);
        }

        [StackTraceIgnore]
        public void Trace(UnityEngine.Object context, Exception exception, string message)
        {
            Log(LogLevel.Trace, context, exception, message);
        }

        [StackTraceIgnore]
        public void Trace(UnityEngine.Object context, Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Trace, context, exception, format, args);
        }

        #endregion Trace


        #region Debug

        [StackTraceIgnore]
        public void Debug<T>(T value)
        {
            Log(LogLevel.Debug, value);
        }

        [StackTraceIgnore]
        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        [StackTraceIgnore]
        public void Debug(string format, params object[] args)
        {
            Log(LogLevel.Debug, format, args);
        }

        [StackTraceIgnore]
        public void Debug(Exception exception, string message)
        {
            Log(LogLevel.Debug, exception, message);
        }

        [StackTraceIgnore]
        public void Debug(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Debug, exception, format, args);
        }

        [StackTraceIgnore]
        public void Debug(UnityEngine.Object context, string message)
        {
            Log(LogLevel.Debug, context, message);
        }

        [StackTraceIgnore]
        public void Debug(UnityEngine.Object context, string format, params object[] args)
        {
            Log(LogLevel.Debug, context, format, args);
        }

        [StackTraceIgnore]
        public void Debug(UnityEngine.Object context, Exception exception, string message)
        {
            Log(LogLevel.Debug, context, exception, message);
        }

        [StackTraceIgnore]
        public void Debug(UnityEngine.Object context, Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Debug, context, exception, format, args);
        }

        #endregion Debug


        #region Info

        [StackTraceIgnore]
        public void Info<T>(T value)
        {
            Log(LogLevel.Debug, value);
        }

        [StackTraceIgnore]
        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        [StackTraceIgnore]
        public void Info(string format, params object[] args)
        {
            Log(LogLevel.Info, format, args);
        }

        [StackTraceIgnore]
        public void Info(Exception exception, string message)
        {
            Log(LogLevel.Info, exception, message);
        }

        [StackTraceIgnore]
        public void Info(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Info, exception, format, args);
        }

        [StackTraceIgnore]
        public void Info(UnityEngine.Object context, string message)
        {
            Log(LogLevel.Info, context, message);
        }

        [StackTraceIgnore]
        public void Info(UnityEngine.Object context, string format, params object[] args)
        {
            Log(LogLevel.Info, context, format, args);
        }

        [StackTraceIgnore]
        public void Info(UnityEngine.Object context, Exception exception, string message)
        {
            Log(LogLevel.Info, context, exception, message);
        }

        [StackTraceIgnore]
        public void Info(UnityEngine.Object context, Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Info, context, exception, format, args);
        }

        #endregion Info


        #region Warn

        [StackTraceIgnore]
        public void Warn<T>(T value)
        {
            Log(LogLevel.Warn, value);
        }

        [StackTraceIgnore]
        public void Warn(string message)
        {
            Log(LogLevel.Warn, message);
        }

        [StackTraceIgnore]
        public void Warn(string format, params object[] args)
        {
            Log(LogLevel.Warn, format, args);
        }

        [StackTraceIgnore]
        public void Warn(Exception exception, string message)
        {
            Log(LogLevel.Warn, exception, message);
        }

        [StackTraceIgnore]
        public void Warn(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Warn, exception, format, args);
        }

        [StackTraceIgnore]
        public void Warn(UnityEngine.Object context, string message)
        {
            Log(LogLevel.Warn, context, message);
        }

        [StackTraceIgnore]
        public void Warn(UnityEngine.Object context, string format, params object[] args)
        {
            Log(LogLevel.Warn, context, format, args);
        }

        [StackTraceIgnore]
        public void Warn(UnityEngine.Object context, Exception exception, string message)
        {
            Log(LogLevel.Warn, context, exception, message);
        }

        [StackTraceIgnore]
        public void Warn(UnityEngine.Object context, Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Warn, context, exception, format, args);
        }

        #endregion Warn


        #region Error

        [StackTraceIgnore]
        public void Error<T>(T value)
        {
            Log(LogLevel.Error, value);
        }

        [StackTraceIgnore]
        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        [StackTraceIgnore]
        public void Error(string format, params object[] args)
        {
            Log(LogLevel.Error, format, args);
        }

        [StackTraceIgnore]
        public void Error(Exception exception, string message)
        {
            Log(LogLevel.Error, exception, message);
        }

        [StackTraceIgnore]
        public void Error(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Error, exception, format, args);
        }

        [StackTraceIgnore]
        public void Error(UnityEngine.Object context, string message)
        {
            Log(LogLevel.Error, context, message);
        }

        [StackTraceIgnore]
        public void Error(UnityEngine.Object context, string format, params object[] args)
        {
            Log(LogLevel.Error, context, format, args);
        }

        [StackTraceIgnore]
        public void Error(UnityEngine.Object context, Exception exception, string message)
        {
            Log(LogLevel.Error, context, exception, message);
        }

        [StackTraceIgnore]
        public void Error(UnityEngine.Object context, Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Error, context, exception, format, args);
        }

        #endregion Warn


        #region Fatal

        [StackTraceIgnore]
        public void Fatal<T>(T value)
        {
            Log(LogLevel.Fatal, value);
        }

        [StackTraceIgnore]
        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        [StackTraceIgnore]
        public void Fatal(string format, params object[] args)
        {
            Log(LogLevel.Fatal, format, args);
        }

        [StackTraceIgnore]
        public void Fatal(Exception exception, string message)
        {
            Log(LogLevel.Fatal, exception, message);
        }

        [StackTraceIgnore]
        public void Fatal(Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Fatal, exception, format, args);
        }

        [StackTraceIgnore]
        public void Fatal(UnityEngine.Object context, string message)
        {
            Log(LogLevel.Fatal, context, message);
        }

        [StackTraceIgnore]
        public void Fatal(UnityEngine.Object context, string format, params object[] args)
        {
            Log(LogLevel.Fatal, context, format, args);
        }

        [StackTraceIgnore]
        public void Fatal(UnityEngine.Object context, Exception exception, string message)
        {
            Log(LogLevel.Fatal, context, exception, message);
        }

        [StackTraceIgnore]
        public void Fatal(UnityEngine.Object context, Exception exception, string format, params object[] args)
        {
            Log(LogLevel.Fatal, context, exception, format, args);
        }

        #endregion Fatal
    }
}
