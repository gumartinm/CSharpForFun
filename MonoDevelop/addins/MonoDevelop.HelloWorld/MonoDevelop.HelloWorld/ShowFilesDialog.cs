using System;
using MonoDevelop.Projects;
using System.Text;

namespace MonoDevelop.HelloWorld
{
	public partial class ShowFilesDialog : Gtk.Dialog
	{
		readonly Project project;

		public ShowFilesDialog (Project project)
		{
			this.project = project;
			this.Build ();
		}

		protected void OnButtonShowFilesClickEvent (object sender, EventArgs e)
		{
			var fileNames = new StringBuilder ();
			fileNames.Append ("Project name: " + project.Name);
			fileNames.Append (Environment.NewLine);
			foreach (var file in project.Files)
			{
				fileNames.Append(file.Name);
				fileNames.Append (Environment.NewLine);
			}
			this.ShowFilesTextView.Buffer.Text = fileNames.ToString ();
		}
	}
}

