using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using Swift.Extensibility.Functions;

namespace Swift.Modules.InputBoxModule
{
    internal class FunctionInfoToInlineCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var fi = (IFunctionInfo)value;
            var span = new Span();
            var inlines = span.Inlines;
            try
            {
                inlines.Add(new Bold(new Run(fi.DisplayName + " ")));
                inlines.Add(new Run("(" + fi.CallName) { Foreground = Brushes.Gray });
                foreach (var al in fi.Aliases)
                {
                    inlines.Add(new Run(", " + al) { Foreground = Brushes.Gray });
                }
                inlines.Add(new Run(") ") { Foreground = Brushes.Gray });
                var first = true;
                foreach (var pi in fi.Parameters)
                {
                    if (first)
                        inlines.Add(new Bold(new Run("(")));
                    if (!first)
                        inlines.Add(new Run(", "));
                    if (pi.IsOptional)
                        inlines.Add(new Run("["));

                    inlines.Add(new Run(pi.Type.Name + " ") { Foreground = Brushes.DeepSkyBlue });
                    inlines.Add(new Run(pi.DisplayName));

                    if (pi.IsOptional)
                        inlines.Add(new Run("]"));

                    first = false;
                }
                if (!first)
                {
                    inlines.Add(new Bold(new Run(")")));
                }
                inlines.Add(new LineBreak());
                inlines.Add(new Run(fi.Description));
            }
            catch { }
            return inlines;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // not used
            return null;
        }
    }
}
