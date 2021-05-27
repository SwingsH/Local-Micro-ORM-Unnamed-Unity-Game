using System.Collections.Generic;
using System.IO;
using UberLogger;

namespace TIZSoft.Utils.Log
{
    /// <summary>
    /// based on : UberLogger。
    /// </summary>
    class FileTarget : ILogger
    {
        static readonly Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>();

        readonly StreamWriter writer;

        public FileTarget(string fullFilePath)
        {
            // 防止多重開檔又寫檔
            writers.TryGetValue(fullFilePath, out writer);
            if (writer == null)
            {
                writer = new StreamWriter(fullFilePath);
                writer.AutoFlush = true;
                writers[fullFilePath] = writer;
            }
        }

        public void Log(LogInfo logInfo)
        {
            writer.Write(logInfo.Message);
            if (logInfo.Callstack.Count > 0)
            {
                foreach (var frame in logInfo.Callstack)
                {
                    writer.WriteLine(frame.GetFormattedMethodName());
                }
            }
            writer.WriteLine();
        }
    }
}
 