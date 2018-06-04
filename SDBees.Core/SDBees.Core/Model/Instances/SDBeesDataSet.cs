// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2014 by
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
namespace SDBees.Core.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.Runtime.Serialization;
    using System.IO;
    using System.Xml;
    using System.Collections;
    using Instances;
    /// <summary>
    /// SDBees Datenaustauschklasse
    /// </summary>
    [DataContract(Name = "sdbeesdataset", Namespace = "http://www.smartdatabees.de")]
    public class SDBeesDataSet : SDBees.Core.Model.Basic.SDBeesSetBase
    {
        public SDBeesDataSet()
            : base()
        {
            this.m_pluginId = new SDBeesPluginId();
            this.m_documentId = new SDBeesDocumentId();
            this.m_pluginRoleId = new SDBeesPluginRoleId();
            this.m_documents = new List<SDBeesCADDocument>();
            this.m_entityDefinitions = null;
            //m_externalDocuments = new List<Connectivity.SDBeesLink.SDBeesExternalDocument>();
            this.m_explorerPlugin = null;
        }

        private string m_explorerPlugin;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string ExplorerPlugin
        {
            get { return m_explorerPlugin; }
            set { m_explorerPlugin = value; }
        }

        private SDBeesExternalMappings m_mappings;
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public SDBeesExternalMappings Mappings
        {
            get { return m_mappings; }
            set { m_mappings = value; }
        }

        private List<SDBeesCADDocument> m_documents;
        /// <summary>
        /// List of documents that are registered with the database
        /// </summary>
        [DataMember]
        public List<SDBeesCADDocument> Documents
        {
            get { return m_documents; }
            set { m_documents = value; }
        }

        //public SDBeesDataSet(SDBeesDocumentId documentId, SDBeesPluginId pluginId, SDBeesPluginRoleId roleId, HashSet<SDBeesEntity> entities, HashSet<SDBeesRelation> relations) : base()
        //{
        //    this.m_entities = entities;
        //    this.m_pluginId = pluginId;
        //    this.m_relations = relations;
        //    this.m_documentId = documentId;
        //    this.m_pluginRoleId = roleId;
        //    this.m_timeStamp = DateTime.Now;
        //}

        //private List<SDBees.Core.Connectivity.SDBeesLink.SDBeesExternalDocument> m_externalDocuments;
        ///// <summary>
        ///// Externe Docs für den partiellen Datenbankexport
        ///// </summary>
        //[DataMember]
        //public List<SDBees.Core.Connectivity.SDBeesLink.SDBeesExternalDocument> ExternalDocuments
        //{
        //    get { return m_externalDocuments; }
        //    set { m_externalDocuments = value; }
        //}

        private SDBeesPluginId m_pluginId;
        /// <summary>
        /// Kennung des Plugins
        /// </summary>
        [DataMember]
        public SDBeesPluginId PluginId
        {
            get { return m_pluginId; }
            set { m_pluginId = value; }
        }

        private SDBeesPluginRoleId m_pluginRoleId;
        /// <summary>
        /// Rolle des Plugins
        /// </summary>
        [DataMember]
        public SDBeesPluginRoleId PluginRoleId
        {
            get { return m_pluginRoleId; }
            set { m_pluginRoleId = value; }
        }

        private SDBeesDocumentId m_documentId;
        /// <summary>
        /// Der Id des zugeordneten SDBees Dokuments 
        /// </summary>
        [DataMember]
        public SDBeesDocumentId DocumentId
        {
            get { return m_documentId; }
            set { m_documentId = value; }
        }

        private Dictionary<string, SDBeesEntityDefinition> m_entityDefinitions;
        ///// <summary>
        ///// ...
        ///// </summary>
        [DataMember]
        public Dictionary<string, SDBeesEntityDefinition> EntityDefinitions
        {
            get { return m_entityDefinitions; }
            set { m_entityDefinitions = value; }
        }

        internal List<SDBeesRelation> GetRelationsForEntityIsSource(SDBeesEntity ent)
        {
            List<SDBeesRelation> lst = new List<SDBeesRelation>();
            //Linq query for all products
            var entQuery = from rel in Relations
                           where rel.SourceId.Id == ent.Id.Id
                           select rel;

            if (entQuery != null)
                lst = entQuery.ToList();

            return lst;
        }

        internal List<SDBeesRelation> GetRelationsForEntityIsTarget(SDBeesEntity ent)
        {
            List<SDBeesRelation> lst = new List<SDBeesRelation>();
            //Linq query for all products
            var entQuery = from rel in Relations
                           where rel.TargetId.Id == ent.Id.Id
                           select rel;

            if (entQuery != null)
                lst = entQuery.ToList();

            return lst;
        }

        public static SDBeesDataSet CreateExportDataset(SDBees.Core.Connectivity.SDBeesLink.SDBeesExternalDocument doc)
        {
#if PROFILER
            SDBees.Profiler.Start("SDBeesSetBase.CreateExportDataset");
#endif

            SDBeesDataSet dset = null;

            dset = new SDBeesDataSet();
            dset.DocumentId = doc.DocumentId;
            SDBees.DB.Error _error = null;

            SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database.Open(false, ref _error);

            try
            {
                SDBees.ViewAdmin.ViewCache.Enable();

                List<SDBeesDocumentId> ids = new List<SDBeesDocumentId>();
                SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData docdata = new Connectivity.ConnectivityManagerDocumentBaseData();
                ArrayList _objLst = new ArrayList();
                if (SDBees.Plugs.TemplateTreeNode.TemplateTreenodeBaseData.ObjectExistsInDbWithSDBeesId(docdata.Table, doc.DocumentId.Id, ref _error, ref _objLst))
                {
                    if (docdata.Load(SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database, _objLst[0], ref _error))
                    {
                        SDBeesDocumentId docid = new SDBeesDocumentId()
                        {
                            Id = docdata.GetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_IdSDBeesColumnName).ToString(),
                        };

                        ids.Add(docid);

                        try
                        {
                            SDBeesCADDocument cadDoc = SDBeesCADDocument.CreateFromDBRecord(docdata);
                            if (doc != null)
                            {
                                dset.Documents.Add(cadDoc);
                            }
                        }
                        catch (Exception ex)
                        {
                        }

                    }
                }

                CreateSet(ids, dset);
            }
            finally
            {
                SDBees.ViewAdmin.ViewCache.Disable();

                SDBees.Core.Connectivity.ConnectivityManager.Current.MyDBManager.Database.Close(ref _error);

#if PROFILER
                SDBees.Profiler.Stop();
#endif
            }

            return dset;
        }

        public static SDBeesDataSet DataSetRead(string filenName)
        {
            return DataSetRead(filenName, typeof(SDBeesDataSet)) as SDBeesDataSet;
        }

        public virtual void CleanUpExport()
        { }

        internal SDBeesEntity GetEntityByDbId(string id)
        {
            SDBeesEntity res = null;
            foreach (SDBeesEntity ent in this.Entities)
            {
                if (ent.Id.Id == id)
                {
                    res = ent;
                    break;
                }
            }
            return res;
        }

        internal SDBeesEntity GetEntityBySDBeesId(string sdbeesIdSource)
        {
            SDBeesEntity res = null;
            foreach (SDBeesEntity ent in this.Entities)
            {
                if (ent.InstanceId.Id == sdbeesIdSource)
                {
                    res = ent;
                    break;
                }
            }
            return res;
        }

        internal bool HasRelationToDocsSub(SDBeesEntity ent, SDBeesDBDocument docitem)
        {
            bool hasRelation = false;

            //Check direct relations
            foreach (SDBeesAlienId aId in ent.AlienIds)
            {
                if (aId.DocumentId.Id == docitem.InstanceId.ToString())
                {
                    hasRelation = true;
                    break;
                }
            }

            return hasRelation;
        }
    }
}

