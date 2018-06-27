using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    /// <summary>
    /// This class contains the local coord system plus the local unit for the document related geometry
    /// </summary>
    [DataContract]
    public class SDBeesDocumentCADInfo
    {
        public SDBeesDocumentCADInfo()
        {
            m_origin = null; ;
            m_zDirection = null;
            m_xDirection = null;
            m_mmToCADUnit = 0.0;
        }

        public SDBeesDocumentCADInfo(SDBeesCoordinate origin, SDBeesCoordinate zDirection, SDBeesCoordinate xDirection, double mmToCADUnit)
        {
            m_origin = origin;
            m_zDirection = zDirection;
            m_xDirection = xDirection;
            m_mmToCADUnit = mmToCADUnit;
        }

        static public SDBeesDocumentCADInfo Create(string serialized)
        {
            SDBeesDocumentCADInfo result = null;

            if (serialized != "")
            {
                result = SerializationTools.Deserialize<SDBeesDocumentCADInfo>(serialized);
            }

            return result;
        }

        public static string Serialize(SDBeesDocumentCADInfo info)
        {
            var result = "";

            if (info != null)
            {
                return SerializationTools.Serialize(info);
            }

            return result;
        }

        private SDBeesCoordinate m_origin;
        [DataMember]
        public SDBeesCoordinate Origin
		{
			get {return m_origin;}
			set {m_origin = value;}
		}

        private SDBeesCoordinate m_zDirection;
        [DataMember]
        public SDBeesCoordinate ZDirection
		{
            get { return m_zDirection; }
            set { m_zDirection = value; }
		}

        private SDBeesCoordinate m_xDirection;
        [DataMember]
        public SDBeesCoordinate XDirection
		{
            get { return m_xDirection; }
            set { m_xDirection = value; }
		}

        private double m_mmToCADUnit;
        [DataMember]
        public double MMToCADUnit
		{
            get { return m_mmToCADUnit; }
            set { m_mmToCADUnit = value; }
		}
    }
}
