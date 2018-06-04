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

	/// <remarks>
	/// Definition einer Eigenschaft
	/// </remarks>
	[DataContract]
	public class SDBeesPropertyDefinition
	{
        public SDBeesPropertyDefinition()
        {
            m_Id = new SDBeesPropertyDefinitionId();
            m_name = new SDBeesLabel();
            m_propertyType = SDBeesPropertyValueType.Text;
        }

        public SDBeesPropertyDefinition(SDBeesPropertyDefinitionId id, SDBeesLabel name, SDBeesPropertyType propType)
        {
            m_Id = id;
            m_name = name;
            m_propertyType = propType;
        }

        public SDBeesPropertyDefinition(string id, string name, string propertyTypeConverter, string propertyUiTypeEditor, bool browsable, bool editable)
        {
            m_Id = id;
            m_name = name;
            m_propertyType = SDBeesPropertyValueType.Text;
            m_propertyTypeConverter = propertyTypeConverter;
            m_propertyUiTypeEditor = propertyUiTypeEditor;
            m_Browsable = browsable;
            m_Editable = editable;
        }

		private SDBeesPropertyDefinitionId m_Id;
        /// <summary>
        /// Die Id der zu prüfenden Eigenschaft
        /// </summary>
        [DataMember]
		public SDBeesPropertyDefinitionId Id
		{
            get { return m_Id; }
            set { m_Id = value; }
		}

		private SDBeesLabel m_name;
        /// <summary>
        /// Name des Filters
        /// </summary>
        [DataMember]
		public SDBeesLabel Name
		{
            get { return m_name; }
            set { m_name = value; }
		}

        SDBeesPropertyType m_propertyType;
        /// <summary>
        /// Datentyp der Eigenschaft
        /// </summary>
        [DataMember]
        public SDBeesPropertyType PropertyType
		{
            get { return m_propertyType; }
            set { m_propertyType = value; }
		}

        string m_propertyUiTypeEditor;
        /// <summary>
        /// UiTypeEditor for this Property
        /// </summary>
        [DataMember]
        public string PropertyUiTypeEditor
        {
            get { return m_propertyUiTypeEditor; }
            set { m_propertyUiTypeEditor = value; }
        }

        string m_propertyTypeConverter;
        /// <summary>
        /// TypeConverter for this Property
        /// </summary>
        [DataMember]
        public string PropertyTypeConverter
        {
            get { return m_propertyTypeConverter; }
            set { m_propertyTypeConverter = value; }
        }

        bool m_Editable = false;
        /// <summary>
        /// Is Property Editable?
        /// </summary>
        [DataMember]
        public bool Editable
        {
            get { return m_Editable; }
            set { m_Editable = value; }
        }

        bool m_Browsable = false;
        /// <summary>
        /// Is Property Browsable?
        /// </summary>
        [DataMember]
        public bool Browsable
        {
            get { return m_Browsable; }
            set { m_Browsable = value; } 
        }
    }
}

