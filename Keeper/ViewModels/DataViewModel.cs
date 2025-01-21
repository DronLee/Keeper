using Data.Abstractions;
using DataProvider.Abstractions;
using Keeper.Models;
using Keeper.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Keeper.ViewModels
{
    public class DataViewModel : INotifyPropertyChanged
    {
        private readonly IDataAdapter _dataAdapter;
        private readonly IApplicationLanguageManager _languageManager;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Func<Task> DeletedRow;
        public event Func<Task> ChangedRow;

        public DataViewModel(IDataAdapter dataAdapter, IApplicationLanguageManager languageManager)
        {
            Data = new ObservableCollection<DataSectionViewModel>();
            _dataAdapter = dataAdapter;
            _languageManager = languageManager;
        }

        public ObservableCollection<DataSectionViewModel> Data { get; private set; }

        public void LoadData(DataRow[] dataRows)
        {
            foreach (DataRowViewModel row in dataRows.Select(r => new DataRowViewModel(r, _languageManager)))
                AddRow(row);
        }

        public DataRowViewModel CreateRow(string currentSection)
        {
            var dataRowViewModel = new DataRowViewModel(new DataRow(), _languageManager);
            dataRowViewModel.Section = currentSection;

            new ItemView(dataRowViewModel).ShowDialog();

            if (dataRowViewModel.Changed)
                return dataRowViewModel;
            return null;
        }

        public DataSectionViewModel AddRow(DataRowViewModel dataRow)
        {
            dataRow.EditClick += EditRow;
            dataRow.DeletedEvent += DeleteRow;

            if (string.IsNullOrEmpty(dataRow.Section))
            {
                dataRow.Section = _languageManager.GetLocalizedString("OtherSectionName");
            }

            return AddRowToSection(dataRow);
        }

        private void InsertSection(DataSectionViewModel section)
        {
            for (int i = 0; i < Data.Count; i++)
                if (section.CompareTo(Data[i]) < 0)
                {
                    Data.Insert(i, section);
                    return;
                }
            Data.Add(section);
        }

        private DataSectionViewModel AddRowToSection(DataRowViewModel dataRow)
        {
            var dataSection = Data.SingleOrDefault(s => s.SectionName == dataRow.Section);
            if (dataSection == null)
            {
                dataSection = new DataSectionViewModel(dataRow.Section);
                InsertSection(dataSection);
            }
            dataSection.Data.Add(dataRow);
            return dataSection;
        }

        private void EditRow(DataRowViewModel dataRow)
        {
            var oldSectionName = dataRow.Section;

            // To block of save record button if connection with cloud storage doesn't exist.
            dataRow.Online = !_dataAdapter.Offline;

            new ItemView(dataRow).ShowDialog();

            if (dataRow.Changed)
            {
                if (dataRow.Section != oldSectionName)
                {
                    var oldSection = Data.Single(s => s.SectionName == oldSectionName);
                    RemoveDataRowFromSection(oldSection, dataRow);
                    AddRowToSection(dataRow);
                }
                ChangedRow?.Invoke();
                if (!dataRow.PasswordIsHided)
                    dataRow.HidePassword();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
            }
        }

        private void RemoveDataRowFromSection(DataSectionViewModel section, DataRowViewModel dataRow)
        {
            section.Data.Remove(dataRow);
            if (section.Data.Count == 0)
                Data.Remove(section);
        }

        private void DeleteRow(DataRowViewModel dataRow)
        {
            var messageViewModel = new MessageViewModel
            {
                Message = _languageManager.GetLocalizedString("WindowDeleteRecordAsk"),
                ApplyText = _languageManager.GetLocalizedString("WindowDeleteRecordOk"),
                CancelText = _languageManager.GetLocalizedString("WindowDeleteRecordCancel")
            };

            new MessageView(messageViewModel).ShowDialog();

            if (messageViewModel.IsApplied)
            {
                var dataSection = Data.Single(s => s.SectionName == dataRow.Section);
                RemoveDataRowFromSection(dataSection, dataRow);
                dataSection.Refresh();
                DeletedRow?.Invoke();
            }
        }
    }
}