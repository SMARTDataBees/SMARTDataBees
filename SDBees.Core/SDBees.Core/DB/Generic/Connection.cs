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
using System.Data;
using System.Linq;
using SDBees.Core.Model;

namespace SDBees.DB
{
    /// <summary>
    /// Operator for defining SQL criterias
    /// </summary>
    public enum DbBinaryOperator
    {
        /// <summary>
        /// Unset or illegal value
        /// </summary>
        eUnknown            = 0,
        /// <summary>
        /// Values equal
        /// </summary>
        eIsEqual            = 1,
        /// <summary>
        /// Values different
        /// </summary>
        eIsDifferent        = 2,
        /// <summary>
        /// First value greater than
        /// </summary>
        eIsGreaterThan      = 3,
        /// <summary>
        /// First value greater or equal
        /// </summary>
        eIsGreaterOrEqual   = 4,
        /// <summary>
        /// First value not greater than
        /// </summary>
        eIsNotGreaterThan   = 5,
        /// <summary>
        /// First values smaller than
        /// </summary>
        eIsSmallerThan      = 6,
        /// <summary>
        /// First value smaller or equal
        /// </summary>
        eIsSmallerOrEqual   = 7,
        /// <summary>
        /// First value not smaller than
        /// </summary>
        eIsNotSmallerThan   = 8,
        /// <summary>
        /// First value is "like", means is contained in
        /// </summary>
        eIsLike             = 9,
        /// <summary>
        /// First value is not like, means is not contained in
        /// </summary>
        eIsNotLike          = 10
    }

    /// <summary>
    /// Operator for combining SQL criterias
    /// </summary>
    public enum DbBooleanOperator
    {
        /// <summary>
        /// Unset or illegal value
        /// </summary>
        eUnknown = 0,
        /// <summary>
        /// Both criterias must be true to fulfill the criteria
        /// </summary>
        eAnd                = 1,
        /// <summary>
        /// One of both values must be true to fulfill the criteria
        /// </summary>
        eOr                 = 2   
    }

    /// <summary>
    /// Class as a wrapper connections to an SQL database
    /// </summary>
    public abstract class Connection
    {
        #region Private Data Members

        private Database mDatabase;

        #endregion

        #region Public Properties

        /// <summary>
        /// Read only property to get the database this connection is opened for
        /// </summary>
        public Database Database
        {
            get
            {
                return mDatabase;
            }
        }

        #endregion

        #region Constructor/Destructor

        public Connection(Database database)
        {
            mDatabase = database;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Open a connection
        /// </summary>
        /// <param name="database"></param>
        /// <param name="bReadOnly"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public abstract bool Open(Database database, bool bReadOnly, ref Error error);

        public abstract object GetNativeConnection();

        /// <summary>
        /// Close a connection to a database
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public abstract bool Close(ref Error error);

        // collect the values of a (filtered) column
        /// <summary>
        /// Get a unique value from a table
        /// </summary>
        /// <param name="tableName">Table to search in</param>
        /// <param name="columnName">column to search for</param>
        /// <param name="criteria">Criteria as a filter, use FormatCriteria to create the syntax</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>The unique value or null if not unique or not found</returns>
        public object GetUniqueValue(string tableName, string columnName, string criteria, ref Error error)
        {
            object result = null;

            ArrayList values = null;
            var numFound = GetRowValues(ref values, tableName, columnName, criteria, false, true, 0, ref error);
            if (numFound == 1)
            {
                result = values[0];
            }

            return result;
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
        public virtual int GetRowValues(ref ArrayList values, string tableName, string columnName, string criteria, bool allowMultiple,
                        bool clearFirst, int topCount, ref Error error)
        {
            if (clearFirst || (values == null))
            {
                values = new ArrayList();
            }

            // now build the query...
            var query = MakeSelectQuery(tableName, columnName, criteria, topCount);

            // Execute the query and fill the data set...
            var ds = new DataSet();
            if (FillDataSet(query,ref ds, ref error, tableName))
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

        /// <summary>
        /// Create a table in this database
        /// </summary>
        /// <param name="table">Table to create</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public virtual bool CreateTable(Table table, ref Error error)
        {
            var success = false;

            if (mDatabase != null)
            {
                var columnDefinitions = "";

                foreach (var column in table.Columns)
                {
                    
                    var columnDefinition = GetColumnDefinition(column);

                    if (columnDefinitions != "")
                    {
                        columnDefinitions += ", ";
                    }

                    if (table.PrimaryKey == column.Name)
                    {
                        columnDefinition += " PRIMARY KEY";
                    }

                    columnDefinitions += columnDefinition;
                }

                var cmdString = "CREATE TABLE " + table.Name + " (" + columnDefinitions + ")";

                success = ExecuteCommand(cmdString, ref error);
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
        public virtual bool UpdateTable(Table table, Table oldTable, ref Error error)
        {
            var success = false;

            if (mDatabase != null)
            {
                var specifications = "";

                // First check for new and modified columns...
                foreach (var column in table.Columns)
                {
                    var columnDefinition = GetColumnDefinition(column);

                    var clm = oldTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(column.Name));
                    if (clm == null)
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
                        if (specifications != "")
                        {
                            specifications += ", ";
                        }

                        specifications += " CHANGE " + column.Name + " " + columnDefinition;
                    }
                }

                // second step columns to drop
                foreach (var column in oldTable.Columns)
                {
                    var clm = table.Columns.FirstOrDefault(clmn => clmn.Name.Equals(column.Name));
                    if (clm == null)
                    {
                        if (specifications != "")
                        {
                            specifications += ", ";
                        }

                        specifications += " DROP " + column.Name;
                    }
                }

                var cmdString = "ALTER TABLE " + table.Name + specifications;

                success = ExecuteCommand(cmdString, ref error);
            }

            return success;
        }

        /// <summary>
        /// Read, restore a table
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Table with all the information about the schema</returns>
        public virtual Table ReadTable(string tableName, ref Error error)
        {
            // This function is not complete yet and always returns null (Kira 29.11.2006)
            Table table = null;

            if (mDatabase != null)
            {
                var criteria = "(TABLE_NAME = '" + tableName + "') AND (TABLE_SCHEMA = '" + mDatabase.Name + "')";

                // now build the query...
                var query = MakeSelectQuery("INFORMATION_SCHEMA.COLUMNS", "*", criteria, 0);

                // Execute the query and fill the data set...
                var dataSet = new DataSet();
                if (FillDataSet(query,ref dataSet, ref error, tableName))
                {
                    // Get values from the table...
                    var dataTable = dataSet.Tables[0];

                    if (dataTable.Rows.Count > 0)
                    {
                        // table = new Table();

                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            var columnName = dataRow["COLUMN_NAME"].ToString();
                            var ordinalPosition = dataRow["ORDINAL_POSITION"].ToString();
                            var dataType = dataRow["DATA_TYPE"].ToString();
                            var defaultValue = dataRow["COLUMN_DEFAULT"].ToString();
                            var isNullable = dataRow["IS_NULLABLE"].ToString();
                        }
                    }
                }
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
            var success = false;

            try
            {
                // Use FormatAttributesForInsert to create the query...
                var setClause = "SET COLUMN_NAME = '" + newColumnName + "'";
                var criteria = "(TABLE_NAME = '" + tableName + "') AND (TABLE_SCHEMA = '" + mDatabase.Name + "')";
                criteria += " AND (COLUMN_NAME = '" + oldColumnName + "')";
                var query = "UPDATE INFORMATION_SCHEMA.COLUMNS " + setClause + " WHERE " + criteria;

                success = ExecuteCommand(query, ref error);
            }
            catch (Exception ex)
            {
                var myError = new Error(ex.Message, 9999, GetType(), error);
                error = myError;
                success = false;
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
            var success = false;

            try
            {
                // Use FormatAttributesForInsert to create the query...
                var query = "ALTER TABLE " + tableName + " DROP COLUMN " + columnName;

                success = ExecuteCommand(query, ref error);
            }
            catch (Exception ex)
            {
                var myError = new Error(ex.Message, 9999, GetType(), error);
                error = myError;
                success = false;
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
            var numColumns = 0;
            columnNames = null;

            if (mDatabase != null)
            {
                var criteria = "(TABLE_NAME = '" + tableName + "') AND (TABLE_SCHEMA = '" + mDatabase.Name + "')";

                // now build the query...
                var query = MakeSelectQuery("INFORMATION_SCHEMA.COLUMNS", "*", criteria, 0);

                // Execute the query and fill the data set...
                var dataSet = new DataSet();
                if (FillDataSet(query,ref dataSet, ref error, tableName))
                {
                    // Get values from the table...
                    var dataTable = dataSet.Tables[0];

                    if (dataTable.Rows.Count > 0)
                    {
                        numColumns = dataTable.Rows.Count;
                        columnNames = new List<string>();

                        foreach (DataRow dataRow in dataTable.Rows)
                        {
                            var columnName = dataRow["COLUMN_NAME"].ToString();
                            columnNames.Add(columnName);
                        }
                    }
                }
            }

            return numColumns;
        }

        /// <summary>
        /// Delete a table from the database
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public virtual bool DeleteTable(string tableName, ref Error error)
        {
            var success = false;

            if (mDatabase != null)
            {
                var cmdString = "DROP TABLE " + tableName;

                success = ExecuteCommand(cmdString, ref error);
            }

            return success;
        }

        /// <summary>
        /// Check if a table exists in this database
        /// </summary>
        /// <param name="tableName">Name of the table</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if it exists</returns>
        public virtual bool TableExists(string tableName, ref Error error)
        {
            var success = false;

            if (mDatabase != null)
            {
                var criteria = "(TABLE_NAME = '" + tableName + "') AND (TABLE_SCHEMA = '" + mDatabase.Name + "')";

                ArrayList names = null;
                var numTables = GetRowValues(ref names, "INFORMATION_SCHEMA.Tables", "TABLE_NAME", criteria, false, true, 1, ref error);

                return numTables > 0;
            }

            return success;
        }

        /// <summary>
        /// Create a row in a given table of this database
        /// </summary>
        /// <param name="table">Table to create a row in</param>
        /// <param name="idColumn">Id column</param>
        /// <param name="uniqueColumns">Unique columns</param>
        /// <param name="attributes">Attributes with values for the row to create</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>id of the created row</returns>
        public virtual object CreateRow(Table table, Column idColumn, List<Column> uniqueColumns, Attributes attributes, ref Error error)
        {
            object id = null;

            try
            {
                // Use FormatAttributesForInsert to create the query...
                var query = "INSERT INTO " + table.Name + " " + FormatAttributesForInsert(attributes);

                if (ExecuteCommand(query, ref error))
                {
                    if (table.ReloadIdOnInsert)
                    {
                        // Now get the id using the unique criteria
                        var criteria = "";
                        foreach (var column in uniqueColumns)
                        {
                           
                            var attribute = attributes[column.Name];

                            var singleCriteria = "(" + FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error) + ")";

                            if (criteria != "")
                            {
                                criteria += " AND ";
                            }
                            criteria += singleCriteria;
                        }

                        id = GetUniqueValue(table.Name, table.PrimaryKey, criteria, ref error);
                    }
                    else
                    {
                        id = attributes[table.PrimaryKey].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                var myError = new Error(ex.Message, 9999, GetType(), error);
                error = myError;
            }

            return id;
        }

        /// <summary>
        /// Update/modify a row in a table of this database
        /// </summary>
        /// <param name="table"></param>
        /// <param name="idColumn"></param>
        /// <param name="id"></param>
        /// <param name="attributes"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public virtual bool UpdateRow(Table table, Column idColumn, object id, Attributes attributes, ref Error error)
        {
            var success = false;

            try
            {
                // Use FormatAttributesForInsert to create the query...
                var attribute = new Attribute(idColumn, id);
                var criteria = "(" + FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error) + ")";
                var query = "UPDATE " + table.Name + FormatAttributesForUpdate(attributes) + " WHERE " + criteria;

                success = ExecuteCommand(query, ref error);
            }
            catch (Exception ex)
            {
                var myError = new Error(ex.Message, 9999, GetType(), error);
                error = myError;
                            success = false;
            }

            return success;
        }

        /// <summary>
        /// Load a row from the database
        /// </summary>
        /// <param name="table"></param>
        /// <param name="idColumn"></param>
        /// <param name="id"></param>
        /// <param name="attributes"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public virtual bool LoadRow(Table table, Column idColumn, object id, ref Attributes attributes, ref Error error)
        {
            var success = false;

            try
            {
                var dataset = new DataSet();

                var pkColumn = table.Columns.FirstOrDefault(clmn => clmn.Name.Equals(table.PrimaryKey));
                var type = pkColumn.Type;
                var query = "SELECT * FROM " + table.Name + " WHERE " + table.PrimaryKey + " = " + GetQuotedValue(type, id);

                if (FillDataSet(query,ref dataset, ref error, table.Name))
                {
                    // Get values from table
                    var dataTable = dataset.Tables[0];

                    if (dataTable.Rows.Count == 1)
                    {
                        var dataRow = dataTable.Rows[0];

                        attributes = new Attributes();

                        foreach (DataColumn dataColumn in dataTable.Columns)
                        {
                            var attributeName = dataColumn.ColumnName;

                            var column = table.Columns.FirstOrDefault(clmn => clmn.Name.Equals(attributeName));
                            var value = column.ConvertValueFromDataRow(dataRow[attributeName]); // Convert raw value from the database to a value for the column!

                            var attribute = new Attribute(column, value);

                            attributes.Add(attributeName, attribute);
                        }

                        success = true;
                    }
                    else
                    {
                        // Add this error to the list...
                        var errMessage = "Fehler:" + dataTable.Rows.Count + " Einträge mit " + table.PrimaryKey + " = " + id + " gefunden in Tabelle '" + table.Name + "'!";
                        var myError = new Error(errMessage, 9999, GetType(), error);
                        error = myError;
                    }

                }
            }
            catch (Exception ex)
            {
                var myError = new Error(ex.Message, 9999, GetType(), error);
                error = myError;
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Erase a row from the database
        /// </summary>
        /// <param name="table"></param>
        /// <param name="criteria"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool EraseRow(Table table, string criteria, ref Error error)
        {
            var success = false;

            try
            {
                var query = "DELETE FROM " + table.Name + " WHERE " + criteria;

                success = ExecuteCommand(query, ref error);
            }
            catch (Exception ex)
            {
                var myError = new Error(ex.Message, 9999, GetType(), error);
                error = myError;
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Format the criteria to match SQL syntax for the current SQL vendor
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="operation"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public string FormatCriteria(Attribute attribute, DbBinaryOperator operation, ref Error error)
        {
            var criteria = "";

            if (attribute.Column != null)
                criteria = "[" + attribute.Column.Name + "]" + " " + SQL_Label(operation, ref error) + " " +
                           GetQuotedValue(attribute.Column.Type, attribute.Value);

            //criteria = attribute.Column.Name + " " + SQL_Label(operation, ref error) + " " + GetQuotedValue(attribute.Column.Type, attribute.Value);

            return criteria;
        }

        /// <summary>
        /// Combine criterias to match SQL syntax for the current SQL vendor
        /// </summary>
        /// <param name="criteria1"></param>
        /// <param name="criteria2"></param>
        /// <param name="operation"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public string FormatCriteria(string criteria1, string criteria2, DbBooleanOperator operation, ref Error error)
        {
            var criteria = "";

            criteria = "(" + criteria1 + ") " + SQL_Label(operation, ref error) + " (" + criteria2 + ")";

            return criteria;
        }

        /// <summary>
        /// Combine criterias to match SQL syntax for the current SQL vendor
        /// </summary>
        /// <param name="criterias">criterias as strings</param>
        /// <param name="operation"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public string FormatCriteria(ArrayList criterias, DbBooleanOperator operation, ref Error error)
        {
            var criteria = "";

            if (criterias != null)
            {
                var operationString = SQL_Label(operation, ref error);

                if (error != null)
                    return criteria;

                foreach (string singleCriteria in criterias)
                {
                    if (criteria.Length > 0)
                    {
                        criteria += " " + operationString + " ";
                    }

                    criteria += "(" + singleCriteria + ")";
                }
            }

            return criteria;
        }

        /// <summary>
        /// derived class should override this...
        /// </summary>
        /// <param name="cmdString"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public abstract bool ExecuteCommand(string cmdString, ref Error error);

        public abstract DataSet GetReadOnlyDataSet();

        public abstract DataTable GetReadOnlyDataTable(string sTablename);

        #endregion

        #region Protected Methods

        // derived class should override this...
        protected abstract bool FillDataSet(string query,ref DataSet ds, ref Error error, string sTablename);
        protected abstract string SQL_Label(DbType type, ref Error error);
        protected abstract string SQL_Label(DbBinaryOperator operation, ref Error error);
        protected abstract string SQL_Label(DbBooleanOperator operation, ref Error error);
        protected abstract string MakeSelectQuery(string tableName, string columnName, string criteria, int topCount);

        internal const string _errorMsgWrongColumnDefinition = "Column definition not valid!";
        internal const string _errorMsgWrongColumnDefinitionTitle = "Create Table";

        protected abstract string GetColumnDefinition(Column column);
        protected string DuplicateCharacters(string value)
        {
            var newValue = value.Replace("'", "''");
            newValue = newValue.Replace("\\", "\\\\");

            return newValue;
        }
        protected virtual string GetQuotedValue(DbType type, object value)
        {
            var quotedValue = "";

            if (value == null)
            {
                // NULL is not quoted!
                quotedValue = "NULL";
            }
            else if ((type == DbType.String) || (type == DbType.StringFixed) || (type == DbType.Binary) ||
                (type == DbType.Guid) || (type == DbType.GuidString) ||
                (type == DbType.Text) || (type == DbType.LongText))
            {
                quotedValue = "'" + DuplicateCharacters(value.ToString()) + "'";
            }
            else if ((type == DbType.Double) || (type == DbType.Single) || (type == DbType.Currency))
            {
                quotedValue = value.ToString();
                if (quotedValue.Length == 0) quotedValue = "0"; // TODO: Ralf Check this with Tim!
                quotedValue = quotedValue.Replace(",", "."); // SQL requires "." not a comma for floating point numbers
            }
            else if (type == DbType.CrossSize)
            {
                SDBeesOpeningSize dtValue;
                if (value.GetType() == typeof(string))
                {
                    dtValue = new SDBeesOpeningSize((string)value);
                }
                else
                {
                    dtValue = (SDBeesOpeningSize)value;
                }
                quotedValue = "'" + dtValue + "'";
            }
            else if (type == DbType.Date)
            {
                DateTime dtValue;
                if (value.GetType() == typeof(string))
                {
                    dtValue = DateTime.Parse((string)value);
                }
                else
                {
                    dtValue = (DateTime)value;
                }
                quotedValue = "'" + dtValue.ToString("yyyy-MM-dd") + "'";
            }
            else if (type == DbType.DateTime)
            {
                DateTime dtValue;
                if (value.GetType() == typeof(string))
                {
                    dtValue = DateTime.Parse((string)value);
                }
                else
                {
                    dtValue = (DateTime)value;
                }
                quotedValue = "'" + dtValue.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
            else if (type == DbType.Boolean)
            {
                bool dtValue;
                if (value.GetType() == typeof(string))
                {
                    dtValue = Boolean.Parse((string)value);
                }
                else
                {
                    dtValue = (bool)value;
                }
                quotedValue = "'" + (dtValue ? "1" : "0") + "'";
            }
            else
            {
                quotedValue = DuplicateCharacters(value.ToString());
            }

            return quotedValue;
        }
        protected virtual string FormatColumnNameForInsert(string columnName)
        {
            return "[" + columnName + "]";
        }
        protected virtual string FormatColumnNameForUpdate(string columnName)
        {
            return "[" + columnName + "]";
        }
        protected virtual string FormatAttributesForInsert(Attributes attributes)
        {
            var strParams = "";
            var strValues = "";

            foreach (var iterator in attributes)
            {
                var attribute = iterator.Value;

                // First add the column name to the list of parameters...
                if (strParams != "")
                {
                    strParams += ", ";
                }
                strParams += FormatColumnNameForInsert(iterator.Key);

                // Now add the value to the list of values...
                if (strValues != "")
                {
                    strValues += ", ";
                }
                strValues += GetQuotedValue(attribute.Column.Type, attribute.Value);
            }

            var result = "";
            if (strParams != "")
            {
                result = " (" + strParams + ") VALUES (" + strValues + ")";
            }

            return result;
        }
        protected virtual string FormatAttributesForUpdate(Attributes attributes)
        {
            var strValues = "";

            foreach (var iterator in attributes)
            {
                var attribute = iterator.Value;

                // Now add the value to the list of values...
                if (strValues != "")
                {
                    strValues += ", ";
                }
                strValues += FormatColumnNameForUpdate(iterator.Key) + " = " + GetQuotedValue(attribute.Column.Type, attribute.Value);
            }

            var result = "";
            if (strValues != "")
            {
                result = " SET " + strValues;
            }

            return result;
        }
        protected string FormatAttributes(bool forInsert, Attributes attributes)
        {
            if (forInsert)
                return FormatAttributesForInsert(attributes);
            return FormatAttributesForUpdate(attributes);
        }

        #endregion
    }
}
