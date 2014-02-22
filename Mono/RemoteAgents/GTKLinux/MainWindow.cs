using System;
using Gtk;
using System.Net.Http;
using System.Threading.Tasks;
using Example.RemoteAgents.GTKLinux.View;

namespace Example.RemoteAgents.GTKLinux
{
    public partial class MainWindow: Gtk.Window
    {
        ViewImpl view;

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
            string currentDate = await view.getCurrentDate();
            if (currentDate != null)
            {
                this.RemoteDate.Buffer.Text = currentDate;
            }
        }
    }
}
