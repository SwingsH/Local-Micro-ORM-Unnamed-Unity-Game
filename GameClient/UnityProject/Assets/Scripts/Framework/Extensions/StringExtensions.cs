using System.Text;

namespace TIZSoft.Extensions
{
    public static class StringExtensions
    {
        public static byte[] ToAsciiBytes(this string utf8String)
        {
            return Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(utf8String));
        }

        public static char[] ToAsciiChars(this string utf8String)
        {
            return Encoding.ASCII.GetChars(ToAsciiBytes(utf8String));
        }

        public static string ToAsciiString(this string utf8String)
        {
            return Encoding.ASCII.GetString(ToAsciiBytes(utf8String));
        }

        public static byte[] ToUtf8Bytes(this string asciiString)
        {
            return Encoding.Convert(Encoding.ASCII, Encoding.UTF8, Encoding.ASCII.GetBytes(asciiString));
        }

        public static char[] ToUtf8Chars(this string asciiString)
        {
            return Encoding.UTF8.GetChars(ToUtf8Bytes(asciiString));
        }

        public static string ToUtf8String(this string asciiString)
        {
            return Encoding.UTF8.GetString(ToUtf8Bytes(asciiString));
        }

        public static string ToUtf8String(this byte[] ansiBytes)
        {
            return Encoding.UTF8.GetString(Encoding.Convert(Encoding.ASCII, Encoding.UTF8, ansiBytes));
        }

        public static string ToAnsiString(this byte[] ansiBytes)
        {
            return Encoding.ASCII.GetString(ansiBytes);
        }

        public static int GetDeterministicHashCode(this string value)
        {
            unchecked
            {
                int hash1 = (8891 << 16) + 8891;
                int hash2 = hash1;

                for (int i = 0; i < value.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ value[i];
                    if (i == value.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
                }
                return hash1 + (hash2 * 14432);
            }
        }

        public static string ReplaceFirstInstance(this string source, string find, string replace)
        {
            int index = source.IndexOf(find);
            return index < 0 ? source : source.Substring(0, index) + replace +
                 source.Substring(index + find.Length);
        }
    }
}
