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
using System.Data;
using SDBees.DB.Forms;
using Carbon.Configuration;

namespace SDBees.DB.SQLite
{
    /// <summary>
    /// Class for wrapping a connection to a SQLight database
    /// </summary>
    public class SQLiteConnection : Connection
    {
        #region Private Data Members

        private System.Data.SQLite.SQLiteConnection mDbConnection;
        private System.Data.SQLite.SQLiteTransaction mDbTransaction;
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
                "OR",       // eOr
            };

        private static string m_ConfigurationSection = "SQLite";
        private static XmlConfigurationCategory SDBeesSQLiteLocalConfiguration()
        {
            return SDBees.Core.Global.SDBeesGlobalVars.SDBeesLocalUsersConfiguration().Categories[m_ConfigurationSection, true];
        }

        private static string m_SQLiteWAL = "Use WAL";
        private static bool m_SQLiteWALDefault = false;

        private void SetUpConfiguration()
        {
            XmlConfigurationCategory catLog = SDBeesSQLiteLocalConfiguration();

            XmlConfigurationOption opSQLiteWAL = new XmlConfigurationOption(m_SQLiteWAL, m_SQLiteWALDefault);
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
            mPort = System.Convert.ToInt16(database.Port);

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
            string connectionString = ConnectionString(database, bReadOnly);

#if DEBUG
            bool m_logValue = false;
            if (Boolean.TryParse(SDBees.GuiTools.LogfileWriter.SDBeesLogLocalConfiguration().Options[SDBees.GuiTools.LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue == true)
                SDBeesDBConnection.Current.LogfileWriter.Writeline("Open", connectionString, "DB.Details");
#endif

            try
            {
                mDbConnection = new System.Data.SQLite.SQLiteConnection(connectionString);
                mDbConnection.Open();
                mDbTransaction = mDbConnection.BeginTransaction();
            }
            catch (System.Data.SQLite.SQLiteException ex)
            {

            }
            catch(System.Exception ex)
            {
                // Add this error to the list...
                Error myError = new Error(ex.Message, 9999, this.GetType(), error);
                error = myError;
                mDbConnection = null;
            }

#if DEBUG
            string msg = "Success";
            if (mDbConnection == null)
            {
                msg = "Failed";
            }
            if (Boolean.TryParse(SDBees.GuiTools.LogfileWriter.SDBeesLogLocalConfiguration().Options[SDBees.GuiTools.LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue == true)
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
                bool m_logValue = false;
                if (Boolean.TryParse(SDBees.GuiTools.LogfileWriter.SDBeesLogLocalConfiguration().Options[SDBees.GuiTools.LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue == true)
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
                catch (System.Data.SQLite.SQLiteException ex)
                {
                    Error myError = new Error(ex.Message, 9999, this.GetType(), error);
                    error = myError;
                    mDbConnection = null;
                }
                catch (System.Exception ex)
                {
                    Error myError = new Error(ex.Message, 9999, this.GetType(), error);
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
            bool success = false;

            if (mDbConnection != null)
            {

#if DEBUG
                bool m_logValue = false;
                if (Boolean.TryParse(SDBees.GuiTools.LogfileWriter.SDBeesLogLocalConfiguration().Options[SDBees.GuiTools.LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue == true)
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("ExecuteCommand", cmdString, "DB.Details");
#endif

                // Execute the query
                System.Data.SQLite.SQLiteCommand sqlCmd = new System.Data.SQLite.SQLiteCommand(cmdString, mDbConnection);
                // MySql doesn't support this method...
                // sqlCmd.CommandTimeout = 30;

                try
                {
                    int rowsAffected = sqlCmd.ExecuteNonQuery();

#if DEBUG
                    if (Boolean.TryParse(SDBees.GuiTools.LogfileWriter.SDBeesLogLocalConfiguration().Options[SDBees.GuiTools.LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue == true)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("ExecuteCommand", "Success " + rowsAffected + " rows were affected.", "DB.Details");
#endif
                    success = true;
                }
                catch (System.Exception ex)
                {
                    Error myError = new Error(ex.Message, 9999, this.GetType(), error);
                    error = myError;

#if DEBUG
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("ExecuteCommand", "Failed: " + ex.Message, "DB.Details");
#endif
                }
            }

            return success;
        }

        public override System.Data.DataSet GetReadOnlyDataSet()
        {
            System.Data.DataSet _dtSet = new System.Data.DataSet();
            Error _error = null;
            //this.Open(this,true, ref _error);
            ArrayList _lstTblNames = new ArrayList();
            this.Database.TableNames(ref _lstTblNames, ref  _error);

            //this.DbConnection.
            foreach (string sTblName in _lstTblNames)
            {
                string sSelect = "SELECT * FROM `" + sTblName + "`";
                this.FillDataSet(sSelect, ref _dtSet, ref _error, sTblName);
            }

            return _dtSet;
        }

        public override DataTable GetReadOnlyDataTable(string sTablename)
        {
            System.Data.DataTable _dtTable = new DataTable();
            System.Data.DataSet _dtSet = new DataSet();
            Error _error = null;

            string sSelect = "SELECT * FROM `" + sTablename + "`";
            //string sSelect = "SELECT * FROM `" + this.Database.Name + "`.`" + sTablename + "`";

            this.FillDataSet(sSelect, ref _dtSet, ref _error, sTablename);

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
            bool success = false;

            if (mDbConnection != null)
            {
                string criteria = "(type = '" + "table" + "') AND (name = '" + tableName + "')";
                ArrayList names = null;
                int numTables = GetRowValues(ref names, "sqlite_master", "name", criteria, false, true, 1, ref error);

                return numTables > 0;
            }

            return success;
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
            bool success = false;

            if (mDbConnection != null)
            {
                string specifications = "";

                // First check for new and modified columns...
                foreach (KeyValuePair<string, Column> iterator in table.Columns)
                {
                    Column column = iterator.Value;

                    string columnDefinition = GetColumnDefinition(column);

                    if (!oldTable.Columns.ContainsKey(column.Name))
                    {
                        // This is a new column...
                        if (specifications != "")
                        {
                            specifications += ", ";
                        }

                        if (table.PrimaryKey == column.Name)
                        {
                            columnDefinition += " PRIMARY KEY";
                        }

                        specifications += " ADD " + columnDefinition;
                    }
                    else
                    {
                        // this column might have been modified...
                        // do nothing, not supported by SQLite
                        //if (specifications != "")
                        //{
                        //    specifications += ", ";
                        //}

                        //specifications += " CHANGE " + column.Name + " " + columnDefinition;
                    }
                }

                // second step columns to drop
                foreach (KeyValuePair<string, Column> iterator in oldTable.Columns)
                {
                    Column column = iterator.Value;

                    if (!table.Columns.ContainsKey(column.Name))
                    {
                        if (specifications != "")
                        {
                            specifications += ", ";
                        }

                        specifications += " DROP " + column.Name;
                    }
                }

                if (!String.IsNullOrEmpty(specifications))
                {
                    string cmdString = "ALTER TABLE " + table.Name + specifications;

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
            else
            {
                // now build the query...
                string query = MakeSelectQuery(tableName, columnName, criteria, topCount);

                // Execute the query and fill the data set...
                DataSet ds = new DataSet();
                if (FillDataSet(query, ref ds, ref error, tableName))
                {
                    // Get values from the table...
                    DataTable table = ds.Tables[0];
                    foreach (DataRow dataRow in table.Rows)
                    {
                        object vObject = dataRow[0];
                        if (vObject != DBNull.Value)
                        {
                            string strValue = vObject.ToString();
                            if (allowMultiple || (!values.Contains(strValue)))
                            {
                                values.Add(strValue);
                            }
                        }
                    }
                }

                return values.Count;
            }
        }

        #endregion

        #region Protected Methods

        // Get the connection string... derived class should override this
        protected virtual string ConnectionString(Database database, bool bReadOnly)
        {
            //string connectionString = "Server=" + database.Server.Name + ";Port=" + mPort + ";Database=" + database.Name + ";Uid=" + database.User + ";Pwd=" + database.Password + ";";
            string connectionString = "";

            if (!String.IsNullOrEmpty(database.Server.GetServerConfigItem().ServerDatabasePath))
            {
                bool m_logValue = false;
                if (Boolean.TryParse(SDBeesSQLiteLocalConfiguration().Options[m_SQLiteWAL, true].Value.ToString(), out m_logValue) && m_logValue == true)
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
            bool success = false;

            if (mDbConnection != null)
            {
                try
                {

#if DEBUG
                    bool m_logValue = false;
                    if (Boolean.TryParse(SDBees.GuiTools.LogfileWriter.SDBeesLogLocalConfiguration().Options[SDBees.GuiTools.LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue == true)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Query: '" + query + "'", "DB.Details");
#endif

                    System.Data.SQLite.SQLiteCommand selectCmd = new System.Data.SQLite.SQLiteCommand(query, mDbConnection);
                    // MySql doesn't support this method...
                    // selectCmd.CommandTimeout = 30;

                    // collect the results using an adapter
                    System.Data.SQLite.SQLiteDataAdapter da = new System.Data.SQLite.SQLiteDataAdapter();
                    da.SelectCommand = selectCmd;

                    da.Fill(ds, sTablename);

#if DEBUG
                    if (Boolean.TryParse(SDBees.GuiTools.LogfileWriter.SDBeesLogLocalConfiguration().Options[SDBees.GuiTools.LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue == true)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Successful.", "DB.Details");
#endif

                    success = true;
                }
                catch (System.Data.SQLite.SQLiteException ex)
                {
                    Error myError = new Error(ex.Message, 9999, this.GetType(), error);
                    error = myError;

#if DEBUG
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Failed: " + ex.Message, "DB.Details");
#endif
                }
                catch (System.Exception ex)
                {
                    Error myError = new Error(ex.Message, 9999, this.GetType(), error);
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
            string label = "";

            switch (type)
            {
                case DbType.eUnknown:
                    {
                        Error newError = new Error("Cannot determine SQL Label for unknown type", 9999, this.GetType(), error);
                        error = newError;
                    }
                    break;

                default:
                    {
                        int index = (int)type;
                        label = SQLTypeLabels[index];
                    }
                    break;
            }


            return label;
        }

        protected override string SQL_Label(DbBinaryOperator operation, ref Error error)
        {
            string label = "";

            switch (operation)
            {
                case DbBinaryOperator.eUnknown:
                    {
                        Error newError = new Error("Cannot determine SQL Label for unknown operation", 9999, this.GetType(), error);
                        error = newError;
                    }
                    break;

                default:
                    {
                        int index = (int)operation;
                        label = SQLBinaryOperatorLabels[index];
                    }
                    break;
            }

            return label;
        }

        protected override string SQL_Label(DbBooleanOperator operation, ref Error error)
        {
            string label = "";

            switch (operation)
            {
                case DbBooleanOperator.eUnknown:
                    {
                        Error newError = new Error("Cannot determine SQL Label for unknown operation", 9999, this.GetType(), error);
                        error = newError;
                    }
                    break;

                default:
                    {
                        int index = (int)operation;
                        label = SQLBooleanOperatorLabels[index];
                    }
                    break;
            }

            return label;
        }

        protected override string MakeSelectQuery(string tableName, string columnName, string criteria, int topCount)
        {
            // build the query...
            string query = "SELECT " + columnName + " FROM " + tableName;

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
            string definition = "";

            Error error = null;
            definition = column.Name + " " + SQL_Label(column.Type, ref error);

            if (error != null)
            {
                // login failed... display error message...
                frmError dlg = new frmError(Connection._errorMsgWrongColumnDefinition, Connection._errorMsgWrongColumnDefinitionTitle, error);
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
