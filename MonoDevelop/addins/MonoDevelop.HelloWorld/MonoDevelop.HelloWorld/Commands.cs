using System;
using System.Linq;
using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Projects;


namespace MonoDevelop.HelloWorld
{
	public enum Commands
	{
		ShowFiles,
	}

	public class ShowFilesCommandHandler : CommandHandler
	{
		protected override void Run()
		{
			var proj = IdeApp.Workspace.GetAllProjects ().FirstOrDefault ();
			if (proj != null)
			{
				var dlg = new ShowFilesDialog (proj);
				MessageService.ShowCustomDialog (dlg);
			}
		}
	}
}

