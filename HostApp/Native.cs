using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HostApp
{
    [StructLayout(LayoutKind.Sequential)]
    public class SCROLLINFO
    {
        public int cbSize = Marshal.SizeOf(typeof(SCROLLINFO));

        public int fMask;

        public int nMin;

        public int nMax;

        public int nPage;

        public int nPos;

        public int nTrackPos;
    }
    
    internal class Native
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, SCROLLINFO scrollInfo);
    }
}
