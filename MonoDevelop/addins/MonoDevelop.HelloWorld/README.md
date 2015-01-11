1. Create Addin:

xbuild (MonoDevelop.HelloWorld.sln being in the same path)


2. Packaging an add-in, from: http://www.monodevelop.com/developers/articles/publishing-an-addin/

mdtool setup pack MonoDevelop.HelloWorld/bin/Release/MonoDevelop.HelloWorld.dll

(it creates a .mpack file) MonoDevelop.HelloWorld.MonoDevelop.HelloWorld_1.0.mpack (it can be installed from the MonoDevelop GUI.


3. The installed Addins from the GUI are located in: ~/.local/share/MonoDevelop-5.0/LocalInstall/Addins/
