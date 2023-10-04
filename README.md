# Example: Plugins serving (WPF) Controls, restricted to an AppDomain

> **Original problem:** I ran into a problem when I used [`SetParent`](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setparent) to parent a UserControl from a different AppDomain to our Form. A SecurityException would occur as soon a WndProc on our Form (or child of our Form) does something that the AppDomain has no access to (e.g: use a native method). This was caused by the MessageBox sending `WM_*` messages, which caused the WndProc to be called from some other AppDomain. 
> 
> *You can find a demonstration of the problematic code in the `with-problems` branch.*

**🎉 Solution:** With the help of [the Baktun Shell demo by Ivan Krivyakov](https://www.codeproject.com/articles/516431/baktun-shell-hosting-wpf-child-windows-in-another) I found a nice solution. We run the plugin in a completely different process, communicating via IPC. This way the WndProc of the Form is never called from the plugin. To further limit the plugin, it is loaded into an AppDomain. This way we can limit the permissions of the plugin.

*I'm leaving this repo online in the hope that it helps others, especially since I had trouble finding useful sources on this topic. Note that the code is a hodgepodge of different experiments, so some code may be more vital to the solution than others.*

## Projects

* **HostApp:** The main application that loads the Plugins into separate processes using the PluginLoader.exe. It communicates with the PluginLoader using IPC.
* **PluginLoader:** The application that loads the plugins into separate AppDomains. It also handles the IPC with the HostApp. If needed it has access to the AppDomains for the plugins.
* **ClientPlugin:** The plugin that contains two UserControls that will be loaded into the HostApp.
  - **ClientPlugin.MyPluginUserControl:** Simply shows a MessageBox.
  - **ClientPlugin.AnotherPluginUserControl:** The first button on this control uses a native method. The second writes a txt file to the `D:/` drive. Both should cause a SecurityException since the AppDomain restricts IO.
* **SharedInterfaces:** Some common interfaces that are used by both the HostApp and the ClientPlugin.

## Starting the demo

1. Build the entire solution. *The ClientPlugin builds to `../bin/Debug/Plugins/ClientPlugin` so that it can be discovered by the HostApp*
2. Run the HostApp.

In Debug mode the PluginLoader command line will be visible. *PS: I haven't tested anything in Release mode.*