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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SDBees.DB
{
    /// <summary>
    /// Database class to wrap different SQL Server vendors. With this class it is possible
    /// to open, read and manipulate SQL databases of different SQL server vendors using
    /// the same code.
    /// </summary>
    public abstract class Database
    {
        #region Private Data Members

        private string mName;
        private Server mServer;
        private string mUser;
        private string mPassword;
		private string mPort;
        private string mDescription;
        private int mMajorVersion;
        private int mMinorVersion;
        private Connection mConnection;
        private int mOpenConnectionCount;

        // Optional caching of table names for improved performance
        private Hashtable mTableNameCache;
        private bool mUseTableNameCaching;
        private bool mUseGlobalCaching;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of the database
        /// </summary>
        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        /// <summary>
        /// Port to communicate with the server
        /// </summary>
		public string Port
		{
			get
			{
				return mPort;
			}
			set
			{
				mPort = value;
			}
		}

        /// <summary>
        /// SQL Server wrapper
        /// </summary>
        public Server Server
        {
            get
            {
                return mServer;
            }
            set
            {
                mServer = value;
            }
        }

        /// <summary>
        /// The current logged in user
        /// </summary>
        public string User
        {
            get
            {
                return mUser;
            }
            set
            {
                mUser = value;
            }
        }

        /// <summary>
        /// The password of the current logged in user
        /// </summary>
        public string Password
        {
            get
            {
                return mPassword;
            }
            set
            {
                mPassword = value;
            }
        }

        /// <summary>
        /// Description of this database
        /// </summary>
        public string Description
        {
            get
            {
                return mDescription;
            }
            set
            {
                mDescription = value;
            }
        }

        /// <summary>
        /// Major version of the database. This version information can be used by application
        /// to reject opening a database with the wrong version of the product or automatically
        /// migrate the database to a new version.
        /// </summary>
        public int MajorVersion
        {
            get
            {
                return mMajorVersion;
            }
            set
            {
                mMajorVersion = value;
            }
        }

        /// <summary>
        /// Minor version of the database. This version information can be used by application
        /// to reject opening a database with the wrong version of the product or automatically
        /// migrate the database to a new version.
        /// </summary>
        public int MinorVersion
        {
            get
            {
                return mMinorVersion;
            }
            set
            {
                mMinorVersion = value;
            }
        }

        /// <summary>
        /// The current connection, null if not open
        /// </summary>
        public Connection Connection
        {
            get
            {
                return mConnection;
            }
        }


        /// <summary>
        /// Get or Set the database to cache the tablenames
        /// </summary>
        public bool UseTableNameCaching
        {
            get { return mUseTableNameCaching; }
            set
            {
                if (mUseTableNameCaching != value)
                {
                    mUseTableNameCaching = value;

                    // reset the cache...
                    mTableNameCache = null;
                }
            }
        }

        /// <summary>
        /// Get or Set the database to use global caching
        /// </summary>
        public bool UseGlobalCaching
        {
            get { return mUseGlobalCaching; }
            set
            {
                if (mUseGlobalCaching != value)
                {
                    mUseGlobalCaching = value;
                }
            }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Database ()
            : this("unknown", null)
        {
        }

        /// <summary>
        /// Standard constructor with parameters
        /// </summary>
        /// <param name="name">Name of the database</param>
        /// <param name="server">Server for this database</param>
        public Database(string name, Server server)
        {
            mName = name;
            mServer = server;
            mUser = "";
            mPassword = "";
            mDescription = "";
            mMajorVersion = -1;
            mMinorVersion = -1;
            mConnection = null;
            mOpenConnectionCount = 0;

            mTableNameCache = null;
            mUseTableNameCaching = true;
            mUseGlobalCaching = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// open the database connection... This works with a reference counter so for each open
        /// there must be a call to close. Multiple opens will only change the reference counter
        /// and therefore are fast.
        /// </summary>
        /// <param name="bReadOnly">If true the database will only be opened for read. Not supported by all vendors.</param>
        /// <param name="error">If this fails this will be set appropriately</param>
        /// <returns></returns>
        public Connection Open(bool bReadOnly, ref Error error)
        {
            if (mConnection == null)
            {
                mConnection = CreateConnection(bReadOnly, ref error);
                if (mConnection != null)
                {
                    mOpenConnectionCount = 1;
                }
                else
                {
                    mOpenConnectionCount = 0;
                }
            }
            else
            {
                mOpenConnectionCount++;
            }

            // Update database version information only once...
            if ((mMajorVersion < 0) && (mConnection != null))
            {
                // Get the version information
                GetVersion(ref mDescription, ref mMajorVersion, ref mMinorVersion);

            }

            return mConnection;
        }

        /// <summary>
        /// Close the connection to the database or just reduce the reference counter if multiply opened.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public int Close(ref Error error)
        {
            if (mOpenConnectionCount > 1)
            {
                // we have multiple open connections... just reduce the counter
                mOpenConnectionCount--;
            }
            else if (mOpenConnectionCount == 1)
            {
                // this is the last open connection, now really close it...
                mConnection.Close(ref error);
                mConnection = null;
                mOpenConnectionCount = 0;
            }
            else
            {
                // Display error...
            }

            return mOpenConnectionCount;
        }

        // table manipulation

        /// <summary>
        /// Get all the table names of this database
        /// </summary>
        /// <param name="names"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public virtual int TableNames(ref ArrayList names, ref Error error) // Overridden in SQLiteDatabase!
        {
            int numTables = 0;

            // could be recursive... think about a solution...
            // UpdateTableNameCache(ref error);

            if (mUseTableNameCaching && (mTableNameCache != null))
            {
                names = new ArrayList();

                foreach (DictionaryEntry entry in mTableNameCache)
                {
                    string name = (string)entry.Value;
                    names.Add(name);
                }

                numTables = names.Count;
            }
            else
            {
                string criteria = "(TABLE_NAME NOT LIKE 'sys%') AND (TABLE_NAME <> 'dtproperties') AND (TABLE_SCHEMA = '" + this.Name + "')";

                numTables = GetRowValues(ref names, "INFORMATION_SCHEMA.Tables", "TABLE_NAME", criteria, false, true, 0, ref error);
            }


            return numTables;
        }

        /// <summary>
        /// Create a table in this database
        /// </summary>
        /// <param name="table">Table to create</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public virtual bool CreateTable(Table table, ref Error error)
        {
            Connection conn = Open(false, ref error);

            bool success = false;

            // For now this will stop the caching...
            UseTableNameCaching = false;

            if (conn != null)
            {
                success = conn.CreateTable(table, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Update/modify a table
        /// </summary>
        /// <param name="table">New or updated table</param>
        /// <param name="oldTable">Table to replace</param>
        /// <param name="error">Contains information if this fails</param>
        /// <returns></returns>
        public virtual bool UpdateTable(Table table, Table oldTable, ref Error error)
        {
            Connection conn = Open(false, ref error);

            bool success = false;

            if (conn != null)
            {
                success = conn.UpdateTable(table, oldTable, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Read a table from the database
        /// </summary>
        /// <param name="tableName">Name of the table in this database</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public virtual Table ReadTable(string tableName, ref Error error)
        {
            Connection conn = Open(false, ref error);

            Table table = null;

            if (conn != null)
            {
                table = conn.ReadTable(tableName, ref error);

                Close(ref error);
            }

            return table;
        }

        /// <summary>
        /// Rename a column in a table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="oldColumnName">current column name to change</param>
        /// <param name="newColumnName">new column name to change to</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>True if successful</returns>
        public virtual bool RenameColumn(string tableName, string oldColumnName, string newColumnName, ref Error error)
        {
            bool success = false;

            Connection conn = Open(false, ref error);

            if (conn != null)
            {
                success = conn.RenameColumn(tableName, oldColumnName, newColumnName, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Delete a column from the database
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="columnName">Name of the column</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>True if successful</returns>
        public virtual bool EraseColumn(string tableName, string columnName, ref Error error)
        {
            bool success = false;

            Connection conn = Open(false, ref error);

            if (conn != null)
            {
                success = conn.EraseColumn(tableName, columnName, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Get the column names for a table in the database
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnNames"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public virtual int GetTableColumns(string tableName, out List<string> columnNames, ref Error error)
        {
            Connection conn = Open(false, ref error);

            columnNames = null;
            int numColumns = 0;

            if (conn != null)
            {
                numColumns = conn.GetTableColumns(tableName, out columnNames, ref error);

                Close(ref error);
            }

            return numColumns;
        }

        /// <summary>
        /// Delete a table from this database
        /// </summary>
        /// <param name="tableName">Name of the table to delete</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public virtual bool DeleteTable(string tableName, ref Error error)
        {
            Connection conn = Open(false, ref error);

            bool success = false;

            // For now this will stop the caching...
            UseTableNameCaching = false;

            if (conn != null)
            {
                success = conn.DeleteTable(tableName, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Check if a table exits in this database
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public virtual bool TableExists(string tableName, ref Error error)
        {
            bool success = false;

            if (mUseTableNameCaching)
            {
                Error cacheError = null;
                UpdateTableNameCache(ref cacheError);

                if ((cacheError == null) && (mTableNameCache != null))
                {
                    return mTableNameCache.ContainsKey(tableName.ToLower());
                }

                Error.Display("ERROR in table name cache!", cacheError);
            }
            Connection conn = Open(false, ref error);

            if (conn != null)
            {
                success = conn.TableExists(tableName, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Update the cached list of tables...
        /// </summary>
        /// <param name="error"></param>
        protected void UpdateTableNameCache(ref Error error)
        {
            if (mUseTableNameCaching && (mTableNameCache == null))
            {
                ArrayList names = null;
                TableNames(ref names, ref error);

                if (error == null)
                {
                    mTableNameCache = new Hashtable();

                    foreach (string name in names)
                    {
                        mTableNameCache[name.ToLower()] = name;
                    }
                }
            }
        }

        // row manipulation

        /// <summary>
        /// Create a row for a specific table
        /// </summary>
        /// <param name="table">Table to modify</param>
        /// <param name="idColumn">The id column</param>
        /// <param name="uniqueColumns">The unique columns (required for reload when automatic ids are set)</param>
        /// <param name="attributes">Attributes of this row</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public object CreateRow(Table table, Column idColumn, Columns uniqueColumns, Attributes attributes, ref Error error)
        {
            object id = null;

            Connection conn = Open(true, ref error);

            if (conn != null)
            {
                id = conn.CreateRow(table, idColumn, uniqueColumns, attributes, ref error);

                Close(ref error);
            }

            return id;
        }

        /// <summary>
        /// Update a row in a table
        /// </summary>
        /// <param name="table">Table to modify</param>
        /// <param name="idColumn">Id column of this table</param>
        /// <param name="id">Id of the row to update</param>
        /// <param name="attributes">Attributes to set/update</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public bool UpdateRow(Table table, Column idColumn, object id, Attributes attributes, ref Error error)
        {
            bool success = false;

            Connection conn = Open(true, ref error);

            if (conn != null)
            {
                success = conn.UpdateRow(table, idColumn, id, attributes, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Load/read a row from a table in this database
        /// </summary>
        /// <param name="table">Table to read a row from</param>
        /// <param name="idColumn">id column of this table</param>
        /// <param name="id">id of the row to read</param>
        /// <param name="attributes">Attributes read form the row</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public bool LoadRow(Table table, Column idColumn, object id, ref Attributes attributes, ref Error error)
        {
            bool success = false;

            Connection conn = Open(true, ref error);

            if (conn != null)
            {
                success = conn.LoadRow(table, idColumn, id, ref attributes, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Erase a row from a table in this database
        /// </summary>
        /// <param name="table">Table to modify</param>
        /// <param name="criteria">SQL criteria for the selection (use FormatCriteria to be vendor independant)</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public bool EraseRow(Table table, string criteria, ref Error error)
        {
            bool success = false;

            Connection conn = Open(true, ref error);

            if (conn != null)
            {
                success = conn.EraseRow(table, criteria, ref error);

                Close(ref error);
            }

            return success;
        }

        // selection

        /// <summary>
        /// Select all values of a certain column from a table in this database
        /// </summary>
        /// <param name="table">Table to select from</param>
        /// <param name="searchColumn">Column name</param>
        /// <param name="values">Values returned</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public int Select(Table table, string searchColumn, ref ArrayList values, ref Error error)
        {
            return GetRowValues(ref values, table.Name, searchColumn, "", true, true, 0, ref error);
        }

        /// <summary>
        /// Select filtered values of a certain column from a table in this database
        /// </summary>
        /// <param name="table">Table to select from</param>
        /// <param name="searchColumn">Column name</param>
        /// <param name="criteria">SQL criteria for the selection (use FormatCriteria to be vendor independant)</param>
        /// <param name="values">Values returned</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public int Select(Table table, string searchColumn, string criteria, ref ArrayList values, ref Error error)
        {
            return GetRowValues(ref values, table.Name, searchColumn, criteria, true, true, 0, ref error);
        }

        /// <summary>
        /// Select filtered values of a certain column from a table in this database
        /// </summary>
        /// <param name="tableName">Tablename to select from</param>
        /// <param name="searchColumn">Column name</param>
        /// <param name="criteria">SQL criteria for the selection (use FormatCriteria to be vendor independant)</param>
        /// <param name="values">Values returned</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public int Select(string tableName, string searchColumn, string criteria, ref ArrayList values, ref Error error)
        {
            return GetRowValues(ref values, tableName, searchColumn, criteria, true, true, 0, ref error);
        }

        /// <summary>
        /// Select values of a certain column from a table in this database
        /// </summary>
        /// <param name="tableName">Tablename to select from</param>
        /// <param name="searchColumn">Column name</param>
        /// <param name="values">Values returned</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public int Select(string tableName, string searchColumn, ref ArrayList values, ref Error error)
        {
            return GetRowValues(ref values, tableName, searchColumn, "", true, true, 0, ref error);
        }

        /// <summary>
        /// Select filtered value of a certain column from a table in this database
        /// </summary>
        /// <param name="tableName">Tablename to select from</param>
        /// <param name="searchColumn">Column name</param>
        /// <param name="criteria">SQL criteria for the selection (use FormatCriteria to be vendor independant)</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Found object or null if fails</returns>
        public object SelectSingle(string tableName, string searchColumn, string criteria, ref Error error)
        {
            ArrayList values = null;
            int numFound = GetRowValues(ref values, tableName, searchColumn, criteria, true, true, 0, ref error);
            if (numFound == 1)
            {
                return values[0];
            }

            error = new Error("SelectSingle matches " + numFound + " entries!", 9999, this.GetType(), error);
            return null;
        }

        /// <summary>
        /// Format search criteria to match the SQL syntax of the current SQL vendor
        /// </summary>
        /// <param name="attribute">Attribute to use</param>
        /// <param name="operation">Comparison operation</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Formated criteria string to use in SQL queries</returns>
        public string FormatCriteria(Attribute attribute, DbBinaryOperator operation, ref Error error)
        {
            string criteria = "";

            Connection conn = Open(true, ref error);

            if (conn != null)
            {
                criteria = conn.FormatCriteria(attribute, operation, ref error);

                Close(ref error);
            }

            return criteria;
        }

        /// <summary>
        /// Combine SQL criterias to match the SQL syntax of the current SQL vendor
        /// </summary>
        /// <param name="criteria1">First formatted criteria</param>
        /// <param name="criteria2">Second formatted criteria</param>
        /// <param name="operation">Operation for combination</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Formated criteria string to use in SQL queries</returns>
        public string FormatCriteria(string criteria1, string criteria2, DbBooleanOperator operation, ref Error error)
        {
            string criteria = "";

            Connection conn = Open(true, ref error);

            if (conn != null)
            {
                criteria = conn.FormatCriteria(criteria1, criteria2, operation, ref error);

                Close(ref error);
            }

            return criteria;
        }

        /// <summary>
        /// Combine SQL criterias to match the SQL syntax of the current SQL vendor
        /// </summary>
        /// <param name="criterias">formatted criterias as strings</param>
        /// <param name="operation">Operation for combination</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Formated criteria string to use in SQL queries</returns>
        public string FormatCriteria(ArrayList criterias, DbBooleanOperator operation, ref Error error)
        {
            string criteria = "";

            Connection conn = Open(true, ref error);

            if (conn != null)
            {
                criteria = conn.FormatCriteria(criterias, operation, ref error);

                Close(ref error);
            }

            return criteria;
        }

        /// <summary>
        /// Execute an SQL command for this database on this server
        /// </summary>
        /// <param name="cmdString">SQL command</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public bool ExecuteCommand(string cmdString, ref Error error)
        {
            Connection conn = Open(false, ref error);

            bool success = false;

            if (conn != null)
            {
                success = conn.ExecuteCommand(cmdString, ref error);

                Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Replaces invalid characters by valid characters
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns>A valid column name as similar as the given name as possible</returns>
        public virtual string MakeValidColumnName(string columnName)
        {
            string result = columnName;

            result = result.Replace("Ä", "AE");
            result = result.Replace("Ö", "OE");
            result = result.Replace("Ü", "UE");
            result = result.Replace("ä", "ae");
            result = result.Replace("ö", "oe");
            result = result.Replace("ü", "ue");
            result = result.Replace("ß", "ss");
            result = result.Replace(".", "_");
            result = result.Replace(" ", "_");
            result = result.Replace("?", "_");
            result = result.Replace("&", "_");
            result = result.Replace("/", "_");
            result = result.Replace("\\", "_");
            result = result.Replace("\"", "_");
            result = result.Replace("(", "_");
            result = result.Replace(")", "_");
            result = result.Replace("[", "_");
            result = result.Replace("]", "_");
            result = result.Replace("{", "_");
            result = result.Replace("}", "_");
            result = result.Replace("=", "_");
            result = result.Replace("%", "_");
            result = result.Replace("$", "_");
            result = result.Replace("§", "_");
            result = result.Replace("!", "_");
            result = result.Replace("*", "_");
            result = result.Replace("+", "_");
            result = result.Replace("~", "_");
            result = result.Replace("#", "_");
            result = result.Replace("'", "_");
            result = result.Replace(":", "_");
            result = result.Replace(";", "_");
            result = result.Replace(",", "_");
            result = result.Replace("|", "_");
            result = result.Replace("<", "_");
            result = result.Replace(">", "_");
            result = result.Replace("^", "_");
            result = result.Replace("°", "_");

            return result;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// This method should be overloaded in derived class
        /// </summary>
        /// <param name="bReadOnly"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        protected abstract Connection CreateConnection(bool bReadOnly, ref Error error);

        /// <summary>
        /// get description and version information from the database (not implemented yet)
        /// </summary>
        /// <param name="description"></param>
        /// <param name="majorVersion"></param>
        /// <param name="minorVersion"></param>
        /// <returns></returns>
        protected virtual bool GetVersion(ref string description, ref int majorVersion, ref int minorVersion)
        {
            bool foundVersion = false;

            description = mName + " (" + mServer.Name + ")";
            majorVersion = 1;
            minorVersion = 0;

            return foundVersion;
        }

        /// <summary>
        /// Get filtered values for a specific column from a table in this database
        /// </summary>
        /// <param name="values">The filtered values are returned here</param>
        /// <param name="tableName">Name of the table to search</param>
        /// <param name="columnName">Name of the column</param>
        /// <param name="criteria">SQL formatted criteria (use FormatCriteria to be vendor independent)</param>
        /// <param name="allowMultiple">If false, multiple values will be eliminated</param>
        /// <param name="clearFirst">If false the values will not be deleted first</param>
        /// <param name="topCount">If greater than 0, only this number of entries will be returned</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of values returned</returns>
        protected int GetRowValues(ref ArrayList values, string tableName, string columnName, string criteria, bool allowMultiple,
                        bool clearFirst, int topCount, ref Error error)
        {
            int numTables = 0;

            Connection conn = Open(true, ref error);

            if (conn != null)
            {
                numTables = conn.GetRowValues(ref values, tableName, columnName, criteria, allowMultiple, clearFirst, topCount, ref error);

                Close(ref error);
            }

            return numTables;
        }


        #endregion

    }
}
