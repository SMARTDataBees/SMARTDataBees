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
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Admin;
using SDBees.Core.Connectivity.SDBeesLink;
using SDBees.Core.Connectivity.SDBeesLink.Service;
using SDBees.Core.Global;
using SDBees.Core.Main.Systemtray;
using SDBees.Core.Model;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateBase;
using SDBees.Plugs.TemplateTreeNode;
using Attribute = SDBees.DB.Attribute;
using DbType = SDBees.DB.DbType;
using Object = SDBees.DB.Object;

namespace SDBees.Core.Connectivity
{
    [PluginName("ConnectivityManager Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the connectivity to SmartDataBees")]
    [PluginId("519A4B0B-0C76-4829-9D09-28C9E1795745")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBeesDBConnection))]
    [PluginDependency(typeof(MainWindowApplication))]
    [PluginDependency(typeof(ProcessIcon))]
    [PluginDependency(typeof(GlobalManager))]
    public class ConnectivityManager : TemplatePlugin
    {
        private static ConnectivityManager _theInstance;
        private PluginContext m_context;
        private ViewAdmin m_viewAdmin;
        private ProcessIcon _processIcon;
        private bool m_ready;

        /// <summary>
        /// Returns the one and only UserAdmin Plugin instance.
        /// </summary>
        public static ConnectivityManager Current
        {
            get
            {
                return _theInstance;
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
        {
            _theInstance = this;
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
                if (context.PluginDescriptors.Contains(new PluginDescriptor(typeof(ViewAdmin))))
                {
                    m_viewAdmin = (ViewAdmin)context.PluginDescriptors[typeof(ViewAdmin)].PluginInstance;
                }

                //Das Viewadmin Plugin besorgen
                if (context.PluginDescriptors.Contains(new PluginDescriptor(typeof(ProcessIcon))))
                {
                    _processIcon = (ProcessIcon)context.PluginDescriptors[typeof(ProcessIcon)].PluginInstance;
                }

                //Open the WCF Host
                ConnectivityHost.Instance().Open();
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

        public override TemplateDBBaseData CreateDataObject()
        {
            return new ConnectivityManagerDocumentBaseData();
        }

        public TemplateDBBaseData CreateDataObjectAlienIds()
        {
            return new ConnectivityManagerAlienBaseData();
        }

        public override TemplatePlugin GetPlugin() => _theInstance;

        static Hashtable m_connectedClients = new Hashtable();
        public Hashtable ConnectedClients
        {
            get { return m_connectedClients; }
            set { m_connectedClients = value; }
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
            var result = false;

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

        internal SDBeesExternalDocument DocumentGet(SDBeesExternalDocument document)
        {
            Error error = null;

            var doc = ConnectivityManagerDocumentBaseData.DocumentGet(document, ref error);
            Error.Display("Document get errors", error);
            return doc;
        }

        internal SDBeesExternalDocument DocumentRegister(SDBeesExternalDocument doc, string pluginId, string roleId)
        {
            Error _error = null;
            var count = -1;
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
            var result = false;

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
            var result = false;

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
            var m_Ents = new Dictionary<string, SDBeesEntityDefinition>();

            foreach (var nod in TemplateTreenode.GetAllTreenodePlugins())
            {
                var entdef = nod.GetEntityDefinition();
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

            var result = SDBeesDataSet.CreateExportDataset(doc);

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

            var progressTool = new ProgressTool();
            progressTool.StartActiveProcess(true, true);
            progressTool.WriteStatus("Entities will be added to db ...");

            progressTool.ProgressBar.Maximum = data.Entities.Count;
            progressTool.ProgressBar.Value = 0;

            //TBD : Check for valid document id
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

            var connection = MyDBManager.Database.Open(false, ref _error);

            var alienIds = new ArrayList();

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.GetAlienIds");
#endif

            ConnectivityManagerAlienBaseData.GetAlienIds(ref _error, ref alienIds);

            var objectId2AlienIdsMap = ConnectivityManagerAlienBaseData.GetObjectId2AlienIdsMap(connection, ref _error);

#if PROFILER
            SDBees.Profiler.Stop();

            SDBees.Profiler.Start("ConnectivityManager.SynchronizeServer.Entities");
#endif

            var _newObjects = new Hashtable();

            try
            {
                var _lstDocs = new ArrayList();
                foreach (var caddoc in data.Documents)
                {
                    var count = 0;
                    if (ConnectivityManagerDocumentBaseData.DocumentFound(caddoc.Id.ToString(), ref _error, ref count, ref _lstDocs))
                    {
                        foreach (var idDocs in _lstDocs)
                        {
                            var docData = new ConnectivityManagerDocumentBaseData();
                            if (docData.Load(MyDBManager.Database, idDocs, ref _error))
                            {
                                docData.SetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentCADInfoColumnName, SDBeesDocumentCADInfo.Serialize(caddoc.CADInfo));
                                docData.Save(MyDBManager.Database, ref _error);
                            }
                        }
                    }
                }

                foreach (var elem in data.Entities)
                {
                    foreach (var alienId in elem.AlienIds)
                    {
                        if (ConnectivityManagerAlienBaseData.AlienIdFound(alienId.AlienInstanceId.ToString(), alienId.DocumentId.Id, ref _error, ref _objectIds))
                        {
                            // Load the alienid record
                            var idAlien = new ConnectivityManagerAlienBaseData();
                            if (idAlien.Load(MyDBManager.Database, _objectIds[0], ref _error))
                            {
                                //TBD Update for already existing db objects
                                //Currently only existing entities esp their parameters are updated
                                //No alienids and relations will be added...
                                var plug = TemplateTreenode.GetPluginForType(elem.DefinitionId.ToString());
                                if (plug != null)
                                {
                                    var dbObject = plug.CreateDataObject();
                                    var dbOjectId = idAlien.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName);
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
                                                    foreach (var objectAlienId in objectAlienIds)
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
                            var plug = TemplateTreenode.GetPluginForType(elem.DefinitionId.Id);
                            if (plug != null)
                            {
                                if (!string.IsNullOrEmpty(elem.Id.ToString()))
                                {
                                    //TBD : Object exists in db, we have to add a new alien id
                                    var dataObj = plug.CreateDataObject();
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

        private Error CreateEntityAndAddAlienIds(Error _error, Hashtable _newObjects, SDBeesEntity elem, SDBeesAlienId alienId, TemplateTreenode plug)
        {
            try
            {
                var dataObj = plug.CreateDataObject();
                dataObj.SetDefaults(Current.MyDBManager.Database);

                SetObjectData(ref _error, elem, dataObj);

                // Add to table for alien id handling
                ConnectivityManagerAlienBaseData.AlienIdsAdd(plug.GetType().ToString(), dataObj.Id.ToString(), elem.AlienIds, ref _error);

                // Add to internal collection for relationship handling
                var kvpair = new KeyValuePair<TemplateDBBaseData, SDBeesEntity>(dataObj, elem);
                _newObjects.Add(alienId.AlienInstanceId.ToString(), kvpair);
            }
            catch (Exception ex)
            {
            }

            return _error;
        }

        private void DeleteUntouchedObjects(ArrayList objectIds, Dictionary<object, List<object>> objectId2AlienIdsMap, SDBeesSyncMode mode, ref Error error)
        {
            var map = new Dictionary<TemplateTreenode, List<TemplateDBBaseData>>();

            foreach (var objectId in objectIds)
            {
                var idAlien = new ConnectivityManagerAlienBaseData();
                if (idAlien.Load(MyDBManager.Database, objectId, ref error))
                {
                    var type = idAlien.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementTypeColumnName);
                    var plug = TemplateTreenode.GetPluginForType(type.ToString());
                    if (plug != null)
                    {
                        var dbObject = plug.CreateDataObject();
                        if (dbObject.Load(MyDBManager.Database, idAlien.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName), ref error))
                        {
                            if (map.ContainsKey(plug))
                            {
                                var objects = map[plug];
                                objects.Add(dbObject);
                            }
                            else
                            {
                                var objects = new List<TemplateDBBaseData>();
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

            foreach (var keyValue in map)
            {
                if (mode == SDBeesSyncMode.UpdateServer)
                {
                    keyValue.Key.DeleteInstancesAndRelations(keyValue.Value, ref error);
                }
                else if (mode == SDBeesSyncMode.UpdateServerValidation)
                {
			        foreach (var item in keyValue.Value)
			        {
                        var numberOfAlienIds = (objectId2AlienIdsMap != null) ? objectId2AlienIdsMap[item.Id].Count :  0;

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
            //TBD : Check for valid document id
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
            if (_processIcon != null)
            {
                _processIcon.OpenMainDialog();
            }
        }

        internal void UpdateServerDialog()
        {
            Update();
        }

        // The delegate procedure we are assigning to our object
        public delegate void SynchronizeHandler(object myObject, SyncArgs myArgs);

        public event SynchronizeHandler OnSyncronizationWithServerEnded;

        public static void SetObjectData(ref Error _error, SDBeesEntity elem, TemplateDBBaseData dataObj)
        {
            if (!string.IsNullOrEmpty(elem.InstanceId.Id))
                dataObj.SetPropertyByColumn(Object.m_IdSDBeesColumnName, elem.InstanceId.Id);

            foreach (var prp in elem.Properties)
            {
                foreach (var col in dataObj.Table.Columns)
                {
                    if (col.Name == prp.DefinitionId)
                    {
                        dataObj.SetPropertyByColumn(col.Name, prp.InstanceValue.ObjectValue);
                        break;
                    }
                }
            }

            dataObj.Save(Current.MyDBManager.Database, ref _error);

            //elem.Id.Id = dataObj.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString();
            //elem.InstanceId.Id = dataObj.GetPropertyByColumn(SDBees.DB.Object.m_IdSDBeesColumnName).ToString();
        }

        private void AddRelationsToChild(Hashtable _newObjects, SDBeesDataSet data, ref Error _error)
        {
#if PROFILER
            SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild");
#endif

            var progressTool = new ProgressTool();
            progressTool.StartActiveProcess(true, true);

            progressTool.WriteStatus("Relations to childs will be added ...");

            progressTool.ProgressBar.Maximum = data.Relations.Count;
            progressTool.ProgressBar.Value = 0;

            foreach (var relation in data.Relations)
            {
#if PROFILER
                SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild.1");
#endif

                if ((relation.AlienSourceId != null) && !string.IsNullOrEmpty(relation.AlienSourceId.AlienInstanceId.ToString()))
                {
                    if (_newObjects.ContainsKey(relation.AlienSourceId.AlienInstanceId.ToString()))
                    {
                        var item = (KeyValuePair<TemplateDBBaseData, SDBeesEntity>)_newObjects[relation.AlienSourceId.AlienInstanceId.ToString()];

                        ArrayList lstTarget = null;

#if PROFILER
                        SDBees.Profiler.Start("ConnectivityManagerAlienBaseData.AlienIdFound");
#endif
                        var ok1 = ConnectivityManagerAlienBaseData.AlienIdFound(relation.AlienTargetId.AlienInstanceId.ToString(), relation.AlienTargetId.DocumentId.Id, ref _error, ref lstTarget);

#if PROFILER
                        SDBees.Profiler.Stop();
#endif

                        if (ok1)
                        {
#if PROFILER
                            SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild.2");
#endif

                            var plug = CreateDataObjectAlienIds();
                            if (plug.Load(MyDBManager.Database, lstTarget[0], ref _error))
                            {
                                var rel = new ViewRelation();
                                rel.SetDefaults(MyDBManager.Database);
                                rel.ParentId = Guid.Parse(item.Key.Id.ToString());
                                rel.ParentType = GetNodeType(item.Value);
                                rel.ChildId = Guid.Parse(plug.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName).ToString());
                                rel.ChildType = plug.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementTypeColumnName).ToString();
                                rel.ChildName = GetChildName(rel.ChildId, rel.ChildType, ref _error);

#if PROFILER
                                SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild.3");
#endif
                                ArrayList objIds = null;
                                var ok2 = ViewRelation.FindViewRelationByParentIdAndChildId(MyDBManager.Database, rel.ParentId, rel.ChildId, ref objIds, ref _error);

#if PROFILER
                                SDBees.Profiler.Stop();
#endif

                                if (!ok2)
                                {
#if PROFILER
                                    SDBees.Profiler.Start("ConnectivityManager.RelationsAddChild.4");
#endif

                                    rel.Save(MyDBManager.Database, ref _error);

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
            var temp = "Temp";
            foreach (var plug in TemplateTreenode.GetAllTreenodePlugins())
            {
                if (plug.GetType().ToString() == p)
                {
                    var data = plug.CreateDataObject();
                    data.Load(MyDBManager.Database, guid, ref _error);
                    temp = data.GetPropertyByColumn(TemplateDBBaseData.m_NameColumnName).ToString();
                }
            }

            return temp;
        }

        private void AddRelationsToRoot(SDBeesExternalDocument doc, Hashtable _newObjects, ref Error _error)
        {
            var _rootDoc = ConnectivityManagerDocumentBaseData.GetDocumentData(doc.DocumentId.Id, ref _error);
            if (_rootDoc != null)
            {
                var progressTool = new ProgressTool();
                progressTool.StartActiveProcess(true, true);

                progressTool.WriteStatus("Root relations will be created ...");

                progressTool.ProgressBar.Maximum = _newObjects.Values.Count;
                progressTool.ProgressBar.Value = 0;

                foreach (KeyValuePair<TemplateDBBaseData, SDBeesEntity> item in _newObjects.Values)
                {
                    //check if relation already exists
                    ArrayList _oIdsFound = null;
                    if (ViewRelation.FindViewRelationByChildId(MyDBManager.Database, Guid.Parse(item.Key.Id.ToString()), ref _oIdsFound, ref _error) > 0)
                    {
                        var idfound = false;
                        // we have alreads relations for this child, check if same root is already in place
                        ArrayList _idRoot = null;
                        if (ViewRelation.FindViewRelationByParentId(MyDBManager.Database, Guid.Parse(_rootDoc.GetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName).ToString()), ref _idRoot, ref _error) > 0)
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

                    var rel = new ViewRelation();
                    rel.SetDefaults(MyDBManager.Database);
                    rel.ParentId = Guid.Parse(_rootDoc.GetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName).ToString());
                    rel.ParentType = _rootDoc.GetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentRootTypeColumnName).ToString();
                    rel.ChildId = Guid.Parse(item.Key.Id.ToString());
                    rel.ChildName = item.Key.GetPropertyByColumn(TemplateDBBaseData.m_NameColumnName).ToString();
                    rel.ChildType = GetNodeType(item.Value);

                    rel.Save(MyDBManager.Database, ref _error);

                    progressTool.ProgressBar.Value++;
                }
                progressTool.EndActiveProcess();
            }
        }

        private string GetNodeType(SDBeesEntity sDBeesEntity)
        {
            string m_childType = null;
            foreach (var plug in TemplateTreenode.GetAllTreenodePlugins())
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
                CreateDataObjectAlienIds().InitTableSchema(ref ConnectivityManagerAlienBaseData.gTable, MyDBManager.Database);
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
            m_message = message;
        }

        public string Message
        {
            get
            {
                return m_message;
            }
        }
    }

    public class ConnectivityManagerDocumentBaseData : TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable;

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
            Table = gTable;
        }

        #endregion

        internal static bool DocumentGetRootAndType(ref TemplateDBBaseData idObj, bool interactive)
        {
            if (interactive)
            {
                return GetDocumentRootAndTypeInteractive(ref idObj);
            }
            return GetDocumentRootAndTypeNonInteractive(ref idObj);
        }

        private static bool GetDocumentRootAndTypeNonInteractive(ref TemplateDBBaseData idObj)
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

        private static bool GetDocumentRootAndTypeInteractive(ref TemplateDBBaseData rootDocData)
        {
            if (ViewAdmin.Current.MyViewRelationWindow().ShowDialog() == DialogResult.OK)
            {
                rootDocData.SetPropertyByColumn(m_DocumentRootColumnName, ViewAdmin.Current.MyViewRelationWindow().TagSelected.NodeGUID);
                rootDocData.SetPropertyByColumn(m_DocumentRootTypeColumnName, ViewAdmin.Current.MyViewRelationWindow().TagSelected.NodeTypeOf);
                return true;
            }
            return false;
        }

        internal static TemplateDBBaseData GetDocumentData(string docid, ref Error _error)
        {
            var column = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_IdSDBeesColumnName));
            var attDocRoot = new Attribute(column, docid);
            var criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attDocRoot, DbBinaryOperator.eIsEqual, ref _error);

            ArrayList objectIds = null;
            var count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
            if (count > 0)
            {
                var _root = ConnectivityManager.Current.CreateDataObject();
                _root.Load(ConnectivityManager.Current.MyDBManager.Database, objectIds[0], ref _error);
                return _root;
            }
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
            var column = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_DocumentAssignmentColumnName));
            var attParent = new Attribute(column, doc.DocOriginalName);
            var criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

            count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
            if (count > 0)
                return true;
            return false;
        }

        public static bool DocumentFound(string sdbeesid, ref Error error, ref int count, ref ArrayList objectIds)
        {
            var column = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_DocumentAssignmentColumnName));
            var attParent = new Attribute(column, sdbeesid);
            var criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

            count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
            if (count > 0)
                return true;
            return false;
        }


        internal static SDBeesExternalDocument DocumentGet(SDBeesExternalDocument doc, ref Error _error)
        {
            SDBeesExternalDocument _doc = null;

            Error error = null;
            var count = -1;
            ArrayList objectIds = null;

            if (DocumentFound(doc, ref error, ref count, ref objectIds))
            {
                var idObj = ConnectivityManager.Current.CreateDataObject();

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

            var baseData = ConnectivityManager.Current.CreateDataObject();
            baseData.SetDefaults(ConnectivityManager.Current.MyDBManager.Database);

            baseData.SetPropertyByColumn(m_DocumentFileColumnName, doc.DocOriginalName);
            baseData.SetPropertyByColumn(m_ApplicationColumnName, pluginId);
            baseData.SetPropertyByColumn(m_RoleIdColumnName, roleId);

            if (DocumentGetRootAndType(ref baseData, false))
            {
                _doc = new SDBeesExternalDocument();
                baseData.Save(ConnectivityManager.Current.MyDBManager.Database, ref _error);

                _doc.DocumentId = new SDBeesDocumentId { Id = baseData.GetPropertyByColumn(m_IdSDBeesColumnName).ToString() };
                _doc.ProjectId = ConnectivityManager.Current.ProjectGetCurrentId().ToString();
            }

            return _doc;
        }

        internal static string GetDocumentApplicationBySDBeesId(string docid, ref Error _error)
        {
            string result;
            var dt = GetDocumentData(docid, ref _error);
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
            var dt = GetDocumentData(docid, ref _error);
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
            AddColumn(new Column(m_DocumentFileColumnName, DbType.String, m_DocumentFileDisplayName, "The filename of the external document", "ProjectInfo", 250, "", 0) { IsEditable = false }, database);

            var appCol = new Column(m_DocumentAssignmentColumnName, DbType.String, m_DocumentAssignmentDisplayName, "The assignment for this document", "ProjectInfo", 250, "", 0);
            appCol.Flags |= (int)DbFlags.eAllowNull;
            AddColumn(appCol, database);

            AddColumn(new Column(m_ApplicationColumnName, DbType.String, m_ApplicationDisplayName, "The application that created the external document", "ProjectInfo", 250, "", 0) { IsEditable = false, IsBrowsable = false }, database);
            AddColumn(new Column(m_RoleIdColumnName, DbType.String, m_RoleIdDisplayName, "The role id for the external document", "ProjectInfo", 250, "", 0) { IsBrowsable = false, IsEditable = false }, database);
            AddColumn(new Column(m_DocumentRootColumnName, DbType.String, m_DocumentRootDisplayName, "The root id for the external document", "ProjectInfo", 250, "", 0) { IsEditable = false, IsBrowsable = false }, database);
            AddColumn(new Column(m_DocumentRootTypeColumnName, DbType.String, m_DocumentRootTypeDisplayName, "The root type for the external document", "ProjectInfo", 250, "", 0) { IsEditable = false, IsBrowsable = false }, database);
            AddColumn(new Column(m_DocumentCADInfoColumnName, DbType.String, m_DocumentCADInfoDisplayName, "The units and coordinates used in the bim software", "ProjectInfo", 2000, "", 0) { Flags = (int)DbFlags.eAllowNull, IsEditable = false, IsBrowsable = false }, database);
        }
        #endregion

        internal static bool GetDocumentsByAssignedSubContractor(string p, ref Error _error, ref ArrayList _objDocIds)
        {
            var column = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_DocumentAssignmentColumnName));
            var attParent = new Attribute(column, p);
            var criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error);
            var count = 0;

            count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref _objDocIds, ref _error);
            if (count > 0)
                return true;
            return false;
        }
    }

    public class ConnectivityManagerAlienBaseData : TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable;

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
            Table = gTable;
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
            var count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, ref objectIds, ref _error);

            return (0 < count);
        }

        public static bool GetAlienIdsByDocumentSDBeesId(string idSDBeesDoc, ref Error _error, ref ArrayList objectIds)
        {
            var count = 0;
            var column = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_AlienDocumentIdColumnName));
            var attParent = new Attribute(column, idSDBeesDoc);
            var criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error);

            count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
            if (count > 0)
                return true;
            return false;
        }

        public static bool GetAlienIdsByDbId(string idDb, ref Error _error, ref ArrayList objectIds)
        {
            var column = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_AlienInternalDBElementIdColumnName));
            var attParent = new Attribute(column, idDb);
            var criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error);
            var count = 0;

            count = ConnectivityManager.Current.MyDBManager.Database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
            if (count > 0)
                return true;
            return false;
        }

        public static bool AlienIdFound(string idAlien, string idDoc, ref Error _error, ref ArrayList objectIds)
        {
            var criterias = new ArrayList();

            //SDBees.DB.Attribute attAlienId = new SDBees.DB.Attribute(ConnectivityManagerAlienBaseData.gTable.Columns[ConnectivityManagerAlienBaseData.m_AlienIdColumnName], idAlien);
            var criteria1 = string.Format("{0} = '{1}'", m_AlienIdColumnName, idAlien);
            criterias.Add(criteria1);

            //SDBees.DB.Attribute attDoc = new SDBees.DB.Attribute(ConnectivityManagerAlienBaseData.gTable.Columns[ConnectivityManagerAlienBaseData.m_AlienDocumentIdColumnName], idDoc);
            var criteria2 = string.Format("{0} = '{1}'", m_AlienDocumentIdColumnName, idDoc);
            criterias.Add(criteria2);

            var criteria = ConnectivityManager.Current.MyDBManager.Database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref _error);
            var count = 0;

            count = Select(ConnectivityManager.Current.MyDBManager.Database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
            if (count > 0)
                return true;
            return false;
        }

        public static void AlienIdsAdd(string typeName, string internalId, HashSet<SDBeesAlienId> items, ref Error _error)
        {
            foreach (var idAlien in items)
            {
                ArrayList obj = null;
                if (!AlienIdFound(idAlien.AlienInstanceId.ToString(), idAlien.DocumentId.Id, ref _error, ref obj))
                {
                    var alienidObj = ConnectivityManager.Current.CreateDataObjectAlienIds();

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
            var result = new Dictionary<object, List<object>>();

            var dataTable = connection.GetReadOnlyDataTable(gTable.Name);

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var alienId = dataRow[m_IdColumnName];

                var objectId = dataRow[m_AlienInternalDBElementIdColumnName];

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
            AddColumn(new Column(m_AlienIdColumnName, DbType.String, m_AlienIdDisplayName, "The alien id for the externally created element, handle or guid", "Alien", 250, "0000", 0), database);
            AddColumn(new Column(m_AlienDocumentIdColumnName, DbType.String, m_AlienDocumentIdDisplayName, "The aliendocument for the externally created element", "Alien", 250, "0000", 0), database);
            AddColumn(new Column(m_AlienApplicationColumnName, DbType.String, "alien application", "The alien creator app for the externally created element", "Alien", 250, "0000", 0) { Flags = (int)DbFlags.eAllowNull }, database);
            AddColumn(new Column(m_AlienInternalDBElementIdColumnName, DbType.String, "internal db id", "The internal id for the externally created element", "Alien", 250, "0000", 0), database);
            AddColumn(new Column(m_AlienInternalDBElementTypeColumnName, DbType.String, "internal db element type", "The internal element type for the externally created element", "Alien", 250, "0000", 0), database);
        }
        #endregion

        private static int Select(Database database, Table table, string searchColumn, string criteria, ref ArrayList objectIds, ref Error error)
        {
            return database.Select(table, searchColumn, criteria, ref objectIds, ref error);
        }
    }
}
