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

namespace ClientPlugin
{
    [MappingControl(Title="Always Crash Demo", Order=99)]
    public partial class AnotherPluginUserControl : AbstractPluginUserControl
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_USER = 0x0400;
        private const int WM_BTNCLICKED = WM_USER + 1;

        public override int DesiredHeight { get; protected set; } = 50;
        public AnotherPluginUserControl()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            SendMessage(this.Handle, WM_BTNCLICKED, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
