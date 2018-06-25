// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2007 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// SMARTDataBees is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SMARTDataBees is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SMARTDataBees.  If not, see <http://www.gnu.org/licenses/>.
//
// #EndHeader# ================================================================

using System;
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Global;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;

namespace SDBees.Core.Admin
{
	[PluginName("View Admin LinkHelper Plugin")]
	[PluginAuthors("Tim Hoffeller")]
	[PluginDescription("Helper Plugin for the viewadmin manager")]
	[PluginId("109A3C52-E543-452A-9CDF-BD9A32B51639")]
	[PluginManufacturer("CAD-Development")]
	[PluginVersion("1.0.0")]
	[PluginDependency(typeof(MainWindowApplication))]
	[PluginDependency(typeof(SDBeesDBConnection))]
	[PluginDependency(typeof(ViewAdmin))]
    [PluginDependency(typeof(GlobalManager))]

    public class ViewAdminLinkHelper : TemplateTreenodeHelper
	{
	    private readonly ViewAdminLinkHelperCtrl _myControl;

		public static ViewAdminLinkHelper Current { get; private set; }

	    public ViewAdminLinkHelper()
		{
			Current = this;
			_myControl = new ViewAdminLinkHelperCtrl(this);
		}

		protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
		{
            Console.WriteLine("View Admin LinkHelper Plugin starts\n");

            StartMe(context, e);
        }

		protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
		{
            Console.WriteLine("View Admin LinkHelper Plugin stops\n");
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
                _myControl.TemplateTreenodeTag = selectedTag;
                _myControl.ParentTemplateTreenodeTag = parentTag;
                _myControl.ViewId = viewId;

                tabPage.Controls.Clear();
                tabPage.Controls.Add(MyUserControl());
                MyUserControl().Dock = DockStyle.Fill;

                _myControl.UpdateView();
            }
        }

		public override string TabPageName()
		{
			return "Hierarchy editor";
		}

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            // empty
        }
	}
}
