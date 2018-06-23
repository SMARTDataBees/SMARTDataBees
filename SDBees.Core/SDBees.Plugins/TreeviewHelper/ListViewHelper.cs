using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Plugs.TemplateTreeNode;
using System;
using System.Windows.Forms;

namespace SDBees.Plugins.TreeviewHelper
{
    [PluginName("ListView Helper Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("ListView Helper Plugin")]
    [PluginId("DC81C0D4-0814-49C5-94D7-63B58354D786")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(DB.SDBeesDBConnection))]
    [PluginDependency(typeof(Core.Global.GlobalManager))]

    public class ListViewHelper : Plugs.TreenodeHelper.TemplateTreenodeHelper
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

            StartMe(context, e);
        }

        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("ListView Helper Plugin\n");
        }

        public override UserControl MyUserControl()
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
                    _myControl.ChildTemplateTreenodeTag = selectedTag;
                    if(parentTag != null)
                        _myControl.ParentTemplateTreenodeTag = parentTag;

                    _myControl.ViewId = viewId;

                    tabPage.Controls.Clear();
                    tabPage.Controls.Add(MyUserControl());
                    MyUserControl().Dock = DockStyle.Fill;

                    _myControl.UpdateView();
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
