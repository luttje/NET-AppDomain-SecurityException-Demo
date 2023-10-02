using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SharedInterfaces
{
    public abstract class AbstractPluginUserControl : UserControl
    {
        public abstract int DesiredHeight { get; protected set; }
        
        public AbstractPluginUserControl()
            : base()
        {

        }
    }
}
