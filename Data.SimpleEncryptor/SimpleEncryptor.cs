using Data.Abstractions;
using Data.Encrypter;
using System;
using System.Linq;
using System.Text;

namespace Data.SimpleEncryptor
{
    /// <summary>
    /// It's just simple example of encryptor without some tricks
    /// </summary>
    public class SimpleEncryptor : IEncryptor
    {
        private const string _linesSeparator = "\n";
        private static readonly Encoding _encoding = Encoding.UTF8;

        // The signature just is adding end of string
        private readonly string _signature;

        public SimpleEncryptor(string signature)
        {
            _signature = signature;
        }

        public DataRow[] Decrypt(byte[] data)
        {
            var fullDataText = _encoding.GetString(data)
                // Remove signature
                .TrimEnd(_signature.ToCharArray());

            return fullDataText.Split(new string[] { _linesSeparator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => DataRowConverter.DataRowFromString(s)).ToArray();
        }

        public byte[] Encrypt(DataRow[] dataRows)
        {
            var fullDataText = string.Join(_linesSeparator, dataRows.Select(r => DataRowConverter.DataRowToString(r)))
                // Add signature
                + _signature;
            return _encoding.GetBytes(fullDataText);
        }
    }
}
