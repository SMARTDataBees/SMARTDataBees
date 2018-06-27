using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Global;
using SDBees.Core.Properties;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateBase;

namespace SDBees.Core.Main.Systemtray
{
    /// <summary>
    /// 
    /// </summary>
    [PluginName("Systemtray Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the systemtray icon of SmartDataBees")]
    [PluginId("B39961CF-AAC5-4F27-BC44-AD51DC1AABBA")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBeesDBConnection))]
    [PluginDependency(typeof(MainWindowApplication))]
    [PluginDependency(typeof(GlobalManager))]

    public sealed class ProcessIcon : TemplateBase, IDisposable
    {
        /// <summary>
        /// The NotifyIcon object.
        /// </summary>
        NotifyIcon m_notifyicon;
        ControlContainer m_container;
        ContextMenus m_contextmenu;

        public ContextMenus MyContextMenu
        {
            get { return m_contextmenu; }
            set { m_contextmenu = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessIcon"/> class.
        /// </summary>
        public ProcessIcon()
        {
            // Instantiate the NotifyIcon object.
            m_container = new ControlContainer();
            m_notifyicon = new NotifyIcon(m_container);

            _theInstance = this;
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public void Dispose()
        {
            // When the application closes, this will remove the icon from the system tray immediately.
            m_notifyicon.Dispose();
        }

        private static ProcessIcon _theInstance;
        /// <summary>
        /// Returns the one and only SystemTrayApplicationPlugin instance.
        /// </summary>
        public static ProcessIcon Current
        {
            get
            {
                return _theInstance;
            }
        }

        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            //Store the context for internal use
            StartMe(context, e);

            Display(context);
        }

        /// <summary>
        /// Displays the icon in the system tray.
        /// </summary>
        public void Display(PluginContext context)
        {
            // Put the icon in the system tray and allow it react to mouse clicks.			
            m_notifyicon.Icon = Resources.SDBeesIcon;
            try
            {
                var _path = Path.GetDirectoryName(GetType().Assembly.Location) + ConfigurationManager.AppSettings["MainWindowIcon"];
                m_notifyicon.Icon = new Icon(_path);
            }
            catch (Exception ex)
            {
            }

            var _TitleText = "SMARTDataBees";
            try
            {
                _TitleText = ConfigurationManager.AppSettings["MainWindowTitle"];
            }
            catch (Exception ex)
            {
            }

            m_notifyicon.Text = _TitleText;

            // Attach a context menu.
            m_contextmenu = new ContextMenus(context);
            //this.m_contextmenu.Create();

            m_notifyicon.ContextMenuStrip = m_contextmenu;

            m_contextmenu.Show();
            m_contextmenu.Hide();

            m_notifyicon.Visible = true;
        }

        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Dispose();
        }

        private delegate void OpenMainDialogHelper();
        internal void OpenMainDialog()
        {
            var contextMenu = Current.m_contextmenu;

            if (contextMenu.InvokeRequired)
            {
                OpenMainDialogHelper d = OpenMainDialog;
                contextMenu.BeginInvoke(d);
            }
            else
            {
                contextMenu.MainWindowItem.PerformClick();
            }
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            // empty
        }
    }

    class ControlContainer : IContainer
    {
        ComponentCollection _components;

        public ControlContainer()
        {
            _components = new ComponentCollection(new IComponent[] { });
        }

        public void Add(IComponent component)
        { }

        public void Add(IComponent component, string Name)
        { }

        public void Remove(IComponent component)
        { }

        public ComponentCollection Components
        {
            get { return _components; }
        }

        public void Dispose()
        {
            _components = null;
        }
    }
}