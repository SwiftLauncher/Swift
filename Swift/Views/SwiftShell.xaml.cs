using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility.Events;
using Swift.Extensibility.UI;
using Swift.ViewModels;

namespace Swift.Views
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    [Export(typeof(SwiftShell))]
    public partial class SwiftShell : Window
    {
        public SwiftShell()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        //private void OnFocusChangeRequested(FocusChangeRequestedEventArgs args)
        //{
        //if (args.Target == FocusTargets.ViewSwitcherRegionTarget && ViewSwitcher.HasContent)
        //{
        //    var content = (ViewSwitcher.Content as FrameworkElement);
        //    if (content != null)
        //        content.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
        //}
        //}
    }
}
