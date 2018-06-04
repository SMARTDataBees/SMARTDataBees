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
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.EDM;

using System.Configuration;
using System.IO;

using Carbon;
using Carbon.Plugins.Attributes;
using Carbon.Plugins;
using System.Data;

namespace SDBees.Reporting
{
    [PluginName("Reporting Manager Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the reporting manager")]
    [PluginId("380F73A8-D99E-40EA-971C-E4F753DBF773")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.EDM.EDMManager))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

    public class ReportingManager : SDBees.EDM.EDMTreeNodeHelper
    {
        private static ReportingManager m_theInstance = null;
        private EDMManager _myEDMManager = null;

        private ToolStripMenuItem _itemNew = null;
        private ToolStripMenuItem _itemDelete = null;
        private ToolStripMenuItem _itemModify = null;

        public static ReportingManager Current
        {
            get { return m_theInstance; }
        }

        public ReportingManager()
            : base()
        {
            m_theInstance = this;

            //_myControl = new ReportingUserControl();
            this._itemNew = new ToolStripMenuItem("New RDLReport");
            this._itemDelete = new ToolStripMenuItem("Delete RDLReport");
            this._itemModify = new ToolStripMenuItem("Modify / View RDLReport");

            this._itemModify.Click += new EventHandler(_itemModify_Click);
            this._itemNew.Click += new EventHandler(_itemNew_Click);
            this._itemDelete.Click += new EventHandler(_itemDelete_Click);
        }

        void _itemDelete_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void _itemNew_Click(object sender, EventArgs e)
        {
            try
            {
                string sFullPath = this._myEDMManager.GetFullCurrentPath();
                DataSet ds = SDBeesDBConnection.Current.GetDataSetForAllTables();

                AddReport _addReportDLG = new AddReport();
                DialogResult _dlgRes = _addReportDLG.ShowDialog();

                if (_dlgRes == DialogResult.OK)
                {
                    ReportDLG _reportDLG = new ReportDLG();
                    _reportDLG.Reportname = Path.Combine(sFullPath, _addReportDLG.ReportName);
                    _reportDLG.NewReport = true;
                    DialogResult _dlgResReport = _reportDLG.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        void _itemModify_Click(object sender, EventArgs e)
        {
            try
            {
                string sFullPath = this._myEDMManager.GetFullCurrentPath();


                //ReportDLG _reportDLG = new ReportDLG();
                //_reportDLG.Reportname = sFullPath;
                //_reportDLG.NewReport = false;
                //DialogResult _dlgResReport = _reportDLG.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
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
                Console.WriteLine("Reporting Manager Plugin starts\n");

                this.StartMe(context, e);

                //Das EDM-Manager Plugin besorgen
                if (context.PluginDescriptors.Contains(new PluginDescriptor(typeof(EDMManager))))
                {
                    _myEDMManager = (EDMManager)context.PluginDescriptors[typeof(SDBees.EDM.EDMManager)].PluginInstance;
                }
                else
                {
                    MessageBox.Show("Es konnte kein EDM-Manager gefunden werden!", this.ToString());
                    _myEDMManager = null;
                }

                InitDatabase();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            //MessageBox.Show("Das Raumplugin stirbt");
            Console.WriteLine("Reporting Manager Plugin stops\n");
        }

        public override string PluginSectionText()
        {
            return "Reporting Manager";
        }

        public override ToolStripMenuItem NewItemsMenue()
        {
            return this._itemNew;
        }

        public override void ReactOnNew()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override ToolStripMenuItem DeleteItemsMenue()
        {
            return this._itemDelete;
        }

        public override void ReactOnDelete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override ToolStripMenuItem ExecuteItemsMenue()
        {
            return this._itemModify;
        }

        public override void ReactOnExecute()
        {
            throw new Exception("The method or operation is not implemented.");
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
                ReportingBaseData.InitTableSchema(database);
            }
        }
    }
}
