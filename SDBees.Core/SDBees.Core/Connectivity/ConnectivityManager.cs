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
using System.Windows.Forms;

using Carbon;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;

using SDBees.DB;
using SDBees.Main.Window;
using SDBees.GuiTools;

using SDBees.Core.Model;
using SDBees.Core.Connectivity.SDBeesLink;
using SDBees.Core.Connectivity.SDBeesLink.Service;

namespace SDBees.Core.Connectivity
{
    [PluginName("ConnectivityManager Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the connectivity to SmartDataBees")]
    [PluginId("519A4B0B-0C76-4829-9D09-28C9E1795745")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.Core.Main.Systemtray.ProcessIcon))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]
    /// <summary>
    /// Klasse ConnectivityManager
    /// </summary>
    public class ConnectivityManager : SDBees.Plugs.TemplateBase.TemplatePlugin
    {
        private static ConnectivityManager m_theInstance;
        private PluginContext m_context;
        private ViewAdmin.ViewAdmin m_viewAdmin;
        private Main.Systemtray.ProcessIcon m_ProcessIcon;
        private bool m_ready = false;

        /// <summary>
        /// Returns the one and only UserAdmin Plugin instance.
        /// </summary>
        public static ConnectivityManager Current
        {
            get
            {
                return m_theInstance;
            }
        }
        
        public PluginContext MyContext
        { get { return m_context; } }

        public bool Ready
        {
            get
            {
                return m_ready;
            }

            set
            {
                m_ready = value;
            }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ConnectivityManager()
            : base()
        {
            m_theInstance = this;
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
                Console.WriteLine("ConnectivityManager starts\n");
                m_context = context;

                //m_context.BeforePluginsStopped += _context_BeforePluginsStopped;
                StartMe(context, e);

                InitDatabase();

                //Das Viewadmin Plugin besorgen
                if (context.PluginDescriptors.Contains(new PluginDescriptor(typeof(ViewAdmin.ViewAdmin))))
                {
                    m_viewAdmin = (ViewAdmin.ViewAdmin)context.PluginDescriptors[typeof(ViewAdmin.ViewAdmin)].PluginInstance;
                }

                //Das Viewadmin Plugin besorgen
                if (context.PluginDescriptors.Contains(new PluginDescriptor(typeof(Main.Systemtray.ProcessIcon))))
                {
                    m_ProcessIcon = (Main.Systemtray.ProcessIcon)context.PluginDescriptors[typeof(Main.Systemtray.ProcessIcon)].PluginInstance;
                }

                //Open the WCF Host
                ConnectivityHost.Instance().OpenHost();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        //void _context_BeforePluginsStopped(object sender, PluginContextEventArgs e)
        //{
        //    if (m_connectedClients.Count > 0)
        //    {
        //        MessageBox.Show("There are clients connected to the external interface! Close them before proceeding!!");
        //    }
        //}

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("ConnectivityManager stops ...");
        }

        public override Table MyTable()
        {
            return ConnectivityManagerDocumentBaseData.gTable;
        }

        public Table MyTableAlienId()
        {
            return ConnectivityManagerAlienBaseData.gTable;
        }

        public override Plugs.TemplateBase.TemplateDBBaseData CreateDataObject()
        {
            return new ConnectivityManagerDocumentBaseData();
        }

        public Plugs.TemplateBase.TemplateDBBaseData CreateDataObjectAlienIds()
        {
            return new ConnectivityManagerAlienBaseData();
        }

        public override Plugs.TemplateBase.TemplatePlugin GetPlugin()
        {
            return m_theInstance;
        }

        static Hashtable m_connectedClients = new Hashtable();
        public Hashtable ConnectedClients
        {
            get { return ConnectivityManager.m_connectedClients; }
            set { ConnectivityManager.m_connectedClients = value; }
        }

        internal bool ExternalClientConnect(string name, SDBeesExternalPluginService externalPluginService)
        {
            if (m_ready)
            {
                if (!m_connectedClients.ContainsKey(name))
                {
                    m_connectedClients.Add(name, externalPluginService);
                }
            }

            return m_ready;
        }

        internal void ExternalClientDisconnect(string name, SDBeesExternalPluginService externalPluginService)
        {
            if (m_connectedClients.ContainsKey(name))
            {
                m_connectedClients.Remove(name);
            }
        }

        internal bool IsEditDataSet()
        {
            bool result = false;

            foreach (SDBeesExternalPluginService externalPluginService in m_connectedClients.Values)
            {
                result |= externalPluginService.IsEditDataSet;
            }

            return result;
        }

        internal void OnClose()
        {
            foreach (SDBeesExternalPluginService externalPluginService in m_connectedClients.Values)
            {
                if (!externalPluginService.IsEditDataSet)
                {
                    externalPluginService.ReturnFromEdit(null);
                }
            }
        }

        public void ShowEntity(string instanceId, HashSet<SDBeesAlienId> alienId)
        {
            foreach (SDBeesExternalPluginService externalPluginService in m_connectedClients.Values)
            {
                if(externalPluginService != null)
                    externalPluginService.ShowEntity(instanceId, alienId);
            }
        }

        internal SDBeesExternalDocument DocumentGet(SDBeesExternalDocument doc)
        {
            Error _error = null;

            SDBeesExternalDocument _doc = ConnectivityManagerDocumentBaseData.DocumentGet(doc, ref _error);

            Error.Display("Document get errors", _error);

            return _doc;
        }

        internal SDBeesExternalDocument DocumentRegister(SDBeesExternalDocument doc, string pluginId, string roleId)
        {
            Error _error = null;
            int count = -1;
            ArrayList lst = null;
            SDBeesExternalDocument _doc = null;

            if (!ConnectivityManagerDocumentBaseData.DocumentFound(doc, ref _error, ref count, ref lst))
            {
                _doc = ConnectivityManagerDocumentBaseData.DocumentAdd(doc, pluginId, roleId, ref _error);

               //Update();
            }

            Error.Display("Document registration errors", _error);

            return _doc;
        }

        internal SDBeesProjectId ProjectGetCurrentId()
        {
            return new SDBeesProjectId(MyDBManager.CurrentProjectId().ToString());
        }

        internal bool ProjectOpen(SDBeesProjectId _inputId)
        {
            bool result = false;

            if (_inputId != ProjectGetCurrentId())
            {
                result = MyDBManager.OpenProject(_inputId);

                if (result)
                {
                    Update();
                }
            }
            else
            {
                result = true;
            }

            return result;
        }

        internal bool ProjectOpen(string filenameDatabase, bool createIfNotFound)
        {
            bool result = false;

            if (filenameDatabase != MyDBManager.CurrentFilenameDatabase())
            {
                result = MyDBManager.OpenProject(filenameDatabase, createIfNotFound, false);

                if (result)
                {
                    Update();
                }
            }
            else
            {
                result = true;
            }

            return result;
        }

        /// <summary>
        /// EntityDefinitions for TreeNode Plugins
        /// Other Plugintypes currently not handled in Connectivity
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, SDBeesEntityDefinition> GetEntityDefinitions()
        {
            Dictionary<string, SDBeesEntityDefinition> m_Ents = new Dictionary<string, SDBeesEntityDefinition>();

            foreach (SDBees.Plugs.TemplateTreeNode.TemplateTreenode nod in SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetAllTreenodePlugins())
            {
                SDBeesEntityDefinition entdef = nod.GetEntityDefinition();
                m_Ents.Add(entdef.Id.ToString(), entdef);
            }

            return m_Ents;
        }

        internal SDBeesDataSet SynchronizeClient(SDBeesExternalDocument doc)
        {
#if PROFILER
            SDBees.Profiler.Enabled = true;

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeClient");
#endif

            if (doc.DocumentId == Guid.Empty.ToString())
            {
                MessageBox.Show("Document doesn't belong to current database! Check in before syncing!");
                return null;
            }

            SDBeesDataSet result = SDBeesDataSet.CreateExportDataset(doc);

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.RememberAndOpenNotepad();

            SDBees.Profiler.Enabled = false;
#endif

            return result;
        }

        public void SynchronizeServer(SDBeesExternalDocument doc, SDBeesDataSet data, SDBeesSyncMode mode)
        {
#if PROFILER
            SDBees.Profiler.Enabled = true;

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer");
#endif

            ProgressTool progressTool = new ProgressTool();
            progressTool.StartActiveProcess(true, true);
            progressTool.WriteStatus("Entities will be added to db ...");

            progressTool.ProgressBar.Maximum = data.Entities.Count;
            progressTool.ProgressBar.Value = 0;

            ///TBD : Check for valid document id
            if (doc.DocumentId == Guid.Empty.ToString())
            {
                MessageBox.Show("Document doesn't belong to current database! Checkin before syncing!");
                return;
            }

#if PROFILER
            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.Open");
#endif

            Error _error = null;
            ArrayList _objectIds = null;

            Connection connection = MyDBManager.Database.Open(false, ref _error);

            ArrayList alienIds = new ArrayList();

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.GetAlienIds");
#endif

            ConnectivityManagerAlienBaseData.GetAlienIds(ref _error, ref alienIds);

            Dictionary<object, List<object>> objectId2AlienIdsMap = ConnectivityManagerAlienBaseData.GetObjectId2AlienIdsMap(connection, ref _error);

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.Entities");
#endif

            Hashtable _newObjects = new Hashtable();

            try
            {
                ArrayList _lstDocs = new ArrayList();
                foreach (SDBeesCADDocument caddoc in data.Documents)
                {
                    int count = 0;
                    if (SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.DocumentFound(caddoc.Id.ToString(), ref _error, ref count, ref _lstDocs))
                    {
                        foreach (var idDocs in _lstDocs)
                        {
                            Connectivity.ConnectivityManagerDocumentBaseData docData = new ConnectivityManagerDocumentBaseData();
                            if (docData.Load(MyDBManager.Database, idDocs, ref _error))
                            {
                                docData.SetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentCADInfoColumnName, SDBeesDocumentCADInfo.Serialize(caddoc.CADInfo));
                                docData.Save(MyDBManager.Database, ref _error);
                            }
                        }
                    }
                }

                foreach (SDBeesEntity elem in data.Entities)
                {
                    foreach (SDBeesAlienId alienId in elem.AlienIds)
                    {
                        if (ConnectivityManagerAlienBaseData.AlienIdFound(alienId.AlienInstanceId.ToString(), alienId.DocumentId.Id, ref _error, ref _objectIds))
                        {
                            // Load the alienid record
                            Connectivity.ConnectivityManagerAlienBaseData idAlien = new ConnectivityManagerAlienBaseData();
                            if (idAlien.Load(MyDBManager.Database, _objectIds[0], ref _error))
                            {
                                //TBD Update for already existing db objects
                                //Currently only existing entities esp their parameters are updated
                                //No alienids and relations will be added...
                                SDBees.Plugs.TemplateTreeNode.TemplateTreenode plug = SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetPluginForType(elem.DefinitionId.ToString());
                                if (plug != null)
                                {
                                    Plugs.TemplateBase.TemplateDBBaseData dbObject = plug.CreateDataObject();
                                    object dbOjectId = idAlien.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName);
                                    if (dbObject.Load(MyDBManager.Database, dbOjectId, ref _error))
                                    {
                                        SetObjectData(ref _error, elem, dbObject);
                                        alienIds.Remove(_objectIds[0]);
                                        if (mode == SDBeesSyncMode.UpdateServer)
                                        {
                                            if (objectId2AlienIdsMap != null)
                                            {
                                                List<object> objectAlienIds = null;
                                                if (objectId2AlienIdsMap.TryGetValue(dbOjectId, out objectAlienIds))
                                                {
                                                    foreach (object objectAlienId in objectAlienIds)
                                                    {
                                                        alienIds.Remove(objectAlienId);
                                                    }
                                                }
                                            }
                                        }
                                        //break;
                                    }
                                }

                                //TH TBD: WE have to handle additional AlienIds, Relations?
                            }
                        }
                        else
                        {
                            // We have a new object, never synced before!
                            // Or it has a new alienId?
                            SDBees.Plugs.TemplateTreeNode.TemplateTreenode plug = SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetPluginForType(elem.DefinitionId.Id);
                            if (plug != null)
                            {
                                if (!String.IsNullOrEmpty(elem.Id.ToString()))
                                {
                                    //TBD : Object exists in db, we have to add a new alien id
                                    Plugs.TemplateBase.TemplateDBBaseData dataObj = plug.CreateDataObject();
                                    if (dataObj.Load(MyDBManager.Database, elem.Id, ref _error))
                                    {
                                        SetObjectData(ref _error, elem, dataObj);
                                        ConnectivityManagerAlienBaseData.AlienIdsAdd(plug.GetType().ToString(), dataObj.Id.ToString(), elem.AlienIds, ref _error);
                                        alienIds.Remove(dataObj.Id);
                                    }
                                    else
                                    {
                                        //We are in first run from Import?
                                        _error = CreateEntityAndAddAlienIds(_error, _newObjects, elem, alienId, plug);
                                    }
                                }
                                else
                                {
                                    // We have a new object, never added to db before
                                    try
                                    {
                                        _error = CreateEntityAndAddAlienIds(_error, _newObjects, elem, alienId, plug);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    progressTool.ProgressBar.Value++;
                }
            }
            catch (Exception ex)
            {

            }

            progressTool.EndActiveProcess();

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.RelationsAddRoot");
#endif

            try
            {
                //Add relation to root object
                AddRelationsToRoot(doc, _newObjects, ref _error);
            }
            catch (Exception ex)
            {
            }

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.RelationsAddChild");
#endif

            try
            {
                //Add relations to child objects
                AddRelationsToChild(_newObjects, data, ref _error);
            }
            catch (Exception ex)
            {
            }

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.DeleteUntouchedObjects");
#endif

            DeleteUntouchedObjects(alienIds, objectId2AlienIdsMap, mode, ref _error);

            Error.Display("Errors when importing Dataset", _error);

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.Close");
#endif

            MyDBManager.Database.Close(ref _error);

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.OnSyncronizationWithServerEnded");
#endif

            // Fire event when Syncronization is finished
            OnSyncronizationWithServerEnded(this, new SyncArgs("Sync with server ended"));

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Stop();

            SDBees.Profiler.RememberAndOpenNotepad();

            SDBees.Profiler.Enabled = false;
#endif
        }

        private Error CreateEntityAndAddAlienIds(Error _error, Hashtable _newObjects, SDBeesEntity elem, SDBeesAlienId alienId, SDBees.Plugs.TemplateTreeNode.TemplateTreenode plug)
        {
            try
            {
                Plugs.TemplateBase.TemplateDBBaseData dataObj = plug.CreateDataObject();
                dataObj.SetDefaults(ConnectivityManager.Current.MyDBManager.Database);

                SetObjectData(ref _error, elem, dataObj);

                // Add to table for alien id handling
                ConnectivityManagerAlienBaseData.AlienIdsAdd(plug.GetType().ToString(), dataObj.Id.ToString(), elem.AlienIds, ref _error);

                // Add to internal collection for relationship handling
                KeyValuePair<Plugs.TemplateBase.TemplateDBBaseData, SDBeesEntity> kvpair = new KeyValuePair<Plugs.TemplateBase.TemplateDBBaseData, SDBeesEntity>(dataObj, elem);
                _newObjects.Add(alienId.AlienInstanceId.ToString(), kvpair);
            }
            catch (Exception ex)
            {
            }

            return _error;
        }

        private void DeleteUntouchedObjects(ArrayList objectIds, Dictionary<object, List<object>> objectId2AlienIdsMap, SDBeesSyncMode mode, ref Error error)
        {
            Dictionary<SDBees.Plugs.TemplateTreeNode.TemplateTreenode, List<Plugs.TemplateBase.TemplateDBBaseData>> map = new Dictionary<SDBees.Plugs.TemplateTreeNode.TemplateTreenode, List<Plugs.TemplateBase.TemplateDBBaseData>>();

            foreach (object objectId in objectIds)
            {
                Connectivity.ConnectivityManagerAlienBaseData idAlien = new ConnectivityManagerAlienBaseData();
                if (idAlien.Load(MyDBManager.Database, objectId, ref error))
                {
                    object type = idAlien.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementTypeColumnName);
                    SDBees.Plugs.TemplateTreeNode.TemplateTreenode plug = SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetPluginForType(type.ToString());
                    if (plug != null)
                    {
                        Plugs.TemplateBase.TemplateDBBaseData dbObject = plug.CreateDataObject();
                        if (dbObject.Load(MyDBManager.Database, idAlien.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName), ref error))
                        {
                            if (map.ContainsKey(plug))
                            {
                                List<Plugs.TemplateBase.TemplateDBBaseData> objects = map[plug];
                                objects.Add(dbObject);
                            }
                            else
                            {
                                List<Plugs.TemplateBase.TemplateDBBaseData> objects = new List<Plugs.TemplateBase.TemplateDBBaseData>();
                                objects.Add(dbObject);
                                map.Add(plug, objects);
                            }

                            if (mode == SDBeesSyncMode.UpdateServerValidation)
                            {
                                if (objectId2AlienIdsMap != null)
                                {
                                    List<object> objectAlienIds = null;
                                    if (objectId2AlienIdsMap.TryGetValue(dbObject.Id, out objectAlienIds))
                                    {
                                        objectAlienIds.Remove(objectId);
                                    }
                                }
                            }
                        }
                    }

                    idAlien.Erase(ref error);
                }
            }

            foreach (KeyValuePair<SDBees.Plugs.TemplateTreeNode.TemplateTreenode, List<Plugs.TemplateBase.TemplateDBBaseData>> keyValue in map)
            {
                if (mode == SDBeesSyncMode.UpdateServer)
                {
                    keyValue.Key.DeleteInstancesAndRelations(keyValue.Value, ref error);
                }
                else if (mode == SDBeesSyncMode.UpdateServerValidation)
                {
			        foreach (Plugs.TemplateBase.TemplateDBBaseData item in keyValue.Value)
			        {
                        int numberOfAlienIds = (objectId2AlienIdsMap != null) ? objectId2AlienIdsMap[item.Id].Count :  0;

                        if (numberOfAlienIds == 0)
                        {
    				        keyValue.Key.DeleteInstanceAndRelations(item, ref error);
                        }
			        }
                }
            }
        }

        internal void ShowServerDialog(SDBeesExternalDocument doc)
        {
            ///TBD : Check for valid document id
            if (doc.DocumentId == Guid.Empty.ToString())
            {
                MessageBox.Show("Document doesn't belong to current database! Checkin before syncing!");
                return;
            }

            Update();

            // This doesn't work!
            //MyContext.ApplicationContext.MainForm.Show();

            // This works!
            //this.MyMainWindow.TheDialog.ShowMainDialog();

            // This works!
            if (this.m_ProcessIcon != null)
            {
                this.m_ProcessIcon.OpenMainDialog();
            }
        }

        internal void UpdateServerDialog()
        {
            Update();
        }

        // The delegate procedure we are assigning to our object
        public delegate void SynchronizeHandler(object myObject, SyncArgs myArgs);

        public event SynchronizeHandler OnSyncronizationWithServerEnded;

        public static void SetObjectData(ref Error _error, SDBeesEntity elem, Plugs.TemplateBase.TemplateDBBaseData dataObj)
        {
            if (!String.IsNullOrEmpty(elem.InstanceId.Id.ToString()))
                dataObj.SetPropertyByColumn(SDBees.DB.Object.m_IdSDBeesColumnName, elem.InstanceId.Id.ToString());

            foreach (SDBeesProperty prp in elem.Properties)
            {
                foreach (SDBees.DB.Column col in dataObj.Table.Columns.Values)
                {
                    if (col.Name == prp.DefinitionId)
                    {
                        dataObj.SetPropertyByColumn(col.Name, prp.InstanceValue.ObjectValue);
                        break;
                    }
                }
            }

            dataObj.Save(ConnectivityManager.Current.MyDBManager.Database, ref _error);

            //elem.Id.Id = dataObj.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString();
            //elem.InstanceId.Id = dataObj.GetPropertyByColumn(SDBees.DB.Object.m_IdSDBeesColumnName).ToString();
        }

        private void AddRelationsToChild(Hashtable _newObjects, SDBeesDataSet data, ref Error _error)
        {
#if PROFILER
            SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild");
#endif

            ProgressTool progressTool = new ProgressTool();
            progressTool.StartActiveProcess(true, true);

            progressTool.WriteStatus("Relations to childs will be added ...");

            progressTool.ProgressBar.Maximum = data.Relations.Count;
            progressTool.ProgressBar.Value = 0;

            foreach (SDBeesRelation relation in data.Relations)
            {
#if PROFILER
                SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild.1");
#endif

                if ((relation.AlienSourceId != null) && !String.IsNullOrEmpty(relation.AlienSourceId.AlienInstanceId.ToString()))
                {
                    if (_newObjects.ContainsKey(relation.AlienSourceId.AlienInstanceId.ToString()))
                    {
                        KeyValuePair<Plugs.TemplateBase.TemplateDBBaseData, SDBeesEntity> item = (KeyValuePair<Plugs.TemplateBase.TemplateDBBaseData, SDBeesEntity>)_newObjects[relation.AlienSourceId.AlienInstanceId.ToString()];

                        ArrayList lstTarget = null;

#if PROFILER
                        SDBees.Profiler.Start("ConnectivityManagerAlienBaseData.AlienIdFound");
#endif
                        bool ok1 = ConnectivityManagerAlienBaseData.AlienIdFound(relation.AlienTargetId.AlienInstanceId.ToString(), relation.AlienTargetId.DocumentId.Id, ref _error, ref lstTarget);

#if PROFILER
                        SDBees.Profiler.Stop();
#endif

                        if (ok1)
                        {
#if PROFILER
                            SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild.2");
#endif

                            Plugs.TemplateBase.TemplateDBBaseData plug = this.CreateDataObjectAlienIds();
                            if (plug.Load(this.MyDBManager.Database, lstTarget[0], ref _error))
                            {
                                ViewAdmin.ViewRelation rel = new ViewAdmin.ViewRelation();
                                rel.SetDefaults(this.MyDBManager.Database);
                                rel.ParentId = Guid.Parse(item.Key.Id.ToString());
                                rel.ParentType = GetNodeType(item.Value);
                                rel.ChildId = Guid.Parse(plug.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName).ToString());
                                rel.ChildType = plug.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementTypeColumnName).ToString();
                                rel.ChildName = GetChildName(rel.ChildId, rel.ChildType, ref _error);

#if PROFILER
                                SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild.3");
#endif
                                ArrayList objIds = null;
                                bool ok2 = ViewAdmin.ViewRelation.FindViewRelationByParentIdAndChildId(MyDBManager.Database, rel.ParentId, rel.ChildId, ref objIds, ref _error);

#if PROFILER
                                SDBees.Profiler.Stop();
#endif

                                if (!ok2)
                                {
#if PROFILER
                                    SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild.4");
#endif

                                    rel.Save(this.MyDBManager.Database, ref _error);

#if PROFILER
                                    SDBees.Profiler.Stop();
#endif
                                }
                            }

#if PROFILER
                            SDBees.Profiler.Stop();
#endif

                        }
                    }
                }
                progressTool.ProgressBar.Value++;

#if PROFILER
                SDBees.Profiler.Stop();
#endif

            }
            progressTool.EndActiveProcess();

#if PROFILER
            SDBees.Profiler.Stop();
#endif
        }

        private string GetChildName(Guid guid, string p, ref Error _error)
        {
            string temp = "Temp";
            foreach (SDBees.Plugs.TemplateTreeNode.TemplateTreenode plug in SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetAllTreenodePlugins())
            {
                if (plug.GetType().ToString() == p)
                {
                    Plugs.TemplateBase.TemplateDBBaseData data = plug.CreateDataObject();
                    data.Load(this.MyDBManager.Database, guid, ref _error);
                    temp = data.GetPropertyByColumn(SDBees.Plugs.TemplateBase.TemplateDBBaseData.m_NameColumnName).ToString();
                }
            }

            return temp;
        }

        private void AddRelationsToRoot(SDBeesExternalDocument doc, Hashtable _newObjects, ref Error _error)
        {
            Plugs.TemplateBase.TemplateDBBaseData _rootDoc = ConnectivityManagerDocumentBaseData.GetDocumentData(doc.DocumentId.Id, ref _error);
            if (_rootDoc != null)
            {
                ProgressTool progressTool = new ProgressTool();
                progressTool.StartActiveProcess(true, true);

                progressTool.WriteStatus("Root relations will be created ...");

                progressTool.ProgressBar.Maximum = _newObjects.Values.Count;
                progressTool.ProgressBar.Value = 0;

                foreach (KeyValuePair<Plugs.TemplateBase.TemplateDBBaseData, SDBeesEntity> item in _newObjects.Values)
                {
                    //check if relation already exists
                    ArrayList _oIdsFound = null;
                    if (ViewAdmin.ViewRelation.FindViewRelationByChildId(MyDBManager.Database, Guid.Parse(item.Key.Id.ToString()), ref _oIdsFound, ref _error) > 0)
                    {
                        bool idfound = false;
                        // we have alreads relations for this child, check if same root is already in place
                        ArrayList _idRoot = null;
                        if (ViewAdmin.ViewRelation.FindViewRelationByParentId(MyDBManager.Database, Guid.Parse(_rootDoc.GetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName).ToString()), ref _idRoot, ref _error) > 0)
                        {
                            foreach (string idChild in _idRoot)
                            {
                                if (idChild == _oIdsFound[0].ToString())
                                {
                                    idfound = true;
                                    break;
                                }
                            }
                        }
                        if (idfound)
                        {
                            continue;
                        }
                    }

                    ViewAdmin.ViewRelation rel = new ViewAdmin.ViewRelation();
                    rel.SetDefaults(this.MyDBManager.Database);
                    rel.ParentId = Guid.Parse(_rootDoc.GetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName).ToString());
                    rel.ParentType = _rootDoc.GetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootTypeColumnName).ToString();
                    rel.ChildId = Guid.Parse(item.Key.Id.ToString());
                    rel.ChildName = item.Key.GetPropertyByColumn(SDBees.Plugs.TemplateBase.TemplateDBBaseData.m_NameColumnName).ToString();
                    rel.ChildType = GetNodeType(item.Value);

                    rel.Save(this.MyDBManager.Database, ref _error);

                    progressTool.ProgressBar.Value++;
                }
                progressTool.EndActiveProcess();
            }
        }

        private string GetNodeType(SDBeesEntity sDBeesEntity)
        {
            string m_childType = null;
            foreach (SDBees.Plugs.TemplateTreeNode.TemplateTreenode plug in SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetAllTreenodePlugins())
            {
                if (plug.GetType().ToString().Contains(sDBeesEntity.DefinitionId.ToString()))
                {
                    m_childType = plug.GetType().ToString();
                    break;
                }
            }

            return m_childType;
        }

        internal void SetupBuildingSubLevels(SDBeesExternalDocument doc, SDBeesDataSet data)
        {
            //foreach (SDBeesEntity ent in data.Entities)
            //{
            //    if (ent.DefinitionId == typeof(SDBees.Core.Plugins.AEC.Level.AECSubLevel).ToString())
            //    {
            //        //Setup the sub levels
            //    }
            //}
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
                // Create the required database tables...
                InitTableSchema(ref ConnectivityManagerDocumentBaseData.gTable, MyDBManager.Database);
                this.CreateDataObjectAlienIds().InitTableSchema(ref ConnectivityManagerAlienBaseData.gTable, MyDBManager.Database);
            }
        }

        private void Update()
        {
            if (MyMainWindow.TheDialog.Visible) MyDBManager.OnUpdate(null);
        }
    }

    public class SyncArgs : EventArgs
    {
        private string m_message;

        public SyncArgs(string message)
        {
            this.m_message = message;
        }

        public string Message
        {
            get
            {
                return m_message;
            }
        }
    }

    public class ConnectivityManagerDocumentBaseData : SDBees.Plugs.TemplateBase.TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable = null;

        #endregion

        #region Public Properties
        public override string GetTableName
        {
            get { return "sdbConnectivityManagerAlienDoc"; }
        }

        #endregion

        #region Constructor/Destructor

        public ConnectivityManagerDocumentBaseData() :
            base("ConnectivityManager", "ExternalConnection", "General")
        {
            base.Table = gTable;
        }

        #endregion

        internal static bool DocumentGetRootAndType(ref Plugs.TemplateBase.TemplateDBBaseData idObj, bool interactive)
        {
            if (interactive)
            {
                return GetDocumentRootAndTypeInteractive(ref idObj);
            }
            else
            {
                return GetDocumentRootAndTypeNonInteractive(ref idObj);
            }
        }

        private static bool GetDocumentRootAndTypeNonInteractive(ref Plugs.TemplateBase.TemplateDBBaseData idObj)
        {
            //create building
            // Sollte dann noch schöner gemacht werden, da es sehr FS spezifisch ist und hier nichts verloren hat, da wir ja in den SDBees sind.
            //SDBees.Core.Plugins.AEC.Building.AECBuildingBaseData _basedt = SDBees.Core.Plugins.AEC.Building.AECBuilding.Current.CreateDefaultBuildingAsRoot();
            //if (_basedt != null)
            //{
            //    // return values
            //    idObj.SetPropertyByColumn(m_DocumentRootColumnName, _basedt.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());
            //    idObj.SetPropertyByColumn(m_DocumentRootTypeColumnName, SDBees.Core.Plugins.AEC.Building.AECBuildingBaseData.GetPluginForBaseData(_basedt).GetType().ToString());
            //    return true;
            //}
            //else
            //{ return false; }
            return false;
        }

        private static bool GetDocumentRootAndTypeInteractive(ref Plugs.TemplateBase.TemplateDBBaseData rootDocData)
        {
            if (SDBees.ViewAdmin.ViewAdmin.Current.MyViewRelationWindow().ShowDialog() == DialogResult.OK)
            {
                rootDocData.SetPropertyByColumn(m_DocumentRootColumnName, SDBees.ViewAdmin.ViewAdmin.Current.MyViewRelationWindow().TagSelected.NodeGUID);
                rootDocData.SetPropertyByColumn(m_DocumentRootTypeColumnName, SDBees.ViewAdmin.ViewAdmin.Current.MyViewRelationWindow().TagSelected.NodeTypeOf);
                return true;
            }
            else
                return false;
        }

        internal static Plugs.TemplateBase.TemplateDBBaseData GetDocumentData(string docid, ref Error _error)
        {
            SDBees.DB.Attribute attDocRoot = new SDBees.DB.Attribute(ConnectivityManagerDocumentBaseData.gTable.Columns[m_IdSDBeesColumnName], docid);
            string criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attDocRoot, DbBinaryOperator.eIsEqual, ref _error);

            ArrayList objectIds = null;
            int count = ConnectivityManager.Current.MyDBManager.Database.Select(ConnectivityManagerDocumentBaseData.gTable, ConnectivityManagerDocumentBaseData.gTable.PrimaryKey, criteria, ref objectIds, ref _error);
            if (count > 0)
            {
                Plugs.TemplateBase.TemplateDBBaseData _root = ConnectivityManager.Current.CreateDataObject();
                _root.Load(ConnectivityManager.Current.MyDBManager.Database, objectIds[0], ref _error);
                return _root;
            }
            else
                return null;
        }

        /// <summary>
        /// Searches for a given filename for external documents
        /// </summary>
        /// <param name="DocumentName"></param>
        /// <param name="error"></param>
        /// <param name="count"></param>
        /// <param name="objectIds"></param>
        /// <returns></returns>
        internal static bool DocumentFound(SDBeesExternalDocument doc, ref Error error, ref int count, ref ArrayList objectIds)
        {
            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(ConnectivityManagerDocumentBaseData.gTable.Columns[m_DocumentFileColumnName], doc.DocOriginalName);
            string criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

            count = ConnectivityManager.Current.MyDBManager.Database.Select(ConnectivityManagerDocumentBaseData.gTable, ConnectivityManagerDocumentBaseData.gTable.PrimaryKey, criteria, ref objectIds, ref error);
            if (count > 0)
                return true;
            else
                return false;
        }

        public static bool DocumentFound(string sdbeesid, ref Error error, ref int count, ref ArrayList objectIds)
        {
            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(ConnectivityManagerDocumentBaseData.gTable.Columns[m_IdSDBeesColumnName], sdbeesid);
            string criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

            count = ConnectivityManager.Current.MyDBManager.Database.Select(ConnectivityManagerDocumentBaseData.gTable, ConnectivityManagerDocumentBaseData.gTable.PrimaryKey, criteria, ref objectIds, ref error);
            if (count > 0)
                return true;
            else
                return false;
        }


        internal static SDBeesExternalDocument DocumentGet(SDBeesExternalDocument doc, ref Error _error)
        {
            SDBeesExternalDocument _doc = null;

            Error error = null;
            int count = -1;
            ArrayList objectIds = null;

            if (DocumentFound(doc, ref error, ref count, ref objectIds))
            {
                Plugs.TemplateBase.TemplateDBBaseData idObj = ConnectivityManager.Current.CreateDataObject();

                if (idObj.Load(ConnectivityManager.Current.MyDBManager.Database, objectIds[0], ref error))
                {
                    _doc = new SDBeesExternalDocument();

                    _doc.DocumentId = new SDBeesDocumentId(idObj.GetPropertyByColumn(m_IdSDBeesColumnName).ToString());
                    _doc.ProjectId = ConnectivityManager.Current.ProjectGetCurrentId().ToString();
                }
            }

            return _doc;
        }

        internal static SDBeesExternalDocument DocumentAdd(SDBeesExternalDocument doc, string pluginId, string roleId, ref Error _error)
        {
            SDBeesExternalDocument _doc = null;

            Plugs.TemplateBase.TemplateDBBaseData baseData = ConnectivityManager.Current.CreateDataObject();
            baseData.SetDefaults(ConnectivityManager.Current.MyDBManager.Database);

            baseData.SetPropertyByColumn(m_DocumentFileColumnName, doc.DocOriginalName);
            baseData.SetPropertyByColumn(m_ApplicationColumnName, pluginId);
            baseData.SetPropertyByColumn(m_RoleIdColumnName, roleId);

            if (DocumentGetRootAndType(ref baseData, false))
            {
                _doc = new SDBeesExternalDocument();
                baseData.Save(ConnectivityManager.Current.MyDBManager.Database, ref _error);

                _doc.DocumentId = new SDBeesDocumentId() { Id = baseData.GetPropertyByColumn(m_IdSDBeesColumnName).ToString() };
                _doc.ProjectId = ConnectivityManager.Current.ProjectGetCurrentId().ToString();
            }

            return _doc;
        }

        internal static string GetDocumentApplicationBySDBeesId(string docid, ref Error _error)
        {
            string result;
            Plugs.TemplateBase.TemplateDBBaseData dt = GetDocumentData(docid, ref _error);
            if (dt != null)
            {
                result = dt.GetPropertyByColumn(m_ApplicationColumnName).ToString();
            }
            else
            {
                result = null;
            }

            return result;
        }

        internal static string GetDocumentDbIdBySDBeesId(string docid, ref Error _error)
        {
            string result;
            Plugs.TemplateBase.TemplateDBBaseData dt = GetDocumentData(docid, ref _error);
            if (dt != null)
            {
                result = dt.GetPropertyByColumn(m_IdColumnName).ToString();
            }
            else
            {
                result = null;
            }

            return result;
        }


        #region Public Methods
        public const string m_DocumentFileColumnName = "docfilename";
        public const string m_DocumentFileDisplayName = "File name";
        public const string m_DocumentAssignmentColumnName = "file_assignment";
        public const string m_DocumentAssignmentDisplayName = "file assignment";
        public const string m_ApplicationColumnName = "application";
        public const string m_ApplicationDisplayName = "File application";
        public const string m_RoleIdColumnName = "roleid";
        public const string m_RoleIdDisplayName = "File role id";
        public const string m_DocumentRootColumnName = "document_root";
        public const string m_DocumentRootDisplayName = "File root object id";
        public const string m_DocumentRootTypeColumnName = "document_root_type";
        public const string m_DocumentRootTypeDisplayName = "File root object type";
        public const string m_DocumentCADInfoColumnName = "CAD_info";
        public const string m_DocumentCADInfoDisplayName = "CAD info from external file";

        public override void InitTableSchema(ref Table table, Database database)
        {
            base.InitTableSchema(ref table, database);
            //required columns
            this.AddColumn(new Column(m_DocumentFileColumnName, DbType.eString, m_DocumentFileDisplayName, "The filename of the external document", "ProjectInfo", 250, "", 0) { Editable = false }, database);

            Column appCol = new Column(m_DocumentAssignmentColumnName, DbType.eString, m_DocumentAssignmentDisplayName, "The assignment for this document", "ProjectInfo", 250, "", 0);
            appCol.Flags |= (int)DbFlags.eAllowNull;
            this.AddColumn(appCol, database);

            this.AddColumn(new Column(m_ApplicationColumnName, DbType.eString, m_ApplicationDisplayName, "The application that created the external document", "ProjectInfo", 250, "", 0) { Editable = false, Browsable = false }, database);
            this.AddColumn(new Column(m_RoleIdColumnName, DbType.eString, m_RoleIdDisplayName, "The role id for the external document", "ProjectInfo", 250, "", 0) { Browsable = false, Editable = false }, database);
            this.AddColumn(new Column(m_DocumentRootColumnName, DbType.eString, m_DocumentRootDisplayName, "The root id for the external document", "ProjectInfo", 250, "", 0) { Editable = false, Browsable = false }, database);
            this.AddColumn(new Column(m_DocumentRootTypeColumnName, DbType.eString, m_DocumentRootTypeDisplayName, "The root type for the external document", "ProjectInfo", 250, "", 0) { Editable = false, Browsable = false }, database);
            this.AddColumn(new Column(m_DocumentCADInfoColumnName, DbType.eString, m_DocumentCADInfoDisplayName, "The units and coordinates used in the bim software", "ProjectInfo", 2000, "", 0) { Flags = (int)DbFlags.eAllowNull, Editable = false, Browsable = false }, database);
        }
        #endregion

        internal static bool GetDocumentsByAssignedSubContractor(string p, ref Error _error, ref ArrayList _objDocIds)
        {
            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_DocumentAssignmentColumnName], p);
            string criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error);
            int count = 0;

            count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref _objDocIds, ref _error);
            if (count > 0)
                return true;
            else
                return false;
        }
    }

    public class ConnectivityManagerAlienBaseData : SDBees.Plugs.TemplateBase.TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable = null;

        #endregion

        #region Public Properties
        public override string GetTableName
        {
            get { return "sdbConnectivityManagerAlien"; }
        }

        #endregion

        #region Constructor/Destructor

        public ConnectivityManagerAlienBaseData() :
            base("ConnectivityManager", "ExternalConnectionIds", "General")
        {
            base.Table = gTable;
        }

        #endregion

        //public static bool GetAlienIdsByRootId(SDBeesExternalDocument doc, ref Error _error, ref ArrayList _existingObjects)
        //{
        //    bool result = false;

        //    Plugs.TemplateBase.TemplateDBBaseData data = ConnectivityManagerDocumentBaseData.GetDocumentData(doc.DocumentId.Id, ref _error);
        //    int _count = 0;

        //    SDBees.DB.Attribute attDocRoot = new SDBees.DB.Attribute(gTable.Columns["aliendocumentid"], data.GetPropertyByColumn("id"));
        //    string criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attDocRoot, DbBinaryOperator.eIsEqual, ref _error);

        //    _count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref _existingObjects, ref _error);
        //    if (_count > 0)
        //    {
        //        result = true;
        //    }
        //    return result;
        //}

        public static bool GetAlienIds(ref Error _error, ref ArrayList objectIds)
        {
            int count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, ref objectIds, ref _error);

            return (0 < count);
        }

        public static bool GetAlienIdsByDocumentSDBeesId(string idSDBeesDoc, ref Error _error, ref ArrayList objectIds)
        {
            int count = 0;
            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_AlienDocumentIdColumnName], idSDBeesDoc);
            string criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error);

            count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
            if (count > 0)
                return true;
            else
                return false;
        }

        public static bool GetAlienIdsByDbId(string idDb, ref Error _error, ref ArrayList objectIds)
        {
            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_AlienInternalDBElementIdColumnName], idDb);
            string criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error);
            int count = 0;

            count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
            if (count > 0)
                return true;
            else
                return false;
        }

        public static bool AlienIdFound(string idAlien, string idDoc, ref Error _error, ref ArrayList objectIds)
        {
            ArrayList criterias = new ArrayList();

            //SDBees.DB.Attribute attAlienId = new SDBees.DB.Attribute(ConnectivityManagerAlienBaseData.gTable.Columns[ConnectivityManagerAlienBaseData.m_AlienIdColumnName], idAlien);
            string criteria1 = string.Format("{0} = '{1}'", m_AlienIdColumnName, idAlien);
            criterias.Add(criteria1);

            //SDBees.DB.Attribute attDoc = new SDBees.DB.Attribute(ConnectivityManagerAlienBaseData.gTable.Columns[ConnectivityManagerAlienBaseData.m_AlienDocumentIdColumnName], idDoc);
            string criteria2 = string.Format("{0} = '{1}'", m_AlienDocumentIdColumnName, idDoc);
            criterias.Add(criteria2);

            string criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref _error);
            int count = 0;

            count = Select(ConnectivityManager.Current.MyDBManager.Database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
            if (count > 0)
                return true;
            else
                return false;
        }

        public static void AlienIdsAdd(string typeName, string internalId, HashSet<SDBeesAlienId> items, ref Error _error)
        {
            foreach (SDBeesAlienId idAlien in items)
            {
                ArrayList obj = null;
                if (!AlienIdFound(idAlien.AlienInstanceId.ToString(), idAlien.DocumentId.Id, ref _error, ref obj))
                {
                    Plugs.TemplateBase.TemplateDBBaseData alienidObj = ConnectivityManager.Current.CreateDataObjectAlienIds();

                    alienidObj.SetDefaults(ConnectivityManager.Current.MyDBManager.Database);

                    alienidObj.SetPropertyByColumn(m_AlienIdColumnName, idAlien.AlienInstanceId.ToString());
                    alienidObj.SetPropertyByColumn(m_AlienDocumentIdColumnName, idAlien.DocumentId.Id);
                    alienidObj.SetPropertyByColumn(m_AlienInternalDBElementIdColumnName, internalId);
                    alienidObj.SetPropertyByColumn(m_AlienInternalDBElementTypeColumnName, typeName);
                    alienidObj.SetPropertyByColumn(m_AlienApplicationColumnName, ConnectivityManagerDocumentBaseData.GetDocumentApplicationBySDBeesId(idAlien.DocumentId.ToString(), ref _error));

                    alienidObj.Save(ConnectivityManager.Current.MyDBManager.Database, ref _error);  //TODO : TH - Hier bekommen wir noch ein Problem mit der DB, wenn gleiche Handles ankommen in AlienIds...
                }
            }
        }

        public static Dictionary<object, List<object>> GetObjectId2AlienIdsMap(Connection connection, ref Error _error)
        {
            Dictionary<object, List<object>> result = new Dictionary<object, List<object>>();

            System.Data.DataTable dataTable = connection.GetReadOnlyDataTable(gTable.Name);

            foreach (System.Data.DataRow dataRow in dataTable.Rows)
            {
                object alienId = dataRow[m_IdColumnName];

                object objectId = dataRow[m_AlienInternalDBElementIdColumnName];

                List<object> alienIds = null;

                if (result.TryGetValue(objectId, out alienIds))
                {
                    if (!alienIds.Contains(alienId))
                    {
                        alienIds.Add(alienId);
                    }
                }
                else
                {
                    alienIds = new List<object>();

                    alienIds.Add(alienId);

                    result.Add(objectId, alienIds);
                }
            }

            return result;
        }

        #region Public Methods

        /// <summary>
        /// The id in the external BIM software: Handle in AutoCAD, Id in Revit
        /// </summary>
        public const string m_AlienIdColumnName = "alienid"; //The id in the external BIM software: Handle in AutoCAD, Id in Revit
        public const string m_AlienIdDisplayName = "alien id";
        /// <summary>
        /// The relation to the document stored in db. It is the SDBeesId of the document record!!
        /// </summary>
        public const string m_AlienDocumentIdColumnName = "aliendocumentid"; //The relation to the document stored in db. It is the SDBeesId of the document record!!
        public const string m_AlienDocumentIdDisplayName = "alien document id";
        public const string m_AlienApplicationColumnName = "alienapp"; // The alien application
        public const string m_AlienInternalDBElementIdColumnName = "db_id";
        public const string m_AlienInternalDBElementTypeColumnName = "db_element_type";

        public override void InitTableSchema(ref Table table, Database database)
        {
            base.InitTableSchema(ref table, database);
            //required columns
            this.AddColumn(new Column(m_AlienIdColumnName, DbType.eString, m_AlienIdDisplayName, "The alien id for the externally created element, handle or guid", "Alien", 250, "0000", 0), database);
            this.AddColumn(new Column(m_AlienDocumentIdColumnName, DbType.eString, m_AlienDocumentIdDisplayName, "The aliendocument for the externally created element", "Alien", 250, "0000", 0), database);
            this.AddColumn(new Column(m_AlienApplicationColumnName, DbType.eString, "alien application", "The alien creator app for the externally created element", "Alien", 250, "0000", 0) { Flags = (int)DbFlags.eAllowNull }, database);
            this.AddColumn(new Column(m_AlienInternalDBElementIdColumnName, DbType.eString, "internal db id", "The internal id for the externally created element", "Alien", 250, "0000", 0), database);
            this.AddColumn(new Column(m_AlienInternalDBElementTypeColumnName, DbType.eString, "internal db element type", "The internal element type for the externally created element", "Alien", 250, "0000", 0), database);
        }
        #endregion

        private static int Select(Database database, Table table, string searchColumn, string criteria, ref ArrayList objectIds, ref Error error)
        {
            return database.Select(table, searchColumn, criteria, ref objectIds, ref error);
        }
    }
}
