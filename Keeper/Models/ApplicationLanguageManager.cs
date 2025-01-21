using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace Keeper.Models
{
    /// <summary>
    /// Class for management of application languages
    /// </summary>
    internal class ApplicationLanguageManager : IApplicationLanguageManager
    {
        private const string _defaultResourceFilePath = "Resources/lang.xaml";
        private const string _resourceFilePathFormat = "Resources/lang.{0}.xaml";

        private static readonly ApplicationLanguage[] _supportedLanguages = new[]
        {
            new ApplicationLanguage{ Name = "English", CultureInfo = new CultureInfo("en-US") },
            new ApplicationLanguage{ Name = "Русский", CultureInfo = new CultureInfo("ru-RU") }
        };

        /// <inheritdoc/>
        public event Action<ApplicationLanguage> LanguageChanged;

        public ApplicationLanguageManager()
        {
            ApplyCurrentCulture();
        }

        /// <inheritdoc/>
        public ApplicationLanguage[] SupportedLanguages => _supportedLanguages;

        /// <inheritdoc/>
        public ApplicationLanguage CurrentLanguage
        {
            get
            {
                var currentUICulture = Thread.CurrentThread.CurrentUICulture;
                var result = _supportedLanguages.FirstOrDefault(l => l.CultureInfo.Equals(currentUICulture));
                return result ?? _supportedLanguages.First();
            }
        }

        /// <inheritdoc/>
        public void SetLanguage(string languageName)
        {
            if (languageName == null) throw new ArgumentNullException(nameof(languageName));
            if (CurrentLanguage.Name.Equals(languageName)) return;

            var value = _supportedLanguages.FirstOrDefault(l => l.Name.Equals(languageName));
            if (value == null)
            {
                throw new ArgumentException("Language is not found", nameof(languageName));
            }

            Thread.CurrentThread.CurrentUICulture = value.CultureInfo;

            ApplyCurrentCulture();

            // Notify every windows
            LanguageChanged?.Invoke(value);
        }

        /// <inheritdoc/>
        public string GetLocalizedString(string key)
        {
            var resourceDictionary = GetCurrentResourceDictionary();
            return resourceDictionary[key]?.ToString() ?? null;
        }

        private void ApplyCurrentCulture()
        {
            var resourceDictionary = GetCurrentResourceDictionary();

            // Find an old ResourceDictionary and replace to the new ResourceDictionary
            var oldResourceDictionary = (from d in Application.Current.Resources.MergedDictionaries
                                         where d.Source != null && d.Source.OriginalString.StartsWith("Resources/lang.")
                                         select d).FirstOrDefault();
            if (oldResourceDictionary != null)
            {
                var resourceDictionaryIndex = Application.Current.Resources.MergedDictionaries.IndexOf(oldResourceDictionary);
                Application.Current.Resources.MergedDictionaries.Remove(oldResourceDictionary);
                Application.Current.Resources.MergedDictionaries.Insert(resourceDictionaryIndex, resourceDictionary);
            }
            else
            {
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }

        /// <summary>
        /// Get current resource dictionary (for selected language)
        /// </summary>
        /// <returns></returns>
        private ResourceDictionary GetCurrentResourceDictionary()
        {
            return GetResourceDictionary(Thread.CurrentThread.CurrentUICulture.Name);
        }

        private ResourceDictionary GetResourceDictionary(string cultureName)
        {
            var result = new ResourceDictionary();
            switch (cultureName)
            {
                case "ru-RU":
                    result.Source = new Uri(string.Format(_resourceFilePathFormat, cultureName), UriKind.Relative);
                    break;
                default:
                    result.Source = new Uri(_defaultResourceFilePath, UriKind.Relative);
                    break;
            }
            return result;
        }
    }
}
