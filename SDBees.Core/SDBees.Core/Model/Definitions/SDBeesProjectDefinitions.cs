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
    /// <remarks>SDBees Projekt Definition</remarks>
	[DataContract]
	public class SDBeesProjectDefinitions
	{
	    /// <summary>
        /// Mapping definitionen für alle Plugins
        /// </summary>
        [DataMember]
		public HashSet<SDBeesExternalMappings> PluginMappings { get; set; }

	    /// <summary>
        /// Definitionen aller Entities mit Eigenschaften
        /// </summary>
        [DataMember]
		public HashSet<SDBeesEntityDefinition> EntityDefinitions { get; set; }

	    /// <summary>
        /// Definitionen aller Entity Beziehungen
        /// </summary>
        [DataMember]
		public HashSet<SDBeesRelationshipDefinition> RelationshipDefinitions { get; set; }


	    public SDBeesExternalMappings getPluginMappings(SDBeesPluginId pluginId)
		{
			throw new NotImplementedException();
		}

		public  bool addMappedProperty(SDBeesExternalMappings mappings)
		{
			throw new NotImplementedException();
		}

        public bool addEntityDefinition(SDBeesEntityDefinition entityDefinition)
        {
            throw new NotImplementedException();
        }
	}
}

