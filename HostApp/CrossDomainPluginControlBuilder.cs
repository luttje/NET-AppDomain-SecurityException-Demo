using SharedInterfaces;
using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;

namespace HostApp
{
    // https://www.codeproject.com/Articles/478659/WPF-Hosting-WinForms-Control-from-Another-AppDomai
    internal class CrossDomainPluginControlBuilder
    {
        private AppDomain appDomain;
        private string pluginAssemblyName;
        private string mappingControlTypeName;

        public CrossDomainPluginControlBuilder(AppDomain appDomain, string pluginAssemblyPath, string mappingControlTypeName)
        {
            this.appDomain = appDomain;
            this.pluginAssemblyName = Path.GetFileNameWithoutExtension(pluginAssemblyPath);
            this.mappingControlTypeName = mappingControlTypeName;
        }

        public ElementHost CreatePluginControl()
        {
            var pluginControlBuilder = appDomain.CreateInstanceAndUnwrap(
                typeof(PluginControlBuilder).Assembly.FullName,
                typeof(PluginControlBuilder).FullName) as PluginControlBuilder;
            var contract = pluginControlBuilder.CreateControlAsContract(pluginAssemblyName, mappingControlTypeName);
            var control = FrameworkElementAdapters.ContractToViewAdapter(contract);
            var wpfHost = new ElementHost();
            wpfHost.Child = control;
            return wpfHost;
        }
    }
}
