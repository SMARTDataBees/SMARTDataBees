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
	/// Definition 
	/// Mehrsprachigkeit ?
	/// </summary>
	[DataContract]
	public class SDBeesRelationshipDefinition
	{
        public SDBeesRelationshipDefinition()
        {
            Id = new SDBeesRelationshipDefinitionId();
            Name = new SDBeesLabel();
            RelationshipType = new SDBeesRelationshipType();
            SourceEntityDefId = new SDBeesEntityDefinitionId();
            TargetEntityDefId = new SDBeesEntityDefinitionId();
        }

        public SDBeesRelationshipDefinition(SDBeesRelationshipDefinitionId id, SDBeesLabel name, SDBeesRelationshipType relType, SDBeesEntityDefinitionId sourceEntityDefId, SDBeesEntityDefinitionId targetEntityDefId)
        {
            Id = id;
            Name = name;
            RelationshipType = relType;
            SourceEntityDefId = sourceEntityDefId;
            TargetEntityDefId = targetEntityDefId;
        }


	    /// <summary>
        /// Id der Beziehungdefinition
        /// </summary>
        [DataMember]
		public SDBeesRelationshipDefinitionId Id { get; set; }

	    /// <summary>
        /// Name der Beziehungdefinition
        /// </summary>
        [DataMember]
        public SDBeesLabel Name { get; set; }

	    /// <summary>
        /// Typ der Beziehung
        /// </summary>
        [DataMember]
		public SDBeesRelationshipType RelationshipType { get; set; }

	    /// <summary>
        /// Ursprungs Entity Typ der Beziehung
        /// </summary>
        [DataMember]
		public SDBeesEntityDefinitionId SourceEntityDefId { get; set; }

	    /// <summary>
        /// Ziel Entity Typ der Beziehung
        /// </summary>
        [DataMember]
		public SDBeesEntityDefinitionId TargetEntityDefId { get; set; }
	}
}

