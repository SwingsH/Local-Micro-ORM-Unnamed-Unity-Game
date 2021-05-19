using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tizsoft.Utils
{
    /// <summary>
    /// 提供常用的雜湊方法。
    /// </summary>
    public static class HashUtils
    {
        // 在 Android 使用 HashAlgorithm.Create() 會無效。
        // 另外，CngAlgorithm 在 .NET 2.0 subset 無法使用。


        #region MD5

        public static byte[] ComputeHashMd5(ArraySegment<byte> segment)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                return ComputeHash(segment, md5);
            }
        }

        public static byte[] ComputeHashMd5(Stream inputStream)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                return ComputeHash(inputStream, md5);
            }
        }

        public static byte[] ComputeHashMd5(byte[] bytes)
        {
            return ComputeHashMd5(new ArraySegment<byte>(bytes));
        }

        public static string ComputeHashMd5String(byte[] bytes)
        {
            return ComputeHashMd5(bytes).ToHexString();
        }

        #endregion MD5


        #region SHA1
        // SHA1CryptoServiceProvider and SHA1Managed are the same in .NET Mono.

        public static byte[] ComputeHashSha1(ArraySegment<byte> segment)
        {
            using (var sha1 = new SHA1Managed())
            {
                return ComputeHash(segment, sha1);
            }
        }

        public static byte[] ComputeHashSha1(Stream inputStream)
        {
            using (var sha1 = new SHA1Managed())
            {
                return ComputeHash(inputStream, sha1);
            }
        }

        public static byte[] ComputeHashSha1(byte[] bytes)
        {
            return ComputeHashSha1(new ArraySegment<byte>(bytes));
        }

        public static string ComputeHashSha1String(byte[] bytes)
        {
            return ComputeHashSha1(bytes).ToHexString();
        }

        #endregion SHA1


        #region SHA256
        // SHA256CryptoServiceProvider and SHA256Managed are the same in .NET Mono.

        public static byte[] ComputeHashSha256(ArraySegment<byte> segment)
        {
            using (var sha256 = new SHA256Managed())
            {
                return ComputeHash(segment, sha256);
            }
        }

        public static byte[] ComputeHashSha256(Stream inputStream)
        {
            using (var sha256 = new SHA256Managed())
            {
                return ComputeHash(inputStream, sha256);
            }
        }

        public static byte[] ComputeHashSha256(byte[] bytes)
        {
            return ComputeHashSha256(new ArraySegment<byte>(bytes));
        }

        public static string ComputeHashSha256String(byte[] bytes)
        {
            return ComputeHashSha256(bytes).ToHexString();
        }

        #endregion SHA256


        public static byte[] ComputeHash(byte[] bytes, HashAlgorithm hashAlgorithm)
        {
            ExceptionUtils.VerifyArgumentNull(hashAlgorithm, "hashAlgorithm");
            return hashAlgorithm.ComputeHash(bytes);
        }

        public static byte[] ComputeHash(ArraySegment<byte> segment, HashAlgorithm hashAlgorithm)
        {
            ExceptionUtils.VerifyArgumentNull(hashAlgorithm, "hashAlgorithm");
            return hashAlgorithm.ComputeHash(segment.Array, segment.Offset, segment.Count);
        }

        public static byte[] ComputeHash(Stream inputStream, HashAlgorithm hashAlgorithm)
        {
            ExceptionUtils.VerifyArgumentNull(hashAlgorithm, "hashAlgorithm");
            return hashAlgorithm.ComputeHash(inputStream);
        }

        public static string ComputeHashString(byte[] bytes, HashAlgorithm hashAlgorithm)
        {
            var hash = ComputeHash(bytes, hashAlgorithm);
            return hash.ToHexString();
        }

        public static string ComputeFileHashString(string path, HashAlgorithm hashAlgorithm)
        {
            if (File.Exists(path))
            {
                var bytes = File.ReadAllBytes(path);
                return ComputeHashString(bytes, hashAlgorithm);
            }
            return string.Empty;
        }

        /// <summary>
        /// 將 bytes 轉為 hex 字串，hex 格式預設為 "x2"。
        /// </summary>
        /// <example>
        /// <code>
        /// var bytes = new byte[] { 0x38, 0x1A, 0x2b, 0xc };
        /// var str = bytes.ToHexString();
        /// // result: str = "381a2b0c"
        /// </code>
        /// </example>
        /// <returns>The hash string.</returns>
        /// <param name="bytes">Bytes.</param>
        public static string ToHexString(this byte[] bytes)
        {
            return ToHexString(bytes, "x2");
        }

        /// <summary>
        /// 將 bytes 轉為 hex 字串。
        /// </summary>
        /// <returns>The hash string.</returns>
        /// <param name="bytes">Bytes.</param>
        /// <param name="format">Byte format.</param>
        public static string ToHexString(this byte[] bytes, string format)
        {
            if (bytes == null)
            {
                return string.Empty;
            }
            var hashStr = new StringBuilder();
            foreach (var b in bytes)
            {
                hashStr.Append(b.ToString(format));
            }
            return hashStr.ToString();
        }
    }
}
