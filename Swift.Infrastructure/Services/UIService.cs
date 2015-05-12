using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility;
using Swift.Extensibility.Services.Logging;
using Swift.Extensibility.UI;

namespace Swift.Infrastructure.UI
{
    /// <summary>
    /// UIService implementation.
    /// </summary>
    [Export(typeof(IUIService))]
    public class UIService : IUIService
    {
        private ILoggingChannel _log;

        [ImportingConstructor]
        public UIService(ILogger logger)
        {
            _log = logger.GetChannel<UIService>();
        }

        /// <summary>
        /// Navigates the specified view to the given viewmodel.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        /// <param name="target">The target.</param>
        public void Navigate(IViewModel viewModel, string target)
        {
            var vms = ServiceLocator.Current.GetAllInstances<IViewModel>();
            var targetvm = vms.FirstOrDefault(_ =>
                _.GetType().GetProperties().Any(p =>
                    Attribute.IsDefined(p, typeof(NavigationTargetAttribute))
                    && (Attribute.GetCustomAttribute(p, typeof(NavigationTargetAttribute)) as NavigationTargetAttribute).Name == target));
            if (targetvm != null)
            {
                var property = targetvm.GetType().GetProperties().First(_ => Attribute.IsDefined(_, typeof(NavigationTargetAttribute))
                    && (Attribute.GetCustomAttribute(_, typeof(NavigationTargetAttribute)) as NavigationTargetAttribute).Name == target);
                property.SetValue(targetvm, viewModel);
            }
        }

        public void AddUIResource(Uri resourceDictionaryUri)
        {
            try
            {
                var r = new ResourceDictionary() { Source = resourceDictionaryUri };
                Application.Current.Resources.MergedDictionaries.Add(r);
            }
            catch (Exception ex)
            {
                // TODO handle this better and add return value
                _log.Log("Exception in AddUIResource: " + ex.Message);
            }
        }

        /// <summary>
        /// Registers the menu item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// The corresponding <see cref="T:Swift.Extensibility.UI.RegisterMenuItemResult" />.
        /// </returns>
        public RegisterMenuItemResult RegisterMenuItem(MenuItem item)
        {
            try
            {
                if (!ServiceLocator.Current.GetInstance<IPluginManager>().MenuItems.Contains(item))
                {
                    ServiceLocator.Current.GetInstance<IPluginManager>().MenuItems.Add(item);
                    return RegisterMenuItemResult.Successful;
                }
                else
                {
                    return RegisterMenuItemResult.AlreadyExisting;
                }
            }
            catch
            {
                return RegisterMenuItemResult.Failed;
            }
        }


        /// <summary>
        /// Executes the given <see cref="T:System.Action" /> on the UI-Thread.
        /// </summary>
        /// <param name="callback">The <see cref="T:System.Action" /> to be executed on the UI-Thread.</param>
        public void UIDispatch(Action callback)
        {
            Application.Current.Dispatcher.Invoke(callback, TimeSpan.FromSeconds(5));
        }

        /// <summary>
        /// Executes the given <see cref="!:Func" /> on the UI-Thread.
        /// </summary>
        /// <typeparam name="T">The return type of the function to be dispatched.</typeparam>
        /// <param name="callback">The <see cref="!:Func" /> to be executed on the UI-Thread.</param>
        /// <returns></returns>
        public T UIDispatch<T>(Func<T> callback)
        {
            var c = new CancellationToken();
            return Application.Current.Dispatcher.Invoke<T>(callback, DispatcherPriority.Normal, c, TimeSpan.FromSeconds(5));
        }
    }
}
