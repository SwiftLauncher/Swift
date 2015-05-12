using System.Collections.Generic;
using System.Linq;
using Swift.Extensibility.Input;

namespace Swift.Modules.Services.Parsing
{
    public sealed class Input : IInput
    {
        /// <summary>
        /// Gets the complete string representation of this <see cref="Input"/>.
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Gets the <see cref="InputType"/> of this Input.
        /// </summary>
        public InputType Type { get; private set; }

        /// <summary>
        /// Gets the Function parts (like note.remove: -> "note","remove") of this <see cref="Input"/>, if this <see cref="Input"/>'s Type is Function. Otherwise, this yields an empty collection.
        /// </summary>
        public IEnumerable<string> FunctionParts { get; private set; }

        public Input(string value, InputType type, IEnumerable<string> functionParts)
        {
            FunctionParts = functionParts;
            Value = value;
            Type = type;
        }

        private static Input _empty = new Input("", InputType.General, Enumerable.Empty<string>());
        public static Input Empty { get { return _empty; } }
    }
}
