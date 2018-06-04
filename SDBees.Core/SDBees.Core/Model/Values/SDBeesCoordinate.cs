using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using System.Runtime.Serialization;


namespace SDBees.Core.Model
{
    /// <summary>
    /// Repräsentation einer 3D Coordinate
    /// </summary>
    [DataContract]
    public class SDBeesCoordinate
	{
        public SDBeesCoordinate()
        {
            m_x = 0.0;
            m_y = 0.0;
            m_z = 0.0;
        }


        public SDBeesCoordinate(double x, double y, double z)
        {
            m_x = x;
            m_y = y;
            m_z = z;
        }

        public SDBeesCoordinate(string serialized)
        {
            SDBeesCoordinate coord = SerializationTools.Deserialize<SDBeesCoordinate>(serialized);
            m_x = coord.m_x;
            m_y = coord.m_y;
            m_z = coord.m_z;
        }

        private double m_x;
        [DataMember]
        public double X
		{
			get {return m_x;}
			set {m_x = value;}
		}

        private double m_y;
        [DataMember]
        public double Y
		{
			get {return m_y;}
			set {m_y = value;}
		}

        private double m_z;
        [DataMember]
        public double Z
		{
			get {return m_z;}
			set {m_z = value;}
		}

        public string Serialize()
        {
 	         return SerializationTools.Serialize(this);
        }
    }
}
