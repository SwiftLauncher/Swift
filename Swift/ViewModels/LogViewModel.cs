using System;
using System.ComponentModel.Composition;
using System.Linq;
using Swift.Extensibility;
using Swift.Extensibility.Logging;
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
            var w = new System.Windows.Window();
            var tb = new System.Windows.Controls.TextBlock();
            w.Content = tb;
            tb.Text = logger.Channels.Select(_ => _.Messages).Aggregate("", (s, t) => s + t.Aggregate("", (x, y) => x + (y.ToString() + Environment.NewLine)));
            logger.MessageAdded += s => ps.GetService<IUiService>().UiDispatch(() => tb.Text += s.Message + Environment.NewLine);
            w.Show();
        }
    }
}
