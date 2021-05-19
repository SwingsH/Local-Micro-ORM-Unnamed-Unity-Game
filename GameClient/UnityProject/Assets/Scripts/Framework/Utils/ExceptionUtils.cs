using System;
using System.Collections.Generic;
using Tizsoft.Extensions;

namespace Tizsoft.Utils
{
    /// <summary>
    /// Exception utilities.
    /// </summary>
    public static class ExceptionUtils
    {
        /// <summary>
        /// 檢查 argument 是否為 null，是的話就 throw exception。
        /// </summary>
        /// <param name="arg">Argument.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void VerifyArgumentNull<T>(T arg, string paramName)
            where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// 檢查 argument 是否為 null，是的話就 throw exception。
        /// </summary>
        /// <param name="arg">Argument.</param>
        /// <param name="paramName">Parameter name.</param>
        /// <param name="format">Format.</param>
        /// <param name="formatArgs">Format arguments.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void VerifyArgumentNull<T>(T arg, string paramName, string format, params object[] formatArgs)
            where T : class
        {
            if (arg == null)
            {
                throw new ArgumentNullException(paramName, formatArgs != null ? string.Format(format, formatArgs) : format);
            }
        }

        public static void VerifyIndex<T>(IEnumerable<T> container, int index)
        {
            if (container != null && 0 <= index)
            {
                // 避免使用 Linq
                var count = 0;
                foreach (var _ in container)
                {
                    ++count;
                }

                if (index >= count)
                {
                    throw new IndexOutOfRangeException(string.Format(
                        "Valid range is [0, {0}). Actual index is {1}.", count, index));
                }
            }
        }

        public static void VerifyIndex<T>(T[] container, int index)
        {
            if (!container.IsValidIndex(index))
            {
                throw new IndexOutOfRangeException(string.Format(
                    "Valid range is [0, {0}). Actual index is {1}.", container.Length, index));
            }
        }

        public static void VerifyIndex<T>(ICollection<T> container, int index)
        {
            if (!container.IsValidIndex(index))
            {
                throw new IndexOutOfRangeException(string.Format(
                    "Valid range is [0, {0}). Actual index is {1}.", container.Count, index));
            }
        }

        public static void VerifyArgumentNullOrEmpty(string arg, string paramName)
        {
            VerifyArgumentNullOrEmpty(arg, paramName, "Argument \"{0}\" is null or empty", paramName);
        }

        public static void VerifyArgumentNullOrEmpty(string arg, string paramName, string format, params object[] formatArgs)
        {
            if (string.IsNullOrEmpty(arg))
            {
                throw new ArgumentException(paramName, string.Format(format, formatArgs));
            }
        }
    }
}
