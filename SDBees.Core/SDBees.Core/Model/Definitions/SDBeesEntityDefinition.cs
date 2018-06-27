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
using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    /// <remarks>
	/// Definition eines Entity Typs
	/// </remarks>
	[DataContract]
    public class SDBeesEntityDefinition
	{
        public SDBeesEntityDefinition()
        {
            m_properties = new HashSet<SDBeesPropertyDefinition>();
            m_id = new SDBeesEntityDefinitionId();
            m_name = new SDBeesLabel();
        }

        public SDBeesEntityDefinition(SDBeesEntityDefinitionId id, SDBeesLabel label, HashSet<SDBeesPropertyDefinition> properties)
        {
            m_properties = properties;
            m_id = id;
            m_name = label;
        }

        public SDBeesEntityDefinition(Type AddinType)
        {
            m_properties = new HashSet<SDBeesPropertyDefinition>();
            m_id = new SDBeesEntityDefinitionId(AddinType.ToString());
            m_name = AddinType.Name;
        }

		private SDBeesEntityDefinitionId m_id;
        /// <summary>
        /// id dieser Entity Typ Definition
        /// </summary>
        [DataMember]
        public SDBeesEntityDefinitionId Id
		{
            get { return m_id; }
            set { m_id = value; }
        }

        private SDBeesLabel m_name;
        /// <summary>
        /// Name des Entities
        /// </summary>
        [DataMember]
		public SDBeesLabel Name
		{
            get { return m_name; }
            set { m_name = value; }
        }

        private HashSet<SDBeesPropertyDefinition> m_properties;
        /// <summary>
        /// Alle für das Entity definierten Eigenschaften
        /// </summary>
        [DataMember]
        public HashSet<SDBeesPropertyDefinition> Properties
        {
            get { return m_properties; }
            set { m_properties = value; }
        }

	}
}

