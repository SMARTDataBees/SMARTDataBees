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

    /// <summary>
    /// Wrapper für einen Varianten Objekttyp
    /// </summary>
	[DataContract]
	public class SDBeesPropertyValue
	{
        public SDBeesPropertyValue()
        {
            m_state = 0;

            m_objectValue = null;

            m_currentAutomaticObjectValue = null;

            m_newAutomaticObjectValue = null;
        }

        public SDBeesPropertyValue(object value)
        {
            m_state = 0;

            m_objectValue = value;

            m_currentAutomaticObjectValue = null;

            m_newAutomaticObjectValue = null;
        }

        public void SetObjectValue(object value)
        {
            m_state = 0;

            m_objectValue = value;

            m_currentAutomaticObjectValue = null;

            m_newAutomaticObjectValue = null;
        }

        public void SetObjectValueAutomatic(object automaticValue, bool force = false)
        {
            if (force == true)
            {
                m_state = 1;

                m_objectValue = automaticValue;

                m_currentAutomaticObjectValue = null;

                m_newAutomaticObjectValue = null;
            }
            else
            {
                if (m_state == 0)
                {
                    if (automaticValue == null)
                    {
                        if (isEmptyOrNull(m_objectValue))
                        {
                            m_state = 1; //0; // Should we introduce another state 3?
                        }
                        else
                        {
                            m_state = 2;
                        }
                    }
                    else if (hasChanged(automaticValue, m_objectValue))
                    {
                        m_state = 2;

                        m_currentAutomaticObjectValue = m_objectValue;

                        m_newAutomaticObjectValue = automaticValue;
                    }
                    else
                    {
                        m_state = 1;

                        m_objectValue = automaticValue;
                    }
                }
                else if (m_state == 1)
                {
                    m_objectValue = automaticValue;
                }
                else if (m_state == 2)
                {
                    if (isEqual(automaticValue, m_objectValue))
                    { 
                        m_currentAutomaticObjectValue = automaticValue;
                    }
                    else
                    {
                        m_newAutomaticObjectValue = automaticValue;
                    }
                }
            }

            if (m_objectValue == null) m_objectValue = "";
        }

        public void UpdateObjectAutomaticValues()
        {
            m_currentAutomaticObjectValue = m_newAutomaticObjectValue;

            m_newAutomaticObjectValue = null;
        }

        public void SetObjectValueManual(object manualValue)
        {
            if (m_state == 2)
            {
                m_objectValue = manualValue;
            }
            else
            {
                m_state = 2;

                m_currentAutomaticObjectValue = m_objectValue;

                m_objectValue = manualValue;
            }

            if (m_objectValue == null) m_objectValue = "";
        }

        public bool isAutomatic()
        {
            return m_state == 1;
        }

        public bool isManual()
        {
            return m_state == 2;
        }

        public bool isUptodate()
        {
            return (m_newAutomaticObjectValue == null) || isEqual(m_currentAutomaticObjectValue, m_newAutomaticObjectValue);
        }

        private static bool isEqual(object value1, object value2)
        {
            return ((value1 != null) && (value2 != null)) ? value1.Equals(value2) : value1 == value2;
        }

        private static bool hasChanged(object automaticValue, object objectValue)
        {
            return !isEmptyOrNull(objectValue) && !isEqual(automaticValue, objectValue);
        }

        private static bool isEmptyOrNull(object objectValue)
        {
            return (objectValue == null) || (objectValue.ToString() == "");
        }

        private int m_state;
        /// <summary>
        /// Das variante Objekt
        /// </summary>
        [DataMember]
		public int State
		{
            get { return m_state; }
            set { m_state = value; }
        }

        private object m_objectValue;
        /// <summary>
        /// Das variante Objekt
        /// </summary>
        [DataMember]
		public object ObjectValue
		{
            get { return m_objectValue; }
            set { m_objectValue = value; }
        }

        private object m_currentAutomaticObjectValue;
        /// <summary>
        /// Das variante Objekt
        /// </summary>
        [DataMember]
        public object CurrentAutomaticObjectValue
        {
            get { return m_currentAutomaticObjectValue; }
            set { m_currentAutomaticObjectValue = value; }
        }

        private object m_newAutomaticObjectValue;
        /// <summary>
        /// Das variante Objekt
        /// </summary>
        [DataMember]
        public object NewAutomaticObjectValue
        {
            get { return m_newAutomaticObjectValue; }
            set { m_newAutomaticObjectValue = value; }
        }
    }
}

