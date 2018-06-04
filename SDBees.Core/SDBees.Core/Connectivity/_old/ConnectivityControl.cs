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
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.Connectivity
{
    /// <summary>
    /// Control for displaying information about, creating and modifying external connections
    /// </summary>
    public partial class ConnectivityControl : UserControl
    {
        #region Private Data Members

        private TemplateTreenodeTag mSelectedTag;
        private Guid mViewId;
        private ConnectivityInterface mInterface;
        private Database mDatabase;
        private ArrayList mObjectIds;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/Set the currently selected tag
        /// </summary>
        public TemplateTreenodeTag SelectedTag
        {
            get { return mSelectedTag; }
            set { mSelectedTag = value; }
        }

        /// <summary>
        /// Get/Set the viewId this control works with
        /// </summary>
        public Guid ViewId
        {
            get { return mViewId; }
            set { mViewId = value; }
        }

        /// <summary>
        /// Get/Set the Interface type this works with
        /// </summary>
        public ConnectivityInterface Interface
        {
            get { return mInterface; }
            set { mInterface = value; }
        }

        /// <summary>
        /// Get/Set the database
        /// </summary>
        public Database Database
        {
            get { return mDatabase; }
            set { mDatabase = value; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ConnectivityControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the control
        /// </summary>
        public void UpdateContents()
        {
            mObjectIds = null;

            if ((mInterface != null) && (mDatabase != null) && (mSelectedTag != null))
            {
                Error error = null;
                ExternalConnection.FindConnectionByNodeId(mDatabase, mInterface.InterfaceType(), mSelectedTag.NodeTypeOf, mSelectedTag.NodeGUID, ref mObjectIds, ref error);
                if (mObjectIds != null)
                {
                    if (mObjectIds.Count == 0)
                    {
                        mObjectIds = null;
                    }

                    this.ebOutput.Text = GetConnectionInformation();
                }
            }

            EnableControls();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Enable/Disable buttons and other controls...
        /// </summary>
        protected void EnableControls()
        {
            bool isConnected = (mObjectIds != null);

            bnConnect.Enabled = !isConnected;
            bnDisconnect.Enabled = isConnected;
            bnSynchronize.Enabled = isConnected;
            bnHighlight.Enabled = isConnected;
        }

        /// <summary>
        /// Get the information about the connections of the current node as a string
        /// </summary>
        /// <returns></returns>
        protected string GetConnectionInformation()
        {
            string message = "";

            if (mObjectIds != null)
            {
                Error error = null;

                foreach (object objectId in mObjectIds)
                {
                    ExternalConnection connection = new ExternalConnection();
                    if (connection.Load(mDatabase, objectId, ref error))
                    {
                        message += mInterface.GetDisplayInformation(connection.ExternalId, connection.ObjectType, connection.NodeId);
                    }
                    else
                    {
                        message += "Verbindung konnte nicht geöffnet werden!\r\n";
                    }
                }

                Error.Display("Verbindungs information konnte nicht ausgelesen werden.", error);
            }

            return message;
        }

        #endregion

        #region Events

        private void bnConnect_Click(object sender, EventArgs e)
        {
            Error error = null;

            TemplateTreenode plugin = TemplateTreenode.GetPluginForType(mSelectedTag.NodeTypeOf);
            if (plugin != null)
            {
                try
                {
                    SDBees.Plugs.TemplateBase.TemplateDBBaseData baseData = plugin.CreateDataObject();
                    if (baseData.Load(mDatabase, new Guid(mSelectedTag.NodeGUID), ref error))
                    {
                        bool exportAttributes = true;
                        bool importAttributes = true;
                        if (mInterface.LinkSDBeeNodeToExternalObjectInteractively(baseData, exportAttributes, importAttributes))
                        {
                            UpdateContents();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                error = new Error("Plugin für " + mSelectedTag.NodeTypeOf + " nicht gefunden!", 9999, this.GetType(), error);
            }

            Error.Display("Objekt konnte nicht verbunden werden", error);
        }

        private void bnDisconnect_Click(object sender, EventArgs e)
        {
            if (mObjectIds != null)
            {
                Error error = null;

                foreach (object objectId in mObjectIds)
                {
                    ExternalConnection connection = new ExternalConnection();
                    if (connection.Load(mDatabase, objectId, ref error))
                    {
                        connection.Erase(ref error);
                    }
                    else if (error == null)
                    {
                        error = new Error("Verbindung konnte nicht geöffnet werden!", 9999, this.GetType(), error);
                    }
                }

                Error.Display("Verbindungs information konnte nicht ausgelesen werden.", error);

                UpdateContents();
            }
        }

        private void bnImport_Click(object sender, EventArgs e)
        {
            if (mInterface != null)
            {
                mInterface.ImportObjects(mViewId, mSelectedTag);
            }
        }

        private void bnSynchronize_Click(object sender, EventArgs e)
        {
            try
            {
                if ((mInterface != null) && (mObjectIds != null))
                {
                    Error error = null;
                    foreach (object objectId in mObjectIds)
                    {
                        ExternalConnection connection = new ExternalConnection();
                        if (connection.Load(mDatabase, objectId, ref error))
                        {
                            string externalId = connection.ExternalId;
                            SDBees.Plugs.TemplateBase.TemplateDBBaseData nodeObject = connection.getNodeObject(ref error);

                            if (nodeObject != null)
                            {
                                mInterface.SynchronizeAttributes(externalId, nodeObject);

                                nodeObject.Save(ref error);
                            }
                            else
                            {
                                error = new Error("Keine Verbindung wurde gefunden!", 9999, this.GetType(), error);
                            }
                        }
                        else if (error == null)
                        {
                            error = new Error("Verbindung konnte nicht geöffnet werden!", 9999, this.GetType(), error);
                        }
                    }

                    Error.Display("Verbindungs information konnte nicht ausgelesen werden.", error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void bnHighlight_Click(object sender, EventArgs e)
        {
            if ((mInterface != null) && (mObjectIds != null))
            {
                List<string> externalIds = new List<string>();

                Error error = null;
                foreach (object objectId in mObjectIds)
                {
                    ExternalConnection connection = new ExternalConnection();
                    if (connection.Load(mDatabase, objectId, ref error))
                    {
                        externalIds.Add (connection.ExternalId);
                    }
                    else if (error == null)
                    {
                        error = new Error("Verbindung konnte nicht geöffnet werden!", 9999, this.GetType(), error);
                    }
                }

                mInterface.HighlightExternalObject(externalIds);

                Error.Display("Verbindungs information konnte nicht ausgelesen werden.", error);
            }
        }

        #endregion
    }
}
