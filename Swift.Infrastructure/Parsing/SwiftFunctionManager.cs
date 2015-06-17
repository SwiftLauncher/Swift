using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Swift.Extensibility.Input;
using Swift.Extensibility.Input.Functions;
using Swift.Extensibility.Services;
using ParameterInfo = Swift.Extensibility.Input.Functions.ParameterInfo;

namespace Swift.Infrastructure.Parsing
{
    [Export(typeof(IFunctionManager))]
    public class SwiftFunctionManager : IFunctionManager, IInitializationAware
    {
        private Dictionary<FunctionInfo, Tuple<MethodInfo, ISwiftFunctionSource>> _functions;

        public IEnumerable<FunctionInfo> GetFunctions() => _functions.Keys;

        public FunctionInfo GetMatchingFunction(Input input) => _functions.FirstOrDefault(_ => _.Key.FullName == input.FunctionDescriptor).Key;

        public bool HasMatchingFunction(Input input) => _functions.Keys.Any(_ => _.FullName == input.FunctionDescriptor);
        public void Invoke(FunctionInfo info, Input input, SwiftFunctionCallContext context)
        {
            // TODO correct handling of parameters
            var f = _functions[info];
            f.Item1.Invoke(f.Item2, input == null ? new object[] { } : new object[] { input.TextValue, context });
        }

        public void OnInitialization(InitializationEventArgs args)
        {
            var functionSources = ServiceLocator.Current.GetAllInstances<ISwiftFunctionSource>();
            var functions =
                functionSources.SelectMany(
                    _ =>
                        _.GetType()
                            .GetMethods()
                            .Where(m => Attribute.IsDefined(m, typeof(SwiftFunctionAttribute))).Select(mi => Tuple.Create(mi, _)));
            _functions = new Dictionary<FunctionInfo, Tuple<MethodInfo, ISwiftFunctionSource>>();
            foreach (var f in functions)
            {
                var att = (SwiftFunctionAttribute)Attribute.GetCustomAttribute(f.Item1, typeof(SwiftFunctionAttribute));
                var paraatts = Attribute.GetCustomAttributes(f.Item1, typeof(ParameterDescriptionAttribute)).Cast<ParameterDescriptionAttribute>();
                var para = f.Item1.GetParameters().Select(p =>
                {
                    var descratt = paraatts.FirstOrDefault(a => a.ParameterName == p.Name);
                    return new ParameterInfo(p.Name, p.ParameterType, descratt?.Description ?? "", descratt?.ExampleUsage ?? "");
                });
                _functions.Add(new FunctionInfo(att.FunctionName, att.CallMode, para), f);
            }
        }

        public int InitializationPriority => int.MinValue;
    }
}
