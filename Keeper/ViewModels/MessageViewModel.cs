using System.Windows;
using System.Windows.Input;

namespace Keeper.ViewModels
{
    public class MessageViewModel
    {
        private ICommand _applyCommand;
        private ICommand _cancelCommand;

        /// <summary>
        /// A message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// A text for button Apply
        /// </summary>
        public string ApplyText { get; set; }

        /// <summary>
        /// A text for button Cancel
        /// </summary>
        public string CancelText { get; set; }

        /// <summary>
        /// True - an action was applied
        /// </summary>
        public bool IsApplied { get; private set; }

        /// <summary>
        /// A command for action applying
        /// </summary>
        public ICommand ApplyCommand
        {
            get
            {
                if (_applyCommand == null)
                    _applyCommand = new Command<Window>((Window window) =>
                    {
                        IsApplied = true;
                        window.Close();
                    });
                return _applyCommand;
            }
        }

        /// <summary>
        /// A command for action canceling
        /// </summary>
        public ICommand CancelCommand
        {
            get
            {
                if (_cancelCommand == null)
                    _cancelCommand = new Command<Window>((Window window) =>
                    {
                        window.Close();
                    });
                return _cancelCommand;

            }
        }
    }
}