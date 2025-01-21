using Keeper.Models;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Keeper.ViewModels
{
    public class SettingsViewModel
    {
        private readonly IApplicationLanguageManager _applicationLanguageManager;
        private readonly Settings _settings;
        private ICommand _acceptCommand;

        public event Func<Task> SignatureChanged;

        public SettingsViewModel(Settings settings, IApplicationLanguageManager applicationLanguageManager)
        {
            _settings = settings;
            _settings.SignatureChanged += () => { SignatureChanged?.Invoke(); };
            Signature = _settings.Signature;
            _applicationLanguageManager = applicationLanguageManager;
            CurrentLanguage = _applicationLanguageManager.CurrentLanguage;
        }

        public ICommand AcceptCommand
        {
            get
            {
                if (_acceptCommand == null)
                    _acceptCommand = new Command<Window>((Window window) =>
                    {
                        _settings.Signature = Signature;
                        _applicationLanguageManager.SetLanguage(CurrentLanguage.Name);
                        _settings.CurrentLanguageName = CurrentLanguage.Name;
                        _settings.Save();
                        window.Close();
                    });
                return _acceptCommand;
            }
        }

        public ApplicationLanguage[] Languages => _applicationLanguageManager.SupportedLanguages;

        public string Signature { get; set; }

        public ApplicationLanguage CurrentLanguage { get; set; }
    }
}