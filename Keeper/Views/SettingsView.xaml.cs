using Keeper.ViewModels;
using System.Windows;

namespace Keeper.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView(SettingsViewModel settingsViewModel)
        {
            InitializeComponent();
            DataContext = settingsViewModel;
        }
    }
}