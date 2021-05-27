using System;

namespace TIZSoft.Utils.Log
{
    [Serializable]
    public class LogConfig
    {
        public bool IsTraceEnabled = true;
        public bool IsDebugEnabled = true;
        public bool IsInfoEnabled = true;
        public bool IsWarnEnabled = true;
        public bool IsErrorEnabled = true;
        public bool IsFatalEnabled = true;

        /// <summary>
        /// 可用預設保留字替換。
        /// ${dataPath} 等同於 <see cref="UnityEngine.Application.dataPath"/>
        /// ${persistentDataPath} 等同於 <see cref="UnityEngine.Application.persistentDataPath"/>
        /// ${streamingDataPath} 等同於 <see cref="UnityEngine.Application.streamingAssetsPath"/>
        /// ${temporaryCachePath} 等同於 <see cref="UnityEngine.Application.temporaryCachePath"/>
        /// </summary>
        public string LogFilePath = "${dataPath}/../log.txt";
    }
}
