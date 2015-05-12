using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Swift.AppServices.Extensibility;
using Swift.Extensibility;
using Swift.Extensibility.Functions;
using Swift.Extensibility.Input;

namespace Swift.AppServices
{
    /// <summary>
    /// Handles all function-related services.
    /// </summary>
    [Export(typeof(IFunctionInfoProvider))]
    [Export(typeof(IFunctionProvider))]
    internal class SwiftFunctionHandler : IFunctionInfoProvider, IFunctionProvider
    {
        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<Function> _functions;

        #region IFunctionInfoProvider Implementation

        /// <summary>
        /// Gets the function infos.
        /// </summary>
        /// <value>
        /// The function infos.
        /// </value>
        public IEnumerable<IFunctionInfo> FunctionInfos
        {
            get { return _functions.Cast<IFunctionInfo>(); }
        }

        /// <summary>
        /// Gets the matching function infos.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public IEnumerable<IFunctionInfo> GetMatchingFunctionInfos(IInput input)
        {
            // TODO improve this
            return _functions.Where(_ =>
            {
                return !String.IsNullOrWhiteSpace(input.Value) && (_.CallName.StartsWith(input.Value) || _.DisplayName.StartsWith(input.Value));
            });
        }

        #endregion

        #region IFunctionProvider Implementation

        /// <summary>
        /// Gets the functions.
        /// </summary>
        /// <value>
        /// The functions.
        /// </value>
        public IEnumerable<Function> Functions
        {
            get { return _functions; }
        }

        /// <summary>
        /// Gets the matching functions.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        public IEnumerable<Function> GetMatchingFunctions(IInput input, FunctionMatchMode mode)
        {
            // TODO improve this
            return _functions.Where(_ =>
            {
                if (mode == FunctionMatchMode.Suggestion)
                    return !String.IsNullOrWhiteSpace(input.Value) && (_.CallName.StartsWith(input.Value) || _.DisplayName.StartsWith(input.Value));
                if (mode == FunctionMatchMode.Execution)
                    return !String.IsNullOrWhiteSpace(input.Value) && (_.CallName == input.Value);
                return false;
            });
        }

        #endregion
    }
}
