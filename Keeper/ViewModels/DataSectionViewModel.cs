using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Keeper.ViewModels
{
    public class DataSectionViewModel : INotifyPropertyChanged, IComparable
    {
        public string SectionName { get; private set; }

        public ObservableCollection<DataRowViewModel> Data { get; } = new ObservableCollection<DataRowViewModel>();

        public DataSectionViewModel(string sectionName)
        {
            SectionName = sectionName;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Refresh()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Data"));
        }

        public int CompareTo(object obj)
        {
            var section = obj as DataSectionViewModel;
            if (section == null)
                return -1;
            return SectionName.CompareTo(section.SectionName);
        }
    }
}