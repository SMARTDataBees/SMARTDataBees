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
using System.Data;
using System.IO;
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Global;
using SDBees.DB;
using SDBees.EDM;
using SDBees.Main.Window;

namespace SDBees.Reporting
{
    [PluginName("Reporting Manager Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the reporting manager")]
    [PluginId("380F73A8-D99E-40EA-971C-E4F753DBF773")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(MainWindowApplication))]
    [PluginDependency(typeof(SDBeesDBConnection))]
    [PluginDependency(typeof(EDMManager))]
    [PluginDependency(typeof(GlobalManager))]

    public class ReportingManager : EDMTreeNodeHelper
    {
        private static ReportingManager m_theInstance;
        private EDMManager _myEDMManager;

        private ToolStripMenuItem _itemNew;
        private ToolStripMenuItem _itemDelete;
        private ToolStripMenuItem _itemModify;

        public static ReportingManager Current
        {
            get { return m_theInstance; }
        }

        public ReportingManager()
        {
            m_theInstance = this;

            //_myControl = new ReportingUserControl();
            _itemNew = new ToolStripMenuItem("New RDLReport");
            _itemDelete = new ToolStripMenuItem("Delete RDLReport");
            _itemModify = new ToolStripMenuItem("Modify / View RDLReport");

            _itemModify.Click += _itemModify_Click;
            _itemNew.Click += _itemNew_Click;
            _itemDelete.Click += _itemDelete_Click;
        }

        void _itemDelete_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void _itemNew_Click(object sender, EventArgs e)
        {
            try
            {
                var sFullPath = _myEDMManager.GetFullCurrentPath();
                var ds = SDBeesDBConnection.Current.GetDataSetForAllTables();

                var _addReportDLG = new AddReport();
                var _dlgRes = _addReportDLG.ShowDialog();

                if (_dlgRes == DialogResult.OK)
                {
                    var _reportDLG = new ReportDLG();
                    _reportDLG.Reportname = Path.Combine(sFullPath, _addReportDLG.ReportName);
                    _reportDLG.NewReport = true;
                    var _dlgResReport = _reportDLG.ShowDialog();
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
                var sFullPath = _myEDMManager.GetFullCurrentPath();


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

                StartMe(context, e);

                //Das EDM-Manager Plugin besorgen
                if (context.PluginDescriptors.Contains(new PluginDescriptor(typeof(EDMManager))))
                {
                    _myEDMManager = (EDMManager)context.PluginDescriptors[typeof(EDMManager)].PluginInstance;
                }
                else
                {
                    MessageBox.Show("Es konnte kein EDM-Manager gefunden werden!", ToString());
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
            return _itemNew;
        }

        public override void ReactOnNew()
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public override ToolStripMenuItem DeleteItemsMenue()
        {
            return _itemDelete;
        }

        public override void ReactOnDelete()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override ToolStripMenuItem ExecuteItemsMenue()
        {
            return _itemModify;
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
                var database = MyDBManager.Database;
                ReportingBaseData.InitTableSchema(database);
            }
        }
    }
}
