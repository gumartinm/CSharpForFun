using System;
using Gtk;
using Example.RemoteAgents.GTKLinux.View;
using NLog;

namespace Example.RemoteAgents.GTKLinux
{
    public partial class MainWindow: Gtk.Window
    {
        private readonly ViewImpl _view;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

	    public MainWindow () : base (Gtk.WindowType.Toplevel)
	    {
            _view = new ViewImpl();
		    Build ();
            this.ButtonGetDate.Clicked += this.ButtonGetDateClicked;
            this.SendDataButton.Clicked += this.SendDataButtonClicked;
	    }

	    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	    {
		    Application.Quit ();
		    a.RetVal = true;
	    }

        async private void ButtonGetDateClicked(object sender, EventArgs e)
        {
            try {
                this.RemoteDate.Buffer.Text = await _view.GetCurrentDateAsync();
            }
            catch (Exception exception)
            {
                logger.Error("ButtonGetDateClicked error: ", exception);
            }
        }

        async private void SendDataButtonClicked(object sender, EventArgs e)
        {
            try {
                await _view.SetWriteTextAsync(this.TextToSend.Buffer.Text,
                                              this.SpinButtonNumbers.Value);
            }
            catch (Exception exception)
            {
                logger.Error("SendDataButtonClicked error: ", exception);
            }
        }
    }
}
