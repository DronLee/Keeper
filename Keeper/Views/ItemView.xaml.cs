using Keeper.ViewModels;
using System.Windows;

namespace Keeper.Views
{
    public partial class ItemView : Window
    {
        public ItemView(DataRowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}