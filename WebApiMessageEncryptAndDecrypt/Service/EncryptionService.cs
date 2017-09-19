using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WebApiMessageEncryptAndDecrypt.Service
{
    /// <summary>
    /// 
    /// </summary>
    public class EncryptionService : IEncryptionService
    {
        private readonly SecuritySettings _securitySettings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="securitySettings"></param>
        public EncryptionService(SecuritySettings securitySettings)
        {
            _securitySettings = securitySettings;
        }

        /// <summary>
        ///     Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        public virtual string CreateSaltKey(int size)
        {
            // Generate a cryptographic random number
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number
            return Convert.ToBase64String(buff);
        }

        /// <summary>
        ///     Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        public virtual string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA256")
        {
            return CreateHash(Encoding.UTF8.GetBytes(string.Concat(password, saltkey)), passwordFormat);
        }

        /// <summary>
        ///     Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Data hash</returns>
        public virtual string CreateHash(byte[] data, string hashAlgorithm = "SHA256")
        {
            if (string.IsNullOrEmpty(hashAlgorithm))
                hashAlgorithm = "SHA256";

            //return FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPassword, passwordFormat);
            var algorithm = HashAlgorithm.Create(hashAlgorithm);
            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            var hashByteArray = algorithm.ComputeHash(data);
            return BitConverter.ToString(hashByteArray).Replace("-", "");
        }

        /// <summary>
        ///     Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionKey">Encryption private key</param>
        /// <param name="ivKey"></param>
        /// <returns>Encrypted text</returns>
        public virtual string EncryptText(string plainText, string encryptionKey = "", string ivKey = "")
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            if (string.IsNullOrEmpty(encryptionKey))
                encryptionKey = _securitySettings.EncryptionKey;
            if (string.IsNullOrEmpty(ivKey))
                ivKey = _securitySettings.IvKey;
            var alg = GetTdesProvider(encryptionKey, ivKey);

            var encryptedBinary = EncryptTextToMemory(plainText, alg.CreateEncryptor(alg.Key, alg.IV));
            return Convert.ToBase64String(encryptedBinary);
        }

        /// <summary>
        ///     Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionKey">Encryption private key</param>
        /// <param name="ivKey"></param>
        /// <returns>Decrypted text</returns>
        public virtual string DecryptText(string cipherText, string encryptionKey = "", string ivKey = "")
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            if (string.IsNullOrEmpty(encryptionKey))
                encryptionKey = _securitySettings.EncryptionKey;
            if (string.IsNullOrEmpty(ivKey))
                ivKey = _securitySettings.IvKey;
            var alg = GetTdesProvider(encryptionKey, ivKey);

            var buffer = Convert.FromBase64String(cipherText);
            return DecryptTextFromMemory(buffer, alg.CreateDecryptor(alg.Key, alg.IV));
        }

        private static TripleDESCryptoServiceProvider GetTdesProvider(string encryptionKey, string ivKey)
        {
            return new TripleDESCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(encryptionKey.Substring(0, 16)),
                IV = Encoding.ASCII.GetBytes(encryptionKey.Substring(0, 8)),
                Mode = CipherMode.CBC
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="encryptionKey"></param>
        /// <param name="ivKey"></param>
        /// <returns></returns>
        public string EncryptText_Aes(string plainText, string encryptionKey = "", string ivKey = "")
        {
            if (string.IsNullOrEmpty(plainText))
                return plainText;

            if (string.IsNullOrEmpty(encryptionKey))
                encryptionKey = _securitySettings.EncryptionKey;
            if (string.IsNullOrEmpty(ivKey))
                ivKey = _securitySettings.IvKey;
            var alg = GetAesProvider(encryptionKey, ivKey);

            var encryptedBinary = EncryptTextToMemory(plainText, alg.CreateEncryptor(alg.Key, alg.IV));
            return Convert.ToBase64String(encryptedBinary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <param name="encryptionKey"></param>
        /// <param name="ivKey"></param>
        /// <returns></returns>
        public string DecryptText_Aes(string cipherText, string encryptionKey = "", string ivKey = "")
        {
            if (string.IsNullOrEmpty(cipherText))
                return cipherText;

            if (string.IsNullOrEmpty(encryptionKey))
                encryptionKey = _securitySettings.EncryptionKey;
            if (string.IsNullOrEmpty(ivKey))
                ivKey = _securitySettings.IvKey;
            var alg = GetAesProvider(encryptionKey, ivKey);

            var buffer = Convert.FromBase64String(cipherText);
            return DecryptTextFromMemory(buffer, alg.CreateDecryptor(alg.Key, alg.IV));
        }

        private static AesCryptoServiceProvider GetAesProvider(string encryptionKey, string ivKey)
        {
            return new AesCryptoServiceProvider()
            {
                Key = Encoding.ASCII.GetBytes(encryptionKey.Substring(0, 32)),
                IV = Encoding.ASCII.GetBytes(ivKey.Substring(0, 16)),
                Mode = CipherMode.CBC
            };
        }

        #region Utilities

        private static byte[] EncryptTextToMemory(string data, ICryptoTransform encryptor)
        {
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    var toEncrypt = Encoding.Unicode.GetBytes(data);
                    cs.Write(toEncrypt, 0, toEncrypt.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray();
            }
        }

        private static string DecryptTextFromMemory(byte[] data, ICryptoTransform decryptor)
        {
            using (var ms = new MemoryStream(data))
            {
                using (var cs = new CryptoStream(ms, decryptor,
                    CryptoStreamMode.Read))
                {
                    using (var sr = new StreamReader(cs, Encoding.Unicode))
                    {
                        return sr.ReadLine();
                    }
                }
            }
        }

        #endregion
    }
}