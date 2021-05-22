using System;

namespace TIZSoft.Utils
{
    // TODO: Unit test required.

    /// <summary>
    /// Provides enumeration utility methods.
    /// </summary>
    public static class EnumUtils
    {
        /// <summary>
        /// Parses the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TEnum ToEnum<TEnum>(string value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.Parse(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(object value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(byte value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(sbyte value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(short value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(ushort value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(int value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(uint value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(long value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// Converts the value to an enumeration member. If it is undefined, then returns default one.
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="value">The value to convert to an enumeration member.</param>
        /// <returns>An instance of the enumeration set to <paramref name="value"/>.</returns>
        public static TEnum ToEnum<TEnum>(ulong value)
        {
            var enumType = typeof(TEnum);
            return Enum.IsDefined(enumType, value) ? (TEnum)Enum.ToObject(typeof(TEnum), value) : default(TEnum);
        }

        /// <summary>
        /// 檢查 <paramref name="enumValue"/> 是否包含 <paramref name="flag"/>。<br />
        /// </summary>
        /// <returns><c>true</c>, if has flag, <c>false</c> otherwise.</returns>
        /// <param name="enumValue">Enum value.</param>
        /// <param name="flag">Flag.</param>
        public static bool HasFlag(this Enum enumValue, Enum flag)
        {
            return HasFlag(enumValue, flag, false);
        }

        /// <summary>
        /// 檢查 <paramref name="enumValue"/> 是否包含 <paramref name="flag"/>。<br />
        /// <br />
        /// 下列情況將會回傳 <c>false</c>。
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             <paramref name="enumValue"/> 為 <c>null</c>。
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             <paramref name="flag"/> 為 <c>null</c>。
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             指定要檢查型別時，<paramref name="flag"/> 型別與 <paramref name="enumValue"/> 不一致。
        ///         </description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <param name="enumValue">Enum value.</param>
        /// <param name="flag">Flag.</param>
        /// <param name="checkType">是否要檢查型別。</param>
        /// <returns><c>true</c>, if has flag, <c>false</c> otherwise.</returns>
        public static bool HasFlag(this Enum enumValue, Enum flag, bool checkType)
        {
            if (enumValue == null)
            {
                return false;
            }

            if (flag == null)
            {
                return false;
            }

            if (checkType && enumValue.GetType() != flag.GetType())
            {
                return false;
            }

            var v1 = Convert.ToUInt64(enumValue);
            var v2 = Convert.ToUInt64(flag);
            return (v1 & v2) == v2;
        }
    }
}
