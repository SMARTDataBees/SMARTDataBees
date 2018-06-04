using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    [PluginName("External Document Manager Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the management of external documents in SDBeesLink")]
    [PluginId("8266690D-0257-4881-9822-980202779D23")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

    class SDBeesExternalDocumentAdmin : SDBees.Plugs.TemplateMenue.TemplateMenue
    {
        private static SDBeesExternalDocumentAdmin _theInstance;
        private MenuItem _mnuItem;
        private PluginContext _context;

        public SDBeesExternalDocumentAdmin() : base()
        {
            _theInstance = this;
            _mnuItem = new MenuItem("External document manager ...");
            this._mnuItem.Click += _mnuItem_Click;
        }

        /// <summary>
        /// Returns the one and only ViewAdminManager Plugin instance.
        /// </summary>
        public static SDBeesExternalDocumentAdmin Current
        {
            get { return _theInstance; }
        }

        /// <summary>
        /// The Context for the loaded Plugin
        /// </summary>
        public PluginContext MyContext
        {
            get { return this._context; }
        }

        void _mnuItem_Click(object sender, EventArgs e)
        {
            UI.SDBeesExternalDocumentDLG dlg = new UI.SDBeesExternalDocumentDLG();
            DialogResult dialogResult = dlg.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                MyDBManager.OnUpdate(null);
            }
        }

        protected override void Start(Carbon.Plugins.PluginContext context, Carbon.Plugins.PluginDescriptorEventArgs e)
        {
            try
            {
                Console.WriteLine("SDBeesExternalDocumentAdmin manager starts\n");

                _context = context;

                StartMe(context, e);

                if (MyDBManager != null)
                {
                }

                //Setting up the menuitem
                if (MyMainWindow != null)
                {
                    MyMainWindow.TheDialog.MenueTools().MenuItems.Add(_mnuItem);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void Stop(Carbon.Plugins.PluginContext context, Carbon.Plugins.PluginDescriptorEventArgs e)
        {
            //throw new NotImplementedException();
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            // empty
        }
    }
}
