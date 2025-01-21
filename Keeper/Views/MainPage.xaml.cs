using System.Windows;
using Keeper.ViewModels;

namespace Keeper.Views
{
    public sealed partial class MainPage : Window
    {
        public MainPage(MainPageViewModel viewModel)
        {
            this.InitializeComponent();
            DataContext = viewModel;
        }
    }
}