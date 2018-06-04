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
using SDBees.Plugs.TemplateTreenode;


namespace SDBees.GIS.OutdoorArea
{
	/// <summary>
  /// Provides a generic treenode plugin that other plugins can modify as their main treenode
  /// </summary>
  /// 

  [PluginName("GIS Outdoor Area Plugin")]
  [PluginAuthors("Tim Hoffeller")]
  [PluginDescription("Plugin for the basic outdoor area")]
  [PluginId("2AFF6C0E-88F2-4936-BED3-29C137B5328F")]
  [PluginManufacturer("CAD-Development")]
  [PluginVersion("1.0.0")]
  [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
  [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]


  //[PluginTypeDef("Treenode")] //The SDBees PluginType


  public class GISOutdoorArea : SDBees.Plugs.TemplateTreenode.TemplateTreenode
  {
    private static GISOutdoorArea _theInstance;

    /// <summary>
    /// Returns the one and only instance.
    /// </summary>
    public static GISOutdoorArea Current
    {
      get
      {
        return _theInstance;
      }
    }

    /// <summary>
    /// Konstruktor des AECRoomNode
    /// </summary>
    public GISOutdoorArea() : base()
    {
      _theInstance = this;
	  CreateMenuItem.Text = "Create Outdoor Area Data";
	  DeleteMenuItem.Text = "Delete Outdoor Area Data";
	  EditSchemaMenuItem.Text = "Edit Outdoor Area Schema";
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
        Console.WriteLine("Outdoor Area Plugin starts\n");

        this.StartMe(context, e);

		//Das Databaseplugin besorgen
        if (MyDBManager != null)
        {

          // Verify that the required Tables are created/updated in the database
          Database database = MyDBManager.Database;
          InitTableSchema(ref GISOutdoorAreaBaseData.gTable, database);
        }
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
      Console.WriteLine("Outdoor Area Plugin stops\n");
    }


	  public override void SetName(string sName)
	  {
		  //throw new Exception("The method or operation is not implemented.");
	  }

      public override Table MyTable()
      {
          return GISOutdoorAreaBaseData.gTable;
      }

      public override TemplateTreenodeBaseData CreateDataObject()
      {
          return new GISOutdoorAreaBaseData();
      }
  }
}
