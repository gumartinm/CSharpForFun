
// This file has been generated by the GUI designer. Do not modify.
namespace Example.RemoteAgents.GTKLinux
{
	public partial class MainWindow
	{
		private global::Gtk.VBox vbox3;

		private global::Gtk.TextView RemoteDate;

		private global::Gtk.HBox hbox1;

		private global::Gtk.ScrolledWindow GtkScrolledWindow;

		private global::Gtk.TextView TextToSend;

		private global::Gtk.SpinButton SpinButtonNumbers;

		private global::Gtk.HBox hbox2;

		private global::Gtk.Button ButtonGetDate;

		private global::Gtk.Button SendDataButton;

		protected virtual void Build ()
		{
			global::Stetic.Gui.Initialize (this);
			// Widget Example.RemoteAgents.GTKLinux.MainWindow
			this.Name = "Example.RemoteAgents.GTKLinux.MainWindow";
			this.Title = global::Mono.Unix.Catalog.GetString ("MainWindow");
			this.WindowPosition = ((global::Gtk.WindowPosition)(4));
			// Container child Example.RemoteAgents.GTKLinux.MainWindow.Gtk.Container+ContainerChild
			this.vbox3 = new global::Gtk.VBox ();
			this.vbox3.Name = "vbox3";
			this.vbox3.Spacing = 6;
			// Container child vbox3.Gtk.Box+BoxChild
			this.RemoteDate = new global::Gtk.TextView ();
			this.RemoteDate.CanFocus = true;
			this.RemoteDate.Name = "RemoteDate";
			this.RemoteDate.Editable = false;
			this.RemoteDate.CursorVisible = false;
			this.vbox3.Add (this.RemoteDate);
			global::Gtk.Box.BoxChild w1 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.RemoteDate]));
			w1.Position = 0;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox1 = new global::Gtk.HBox ();
			this.hbox1.Name = "hbox1";
			this.hbox1.Spacing = 6;
			// Container child hbox1.Gtk.Box+BoxChild
			this.GtkScrolledWindow = new global::Gtk.ScrolledWindow ();
			this.GtkScrolledWindow.Name = "GtkScrolledWindow";
			this.GtkScrolledWindow.ShadowType = ((global::Gtk.ShadowType)(1));
			// Container child GtkScrolledWindow.Gtk.Container+ContainerChild
			this.TextToSend = new global::Gtk.TextView ();
			this.TextToSend.CanFocus = true;
			this.TextToSend.Name = "TextToSend";
			this.GtkScrolledWindow.Add (this.TextToSend);
			this.hbox1.Add (this.GtkScrolledWindow);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.GtkScrolledWindow]));
			w3.Position = 0;
			// Container child hbox1.Gtk.Box+BoxChild
			this.SpinButtonNumbers = new global::Gtk.SpinButton (0, 1000, 10);
			this.SpinButtonNumbers.CanFocus = true;
			this.SpinButtonNumbers.Name = "SpinButtonNumbers";
			this.SpinButtonNumbers.Adjustment.PageIncrement = 10;
			this.SpinButtonNumbers.ClimbRate = 1;
			this.SpinButtonNumbers.Numeric = true;
			this.hbox1.Add (this.SpinButtonNumbers);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.hbox1 [this.SpinButtonNumbers]));
			w4.Position = 1;
			w4.Fill = false;
			this.vbox3.Add (this.hbox1);
			global::Gtk.Box.BoxChild w5 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox1]));
			w5.Position = 1;
			w5.Fill = false;
			// Container child vbox3.Gtk.Box+BoxChild
			this.hbox2 = new global::Gtk.HBox ();
			this.hbox2.Name = "hbox2";
			this.hbox2.Spacing = 6;
			// Container child hbox2.Gtk.Box+BoxChild
			this.ButtonGetDate = new global::Gtk.Button ();
			this.ButtonGetDate.CanFocus = true;
			this.ButtonGetDate.Name = "ButtonGetDate";
			this.ButtonGetDate.UseUnderline = true;
			this.ButtonGetDate.Label = global::Mono.Unix.Catalog.GetString ("GetRemoteDate");
			this.hbox2.Add (this.ButtonGetDate);
			global::Gtk.Box.BoxChild w6 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.ButtonGetDate]));
			w6.Position = 0;
			w6.Expand = false;
			w6.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.SendDataButton = new global::Gtk.Button ();
			this.SendDataButton.CanFocus = true;
			this.SendDataButton.Name = "SendDataButton";
			this.SendDataButton.UseUnderline = true;
			this.SendDataButton.Label = global::Mono.Unix.Catalog.GetString ("SendData");
			this.hbox2.Add (this.SendDataButton);
			global::Gtk.Box.BoxChild w7 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.SendDataButton]));
			w7.Position = 2;
			w7.Expand = false;
			w7.Fill = false;
			this.vbox3.Add (this.hbox2);
			global::Gtk.Box.BoxChild w8 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox2]));
			w8.Position = 2;
			w8.Expand = false;
			w8.Fill = false;
			this.Add (this.vbox3);
			if ((this.Child != null)) {
				this.Child.ShowAll ();
			}
			this.DefaultWidth = 484;
			this.DefaultHeight = 332;
			this.Show ();
			this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		}
	}
}
