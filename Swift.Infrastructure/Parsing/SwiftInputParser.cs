using System.ComponentModel.Composition;
using System.Linq;
using Swift.Extensibility.Input;

namespace Swift.Infrastructure.Parsing
{
    /// <summary>
    /// Swift implementation of IInputParser.
    /// </summary>
    [Export(typeof(IInputParser))]
    public class SwiftInputParser : IInputParser
    {
        /// <summary>
        /// Returns an <see cref="Input"/> representing the given string.
        /// </summary>
        /// <param name="text">The input string to be parsed.</param>
        /// <returns>An <see cref="Input"/> object representing the given input string.</returns>
        public Input Parse(string text)
        {
            if (string.IsNullOrEmpty(text))
                return new Input("", "");
            var parts = text.Split(' ');
            if (parts[0].StartsWith("."))
            {
                // Function
                return new Input(text, parts[0], parts.Skip(1).ToArray());
            }
            return new Input(text, "");
        }
    }
}
