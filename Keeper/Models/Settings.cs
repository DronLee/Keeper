using Newtonsoft.Json;
using System;
using System.IO;

namespace Keeper.Models
{
    /// <summary>
    /// Application settings model
    /// </summary>
    public class Settings
    {

#if DEBUG
        private const string _fileName = "appsettings.Development.json";
#else
        private const string _fileName = "appsettings.Production.json";
#endif

        private static string _filePath = Path.Combine(Directory.GetCurrentDirectory(), _fileName);

        private string _oldSignature = null;

        /// <summary>
        /// A signature for encoding and decoding data file
        /// </summary>
        public string Signature { get; set; }

        /// <summary>
        /// File name which data are kept
        /// </summary>
        public string DataFileName { get; set; }

        /// <summary>
        /// Name of language is selected now
        /// </summary>
        public string CurrentLanguageName { get; set; }

        /// <summary>
        /// Event of changing signature
        /// </summary>
        public event Action SignatureChanged;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        /// Save settings to file
        /// </summary>
        public void Save()
        {
            File.WriteAllText(_filePath, ToString());

            if (!_oldSignature.Equals(Signature))
            {
                SignatureChanged?.Invoke();
                _oldSignature = Signature;
            }
        }

        /// <summary>
        /// Get settings from file
        /// </summary>
        public static Settings Get()
        {
            var result = File.Exists(_filePath) ? JsonConvert.DeserializeObject<Settings>(File.ReadAllText(_filePath)) : new Settings();
            result._oldSignature = result.Signature;
            return result;
        }
    }
}