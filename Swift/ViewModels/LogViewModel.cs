using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Swift.Extensibility.Logging;
using Swift.Extensibility.Services;
using Swift.Extensibility.UI;
using Swift.Toolkit;

namespace Swift.ViewModels
{
    [Export]
    public class LogViewModel : BindableBase
    {
        [ImportingConstructor]
        public LogViewModel(ILoggingManager logger, IPluginServices ps)
        {
            var w = new Window();
            var tb = new TextBlock();
            w.Content = tb;
            tb.Text = logger.Channels.Select(_ => _.Messages).Aggregate("", (s, t) => s + t.Aggregate("", (x, y) => x + (y.ToString() + Environment.NewLine)));
            logger.MessageAdded += s => ps.GetService<IUiService>().UiDispatch(() => tb.Text += s.Message + Environment.NewLine);
            w.Show();
        }
    }
}
