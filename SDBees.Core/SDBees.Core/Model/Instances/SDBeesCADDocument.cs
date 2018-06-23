using System;
using System.Runtime.Serialization;
using SDBees.Core.Connectivity;
using Object = SDBees.DB.Object;

namespace SDBees.Core.Model
{
    /// <summary>
    /// Class represents a document record in SDBees database
    /// </summary>
    [DataContract]
    public class SDBeesCADDocument
    {
        Guid m_id = Guid.Empty;
        [DataMember]
        public Guid Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        SDBeesDocumentCADInfo m_CADInfo;
        [DataMember]
        public SDBeesDocumentCADInfo CADInfo
        {
            get { return m_CADInfo; }
            set { m_CADInfo = value; }
        }

        internal static SDBeesCADDocument CreateFromDBRecord(ConnectivityManagerDocumentBaseData docdata)
        {
            var doc = new SDBeesCADDocument();
            try
            {
                doc.m_id = new Guid(docdata.GetPropertyByColumn(Object.m_IdSDBeesColumnName).ToString());
                doc.m_CADInfo = SDBeesDocumentCADInfo.Create(docdata.GetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentCADInfoColumnName).ToString());
            }
            catch (Exception ex)
            {

            }

            return doc;
        }
    }
}
