using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Controls;

namespace HostApp
{
    public partial class MainForm : Form
    {
        public const string PLUGIN_DIRECTORY_PATH = @"Plugins";

        private bool isUsingWorkaround = false;
        private bool isEnablingProblems = false;
        
        private System.Windows.Forms.TabControl tbcPlugins;

        private IList<PluginHostProxy> proxiesToDispose = new List<PluginHostProxy>();

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            foreach (var disposable in proxiesToDispose)
            {
                disposable.Disposing -= PluginHost_Disposing;
                disposable.Dispose();
            }

            base.OnFormClosed(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            var currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            lstMessages.Items.Insert(0, $"{currentTime}: {m.Msg} ({m.HWnd})");

            if (isEnablingProblems)
            {
                void causeSecurityExceptionIfAcrossAppDomain()
                {
                    var scrollPos = GetScrollPosition(lstMessages, false);
                    lstMessages.Items.Insert(0, $"{currentTime}: Scroll position of `lstMessages` = {scrollPos}");
                }
                
                if (!isUsingWorkaround) 
                {
                    causeSecurityExceptionIfAcrossAppDomain();
                }
                else
                {
                    try
                    {
                        causeSecurityExceptionIfAcrossAppDomain();
                    }
                    catch(SecurityException ex)
                    {
                        // Can we somehow please stop the plugins from sending messages to the host app? Is SetParent the problem? Probably...
                        Debug.WriteLine($"Although we prevent a crash, I'd rather find a way to prevent the SecurityException from happening in the first place. Exception message: {ex.Message}");
                    }
                }
            }
        }

        public static int GetScrollPosition(System.Windows.Forms.ListBox listBox, bool horizontalBar)
        {
            int fnBar = ((!horizontalBar) ? 1 : 0);
            SCROLLINFO sCROLLINFO = new SCROLLINFO();
            sCROLLINFO.fMask = 4;
            if (Native.GetScrollInfo(listBox.Handle, fnBar, sCROLLINFO))
            {
                return sCROLLINFO.nPos;
            }
            return -1;
        }

        private void chkProblems_CheckedChanged(object sender, EventArgs e)
        {
            isEnablingProblems = chkProblems.Checked;

            lblHint.Visible = isEnablingProblems;
            chkWorkaround.Visible = isEnablingProblems;
        }

        private void chkWorkaround_CheckedChanged(object sender, EventArgs e)
        {
            isUsingWorkaround = chkWorkaround.Checked;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadAllPlugins();
        }

        private void LoadAllPlugins()
        {
            var pluginDirectoriesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pluginDirectoriesPath = Path.Combine(pluginDirectoriesPath, PLUGIN_DIRECTORY_PATH);

            if (!Directory.Exists(pluginDirectoriesPath))
            {
                Directory.CreateDirectory(pluginDirectoriesPath);
            }

            var mappingControlFactories = new List<MappingControlFactory>();
            var pluginDirectoryPaths = Directory.GetDirectories(pluginDirectoriesPath);

            foreach (var pluginDirectoryPath in pluginDirectoryPaths)
            {
                mappingControlFactories.AddRange(LoadPlugin(pluginDirectoryPath));
            }

            MappingControlRepository.Buffer(mappingControlFactories);

            DisplayPlugins();
        }

        private IList<MappingControlFactory> LoadPlugin(string pluginDirectoryPath)
        {
            var pluginAssemblyName = Path.GetFileName(pluginDirectoryPath);
            var pluginAssemblyFileName = $"{pluginAssemblyName}.dll";
            var pluginAssemblyPath = Path.Combine(pluginDirectoryPath, pluginAssemblyFileName);

            var pluginHost = new PluginHostProxy();
            var pluginClassName = $"{pluginAssemblyName}.Plugin";
            
            if(!pluginHost.LoadPlugin(pluginAssemblyPath, pluginAssemblyName, pluginClassName))
            {
                pluginHost.Dispose();
                throw new ApplicationException($"Failed to load plugin {pluginAssemblyPath}.");
            }

            pluginHost.Disposing += PluginHost_Disposing;
            proxiesToDispose.Add(pluginHost);

            return pluginHost.GetMappingControlFactories();
        }

        private void PluginHost_Disposing(object sender, EventArgs e)
        {
            if (this.Disposing || this.IsDisposed)
                return;

            var pluginHost = (PluginHostProxy)sender;

            if (proxiesToDispose.Contains(pluginHost))
                proxiesToDispose.Remove(pluginHost);

            this.Invoke((MethodInvoker)delegate
            {
                if (this.Controls.Contains(tbcPlugins))
                    this.Controls.Remove(tbcPlugins);

                // This wont work, since the tabpage being removed causes a WndProc which triggers other dipsosed plugin controls to get called (which causes an error)
                //if (tbcPlugins.TabPages.Contains(tabPage))
                //    tbcPlugins.TabPages.Remove(tabPage);

                // Trigger a rebuild for the tab control
                LoadAllPlugins();

                MessageBox.Show(this, $"A plugin unloaded unexpectedly, it may have crashed or been forcefully shut down. All plugins have been reloaded.", "Plugin unloaded unexpectedly!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            });
        }

        private void DisplayPlugins()
        {
            var mappingControlFactories = MappingControlRepository.GetAllMappingControls();
            var sortedFactories = mappingControlFactories.OrderBy(f => f.Value.Order);

            this.tbcPlugins = new System.Windows.Forms.TabControl();
            this.tbcPlugins.Dock = DockStyle.Fill;
            this.tbcPlugins.Location = new Point(0, 0);
            this.tbcPlugins.Name = "tbcPlugins";
            this.tbcPlugins.SelectedIndex = 0;
            this.tbcPlugins.Size = new Size(800, 450);
            this.tbcPlugins.TabIndex = 0;
            this.Controls.Add(this.tbcPlugins);

            foreach (var mappingControlWithFactory in sortedFactories)
            {
                var mappingControlTypeName = mappingControlWithFactory.Key;
                var mappingControlFactory = mappingControlWithFactory.Value as PluginMappingControlFactory; // hacks (easy access to properties and event) for quick testing
                var mappingControl = mappingControlFactory.CreateInstance<System.Windows.Forms.Control>();

                // Plugin crashed during creation.
                if (mappingControl == null)
                {
                    MessageBox.Show(this, $"Plugin {mappingControlTypeName} failed to load.", "Plugin failed to load!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Give the form a moment to get to know the parent size
                mappingControl.Visible = false;

                Task.Run(() =>
                {
                    // Then maximize it, so it fills the parent
                    Task.Delay(150).Wait();
                    this.Invoke((MethodInvoker)delegate
                    {
                        mappingControl.Dock = DockStyle.Fill;
                        mappingControl.Visible = true;
                        mappingControl.PerformLayout();
                    });
                });

                var tabPage = new TabPage(mappingControlTypeName);
                tabPage.Text = mappingControlFactory.Title;
                tabPage.Controls.Add(mappingControl);

                tbcPlugins.TabPages.Add(tabPage);
            }

            tbcPlugins.SizeMode = TabSizeMode.Fixed;
            tbcPlugins.DrawMode = TabDrawMode.OwnerDrawFixed;
            tbcPlugins.DrawItem += TbcPlugins_DrawItem;
            tbcPlugins.MouseMove += TbcPlugins_MouseMove;
            tbcPlugins.MouseLeave += TbcPlugins_MouseLeave;
            tbcPlugins.MouseUp += TbcPlugins_MouseUp;
            
            // Only for DEBUG mode, where we display the console window.
            this.BringToFront();
        }

        // Close button hack from https://stackoverflow.com/a/59214333
        private int HoverIndex = -1;
        
        private void TbcPlugins_MouseUp(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tbcPlugins.TabCount; i++)
            {
                var rx = (Rectangle)tbcPlugins.TabPages[i].Tag;

                if (rx.Contains(e.Location))
                {
                    tbcPlugins.TabPages[i].Dispose();
                    return;
                }
            }
        }

        private void TbcPlugins_MouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tbcPlugins.TabCount; i++)
            {
                var rx = (Rectangle)tbcPlugins.TabPages[i].Tag;

                if (rx.Contains(e.Location))
                {
                    //To avoid the redundant calls. 
                    if (HoverIndex != i)
                    {
                        HoverIndex = i;
                        tbcPlugins.Invalidate();
                    }
                    return;
                }
            }

            //To avoid the redundant calls.
            if (HoverIndex != -1)
            {
                HoverIndex = -1;
                tbcPlugins.Invalidate();
            }
        }

        private void TbcPlugins_MouseLeave(object sender, EventArgs e)
        {
            if (HoverIndex != -1)
            {
                HoverIndex = -1;
                tbcPlugins.Invalidate();
            }
        }

        private void TbcPlugins_DrawItem(object sender, DrawItemEventArgs e)
        {
            var g = e.Graphics;
            var tp = tbcPlugins.TabPages[e.Index];
            var rt = e.Bounds;
            var rx = new Rectangle(rt.Right - 20, (rt.Y + (rt.Height - 12)) / 2 + 1, 12, 12);

            if ((e.State & DrawItemState.Selected) != DrawItemState.Selected)
            {
                rx.Offset(0, 2);
            }

            rt.Inflate(-rx.Width, 0);
            rt.Offset(-(rx.Width / 2), 0);

            using (Font f = new Font("Marlett", 8f))
            using (StringFormat sf = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter,
                FormatFlags = StringFormatFlags.NoWrap,
            })
            {
                g.DrawString(tp.Text, tp.Font ?? Font, Brushes.Black, rt, sf);
                g.DrawString("r", f, HoverIndex == e.Index ? Brushes.Black : Brushes.LightGray, rx, sf);
            }
            tp.Tag = rx;
        }
    }
}
