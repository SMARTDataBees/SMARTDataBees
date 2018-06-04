using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SDBees.Core.Model.Instances
{
    /// <summary>
    /// Class represents a external document record in SDBees database
    /// </summary>
    [DataContract]
    public class SDBeesDBDocument
    {
        string m_name = "";
        [DataMember]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        Guid m_id = Guid.Empty;
        [DataMember]
        public Guid Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        Guid m_instanceId = Guid.Empty;
        [DataMember]
        public Guid InstanceId
        {
            get { return m_instanceId; }
            set { m_instanceId = value; }
        }

        string m_docfilename = "";
        [DataMember]
        public string Docfilename
        {
            get { return m_docfilename; }
            set { m_docfilename = value; }
        }

        string m_documentassignment = "";
        [DataMember]
        public string Documentassignment
        {
            get { return m_documentassignment; }
            set { m_documentassignment = value; }
        }

        string m_application = "";
        [DataMember]
        public string Application
        {
            get { return m_application; }
            set { m_application = value; }
        }

        string m_roleid = "";
        [DataMember]
        public string Roleid
        {
            get { return m_roleid; }
            set { m_roleid = value; }
        }

        string m_document_root = "";
        [DataMember]
        public string Document_root
        {
            get { return m_document_root; }
            set { m_document_root = value; }
        }

        string m_document_root_type = "";
        [DataMember]
        public string Document_root_type
        {
            get { return m_document_root_type; }
            set { m_document_root_type = value; }
        }

        SDBeesDocumentCADInfo m_CADInfo;
        [DataMember]
        public SDBeesDocumentCADInfo CADInfo
        {
            get { return m_CADInfo; }
            set { m_CADInfo = value; }
        }

        internal static SDBeesDBDocument CreateFromDBRecord(Connectivity.ConnectivityManagerDocumentBaseData docdata)
        {
            SDBeesDBDocument doc = new SDBeesDBDocument();
            try
            {
                doc.m_id= new Guid(docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_IdColumnName).ToString());

                Guid gSDBees = new Guid(docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_IdSDBeesColumnName).ToString());
                if (gSDBees == Guid.Empty)
                    gSDBees = Guid.NewGuid();

                doc.m_instanceId = gSDBees;

                doc.m_name = docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_NameColumnName).ToString();
                doc.m_roleid = docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_RoleIdColumnName).ToString();
                doc.m_application = docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_ApplicationColumnName).ToString();
                doc.m_docfilename = docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentFileColumnName).ToString();
                doc.m_document_root = docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName).ToString();
                doc.m_document_root_type = docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootTypeColumnName).ToString();
                doc.m_documentassignment = docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentAssignmentColumnName).ToString();
                doc.m_CADInfo = SDBeesDocumentCADInfo.Create(docdata.GetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentCADInfoColumnName).ToString());
            }
            catch (Exception ex)
            {

            } 
            
            return doc;
        }


        public static Connectivity.ConnectivityManagerDocumentBaseData CreateFromSDBeesDocument(SDBeesDBDocument doc)
        {
            SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData docData = new Connectivity.ConnectivityManagerDocumentBaseData();
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_IdSDBeesColumnName, doc.InstanceId);
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_NameColumnName, doc.Name);
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_RoleIdColumnName, doc.Roleid);
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_ApplicationColumnName, doc.Application);
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentFileColumnName, doc.Docfilename);
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName, doc.Document_root);
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootTypeColumnName, doc.Document_root_type);
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentAssignmentColumnName, doc.Documentassignment);
            docData.SetPropertyByColumn(Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentCADInfoColumnName, SDBeesDocumentCADInfo.Serialize(doc.CADInfo));

            return docData;
        }
    }
}
