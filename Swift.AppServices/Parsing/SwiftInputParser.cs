using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Swift.Extensibility.Input;

namespace Swift.Modules.Services.Parsing
{
    public class SwiftInputParser : IInputParser
    {
        /// <summary>
        /// Returns an <see cref="Input"/> representing the given string.
        /// </summary>
        /// <param name="text">The input string to be parsed.</param>
        /// <returns>An <see cref="Input"/> object representing the given input string.</returns>
        public IInput Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
                return Input.Empty;
            var value = text;
            InputType type;
            List<string> functionParts;
            if (text.Contains(":"))
            {
                type = InputType.Function;
                functionParts = new List<string>(text.Substring(0, text.IndexOf(":") - 1).Replace(" ", "").Split('.'));
            }
            else if (text.Contains(".") && text.Length > 0)
            {
                type = InputType.Function;
                functionParts = new List<string>(text.Substring(0, text.IndexOf(" ") > -1 ? text.IndexOf(" ") - 1 : text.Length).Split('.'));
            }
            else
            {
                type = InputType.General;
                var l = new List<string>();
                l.Add(text);
                functionParts = l;
            }
            return new Input(value, type, functionParts);
        }
    }
}
