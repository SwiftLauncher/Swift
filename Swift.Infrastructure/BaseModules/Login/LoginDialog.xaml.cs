using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility.Internal;
using Swift.Extensibility.Services.Profile;

namespace Swift.Infrastructure.BaseModules
{
    /// <summary>
    /// Interaktionslogik für LoginDialog.xaml
    /// </summary>
    [Export(typeof(ILoginDialog))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class LoginDialog : INotifyPropertyChanged, ILoginDialog
    {
        #region Properties

        public IEnumerable<IProfileProvider> ProfileProviders { get; } = ServiceLocator.Current.GetAllInstances<IProfileProvider>();

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

        private bool _loginViewVisible;
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
                            UserProfile = new UserProfile("SwiftUser", new byte[] { 19, 13, 37 });
                            DialogResult = true;
                            _loginResult = LoginResult.Successful;
                            Close();
                        }
                    }
                }, () => ProfileProviders == null || !ProfileProviders.Any() || (SelectedProvider != null && !LoginViewVisible)));
            }
        }

        public string ContinueWithProviderButtonText => (ProfileProviders == null || !ProfileProviders.Any()) ? " Continue without Login > " : " Continue > ";

        private static bool _isRetry;
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
            DataContext = this;
            _loginResult = LoginResult.Aborted;
        }

        private void LoginEventCallback(bool success)
        {
            if (success)
            {
                UserProfile = SelectedProvider.UserProfile;
                DialogResult = true;
                _loginResult = LoginResult.Successful;
            }
            else
            {
                UserProfile = null;
                DialogResult = false;
                _loginResult = LoginResult.Failed;
            }
            Close();
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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

        public UserProfile UserProfile { get; private set; }

        private LoginResult _loginResult;

        public LoginResult ShowLoginDialog()
        {
            LoginViewVisible = false;
            ShowDialog();
            IsRetry = true;
            return _loginResult;
        }

        #endregion
    }
}
