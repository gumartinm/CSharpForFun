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

