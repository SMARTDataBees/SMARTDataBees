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
using System.Xml;

namespace SDBees.DB
{
    /// <summary>
    /// Class wrapping a database table
    /// </summary>
    public class Table
    {
        #region Private Data Members

        private string mName;
        private Columns mColumns;
        private string mPrimaryKey;
        private bool mReloadIdOnInsert;
        private int mSchemaVersion;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of the table as stored in the database
        /// </summary>
        public string Name 
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Collection of columns in this table
        /// </summary>
        public Columns Columns
        {
            get { return mColumns; }
            set { mColumns = value; }
        }

        /// <summary>
        /// Set or get the name of the primary key of this table
        /// </summary>
        public string PrimaryKey
        {
            get { return mPrimaryKey; }
            set { mPrimaryKey = value; }
        }

        /// <summary>
        /// Set or get ReloadOnInsert. This flag is used to get the id determined by the SQL server
        /// when automatic ids are specified.
        /// </summary>
        public bool ReloadIdOnInsert
        {
            get { return mReloadIdOnInsert; }
            set { mReloadIdOnInsert = value; }
        }

        /// <summary>
        /// Set or get the version of this schema
        /// </summary>
        public int SchemaVersion
        {
            get { return mSchemaVersion; }
            set { mSchemaVersion = value; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Table()
        {
            Init();
        }

        /// <summary>
        /// Standard constructor passing the name as parameter
        /// </summary>
        /// <param name="name"></param>
        public Table(string name)
        {
            Init();
            mName = name;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create an XML string from from the definition of this table
        /// </summary>
        /// <returns>XML description</returns>
        public string writeXml()
        {
            var strXml = "";

            // create the XML structure and consider all the columns
            var settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.IndentChars = ("  ");
            var xmlStringBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(xmlStringBuilder, settings))
            {
                // Write XML data.
                writer.WriteStartElement("TableSchema");
                writer.WriteAttributeString("Name", mName);
                writer.WriteAttributeString("PrimaryKey", mPrimaryKey);
                writer.WriteAttributeString("ReloadOnInsert", mReloadIdOnInsert.ToString());
                writer.WriteAttributeString("SchemaVersion", mSchemaVersion.ToString());

                // Now write the columns
                writer.WriteStartElement("Columns");

                foreach (var iterator in mColumns)
                {
                    var column = iterator.Value;
                    var xmlColumn = column.Xmlwrite();

                    if (xmlColumn != "")
                    {
                        var xmlDocument = new XmlDocument();
                        xmlDocument.LoadXml(xmlColumn);
                        var xmlNode = xmlDocument.SelectSingleNode("Column");

                        writer.WriteNode(new XmlNodeReader(xmlNode), false);
                    }
                }

                writer.WriteEndElement(); // Columns

                writer.WriteEndElement(); // TableSchema
                writer.Flush();

                strXml = xmlStringBuilder.ToString();
            }


            return strXml;
        }

        /// <summary>
        /// Redefine the table properties and columns from an XML description
        /// </summary>
        /// <param name="xmlContent">XML description of a table and it's columns</param>
        public void readXml(string xmlContent)
        {
            // remove all columns and other information...
            Init();

            // Interpret the xml and create the necessary columns
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var rootNode = xmlDoc.SelectSingleNode("TableSchema");
            XmlReader reader = new XmlNodeReader(rootNode);

            reader.Read();
            mName = reader.GetAttribute("Name");
            mPrimaryKey = reader.GetAttribute("PrimaryKey");
            var strVersion = reader.GetAttribute("SchemaVersion");
            if ((strVersion == null) || (strVersion == ""))
            {
                mSchemaVersion = 100;
            }
            else 
            {
                mSchemaVersion = (int)Convert.ChangeType(strVersion, typeof(int));
            }

            mReloadIdOnInsert = Convert.ToBoolean(reader.GetAttribute("ReloadOnInsert"));

            // Find the columns and read them...
            var columnsNodeList = rootNode.SelectSingleNode("Columns").SelectNodes("Column");
            foreach (XmlNode columnNode in columnsNodeList)
            {
                var newColumn = new Column();
                newColumn.Xmlread(columnNode.OuterXml);

                mColumns.Add(newColumn);
            }
        }

        #endregion

        #region Protected Methods

        private void Init()
        {
            mName = "";
            mColumns = new Columns();
            mPrimaryKey = "";
            mReloadIdOnInsert = false;
            mSchemaVersion = 100;
        }
        #endregion
    }
}
