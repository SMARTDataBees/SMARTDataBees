﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;
using System.Xml;
using SDBees.Core.Connectivity;
using SDBees.DB;
using SDBees.Plugs.Properties;
using SDBees.Plugs.TemplateBase;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Utils;
using SDBees.ViewAdmin;
using Object = SDBees.DB.Object;

namespace SDBees.Core.Model.Basic
{
    [DataContract(Name = "sdbeessetbase", Namespace = "http://www.smartdatabees.de")]
    [KnownType(typeof(SDBeesOpeningSize))]
    public class SDBeesSetBase
    {
        public SDBeesSetBase()
        {
            m_entities = new HashSet<SDBeesEntity>();
            m_relations = new HashSet<SDBeesRelation>();
            m_timeStamp = DateTime.Now;
        }

        private HashSet<SDBeesEntity> m_entities = new HashSet<SDBeesEntity>();
        /// <summary>
        /// Die zu übertragenden Entity Instanzen
        /// </summary>
        [DataMember]
        public HashSet<SDBeesEntity> Entities
        {
            get { return m_entities; }
            set { m_entities = value; }
        }

        private HashSet<SDBeesRelation> m_relations;
        /// <summary>
        /// Die zu übertragenden Beziehungs Instanzen
        /// </summary>
        [DataMember]
        public HashSet<SDBeesRelation> Relations
        {
            get { return m_relations; }
            set { m_relations = value; }
        }

        private DateTime m_timeStamp;
        /// <summary>
        /// ZeitStempel
        /// </summary>
        [DataMember]
        public DateTime TimeStamp
        {
            get { return m_timeStamp; }
            set { m_timeStamp = value; }
        }

        protected static SDBeesSetBase DataSetRead(string fileName, Type t)
        {
            SDBeesSetBase deserializedDataSet = null;

            try
            {
                Console.WriteLine("Deserializing an instance of the object.");
                if (!File.Exists(fileName))
                    return deserializedDataSet;

                var fs = new FileStream(fileName, FileMode.Open);
                var reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas());
                var ser = new DataContractSerializer(t);

                // Deserialize the data and read it from the instance.
                deserializedDataSet = (SDBeesSetBase)ser.ReadObject(reader, true);
                reader.Close();
                fs.Close();
            }
            catch (Exception ex)
            {

            }
            return deserializedDataSet;
        }

        public static void DataSetWrite(string fileName, SDBeesSetBase dataset)
        {
            try
            {
                var writer = new FileStream(fileName, FileMode.Create);
                var ser = new DataContractSerializer(dataset.GetType());
                ser.WriteObject(writer, dataset);
                writer.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static void DebugDataSetWrite(string fileName, SDBeesSetBase dataset)
        {
#if DEBUG && !PROFILER
            DataSetWrite(DirectoryTools.GetRootedFileName(fileName), dataset);
#endif
        }

        internal static void CreateSet(List<SDBeesDocumentId> docIds, SDBeesSetBase dset)
        {
            Error _error = null;

#if PROFILER
            SDBees.Profiler.Start("SDBeesSetBase.CreateSet");
#endif

            //SDBees.ViewAdmin.ViewRelation.FillCache();

            try
            {
                foreach (var docId in docIds)
                {
                    CreateSet_AddEntities1(docId, dset, ref _error);

                    CreateSet_AddEntities2(docId, dset, ref _error);

                    CreateSet_AddRelations(dset, ref _error);
                }
            }
            finally
            {
                //SDBees.ViewAdmin.ViewRelation.ClearCache();

#if PROFILER
                SDBees.Profiler.Stop();
#endif
            }
        }

        private static void CreateSet_AddEntities1(SDBeesDocumentId docId, SDBeesSetBase dset, ref Error _error)
        {
#if PROFILER
            SDBees.Profiler.Start("SDBeesSetBase::CreateSet_AddEntities1");
#endif
            var _existingObjects = new ArrayList();

            //Get all alien ids for this root
            if (ConnectivityManagerAlienBaseData.GetAlienIdsByDocumentSDBeesId(docId.Id, ref _error, ref _existingObjects))
            {
                //add all entities to the dataset for this doc
                foreach (string id in _existingObjects)
                {
                    var alienData = new ConnectivityManagerAlienBaseData();
                    if (alienData.Load(SDBeesDBConnection.Current.MyDBManager.Database, id, ref _error))
                    {
                        try
                        {
                            //Get Plugin linked to by alien id
                            var tnEntity = TemplateTreenode.GetPluginForType(alienData.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementTypeColumnName).ToString());
                            //Get PluginData
                            var tnEntityBasedata = tnEntity.CreateDataObject();
                            if (tnEntityBasedata.Load(SDBeesDBConnection.Current.MyDBManager.Database, alienData.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName), ref _error))
                            {
                                CreateEntity(dset, ref _error, docId, tnEntityBasedata);
                            }
                        }
                        catch (Exception ex)
                        {
                            var debugtest = 1;
                        }
                    }
                }
            }

#if PROFILER
            SDBees.Profiler.Stop();
#endif
        }

        private static void CreateSet_AddEntities2(SDBeesDocumentId docId, SDBeesSetBase dset, ref Error _error)
        {
#if PROFILER
            SDBees.Profiler.Start("SDBeesSetBase::CreateSet_AddEntities2");
#endif
            //Get all elements with Reference to same root which are not part of the selection above
            ArrayList _existingObjectsWithSameRootId = null;
            if (ViewRelation.GetViewRelationsByRootId(docId, ref _error, ref _existingObjectsWithSameRootId))
            {
                foreach (string idElem in _existingObjectsWithSameRootId)
                {
                    var rel = new ViewRelation();
                    if (rel.Load(SDBeesDBConnection.Current.MyDBManager.Database, idElem, ref _error))
                    {
                        var plgChild = TemplateTreenode.GetPluginForType(rel.GetPropertyByColumn(ViewRelation.m_ChildTypeColumnName).ToString());
                        if (plgChild != null)
                        {
                            var childBaseData = plgChild.CreateDataObject();
                            if (childBaseData != null)
                            {
                                if (childBaseData.Load(SDBeesDBConnection.Current.MyDBManager.Database, rel.GetPropertyByColumn(ViewRelation.m_ChildIdColumnName).ToString(), ref _error))
                                {
                                    CreateEntity(dset, ref _error, null, childBaseData);
                                }
                            }
                        }
                    }
                }
            }
#if PROFILER
            SDBees.Profiler.Stop();
#endif
        }

        private static void CreateSet_AddRelations(SDBeesSetBase dset, ref Error _error)
        {
            // Add relations
            foreach (var ent in dset.Entities)
            {
                CreateSet_AddRelationsToChilds(ent, dset, ref _error);

                CreateSet_AddRelationsToParents(ent, dset, ref _error);
            }
        }

        private static void CreateSet_AddRelationsToChilds(SDBeesEntity ent, SDBeesSetBase dset, ref Error _error)
        {
#if PROFILER
            SDBees.Profiler.Start("SDBeesSetBase::CreateSet_AddRelationsToChilds");
#endif
            //Relations to childs
            ArrayList _lstchild = null;
            var countchilds = ViewRelation.FindViewRelationByParentId(SDBeesDBConnection.Current.MyDBManager.Database, new Guid(ent.Id.ToString()), ref _lstchild, ref _error);
            if (countchilds > 0)
            {
                foreach (var itemid in _lstchild)
                {
                    var relSD = new ViewRelation();
                    if (relSD.Load(SDBeesDBConnection.Current.MyDBManager.Database, itemid, ref _error))
                    {
                        var rel = new SDBeesRelation();
                        rel.SourceId = ent.Id.ToString();
                        rel.TargetId = relSD.GetPropertyByColumn(ViewRelation.m_ChildIdColumnName).ToString();

                        if (!IsRelationInstanceContainedInDataSet(rel, dset))
                            dset.Relations.Add(rel);
                    }
                }
            }
#if PROFILER
            SDBees.Profiler.Stop();
#endif
        }

        private static void CreateSet_AddRelationsToParents(SDBeesEntity ent, SDBeesSetBase dset, ref Error _error)
        {
#if PROFILER
            SDBees.Profiler.Start("SDBeesSetBase::CreateSet_AddRelationsToParents");
#endif
            //Relations to parents
            ArrayList _lstparent = null;
            var countparents = ViewRelation.FindViewRelationByChildId(SDBeesDBConnection.Current.MyDBManager.Database, new Guid(ent.Id.ToString()), ref _lstparent, ref _error);
            if (countparents > 0)
            {
                foreach (var itemid in _lstparent)
                {
                    var relSD = new ViewRelation();
                    if (relSD.Load(SDBeesDBConnection.Current.MyDBManager.Database, itemid, ref _error))
                    {
                        var rel = new SDBeesRelation();
                        rel.SourceId = relSD.GetPropertyByColumn(ViewRelation.m_ParentIdColumnName).ToString();
                        rel.TargetId = ent.Id.ToString();

                        if (!IsRelationInstanceContainedInDataSet(rel, dset))
                            dset.Relations.Add(rel);
                    }
                }
            }
#if PROFILER
            SDBees.Profiler.Stop();
#endif
        }

        internal static void CreateEntity(SDBeesSetBase dset, ref Error _error, SDBeesDocumentId docId, TemplateDBBaseData tnbasedata)
        {
#if PROFILER
            SDBees.Profiler.Start("SDBeesSetBase::CreateEntity");
#endif
            if (!IsEntityInstanceContainedInDataSet(tnbasedata, dset))
            {
#if PROFILER
                SDBees.Profiler.Start("SDBeesSetBase::CreateEntity 1");
#endif
                var ent = new SDBeesEntity();
                ent.Id = tnbasedata.GetPropertyByColumn(Object.m_IdColumnName).ToString();
                ent.DefinitionId = TemplateDBBaseData.GetPluginForBaseData(tnbasedata).GetEntityDefinition().Id;
                var instanceId = tnbasedata.GetPropertyByColumn(Object.m_IdSDBeesColumnName).ToString();
                if (string.IsNullOrEmpty(instanceId))
                {
                    instanceId = Guid.NewGuid().ToString();
                    tnbasedata.SetPropertyByColumn(Object.m_IdSDBeesColumnName, instanceId);
                    tnbasedata.Save(SDBeesDBConnection.Current.MyDBManager.Database, ref _error);
                }
                ent.InstanceId.Id = tnbasedata.GetPropertyByColumn(Object.m_IdSDBeesColumnName).ToString();
#if PROFILER
                SDBees.Profiler.Stop();

                SDBees.Profiler.Start("SDBeesSetBase::CreateEntity 2");
#endif
                //Get database relevant properties
                foreach (var name in tnbasedata.GetPropertyNames())
                {
                    if (name != Object.m_IdSDBeesColumnName || name != Object.m_IdColumnName)
                    {
                        var prop = new SDBeesProperty();
                        prop.DefinitionId = name;
                        prop.InstanceValue.SetObjectValue(tnbasedata.GetPropertyByColumn(name));
                        ent.Properties.Add(prop);
                    }
                }
#if PROFILER
                SDBees.Profiler.Stop();

                SDBees.Profiler.Start("SDBeesSetBase::CreateEntity 3");
#endif
                // Apply automatic properties from plugin!
                // Currently you will get Display names! We have to think about a translation mechanism generally!
                // TODO: TH - Global names instead of display names!!
#if PROFILER
                SDBees.Profiler.Start("SDBeesSetBase::CreateEntity 3 - GetAutomaticProperties");
#endif
                var automaticProperties = tnbasedata.GetAutomaticProperties();
#if PROFILER
                SDBees.Profiler.Stop();

                SDBees.Profiler.Start("SDBeesSetBase::CreateEntity 3 - Create/Add properties");
#endif
                foreach (var pair in automaticProperties)
                {
                    var prop = new SDBeesProperty();
                    prop.DefinitionId = pair.Key.Name;
                    prop.InstanceValue.SetObjectValue(pair.Value);
                    ent.Properties.Add(prop);
                }
#if PROFILER
                SDBees.Profiler.Stop();

                SDBees.Profiler.Stop();

                SDBees.Profiler.Start("SDBeesSetBase::CreateEntity 4");
#endif
                //if (docId != null)
                {
                    var alienData = new ConnectivityManagerAlienBaseData();
                    var lst = new ArrayList();
                    ConnectivityManagerAlienBaseData.GetAlienIdsByDbId(tnbasedata.GetPropertyByColumn(Object.m_IdColumnName).ToString(), ref _error, ref lst);
                    foreach (var item in lst)
                    {
                        if (alienData.Load(ConnectivityManager.Current.MyDBManager.Database, item, ref _error))
                        {
                            var aid = new SDBeesAlienId();
                            aid.AlienInstanceId.Id = alienData.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienIdColumnName).ToString();
                            aid.App = alienData.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienApplicationColumnName).ToString();

#if false
                            //Create the documentData
                            // retrieve the id in db based on sdbeesid
                            string dbid = ConnectivityManagerDocumentBaseData.GetDocumentDbIdBySDBeesId(docId.Id, ref _error);
                            ConnectivityManagerDocumentBaseData docData = new ConnectivityManagerDocumentBaseData();
                            if (docData.Load(SDBees.DB.SDBeesDBConnection.Current.MyDBManager.Database, dbid, ref _error))
                            {
                                aid.DocumentId.Id = docData.GetPropertyByColumn(SDBees.DB.Object.m_IdSDBeesColumnName).ToString();
                            }
#else
                            aid.DocumentId.Id = alienData.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienDocumentIdColumnName).ToString();
#endif
                            aid.Id.Id = alienData.GetPropertyByColumn(Object.m_IdColumnName).ToString();

                            aid.InternalId = alienData.GetPropertyByColumn(Object.m_IdSDBeesColumnName).ToString();
                            aid.InternalDbObjectType = alienData.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementTypeColumnName).ToString();
                            aid.InternalDbObjectId = alienData.GetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName).ToString();

                            //if (docId != null && aid.DocumentId.Id == docId)
                                ent.AlienIds.Add(aid);
                        }
                    }
                }

                dset.Entities.Add(ent);
#if PROFILER
                SDBees.Profiler.Stop();
#endif
            }
#if PROFILER
            SDBees.Profiler.Stop();
#endif
        }

        internal static bool IsEntityInstanceContainedInDataSet(TemplateDBBaseData baseData, SDBeesSetBase set)
        {
#if PROFILER
            SDBees.Profiler.Start("SDBeesSetBase::IsEntityInstanceContainedInDataSet");
#endif
            var contained = false;
            foreach (var ent in set.Entities)
            {
                if (ent.Id.Id == baseData.GetPropertyByColumn(Object.m_IdColumnName).ToString())
                {
                    contained = true;
                    break;
                }
            }
#if PROFILER
            SDBees.Profiler.Stop();
#endif
            return contained;
        }

        internal static bool IsRelationInstanceContainedInDataSet(SDBeesRelation rel, SDBeesSetBase set)
        {
            var contained = false;
            foreach (var relTest in set.Relations)
            {
                if (relTest.SourceId.Id == rel.SourceId.Id &&
                    relTest.TargetId.Id == rel.TargetId.Id)
                {
                    contained = true;
                    break;
                }
            }
            return contained;
        }
    }
}
