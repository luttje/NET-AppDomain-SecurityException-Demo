using SharedInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ClientPlugin
{
    public class Plugin : PluginBase
    {
        public override IList<string> GetControlTypeNames()
        {
            return new List<string>
            {
                typeof(MyPluginUserControl).FullName,
                typeof(AnotherPluginUserControl).FullName,
            };
        }
    }
}
