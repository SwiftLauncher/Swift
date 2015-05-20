using Microsoft.Practices.Prism.Mvvm;
using Swift.Extensibility;
using Swift.Extensibility.Logging;
using Swift.Extensibility.Services.Logging;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using Swift.Extensibility.UI;

namespace Swift.ViewModels.Shell
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
            tb.Text = logger.Channels.Select(_ => _.Messages).Aggregate("", (s, t) => s += t.Aggregate("", (x, y) => x += y.ToString() + Environment.NewLine));
            logger.MessageAdded += (s) => ps.GetService<IUIService>().UIDispatch(() => tb.Text += s.Message.ToString() + Environment.NewLine);
            w.Show();
        }

        //private class LogTestFunction : Function, ILogSource
        //{
        //    private ILoggingChannel _log;

        //    public LogTestFunction()
        //        : base("Log Test", "test.log")
        //    {

        //    }

        //    public override void Execute()
        //    {
        //        _log.Log("Test LogEntry");
        //    }

        //    public void SetLoggingChannel(ILoggingChannel channel)
        //    {
        //        _log = channel;
        //    }
        //}
    }
}
