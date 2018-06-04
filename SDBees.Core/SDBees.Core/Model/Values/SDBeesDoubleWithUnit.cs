using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SDBees.Core.Model
{

    public enum SDBeesUnitType : int
    {
        Text,
        Double,
        Integer,
        Boolean,
        DoubleWithUnit,
        Enumeration
    }

    [DataContract]
    public class SDBeesDoubleWithUnit
    {
        public SDBeesDoubleWithUnit()
        {
            m_value = 0.0;
            m_unit = "";
        }

        public SDBeesDoubleWithUnit(double value, string unit)
        {
            m_value = value;
            m_unit = unit;
        }

		private double m_value;
        /// <summary>
        /// Der Double Wert
        /// </summary>
        [DataMember]
        public double Value
		{
            get { return m_value; }
            set { m_value = value; }
		}

        private string m_unit;
        /// <summary>
        /// Die einheit
        /// </summary>
        [DataMember]
        public string Unit
        {
            get { return m_unit; }
            set { m_unit = value; }
        }

    }
}
