using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility;
using Swift.Extensibility.Events;
using Swift.Extensibility.UI;

namespace Swift.ViewModels
{
    /// <summary>
    /// Main viewmodel for the Swift shell.
    /// </summary>
    [Export(typeof(ShellViewModel))]
    public class ShellViewModel : ViewModelBase
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        #region Properties

        /// <summary>
        /// Backing field for <see cref="TopBarViewModel"/>.
        /// </summary>
        private IViewModel _topBarViewModel;
        /// <summary>
        /// Gets or sets the top bar view model.
        /// </summary>
        /// <value>
        /// The top bar view model.
        /// </value>
        [NavigationTarget(ViewTargetsInternal.TopBar)]
        public IViewModel TopBarViewModel { get { return _topBarViewModel; } set { Set(ref _topBarViewModel, value); } }

        /// <summary>
        /// Backing field for <see cref="CenterViewModel"/>.
        /// </summary>
        private IViewModel _centerViewModel;
        /// <summary>
        /// Gets or sets the center view model.
        /// </summary>
        /// <value>
        /// The center view model.
        /// </value>
        [NavigationTarget(ViewTargetsInternal.CenterView)]
        public IViewModel CenterViewModel { get { return _centerViewModel; } set { Set(ref _centerViewModel, value); } }

        /// <summary>
        /// Backing field for <see cref="BottomBarViewModel"/>.
        /// </summary>
        private IViewModel _bottomBarViewModel;
        /// <summary>
        /// Gets or sets the bottom bar view model.
        /// </summary>
        /// <value>
        /// The bottom bar view model.
        /// </value>
        [NavigationTarget(ViewTargetsInternal.BottomBar)]
        public IViewModel BottomBarViewModel { get { return _bottomBarViewModel; } set { Set(ref _bottomBarViewModel, value); } }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        [ImportingConstructor]
        public ShellViewModel()
        {
            ServiceLocator.Current.GetInstance<IEventBroker>().GetChannel<WindowStateChangeRequestedEventArgs>(InternalConstants.EventNames.WindowStateChangeRequested).Subscribe(OnWindowStateChangeRequested);
        }

        /// <summary>
        /// Handles the <see cref="E:WindowStateChangeRequested" /> event.
        /// </summary>
        /// <param name="args">The <see cref="WindowStateChangeRequestedEventArgs"/> instance containing the event data.</param>
        private void OnWindowStateChangeRequested(WindowStateChangeRequestedEventArgs args)
        {
            var w = App.Current.MainWindow;
            if (args.TargetState == Extensibility.Events.WindowState.Hidden || (w.IsVisible && args.TargetState == Extensibility.Events.WindowState.Toggle))
            {
                w.Hide();
            }
            else if (args.TargetState == Extensibility.Events.WindowState.Shown || (!w.IsVisible && args.TargetState == Extensibility.Events.WindowState.Toggle))
            {
                w.Show();
            }
            else if (args.TargetState == Extensibility.Events.WindowState.Foreground)
            {
                SetForegroundWindow(new WindowInteropHelper(w).Handle);
            }
        }
    }
}
