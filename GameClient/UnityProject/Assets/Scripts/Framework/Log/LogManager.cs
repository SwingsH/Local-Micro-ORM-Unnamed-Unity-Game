using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace TIZSoft.Log
{
    public class LogManager
    {
        static LogManager defaultInstance;

        public static LogManager Default
        {
            get
            {
                if (defaultInstance != null)
                {
                    return defaultInstance;
                }

                defaultInstance = new LogManager();
                return defaultInstance;
            }
        }

        readonly Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();
        readonly Dictionary<LogLevel, bool> logSwitches = new Dictionary<LogLevel, bool>
        {
            { LogLevel.Trace, true },
            { LogLevel.Debug, true },
            { LogLevel.Info, true },
            { LogLevel.Warn, true },
            { LogLevel.Error, true },
            { LogLevel.Fatal, true }
        };

        LogConfig config;

        // 暫不開放自行建立實體，請改用 LogManager.Default。
        LogManager()
        {
        }

        public LogConfig Config
        {
            get
            {
                return config;
            }
            set
            {
                IsTraceEnabled = value.IsTraceEnabled;
                IsDebugEnabled = value.IsDebugEnabled;
                IsInfoEnabled = value.IsInfoEnabled;
                IsWarnEnabled = value.IsWarnEnabled;
                IsErrorEnabled = value.IsErrorEnabled;
                IsFatalEnabled = value.IsFatalEnabled;

                var processedLogFilePath = FilePathVariables.ReplaceVariables(value.LogFilePath);
                LoggerImpl.AddFileTarget(processedLogFilePath);

                config = value;
            }
        }

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
        /// 取得 <paramref name="level"/> 是否停用所有 logger 的 log。
        /// 如果設為停用，則所有 logger 均不再有作用。
        /// </summary>
        /// <returns><c>true</c>, if enabled, <c>false</c> otherwise.</returns>
        /// <param name="level">Log level.</param>
        public bool IsEnabled(LogLevel level)
        {
            bool isEnabled;
            logSwitches.TryGetValue(level, out isEnabled);
            return isEnabled;
        }

        /// <summary>
        /// 設定 <paramref name="level"/> 是否要停用所有 logger 的 log。
        /// 如果設為停用，則所有 logger 均不再有作用。
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

        public Logger FindOrCreateCurrentTypeLogger()
        {
            var stackFrame = new StackFrame(1);
            var callerMethod = stackFrame.GetMethod();
            var callingType = callerMethod.DeclaringType;
            return FindOrCreateLogger(callingType);
        }

        public Logger FindOrCreateLogger<T>()
        {
            return FindOrCreateLogger(typeof(T).FullName);
        }

        public Logger FindOrCreateLogger(Type loggerType)
        {
            return FindOrCreateLogger(loggerType.FullName);
        }

        public Logger FindOrCreateLogger(string loggerName)
        {
            var logger = FindLogger(loggerName);
            if (logger != null)
            {
                return logger;
            }

            logger = new Logger(loggerName);
            loggers.Add(loggerName, logger);
            return logger;
        }

        public Logger FindLogger<T>()
        {
            return FindLogger(typeof(T).FullName);
        }

        public Logger FindLogger(Type loggerType)
        {
            return FindLogger(loggerType.FullName);
        }

        public Logger FindLogger(string loggerName)
        {
            Logger logger;
            loggers.TryGetValue(loggerName, out logger);
            return logger;
        }
    }
}
