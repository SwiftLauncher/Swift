using Microsoft.Practices.Unity;
using Swift.Extensibility.Input;
using Swift.Extensibility.Services;
using Swift.Extensibility.Services.Profile;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift.Infrastructure.Extensibility
{
    public sealed class ExtensionRegistry : IExtensionRegistry
    {
        private static readonly List<Type> _extensionPoints = new List<Type> {
            typeof(IProfileProvider),
            typeof(IDataItemSource)
        };

        public static ExtensionRegistry Current { get; } = new ExtensionRegistry();

        private UnityContainer _container = new UnityContainer();

        #region IExtensionRegistry Implementation

        public IEnumerable<Type> GetExtensionPoints() => _extensionPoints;

        public void RegisterExtension<T>(T extension)
        {
            if (_extensionPoints.Any(_ => _ == typeof(T)))
            {
                _container.RegisterInstance(extension);
            }
        }

        #endregion

        #region Methods

        public T GetExtension<T>()
        {
            return _container.Resolve<T>();
        }

        public IEnumerable<T> GetExtensions<T>()
        {
            return _container.ResolveAll<T>();
        }

        #endregion

        private ExtensionRegistry() { }
    }
}
