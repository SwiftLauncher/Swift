using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swift.ItemModel
{
    public class Input
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

        private Input()
        {
            FunctionParts = new List<string>();
        }

        /// <summary>
        /// Returns an <see cref="Input"/> representing the given string.
        /// </summary>
        /// <param name="text">The input string to be parsed.</param>
        /// <returns>An <see cref="Input"/> object representing the given input string.</returns>
        public static Input Parse(string text)
        {
            var i = new Input();
            i.Value = text;
            if (text.Contains(":"))
            {
                i.Type = InputType.Function;
                i.FunctionParts = new List<string>(text.Substring(0, text.IndexOf(":") - 1).Replace(" ", "").Split('.'));
            }
            else if (text.Contains("."))
            {
                i.Type = InputType.Function;
                i.FunctionParts = new List<string>(text.Substring(0, text.IndexOf(" ") - 1).Split('.'));
            }
            else
            {
                i.Type = InputType.General;
            }
            return i;
        }
    }

    public enum InputType
    {
        Function,
        General
    }
}
