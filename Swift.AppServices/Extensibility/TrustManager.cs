using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swift.Extensibility.Plugins;

namespace Swift.AppServices.Extensibility
{
    public class TrustManager
    {
        private static TrustManager _instance;
        /// <summary>
        /// Returns the currently active Instance of the <see cref="TrustManager"/>.
        /// </summary>
        public static TrustManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TrustManager();
                return _instance;
            }
            private set { }
        }

        /// <summary>
        /// Contains the public keys of user-trusted assemblies.
        /// </summary>
        private List<string> _trustedPlugins = new List<string>();

        private TrustManager() { }

        /// <summary>
        /// Gets the <see cref="TrustStatus"/> of the given dll. This is one of 4 possibilities:
        ///   a) the Dll is signed with Swift's own key
        ///   b) the Dll is signed with Swift's external key
        ///   c) the Dll has been manually trusted by the user or
        ///   d) the Dll is not trusted.
        /// </summary>
        /// <param name="dll">The full path of the dll to look at.</param>
        /// <returns>The <see cref="TrustStatus"/> of the Dll.</returns>
        public TrustStatus GetTrustStatus(string dll)
        {
            var ass = Assembly.LoadFrom(dll);
            return GetTrustStatus(ass);
        }

        /// <summary>
        /// Gets the <see cref="TrustStatus"/> of the given assembly. This is one of 4 possibilities:
        ///   a) the Dll is signed with Swift's own key
        ///   b) the Dll is signed with Swift's external key
        ///   c) the Dll has been manually trusted by the user or
        ///   d) the Dll is not trusted.
        /// </summary>
        /// <param name="ass">The Assembly to check.</param>
        /// <returns>The <see cref="TrustStatus"/> of the Dll.</returns>
        public TrustStatus GetTrustStatus(Assembly ass)
        {
            var key = ass.GetName().GetPublicKey();
            // Same Key as Swift.Plugins, which is signed with Swift's key.
            if (key.SequenceEqual(Assembly.GetAssembly(typeof(IPlugin)).GetName().GetPublicKey()))
                return TrustStatus.SwiftBuiltin;
            // TODO Swift's publisher key.
            //if (key.SequenceEqual(Assembly.GetAssembly(typeof(IPlugin)).GetName().GetPublicKey()))
            //    return TrustStatus.SwiftBuiltin;
            // Trusted by user, key contained in trusted plugins
            if (_trustedPlugins.Contains(BitConverter.ToString(key)))
                return TrustStatus.TrustedByUser;
            return TrustStatus.Untrusted;
        }

        /// <summary>
        /// Checks wether the assembly might be loaded. This is true, if its <see cref="TrustStatus"/> is not Untrusted.
        /// </summary>
        /// <param name="dll">The dll to check.</param>
        /// <returns>True, if the <see cref="TrustStatus"/> of the Dll is not Untrusted. False otherwise.</returns>
        internal bool MayLoad(string dll)
        {
            var ts = GetTrustStatus(dll);
            return ts != TrustStatus.Untrusted;
        }
    }

    public enum TrustStatus
    {
        SwiftBuiltin,
        TrustedPublisher,
        TrustedByUser,
        Untrusted
    }
}
