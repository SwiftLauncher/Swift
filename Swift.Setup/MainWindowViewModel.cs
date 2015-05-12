using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using NuGet;
using Ookii.Dialogs.Wpf;

namespace Swift.Setup
{
    public class MainWindowViewModel : BindableBase
    {
        private const string SWIFT_REPOSITORY_PATH = "https://www.myget.org/F/swift/";
        private const string PLUGINS_REPOSITORY_PATH = "https://www.myget.org/F/swift-packages/";

        #region Properties

        private string _topText;
        public string TopText
        {
            get { return _topText; }
            set { SetProperty(ref _topText, value); }
        }

        private string _centerText;
        public string CenterText
        {
            get { return _centerText; }
            set { SetProperty(ref _centerText, value); }
        }

        private string _installPath;
        public string InstallPath
        {
            get { return _installPath; }
            set
            {
                SetProperty(ref _installPath, value);
                InstallCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _progressVisible;
        public bool ProgressVisible
        {
            get { return _progressVisible; }
            set { SetProperty(ref _progressVisible, value); }
        }

        private DelegateCommand _installCommand;
        public DelegateCommand InstallCommand
        {
            get
            {
                return _installCommand ?? (_installCommand = new DelegateCommand(async () =>
                {
                    try
                    {
                        var path = Path.Combine(InstallPath, "Modules");
                        var ppath = Path.Combine(InstallPath, "Plugins");
                        await Task.Run(() =>
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            CenterText = "Installing...";
                            ProgressVisible = true;
                            var spm = new PackageManager(PackageRepositoryFactory.Default.CreateRepository(SWIFT_REPOSITORY_PATH), new DefaultPackagePathResolver(SWIFT_REPOSITORY_PATH), new PhysicalFileSystem(path));
                            var p = spm.SourceRepository.FindPackage("Swift");
                            spm.InstallPackage(p, true, true);

                            var ppm = new PackageManager(PackageRepositoryFactory.Default.CreateRepository(PLUGINS_REPOSITORY_PATH), new DefaultPackagePathResolver(PLUGINS_REPOSITORY_PATH), new PhysicalFileSystem(ppath));
                            var pp = ppm.SourceRepository.FindPackage("Swift.Plugins.Login");
                            if (pp != null)
                            {
                                ppm.InstallPackage(pp, true, true);
                            }
                            var spec = ppm.SourceRepository.FindPackage("Spec");
                            if (spec != null)
                            {
                                ppm.InstallPackage(spec, true, true);
                            }
                            CenterText = "Installation successful.";
                        });
                        if (MessageBox.Show("Swift is now successfully installed. Do you want to start Swift?", "Swift Setup - Success", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            var swiftexe = Directory.GetFiles(path, "Swift.exe", SearchOption.AllDirectories).First();
                            Process.Start(swiftexe);
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                Application.Current.MainWindow.Close();
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Swift could not be installed because of the following error:" + Environment.NewLine + Environment.NewLine + ex.Message, "Swift Setup - Error");
                    }
                    finally
                    {
                        ProgressVisible = false;
                    }
                }, () =>
                {
                    return !String.IsNullOrWhiteSpace(InstallPath);
                }));
            }
        }

        private DelegateCommand _selectPathCommand;
        public DelegateCommand SelectPathCommand
        {
            get
            {
                return _selectPathCommand ?? (_selectPathCommand = new DelegateCommand(() =>
                {
                    var ofd = new VistaFolderBrowserDialog() { UseDescriptionForTitle = true, Description = "Select the installation path for Swift", ShowNewFolderButton = true };
                    if ((bool)ofd.ShowDialog())
                    {
                        InstallPath = ofd.SelectedPath;
                    }
                }));
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            TopText = "Swift Setup";
            CenterText = "Choose an installation location:";
            InstallPath = "";
            ProgressVisible = false;
        }
    }
}
