using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SharedInterfaces
{
    public abstract class PluginBase : MarshalByRefObject
    {
        public virtual void Initialize()
        { }

        public virtual IList<string> GetControlTypeNames()
        {
            return new List<string>();
        }
    }
}
