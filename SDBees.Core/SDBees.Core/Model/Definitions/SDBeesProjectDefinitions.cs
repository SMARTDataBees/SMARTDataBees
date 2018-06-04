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

	/// <remarks>SDBees Projekt Definition</remarks>
	[DataContract]
	public class SDBeesProjectDefinitions
	{
		private HashSet<SDBeesExternalMappings> m_pluginMappings;
        /// <summary>
        /// Mapping definitionen für alle Plugins
        /// </summary>
        [DataMember]
		public HashSet<SDBeesExternalMappings> PluginMappings
		{
            get { return m_pluginMappings; }
            set { m_pluginMappings = value; }
		}

		private HashSet<SDBeesEntityDefinition> m_entityDefinitions;
        /// <summary>
        /// Definitionen aller Entities mit Eigenschaften
        /// </summary>
        [DataMember]
		public HashSet<SDBeesEntityDefinition> EntityDefinitions
		{
            get { return m_entityDefinitions; }
            set { m_entityDefinitions = value; }
		}

		private HashSet<SDBeesRelationshipDefinition> m_relationshipDefinitions;
        /// <summary>
        /// Definitionen aller Entity Beziehungen
        /// </summary>
        [DataMember]
		public HashSet<SDBeesRelationshipDefinition> RelationshipDefinitions
		{
            get { return m_relationshipDefinitions; }
            set { m_relationshipDefinitions = value; }
		}


		public SDBeesExternalMappings getPluginMappings(SDBeesPluginId pluginId)
		{
			throw new System.NotImplementedException();
		}

		public  bool addMappedProperty(SDBeesExternalMappings mappings)
		{
			throw new System.NotImplementedException();
		}

        public bool addEntityDefinition(SDBeesEntityDefinition entityDefinition)
        {
            throw new System.NotImplementedException();
        }
	}
}

