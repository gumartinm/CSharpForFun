/**
* Copyright 2015 Gustavo Martin Morcuende
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
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

		protected override void Update (CommandInfo info)
		{
			var proj = IdeApp.Workspace.GetAllProjects ().FirstOrDefault ();
			if (proj != null) {
				info.Enabled = true;
				info.Visible = true;
			} else {
				info.Enabled = false;
				info.Visible = true;
			}
		}
	}
}

