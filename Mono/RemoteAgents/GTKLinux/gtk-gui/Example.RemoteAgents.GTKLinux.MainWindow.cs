
// This file has been generated by the GUI designer. Do not modify.
namespace Example.RemoteAgents.GTKLinux
{
	public partial class MainWindow
	{
		private global::Gtk.VBox vbox3;
		private global::Gtk.TextView RemoteDate;
		private global::Gtk.HBox hbox2;
		private global::Gtk.Button ButtonGetDate;
		private global::Gtk.Button RetrieveRemoteDateButton;

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
			global::Gtk.Box.BoxChild w2 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.ButtonGetDate]));
			w2.Position = 0;
			w2.Expand = false;
			w2.Fill = false;
			// Container child hbox2.Gtk.Box+BoxChild
			this.RetrieveRemoteDateButton = new global::Gtk.Button ();
			this.RetrieveRemoteDateButton.CanFocus = true;
			this.RetrieveRemoteDateButton.Name = "RetrieveRemoteDateButton";
			this.RetrieveRemoteDateButton.UseUnderline = true;
			this.RetrieveRemoteDateButton.Label = global::Mono.Unix.Catalog.GetString ("RemoteDate");
			this.hbox2.Add (this.RetrieveRemoteDateButton);
			global::Gtk.Box.BoxChild w3 = ((global::Gtk.Box.BoxChild)(this.hbox2 [this.RetrieveRemoteDateButton]));
			w3.Position = 2;
			w3.Expand = false;
			w3.Fill = false;
			this.vbox3.Add (this.hbox2);
			global::Gtk.Box.BoxChild w4 = ((global::Gtk.Box.BoxChild)(this.vbox3 [this.hbox2]));
			w4.Position = 2;
			w4.Expand = false;
			w4.Fill = false;
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