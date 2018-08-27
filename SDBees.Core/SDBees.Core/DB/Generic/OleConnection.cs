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
using System.Data;
using System.Data.OleDb;
using SDBees.DB.Forms;

namespace SDBees.DB
{
    /// <summary>
    /// Class to wrap connections to SQL databases using OleDb
    /// </summary>
    public abstract class OleConnection : Connection
    {
        #region Private Data Members

        private OleDbConnection mDbConnection;
        private static string[] SQLTypeLabels =
            {
                "Unknown",          // eUnknown
                "BINARY",           // eBinary
                "BIT",              // eBoolean
                "TINYINT",          // eByte
                "MONEY",            // eCurrency
                "DATE",             // eDate
                "DATETIME",         // eDateTime
                "DECIMAL",          // eDecimal
                "FLOAT",            // eDouble
                "UNIQUEIDENTIFIER", // eGuid
                "SMALLINT",         // eInt16
                "INT",              // eInt32
                "BIGINT",           // eInt64
                "REAL",             // eSingle
                "NVARCHAR",         // eString
                "NCHAR",            // eStringFixed
                "NCHAR",            // eGuidString
                "NTEXT",            // eText
                "NTEXT",            // eLongText
                "NVARCHAR"         // eCrossSize
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
        /// Standard constructor
        /// </summary>
        public OleDbConnection DbConnection
        {
            get
            {
                return mDbConnection;
            }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor with parameters
        /// </summary>
        /// <param name="database"></param>
        public OleConnection(Database database)
            : base(database)
        {
            mDbConnection = null;
        }

        #endregion

        #region Public Methods

        // Open the connection here...
        public override bool Open(Database database, bool bReadOnly, ref Error error)
        {
            var connectionString = ConnectionString(database, bReadOnly);

            try
            {
                mDbConnection = new OleDbConnection(connectionString);
                mDbConnection.Open();
            }
            catch (OleDbException ex)
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

            return (mDbConnection != null);
        }

        // Close the connection here...
        public override bool Close(ref Error error)
        {
            if (mDbConnection != null)
            {
                try
                {
                    mDbConnection.Close();
                    mDbConnection.Dispose();
                    mDbConnection = null;
                }
                catch (OleDbException ex)
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

        // execute an SQL command
        public override bool ExecuteCommand(string cmdString, ref Error error)
        {
            var success = false;

            if (mDbConnection != null)
            {
                // Execute the query
                var sqlCmd = new OleDbCommand(cmdString, mDbConnection);
                sqlCmd.CommandTimeout = 30;

                try
                {
                    var rowsAffected = sqlCmd.ExecuteNonQuery();

                    success = true;
                }
                catch (Exception ex)
                {
                    var myError = new Error(ex.Message, 9999, GetType(), error);
                    error = myError;
                }
            }

            return success;
        }

        #endregion

        #region Protected Methods

        // Get the connection string... derived class should override this
        protected abstract string ConnectionString(Database database, bool bReadOnly);

        protected override bool FillDataSet(string query,ref DataSet ds, ref Error error, string sTablename)
        {
            var success = false;

            if (mDbConnection != null)
            {
                // Execute the query
                var selectCmd = new OleDbCommand(query, mDbConnection);
                selectCmd.CommandTimeout = 30;

                // collect the results using an adapter
                var da = new OleDbDataAdapter();
                da.SelectCommand = selectCmd;

                // Fill the data set
                da.Fill(ds);

                success = true;
            }
            else
            {
                var myError = new Error("DataSet could not be filled, no connection available.", 9999, GetType(), error);
                error = myError;
            }

            return success;
        }
        protected override string SQL_Label(DbType type, ref Error error)
        {
            var label = "";

            switch(type)
            {
                case DbType.Unknown:
                    {
                        var newError = new Error("Cannot determine SQL Label for unknown type", 9999, GetType(), error);
                        error = newError;
                    }
                    break;

                default:
                    {
                        var index = (int) type;
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
            var query = "";
            if (topCount <= 0)
            {
                query = "SELECT " + columnName + " FROM " + tableName;
            }
            else
            {
                query = "SELECT TOP " + topCount + " " + columnName + " FROM " + tableName;
            }

            if (criteria != "")
            {
                query = query + " WHERE " + criteria;
            }

            return query;
        }
        protected override string GetColumnDefinition(Column column)
        {
            var definition = "";

            Error error = null;
            definition = "[" + column.Name + "] " + SQL_Label(column.Type, ref error);

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
                    definition += " IDENTITY";

                    if ((column.Flags & (int)DbFlags.eAutoIncrement) != 0)
                    {
                        definition += " (" + column.Seed + ", " + column.Increment + ")";
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

        #endregion
    }
}
