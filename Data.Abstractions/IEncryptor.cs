namespace Data.Abstractions
{
    /// <summary>
    /// Interface of data encryptor
    /// </summary>
    public interface IEncryptor
    {
        /// <summary>
        /// Encrypt data
        /// </summary>
        /// <param name="dataRows">Array of data for encryption</param>
        /// <returns>Encrypted data</returns>
        byte[] Encrypt(DataRow[] dataRows);

        /// <summary>
        /// Decrypt data
        /// </summary>
        /// <param name="data">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        DataRow[] Decrypt(byte[] data);
    }
}
