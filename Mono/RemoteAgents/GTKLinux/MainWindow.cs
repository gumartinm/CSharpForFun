using System;
using Gtk;
using System.Threading.Tasks;
using Example.RemoteAgents.GTKLinux.View;
using log4net;

namespace Example.RemoteAgents.GTKLinux
{
    public partial class MainWindow: Gtk.Window
    {
        private readonly ViewImpl _view;
        private static readonly ILog logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                string currentDate = await _view.GetCurrentDateAsync();
                if (currentDate != null)
                {
                    this.RemoteDate.Buffer.Text = currentDate;
                }
            }
            catch (Exception e)
            {
                logger.Error("ButtonGetDateClicked error: ", e);
            }
        }
    }
}
