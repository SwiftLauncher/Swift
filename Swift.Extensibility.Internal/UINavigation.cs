using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Swift.Extensibility.UI;

namespace Swift.Extensibility
{
    /// <summary>
    /// Attribute to mark navigation targets.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class NavigationTargetAttribute : Attribute
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationTargetAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of this target. Has to be unique.</param>
        public NavigationTargetAttribute(string name)
        {
            Name = name;
        }
    }

    /// <summary>
    /// Contains all available UI navigation targets.
    /// </summary>
    public static class ViewTargetsInternal
    {
        /// <summary>
        /// The top bar
        /// </summary>
        public const string TopBar = "Shell.TopBar";

        /// <summary>
        /// The center view
        /// </summary>
        public const string CenterView = "Shell.CenterView";

        /// <summary>
        /// The bottom bar
        /// </summary>
        public const string BottomBar = "Shell.BottomBar";

        /// <summary>
        /// The input box place holder
        /// </summary>
        public const string InputBoxPlaceHolder = "TopBar.InputBoxPlaceHolder";
    }
}
