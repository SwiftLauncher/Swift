using System.Collections.Generic;
using Swift.Extensibility.Functions;
using Swift.Extensibility.Input;

namespace Swift.AppServices.Extensibility
{
    public interface IFunctionInfoProvider
    {
        IEnumerable<IFunctionInfo> FunctionInfos { get; }

        IEnumerable<IFunctionInfo> GetMatchingFunctionInfos(IInput input);
    }
}
