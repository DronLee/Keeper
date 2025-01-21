using Data.Abstractions;
using Keeper.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Keeper.ViewModels
{
    public class DataRowViewModel
    {
        private readonly IApplicationLanguageManager _languageManager;

        private ICommand _passwordClickCommand;
        private ICommand _passwordToBuferCommand;
        private ICommand _editItemCommand;
        private ICommand _deleteItemCommand;
        private ICommand _okCommand;
        private ICommand _generatePasswordCommand;

        public DataRowViewModel(DataRow dataRow, IApplicationLanguageManager languageManager)
        {
            DataRow = dataRow;
            Name = DataRow.Name;
            Login = DataRow.Login;
            Password= DataRow.Password;
            Section= DataRow.Section;
            _languageManager = languageManager;
        }

        /// <summary>
        /// Text is shown in Password field
        /// </summary>
        public string PasswordView => PasswordIsHided ? _languageManager.GetLocalizedString("ShowPassword") : DataRow.Password;

        /// <summary>
        /// Name of keeping information
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A login for authentication
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// A password for authentication
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Name of section for data row
        /// </summary>
        public string Section { get; set; }

        /// <summary>
        /// A command of click to Password field
        /// </summary>
        public ICommand PasswordClickCommand
        {
            get
            {
                if (_passwordClickCommand == null)
                    _passwordClickCommand = new Command(x => HidePassword());
                return _passwordClickCommand;
            }
        }

        /// <summary>
        /// A command for copy password to buffer
        /// </summary>
        public ICommand PasswordToBuferCommand
        {
            get
            {
                if (_passwordToBuferCommand == null)
                    _passwordToBuferCommand = new Command(x => PasswordToBufer());
                return _passwordToBuferCommand;
            }
        }

        /// <summary>
        /// A command of editing row
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                if (_editItemCommand == null)
                    _editItemCommand = new Command(x => Edit());
                return _editItemCommand;
            }
        }

        /// <summary>
        /// A command of deleting row
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteItemCommand == null)
                    _deleteItemCommand = new Command(x => Delete());
                return _deleteItemCommand;
            }
        }

        /// <summary>
        /// A command of applying row changes
        /// </summary>
        public ICommand OkCommand
        {
            get
            {
                if (_okCommand == null)
                    _okCommand = new Command<Window>(ApplyChange);
                return _okCommand;
            }
        }

        /// <summary>
        /// A password generation command
        /// </summary>
        public ICommand GeneratePasswordCommand
        {
            get
            {
                if (_generatePasswordCommand == null)
                    _generatePasswordCommand = new Command(x => GeneratePassword());
                return _generatePasswordCommand;
            }
        }

        /// <summary>
        /// True - row was changed
        /// </summary>
        public bool Changed { get; private set; } = false;

        /// <summary>
        /// True - row is not valuable
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Login) && string.IsNullOrEmpty(Password);

        /// <summary>
        /// True - password was hided
        /// </summary>
        public bool PasswordIsHided { get; private set; } = true;

        /// <summary>
        /// True -  connect with data provider was successful
        /// </summary>
        public bool Online { get; set; } = true;

        /// <summary>
        /// Model of row
        /// </summary>
        public DataRow DataRow { get; private set; }

        /// <summary>
        /// An event of changing some property
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// An event of deleting row
        /// </summary>
        public event DataRowViewModelDelegate DeletedEvent;

        /// <summary>
        /// An event of click to edit for row
        /// </summary>
        public event DataRowViewModelDelegate EditClick;

        /// <summary>
        /// To hide password
        /// </summary>
        public void HidePassword()
        {
            PasswordIsHided = !PasswordIsHided;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PasswordView"));
        }

        private void PasswordToBufer()
        {
            Clipboard.SetText(Password);
        }

        private void Edit()
        {
            Changed = false;
            EditClick?.Invoke(this);
        }

        private void Delete()
        {
            DeletedEvent?.Invoke(this);
        }

        private void GeneratePassword()
        {
            Password = Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 20);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Password"));
        }

        private void ApplyChange(Window window)
        {
            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Name))
                return;

            if (Name != DataRow.Name)
            {
                DataRow.Name = Name;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Name)));
            }
            if (Login != DataRow.Login)
            {
                DataRow.Login = Login;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Login)));
            }
            if (Password != DataRow.Password)
            {
                DataRow.Password = Password;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Password)));
            }
            DataRow.Section = Section;
            Changed = true;
            if (window != null)
                window.Close();
        }
    }
}