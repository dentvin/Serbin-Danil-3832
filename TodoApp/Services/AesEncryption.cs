using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TodoApp.Services
{
    public static class AesEncryption
    {
        // Ключ и IV хранятся централизованно (32 байта для AES-256, 16 байт для IV)
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("1234567890123456");

        public static CryptoStream CreateEncryptStream(Stream outputStream)
        {
            var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var encryptor = aes.CreateEncryptor();
            return new CryptoStream(outputStream, encryptor, CryptoStreamMode.Write);
        }

        public static CryptoStream CreateDecryptStream(Stream inputStream)
        {
            var aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var decryptor = aes.CreateDecryptor();
            return new CryptoStream(inputStream, decryptor, CryptoStreamMode.Read);
        }

        public static byte[] EncryptBytes(byte[] plainBytes)
        {
            using (var ms = new MemoryStream())
            using (var cryptoStream = CreateEncryptStream(ms))
            {
                cryptoStream.Write(plainBytes, 0, plainBytes.Length);
                cryptoStream.FlushFinalBlock();
                return ms.ToArray();
            }
        }

        public static byte[] DecryptBytes(byte[] encryptedBytes)
        {
            using (var ms = new MemoryStream(encryptedBytes))
            using (var cryptoStream = CreateDecryptStream(ms))
            using (var resultMs = new MemoryStream())
            {
                cryptoStream.CopyTo(resultMs);
                return resultMs.ToArray();
            }
        }
    }
}