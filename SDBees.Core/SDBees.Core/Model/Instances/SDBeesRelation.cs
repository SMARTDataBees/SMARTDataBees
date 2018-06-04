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
    /// Instanz einer Entity Beziehung
    /// </summary>
	[DataContract]
	public class SDBeesRelation
	{
        public SDBeesRelation()
        {
            m_alienSourceId = null;
            m_alienTargetId = null;
            m_relationshipDefinition = null;
            m_sourceId = null;
            m_targetId = null;
        }

        public SDBeesRelation(SDBeesRelationshipDefinitionId defId, SDBeesAlienId alienSourceId, SDBeesAlienId alienTargetId)
        {
            m_alienSourceId = alienSourceId;
            m_alienTargetId = alienTargetId;
            m_relationshipDefinition = defId;
            m_sourceId = null;
            m_targetId = null;
        }

        public SDBeesRelation(SDBeesRelationshipDefinitionId defId, SDBeesEntityId sourceId, SDBeesEntityId targetId)
        {
            m_sourceId = sourceId;
            m_targetId = targetId;
            m_relationshipDefinition = defId;
            m_alienSourceId = null;
            m_alienTargetId = null;
        }

        public SDBeesRelation(SDBeesRelationshipDefinitionId defId, SDBeesEntityId sourceId, SDBeesEntityId targetId, SDBeesAlienId alienSourceId, SDBeesAlienId alienTargetId)
        {
            m_sourceId = sourceId;
            m_targetId = targetId;
            m_relationshipDefinition = defId;
            m_alienSourceId = alienSourceId;
            m_alienTargetId = alienTargetId;
        }

		private SDBeesRelationshipDefinitionId m_relationshipDefinition;
        /// <summary>
        /// Der Typ der Beziehung
        /// </summary>
        [DataMember]
		public SDBeesRelationshipDefinitionId DefinitionId
		{
			get { return m_relationshipDefinition; }
			set { m_relationshipDefinition = value; }
		}

		public SDBeesAlienId m_alienSourceId;
        /// <summary>
        /// Das Ursprungs Entity
        /// </summary>
        [DataMember]
        public SDBeesAlienId AlienSourceId
		{
            get { return m_alienSourceId; }
            set { m_alienSourceId = value; }
		}

        public SDBeesEntityId m_sourceId;
        /// <summary>
        /// Das Ursprungs Entity
        /// </summary>
        [DataMember]
        public SDBeesEntityId SourceId
        {
            get { return m_sourceId; }
            set { m_sourceId = value; }
        }

        public SDBeesAlienId m_alienTargetId;
        /// <summary>
        /// Das Ursprungs Entity
        /// </summary>
        [DataMember]
        public SDBeesAlienId AlienTargetId
        {
            get { return m_alienTargetId; }
            set { m_alienTargetId = value; }
        }

        public SDBeesEntityId m_targetId;
        /// <summary>
        /// Das Ziel Entity
        /// </summary>
        [DataMember]
		public SDBeesEntityId TargetId
		{
            get { return m_targetId; }
            set { m_targetId = value; }
        }
	}
}

