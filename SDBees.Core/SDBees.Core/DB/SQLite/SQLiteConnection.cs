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
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Carbon.Configuration;
using SDBees.Core.Global;
using SDBees.DB.Forms;
using SDBees.GuiTools;

namespace SDBees.DB.SQLite
{
    /// <summary>
    /// Class for wrapping a connection to a SQLight database
    /// </summary>
    public class SQLiteConnection : Connection
    {
        #region Private Data Members

        private System.Data.SQLite.SQLiteConnection mDbConnection;
        private SQLiteTransaction mDbTransaction;
        private int mPort;
        private static string[] SQLTypeLabels =
            {
                "Unknown",  // eUnknown
                "BINARY",   // eBinary
                "TINYINT",  // eBoolean
                "TINYINT",  // eByte
                "REAL",     // eCurrency
                "DATE",     // eDate
                "DATETIME", // eDateTime
                "DECIMAL",  // eDecimal
                "DOUBLE",   // eDouble
                "GUID",     // eGuid
                "SMALLINT", // eInt16
                "INT",      // eInt32
                "BIGINT",   // eInt64
                "REAL",     // eSingle
                "VARCHAR",  // eString
                "CHAR",     // eStringFixed
                "CHAR",     // eGuidString
                "TEXT",     // eText
                "LONGTEXT", // eLongText
                "VARCHAR"   // eCrossSize
            };

        private static string[] SQLBinaryOperatorLabels =
            {
                "Unknown",  // eUnknown
                "=",        // eIsEqual
                "<>",       // eIsDifferent
                ">",        // eIsGreaterThan
                ">=",       // eIsGreaterOrEqual
                "!>",       // eIsNotGreaterThan
                "<",        // eIsSmallerThan
                "<=",       // eIsSmallerOrEqual
                "!<",       // eIsNotSmallerThan
                "LIKE",     // eIsLike
                "NOT LIKE"  // eIsNotLike
            };

        private static string[] SQLBooleanOperatorLabels =
            {
                "Unknown",  // eUnknown
                "AND",      // eAnd
                "OR"       // eOr
            };

        private static string m_ConfigurationSection = "SQLite";
        private static XmlConfigurationCategory SDBeesSQLiteLocalConfiguration()
        {
            return SDBeesGlobalVars.SDBeesLocalUsersConfiguration().Categories[m_ConfigurationSection, true];
        }

        private static string m_SQLiteWAL = "Use WAL";
        private static bool m_SQLiteWALDefault = false;

        private void SetUpConfiguration()
        {
            var catLog = SDBeesSQLiteLocalConfiguration();

            var opSQLiteWAL = new XmlConfigurationOption(m_SQLiteWAL, m_SQLiteWALDefault);
            opSQLiteWAL.Description = "Connect to SQLite DB in WAL mode? Attention, this will be stored in the sqlite db and can't be reverted!";
            //opAnullarSpace.EditorAssemblyQualifiedName
            if (!catLog.Options.Contains(opSQLiteWAL))
                catLog.Options.Add(opSQLiteWAL);

        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Port to be used by the connection
        /// </summary>
        public int Port
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


        #endregion

        #region Contructor / Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="database"></param>
        public SQLiteConnection(Database database)
            : base(database)
        {
            mDbConnection = null;
            mDbTransaction = null;
            mPort = Convert.ToInt16(database.Port);

            SetUpConfiguration();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Open a connection to the SQLite database
        /// </summary>
        /// <param name="database">Database to be used for opening</param>
        /// <param name="bReadOnly">Not supported by this database vendor</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public override bool Open(Database database, bool bReadOnly, ref Error error)
        {
            var connectionString = ConnectionString(database, bReadOnly);

#if DEBUG
            if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out var logValue) && logValue)
                SDBeesDBConnection.Current.LogfileWriter.Writeline("Open", connectionString, "DB.Details");
#endif

            try
            {
                mDbConnection = new System.Data.SQLite.SQLiteConnection(connectionString);
                mDbConnection.Open();
                mDbTransaction = mDbConnection.BeginTransaction();
            }
            catch (SQLiteException ex)
            {

            }
            catch (Exception ex)
            {
                // Add this error to the list...
                var myError = new Error(ex.Message, 9999, GetType(), error);
                error = myError;
                mDbConnection = null;
            }

#if DEBUG
            var msg = "Success";
            if (mDbConnection == null)
            {
                msg = "Failed";
            }
            if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out logValue) && logValue)
                SDBeesDBConnection.Current.LogfileWriter.Writeline("Open", msg, "DB.Details");
#endif

            return (mDbConnection != null);
        }

        /// <summary>
        /// Close the connection to the MySQL database
        /// </summary>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public override bool Close(ref Error error)
        {
            if (mDbConnection != null)
            {

#if DEBUG
                if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out var logValue) && logValue)
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("Close", "Closing SQLight connection", "DB.Details");
#endif
                try
                {
                    if (mDbTransaction != null)
                    {
                        mDbTransaction.Commit();
                        mDbTransaction.Dispose();
                        mDbTransaction = null;
                    }
                    mDbConnection.Close();
                    mDbConnection.Dispose();
                    mDbConnection = null;
                }
                catch (SQLiteException ex)
                {
                    var myError = new Error(ex.Message, 9999, GetType(), error);
                    error = myError;
                    mDbConnection = null;
                }
                catch (Exception ex)
                {
                    var myError = new Error(ex.Message, 9999, GetType(), error);
                    error = myError;
                    mDbConnection = null;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Execute an SQL command in this MySQL connection
        /// </summary>
        /// <param name="cmdString">Command to execute, should match MySQL syntax</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public override bool ExecuteCommand(string cmdString, ref Error error)
        {
            var success = false;

            if (mDbConnection != null)
            {

#if DEBUG
                if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out var logValue) && logValue)
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("ExecuteCommand", cmdString, "DB.Details");
#endif

                // Execute the query
                var sqlCmd = new SQLiteCommand(cmdString, mDbConnection);
                // MySql doesn't support this method...
                // sqlCmd.CommandTimeout = 30;

                try
                {
                    var rowsAffected = sqlCmd.ExecuteNonQuery();

#if DEBUG
                    if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out logValue) && logValue)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("ExecuteCommand", "Success " + rowsAffected + " rows were affected.", "DB.Details");
#endif
                    success = true;
                }
                catch (Exception ex)
                {
                    var myError = new Error(ex.Message, 9999, GetType(), error);
                    error = myError;

#if DEBUG
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("ExecuteCommand", "Failed: " + ex.Message, "DB.Details");
#endif
                }
            }

            return success;
        }

        public override DataSet GetReadOnlyDataSet()
        {
            var _dtSet = new DataSet();
            Error _error = null;
            //this.Open(this,true, ref _error);
            var _lstTblNames = new ArrayList();
            Database.TableNames(ref _lstTblNames, ref _error);

            //this.DbConnection.
            foreach (string sTblName in _lstTblNames)
            {
                var sSelect = "SELECT * FROM `" + sTblName + "`";
                FillDataSet(sSelect, ref _dtSet, ref _error, sTblName);
            }

            return _dtSet;
        }

        public override DataTable GetReadOnlyDataTable(string sTablename)
        {
            var _dtTable = new DataTable();
            var _dtSet = new DataSet();
            Error _error = null;

            var sSelect = "SELECT * FROM `" + sTablename + "`";
            //string sSelect = "SELECT * FROM `" + this.Database.Name + "`.`" + sTablename + "`";

            FillDataSet(sSelect, ref _dtSet, ref _error, sTablename);

            foreach (DataTable tbl in _dtSet.Tables)
            {
                if (_dtSet.Tables.Contains(sTablename))
                {
                    _dtTable = _dtSet.Tables[sTablename];
                }
                else
                {
                    _dtTable = null;
                }

            }

            return _dtTable;
        }

        /// <summary>
        /// Check if a table exists in this database
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if it exists</returns>
        public override bool TableExists(string tableName, ref Error error)
        {
            if (mDbConnection != null)
            {
                var criteria = "(type = '" + "table" + "') AND (name = '" + tableName + "')";
                ArrayList names = null;
                var numTables = GetRowValues(ref names, "sqlite_master", "name", criteria, false, true, 1, ref error);

                return numTables > 0;
            }

            return false;
        }

        /// <summary>
        /// Modify a Table in this database
        /// </summary>
        /// <param name="table">New table</param>
        /// <param name="oldTable">Table to replace</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public override bool UpdateTable(Table table, Table oldTable, ref Error error)
        {
            var success = false;

            if (mDbConnection != null)
            {
                var addColumnQuery = "";
                var dropColumnQuery = "";

                // First check for new and modified columns...
                foreach (var column in table.Columns)
                {

                    var columnDefinition = GetColumnDefinition(column);


                    var clm = oldTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(column.Name));
                    if (clm == null)
                    {
                        // This is a new column...
                        if (addColumnQuery != "")
                        {
                            addColumnQuery += ", ";
                        }

                        if (table.PrimaryKey == column.Name)
                        {
                            columnDefinition += " PRIMARY KEY";
                        }

                        addColumnQuery += " ADD " + columnDefinition;
                    }
                }

                // second step columns to drop
                foreach (var column in oldTable.Columns)
                {

                    var clm = table.Columns.FirstOrDefault(clmn => clmn.Name.Equals(column.Name));
                    if (clm == null)
                    {
                        if (dropColumnQuery != "")
                        {
                            dropColumnQuery += ", ";
                        }

                        dropColumnQuery += " DROP " + column.Name;
                    }
                }

                if (!string.IsNullOrEmpty(addColumnQuery))
                {
                    var cmdString = "ALTER TABLE " 
                                    + table.Name + ""
                                    + (string.IsNullOrEmpty(addColumnQuery) == false ? addColumnQuery : "")
                                    + (string.IsNullOrEmpty(dropColumnQuery) == false ? " " + dropColumnQuery : "");
                    success = ExecuteCommand(cmdString, ref error);
                }
                else
                {
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// Get values for a given column in a table
        /// </summary>
        /// <param name="values">Found values</param>
        /// <param name="tableName">Table to search in</param>
        /// <param name="columnName">Column for the values</param>
        /// <param name="criteria">Filter, use FormatCriteria for vendor independent syntax</param>
        /// <param name="allowMultiple">If false, multiple values will be filtered out</param>
        /// <param name="clearFirst">If false, the values will not be cleared first</param>
        /// <param name="topCount">If greater than 0, only the given number of entries will be returned</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of entries found/returned</returns>
        public override int GetRowValues(ref ArrayList values, string tableName, string columnName, string criteria, bool allowMultiple,
                        bool clearFirst, int topCount, ref Error error)
        {
            if (clearFirst || (values == null))
            {
                values = new ArrayList();
            }

            if (criteria.Contains("SCHEMA_NAME"))
            {
                // just check if database exists
                return 1;
            }
            // now build the query...
            var query = MakeSelectQuery(tableName, columnName, criteria, topCount);

            // Execute the query and fill the data set...
            var ds = new DataSet();
            if (FillDataSet(query, ref ds, ref error, tableName))
            {
                // Get values from the table...
                var table = ds.Tables[0];
                foreach (DataRow dataRow in table.Rows)
                {
                    var vObject = dataRow[0];
                    if (vObject != DBNull.Value)
                    {
                        var strValue = vObject.ToString();
                        if (allowMultiple || (!values.Contains(strValue)))
                        {
                            values.Add(strValue);
                        }
                    }
                }
            }

            return values.Count;
        }

        #endregion

        #region Protected Methods

        // Get the connection string... derived class should override this
        protected virtual string ConnectionString(Database database, bool bReadOnly)
        {
            //string connectionString = "Server=" + database.Server.Name + ";Port=" + mPort + ";Database=" + database.Name + ";Uid=" + database.User + ";Pwd=" + database.Password + ";";
            string connectionString;

            if (!string.IsNullOrEmpty(database.Server.GetServerConfigItem().ServerDatabasePath))
            {
                if (bool.TryParse(SDBeesSQLiteLocalConfiguration().Options[m_SQLiteWAL, true].Value.ToString(), out var logValue) && logValue)
                    connectionString = "Data Source=" + database.Server.GetServerConfigItem().ServerDatabasePath + ";Uid=" + database.User + ";Pwd=" + database.Password + ";" + "PRAGMA journal_mode=WAL;";
                else
                    connectionString = "Data Source=" + database.Server.GetServerConfigItem().ServerDatabasePath + ";Uid=" + database.User + ";Pwd=" + database.Password + ";";
            }
            else
                connectionString = "Data Source=" + database.Server.GetServerConfigItem().ServerDatabase + ";Uid=" + database.User + ";Pwd=" + database.Password + ";";

            return connectionString;
        }

        protected override bool FillDataSet(string query, ref DataSet ds, ref Error error, string sTablename)
        {
            var success = false;

            if (mDbConnection != null)
            {
                try
                {

#if DEBUG
                    var m_logValue = false;
                    if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Query: '" + query + "'", "DB.Details");
#endif

                    var selectCmd = new SQLiteCommand(query, mDbConnection);
                    // MySql doesn't support this method...
                    // selectCmd.CommandTimeout = 30;

                    // collect the results using an adapter
                    var da = new SQLiteDataAdapter();
                    da.SelectCommand = selectCmd;

                    da.Fill(ds, sTablename);

#if DEBUG
                    if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Successful.", "DB.Details");
#endif

                    success = true;
                }
                catch (SQLiteException ex)
                {
                    var myError = new Error(ex.Message, 9999, GetType(), error);
                    error = myError;

#if DEBUG
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Failed: " + ex.Message, "DB.Details");
#endif
                }
                catch (Exception ex)
                {
                    var myError = new Error(ex.Message, 9999, GetType(), error);
                    error = myError;

#if DEBUG
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Failed: " + ex.Message, "DB.Details");
#endif
                }
            }

            return success;
        }
        protected override string SQL_Label(DbType type, ref Error error)
        {
            var label = "";

            switch (type)
            {
                case DbType.Unknown:
                    {
                        var newError = new Error("Cannot determine SQL Label for unknown type", 9999, GetType(), error);
                        error = newError;
                    }
                    break;

                default:
                    {
                        var index = (int)type;
                        label = SQLTypeLabels[index];
                    }
                    break;
            }


            return label;
        }

        protected override string SQL_Label(DbBinaryOperator operation, ref Error error)
        {
            var label = "";

            switch (operation)
            {
                case DbBinaryOperator.eUnknown:
                    {
                        var newError = new Error("Cannot determine SQL Label for unknown operation", 9999, GetType(), error);
                        error = newError;
                    }
                    break;

                default:
                    {
                        var index = (int)operation;
                        label = SQLBinaryOperatorLabels[index];
                    }
                    break;
            }

            return label;
        }

        protected override string SQL_Label(DbBooleanOperator operation, ref Error error)
        {
            var label = "";

            switch (operation)
            {
                case DbBooleanOperator.eUnknown:
                    {
                        var newError = new Error("Cannot determine SQL Label for unknown operation", 9999, GetType(), error);
                        error = newError;
                    }
                    break;

                default:
                    {
                        var index = (int)operation;
                        label = SQLBooleanOperatorLabels[index];
                    }
                    break;
            }

            return label;
        }

        protected override string MakeSelectQuery(string tableName, string columnName, string criteria, int topCount)
        {
            // build the query...
            var query = "SELECT " + columnName + " FROM " + tableName;

            if (criteria != "")
            {
                query = query + " WHERE " + criteria;
            }

            if (topCount > 0)
            {
                query += " LIMIT " + topCount;
            }

            return query;
        }

        protected override string GetColumnDefinition(Column column)
        {
            Error error = null;
            var definition = column.Name + " " + SQL_Label(column.Type, ref error);

            if (error != null)
            {
                // login failed... display error message...
                var dlg = new frmError(_errorMsgWrongColumnDefinition, _errorMsgWrongColumnDefinitionTitle, error);
                dlg.ShowDialog();

                dlg.Dispose();
            }
            else
            {
                if (column.HasCustomSize())
                {
                    definition += "(" + column.Size + ")";
                }
                if ((column.Flags & (int)DbFlags.eUnique) != 0)
                {
                    definition += " UNIQUE";
                }
                if ((column.Flags & (int)DbFlags.eIdentity) != 0)
                {
                    // This is not valid for MySQL...
                    // definition += " IDENTITY";

                    if ((column.Flags & (int)DbFlags.eAutoIncrement) != 0)
                    {
                        definition += " AUTO_INCREMENT";
                    }
                }
                if ((column.Flags & (int)DbFlags.eIsRowGuid) != 0)
                {
                    definition += " ROWGUIDCOL";
                }
                if ((column.Flags & (int)DbFlags.eAllowNull) == 0)
                {
                    definition += " NOT NULL";
                }
                if ((column.Flags & (int)DbFlags.eHasDefault) != 0)
                {
                    definition += " DEFAULT " + GetQuotedValue(column.Type, column.Default);
                }
            }

            return definition;
        }

        protected override string FormatColumnNameForInsert(string columnName)
        {
            return columnName;
        }

        protected override string FormatColumnNameForUpdate(string columnName)
        {
            return columnName;
        }

        #endregion

        #region overrides
        /// <summary>
        /// The internal SQLiteSql connection (read-only)
        /// </summary>
        public override object GetNativeConnection()
        {
            return mDbConnection;
        }
        #endregion
    }
}
