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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    [DataContract]
    public abstract class SDBeesId
    {
        private string m_Id = null;
        [DataMember]
        public string Id
        {
            get { return m_Id; }
            set { m_Id = value; }
        }
        
        public SDBeesId()
        {
            m_Id = "";
        }

        public SDBeesId(string id)
        {
            m_Id = id;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesId id = obj as SDBeesId;
            if (id == null)
            {
                return false;
            }

            return m_Id == id.m_Id;
        }

        public override int GetHashCode()
        {
            return m_Id.GetHashCode();
        }

        public override string ToString()
        {
            return m_Id;
        }
    }

    [DataContract]
    public class SDBeesProjectId : SDBeesId
    {
        public SDBeesProjectId() : base()
        {
        }

        public SDBeesProjectId(string id) : base (id)
        {
        }

        public static bool operator ==(SDBeesProjectId id1, SDBeesProjectId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesProjectId id1, SDBeesProjectId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesProjectId id = obj as SDBeesProjectId;
            if (id == null)
            {
                return false;
            }

            return base.Equals (obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesProjectId(string idText)
        {
            SDBeesProjectId id = new SDBeesProjectId(idText);
            return id;
        }

    }

    [DataContract]
    public class SDBeesDocumentId : SDBeesId
    {
        public SDBeesDocumentId(string id) : base(id)
        {
        }

        public SDBeesDocumentId() : base()
        {
        }

        //private SDBeesEntityId m_internalId;
        ///// <summary>
        ///// The internal SDBees id for consistency across databases (export / import)
        ///// </summary>
        //[DataMember]
        //public SDBeesEntityId InternalId
        //{
        //    get { return m_internalId; }
        //    set { m_internalId = value; }
        //}

        public static bool operator ==(SDBeesDocumentId id1, SDBeesDocumentId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesDocumentId id1, SDBeesDocumentId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesDocumentId id = obj as SDBeesDocumentId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesDocumentId(string idText)
        {
            SDBeesDocumentId id = new SDBeesDocumentId(idText);
            return id;
        }

    }


    [DataContract]
    public class SDBeesPluginId : SDBeesId
    {
        public SDBeesPluginId() : base()
        {
        }

        public SDBeesPluginId(string id) : base(id)
        {
        }

        public static bool operator ==(SDBeesPluginId id1, SDBeesPluginId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesPluginId id1, SDBeesPluginId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesPluginId id = obj as SDBeesPluginId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesPluginId(string idText)
        {
            SDBeesPluginId id = new SDBeesPluginId(idText);
            return id;
        }

    }

    [DataContract]
    public class SDBeesPluginRoleId : SDBeesId
    {
        public SDBeesPluginRoleId() : base()
        {
        }

        public SDBeesPluginRoleId(string id) : base(id)
        {
        }

        public static bool operator ==(SDBeesPluginRoleId id1, SDBeesPluginRoleId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesPluginRoleId id1, SDBeesPluginRoleId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesPluginRoleId id = obj as SDBeesPluginRoleId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesPluginRoleId(string idText)
        {
            SDBeesPluginRoleId id = new SDBeesPluginRoleId(idText);
            return id;
        }

    }


    [DataContract]
    public class SDBeesEntityId : SDBeesId
    {
        public SDBeesEntityId() : base()
        {
        }

        public SDBeesEntityId(string id) : base(id)
        {
        }

        public static bool operator ==(SDBeesEntityId id1, SDBeesEntityId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesEntityId id1, SDBeesEntityId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesEntityId id = obj as SDBeesEntityId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesEntityId(string idText)
        {
            SDBeesEntityId id = new SDBeesEntityId(idText);
            return id;
        }

    }

    [DataContract]
    public class SDBeesEntityInstanceId : SDBeesId
    {
        public SDBeesEntityInstanceId()
            : base()
        {
        }

        public SDBeesEntityInstanceId(string id)
            : base(id)
        {
        }

        public static bool operator ==(SDBeesEntityInstanceId id1, SDBeesEntityInstanceId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesEntityInstanceId id1, SDBeesEntityInstanceId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesEntityInstanceId id = obj as SDBeesEntityInstanceId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesEntityInstanceId(string idText)
        {
            SDBeesEntityInstanceId id = new SDBeesEntityInstanceId(idText);
            return id;
        }
    }

    [DataContract]
    public class SDBeesEntityDefinitionId : SDBeesId
    {
        public SDBeesEntityDefinitionId() : base()
        {
        }

        public SDBeesEntityDefinitionId(string id) : base(id)
        {
        }

        public static bool operator ==(SDBeesEntityDefinitionId id1, SDBeesEntityDefinitionId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesEntityDefinitionId id1, SDBeesEntityDefinitionId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesEntityDefinitionId id = obj as SDBeesEntityDefinitionId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesEntityDefinitionId(string idText)
        {
            SDBeesEntityDefinitionId id = new SDBeesEntityDefinitionId(idText);
            return id;
        }

    }

    [DataContract]
    public class SDBeesPropertyDefinitionId : SDBeesId
    {
        public SDBeesPropertyDefinitionId() : base()
        {
        }

        public SDBeesPropertyDefinitionId(string id) : base(id)
        {
        }

        public static bool operator ==(SDBeesPropertyDefinitionId id1, SDBeesPropertyDefinitionId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesPropertyDefinitionId id1, SDBeesPropertyDefinitionId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesPropertyDefinitionId id = obj as SDBeesPropertyDefinitionId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesPropertyDefinitionId(string idText)
        {
            SDBeesPropertyDefinitionId id = new SDBeesPropertyDefinitionId(idText);
            return id;
        }

    }

    [DataContract]
    public class SDBeesRelationshipInstanceId : SDBeesId
    {
        public SDBeesRelationshipInstanceId() : base()
        {
        }

        public SDBeesRelationshipInstanceId(string id) : base(id)
        {
        }

        public static bool operator ==(SDBeesRelationshipInstanceId id1, SDBeesRelationshipInstanceId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesRelationshipInstanceId id1, SDBeesRelationshipInstanceId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesRelationshipInstanceId id = obj as SDBeesRelationshipInstanceId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesRelationshipInstanceId(string idText)
        {
            SDBeesRelationshipInstanceId id = new SDBeesRelationshipInstanceId(idText);
            return id;
        }

    }

    [DataContract]
    public class SDBeesRelationshipDefinitionId : SDBeesId
    {
        public SDBeesRelationshipDefinitionId()
            : base()
        {
        }

        public SDBeesRelationshipDefinitionId(string id) : base(id)
        {
        }

        public static bool operator ==(SDBeesRelationshipDefinitionId id1, SDBeesRelationshipDefinitionId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesRelationshipDefinitionId id1, SDBeesRelationshipDefinitionId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesRelationshipDefinitionId id = obj as SDBeesRelationshipDefinitionId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesRelationshipDefinitionId(string idText)
        {
            SDBeesRelationshipDefinitionId id = new SDBeesRelationshipDefinitionId(idText);
            return id;
        }

    }

    [DataContract]
    public class SDBeesAlienInstanceId : SDBeesId
    {
        public SDBeesAlienInstanceId()
            : base()
        {
        }

        public SDBeesAlienInstanceId(string id)
            : base(id)
        {
        }

        public static bool operator ==(SDBeesAlienInstanceId id1, SDBeesAlienInstanceId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesAlienInstanceId id1, SDBeesAlienInstanceId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesAlienInstanceId id = obj as SDBeesAlienInstanceId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesAlienInstanceId(string idText)
        {
            SDBeesAlienInstanceId id = new SDBeesAlienInstanceId(idText);
            return id;
        }
    }


    [DataContract]
    public class SDBeesAlienId
    {
        private SDBeesEntityId m_id;
        /// <summary>
        /// The database related id for each record.
        /// differs for each db and is not unique in different databases!
        /// </summary>
        [DataMember]
        public SDBeesEntityId Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

        private string m_internalId;
        /// <summary>
        /// The unique internal SDBees guid
        /// </summary>
        [DataMember]
        public string InternalId
        {
            get { return m_internalId; }
            set { m_internalId = value; }
        }
        
        private SDBeesDocumentId m_docId;
        [DataMember]
        public SDBeesDocumentId DocumentId
        {
            get { return m_docId; }
            set { m_docId = value; }
        }

        private string m_app;
        /// <summary>
        /// The alien application
        /// </summary>
        [DataMember]
        public string App
        {
            get { return m_app; }
            set { m_app = value; }
        }

        private SDBeesAlienInstanceId m_alieninstanceId;
        /// <summary>
        /// The handle or external id in BIM software
        /// </summary>
        [DataMember]
        public SDBeesAlienInstanceId AlienInstanceId
        {
            get { return m_alieninstanceId; }
            set { m_alieninstanceId = value; }
        }

        private string m_internaldbobjectid;
        [DataMember]
        public string InternalDbObjectId
        {
            get { return m_internaldbobjectid; }
            set { m_internaldbobjectid = value; }
        }

        private string m_internalobjecttype;
        [DataMember]
        public string InternalDbObjectType
        {
            get { return m_internalobjecttype; }
            set { m_internalobjecttype = value; }
        }

        public SDBeesAlienId()
        {
            m_docId = new SDBeesDocumentId();
            m_alieninstanceId = new SDBeesAlienInstanceId();
            m_id = new SDBeesEntityId();
        }

        public SDBeesAlienId(SDBeesAlienInstanceId alienInstanceId, SDBeesDocumentId documentId)
        {
            m_alieninstanceId = alienInstanceId;
            m_docId = documentId;
        }

        public SDBeesAlienId(SDBeesAlienInstanceId alienInstanceId, SDBeesDocumentId documentId, SDBeesEntityId instanceId)
        {
            m_alieninstanceId = alienInstanceId;
            m_docId = documentId;
            m_id = instanceId;
        }

        public static bool operator ==(SDBeesAlienId id1, SDBeesAlienId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return (id1.m_alieninstanceId == id2.m_alieninstanceId) && (id1.m_docId == id2.m_docId);
                }
                else
                {
                    return true;
                }
            }
            return false;
            
        }

        public static bool operator !=(SDBeesAlienId id1, SDBeesAlienId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesAlienId id = obj as SDBeesAlienId;
            if (id != null)
            {
                return (m_alieninstanceId == id.m_alieninstanceId) && (m_docId == id.m_docId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return m_alieninstanceId.GetHashCode() ^ m_docId.GetHashCode();
        }

    }

    [DataContract]
    public class SDBeesMappedEntityDefinitionId : SDBeesId
    {
        public SDBeesMappedEntityDefinitionId() : base()
        {
        }

        public SDBeesMappedEntityDefinitionId(string id) : base(id)
        {
        }

        public static bool operator ==(SDBeesMappedEntityDefinitionId id1, SDBeesMappedEntityDefinitionId id2)
        {
            if (((object)id1 == null) == ((object)id2 == null))
            {
                if ((object)id1 != null)
                {
                    return id1.Id == id2.Id;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static bool operator !=(SDBeesMappedEntityDefinitionId id1, SDBeesMappedEntityDefinitionId id2)
        {
            return !(id1 == id2);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            SDBeesMappedEntityDefinitionId id = obj as SDBeesMappedEntityDefinitionId;
            if (id == null)
            {
                return false;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator SDBeesMappedEntityDefinitionId(string idText)
        {
            SDBeesMappedEntityDefinitionId id = new SDBeesMappedEntityDefinitionId(idText);
            return id;
        }

    }
}
