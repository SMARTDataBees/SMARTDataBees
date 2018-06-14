// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2007 by
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

namespace SDBees.Plugs.Properties
{
    /// <summary>
    /// Represents a single property in a PropertySpec that has a list of selection values
    /// </summary>
    public class PropertySpecListbox : PropertySpec
    {
        private List<string> mSelectionList;

        public List<string> SelectionList
        {
            get { return mSelectionList; }
            set { mSelectionList = value; }
        }

        public PropertySpecListbox(string name, string type)
            :  this(name, type, null, null, null)
        {

        }
        public PropertySpecListbox(string name, Type type, string category, string description, object defaultValue)
            : base(name, type, category, description, defaultValue, typeof(DropDownUITypeEditor), "")
        {

        }
        public PropertySpecListbox(string name, string type, string category, string description, object defaultValue)
            : base(name, type, category, description, defaultValue, typeof(DropDownUITypeEditor), "")
        {

        }
        public PropertySpecListbox(string name, Type type, string category, string description, object defaultValue, List<string> list)
            : base(name, type, category, description, defaultValue, typeof(DropDownUITypeEditor), "")
        {
            mSelectionList = list;
        }

    }
}
