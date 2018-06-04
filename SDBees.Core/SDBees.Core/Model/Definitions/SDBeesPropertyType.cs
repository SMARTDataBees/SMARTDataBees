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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    public enum SDBeesPropertyValueType : int
    {
		Text,
		Double,
        Coordinate,
		Integer,
		Boolean,
        Enumeration,
        Custom
    }

    public enum SDBeesPropertyMeasureType : int
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
            m_measureType = measureType;
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
                    if (m_valueType == SDBeesPropertyValueType.Enumeration)
                    {
                        m_enumerationValues = new HashSet<string>();
                    }
                    else
                    {
                        m_enumerationValues = null;
                    }
                }
            }
        }

        private SDBeesPropertyMeasureType m_measureType = SDBeesPropertyMeasureType.None;
        [DataMember]
        public SDBeesPropertyMeasureType MeasureType
        {
            get { return m_measureType; }
            set { m_measureType = value; }
        }


        private HashSet<string> m_enumerationValues = null;
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
            if (m_enumerationValues != null)
            {
                m_enumerationValues.Add(value);
            }
        }

        public static implicit operator SDBeesPropertyType(SDBeesPropertyValueType valueType)
        {
            SDBeesPropertyType propType = new SDBeesPropertyType(valueType);
            return propType;
        }
	}
}
