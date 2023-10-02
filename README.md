# WndProc AppDomain SecurityException Demo

We can use [`SetParent`](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setparent) to parent a UserControl from a different AppDomain to our Form. However a SecurityException will occur as soon a WndProc on our Form (or child of our Form) does something that the AppDomain has no access to (e.g: a native method). This demonstrates that problem.

## Demonstration

These projects form a minimal example of the problem. The idea is that the HostApp wants to show Controls from plugins. In order to do that we use `SetParent` to attach the UserControl from the plugin to the HostApp.

### Projects

* **HostApp:** The main application that loads the UserControls from plugins into a different AppDomain.
* **ClientPlugin:** The plugin that contains two UserControls that will be loaded into the HostApp.
  - **ClientPlugin.MyPluginUserControl:** Simply shows a MessageBox. As soon as the Host App starts calling Native methods inside WndProc, opening this MessageBox will throw a SecurityException.
  - **ClientPlugin.AnotherPluginUserControl:** Contains 2 buttons that both directly use a native method. Will each always cause a SecurityException.
* **SharedInterfaces:** Some common interfaces that are used by both the HostApp and the ClientPlugin.

## Proposed solution

I'd love a magical method *(e.g: the fictional <s>`System.Windows.Forms.ContainMessageLoop(pluginControl)`</s>)* that allows me to let the UserControl messages not leave their loop. However I think that because we used SetParent that's not possible.

## Workaround

Just use try-catch in every WndProc on the Form and all children. However this is not ideal because it's easy to forget to do this, or would mean having to override every WndProc in every child control *(which has the risk SecurityException)*.