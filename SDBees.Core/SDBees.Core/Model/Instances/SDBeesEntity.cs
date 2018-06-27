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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    /// <summary>
	/// Repräsentation einer Entity Instanz
	/// </summary>
	[DataContract]
	public class SDBeesEntity
	{
		public SDBeesEntity()
		{
			m_entityDefinitionId = new SDBeesEntityDefinitionId();
			m_alienIds = new HashSet<SDBeesAlienId>();
			m_properties = new HashSet<SDBeesProperty>();
			m_id = new SDBeesEntityId();
			m_instanceId = new SDBeesEntityInstanceId();
		}

		//public SDBeesEntity(SDBeesEntityId instanceId, SDBeesEntityDefinitionId definitionId, HashSet<SDBeesAlienId> alienIds, HashSet<SDBeesProperty> properties)
		//{
		//    m_entityDefinitionId = new SDBeesEntityDefinitionId();
		//    m_alienIds = new HashSet<SDBeesAlienId>();
		//    m_properties = new HashSet<SDBeesProperty>();
		//    m_id = new SDBeesEntityId();
		//    m_instanceId = new SDBeesEntityInstanceId();
		//}

		private SDBeesEntityId m_id;
		/// <summary>
		/// DB Id der Entity Instanz
		/// </summary>
		[DataMember]
		public SDBeesEntityId Id
		{
			get {return m_id;}
			set {m_id = value;}
		}

		private SDBeesEntityInstanceId m_instanceId;
		/// <summary>
		/// SDBees instance Id der Entity Instanz
		/// </summary>
		[DataMember]
		public SDBeesEntityInstanceId InstanceId
		{
			get { return m_instanceId; }
			set { m_instanceId = value; }
		}

		private HashSet<SDBeesAlienId> m_alienIds;
		/// <summary>
		/// id des Entities im externen System
		/// </summary>
		[DataMember]
		public HashSet<SDBeesAlienId> AlienIds
		{
			get {return m_alienIds;}
			set {m_alienIds = value;}
		}

		private HashSet<SDBeesProperty> m_properties = new HashSet<SDBeesProperty>();
		/// <summary>
		/// Liste der zugeordneten Eigenschaften
		/// </summary>
		[DataMember]
		public HashSet<SDBeesProperty> Properties
		{
			get { return m_properties; }
			set { m_properties = value; }
		}

		private SDBeesEntityDefinitionId m_entityDefinitionId = new SDBeesEntityDefinitionId();
		/// <summary>
		/// Id der SDBees Entity Typ Definition
		/// </summary>
		[DataMember]
		public SDBeesEntityDefinitionId DefinitionId
		{
			get { return m_entityDefinitionId; }
			set { m_entityDefinitionId = value; }
		}

		//private SDBeesMappedEntityDefinition m_mappedEntityDefinition = new SDBeesMappedEntityDefinition();
		///// <summary>
		///// Mapped Entity Definition aus der das SDBees Entity enstanden ist.
		///// </summary>
		//[DataMember]
		//public SDBeesMappedEntityDefinition MappedEntityDefinition
		//{
		//    get { return m_mappedEntityDefinition; }
		//    set { m_mappedEntityDefinition = value; }
		//}

		private SDBeesMappedEntityDefinitionId m_mappedEntityDefinitionId = new SDBeesMappedEntityDefinitionId();
		/// <summary>
		/// Id der SDBees Mapped Entity Typ Definition
		/// </summary>
		[DataMember]
		public SDBeesMappedEntityDefinitionId MappedDefinitionId
		{
			get { return m_mappedEntityDefinitionId; }
			set { m_mappedEntityDefinitionId = value; }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyId"></param>
		/// <returns></returns>
		public SDBeesProperty GetProperty(string propertyId)
		{
            SDBeesProperty result = null;

			foreach (var property in Properties)
			{
				if (property.DefinitionId.Id == propertyId)
				{
                    result = property;

                    break;
				}
			}
			
            return result;
		}
	}
}

