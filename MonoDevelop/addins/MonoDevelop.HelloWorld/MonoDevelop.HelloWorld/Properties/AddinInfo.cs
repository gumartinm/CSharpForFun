using Mono.Addins;

[assembly:Addin (
	"MonoDevelop.HelloWorld",
	Namespace = "MonoDevelop.HelloWorld",
	Version = "1.0",
	Category = "HelloWorld Category"
)]

[assembly:AddinName ("HelloWorld")]
[assembly:AddinCategory ("HelloWorld Category")]
[assembly:AddinDescription ("Just a simple Addin for MonoDevelopment IDE")]
[assembly:AddinAuthor ("Gustavo Martin Morcuende")]

[assembly:AddinDependency ("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly:AddinDependency ("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]

