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
using System.Collections;
using System.IO;
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Global;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;
using Attribute = SDBees.DB.Attribute;

namespace SDBees.EDM
{
    [PluginName("EDM Manager Plugin")]
    [PluginAuthors("Gamal Kira")]
    [PluginDescription("Plugin for the edm manager")]
    [PluginId("B11CB5B7-82D2-455B-B99D-8E5A4A7E2119")]
    [PluginManufacturer("G.E.M Team-Solutions")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(MainWindowApplication))]
    [PluginDependency(typeof(SDBeesDBConnection))]
    [PluginDependency(typeof(GlobalManager))]

    public class EDMManager : TemplateTreenodeHelper
    {
        private static EDMManager _theInstance;
        private EDMUserControl1 _myControl;
        private string mRootDirectory;

        public static EDMManager Current
        {
            get { return _theInstance; }
        }

        public override string TabPageName()
        {
            return "EDM Daten";
        }

        public string RootDirectory
        {
            get { return mRootDirectory; }
        }

        public EDMManager()
        {
            _theInstance = this;

            _myControl = new EDMUserControl1();
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
                Console.WriteLine("EDM Manager Plugin starts\n");

                StartMe(context, e);

                mRootDirectory = MyDBManager.CurrentServerConfigItem.EDMRootDirectory;

                if (MyDBManager != null)
                    MyDBManager.AddDatabaseChangedHandler(EDMManager_OnDatabaseChangedHandler);

                if (mRootDirectory != "")
                {
                    // Verify that the root directory exists and create if it doesn't exist
                    if (!Directory.Exists(mRootDirectory))
                    {
                        Directory.CreateDirectory(mRootDirectory);
                    }
                }
                else
                {
                    MessageBox.Show("EDMRootDirectory muss in Konfiguration eingetragen werden");
                }

                InitDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        void EDMManager_OnDatabaseChangedHandler(object myObject, EventArgs myArgs)
        {
            if (MyDBManager != null)
                mRootDirectory = MyDBManager.CurrentServerConfigItem.EDMRootDirectory;
        }

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            //MessageBox.Show("Das Raumplugin stirbt");
            Console.WriteLine("EDM Manager Plugin stops\n");

            if (MyDBManager != null)
                MyDBManager.RemoveDatabaseChangedHandler(EDMManager_OnDatabaseChangedHandler);
        }

        /// <summary>
        /// Override this method if plugin should react to selection changes in the system tree view
        /// </summary>
        public override void UpdatePropertyPage(TabPage tabPage, Guid viewId, TemplateTreenodeTag selectedTag, TemplateTreenodeTag parentTag)
        {
            if (tabPage != null)
            {
                _myControl.TemplateTreenodeTag = selectedTag;

                tabPage.Controls.Clear();
                tabPage.Controls.Add(MyUserControl());
                MyUserControl().Dock = DockStyle.Fill;

                _myControl.Refresh();
            }
        }

        public int FindAllEDMDatas(Database database, ref ArrayList objectIds, ref Error error)
        {
            var table = EDMBaseData.Table;
            return database.Select(table, table.PrimaryKey, ref objectIds, ref error);
        }

        public int FindEDMDatasForPlugin(Database database, string pluginName, ref ArrayList objectIds, ref Error error)
        {
            return FindEDMDatasForObject(database, pluginName, "", ref objectIds, ref error);
        }

        public int FindEDMDatasForObject(Database database, string pluginName, string objectId, ref ArrayList objectIds, ref Error error)
        {
            var table = EDMBaseData.Table;
            var attributePlugin = new Attribute(table.Columns["plugin"], pluginName);
            var attributeObject = new Attribute(table.Columns["object_id"], objectId);
            var criteria1 = database.FormatCriteria(attributePlugin, DbBinaryOperator.eIsEqual, ref error);
            var criteria2 = database.FormatCriteria(attributeObject, DbBinaryOperator.eIsEqual, ref error);
            var criteria = "(" + criteria1 + ") AND (" + criteria2 + ")";
            return database.Select(table, table.PrimaryKey, criteria, ref objectIds, ref error);
        }

        public string GetFullPathname(string relativePath)
        {
            // TBD: prüfen ob relativePath überhaupt relativ ist
            //return mRootDirectory + "\\" + relativePath;
            try
            {
                return Path.Combine(mRootDirectory, relativePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }

        public string GetFullCurrentPath()
        {
            return GetFullPathname(_myControl.CurrentSelectedPath);
        }

        public bool IsInRootDirectory(string fullPath)
        {
            var dirFullPath = new DirectoryInfo(fullPath);
            var dirRoot = new DirectoryInfo(mRootDirectory);
            return dirFullPath.FullName.ToLower().Contains(dirRoot.FullName.ToLower()) ? true : false;
            //return (fullPath.Length >= mRootDirectory.Length) && (fullPath.Substring(0, mRootDirectory.Length).CompareTo(mRootDirectory) == 0);
        }

        /// <summary>
        /// Returns the relative Pathname
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public string GetRelativePathname(string fullPath)
        {
            var partialFoldername = fullPath;

            if (fullPath.CompareTo(mRootDirectory) == 0)
            {
                partialFoldername = ".";
            }
            else if (IsInRootDirectory(fullPath))
            {
                partialFoldername = fullPath.Substring(mRootDirectory.Length + 1, fullPath.Length - mRootDirectory.Length - 1);
            }

            return partialFoldername;
        }

        public override UserControl MyUserControl()
        {
            return _myControl;
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
                EDMBaseData.InitTableSchema(database);
            }
        }
    }
}
