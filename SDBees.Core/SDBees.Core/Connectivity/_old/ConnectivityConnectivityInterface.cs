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
using System.Text;
using System.Windows.Forms;

using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;
using SDBees.Main.Window;

using System.Configuration;
using System.IO;

using Carbon;
using Carbon.Plugins.Attributes;
using Carbon.Plugins;

namespace SDBees.Connectivity
{
    /// <summary>
    /// Abstract base class for external connectivity
    /// </summary>
    public abstract class ConnectivityInterface : TemplateTreenodeHelper
    {
        #region Private Data Members

        private ImportRules mImportRules;
        private ConnectivityControl mMyControl;
        private MenuItem mMenuInterface;
        private MenuItem mMenuSynchronizeAll;

        /// <summary>
        /// The root directory for external files
        /// </summary>
        protected string mRootDirectory;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get the import rules for this interface
        /// </summary>
        public ImportRules ImportRules
        {
            get { return mImportRules; }
        }
        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// default Constructor does nothing
        /// </summary>
        public ConnectivityInterface()
        {
            mImportRules = new ImportRules();

            List<TemplateTreenode> allTreenodePlugins = TemplateTreenode.GetAllPlugins();
            foreach (TemplateTreenode plugin in allTreenodePlugins)
            {
                if (plugin != null)
                {
                    plugin.ObjectDeleted += new TemplateTreenode.NotificationHandler(plugin_ObjectDeleted);
                }
            }

            mMyControl = new ConnectivityControl();
            mMyControl.Interface = this;

            mRootDirectory = "";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// returns the external interface type (e.g. Acad0001, Ifc00001, ...)
        /// </summary>
        /// <returns>the 8 character interface type code</returns>
        public abstract string InterfaceType();

        /// <summary>
        /// returns the external interface name (e.g. Autodesk Architectural Desktop 2006, ...)
        /// </summary>
        /// <returns>the readable name, also used in the menu</returns>
        public abstract string InterfaceName();

        /// <summary>
        /// exports the parameters from the give SDBees node
        /// </summary>
        /// <param name="externalId">the external id of the external target object</param>
        /// <param name="nodeObject">the source SDBees node object that contains the parameters</param>
        /// <returns>true, if successfull</returns>
        public abstract bool ExportAttributes(string externalId, SDBees.Plugs.TemplateBase.TemplateDBBaseData nodeObject);

        /// <summary>
        /// imports the parameters from the external linked object
        /// </summary>
        /// <param name="externalId">the external id of the external source object</param>
        /// <param name="nodeObject">the target SDBees node object that will receive the parameters</param>
        /// <returns>true, if succesfull</returns>
        public abstract bool ImportAttributes(string externalId, SDBees.Plugs.TemplateBase.TemplateDBBaseData nodeObject);

        /// <summary>
        /// Imports objects from an external document and creates nodes as children of the currentTag
        /// in the given view
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="currentTag"></param>
        /// <returns></returns>
        public abstract bool ImportObjects(Guid viewId, TemplateTreenodeTag currentTag);

        /// <summary>
        /// Synchronize the attributes between the external and the internal objects
        /// </summary>
        /// <param name="externalId">the external id of the external source object</param>
        /// <param name="nodeObject">the target SDBees node object</param>
        /// <returns>true, if succesfull</returns>
        public virtual bool SynchronizeAttributes(string externalId, SDBees.Plugs.TemplateBase.TemplateDBBaseData nodeObject)
        {
            bool exportSuccess = false;
            bool importSuccess = false;
            try
            {
                exportSuccess = ExportAttributes(externalId, nodeObject);
                importSuccess = ImportAttributes(externalId, nodeObject);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return importSuccess && exportSuccess;
        }

        /// <summary>
        /// Synchronize all Attributes of all connections for this interface type
        /// </summary>
        /// <returns></returns>
        public virtual int SynchronizeAllAttributes()
        {
            int syncCount = 0;

            try
            {
                Database database = SDBees.DB.SDBeesDBConnection.Current.Database;

                if (database == null)
                {
                    return syncCount;
                }

                // This might be a length process, so entertain the user meanwhile :-)
                ProgressTool progressTool = new ProgressTool();
                progressTool.StartActiveProcess(true, true);

                progressTool.WriteStatus("Verbindungen werden gesucht...");

                ArrayList objectIds = null;
                Error error = null;
                if (ExternalConnection.FindAllConnectionsForInterface(database, InterfaceType(), ref objectIds, ref error) > 0)
                {
                    int totalObjects = objectIds.Count;
                    int index = 0;
                    int numFailed = 0;

                    progressTool.ProgressBar.Maximum = totalObjects;

                    foreach (object objectId in objectIds)
                    {
                        ExternalConnection connection = new ExternalConnection();
                        if (connection.Load(database, objectId, ref error))
                        {
                            // synchronize now...
                            string externalId = connection.ExternalId;
                            SDBees.Plugs.TemplateBase.TemplateDBBaseData nodeObject = connection.getNodeObject(ref error);

                            if (SynchronizeAttributes(externalId, nodeObject))
                            {
                                syncCount++;

                                nodeObject.Save(ref error);
                            }
                            else
                            {
                                error = new Error("Externes Objekt konnte nicht synchronisiert werden!", 9999, this.GetType(), error);
                            }
                        }
                        else if (error == null)
                        {
                            error = new Error("Verbindung konnte nicht geöffnet werden!", 9999, this.GetType(), error);
                        }

                        index++;
                        progressTool.ProgressBar.Value = index;
                        string message = index + " Objekte synchronisiert";
                        if (numFailed > 0)
                        {
                            message += ", " + numFailed + " konnten nicht synchronisiert werden!";
                        }

                        progressTool.WriteStatus(message);
                    }
                }

                // that's enough of entertainment...
                progressTool.EndActiveProcess();

                Error.Display("Verbindungs information konnte nicht ausgelesen werden.", error);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return syncCount;
        }

        /// <summary>
        /// Get the information relevant to display to the user for this connection
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="objectType"></param>
        /// <param name="nodeId"></param>
        /// <returns>Message to display</returns>
        public virtual string GetDisplayInformation(string externalId, string objectType, string nodeId)
        {
            string message = "Verbunden mit " + externalId + "\r\n";

            return message;
        }

        /// <summary>
        /// Creates a new Node under the current selected Node
        /// </summary>
        /// <param name="objectType">Type of the Object/Node/Plugin</param>
        /// <param name="name">Name of the object (can be empty for unnamed objects)</param>
        /// <param name="externalId">Id of the object for the External Interface</param>
        /// <param name="importAttributes">Flag that denotes if paramters should be imported</param>
        /// <param name="parentType">Type of the node the object should be added to</param>
        /// <param name="parentId">Id of the node the object should be added to</param>
        /// <returns>the created node</returns>
        /// 
        public virtual SDBees.Plugs.TemplateBase.TemplateDBBaseData CreateSDBeesNodeAndConnection(string objectType, string name, string externalId, bool importAttributes, string parentType, Guid parentId)
        {
            Database database = SDBees.DB.SDBeesDBConnection.Current.Database;

            Error error = null;
            SDBees.Plugs.TemplateBase.TemplateDBBaseData nodeInstance = TemplateTreenode.AddNewChildNodeInstance(objectType, name, parentType, parentId);

            if (importAttributes)
            {
                if (ImportAttributes (externalId, nodeInstance))
                {
                    error = null;
                    nodeInstance.Save(ref error);

                    Error.Display("Failed to save attributes to object", error);
                }
            }

            CreateExternalConnection(database, objectType, nodeInstance.Id.ToString(), externalId, ref error);

            Error.Display("Failed to save connection object", error);

            return nodeInstance;
        }

        /// <summary>
        /// Create an external connection relationship between a node and an external id
        /// </summary>
        /// <param name="database"></param>
        /// <param name="objectType"></param>
        /// <param name="nodeId"></param>
        /// <param name="externalId"></param>
        /// <param name="error"></param>
        /// <returns>true if successful</returns>
        public virtual bool CreateExternalConnection(Database database, string objectType, string nodeId, string externalId, ref Error error)
        {
            bool success = false;

            if (database != null)
            {
                ExternalConnection connection = new ExternalConnection();
                connection.SetDefaults(database);
                connection.ObjectType = objectType;
                connection.NodeId = nodeId;
                connection.InterfaceType = InterfaceType();
                connection.ExternalId = externalId;
                success = connection.Save(ref error);
            }

            return success;
        }

        /// <summary>
        /// Checks if the given external id has previously been connected to a node and update the attributes if required
        /// </summary>
        /// <param name="externalId">Id of the object for the External Interface</param>
        /// <param name="importAttributes">Flag that denotes if parameters should be imported</param>
        /// <returns>Reference to persistent node or null</returns>
        public virtual SDBees.Plugs.TemplateBase.TemplateDBBaseData FindConnectedSDBeesNodeAndUpdate(string externalId, bool importAttributes)
        {
            SDBees.Plugs.TemplateBase.TemplateDBBaseData result = null;

            Database database = SDBees.DB.SDBeesDBConnection.Current.Database;

            Error error = null;
            ArrayList objectIds = null;
            if (ExternalConnection.FindConnectionByExternalId(database, InterfaceType(), externalId, ref objectIds, ref error) > 0)
            {
                if (objectIds.Count > 1)
                {
                    MessageBox.Show("Externes Objekt is mit mehreren SDBees Objekten verbunden... Datenbank bereinigen!");
                }
                ExternalConnection connection = new ExternalConnection();
                if (connection.Load(database, objectIds[0], ref error))
                {
                    TemplateTreenode plugin = TemplateTreenode.GetPluginForType(connection.ObjectType);
                    if (plugin != null)
                    {
                        result = plugin.CreateDataObject();
                        if (result.Load(database, connection.NodeId, ref error))
                        {
                            if (importAttributes)
                            {
                                if (ImportAttributes(externalId, result))
                                {
                                    error = null;
                                    result.Save(ref error);

                                    Error.Display("Failed to save attributes to object", error);
                                }

                            }
                        }
                        else
                        {
                            result = null;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates an external object of the same type as the node-object and links it with the node-object
        /// </summary>
        /// <param name="externalId">the external id of the created external object</param>
        /// <param name="nodeObject">SDBees node object to link with</param>
        /// <param name="exportAttributes">if true, parameters are exported</param>
        /// <returns>true, if successful</returns>
        public abstract bool CreateExternalObjectAndConnection(out string externalId, SDBees.Plugs.TemplateBase.TemplateDBBaseData nodeObject, bool exportAttributes);

        /// <summary>
        /// Lets the user select an external object interactively
        /// </summary>
        /// <returns>the external id of the selected object</returns>
        public abstract bool SelectExternalObjectInteractively(out string externalId);
        
        /// <summary>
        /// Lets the user select multiple external objects interactively
        /// </summary>
        /// <param name="externalIds"></param>
        /// <returns></returns>
        public abstract bool SelectExternalObjectsInteractively(out List<string> externalIds);

        /// <summary>
        /// Highlights external objects
        /// </summary>
        /// <param name="externalIds"></param>
        /// <returns></returns>
        public abstract bool HighlightExternalObject(List<string> externalIds);

        /// <summary>
        /// Create a connection between an SDBees node and an external object by user interaction
        /// </summary>
        /// <param name="nodeObject">the SDBees node to link with</param>
        /// <param name="exportAttributes">export attributes to linked external objects, if true</param>
        /// <param name="importAttributes">import attributes from external object, if true</param>
        /// <returns>true if successfull</returns>
        public virtual bool LinkSDBeeNodeToExternalObjectInteractively(SDBees.Plugs.TemplateBase.TemplateDBBaseData nodeObject, bool exportAttributes, bool importAttributes)
        {
            bool retval = false;
            string externalId;
            bool selected = SelectExternalObjectInteractively(out externalId);

            // We should get the focus back now...
            MyMainWindow.TheDialog.Activate();

            if (selected)
            {
                retval = true;

                Error error = null;
                ArrayList objectIds = null;
                if (ExternalConnection.FindConnectionByExternalId(nodeObject.Database, InterfaceType(), externalId, ref objectIds, ref error) > 0)
                {
                    MessageBox.Show("Element ist bereits verknüpft");
                    retval = false;
                }

                if (retval == true)
                {
                    //ImportRule rule = FindValidRule(externalId, nodeObject.ObjectType(), nodeObject.Id.ToString());
                    //if (rule == null)
                    //{
                    //    MessageBox.Show("Verknüpfung mit diesem Objekttyp ist ungültig!");
                    //    retval = false;
                    //}
                }

                if ((retval == true) && (error == null))
                {
                    //retval = CreateExternalConnection(nodeObject.Database, nodeObject.ObjectType(), nodeObject.Id.ToString(), externalId, ref error);
                }

                Error.Display("Konnte Externe Verbindung nicht erzeugen!", error);

                if ((retval == true) && importAttributes)
                {

                    if ((retval == true) && exportAttributes)
                    {
                        // TBD: ...
                    }

                    if (ImportAttributes(externalId, nodeObject))
                    {
                        error = null;
                        nodeObject.Save(ref error);

                        Error.Display("Failed to save attributes to object", error);
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// get external ids of objects in the external container
        /// </summary>
        /// <param name="containerId">the id of the container (filename, ...)</param>
        /// <param name="filter">all objects of the types that are listed in this array are returned, if empty all objects are returned</param>
        /// <param name="externalIds">the ids of the found objects</param>
        /// <param name="rules">The matching rules found for the external ids (in sync with externalIds</param>
        /// <returns>true if successful</returns>
        public abstract bool GetExternalIdsFromContainer(string containerId, List<string> filter, out List<string> externalIds, out List<ImportRule> rules);

        /// <summary>
        /// Find a rule for the matching combination
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="objectType"></param>
        /// <param name="nodeId"></param>
        /// <returns>A rule if found or null if combination not valid</returns>
        public abstract ImportRule FindValidRule(string externalId, string objectType, string nodeId);

        /// <summary>
        /// Name of the Tab in the properties window
        /// </summary>
        /// <returns></returns>
        public override string TabPageName()
        {
            return "Externe Verbindungen";
        }

        /// <summary>
        /// Override this method if plugin should react to selection changes in the system tree view
        /// </summary>
        public override void UpdatePropertyPage(TabPage tabPage, Guid viewId, TemplateTreenodeTag selectedTag, TemplateTreenodeTag parentTag)
        {
            if (tabPage != null)
            {
                this.mMyControl.SelectedTag = selectedTag;
                this.mMyControl.ViewId = viewId;
                this.mMyControl.Database = MyDBManager.Database;

                tabPage.Controls.Clear();
                tabPage.Controls.Add(this.mMyControl);
                this.mMyControl.Dock = DockStyle.Fill;

                this.mMyControl.UpdateContents();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Windows.Forms.UserControl MyUserControl()
        {
            return mMyControl;
        }

        /// <summary>
        /// Occurs when the main window plugin starts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            StartMe(context, e);

            Database database = SDBees.DB.SDBeesDBConnection.Current.Database;
            SDBees.Connectivity.ExternalConnection.InitTableSchema(database);

            // creating an inspector is enough, it will register itself...
            ConnectivityInspector inspector = new ConnectivityInspector(database);

            mRootDirectory = ConfigurationManager.AppSettings["EDMRootDirectory"];
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

            string keyName = "ImportRules-" + InterfaceType();
            string ruleFilename = ConfigurationManager.AppSettings[keyName];
            if ((ruleFilename != "") && (ruleFilename != null))
            {
                mImportRules.ReadFromConfig(ruleFilename);
            }
            else
            {
                MessageBox.Show(keyName + " muss in Konfiguration eingetragen werden!");
            }

            mMenuInterface = new MenuItem(this.InterfaceName());
            MyMainWindow.TheDialog.MenueTools.MenuItems.Add(mMenuInterface);

            mMenuSynchronizeAll = new MenuItem("Synchronize");
            mMenuSynchronizeAll.Click += new EventHandler(mMenuSynchronizeAll_Click);

            mMenuInterface.MenuItems.Add(mMenuSynchronizeAll);
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a full pathname considering the root (EDM) directory
        /// </summary>
        /// <param name="pathname"></param>
        /// <returns></returns>
        protected string MakeFullPathname(string pathname)
        {
            return Path.Combine(mRootDirectory, pathname);
        }

        /// <summary>
        /// Remove the root directory from the path if applicable
        /// </summary>
        /// <param name="pathname"></param>
        /// <returns></returns>
        protected string MakeRelativePathname(string pathname)
        {
            string result = pathname;

            if ((pathname.Length > mRootDirectory.Length) && (pathname.Substring(0, mRootDirectory.Length).CompareTo(mRootDirectory) == 0))
            {
                result = pathname.Substring(mRootDirectory.Length + 1);
            }

            return result;
        }

        /// <summary>
        /// Event callback when an object has been deleted. Any connection with this interface will be removed too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void plugin_ObjectDeleted(object sender, TemplateTreenode.NotificationEventArgs args)
        {
            ArrayList objectIds = null;
            Error error = null;
            if (ExternalConnection.FindConnectionByNodeId(args.BaseData.Database, InterfaceType(), args.Tag.NodeTypeOf, args.Tag.NodeGUID, ref objectIds, ref error) > 0)
            {
                foreach (object objectId in objectIds)
                {
                    ExternalConnection connection = new ExternalConnection();
                    if (connection.Load(args.BaseData.Database, objectId, ref error))
                    {
                        connection.Erase(ref error);
                    }
                }
            }
        }

        /// <summary>
        /// Event callback when tools menu is clicked to synchronize all connections
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mMenuSynchronizeAll_Click(object sender, EventArgs e)
        {
            SynchronizeAllAttributes();
        }

        #endregion
    }
}
