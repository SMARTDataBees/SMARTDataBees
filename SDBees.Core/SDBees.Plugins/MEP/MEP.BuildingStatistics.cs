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
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Core.Model;

namespace SDBees.Core.Plugins.MEP.BuildingStatistics
{
    /// <summary>
    /// Provides a generic treenode plugin that other plugins can modify as their main treenode
    /// </summary>
    /// 

    [PluginName("HVAC Buildingstatistics Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the hvac buildings statistics")]
    [PluginId("2C6604DF-36FC-4CC8-8FA2-0B0D198FB31A")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

    //[PluginTypeDef("Treenode")] //The SDBees PluginType

    public class MEPBuildingsStats : SDBees.Plugs.TemplateTreeNode.TemplateTreenode
    {
        private static MEPBuildingsStats _theInstance;

        /// <summary>
        /// Returns the one and only instance.
        /// </summary>
        public static MEPBuildingsStats Current
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Konstruktor des AECRoomNode
        /// </summary>
        public MEPBuildingsStats()
            : base()
        {
            _theInstance = this;
            CreateMenuItem = "Create HVAC Buildingsstats";
            DeleteMenuItem = "Delete HVAC Buildingsstats";
            EditSchemaMenuItem = "Edit HVAC Buildingsstats Schema";
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
                Console.WriteLine("HVAC Buildingsstats Plugin starts\n");

                this.StartMe(context, e);

                InitDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }
        }

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("HVAC Buildingsstats Plugin stops\n");
        }

        public override Icon GetIcon(Size size)
        {
            return SDBees.Plugins.Properties.Resources.SDBees_MEP_BuildingStatistics_MEPBuildingsStats;
        }

        //public override void SetName(string sName)
        //{
        //    //throw new Exception("The method or operation is not implemented.");
        //}

        public override Table MyTable()
        {
            return MEPBuildingStatsBaseData.gTable;
        }

        public override SDBees.Plugs.TemplateBase.TemplateDBBaseData CreateDataObject()
        {
            return new MEPBuildingStatsBaseData();
        }

        public override Plugs.TemplateBase.TemplatePlugin GetPlugin()
        {
            return _theInstance;
        }

        public override SDBeesEntityDefinition GetEntityDefinition()
        {
            return base.GetEntityDefinition(this.GetType());
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            InitDatabase();
        }

        private void InitDatabase()
        {
            // Das Databaseplugin besorgen
            if (MyDBManager != null)
            {
                // Verify that the required Tables are created/updated in the database
                var database = MyDBManager.Database;
                this.CreateDataObject().InitTableSchema(ref MEPBuildingStatsBaseData.gTable, database);
            }
        }
    }

    public class MEPBuildingStatsBaseData : SDBees.Plugs.TemplateBase.TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable = null;

        #endregion

        #region Public Properties
        public override string GetTableName
        {
            get { return "usrMEPBuildingstatistics"; }
        }

        #endregion

        #region Constructor/Destructor

        public MEPBuildingStatsBaseData() :
            base("Key data name", "MEP key data", "General")
        {
            base.Table = gTable;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        /*
        protected override string TableName()
        {
            return "usrHVACBuildingstatistics";
        }
         * */

        #endregion
    }
}
