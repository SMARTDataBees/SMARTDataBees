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

namespace SDBees.Demoplugins.Dummys
{
    /// <summary>
    /// Provides a generic treenode plugin that other plugins can modify as their main treenode
    /// </summary>
    /// 

    [PluginName("Dummy1 Plugin for tests")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Dummy1 Plugin for tests")]
    [PluginId("A25F07FC-4134-41e2-8029-B5AE3675B540")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

    public class Dummy1 : SDBees.Plugs.TemplateTreeNode.TemplateTreenode
    {
        private static Dummy1 _theInstance;

        /// <summary>
        /// Returns the one and only instance.
        /// </summary>
        public static Dummy1 Current
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Konstruktor des Dummy1
        /// </summary>
        public Dummy1() : base()
        {
            _theInstance = this;
            CreateMenuItem = "Create Dummy1";
            DeleteMenuItem = "Delete Dummy1";
            EditSchemaMenuItem = "Edit Dummy1 Schema";
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
                Console.WriteLine("Dummy1 Plugin starts\n");

                this.StartMe(context, e);

                InitDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("Dummy1 Plugin stops\n");
        }

        public override Icon GetIcon(Size size)
        {
            Icon result = null;
            result = SDBees.Core.Properties.Resources.SDBees_Demoplugins_Dummys_Dummy1;
            return result;
        }

        //public override void SetName(string sName)
        //{
        //    //throw new Exception("The method or operation is not implemented.");
        //}

        public override Table MyTable()
        {
            return Dummy1BaseData.gTable;
        }

        public override SDBees.Plugs.TemplateBase.TemplateDBBaseData CreateDataObject()
        {
            return new Dummy1BaseData();
        }

        public override SDBees.Plugs.TemplateBase.TemplatePlugin GetPlugin()
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
                Database database = MyDBManager.Database;
                this.CreateDataObject().InitTableSchema(ref Dummy1BaseData.gTable, database);
            }
        }
    }

    [PluginName("Dummy2 Plugin for tests")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Dummy2 Plugin for tests")]
    [PluginId("2661C28A-C810-45e5-B4A7-3514D0372328")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

    public class Dummy2 : SDBees.Plugs.TemplateTreeNode.TemplateTreenode
    {
        private static Dummy2 _theInstance;

        /// <summary>
        /// Returns the one and only instance.
        /// </summary>
        public static Dummy2 Current
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Konstruktor des Dummy1
        /// </summary>
        public Dummy2()
            : base()
        {
            _theInstance = this;
            CreateMenuItem = "Create Dummy2";
            DeleteMenuItem = "Delete Dummy2";
            EditSchemaMenuItem = "Edit Dummy2 Schema";
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
                Console.WriteLine("Dummy2 Plugin starts\n");

                this.StartMe(context, e);

                InitDatabase();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("Dummy2 Plugin stops\n");
        }

        public override Icon GetIcon(Size size)
        {
            Icon result = null;
            result = SDBees.Core.Properties.Resources.SDBees_Demoplugins_Dummys_Dummy2;
            return result;
        }

        //public override void SetName(string sName)
        //{
        //    //throw new Exception("The method or operation is not implemented.");
        //}

        public override Table MyTable()
        {
            return Dummy2BaseData.gTable;
        }

        public override SDBees.Plugs.TemplateBase.TemplateDBBaseData CreateDataObject()
        {
            return new Dummy2BaseData();
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
                Database database = MyDBManager.Database;
                this.CreateDataObject().InitTableSchema(ref Dummy2BaseData.gTable, database);
            }
        }
    }

    public class Dummy1BaseData : SDBees.Plugs.TemplateBase.TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable = null;

        #endregion

        #region Public Properties
        public override string GetTableName
        {
            get { return "usrDummy1"; }
        }

        #endregion

        #region Constructor/Destructor

        public Dummy1BaseData() :
            base("Dummy1", "Dummy1 Daten", "General")
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

    public class Dummy2BaseData : SDBees.Plugs.TemplateBase.TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable = null;

        #endregion

        #region Public Properties
        public override string GetTableName
        {
            get { return "usrDummy2"; }
        }

        #endregion

        #region Constructor/Destructor

        public Dummy2BaseData() :
            base("Dummy2", "Dummy2 Daten", "General")
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
