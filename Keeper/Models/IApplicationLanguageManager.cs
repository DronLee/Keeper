using System;
using System.Windows;

namespace Keeper.Models
{
    /// <summary>
    /// Interface for management of application languages
    /// </summary>
    public interface IApplicationLanguageManager
    {
        /// <summary>
        /// Event of changing current language
        /// </summary>
        event Action<ApplicationLanguage> LanguageChanged;

        /// <summary>
        /// Language is selected now
        /// </summary>
        ApplicationLanguage CurrentLanguage { get; }

        /// <summary>
        /// Languages are supported in the application
        /// </summary>
        ApplicationLanguage[] SupportedLanguages { get; }

        /// <summary>
        /// Set new current language for the application
        /// </summary>
        void SetLanguage(string languageName);

        /// <summary>
        /// Get localized string (for current language) by key
        /// </summary>
        /// <param name="key">Key of string in dictionary for current language</param>
        /// <returns>Value for current language</returns>
        string GetLocalizedString(string key);
    }
}