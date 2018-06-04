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
using System.Text;

namespace SDBees.DB
{
    /// <summary>
    /// Attribute is used by persistent objects for saving restoring and represents a value
    /// of a certain column.
    /// </summary>
    public class Attribute
    {
        #region Private Data Members

        private Column mColumn;
        private object mValue;

        #endregion

        #region Public Properties

        /// <summary>
        /// The column this attribute reference
        /// </summary>
        public Column Column
        {
            get { return mColumn; }
            set { mColumn = value; }
        }

        /// <summary>
        /// The value of this attribute
        /// </summary>
        public object Value
        {
            get { return mValue; }
            set { mValue = value; }
        }
        
        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Attribute()
        {
            mColumn = null;
            mValue = null;
        }

        /// <summary>
        /// Standard constructor with parameters
        /// </summary>
        /// <param name="column">The column this attribute references</param>
        /// <param name="value">The value of this attribute</param>
        public Attribute(Column column, object value)
        {
            mColumn = column;
            mValue = value;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        #endregion
    }

    /// <summary>
    /// Collection of attribute deriving from Dictionary, so that direct access
    /// through the column name is possible
    /// </summary>
    public class Attributes : Dictionary<string, Attribute>
    {
        /// <summary>
        /// Add an attribute to the collection
        /// </summary>
        /// <param name="attribute">Attribute to add</param>
        public void Add(Attribute attribute)
        {
            Add(attribute.Column.Name, attribute);
        }
    };
}


