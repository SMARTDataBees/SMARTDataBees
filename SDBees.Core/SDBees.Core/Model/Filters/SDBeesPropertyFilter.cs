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
    /// Filtere Entities nach Eigenschaften
    /// </summary>
	[DataContract]
	public class SDBeesPropertyFilter : SDBeesEntityFilter
	{
		private SDBeesPropertyDefinitionId m_propertyDefinitionId;
        /// <summary>
        /// Der Id der zu prüfenden Eigenschaft
        /// </summary>
        [DataMember]
		public SDBeesPropertyDefinitionId propertyDefinitionId
		{
            get { return m_propertyDefinitionId; }
            set { m_propertyDefinitionId = value; }
		}

		private SDBeesPropertyValue m_mainValue;
        /// <summary>
        /// Der erste Vergleichswert
        /// </summary>
        [DataMember]
		public SDBeesPropertyValue mainValue
		{
            get { return m_mainValue; }
            set { m_mainValue = value; }
		}

		private SDBeesPropertyValue m_value2;
        /// <summary>
        /// der zweite Vergleichswert
        /// </summary>
        [DataMember]
		public SDBeesPropertyValue value2
		{
            get { return m_value2; }
            set { m_value2 = value; }
		}

		private bool m_inverted;
        /// <summary>
        /// Inversion der Ergebnismenge
        /// </summary>
        [DataMember]
		public bool inverted
		{
            get { return m_inverted; }
            set { m_inverted = value; }
		}

        private SDBeesPropertyFilterType m_filterType;
        /// <summary>
        /// Der Typ des Filters
        /// </summary>
        [DataMember]
		public SDBeesPropertyFilterType filterType
		{
            get { return m_filterType; }
            set { m_filterType = value; }
		}
	}
}

