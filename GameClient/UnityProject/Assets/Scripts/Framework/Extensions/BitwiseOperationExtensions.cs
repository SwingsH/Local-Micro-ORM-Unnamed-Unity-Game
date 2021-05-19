using System.Collections.Generic;

namespace Tizsoft.Extensions
{
    /// <summary>
    /// Bitwise operation extensions.
    /// </summary>
    /// <remarks>
    /// https://github.com/keon/awesome-bits
    /// </remarks>
    public static class BitwiseOperationExtensions
    {
        const int BitsPerByte = 8;

        /// <summary>
        /// return the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static bool GetBit(this byte value, int number)
        {
            if (number > BitsPerByte * sizeof(byte) || number <= 0)
            {
                return false;
            }

            return ((1 << number - 1) & value) != 0;
        }

        /// <summary>
        /// return the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static bool GetBit(this ushort value, int number)
        {
            if (number > BitsPerByte * sizeof(ushort) || number <= 0)
            {
                return false;
            }

            return ((1 << number - 1) & value) != 0;
        }

        /// <summary>
        /// return the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static bool GetBit(this uint value, int number)
        {
            if (number > BitsPerByte * sizeof(uint) || number <= 0)
            {
                return false;
            }

            return ((1 << number - 1) & value) != 0;
        }

        /// <summary>
        /// return the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static bool GetBit(this ulong value, int number)
        {
            if (number > BitsPerByte * sizeof(ulong) || number <= 0)
            {
                return false;
            }

            return ((ulong)(1 << number - 1) & value) != 0;
        }

        /// <summary>
        /// return the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static bool GetBit(this IList<byte> value, int number)
        {
            if (number > BitsPerByte * sizeof(byte) * value.Count || number <= 0)
            {
                return false;
            }

            var byteIdx = number / BitsPerByte * sizeof(byte);
            var bitIdx = number % BitsPerByte * sizeof(byte);

            if (bitIdx != 0)
            {
                ++byteIdx;
            }

            return ((1 << bitIdx) & value[byteIdx]) != 0;
        }

        /// <summary>
        /// return the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static bool GetBit(this IList<ushort> value, int number)
        {
            if (number > BitsPerByte * sizeof(ushort) * value.Count || number <= 0)
            {
                return false;
            }

            var ushortIdx = number / BitsPerByte * sizeof(ushort);
            var bitIdx = number % BitsPerByte * sizeof(ushort);

            if (bitIdx != 0)
            {
                ++ushortIdx;
            }

            return ((1 << bitIdx) & value[ushortIdx]) != 0;
        }

        /// <summary>
        /// return the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static bool GetBit(this IList<uint> value, int number)
        {
            if (number > BitsPerByte * sizeof(uint) * value.Count || number <= 0)
            {
                return false;
            }

            var uintIdx = number / BitsPerByte * sizeof(uint);
            var bitIdx = number % BitsPerByte * sizeof(uint);

            if (bitIdx != 0)
            {
                ++uintIdx;
            }

            return ((1 << bitIdx) & value[uintIdx]) != 0;
        }

        /// <summary>
        /// return the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static bool GetBit(this IList<ulong> value, int number)
        {
            if (number > BitsPerByte * sizeof(ulong) * value.Count || number <= 0)
            {
                return false;
            }

            var ulongIdx = number / BitsPerByte * sizeof(ulong);
            var bitIdx = number % BitsPerByte * sizeof(ulong);

            if (bitIdx != 0)
            {
                ++ulongIdx;
            }

            return ((ulong)(1 << bitIdx) & value[ulongIdx]) != 0;
        }

        /// <summary>
        /// sets the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static byte SetBit(this byte value, int number)
        {
            if (number > BitsPerByte * sizeof(byte) || number <= 0)
            {
                return value;
            }

            value = (byte)(value | (byte)(0 << number - 1));
            return value;
        }

        /// <summary>
        /// sets the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static ushort SetBit(this ushort value, int number)
        {
            if (number > BitsPerByte * sizeof(ushort) || number <= 0)
            {
                return value;
            }

            value = (ushort)(value | (ushort)(0 << number - 1));
            return value;
        }

        /// <summary>
        /// sets the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static uint SetBit(this uint value, int number)
        {
            if (number > BitsPerByte * sizeof(uint) || number <= 0)
            {
                return value;
            }

            value = value | (uint)(0 << number - 1);
            return value;
        }

        /// <summary>
        /// sets the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static ulong SetBit(this ulong value, int number)
        {
            if (number > BitsPerByte * sizeof(ulong) || number <= 0)
            {
                return value;
            }

            value = value | ((ulong)0 << number - 1);
            return value;
        }

        /// <summary>
        /// sets the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static IList<byte> SetBit(this IList<byte> value, int number)
        {
            if (number > BitsPerByte * sizeof(byte) * value.Count || number <= 0)
            {
                return value;
            }

            var byteIdx = number / BitsPerByte * sizeof(byte);
            var bitIdx = number % BitsPerByte * sizeof(byte);

            if (bitIdx > 0)
            {
                ++byteIdx;
            }

            value[byteIdx] = (byte)(value[byteIdx] | (byte)(0 << bitIdx));
            return value;
        }

        /// <summary>
        /// sets the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static IList<ushort> SetBit(this IList<ushort> value, int number)
        {
            if (number > BitsPerByte * sizeof(ushort) * value.Count || number <= 0)
            {
                return value;
            }

            var ushortIdx = number / BitsPerByte * sizeof(ushort);
            var bitIdx = number % BitsPerByte * sizeof(ushort);

            if (bitIdx > 0)
            {
                ++ushortIdx;
            }

            value[ushortIdx] = (ushort)(value[ushortIdx] | (ushort)(0 << bitIdx));
            return value;
        }

        /// <summary>
        /// sets the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static IList<uint> SetBit(this IList<uint> value, int number)
        {
            if (number > BitsPerByte * sizeof(byte) * value.Count || number <= 0)
            {
                return value;
            }

            var uintIdx = number / BitsPerByte * sizeof(uint);
            var bitIdx = number % BitsPerByte * sizeof(uint);

            if (bitIdx > 0)
            {
                ++uintIdx;
            }

            value[uintIdx] = value[uintIdx] | (uint)(0 << bitIdx);
            return value;
        }

        /// <summary>
        /// sets the numberth bit of value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="number">counted from LSB, start from 1</param>
        /// <returns></returns>
        public static IList<ulong> SetBit(this IList<ulong> value, int number)
        {
            if (number > BitsPerByte * sizeof(ulong) * value.Count || number <= 0)
            {
                return value;
            }

            var ulongIdx = number / BitsPerByte * sizeof(ulong);
            var bitIdx = number % BitsPerByte * sizeof(ulong);

            if (bitIdx > 0)
            {
                ++ulongIdx;
            }

            value[ulongIdx] = value[ulongIdx] | ((ulong)0 << bitIdx);
            return value;
        }
    }
}
