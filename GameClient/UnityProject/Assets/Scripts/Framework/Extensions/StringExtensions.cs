using System.Text;

namespace Tizsoft.Extensions
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
    }
}
