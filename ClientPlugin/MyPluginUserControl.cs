using SharedInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientPlugin
{
    [MappingControl(Title = "MessageBox Demo")]
    public partial class MyPluginUserControl : AbstractPluginUserControl
    {
        public override int DesiredHeight { get; protected set; } = 50;

        public MyPluginUserControl()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello from MyPluginUserControl!");
        }
    }
}
