using SharedInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientPlugin
{
    /// <summary>
    /// Interaction logic for AnotherPluginUserControl.xaml
    /// </summary>
    [MappingControl(Title = "Always Crash Demo", Order = 99)]
    public partial class AnotherPluginUserControl : UserControl, IPluginUserControl
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hwnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public int DesiredHeight { get; set; } = 50;

        private const int WM_USER = 0x0400;
        private const int WM_BTNCLICKED = WM_USER + 1;
        public AnotherPluginUserControl()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var handle = new System.Windows.Interop.WindowInteropHelper(Application.Current.MainWindow).Handle;
            SendMessage(handle, WM_BTNCLICKED, IntPtr.Zero, IntPtr.Zero);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Writing file...");
            File.WriteAllText("D:/write-test.txt", "Test for plugin. This file can be safely removed.");
        }
    }
}
