using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using SDBees.Core.Admin;
using SDBees.Core.Connectivity;
using SDBees.DB;
using SDBees.Plugs.TemplateBase;
using SDBees.Plugs.TemplateTreeNode;
using Object = SDBees.DB.Object;

namespace SDBees.Core.Model.Instances
{
    [DataContract(Name = "sdbeestransferset", Namespace = "http://www.smartdatabees.de")]
    public class SDBeesTransferSet : SDBeesDataSet
    {
        public SDBeesTransferSet()
        {
            m_documents = new List<SDBeesDBDocument>();
        }

        private List<SDBeesDBDocument> m_documents;
        /// <summary>
        /// List of documents that are registered with the database
        /// </summary>
        [DataMember]
        public List<SDBeesDBDocument> DbDocuments
        {
            get { return m_documents; }
            set { m_documents = value; }
        }

        /// <summary>
        /// Creates a datastructure for a list of treenodepropertyrows
        /// </summary>
        /// <param name="lstOfSelctedElements"></param>
        public static SDBeesTransferSet GetDatasetForDBDocuments(Dictionary<string, TreenodePropertyRow> lstOfSelctedElements)
        {
            var dset = new SDBeesTransferSet();
            Error _error = null;

            ConnectivityManager.Current.MyDBManager.Database.Open(false, ref _error);
            try
            {
                var docIds = new List<SDBeesDocumentId>();
                var _objDocIds = new ArrayList();

                //Create list of documents, that are involved in the export
                foreach (var row in lstOfSelctedElements.Values)
                {
                    AddDocuments(dset, ref _error, docIds, ref _objDocIds, row);
                }

                CreateSet(docIds, dset);

                dset.CleanUpExport(lstOfSelctedElements);
            }
            catch (Exception ex)
            {
            }

            ConnectivityManager.Current.MyDBManager.Database.Close(ref _error);

            return dset;
        }

        public static void PartialDataSetImport(string filename)
        {
            var dset = DataSetRead(filename);

            if (dset != null)
            {
                Error _error = null;
                ConnectivityManager.Current.MyDBManager.Database.Open(false, ref _error);

                var docHashOldIdToSDBeesId = new Hashtable();
                var docHashSDBeesIdToNewId = new Hashtable();

                var entHashOldIdToSDBeesId = new Hashtable();
                var entHashSDBeesIdToNewId = new Hashtable();

                //Create documents in db
                foreach (var doc in dset.DbDocuments)
                {
                    var _lstTemp = new ArrayList();
                    var dataDoc = SDBeesDBDocument.CreateFromSDBeesDocument(doc);
                    if (!TemplateDBBaseData.ObjectExistsInDbWithSDBeesId(dataDoc.Table, doc.InstanceId, ref _error, ref _lstTemp))
                    {
                        if (dataDoc.Save(ConnectivityManager.Current.MyDBManager.Database, ref _error))
                        {
                            docHashOldIdToSDBeesId.Add(doc.Id, dataDoc.GetPropertyByColumn(Object.m_IdSDBeesColumnName).ToString());
                            docHashSDBeesIdToNewId.Add(dataDoc.GetPropertyByColumn(Object.m_IdSDBeesColumnName).ToString(), dataDoc.GetPropertyByColumn(Object.m_IdColumnName).ToString());
                        }
                    }
                }

                var _newobjects = false;

                //Import the entities
                foreach (var ent in dset.Entities)
                {
                    //TBD Update for already existing db objects
                    var plug = TemplateTreenode.GetPluginForType(ent.DefinitionId.ToString());
                    if (plug != null)
                    {
                        var dbEntityObject = plug.CreateDataObject();
                        var _lstIds = new ArrayList();
                        if (TemplateDBBaseData.ObjectExistsInDbWithSDBeesId(dbEntityObject.Table, ent.InstanceId.Id, ref _error, ref _lstIds))
                        {
                            if (dbEntityObject.Load(ConnectivityManager.Current.MyDBManager.Database, _lstIds[0], ref _error))
                            {
                                //Existing object
                                ConnectivityManager.SetObjectData(ref _error, ent, dbEntityObject);

                                //Relations?
                            }
                        }
                        else
                        {
                            try
                            {
                                // New object, never synced before
                                _newobjects = true;
                                dbEntityObject.SetDefaults(ConnectivityManager.Current.MyDBManager.Database);
                                ConnectivityManager.SetObjectData(ref _error, ent, dbEntityObject);

                                // Set the hashes
                                try
                                {
                                    if (!string.IsNullOrEmpty(ent.InstanceId.Id))
                                    {
                                        entHashOldIdToSDBeesId.Add(ent.Id.Id, ent.InstanceId.Id);
                                        entHashSDBeesIdToNewId.Add(ent.InstanceId.Id, dbEntityObject.GetPropertyByColumn(Object.m_IdColumnName).ToString());
                                    }
                                    else
                                    {
                                        foreach (var prop in ent.Properties)
                                        {
                                            if (prop.DefinitionId.Id == Object.m_IdSDBeesColumnName)
                                            {
                                                entHashOldIdToSDBeesId.Add(ent.Id.Id, dbEntityObject.GetPropertyByColumn(Object.m_IdColumnName).ToString());
                                                entHashSDBeesIdToNewId.Add(prop.InstanceValue.ObjectValue.ToString(), dbEntityObject.GetPropertyByColumn(Object.m_IdColumnName).ToString());
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }

                                // Create the alien ids
                                foreach (var aId in ent.AlienIds)
                                {
                                    var aIdData = new ConnectivityManagerAlienBaseData();
                                    aIdData.SetDefaults(ConnectivityManager.Current.MyDBManager.Database);

                                    //Alien Id / Handle in BIM software
                                    aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienIdColumnName, aId.AlienInstanceId.Id);
                                    aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementTypeColumnName, aId.InternalDbObjectType);
                                    aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienApplicationColumnName, aId.App);

                                    //Ids
                                    aIdData.SetPropertyByColumn(Object.m_IdSDBeesColumnName, aId.InternalId);
                                    //aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName, dbEntityObject.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());//aId.InternalDbObjectId);
                                    aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName, dbEntityObject.GetPropertyByColumn(Object.m_IdColumnName).ToString());
                                    //aIdData.SetPropertyByColumn(SDBees.DB.Object.m_IdColumnName, aId.Id.Id);

                                    //Set the link to the document
                                    var docSDBeesId = aId.DocumentId.Id;
                                    //if (docHashSDBeesIdToNewId.ContainsKey(aId.DocumentId.Id))
                                    //{
                                    //    object temp = docHashSDBeesIdToNewId[aId.DocumentId.Id];
                                    //    docSDBeesId = temp.ToString();
                                    //}
                                    //if (!String.IsNullOrEmpty(docSDBeesId))
                                    aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienDocumentIdColumnName, docSDBeesId);

                                    //aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienIdColumnName, aId.AlienInstanceId.Id);
                                    aIdData.Save(ConnectivityManager.Current.MyDBManager.Database, ref _error);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex.Message);
                            }
                        }
                    }
                }

                if (_newobjects)
                {
                    // Set document root for all documents
                    foreach (var doc in dset.DbDocuments)
                    {
                        var dataDoc = SDBeesDBDocument.CreateFromSDBeesDocument(doc);
                        var docsdbeesid = ConnectivityManagerDocumentBaseData.GetDocumentDbIdBySDBeesId(doc.InstanceId.ToString(), ref _error);
                        if (dataDoc.Load(ConnectivityManager.Current.MyDBManager.Database, docsdbeesid, ref _error))
                        {
                            if (entHashOldIdToSDBeesId.ContainsKey(dataDoc.GetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName)))
                            {
                                var sdbeesid = entHashOldIdToSDBeesId[dataDoc.GetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName)];
                                var newid = entHashSDBeesIdToNewId[sdbeesid];
                                dataDoc.SetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName, newid);
                                dataDoc.Save(ConnectivityManager.Current.MyDBManager.Database, ref _error);
                            }
                        }
                    }

                    // Import the relations
                    foreach (var rel in dset.Relations)
                    {
                        var viewrel = new ViewRelation();
                        viewrel.SetDefaults(ConnectivityManager.Current.MyDBManager.Database);

                        if (entHashOldIdToSDBeesId.ContainsKey(rel.SourceId.Id))
                        {
                            var sdbeesIdSource = entHashOldIdToSDBeesId[rel.SourceId.Id].ToString();
                            if (entHashSDBeesIdToNewId.ContainsKey(sdbeesIdSource))
                            {
                                var sourceid = entHashSDBeesIdToNewId[sdbeesIdSource].ToString();
                                viewrel.SetPropertyByColumn(ViewRelation.m_ParentIdColumnName, sourceid);
                                viewrel.SetPropertyByColumn(ViewRelation.m_ParentTypeColumnName, dset.GetEntityBySDBeesId(sdbeesIdSource).DefinitionId.Id);
                            }
                        }
                        else if (rel.SourceId.Id == Guid.Empty.ToString())
                        {
                            //Create a start ViewRelation
                            viewrel.SetPropertyByColumn(ViewRelation.m_ParentIdColumnName, Guid.Empty.ToString());
                            viewrel.SetPropertyByColumn(ViewRelation.m_ParentTypeColumnName, ViewRelation.m_StartNodeValue);
                        }

                        if (entHashOldIdToSDBeesId.ContainsKey(rel.TargetId.Id))
                        {
                            var sdbeesIdSource = entHashOldIdToSDBeesId[rel.TargetId.Id].ToString();
                            if (entHashSDBeesIdToNewId.ContainsKey(sdbeesIdSource))
                            {
                                var sourceid = entHashSDBeesIdToNewId[sdbeesIdSource].ToString();
                                viewrel.SetPropertyByColumn(ViewRelation.m_ChildIdColumnName, sourceid);
                                viewrel.SetPropertyByColumn(ViewRelation.m_ChildTypeColumnName, dset.GetEntityBySDBeesId(sdbeesIdSource).DefinitionId.Id);
                                viewrel.SetPropertyByColumn(ViewRelation.m_ChildNameColumnName, dset.GetEntityBySDBeesId(sdbeesIdSource).GetProperty(TemplateDBBaseData.m_NameColumnName).InstanceValue.ObjectValue.ToString());
                            }
                        }
                        viewrel.Save(ConnectivityManager.Current.MyDBManager.Database, ref _error);
                    }
                }

                ConnectivityManager.Current.MyDBManager.Database.Close(ref _error);
            }
        }

        private static void AddDocuments(SDBeesTransferSet dset, ref Error _error, List<SDBeesDocumentId> docIds, ref ArrayList _objDocIds, TreenodePropertyRow row)
        {
            //retrieve the related documents
            ConnectivityManagerDocumentBaseData.GetDocumentsByAssignedSubContractor(row[ConnectivityManagerDocumentBaseData.m_DocumentAssignmentDisplayName].ToString(), ref _error, ref _objDocIds);

            foreach (var item in _objDocIds)
            {
                var docdata = new ConnectivityManagerDocumentBaseData();
                if (docdata.Load(ConnectivityManager.Current.MyDBManager.Database, item.ToString(), ref _error))
                {
                    var docid = new SDBeesDocumentId
                    {
                        Id = docdata.GetPropertyByColumn(Object.m_IdSDBeesColumnName).ToString()
                    };

                    if (!docIds.Contains<SDBeesDocumentId>(docid))
                    {
                        docIds.Add(docid);

                        //Add SDBeesDocument to set
                        try
                        {
                            var doc = SDBeesDBDocument.CreateFromDBRecord(docdata);
                            if (doc != null)
                            {
                                if (!dset.DbDocuments.Contains(doc))
                                {
                                    dset.DbDocuments.Add(doc);
                                    AddRootItemToSet(dset, doc, ref _error);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        private static void AddRootItemToSet(SDBeesTransferSet dset, SDBeesDBDocument doc, ref Error _error)
        {
            //Get Plugin linked to by alien id
            var tn = TemplateTreenode.GetPluginForType(doc.Document_root_type);
            //Get PluginData
            var tnbasedata = tn.CreateDataObject();
            if (tnbasedata.Load(SDBeesDBConnection.Current.MyDBManager.Database, doc.Document_root, ref _error))
            {
                CreateEntity(dset, ref _error, doc.Id.ToString(), tnbasedata);

                //Add the relation
                //SDBeesEntity entRoot = dset.GetEntityByDbId(tnbasedata.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());
                //SDBeesRelation rel = new SDBeesRelation();
                //rel.SourceId = ViewAdmin.ViewRelation.m_StartNodeValue;
                //rel.TargetId = entRoot.Id.Id;

                //dset.Relations.Add(rel);
            }
        }

        /// <summary>
        /// Throws away SDBeesElements, which are not linked by the external documents for this Export
        /// </summary>
        public void CleanUpExport(Dictionary<string, TreenodePropertyRow> lstOfSelctedElements)
        {
            base.CleanUpExport();

            var entsRemove = new List<SDBeesEntity>();

            foreach (var ent in Entities)
            {
                if (!IsEntityInList(ent, lstOfSelctedElements))
                {
                    if (!HasEntityRelationToEntityInList(ent, lstOfSelctedElements))
                    {
                        if (!HasEntityRelationToDBDoc(ent))
                        {
                            if (!HasEntitySubRelationToDBDoc(ent))
                            {
                                entsRemove.Add(ent);
                            }
                        }  
                    }
                }
            }

            foreach (var entRemove in entsRemove)
            {
                Entities.Remove(entRemove);
            }
        }

        private bool IsEntityInList(SDBeesEntity ent, Dictionary<string, TreenodePropertyRow> lstOfSelctedElements)
        {
            var isInList = false;
            foreach (var row in lstOfSelctedElements.Values)
            {
                var id = row[Object.m_IdColumnDisplayName].ToString();
                if (id == ent.Id.Id)
                {
                    isInList = true;
                    break;
                }
            }
            return isInList;
        }

        private bool HasEntityRelationToEntityInList(SDBeesEntity ent, Dictionary<string, TreenodePropertyRow> lstOfSelctedElements)
        {
            var hasRelation = false;
            foreach (var row in lstOfSelctedElements.Values)
            {
                var id = row[Object.m_IdColumnDisplayName].ToString();
                var entList = GetEntityByDbId(id);
                if(entList != null)
                {
                    foreach (var rel in GetRelationsForEntityIsSource(entList))
                    {
                        if(rel.TargetId.Id == ent.Id.Id)
                        {
                            hasRelation = true;
                            break;
                        }
                    }

                    foreach (var rel in GetRelationsForEntityIsTarget(entList))
                    {
                        if(rel.SourceId.Id == ent.Id.Id)
                        {
                            hasRelation = true;
                            break;
                        }
                    }
                }
            }

            return hasRelation;
        }

        private bool HasEntityRelationToDBDoc(SDBeesEntity ent)
        {
            var hasRelation = false;

            foreach (var aId in ent.AlienIds)
            {
                if(HasAlienIdRelationToDBDoc(aId))
                {
                    hasRelation = true;
                    break;
                }
            }

            return hasRelation;
        }

        public static SDBeesTransferSet DataSetRead(string filenName)
        {
            return DataSetRead(filenName, typeof(SDBeesTransferSet)) as SDBeesTransferSet;
        }

        internal bool HasEntitySubRelationToDBDoc(SDBeesEntity ent)
        {
            var hasSourceToDocRelation = false;

            foreach (var relation in GetRelationsForEntityIsSource(ent))
            {
                var targetent = GetEntityByDbId(relation.TargetId.Id);
                if (targetent != null)
                {
                    foreach (var aId in targetent.AlienIds)
                    {
                        if (HasAlienIdRelationToDBDoc(aId))
                        {
                            hasSourceToDocRelation = true;
                        }
                        if (hasSourceToDocRelation)
                            break;
                    }
                }
                if (hasSourceToDocRelation)
                    break;
            }

            var hasTargetToDocRelation = false;
            foreach (var relation in GetRelationsForEntityIsTarget(ent))
            {
                var sourceent = GetEntityByDbId(relation.SourceId.Id);
                if (sourceent != null)
                {
                    foreach (var aId in sourceent.AlienIds)
                    {
                        if (HasAlienIdRelationToDBDoc(aId))
                        {
                            hasTargetToDocRelation = true;
                        }
                        if (hasTargetToDocRelation)
                            break;
                    }
                }
                if (hasTargetToDocRelation)
                    break;
            }

            #region old
            //    //Check direct relations
            //    foreach (SDBeesAlienId aId in ent.AlienIds)
            //    {
            //        if (aId.DocumentId.Id == docitem.InstanceId.ToString())
            //        {
            //            hasRelation = true;
            //            break;
            //        }
            //    }

            //    if (hasRelation)
            //        break;
            //    else
            //    {
            //        //Parse Subentities by relations
            //        foreach (SDBeesRelation relation in this.Relations)
            //        {
            //            if (relation.SourceId != null && relation.TargetId != null)
            //            {
            //                if (relation.SourceId.Id == ent.Id.Id)
            //                {
            //                    SDBeesEntity entTarget = this.GetEntityByDbId(relation.TargetId.Id);
            //                    if (entTarget != null)
            //                    {
            //                        if (HasRelationToDocsSub(entTarget, docitem))
            //                        {
            //                            hasRelation = true;
            //                            break;
            //                        }
            //                    }
            //                }
            //                else if(relation.TargetId.Id == ent.Id.Id)
            //                {
            //                    SDBeesEntity entSource = this.GetEntityByDbId(relation.SourceId.Id);
            //                    if (entSource != null)
            //                    {
            //                        if (HasRelationToDocsSub(entSource, docitem))
            //                        {
            //                            hasRelation = true;
            //                            break;
            //                        }
            //                    }

            //                }
            //            }
            //        }
            //    }
            //}
            #endregion

            if (hasSourceToDocRelation || hasTargetToDocRelation)
                return true;
            return false;
        }

        internal bool HasEntityRelationToEntity(SDBeesEntity ent, SDBeesEntity entTarget)
        {
            var hasSourceToTargetEntityRelation = false;

            foreach (var relation in GetRelationsForEntityIsSource(ent))
            {
                if (entTarget != null)
                {
                    if(relation.TargetId.Id == entTarget.Id.Id)
                            hasSourceToTargetEntityRelation = true;
                }
                if (hasSourceToTargetEntityRelation)
                    break;
            }

            var hasTargetToTargetEntityRelation = false;
            foreach (var relation in GetRelationsForEntityIsTarget(ent))
            {
                if (entTarget != null)
                {
                    if (relation.SourceId.Id == entTarget.Id.Id)
                        hasTargetToTargetEntityRelation = true;
                }
                if (hasTargetToTargetEntityRelation)
                    break;
            }

            if (hasSourceToTargetEntityRelation || hasTargetToTargetEntityRelation)
                return true;
            return false;
        }

        private bool HasAlienIdRelationToDBDoc(SDBeesAlienId aId)
        {
            var result = false;
            foreach (var docitem in DbDocuments)
            {
                if (aId.DocumentId.Id == docitem.InstanceId.ToString())
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
