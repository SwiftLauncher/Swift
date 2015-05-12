using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift
{
    public static class InternalConstants
    {
        public static class InitializationPriorities
        {
            public const int PluginManager = int.MinValue + 1;
            public const int StorageManager = int.MinValue + 2;
            public const int PluginServices = int.MinValue + 3;
        }

        public static class ShutdownPriorities
        {
            public const int PluginServices = int.MaxValue - 2;
            public const int PluginManager = int.MaxValue - 1;
            public const int StorageManager = int.MaxValue;
        }

        public static class EventNames
        {
            public const string WindowStateChangeRequested = "Swift.Internal.WindowStateChangeRequested";
            public const string FocusChangeRequested = "Swift.Internal.FocusChangeRequested";
            public const string ExecutionRequested = "Swift.Internal.ExecutionRequested";
        }
    }
}
