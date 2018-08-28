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
using System.Diagnostics;
using System.Windows.Forms;
using SDBees.Core.Utils;
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.ViewAdmin
{
    /// <summary>
    /// Form to display a view in a tree view
    /// </summary>
    public partial class ViewRelationTreeDLG : Form
    {
        private ViewRelationTreeView tvViewRelations;
        private SDBeesDBConnection m_dbManager;

        /// <summary>
        /// Gets or sets the view id this window works with
        /// </summary>
        public Guid ViewId
        {
            get { return tvViewRelations.ViewId; }
            set
            {
                tvViewRelations.ViewId = value;

                // rename the title of this window
                setTitle(tvViewRelations.ViewName);
            }
        }

        public Database Database
        {
            get
            {
                return m_dbManager.Database;
            }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ViewRelationTreeDLG(SDBeesDBConnection dbManager)
        {
            m_dbManager = dbManager;

            InitializeComponent();

            tvViewRelations = new ViewRelationTreeView(dbManager);

            panelView.Controls.Add(tvViewRelations);
            tvViewRelations.Dock = DockStyle.Fill;

            // DragDrop disable
            tvViewRelations.AllowDrop = false;
            //tvViewRelations.dra

            tvViewRelations.ViewSwitched += tvViewRelations_ViewSwitched;
            tvViewRelations.AfterSelNodeChanged += tvViewRelations_AfterSelNodeChanged;
        }

        /// <summary>
        /// Fills the treeview again, based on current db content
        /// </summary>
        public void RefreshView()
        {
            Error error = null;
            ArrayList objectIds = null;
            //if ((ViewId == Guid.Empty) && (ViewProperty.FindAllViewProperties(Database, ref objectIds, ref error) > 0))
            //{
            //    //ViewProperties viewProps = new ViewProperties();
            //    //viewProps.Load(mDatabase, objectIds[0], ref error);

            //    ViewId = new Guid((string)objectIds[0]);
            //}
            tvViewRelations.Fill(ref error);
        }

        TemplateTreenodeTag m_Tag;
        public TemplateTreenodeTag TagSelected
        {
            get { return m_Tag; }
        }

        void tvViewRelations_AfterSelNodeChanged(object sender, EventArgs e)
        {
            if (tvViewRelations.SelectedNode != null)
                m_Tag = tvViewRelations.SelectedNode.Tag as TemplateTreenodeTag;
        }

        private void ViewRelationWindow_Load(object sender, EventArgs e)
        {
            RefreshView();
            //this.BringToFront();

            Activate();
        }

        private void Activate()
        {
            var currentProcess = Process.GetCurrentProcess();
            var hWnd = currentProcess.MainWindowHandle;
            if (hWnd != User32.InvalidHandleValue)
            {
                User32.SetForegroundWindow(hWnd);
                User32.ShowWindow(hWnd, User32.SW_MAXIMIZE);
            }
        }

        private void setTitle (string viewName)
        {
            Text = "View: " + viewName;
        }

        private void tvViewRelations_ViewSwitched(object sender, ViewRelationTreeView.NotificationEventArgs args)
        {
            setTitle(args.ViewName);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
