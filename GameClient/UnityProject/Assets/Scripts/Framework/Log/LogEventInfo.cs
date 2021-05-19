using System;
using System.Threading;
using System.Text;

namespace Tizsoft.Log
{
    /// <summary>
    /// 表示一筆事件訊息。
    /// </summary>
    public class LogEventInfo
    {
        static int globalSequenceId;

        /// <summary>
        /// Gets the sequence identifier.
        /// </summary>
        /// <value>The sequence identifier.</value>
        public int SequenceId { get; private set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the logger name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>The log level.</value>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets or sets the custom data. e.g. Unity object, etc.
        /// </summary>
        /// <value>The custom data.</value>
        public object CustomData { get; set; }

        /// <summary>
        /// Gets or sets the exception or error.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the message or format.
        /// </summary>
        /// <value>The message or format.</value>
        public string MessageOrFormat { get; set; }

        /// <summary>
        /// Gets or sets the format arguments.
        /// </summary>
        /// <value>The format arguments.</value>
        public object[] FormatArguments { get; set; }

        public LogEventInfo()
        {
            SequenceId = Interlocked.Increment(ref globalSequenceId);
            Timestamp = DateTime.Now;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            var logLevel = LogLevel.ToString();

            // 先加上前置訊息
            // [流水號][log level][時間戳記][logger 名稱]: 
            builder.AppendFormat("[{0}][{1}][{2:yyyy-MM-dd HH:mm:ss.fff}][{3}]: ",
                                 SequenceId,
                                 logLevel,
                                 Timestamp,
                                 Name);

            // 建立一般 log 訊息
            if (FormatArguments != null && FormatArguments.Length > 0)
            {
                try
                {
                    builder.AppendFormat(MessageOrFormat, FormatArguments);
                    builder.AppendLine();
                }
                catch (Exception e)
                {
                    builder.AppendFormat("Internal Log Exception:\n  {0}\n", e);
                }
            }
            else
            {
                builder.AppendLine(MessageOrFormat);
            }

            // 如果有 Exception，就附加 Exception 訊息
            var exception = Exception;
            if (exception != null)
            {
                builder.AppendFormat("Exception:\n  {0}\n", exception);
            }

            return builder.ToString();
        }
    }
}
