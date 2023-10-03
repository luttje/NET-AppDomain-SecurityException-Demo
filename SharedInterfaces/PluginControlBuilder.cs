using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Windows;

namespace SharedInterfaces
{
    public class PluginControlBuilder : MarshalByRefObject
    {
        public INativeHandleContract CreateControlAsContract(string pluginAssemblyName, string controlTypeName)
        {
            var control = (FrameworkElement) AppDomain.CurrentDomain.CreateInstanceAndUnwrap(pluginAssemblyName, controlTypeName);
            var newContract = FrameworkElementAdapters.ViewToContractAdapter(control);

            return newContract;
        }
    }
}
