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
using System.Drawing;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.DB;
using SDBees.Core.Model;

namespace SDBees.Core.Plugins.AEC.Door
{
    /// <summary>
    /// Provides a generic treenode plugin that other plugins can modify as their main treenode
    /// </summary>
    /// 

    [PluginName("AEC Door Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the basic door")]
    [PluginId("8CA76B44-2E2E-4A0C-85B8-CEEB0AACD069")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBeesDBConnection))]
    [PluginDependency(typeof(Global.GlobalManager))]


    public class AECDoor : Plugs.TemplateTreeNode.TemplateTreenode
    {
        private static AECDoor _theInstance;

        /// <summary>
        /// Returns the one and only instance.
        /// </summary>
        public static AECDoor Current
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Konstruktor des AECRoomNode
        /// </summary>
        public AECDoor()
            : base()
        {
            _theInstance = this;
            CreateMenuItem = "Create Door";
            DeleteMenuItem = "Delete Door";
            EditSchemaMenuItem = "Edit Door Schema";
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
                Console.WriteLine("DoorPlugin starts\n");

                StartMe(context, e);

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
            Console.WriteLine("DoorPlugin stops\n");
        }

        public override Icon GetIcon(Size size)
        {
            return SDBees.Plugins.Properties.Resources.SDBees_AEC_Door_AECDoor;
        }

        //public override void SetName(string sName)
        //{
        //    //throw new Exception("The method or operation is not implemented.");
        //}

        public override Table MyTable()
        {
            return AECDoorBaseData.gTable;
        }

        public override Plugs.TemplateBase.TemplateDBBaseData CreateDataObject()
        {
            return new AECDoorBaseData();
        }

        public override Plugs.TemplateBase.TemplatePlugin GetPlugin()
        {
            return _theInstance;
        }

        public override SDBeesEntityDefinition GetEntityDefinition()
        {
            return base.GetEntityDefinition(GetType());
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            InitDatabase();
        }

        private void InitDatabase()
        {
            // Das Databaseplugin besorgen
            if (DBManager != null)
            {
                // Verify that the required Tables are created/updated in the database
                var database = DBManager.Database;
                CreateDataObject().InitTableSchema(ref AECDoorBaseData.gTable, database);
            }
        }
    }

    public class AECDoorBaseData : Plugs.TemplateBase.TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable = null;

        #endregion

        #region Public Properties
        public override string GetTableName
        {
            get { return "usrAECDoors"; }
        }

        #endregion

        #region Constructor/Destructor

        public AECDoorBaseData() :
            base("Doorname", "Door", "General")
        {
            Table = gTable;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods
        /*
        protected override string TableName()
        {
            return "usrAECDoors";
        }
         * */

        #endregion
    }

}
