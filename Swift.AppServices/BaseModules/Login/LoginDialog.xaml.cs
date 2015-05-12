using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Swift.AppServices.Extensibility;
using Swift.Extensibility;
using Swift.Extensibility.Services;
using Swift.Extensibility.Services.Profile;

namespace Swift.AppServices.BaseModules
{
    /// <summary>
    /// Interaktionslogik für LoginDialog.xaml
    /// </summary>
    [Export(typeof(ILoginDialog))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoginDialog : Window, INotifyPropertyChanged, ILoginDialog
    {
        #region Properties

        [ImportMany]
        private IEnumerable<IProfileProvider> _profileProviders;
        public IEnumerable<IProfileProvider> ProfileProviders
        {
            get { return _profileProviders; }
            set
            {
                _profileProviders = value;
                RaisePropertyChanged();
            }
        }

        private IProfileProvider _selectedProvider;
        public IProfileProvider SelectedProvider
        {
            get
            {
                return _selectedProvider;
            }
            set
            {
                _selectedProvider = value;
                RaisePropertyChanged();
            }
        }

        private bool _loginViewVisible = false;
        public bool LoginViewVisible
        {
            get { return _loginViewVisible; }
            set
            {
                _loginViewVisible = value;
                RaisePropertyChanged();
            }
        }

        private DelegateCommand _continueWithProviderCommand;
        public DelegateCommand ContinueWithProviderCommand
        {
            get
            {
                return _continueWithProviderCommand ?? (_continueWithProviderCommand = new DelegateCommand(() =>
                {
                    if (SelectedProvider != null)
                    {
                        SelectedProvider.LoginCompleted += LoginEventCallback;
                        LoginViewVisible = true;
                        Task.Run(() => SelectedProvider.LoginAsync());
                    }
                    else
                    {
                        if (MessageBox.Show("If you continue without login, several advanced features will not be available. To learn more, you can read the online help for Swift.", "Swift - Warning", MessageBoxButton.OKCancel, MessageBoxImage.Warning, MessageBoxResult.Cancel) == MessageBoxResult.OK)
                        {
                            UserProfile = new UserProfile("SwiftUser", "SwiftUserPassword");
                            DialogResult = true;
                            _loginResult = LoginResult.Successful;
                            this.Close();
                        }
                    }
                }, () =>
                {
                    return ProfileProviders == null || ProfileProviders.Count() == 0 || (SelectedProvider != null && !LoginViewVisible);
                }));
            }
        }

        public string ContinueWithProviderButtonText
        {
            get
            {
                return (ProfileProviders == null || ProfileProviders.Count() == 0) ? " Continue without Login > " : " Continue > ";
            }
        }

        private static bool _isRetry = false;
        /// <summary>
        /// Gets or sets a value indicating whether this try is a retry.
        /// </summary>
        public bool IsRetry
        {
            get { return _isRetry; }
            set
            {
                _isRetry = value;
                RaisePropertyChanged();
            }
        }


        #endregion

        [ImportingConstructor]
        public LoginDialog()
        {
            InitializeComponent();
            this.DataContext = this;
            _loginResult = LoginResult.Aborted;
        }

        private void LoginEventCallback(bool success)
        {
            if (success)
            {
                this.UserProfile = SelectedProvider.UserProfile;
                this.DialogResult = true;
                _loginResult = LoginResult.Successful;
            }
            else
            {
                this.UserProfile = null;
                this.DialogResult = false;
                _loginResult = LoginResult.Failed;
            }
            this.Close();
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Event Handlers

        private void LBProviders_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ContinueWithProviderCommand.CanExecute())
                ContinueWithProviderCommand.Execute();
        }

        private void Grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && ContinueWithProviderCommand.CanExecute())
                ContinueWithProviderCommand.Execute();
        }

        #endregion

        #region ILoginDialog Implementation

        public IUserProfile UserProfile { get; private set; }

        private LoginResult _loginResult;

        public LoginResult ShowLoginDialog()
        {
            LoginViewVisible = false;
            this.ShowDialog();
            IsRetry = true;
            return this._loginResult;
        }

        #endregion
    }
}
