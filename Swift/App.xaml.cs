using System;
using System.Reflection;
using System.Windows;

namespace Swift
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private SwiftBootstrapper _bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                _bootstrapper = new SwiftBootstrapper();
                _bootstrapper.Run();
            }
            catch (ReflectionTypeLoadException ex)
            {
                var m = "" + ex.GetType().Name + " occurred while trying to start Swift. This is likely caused by an older Plugin. (" + ex.Message + ")" + Environment.NewLine;
                foreach (var le in ex.LoaderExceptions)
                {
                    m += "- " + le.GetType().Name + ": " + le.Message + Environment.NewLine;
                }
                m += Environment.NewLine + "Swift will now close.";
                MessageBox.Show(m);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _bootstrapper.Shutdown();
            base.OnExit(e);
        }
    }
}
