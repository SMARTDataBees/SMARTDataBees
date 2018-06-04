// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
//
// Copyright (C) 2007 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// #EndHeader# ================================================================
using System;
using System.Collections;
using System.Collections.Generic;

using System.Diagnostics;
using System.Reflection;

using System.Data;

using System.Text;
using System.Drawing;
using System.Windows.Forms;

using Carbon;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;

using SDBees.Plugs.Attributes;
using SDBees.Main.Window;
using SDBees.DB;
using SDBees.Plugs.TemplateTreenode;


namespace SDBees.AEC.Building
{
    /// <summary>
    /// Provides a generic treenode plugin that other plugins can modify as their main treenode
    /// </summary>
    /// 

    [PluginName("AEC Building Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for building")]
    [PluginId("A27DBFD2-5E66-4F30-A13C-4B9B901B22F5")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]

    //[PluginTypeDef("Treenode")] //The SDBees PluginType


    public class AECBuilding : SDBees.Plugs.TemplateTreenode.TemplateTreenode
    {
        private static AECBuilding _theInstance;

        /// <summary>
        /// Returns the one and only instance.
        /// </summary>
        public static AECBuilding Current
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Konstruktor des AECRoomNode
        /// </summary>
        public AECBuilding()
            : base()
        {
            _theInstance = this;
			CreateMenuItem.Text = "Create Building";
			DeleteMenuItem.Text = "Delete Building";
			EditSchemaMenuItem.Text = "Edit Building Schema";
        }

        /// <summary>
        /// Occurs when the plugin starts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                Console.WriteLine("BuildingPlugin starts\n");

                this.StartMe(context, e);

				//Das Databaseplugin besorgen
                if (MyDBManager != null)
                {
                    // Verify that the required Tables are created/updated in the database
                    Database database = MyDBManager.Database;
                    InitTableSchema(ref AECBuildingBaseData.gTable, database);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }
        }

		public override void SetName(string sName)
		{
			//throw new Exception("The method or operation is not implemented.");
		}

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
  
		protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("BuildingPlugin stops\n");
        }

        public override Table MyTable()
        {
            return AECBuildingBaseData.gTable;
        }

        public override TemplateTreenodeBaseData CreateDataObject()
        {
            return new AECBuildingBaseData();
        }
    }
}
