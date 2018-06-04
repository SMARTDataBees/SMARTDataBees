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
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Core.Model;

namespace SDBees.Core.Plugins.AEC.Building
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
	[PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

	//[PluginTypeDef("Treenode")] //The SDBees PluginType


	public class AECBuilding : SDBees.Plugs.TemplateTreeNode.TemplateTreenode
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
			CreateMenuItem = "Create Building";
			DeleteMenuItem = "Delete Building";
			EditSchemaMenuItem = "Edit Building Schema";
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

				InitDatabase();

				//setup event listener
				if(this.MyDBManager != null)
				{
					this.MyDBManager.AddDatabaseChangedHandler(AECBuilding_OnDatabaseChangedHandler);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				//throw;
			}
		}

		void AECBuilding_OnDatabaseChangedHandler(object myObject, EventArgs myArgs)
		{
			m_DefaultBuilding = null;
		}

		//public override void SetName(string sName)
		//{
		//    //throw new Exception("The method or operation is not implemented.");
		//}

		/// <summary>
		/// Occurs when the plugin stops.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="e"></param>
  
		protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
		{
			Console.WriteLine("BuildingPlugin stops\n");

			//remove event listener
			if (this.MyDBManager != null)
				this.MyDBManager.RemoveDatabaseChangedHandler(AECBuilding_OnDatabaseChangedHandler);
		}

		public override Icon GetIcon(Size size)
		{
			Icon result = null;
			result = SDBees.Core.Properties.Resources.SDBees_AEC_Building_AECBuilding;
			return result;
		}

		static AECBuildingBaseData m_DefaultBuilding = null;

		public AECBuildingBaseData CreateDefaultBuildingAsRoot()
		{
			Error _error = null;
			if (m_DefaultBuilding == null)
			{
				m_DefaultBuilding = CreateDataObject() as AECBuildingBaseData;
				m_DefaultBuilding.SetDefaults(this.MyDBManager.Database);

				if (m_DefaultBuilding != null)
				{
					m_DefaultBuilding.SetPropertyByColumn(SDBees.Plugs.TemplateBase.TemplateDBBaseData.m_NameColumnName, "Building");

					if (m_DefaultBuilding.Save(this.MyDBManager.Database, ref _error))
					{
						//create relation for view
						ViewAdmin.ViewRelation rel = new ViewAdmin.ViewRelation();
						rel.SetDefaults(this.MyDBManager.Database);
						rel.ParentId = Guid.Empty;
						rel.ParentType = ViewAdmin.ViewRelation.m_StartNodeValue;
						rel.ChildId = Guid.Parse(m_DefaultBuilding.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());
						rel.ChildName = m_DefaultBuilding.GetPropertyByColumn(SDBees.Plugs.TemplateBase.TemplateDBBaseData.m_NameColumnName).ToString();
						rel.ChildType = typeof(SDBees.Core.Plugins.AEC.Building.AECBuilding).ToString();

						rel.Save(this.MyDBManager.Database, ref _error);

						return m_DefaultBuilding;
					}
					else
						return null;
				}
				else
					return null;
			}
			else
				return m_DefaultBuilding;
		}

		public override Table MyTable()
		{
			return AECBuildingBaseData.gTable;
		}

		public override SDBees.Plugs.TemplateBase.TemplateDBBaseData CreateDataObject()
		{
			return new AECBuildingBaseData();
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
				this.CreateDataObject().InitTableSchema(ref AECBuildingBaseData.gTable, database);
			}
		}

		public override bool AllowRelationLinkingAsChild
		{
			get
			{
				return base.AllowRelationLinkingAsChild;
			}
		}

		public override bool AllowRelationLinkingAsSibling
		{
			get
			{
				return base.AllowRelationLinkingAsSibling;
			}
		}
	}

	public class AECBuildingBaseData : SDBees.Plugs.TemplateBase.TemplateDBBaseData
	{
		#region Private Data Members

		internal static Table gTable = null;

		#endregion

		#region Public Properties
		public override string GetTableName
		{
			get { return "usrAECBuildings"; }
		}

		#endregion

		#region Constructor/Destructor

		public AECBuildingBaseData() :
			base("Buildingname", "Building", "General")
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
			return "usrAECBuildings";
		}
		 * */

		#endregion

		public override bool CheckForUniqueName()
		{
			return true;
		}
	}
}
