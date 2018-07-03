using System;
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Connectivity.SDBeesLink.UI;
using SDBees.Core.Global;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateMenue;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    [PluginName("External Document Manager Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the management of external documents in SDBeesLink")]
    [PluginId("8266690D-0257-4881-9822-980202779D23")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBeesDBConnection))]
    [PluginDependency(typeof(MainWindowApplication))]
    [PluginDependency(typeof(GlobalManager))]

    class SDBeesExternalDocumentAdmin : TemplateMenue
    {
        private readonly MenuItem _mnuItem;
        private PluginContext _context;

        public SDBeesExternalDocumentAdmin()
        {
            Current = this;
            _mnuItem = new MenuItem("External document manager ...");
            _mnuItem.Click += _mnuItem_Click;
        }

        /// <summary>
        /// Returns the one and only ViewAdminManager Plugin instance.
        /// </summary>
        public static SDBeesExternalDocumentAdmin Current { get; private set; }

        /// <summary>
        /// The Context for the loaded Plugin
        /// </summary>
        public PluginContext MyContext
        {
            get { return _context; }
        }

        void _mnuItem_Click(object sender, EventArgs e)
        {
            var dlg = new SDBeesExternalDocumentDLG();
            var dialogResult = dlg.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                DBManager.OnUpdate(null);
            }
        }

        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                Console.WriteLine("SDBeesExternalDocumentAdmin manager starts\n");

                _context = context;

                StartMe(context, e);
                //Setting up the menuitem
                if (MainWindow != null)
                {
                    MainWindow.TheDialog.MenueTools().MenuItems.Add(_mnuItem);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            
        }
    }
}
