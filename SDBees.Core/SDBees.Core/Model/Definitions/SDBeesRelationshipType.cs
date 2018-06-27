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
    /// Der Typ der Entity Beziehung
    /// </summary>
    [DataContract]
    public class SDBeesRelationshipType
    {
        public SDBeesRelationshipType()
        {
            m_type = "";
        }

        public SDBeesRelationshipType(string relType)
        {
            m_type = relType;
        }


        private string m_type;
        /// <summary>
        /// Der Beziehungstyp als String
        /// </summary>
        [DataMember]
        public string TypeName
        {
            get { return m_type; }
            set { m_type = value; }
        }
    }
}
