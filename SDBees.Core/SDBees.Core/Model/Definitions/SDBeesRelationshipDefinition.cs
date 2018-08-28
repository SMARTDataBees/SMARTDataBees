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
            m_Id = new SDBeesRelationshipDefinitionId();
            m_name = new SDBeesLabel();
            m_relationshipType = new SDBeesRelationshipType();
            m_sourceId = new SDBeesEntityDefinitionId();
            m_targetId = new SDBeesEntityDefinitionId();
        }

        public SDBeesRelationshipDefinition(SDBeesRelationshipDefinitionId id, SDBeesLabel name, SDBeesRelationshipType relType, SDBeesEntityDefinitionId sourceEntityDefId, SDBeesEntityDefinitionId targetEntityDefId)
        {
            m_Id = id;
            m_name = name;
            m_relationshipType = relType;
            m_sourceId = sourceEntityDefId;
            m_targetId = targetEntityDefId;
        }


		private SDBeesRelationshipDefinitionId m_Id;
        /// <summary>
        /// Id der Beziehungdefinition
        /// </summary>
        [DataMember]
		public SDBeesRelationshipDefinitionId Id
		{
			get { return m_Id; }
			set { m_Id = value; }
		}

        private SDBeesLabel m_name;
        /// <summary>
        /// Name der Beziehungdefinition
        /// </summary>
        [DataMember]
        public SDBeesLabel Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        public SDBeesRelationshipType m_relationshipType;
        /// <summary>
        /// Typ der Beziehung
        /// </summary>
        [DataMember]
		public SDBeesRelationshipType RelationshipType
		{
            get { return m_relationshipType; }
            set { m_relationshipType = value; }
		}

		private SDBeesEntityDefinitionId m_sourceId;
        /// <summary>
        /// Ursprungs Entity Typ der Beziehung
        /// </summary>
        [DataMember]
		public SDBeesEntityDefinitionId SourceEntityDefId
		{
            get { return m_sourceId; }
            set { m_sourceId = value; }
		}

		private SDBeesEntityDefinitionId m_targetId;
        /// <summary>
        /// Ziel Entity Typ der Beziehung
        /// </summary>
        [DataMember]
		public SDBeesEntityDefinitionId TargetEntityDefId
		{
            get { return m_targetId; }
            set { m_targetId = value; }
		}
	}
}

