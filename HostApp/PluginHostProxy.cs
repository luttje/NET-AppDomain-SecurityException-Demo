using SharedInterfaces;
using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace HostApp
{
    internal class PluginHostProxy : IDisposable
    {
        public event EventHandler Disposing;
        public bool IsDisposing { get; internal set; }

        private string name;
        private EventWaitHandle readyEvent;
        private Process process;
        private IPluginLoader pluginLoader;

        class IpcChannelRegistration
        {
            private static object _lock = new object();
            private static bool _registered;

            public static void RegisterChannel()
            {
                lock (_lock)
                {
                    if (_registered) return;
                    var channel = new IpcChannel();
                    ChannelServices.RegisterChannel(channel, false);
                    _registered = true;
                }
            }
        }

        public bool LoadPlugin(string assemblyPath, string assemblyName, string typeName)
        {
            Start();
            OpenPluginLoader();
            
            if (!pluginLoader.LoadPlugin(assemblyPath, assemblyName, typeName))
            {
                throw new Exception("Failed to load plugin");
            }

            return true;
        }

        /// <summary>
        /// Asks the plugin to construct the WPF FrameworkElement. Then places it inside an ElementHost for use in WinForms.
        /// Can return null if the plugin crashes during creation.
        /// </summary>
        /// <param name="controlTypeName"></param>
        /// <returns></returns>
        internal Control CreateControl(string controlTypeName)
        {
            try { 
                var contract = pluginLoader.CreateFrameworkElementContract(controlTypeName);
                var remoteControl = FrameworkElementAdapters.ContractToViewAdapter(contract);

                return new ElementHost()
                {
                    Child = remoteControl
                };
            } 
            catch (Exception ex)
            {
                return null;
            }
        }

        private static string GetTitle(string typeName)
        {
            int dot = typeName.IndexOf('.');
            if (dot < 0 || dot >= typeName.Length - 1) return typeName;
            return typeName.Substring(dot + 1);
        }

        private void Start()
        {
            if (process != null) 
                return;
            
            name = "PluginLoader." + Guid.NewGuid().ToString();
            
            var eventName = $"{name}.Ready";
            readyEvent = new EventWaitHandle(false, EventResetMode.ManualReset, eventName);

            var processName = "PluginLoader.exe";

            var path = System.IO.Path.GetFullPath(processName);
            
            if (!System.IO.File.Exists(path))
            {
                MessageBox.Show("PluginLoader.exe not found at " + path);
            }

            var info = new ProcessStartInfo
            {
                Arguments = name,
#if !DEBUG
                CreateNoWindow = true,
#endif
                UseShellExecute = false,
                FileName = processName
            };

            process = Process.Start(info);
        }

        private void OpenPluginLoader()
        {
            if (pluginLoader != null) return;

            if (!readyEvent.WaitOne(5000))
            {
                throw new InvalidOperationException("PluginLoader process not ready");
            }

            IpcChannelRegistration.RegisterChannel();

            var url = "ipc://" + name + "/PluginLoader";
            pluginLoader = (IPluginLoader)Activator.GetObject(typeof(IPluginLoader), url);

            var eventName = $"{name}.Exit";
            var exitEvent = new EventWaitHandle(false, EventResetMode.ManualReset, eventName);

            // Wait for the exit event in another thread in the background.
            Thread closeEventListeningThread = null;
            closeEventListeningThread = new Thread(() =>
            {
                while (true)
                {
                    exitEvent.WaitOne();

                    this?.Dispose();
                    
                    // End and dispose this signal thread.
                    closeEventListeningThread.Abort();
                    closeEventListeningThread = null;
                }
            });
            closeEventListeningThread.IsBackground = true;
            closeEventListeningThread.Start();
        }

        internal IList<MappingControlFactory> GetMappingControlFactories()
        {
            var factories = new List<MappingControlFactory>();

            foreach (var controlTypeName in pluginLoader.GetControlTypeNames())
            {
                factories.Add(new PluginMappingControlFactory(
                    GetTitle(controlTypeName),
                    0,
                    this,
                    controlTypeName
                ));
            }

            return factories;
        }

        public void Dispose()
        {
            IsDisposing = true;
            Disposing?.Invoke(this, EventArgs.Empty);

            pluginLoader.Terminate();
            pluginLoader = null;
            process = null;
        }
    }
}
