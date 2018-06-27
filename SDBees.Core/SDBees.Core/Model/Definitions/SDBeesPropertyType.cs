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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    public enum SDBeesPropertyValueType
    {
		Text,
		Double,
        Coordinate,
		Integer,
		Boolean,
        Enumeration,
        Custom
    }

    public enum SDBeesPropertyMeasureType
    {
        None,
        Length,
        Area,
        Volume,
        Weight,
        Time,
        Angle
    }

    /// <summary>
    /// Erlaubte Eigenschaft Typen
    /// </summary>
    [DataContract]
	public class SDBeesPropertyType
	{ 
        public SDBeesPropertyType ()
        {
            m_valueType = SDBeesPropertyValueType.Text;
            m_enumerationValues = null;
        }

        public SDBeesPropertyType (SDBeesPropertyValueType valueType)
        {
            ValueType = valueType;
            MeasureType = SDBeesPropertyMeasureType.None;
        }

        public SDBeesPropertyType(SDBeesPropertyMeasureType measureType)
        {
            ValueType = SDBeesPropertyValueType.Double;
            MeasureType = measureType;
        }

        public SDBeesPropertyType (HashSet<string> enumerationValues)
        {
            m_valueType = SDBeesPropertyValueType.Enumeration;
            m_enumerationValues = enumerationValues;
            MeasureType = SDBeesPropertyMeasureType.None;
        }

        private SDBeesPropertyValueType m_valueType = SDBeesPropertyValueType.Text;
        [DataMember]
        public SDBeesPropertyValueType ValueType
        {
            get { return m_valueType; }
            set 
            {
                if (value != m_valueType)
                {
                    m_valueType = value;
                    m_enumerationValues = m_valueType == SDBeesPropertyValueType.Enumeration ? new HashSet<string>() : null;
                }
            }
        }

	    [DataMember]
        public SDBeesPropertyMeasureType MeasureType { get; set; } = SDBeesPropertyMeasureType.None;


	    private HashSet<string> m_enumerationValues;
        [DataMember]
        public HashSet<string> EnumerationValues
        {
            get { return m_enumerationValues; }
            set {
                    if (value != null)
                    {
                        m_enumerationValues = value; 
                        m_valueType = SDBeesPropertyValueType.Enumeration;
                    }
                }
        }

        public void addEnumerationValue(string value)
        {
            m_enumerationValues?.Add(value);
        }

        public static implicit operator SDBeesPropertyType(SDBeesPropertyValueType valueType)
        {
            var propType = new SDBeesPropertyType(valueType);
            return propType;
        }
	}
}
