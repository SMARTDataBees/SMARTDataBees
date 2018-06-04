using SDBees.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.Core.Model.Instances
{
    [DataContract(Name = "sdbeestransferset", Namespace = "http://www.smartdatabees.de")]
    public class SDBeesTransferSet : SDBees.Core.Model.SDBeesDataSet
    {
        public SDBeesTransferSet()
            : base()
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
        public static SDBeesTransferSet GetDatasetForDBDocuments(Dictionary<string, SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow> lstOfSelctedElements)
        {
            SDBeesTransferSet dset = new SDBeesTransferSet();
            SDBees.DB.Error _error = null;

            SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database.Open(false, ref _error);
            try
            {
                List<SDBeesDocumentId> docIds = new List<SDBeesDocumentId>();
                ArrayList _objDocIds = new ArrayList();

                //Create list of documents, that are involved in the export
                foreach (SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow row in lstOfSelctedElements.Values)
                {
                    AddDocuments(dset, ref _error, docIds, ref _objDocIds, row);
                }

                CreateSet(docIds, dset);

                dset.CleanUpExport(lstOfSelctedElements);
            }
            catch (Exception ex)
            {
            }

            SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database.Close(ref _error);

            return dset;
        }

        public static void PartialDataSetImport(string filename)
        {
            SDBees.Core.Model.Instances.SDBeesTransferSet dset = SDBees.Core.Model.Instances.SDBeesTransferSet.DataSetRead(filename);

            if (dset != null)
            {
                Error _error = null;
                SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database.Open(false, ref _error);

                Hashtable docHashOldIdToSDBeesId = new Hashtable();
                Hashtable docHashSDBeesIdToNewId = new Hashtable();

                Hashtable entHashOldIdToSDBeesId = new Hashtable();
                Hashtable entHashSDBeesIdToNewId = new Hashtable();

                //Create documents in db
                foreach (SDBees.Core.Model.Instances.SDBeesDBDocument doc in dset.DbDocuments)
                {
                    ArrayList _lstTemp = new ArrayList();
                    SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData dataDoc = SDBees.Core.Model.Instances.SDBeesDBDocument.CreateFromSDBeesDocument(doc);
                    if (!SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.ObjectExistsInDbWithSDBeesId(dataDoc.Table, doc.InstanceId, ref _error, ref _lstTemp))
                    {
                        if (dataDoc.Save(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database, ref _error))
                        {
                            docHashOldIdToSDBeesId.Add(doc.Id, dataDoc.GetPropertyByColumn(SDBees.DB.Object.m_IdSDBeesColumnName).ToString());
                            docHashSDBeesIdToNewId.Add(dataDoc.GetPropertyByColumn(SDBees.DB.Object.m_IdSDBeesColumnName).ToString(), dataDoc.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());
                        }
                    }
                }

                bool _newobjects = false;

                //Import the entities
                foreach (SDBeesEntity ent in dset.Entities)
                {
                    //TBD Update for already existing db objects
                    SDBees.Plugs.TemplateTreeNode.TemplateTreenode plug = SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetPluginForType(ent.DefinitionId.ToString());
                    if (plug != null)
                    {
                        Plugs.TemplateBase.TemplateDBBaseData dbEntityObject = plug.CreateDataObject();
                        ArrayList _lstIds = new ArrayList();
                        if (Plugs.TemplateBase.TemplateDBBaseData.ObjectExistsInDbWithSDBeesId(dbEntityObject.Table, ent.InstanceId.Id, ref _error, ref _lstIds))
                        {
                            if (dbEntityObject.Load(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database, _lstIds[0], ref _error))
                            {
                                //Existing object
                                SDBees.Core.Connectivity.ConnectivityManager.SetObjectData(ref _error, ent, dbEntityObject);

                                //Relations?
                            }
                        }
                        else
                        {
                            try
                            {
                                // New object, never synced before
                                _newobjects = true;
                                dbEntityObject.SetDefaults(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database);
                                SDBees.Core.Connectivity.ConnectivityManager.SetObjectData(ref _error, ent, dbEntityObject);

                                // Set the hashes
                                try
                                {
                                    if (!String.IsNullOrEmpty(ent.InstanceId.Id))
                                    {
                                        entHashOldIdToSDBeesId.Add(ent.Id.Id, ent.InstanceId.Id);
                                        entHashSDBeesIdToNewId.Add(ent.InstanceId.Id, dbEntityObject.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());
                                    }
                                    else
                                    {
                                        foreach (SDBeesProperty prop in ent.Properties)
                                        {
                                            if (prop.DefinitionId.Id == SDBees.DB.Object.m_IdSDBeesColumnName)
                                            {
                                                entHashOldIdToSDBeesId.Add(ent.Id.Id, dbEntityObject.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());
                                                entHashSDBeesIdToNewId.Add(prop.InstanceValue.ObjectValue.ToString(), dbEntityObject.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(ex.Message);
                                }

                                // Create the alien ids
                                foreach (SDBeesAlienId aId in ent.AlienIds)
                                {
                                    SDBees.Core.Connectivity.ConnectivityManagerAlienBaseData aIdData = new SDBees.Core.Connectivity.ConnectivityManagerAlienBaseData();
                                    aIdData.SetDefaults(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database);

                                    //Alien Id / Handle in BIM software
                                    aIdData.SetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerAlienBaseData.m_AlienIdColumnName, aId.AlienInstanceId.Id);
                                    aIdData.SetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerAlienBaseData.m_AlienInternalDBElementTypeColumnName, aId.InternalDbObjectType);
                                    aIdData.SetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerAlienBaseData.m_AlienApplicationColumnName, aId.App);

                                    //Ids
                                    aIdData.SetPropertyByColumn(SDBees.DB.Object.m_IdSDBeesColumnName, aId.InternalId);
                                    //aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName, dbEntityObject.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());//aId.InternalDbObjectId);
                                    aIdData.SetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerAlienBaseData.m_AlienInternalDBElementIdColumnName, dbEntityObject.GetPropertyByColumn(SDBees.DB.Object.m_IdColumnName).ToString());
                                    //aIdData.SetPropertyByColumn(SDBees.DB.Object.m_IdColumnName, aId.Id.Id);

                                    //Set the link to the document
                                    string docSDBeesId = aId.DocumentId.Id;
                                    //if (docHashSDBeesIdToNewId.ContainsKey(aId.DocumentId.Id))
                                    //{
                                    //    object temp = docHashSDBeesIdToNewId[aId.DocumentId.Id];
                                    //    docSDBeesId = temp.ToString();
                                    //}
                                    //if (!String.IsNullOrEmpty(docSDBeesId))
                                    aIdData.SetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerAlienBaseData.m_AlienDocumentIdColumnName, docSDBeesId);

                                    //aIdData.SetPropertyByColumn(ConnectivityManagerAlienBaseData.m_AlienIdColumnName, aId.AlienInstanceId.Id);
                                    aIdData.Save(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database, ref _error);
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
                    foreach (SDBees.Core.Model.Instances.SDBeesDBDocument doc in dset.DbDocuments)
                    {
                        SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData dataDoc = SDBees.Core.Model.Instances.SDBeesDBDocument.CreateFromSDBeesDocument(doc);
                        string docsdbeesid = SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.GetDocumentDbIdBySDBeesId(doc.InstanceId.ToString(), ref _error);
                        if (dataDoc.Load(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database, docsdbeesid, ref _error))
                        {
                            if (entHashOldIdToSDBeesId.ContainsKey(dataDoc.GetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName)))
                            {
                                object sdbeesid = entHashOldIdToSDBeesId[dataDoc.GetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName)];
                                object newid = entHashSDBeesIdToNewId[sdbeesid];
                                dataDoc.SetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName, newid);
                                dataDoc.Save(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database, ref _error);
                            }
                        }
                    }

                    // Import the relations
                    foreach (SDBeesRelation rel in dset.Relations)
                    {
                        ViewAdmin.ViewRelation viewrel = new SDBees.ViewAdmin.ViewRelation();
                        viewrel.SetDefaults(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database);

                        if (entHashOldIdToSDBeesId.ContainsKey(rel.SourceId.Id))
                        {
                            string sdbeesIdSource = entHashOldIdToSDBeesId[rel.SourceId.Id].ToString();
                            if (entHashSDBeesIdToNewId.ContainsKey(sdbeesIdSource))
                            {
                                string sourceid = entHashSDBeesIdToNewId[sdbeesIdSource].ToString();
                                viewrel.SetPropertyByColumn(SDBees.ViewAdmin.ViewRelation.m_ParentIdColumnName, sourceid);
                                viewrel.SetPropertyByColumn(SDBees.ViewAdmin.ViewRelation.m_ParentTypeColumnName, dset.GetEntityBySDBeesId(sdbeesIdSource).DefinitionId.Id);
                            }
                        }
                        else if (rel.SourceId.Id == Guid.Empty.ToString())
                        {
                            //Create a start ViewRelation
                            viewrel.SetPropertyByColumn(SDBees.ViewAdmin.ViewRelation.m_ParentIdColumnName, Guid.Empty.ToString());
                            viewrel.SetPropertyByColumn(SDBees.ViewAdmin.ViewRelation.m_ParentTypeColumnName, ViewAdmin.ViewRelation.m_StartNodeValue);
                        }

                        if (entHashOldIdToSDBeesId.ContainsKey(rel.TargetId.Id))
                        {
                            string sdbeesIdSource = entHashOldIdToSDBeesId[rel.TargetId.Id].ToString();
                            if (entHashSDBeesIdToNewId.ContainsKey(sdbeesIdSource))
                            {
                                string sourceid = entHashSDBeesIdToNewId[sdbeesIdSource].ToString();
                                viewrel.SetPropertyByColumn(SDBees.ViewAdmin.ViewRelation.m_ChildIdColumnName, sourceid);
                                viewrel.SetPropertyByColumn(SDBees.ViewAdmin.ViewRelation.m_ChildTypeColumnName, dset.GetEntityBySDBeesId(sdbeesIdSource).DefinitionId.Id);
                                viewrel.SetPropertyByColumn(SDBees.ViewAdmin.ViewRelation.m_ChildNameColumnName, dset.GetEntityBySDBeesId(sdbeesIdSource).GetProperty(SDBees.Plugs.TemplateBase.TemplateDBBaseData.m_NameColumnName).InstanceValue.ObjectValue.ToString());
                            }
                        }
                        viewrel.Save(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database, ref _error);
                    }
                }

                SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database.Close(ref _error);
            }
        }

        private static void AddDocuments(SDBeesTransferSet dset, ref SDBees.DB.Error _error, List<SDBeesDocumentId> docIds, ref ArrayList _objDocIds, SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow row)
        {
            //retrieve the related documents
            SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.GetDocumentsByAssignedSubContractor(row[SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentAssignmentDisplayName].ToString(), ref _error, ref _objDocIds);

            foreach (var item in _objDocIds)
            {
                SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData docdata = new Connectivity.ConnectivityManagerDocumentBaseData();
                if (docdata.Load(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database, item.ToString(), ref _error))
                {
                    SDBeesDocumentId docid = new SDBeesDocumentId()
                    {
                        Id = docdata.GetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_IdSDBeesColumnName).ToString(),
                    };

                    if (!docIds.Contains<SDBeesDocumentId>(docid))
                    {
                        docIds.Add(docid);

                        //Add SDBeesDocument to set
                        try
                        {
                            SDBeesDBDocument doc = SDBeesDBDocument.CreateFromDBRecord(docdata);
                            if (doc != null)
                            {
                                if (!dset.DbDocuments.Contains(doc))
                                {
                                    dset.DbDocuments.Add(doc);
                                    SDBeesTransferSet.AddRootItemToSet(dset, doc, ref _error);
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

        private static void AddRootItemToSet(SDBeesTransferSet dset, SDBeesDBDocument doc, ref SDBees.DB.Error _error)
        {
            //Get Plugin linked to by alien id
            SDBees.Plugs.TemplateTreeNode.TemplateTreenode tn = SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetPluginForType(doc.Document_root_type);
            //Get PluginData
            SDBees.Plugs.TemplateBase.TemplateDBBaseData tnbasedata = tn.CreateDataObject();
            if (tnbasedata.Load(SDBees.DB.SDBeesDBConnection.Current.MyDBManager.Database, doc.Document_root, ref _error))
            {
                SDBeesDataSet.CreateEntity(dset, ref _error, doc.Id.ToString(), tnbasedata);

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
        public void CleanUpExport(Dictionary<string, SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow> lstOfSelctedElements)
        {
            base.CleanUpExport();

            List<SDBeesEntity> entsRemove = new List<SDBeesEntity>();

            foreach (SDBeesEntity ent in Entities)
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

            foreach (SDBeesEntity entRemove in entsRemove)
            {
                Entities.Remove(entRemove);
            }
        }

        private bool IsEntityInList(SDBeesEntity ent, Dictionary<string, TreenodePropertyRow> lstOfSelctedElements)
        {
            bool isInList = false;
            foreach (SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow row in lstOfSelctedElements.Values)
            {
                string id = row[SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_IdColumnDisplayName].ToString();
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
            bool hasRelation = false;
            foreach (SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow row in lstOfSelctedElements.Values)
            {
                string id = row[SDBees.DB.Object.m_IdColumnDisplayName].ToString();
                SDBeesEntity entList = GetEntityByDbId(id);
                if(entList != null)
                {
                    foreach (SDBeesRelation rel in GetRelationsForEntityIsSource(entList))
                    {
                        if(rel.TargetId.Id == ent.Id.Id)
                        {
                            hasRelation = true;
                            break;
                        }
                    }

                    foreach (SDBeesRelation rel in GetRelationsForEntityIsTarget(entList))
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
            bool hasRelation = false;

            foreach (SDBeesAlienId aId in ent.AlienIds)
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
            bool hasSourceToDocRelation = false;

            foreach (SDBeesRelation relation in GetRelationsForEntityIsSource(ent))
            {
                SDBeesEntity targetent = GetEntityByDbId(relation.TargetId.Id);
                if (targetent != null)
                {
                    foreach (SDBeesAlienId aId in targetent.AlienIds)
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

            bool hasTargetToDocRelation = false;
            foreach (SDBeesRelation relation in GetRelationsForEntityIsTarget(ent))
            {
                SDBeesEntity sourceent = GetEntityByDbId(relation.SourceId.Id);
                if (sourceent != null)
                {
                    foreach (SDBeesAlienId aId in sourceent.AlienIds)
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
            else
                return false;
        }

        internal bool HasEntityRelationToEntity(SDBeesEntity ent, SDBeesEntity entTarget)
        {
            bool hasSourceToTargetEntityRelation = false;

            foreach (SDBeesRelation relation in GetRelationsForEntityIsSource(ent))
            {
                if (entTarget != null)
                {
                    if(relation.TargetId.Id == entTarget.Id.Id)
                            hasSourceToTargetEntityRelation = true;
                }
                if (hasSourceToTargetEntityRelation)
                    break;
            }

            bool hasTargetToTargetEntityRelation = false;
            foreach (SDBeesRelation relation in GetRelationsForEntityIsTarget(ent))
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
            else
                return false;
        }

        private bool HasAlienIdRelationToDBDoc(SDBeesAlienId aId)
        {
            bool result = false;
            foreach (Core.Model.Instances.SDBeesDBDocument docitem in DbDocuments)
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
