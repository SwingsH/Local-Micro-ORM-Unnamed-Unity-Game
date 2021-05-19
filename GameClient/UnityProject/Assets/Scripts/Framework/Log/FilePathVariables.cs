using UnityEngine;
using System.Collections.Generic;

namespace Tizsoft.Log
{
    public static class FilePathVariables
    {
        static readonly Dictionary<string, string> filePathVariables = new Dictionary<string, string>();

        static FilePathVariables()
        {
            RegisterFilePathVariable("${dataPath}", Application.dataPath);
            RegisterFilePathVariable("${persistentDataPath}", Application.persistentDataPath);
            RegisterFilePathVariable("${streamingAssetsPath}", Application.streamingAssetsPath);
            RegisterFilePathVariable("${temporaryCachePath}", Application.temporaryCachePath);
        }

        /// <summary>
        /// Registers the file path variable.
        /// e.g. LogManager.Default.RegisterFilePathVariable("${extraPath}", Application.dataPath);
        /// </summary>
        /// <param name="variable">Variable.</param>
        /// <param name="value">Value.</param>
        public static void RegisterFilePathVariable(string variable, string value)
        {
            filePathVariables.Add(variable, value);
        }

        public static string ReplaceVariables(string logFilePath)
        {
            string result = logFilePath;
            foreach (var pair in filePathVariables)
            {
                result = result.Replace(pair.Key, pair.Value);
            }
            return result;
        }

    }
}
