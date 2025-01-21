using DataProvider.Yandex.ViewModels;
using System.Windows;

namespace DataProvider.Yandex.Views
{
    /// <summary>
    /// Interaction logic for UserCodeView.xaml
    /// </summary>
    public partial class UserCodeView : Window
    {
        public UserCodeView(UserCodeViewModel userCodeViewModel)
        {
            InitializeComponent();
            DataContext = userCodeViewModel;
        }
    }
}
