using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility.Input.Functions;
using Swift.Extensibility.UI;
using Swift.Toolkit;

namespace Swift.Infrastructure.BaseModules.TopBar
{
    public class MenuItemViewModel : IViewModel<MenuItem>
    {
        public DelegateCommand ExecuteCommand { get; }

        public BitmapImage Icon { get; }

        public MenuItem Model { get; }

        public MenuItemViewModel(MenuItem item)
        {
            Model = item;
            if (Model.IconSource != null)
                Icon = new BitmapImage(Model.IconSource);
            ExecuteCommand = new DelegateCommand(() =>
            {
                var fm = ServiceLocator.Current.GetInstance<IFunctionManager>();
                var f = fm.GetFunctions().FirstOrDefault(_ => _.FullName == Model.Function);
                if (f != null)
                {
                    fm.Invoke(f, Model.FunctionInput, new SwiftFunctionCallContext(FunctionCallOrigin.CodeCall));
                }
            });
        }
    }
}
