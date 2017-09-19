namespace WebApiMessageEncryptAndDecrypt.Service
{
    /// <summary>
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        ///     Create salt key
        /// </summary>
        /// <param name="size">Key size</param>
        /// <returns>Salt key</returns>
        string CreateSaltKey(int size);

        /// <summary>
        ///     Create a password hash
        /// </summary>
        /// <param name="password">{assword</param>
        /// <param name="saltkey">Salk key</param>
        /// <param name="passwordFormat">Password format (hash algorithm)</param>
        /// <returns>Password hash</returns>
        string CreatePasswordHash(string password, string saltkey, string passwordFormat = "SHA256");

        /// <summary>
        ///     Create a data hash
        /// </summary>
        /// <param name="data">The data for calculating the hash</param>
        /// <param name="hashAlgorithm">Hash algorithm</param>
        /// <returns>Data hash</returns>
        string CreateHash(byte[] data, string hashAlgorithm = "SHA256");

        /// <summary>
        ///     Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <param name="ivKey"></param>
        /// <returns>Encrypted text</returns>
        string EncryptText(string plainText, string encryptionPrivateKey = "", string ivKey = "");

        /// <summary>
        ///     Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <param name="ivKey"></param>
        /// <returns>Decrypted text</returns>
        string DecryptText(string cipherText, string encryptionPrivateKey = "", string ivKey = "");

        /// <summary>
        ///     Encrypt text
        /// </summary>
        /// <param name="plainText">Text to encrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <param name="ivKey"></param>
        /// <returns>Encrypted text</returns>
        string EncryptText_Aes(string plainText, string encryptionPrivateKey = "", string ivKey = "");

        /// <summary>
        ///     Decrypt text
        /// </summary>
        /// <param name="cipherText">Text to decrypt</param>
        /// <param name="encryptionPrivateKey">Encryption private key</param>
        /// <param name="ivKey"></param>
        /// <returns>Decrypted text</returns>
        string DecryptText_Aes(string cipherText, string encryptionPrivateKey = "", string ivKey = "");
    }
}