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

namespace HostApp
{
    [MappingControl(Title = "Internal User Control", Order = 2)]
    public partial class InternalUserControl : UserControl
    {
        public InternalUserControl()
        {
            InitializeComponent();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello World!");
        }
    }
}
