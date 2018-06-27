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
using MySql.Data.MySqlClient;
using SDBees.DB.Forms;
using SDBees.GuiTools;

namespace SDBees.DB.MySQL
{
    /// <summary>
    /// Class for wrapping a connection to a MySql database
    /// </summary>
    public class MySqlConnection : Connection
    {
        #region Private Data Members

        private MySql.Data.MySqlClient.MySqlConnection mDbConnection;
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

        /// <summary>
        /// The internal MySql connection (read-only)
        /// </summary>
        public MySql.Data.MySqlClient.MySqlConnection DbConnection
        {
            get
            {
                return mDbConnection;
            }
        }

        #endregion

        #region Contructor / Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="database"></param>
        public MySqlConnection(Database database)
            : base(database)
        {
            mDbConnection = null;
            mPort = Convert.ToInt16(database.Port);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Open a connection to the MySQL database
        /// </summary>
        /// <param name="database">Database to be used for opening</param>
        /// <param name="bReadOnly">Not supported by this database vendor</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public override bool Open(Database database, bool bReadOnly, ref Error error)
        {
            var connectionString = ConnectionString(database, bReadOnly);

#if DEBUG
            if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out var m_logValue) && m_logValue)
                SDBeesDBConnection.Current.LogfileWriter.Writeline("Open", connectionString, "DB.Details");
#endif

            try
            {
                mDbConnection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
                mDbConnection.Open();
            }
            catch (MySqlException ex)
            {
                // Add this error to the list...
                var myError = new Error(ex.Message, 9999, GetType(), error);
                error = myError;
                mDbConnection = null;
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

            if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue)
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
                if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out var m_logValue) && m_logValue)
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("Close", "Closing MySQL connection", "DB.Details");
#endif
                try
                {
                    mDbConnection.Close();
                    mDbConnection.Dispose();
                    mDbConnection = null;
                }
                catch (MySqlException ex)
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
                if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out var m_logValue) && m_logValue)
                    SDBeesDBConnection.Current.LogfileWriter.Writeline("ExecuteCommand", cmdString, "DB.Details");
#endif

                // Execute the query
                var sqlCmd = new MySqlCommand(cmdString, mDbConnection);
                // MySql doesn't support this method...
                // sqlCmd.CommandTimeout = 30;

                try
                {
                    var rowsAffected = sqlCmd.ExecuteNonQuery();

#if DEBUG
                    if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("ExecuteCommand", "Success " + rowsAffected + " rows were affected.", "DB.Details");
#endif
                    success = true;
                }
                catch (Exception ex)
                {
                    var myError = new Error(ex.Message, 9999, GetType(), error);
                    error = myError;

#if DEBUG
                    if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue)
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
            Database.TableNames(ref _lstTblNames,ref  _error);

            //this.DbConnection.
            foreach (string sTblName in _lstTblNames)
            {
                var sSelect = "SELECT * FROM `" + Database.Name + "`.`" + sTblName + "`";
                FillDataSet(sSelect,ref _dtSet, ref _error, sTblName);
            }

            return _dtSet;
        }

        public override DataTable GetReadOnlyDataTable(string sTablename)
        {
            var _dtTable = new DataTable();
            var _dtSet = new DataSet();
            Error _error = null;

            var sSelect = "SELECT * FROM `" + Database.Name + "`.`" + sTablename + "`";

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

        #endregion

        #region Protected Methods

        // Get the connection string... derived class should override this
        protected virtual string ConnectionString(Database database, bool bReadOnly)
        {
            var connectionString = "Server=" + database.Server.Name + ";Port=" + mPort + ";Database=" + database.Name + ";Uid=" + database.User + ";Pwd=" + database.Password + ";";

            return connectionString;
        }

        protected override bool FillDataSet(string query,ref DataSet ds, ref Error error, string sTablename)
        {
            var success = false;

            if (mDbConnection != null)
            {
                try
                {

#if DEBUG
                    if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out var m_logValue) && m_logValue)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Query: '" + query + "'", "DB.Details");
#endif

                    var selectCmd = new MySqlCommand(query, mDbConnection);
                    // MySql doesn't support this method...
                    // selectCmd.CommandTimeout = 30;

                    // collect the results using an adapter
                    var da = new MySqlDataAdapter();
                    da.SelectCommand = selectCmd;

                    da.Fill(ds, sTablename);

#if DEBUG
                    if (bool.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue)
                        SDBeesDBConnection.Current.LogfileWriter.Writeline("FillDataSet", "Successful.", "DB.Details");
#endif

                    success = true;
                }
                catch (MySqlException ex)
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
        /// The internal LocalDB connection (read-only)
        /// </summary>
        public override object GetNativeConnection()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
