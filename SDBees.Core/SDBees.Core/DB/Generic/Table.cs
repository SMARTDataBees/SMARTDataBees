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
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SDBees.DB
{
    /// <summary>
    /// Class wrapping a database table
    /// </summary>
    [XmlRoot("TableSchema")]
    public class Table
    {
        #region Private Data Members

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of the table as stored in the database
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Collection of columns in this table
        /// </summary>
        public List<Column> Columns { get; set; }

        /// <summary>
        /// Set or get the name of the primary key of this table
        /// </summary>
        [XmlAttribute("PrimaryKey")]
        public string PrimaryKey { get; set; }

        /// <summary>
        /// Set or get ReloadOnInsert. This flag is used to get the id determined by the SQL server
        /// when automatic ids are specified.
        /// </summary>
        [XmlIgnore]
        public bool ReloadIdOnInsert { get; set; }

        /// <summary>
        /// Xml helper value for the serializer
        /// </summary>
        [XmlAttribute("ReloadOnInsert")]
        public string ReloadOnInsert
        {
            get => ReloadIdOnInsert ? "True" : "False";
            set => ReloadIdOnInsert = string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) != 0;
        }

        /// <summary>
        /// Set or get the version of this schema
        /// </summary>
        [XmlAttribute("SchemaVersion"), DefaultValue(100)]
        public int SchemaVersion { get; set; }

        #endregion


        /// <summary>
        /// Standard constructor
        /// </summary>
        public Table() 
        {
            Name = "";
            Columns = new List<Column>();
            PrimaryKey = "";
            ReloadIdOnInsert = false;
            SchemaVersion = 100;
        }

        /// <summary>
        /// Standard constructor passing the name as parameter
        /// </summary>
        /// <param name="name"></param>
        public Table(string name) : this()
        {
            Name = name;
        }

    }


}
