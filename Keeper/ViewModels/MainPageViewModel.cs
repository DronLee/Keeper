using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Data.Abstractions;
using Data.Abstractions.Exceptions;
using DataProvider.Abstractions;
using Keeper.Models;
using Keeper.Views;

namespace Keeper.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly DataViewModel _dataView;
        private readonly SettingsViewModel _settingsViewModel;
        private readonly IDataAdapter _dataAdapter;
        private readonly IEncryptor _encrypter;
        private readonly IApplicationLanguageManager _languageManager;

        public event PropertyChangedEventHandler PropertyChanged;

        private DataSectionViewModel _selectedSection;
        private ICommand _loadDataCommand;
        private ICommand _addItemCommand;
        private ICommand _settingsCommand;

        public MainPageViewModel(DataViewModel dataView, IDataAdapter dataAdapter, IEncryptor encrypter, SettingsViewModel settingsViewModel,
            IApplicationLanguageManager languageManager)
        {
            _dataView = dataView;
            _settingsViewModel = settingsViewModel;
            _settingsViewModel.SignatureChanged += SaveData;
            _dataView.DeletedRow += SaveData;
            _dataView.ChangedRow += SaveData;
            _dataAdapter = dataAdapter;
            _dataAdapter.ChangeConnectEvent += ChangeCloudConnect;
            _encrypter = encrypter;
            _languageManager = languageManager;
        }

        public string PathInfo
        {
            get
            {
                return string.Format(_languageManager.GetLocalizedString("DataFileLocalPathStringFormat"),
                    _dataAdapter.DataFile == null ? string.Empty : _dataAdapter.DataFile);
            }
        }

        public ICommand LoadDataCommand
        {
            get
            {
                if (_loadDataCommand == null)
                {
                    _loadDataCommand = new Command(async x =>
                    {
                        await _dataAdapter.AuthAsync();
                        await LoadData();
                    });
                }
                return _loadDataCommand;
            }
        }

        public ObservableCollection<DataSectionViewModel> Data
        {
            get { return _dataView.Data; }
        }

        public ICommand AddItemCommand
        {
            get
            {
                if (_addItemCommand == null)
                    _addItemCommand = new Command(async x => await AddItem());
                return _addItemCommand;
            }
        }

        public ICommand SettingsCommand
        {
            get
            {
                if (_settingsCommand == null)
                    _settingsCommand = new Command(action => Settings());
                return _settingsCommand;
            }
        }

        public ConnectStatus ConnectStatus =>
            new ConnectStatus()
            {
                Online = !_dataAdapter.Offline,
                ImageUrl = App.IconsDirecory + (_dataAdapter.Offline ? "/DisconnectedCloud.png" : "/ConnectedCloud.png"),
                Comment = _dataAdapter.Offline ? _languageManager.GetLocalizedString("ConnectionWithCloudDoesntExist") :
                    _languageManager.GetLocalizedString("ConnectionWithCloudExists")
            };

        public DataSectionViewModel SelectedSection
        {
            get { return _selectedSection; }
            set
            {
                _selectedSection = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SelectedSection)));
            }
        }

        private async Task LoadData()
        {
            try
            {
                byte[] data;
                try
                {
                    data = await _dataAdapter.DownloadDataAsync();
                }
                catch (ServerDataFileNotFoundException)
                {
                    data = null;
                }

                if (data?.Any() == true)
                {
                    _dataView.LoadData(_encrypter.Decrypt(data));
                    if (Data.Count > 0)
                        SelectedSection = Data[0];
                }

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PathInfo)));
            }
            catch (DecryptException)
            {
                _settingsViewModel.SignatureChanged -= SaveData;
                _settingsViewModel.SignatureChanged += LoadData;
                Settings();
                _settingsViewModel.SignatureChanged -= LoadData;
                _settingsViewModel.SignatureChanged += SaveData;
            }
        }

        private async Task AddItem()
        {
            var dataRow = _dataView.CreateRow(_selectedSection?.SectionName);
            if (dataRow != null)
            {
                var section = _dataView.AddRow(dataRow);
                section.Refresh();
                await SaveData();
                SelectedSection = section;
            }
        }

        private void Settings()
        {
            var settingsView = new SettingsView(_settingsViewModel);
            settingsView.ShowDialog();
        }

        private void ChangeCloudConnect()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConnectStatus)));
        }

        private async Task SaveData()
        {
            var dataRows = Data.SelectMany(v => v.Data.Select(d => d.DataRow)).ToArray();
            await _dataAdapter.SaveDataAsync(_encrypter.Encrypt(dataRows));
        }
    }
}