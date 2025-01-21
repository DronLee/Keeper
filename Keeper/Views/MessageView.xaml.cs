using System.Windows;
using Keeper.ViewModels;

namespace Keeper.Views
{
    /// <summary>
    /// Interaction logic for MessageView.xaml
    /// </summary>
    public sealed partial class MessageView : Window
    {
        public MessageView(MessageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}