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

using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    /// <summary>
    /// Instanz einer Eigenschaft
    /// </summary>
	[DataContract]
	public class SDBeesProperty
	{
        public SDBeesProperty()
        {
            m_propertyId = new SDBeesPropertyDefinitionId();
            m_propertyValue = new SDBeesPropertyValue();
        }

        public SDBeesProperty(SDBeesPropertyDefinitionId definitionId, SDBeesPropertyValue instanceValue)
        {
            m_propertyId = new SDBeesPropertyDefinitionId();
            m_propertyValue = instanceValue;
        }

		private SDBeesPropertyValue m_propertyValue;
        /// <summary>
        /// Der Wert der Eigenschaft
        /// </summary>
        [DataMember]
        public SDBeesPropertyValue InstanceValue
		{
            get { return m_propertyValue; }
            set { m_propertyValue = value; }
		}

        private SDBeesPropertyDefinitionId m_propertyId;
        /// <summary>
        /// Der Id der SDBees Eigenschaft Definition
        /// </summary>
        [DataMember]
        public SDBeesPropertyDefinitionId DefinitionId 
		{
            get { return m_propertyId; }
            set { m_propertyId = value; }
		}
	}
}

