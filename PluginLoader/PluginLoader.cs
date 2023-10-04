using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Security.Policy;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.AddIn.Contract;
using SharedInterfaces;
using System.Diagnostics;

namespace PluginLoader
{
    class PluginLoader : MarshalByRefObject, IPluginLoader
    {
        private PluginBase loadedPlugin;
        private AppDomain sandboxDomain;
        private string pluginAssemblyPath;
        private string pluginAssemblyName;

        /// <summary>
        /// This method can't just return the PluginBase, since it's created in an AppDomain in the PluginLoader process. Passing it 
        /// upwards to the main app would not work, since it has no remote connection to it. Therefor we store the plugin and let
        /// the main app call functions on this loader to interact with it.
        /// </summary>
        /// <param name="assemblyPath"></param>
        /// <param name="assembly"></param>
        /// <param name="typeName"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ApplicationException"></exception>
        /// <returns>Whether we were succesfull</returns>
        public bool LoadPlugin(string assemblyPath, string assembly, string typeName)
        {
            if (String.IsNullOrEmpty(assemblyPath)) throw new ArgumentNullException("assemblyPath");
            if (String.IsNullOrEmpty(assembly)) throw new ArgumentNullException("assembly");
            if (String.IsNullOrEmpty(typeName)) throw new ArgumentNullException("typeName");

            pluginAssemblyPath = assemblyPath;
            pluginAssemblyName = assembly;


            Console.WriteLine("Loading plugin {0},{1}", assembly, typeName);

            try
            {
                var pluginDirectory = Path.GetDirectoryName(assemblyPath);
                var sandboxDomainSetup = new AppDomainSetup
                {
                    ApplicationBase = pluginDirectory,
                };

                var evidence = new Evidence();
                evidence.AddHostEvidence(new Zone(SecurityZone.Internet));

                var permissions = SecurityManager.GetStandardSandbox(evidence);

                // Required to instantiate Controls inside the plugin
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, pluginDirectory));
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, pluginDirectory));
                permissions.AddPermission(new UIPermission(UIPermissionWindow.AllWindows));
                permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

                sandboxDomain = AppDomain.CreateDomain("Sandbox", null, sandboxDomainSetup, permissions);
                loadedPlugin = (PluginBase)sandboxDomain.CreateInstanceAndUnwrap(assembly, typeName);
                return true;
            }
            catch (Exception ex)
            {
                var message = String.Format("Error loading type '{0}' from assembly '{1}'. {2}",
                    assembly, typeName, ex.Message);

                throw new ApplicationException(message, ex);
            }
        }

        public INativeHandleContract CreateFrameworkElementContract(string controlTypeName)
        {
            Func<string, string, AppDomain, INativeHandleContract> createOnUiThread = CreateOnUiThread;
            var contract = (INativeHandleContract)Program.AppDispatcher.Invoke(createOnUiThread, pluginAssemblyName, controlTypeName, sandboxDomain);
            var insulator = new NativeHandleContractInsulator(contract);
            return insulator;
        }

        private static INativeHandleContract CreateOnUiThread(string assembly, string typeName, AppDomain appDomain)
        {
            try
            {
                var controlHandle = appDomain.CreateInstance(assembly, typeName);
                if (controlHandle == null) throw new InvalidOperationException("appDomain.CreateInstance() returned null for " + assembly + "," + typeName);

                var converterHandle = appDomain.CreateInstanceAndUnwrap(
                    typeof(ViewContractConverter).Assembly.FullName,
                    typeof(ViewContractConverter).FullName) as ViewContractConverter;
                if (converterHandle == null) throw new InvalidOperationException("appDomain.CreateInstance() returned null for ViewContractConverter");
                var contract = converterHandle.ConvertToContract(controlHandle);

                return contract;
            }
            catch (Exception ex)
            {
                var message = String.Format("Error loading type '{0}' from assembly '{1}'. {2}",
                    assembly, typeName, ex.Message);

                throw new ApplicationException(message, ex);
            }
        }

        public void Terminate()
        {
            System.Environment.Exit(0);
        }

        /**
         * 
         * Methods forwarded from the loaded plugin
         * 
         */
        public IList<string> GetControlTypeNames()
        {
            return loadedPlugin.GetControlTypeNames();
        }
    }
}
