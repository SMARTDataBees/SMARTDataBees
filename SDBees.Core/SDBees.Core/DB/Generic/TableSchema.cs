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

namespace SDBees.DB
{
    /// <summary>
    /// Class of persistent object that describes the schema of an object
    /// </summary>
    public class TableSchema : Object
    {
        #region Private Data Members

        // Persistent Table definition for this class
        private static Table gTable;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of this schema object. This is the primary key in the database and
        /// represent the table name of the described object
        /// </summary>
        public string Name
        {
            get { return Id.ToString(); }
            set { Id = value; }
        }

        /// <summary>
        /// XML description of this object
        /// </summary>
        public string XmlSchema
        {
            get { return (string) GetPropertyByColumn("xmlschema"); }
            set { SetPropertyByColumn("xmlschema", value); }
        }

        public override string GetTableName
        {
            get { return "usrTableSchema"; }
        }


        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public TableSchema()
        {
            Table = gTable;
        }

        /// <summary>
        /// Standard constructor with parameters
        /// </summary>
        /// <param name="tableName">Table name of the object (Name property)</param>
        public TableSchema(string tableName)
        {
            Id = tableName;
            Table = gTable;
        }

        /// <summary>
        /// Standard constructor with paramaters
        /// </summary>
        /// <param name="tableName">Table name of the object (Name property)</param>
        /// <param name="xmlSchema">XML description of schema (XmlSchema property)</param>
        public TableSchema(string tableName, string xmlSchema)
        {
            Id = tableName;
            XmlSchema = xmlSchema;
            Table = gTable;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Finds and loads a TableSchema object
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="tableName">Name of the table the schema should describe</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>TableSchema object or null if not found</returns>
        public static TableSchema FindSchema(Database database, string tableName, ref Error error)
        {
            var schema = new TableSchema();
            if (!schema.Load(database, tableName, ref error))
            {
                schema = null;
            }

            return schema;
        }

        /// <summary>
        /// Static method to create/initialize the Table containing all the TableSchema objects
        /// </summary>
        /// <param name="database">Database</param>
        public static void InitTableSchema(Database database)
        {
            var schema = new TableSchema();
            schema.InitTableSchema(ref gTable, database);
        }

        #endregion

        #region Protected Methods

        /*
        protected override string TableName()
        {
            return "usrTableSchema";
        }
         * */
        protected override Table CreateTableSchema(Database database)
        {
            var table = new Table();

            table.Name = TableName();

            var column = new Column();
            column.Name = "tablename";
            column.Type = DbType.String;
            column.Size = 80;
            column.Flags = 0;
            table.Columns.Add(column);
            table.PrimaryKey = column.Name;
            table.ReloadIdOnInsert = false;

            column = new Column();
            column.Name = "xmlschema";
            column.Type = DbType.Text;
            table.Columns.Add(column);

            return table;
        }

        #endregion
    }
}
