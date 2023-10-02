using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Permissions;
using Mono.Cecil;
using SharedInterfaces;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace HostApp
{
    public partial class MainForm : Form
    {
        public const string PLUGIN_DIRECTORY_PATH = @".\Plugins";

        private bool isUsingWorkaround = false;
        private bool isEnablingProblems = false;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            var currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
            lstMessages.Items.Insert(0, $"{currentTime}: {m.Msg}");

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

        public static int GetScrollPosition(ListBox listBox, bool horizontalBar)
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
            var pluginDirectoriesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            pluginDirectoriesPath = Path.Combine(pluginDirectoriesPath, PLUGIN_DIRECTORY_PATH);
            
            if (!Directory.Exists(pluginDirectoriesPath))
            {
                Directory.CreateDirectory(pluginDirectoriesPath);
            }

            var pluginDirectoryPaths = Directory.GetDirectories(pluginDirectoriesPath);

            foreach (var pluginDirectoryPath in pluginDirectoryPaths)
            {
                LoadPlugin(pluginDirectoryPath);
            }

            DisplayPlugins();
        }

        private static void LoadPlugin(string pluginDirectoryPath)
        {
            var pluginAssemblyName = Path.GetFileName(pluginDirectoryPath);
            var pluginAssemblyFileName = $"{pluginAssemblyName}.dll";
            var pluginAssemblyPath = Path.Combine(pluginDirectoryPath, pluginAssemblyFileName);

            var sandboxDomainSetup = new AppDomainSetup
            {
                ApplicationBase = pluginDirectoryPath,
            };
            
            var evidence = new Evidence();
            evidence.AddHostEvidence(new Zone(SecurityZone.Internet));

            var permissions = SecurityManager.GetStandardSandbox(evidence);

            // Required to instantiate Controls inside the plugin
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read, pluginDirectoryPath));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, pluginDirectoryPath));
            permissions.AddPermission(new UIPermission(UIPermissionWindow.AllWindows));

            var sandboxDomain = AppDomain.CreateDomain("Sandbox", null, sandboxDomainSetup, permissions);
            var pluginAssembly = AssemblyDefinition.ReadAssembly(pluginAssemblyPath);
            var mappingControlFactories = new List<MappingControlFactory>();

            foreach (var type in pluginAssembly.MainModule.Types)
            {
                var cecilMappingControlAttribute = type.CustomAttributes.SingleOrDefault(ca => ca.AttributeType.Name == nameof(MappingControlAttribute));

                if (cecilMappingControlAttribute == null)
                {
                    continue;
                }

                string title = string.Empty;
                int order = 0;

                foreach (var property in cecilMappingControlAttribute.Properties)
                {
                    if (property.Name == nameof(MappingControlAttribute.Title))
                    {
                        title = (string)property.Argument.Value;
                    }
                    else if (property.Name == nameof(MappingControlAttribute.Order))
                    {
                        order = (int)property.Argument.Value;
                    }
                }

                var mappingControlFactory = new AppDomainMappingControlFactory(
                    title,
                    order,
                    sandboxDomain,
                    pluginAssemblyPath,
                    type.FullName
                );

                mappingControlFactories.Add(mappingControlFactory);
            }

            MappingControlRepository.Buffer(mappingControlFactories);
        }

        private void DisplayPlugins()
        {
            var mappingControlFactories = MappingControlRepository.GetAllMappingControls();
            var sortedFactories = mappingControlFactories.OrderBy(f => f.Value.Order);

            foreach (var mappingControlWithFactory in sortedFactories)
            {
                var mappingControlTypeName = mappingControlWithFactory.Key;
                var mappingControlFactory = mappingControlWithFactory.Value;
                var mappingControl = mappingControlFactory.CreateInstance<Control>();

                if (mappingControl is AbstractPluginUserControl abstractPluginControl)
                {
                    abstractPluginControl.AutoSize = true;
                    mappingControl = new PluginHostControl(abstractPluginControl);
                }

                // Give the form a moment to get to know the parent size
                mappingControl.Visible = false;

                Task.Run(() =>
                {
                    // Then maximize it, so it fills the parent
                    Task.Delay(100).Wait();
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
        }
    }
}
