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
    /// <remarks>
	/// Definition einer Eigenschaft
	/// </remarks>
	[DataContract]
	public class SDBeesPropertyDefinition
	{
        public SDBeesPropertyDefinition()
        {
            Id = new SDBeesPropertyDefinitionId();
            Name = new SDBeesLabel();
            PropertyType = SDBeesPropertyValueType.Text;
        }

        public SDBeesPropertyDefinition(SDBeesPropertyDefinitionId id, SDBeesLabel name, SDBeesPropertyType propType)
        {
            Id = id;
            Name = name;
            PropertyType = propType;
        }

        public SDBeesPropertyDefinition(string id, string name, string propertyTypeConverter, string propertyUiTypeEditor, bool browsable, bool editable)
        {
            Id = id;
            Name = name;
            PropertyType = SDBeesPropertyValueType.Text;
            PropertyTypeConverter = propertyTypeConverter;
            PropertyUiTypeEditor = propertyUiTypeEditor;
            Browsable = browsable;
            Editable = editable;
        }

	    /// <summary>
        /// Die Id der zu prüfenden Eigenschaft
        /// </summary>
        [DataMember]
		public SDBeesPropertyDefinitionId Id { get; set; }

	    /// <summary>
        /// Name des Filters
        /// </summary>
        [DataMember]
		public SDBeesLabel Name { get; set; }

	    /// <summary>
        /// Datentyp der Eigenschaft
        /// </summary>
        [DataMember]
        public SDBeesPropertyType PropertyType { get; set; }

	    /// <summary>
        /// UiTypeEditor for this Property
        /// </summary>
        [DataMember]
        public string PropertyUiTypeEditor { get; set; }

	    /// <summary>
        /// TypeConverter for this Property
        /// </summary>
        [DataMember]
        public string PropertyTypeConverter { get; set; }

	    /// <summary>
        /// Is Property Editable?
        /// </summary>
        [DataMember]
        public bool Editable { get; set; }

	    /// <summary>
        /// Is Property Browsable?
        /// </summary>
        [DataMember]
        public bool Browsable { get; set; }
	}
}

