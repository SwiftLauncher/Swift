using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Swift.Extensibility.Events;
using Swift.Extensibility.Internal;
using Swift.Extensibility.UI;
using WindowState = Swift.Extensibility.Events.WindowState;

namespace Swift.ViewModels
{
    /// <summary>
    /// Main viewmodel for the Swift shell.
    /// </summary>
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : ViewModelBase, INavigationTargetContainer, INavigationAwareTargetContainer
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        #region Properties

        /// <summary>
        /// Backing field for <see cref="TopBarViewModel"/>.
        /// </summary>
        private object _topBarViewModel;
        /// <summary>
        /// Gets or sets the top bar view model.
        /// </summary>
        /// <value>
        /// The top bar view model.
        /// </value>
        [NavigationTarget(ViewTargetsInternal.TopBar)]
        public object TopBarViewModel { get { return _topBarViewModel; } set { Set(ref _topBarViewModel, value); } }

        /// <summary>
        /// Backing field for <see cref="CenterViewModel"/>.
        /// </summary>
        private object _centerViewModel;
        /// <summary>
        /// Gets or sets the center view model.
        /// </summary>
        /// <value>
        /// The center view model.
        /// </value>
        [NavigationTarget(ViewTargetsInternal.CenterView)]
        public object CenterViewModel { get { return _centerViewModel; } set { Set(ref _centerViewModel, value); } }

        /// <summary>
        /// Backing field for <see cref="BottomBarViewModel"/>.
        /// </summary>
        private object _bottomBarViewModel;
        /// <summary>
        /// Gets or sets the bottom bar view model.
        /// </summary>
        /// <value>
        /// The bottom bar view model.
        /// </value>
        [NavigationTarget(ViewTargetsInternal.BottomBar)]
        public object BottomBarViewModel { get { return _bottomBarViewModel; } set { Set(ref _bottomBarViewModel, value); } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public ShellViewModel(IEventBroker eventBroker)
        {
            eventBroker.GetChannel<WindowStateChangeRequestedEventArgs>(InternalConstants.EventNames.WindowStateChangeRequested).Subscribe(OnWindowStateChangeRequested);
        }

        /// <summary>
        /// Handles the <see cref="E:WindowStateChangeRequested" /> event.
        /// </summary>
        /// <param name="args">The <see cref="WindowStateChangeRequestedEventArgs"/> instance containing the event data.</param>
        private static void OnWindowStateChangeRequested(WindowStateChangeRequestedEventArgs args)
        {
            var w = Application.Current.MainWindow;
            if (args.TargetState == WindowState.Hidden || (w.IsVisible && args.TargetState == WindowState.Toggle))
            {
                w.Hide();
            }
            else if (args.TargetState == WindowState.Shown || (!w.IsVisible && args.TargetState == WindowState.Toggle))
            {
                w.Show();
            }
            else if (args.TargetState == WindowState.Foreground)
            {
                SetForegroundWindow(new WindowInteropHelper(w).Handle);
            }
        }

        public NavigationHandlerResult OnIncomingNavigation(object viewModel, string target)
        {
            return NavigationHandlerResult.ContinueNavigation;
        }

        public NavigationHandlerResult OnOutgoingNavigation(string target)
        {
            return NavigationHandlerResult.ContinueNavigation;
        }
    }
}
