using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Plugs.TemplateTreeNode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDBees.Plugins.TreeviewHelper
{
    [PluginName("ListView Helper Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("ListView Helper Plugin")]
    [PluginId("DC81C0D4-0814-49C5-94D7-63B58354D786")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

    public class ListViewHelper : SDBees.Plugs.TreenodeHelper.TemplateTreenodeHelper
    {
        private static ListViewHelper _theInstance = null;
        private ListViewHelperUserControl _myControl = null;

        public static ListViewHelper Current
        {
            get { return _theInstance; }
        }

        public ListViewHelper()
            : base()
        {
            _theInstance = this;
            _myControl = new ListViewHelperUserControl(this);
        }

        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("ListView Helper Plugin\n");

            this.StartMe(context, e);
        }

        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("ListView Helper Plugin\n");
        }

        public override System.Windows.Forms.UserControl MyUserControl()
        {
            return _myControl;
        }

        /// <summary>
        /// Override this method if plugin should react to selection changes in the system tree view
        /// </summary>
        public override void UpdatePropertyPage(TabPage tabPage, Guid viewId, TemplateTreenodeTag selectedTag, TemplateTreenodeTag parentTag)
        {
            if (tabPage != null)
            {
                try
                {
                    this._myControl.ChildTemplateTreenodeTag = selectedTag;
                    if(parentTag != null)
                        this._myControl.ParentTemplateTreenodeTag = parentTag;

                    this._myControl.ViewId = viewId;

                    tabPage.Controls.Clear();
                    tabPage.Controls.Add(this.MyUserControl());
                    this.MyUserControl().Dock = DockStyle.Fill;

                    this._myControl.UpdateView();
                }
                catch (Exception ex)
                {
                }
            }
        }

        public override string TabPageName()
        {
            return "Objectlist";
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            // empty
        }
    }
}
