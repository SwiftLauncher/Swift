using System.Collections.Generic;
using Swift.Extensibility.Functions;
using Swift.Extensibility.Input;

namespace Swift.AppServices.Extensibility
{
    public interface IFunctionProvider
    {
        IEnumerable<Function> Functions { get; }

        IEnumerable<Function> GetMatchingFunctions(IInput input, FunctionMatchMode mode);
    }

    /// <summary>
    /// Defines Function match modes.
    /// </summary>
    public enum FunctionMatchMode
    {
        /// <summary>
        /// In this mode, function should only match on exact matches to prevent wrong execution.
        /// </summary>
        Execution,
        /// <summary>
        /// In this mode, functions should be suggested that start with the input.
        /// </summary>
        Suggestion
    }
}
