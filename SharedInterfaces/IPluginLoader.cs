using System;
using System.AddIn.Contract;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SharedInterfaces
{
    public interface IPluginLoader
    {
        bool LoadPlugin(string pluginDirectoryPath, string assembly, string typeName);
        INativeHandleContract CreateFrameworkElementContract(string controlType);

        [OneWay]
        void Terminate();

        /**
         * 
         * Methods forwarded from the loaded plugin
         * 
         */
        IList<string> GetControlTypeNames();
    }
}
