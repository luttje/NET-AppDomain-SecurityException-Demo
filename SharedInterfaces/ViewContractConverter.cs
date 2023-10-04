using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharedInterfaces
{
    public class ViewContractConverter : MarshalByRefObject
    {
        public INativeHandleContract ConvertToContract(FrameworkElement element)
        {
            var contract = FrameworkElementAdapters.ViewToContractAdapter(element);
            return contract;
        }

        public INativeHandleContract ConvertToContract(ObjectHandle controlHandle)
        {
            return ConvertToContract((FrameworkElement)controlHandle.Unwrap());
        }
    }
}
