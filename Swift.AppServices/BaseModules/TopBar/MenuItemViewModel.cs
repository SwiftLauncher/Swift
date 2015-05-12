using Microsoft.Practices.Prism.Commands;
using Swift.Extensibility.UI;
using Swift.Toolkit;
using System.Windows.Media.Imaging;

namespace Swift.AppServices.BaseModules
{
    public class MenuItemViewModel : IViewModel<MenuItem>
    {
        #region Properties

        private DelegateCommand _executeCommand;
        public DelegateCommand ExecuteCommand
        {
            get
            {
                return _executeCommand ?? (_executeCommand = new DelegateCommand(() =>
                {
                    Model.OnClickCallback();
                }));
            }
        }

        public BitmapImage Icon { get; private set; }

        public MenuItem Model { get; private set; }

        #endregion

        public MenuItemViewModel(MenuItem item)
        {
            Model = item;
            if (Model.IconSource != null)
                Icon = new BitmapImage(Model.IconSource);
        }
    }
}
