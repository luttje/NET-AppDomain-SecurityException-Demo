using SharedInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HostApp
{
    public partial class PluginHostControl : UserControl
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        
        public PluginHostControl()
        {
            InitializeComponent();
        }

        public PluginHostControl(AbstractPluginUserControl abstractPluginControl)
            : this()
        {
            Padding = new Padding(0, 5, 0, 5);
            Height = abstractPluginControl.DesiredHeight + this.Padding.Vertical;

            // We use this hack to embed the control which is actually in a different AppDomain (and thus supposed to be quite unreachable)
            abstractPluginControl.Show();
            SetParent(abstractPluginControl.Handle, this.Handle);

            abstractPluginControl.Dock = DockStyle.Fill;
        }
    }
}
