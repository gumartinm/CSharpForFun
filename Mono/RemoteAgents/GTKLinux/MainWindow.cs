using System;
using Gtk;
using System.Net.Http;
using System.Threading.Tasks;
using Example.RemoteAgents.GTKLinux.View;
using log4net;

namespace Example.RemoteAgents.GTKLinux
{
    public partial class MainWindow: Gtk.Window
    {
        ViewImpl view;
        private static readonly ILog logger = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	    public MainWindow () : base (Gtk.WindowType.Toplevel)
	    {
            view = new ViewImpl();
		    Build ();
		    this.RetrieveRemoteDateButton.Clicked += this.ButtonClicked;
            this.ButtonGetDate.Clicked += this.ButtonGetDateClicked;
	    }

	    protected void OnDeleteEvent (object sender, DeleteEventArgs a)
	    {
		    Application.Quit ();
		    a.RetVal = true;
	    }

	    async private void ButtonClicked(object sender, EventArgs a)
	    {
		    using (HttpClient client = new HttpClient ()) {
                Task<string> resultGET = client.GetStringAsync ("http://gumartinm.name");
                this.RemoteDate.Buffer.Text = await resultGET;
	        }
	    }

        async private void ButtonGetDateClicked(object sender, EventArgs a)
        {
            try {
                string currentDate = await view.GetCurrentDateAsync();
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
