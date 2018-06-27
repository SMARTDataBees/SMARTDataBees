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
using System.IO;
using System.Windows.Forms;
using fyiReporting.RdlViewer;
using SDBees.DB;
//using EasiReports;

namespace SDBees.Reporting
{
    public partial class ReportDLG : Form
    {
        string _sReportname = "";

        public string Reportname
        {
            get { return _sReportname; }
            set { _sReportname = value; }
        }

        bool _bNewReport;

        public bool NewReport
        {
            get { return _bNewReport; }
            set { _bNewReport = value; }
        }

        //Control for the Viewer
        RdlViewer _ctlReport;
        //EasiReports.ReportControl _ctlReport = null;

        public ReportDLG()
        {
            try
            {
                InitializeComponent();

                Error _error = null;

                //Testumgebung mit EasyReports Generator
                //EasiReports.License.Set("SDBees.Reporting", "BuiltNicer;4;CodeProject;71502708;4780EB20");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        string m_currentReportFile = "";
        private void ReportDLG_Load(object sender, EventArgs e)
        {
            _ctlReport = new RdlViewer();
            Controls.Add(_ctlReport);
            _ctlReport.Dock = DockStyle.Fill;

            //this._ctlReport.ShowButtons = true;
            //this._ctlReport.HelpNamespace = Application.StartupPath + "\\help\\EasiReports.chm";
            //this._ctlReport.HelpNavigator = HelpNavigator.Topic;

            m_currentReportFile = _sReportname + ".rdl";
            if (File.Exists(m_currentReportFile))
            {
                _ctlReport.SourceRdl = m_currentReportFile;
                
            }
            //_report.DataSourceObject = _myDB.Database.Connection.GetReadOnlyDataSet();
            //_report.DataSourceObject = _myDB.GetDataTableForPlugin("usrDummy1");
        }

        private void ReportDLG_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this._ctlReport.SaveAs(m_currentReportFile, )
        }
    }
}
