using System;
using Gtk;
using System.Threading.Tasks;
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
	    }

	    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	    {
		    Application.Quit ();
		    a.RetVal = true;
	    }

        async private void ButtonGetDateClicked(object sender, EventArgs a)
        {
            try {
                this.RemoteDate.Buffer.Text = await _view.GetCurrentDateAsync();
            }
            catch (Exception e)
            {
                logger.Error("ButtonGetDateClicked error: ", e);
            }
        }
    }
}
