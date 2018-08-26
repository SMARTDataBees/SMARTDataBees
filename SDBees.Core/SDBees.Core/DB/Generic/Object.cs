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
using System.Linq;
using SDBees.Core.Utils;
using SDBees.Demoplugins.Dummys;
using SDBees.Plugs.Properties;

namespace SDBees.DB
{
    /// <summary>
    /// Base class for all persistent objects
    /// </summary>
    public abstract class Object
    {
        #region Private Data Members

        private object mId;
        private Table mTable;
        private Database mDatabase;
        private bool mIsNewObject;
        private Dictionary<string, object> mValues;

        #endregion

        #region Public Properties

        /// <summary>
        /// The persistent id for this object
        /// </summary>
        public object Id
        {
            get { return mId; }
            set { mId = value; }
        }

        /// <summary>
        /// Table this object is stored in
        /// </summary>
        public Table Table
        {
            get { return mTable; }
            set { mTable = value; }
        }

        /// <summary>
        /// Database this object is stored in
        /// </summary>
        public Database Database
        {
            get { return mDatabase; }
            set { mDatabase = value; }
        }

        /// <summary>
        /// The object has not been saved yet
        /// </summary>
        public bool IsNewObject
        {
            get { return mIsNewObject; }
        }

        public abstract string GetTableName
        {
            get;
        }


        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Object()
        {
            mId = null;
            mTable = null;
            mDatabase = null;
            mIsNewObject = true;
            mValues = new Dictionary<string, object>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the value for a column by its name. The object must already have been loaded
        /// </summary>
        /// <param name="propertyName">Name of the column name in database!</param>
        /// <returns>Value of the property</returns>
        public object GetPropertyByColumn(string columnName)
        {
            try
            {
                if (mTable != null)
                {
                    if (columnName == mTable.PrimaryKey)
                    {
                        return mId;
                    }

                    if (!mValues.ContainsKey(columnName))
                    {
                        var column = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(columnName));
                        mValues[columnName] = column.CreateDefaultValue();
                    }
                    return mValues[columnName];
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the fully qualified name of the UITypeConverter for a column by it's name. The object must already have been loaded
        /// </summary>
        /// <param name="propertyName">Name of the column name in database!</param>
        /// <returns>Fully qualified name of the UITypeConverter</returns>
        public string GetTypeConverterByColumn(string columnName)
        {
            if (columnName == mTable.PrimaryKey)
            {
                return null;
            }

            var column = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(columnName));

            return column.UITypeConverter;
        }

        /// <summary>
        /// Set a (persistent) property by its name
        /// </summary>
        /// <param name="propertyName">Name of the column name in database!</param>
        /// <param name="value">Value to be set</param>
        /// <returns>true if successful</returns>
        public bool SetPropertyByColumn(string propertyName, object value)
        {
            var success = false;

            if (propertyName == mTable.PrimaryKey)
            {
                mId = value;
                success = true;
            }
            else
            {
                var column = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(propertyName));
                if (column != null)
                {
                    mValues[propertyName] = value;
                    //                mValues[propertyName] = mTable.Columns[propertyName].ConvertValueFromDataRow(value);
                    success = true;
                }
            }

            return success;
        }

        /// <summary>
        /// Get a list of the property names available in this object
        /// </summary>
        /// <returns></returns>
        public List<string> GetPropertyNames()
        {
            var propertyNames = new List<string>();

            foreach (var column in mTable.Columns)
            {
                if (column.Name != mTable.PrimaryKey)
                {
                    propertyNames.Add(column.Name);
                }
            }

            return propertyNames;
        }

        /// <summary>
        /// Save the object persistently to the database
        /// </summary>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public virtual bool Save(ref Error error)
        {
            var success = false;

            try
            {
                // For objects that are not yet persistent and for which AutoCreate has been defined, create an ID now.
                if (mIsNewObject && ((idColumn().Flags & (int)DbFlags.eAutoCreate) != 0))
                {
                    if (idColumn().Type == DbType.GuidString)
                    {
                        mId = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        var myError = new Error("AutoCreate not supported for this type!", 9999, GetType(), error);
                        error = myError;
                        return false;
                    }
                }

                Attributes attributes = null;
                GetAttributes(ref attributes);

                // save attributes to database
                if (mIsNewObject)
                {
                    mId = mDatabase.CreateRow(mTable, idColumn(), uniqueColumns(), attributes, ref error);
                    success = (mId != null);
                    mIsNewObject = !success;
                }
                else
                {
                    success = mDatabase.UpdateRow(mTable, idColumn(), mId, attributes, ref error);
                }
            }
            catch (Exception ex)
            {
                Error.Display("Error during saving of object", error);
            }

            return success;
        }

        /// <summary>
        /// Save the object persistently to the database
        /// </summary>
        /// <param name="database">The database to work with</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public virtual bool Save(Database database, ref Error error)
        {
            try
            {
                mDatabase = database;
                return Save(ref error);
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Load the object from the database. Id must have been set.
        /// </summary>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public virtual bool Load(ref Error error)
        {
            Attributes attributes = null;
            GetAttributes(ref attributes);

            // load attributes from database
            var success = mDatabase.LoadRow(mTable, idColumn(), mId, ref attributes, ref error);

            if (success)
            {
                mIsNewObject = false;
                SetAttributes(attributes);
            }

            return success;
        }

        /// <summary>
        /// Load the object by passing the database and the object id
        /// </summary>
        /// <param name="database">Database</param>
        /// <param name="id">Object id</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public bool Load(Database database, object id, ref Error error)
        {
            mDatabase = database;
            mId = id;
            return Load(ref error);
        }

        /// <summary>
        /// Erase this object persistently from the database
        /// </summary>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public virtual bool Erase(ref Error error)
        {
            var success = false;

            if (mDatabase != null)
            {
                var conn = mDatabase.Open(false, ref error);

                var attribute = new Attribute(idColumn(), mId);
                var criteria = "(" + mDatabase.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error) + ")";
                success = mDatabase.EraseRow(mTable, criteria, ref error);

                mDatabase.Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Set the defaults to this object. Derived classes should overload this and set their defaults.
        /// </summary>
        /// <param name="database">Database to use defaults for</param>
        public virtual void SetDefaults(Database database)
        {
            // nothing at this stage, derived classes can do their part...
            mDatabase = database;
        }

        /// <summary>
        /// Get a summary of the objects values. Derive classes should overload and adjust appropriately.
        /// </summary>
        /// <returns>string representing the summary</returns>
        public virtual string Summary()
        {
            var mySummary = "";

            Attributes attributes = null;
            GetAttributes(ref attributes);

            foreach (var iterator in attributes)
            {
                var attribute = iterator.Value;
                mySummary = iterator.Key + " = '" + attribute.Value + "'\n";
            }

            return mySummary;
        }

        // The following Methods are for modifying the schema and affect the whole class.
        // It is not necessary to have a specific object, any instance is fine and it should
        // not be persistent to avoid conflicts.

        /// <summary>
        /// Add a column to the schema of this object
        /// Hint: It is not necessary to have a specific object, any instance is fine and it should
        /// not be persistent to avoid conflicts.
        /// </summary>
        /// <param name="column">New column to add</param>
        /// <param name="database">Database where the schema should be modified</param>
        public virtual void AddColumn(Column column, Database database)
        {
            try
            {
                 var clm = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(column.Name));
                if (clm == null)
                {
                    // Only add the column if it does not exist
                    mTable.Columns.Add(column);

                    ModifyTableSchema(mTable, database);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Remove a column from the schema of this object
        /// Hint: It is not necessary to have a specific object, any instance is fine and it should
        /// not be persistent to avoid conflicts.
        /// </summary>
        /// <param name="columnName">Name of the column that should be removed</param>
        /// <param name="database">Database</param>
        public virtual void RemoveColumn(string columnName, Database database)
        {
            var column = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(columnName));
            if (column != null)
            {
                // Only remove the column if it exists
                mTable.Columns.Remove(column);
                ModifyTableSchema(mTable, database);
            }
        }

        /// <summary>
        /// Rename a column in the schema of this table
        /// </summary>
        /// <param name="oldColumnName">Name of the column as currently set</param>
        /// <param name="newColumnName">Name of the column as it should be changed to</param>
        /// <param name="database">Database</param>
        public virtual void RenameColumn(string oldColumnName, string newColumnName, Database database)
        {
            var clm = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(oldColumnName));
            if (clm != null)
            {
                // Only rename the column if it exists
                var column = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(oldColumnName));
                mTable.Columns.Remove(column);

                // Since in some cases this is used to fix tables we should check before we get problems...
                if (oldColumnName.Trim() != newColumnName.Trim())
                {
                    Error error = null;
                    database.RenameColumn(mTable.Name, oldColumnName, newColumnName, ref error);
                }

                // rename the column and add again...
                column.Name = newColumnName;
                AddColumn(column, database);
            }
        }

        /// <summary>
        /// Modify the schema of a column
        /// Hint: It is not necessary to have a specific object, any instance is fine and it should
        /// not be persistent to avoid conflicts.
        /// TBD: This is not implemented yet.
        /// </summary>
        /// <param name="columnName">Name of the column to be modfied</param>
        /// <param name="newColumnDefinition">Column definition that should replace the current</param>
        /// <param name="database">Database</param>
        public virtual void ModifyColumn(string columnName, Column newColumnDefinition, Database database)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Modify the schema of a table
        /// </summary>
        /// <param name="table">Modified Table definition</param>
        /// <param name="database">Database</param>
        public virtual void ModifyTable(Table table, Database database)
        {
            ModifyTableSchema(table, database);
        }

        /// <summary>
        /// This will create the database schema in the schema table and the "real" database table.
        /// </summary>
        /// <param name="table">Returned table definition</param>
        /// <param name="database">Database</param>
        public virtual void InitTableSchema(ref Table table, Database database)
        {
            try
            {
                Error error = null;
                var conn = database.Open(false, ref error);

                table = CreateTableSchema(database);

                // set this table to the same reference...
                mTable = table;

                var versionDiff = CheckSchemaVersion();

                if (versionDiff < 0)
                {
                    // Schema is older than implementation... Database should be migrated in the future
                    // for now the program will be aborted
                    var message = "";
                    message += "Datenbankversion ist von einer älteren Programmversion und sollte\r\n";
                    message += "zunächst aktualisiert werden!\r\n\r\n";
                    message += "Details:\r\n";
                    message += "Tabelle = " + mTable.Name + "\r\n";
                    message += "Tabellenversion = " + mTable.SchemaVersion + "\r\n";
                    message += "Programmversion = " + RequiredSchemaVersion() + "\r\n\r\n";
                    message += "Programm kann nicht vollständig ausgeführt werden.";
                    throw new Exception(message);
                }
                if (versionDiff > 0)
                {
                    // Schema is newer than implementation... program should be aborted
                    var message = "";
                    message += "Datenbankversion ist von einer zukünftigen Programmversion und kann\r\n";
                    message += "mit dieser Applikation nicht bearbeitet werden!\r\n\r\n";
                    message += "Details:\r\n";
                    message += "Tabelle = " + mTable.Name + "\r\n";
                    message += "Tabellenversion = " + mTable.SchemaVersion + "\r\n";
                    message += "Programmversion = " + RequiredSchemaVersion() + "\r\n\r\n";
                    message += "Programm kann nicht vollständig ausgeführt werden.";
                    throw new Exception(message);
                }

                // check if the table exists and eventually update the schema
                if (!database.TableExists(table.Name, ref error))
                {
                    database.CreateTable(table, ref error);
                }

                Error.Display("Tabelle '" + table.Name + "' Erzeugen", error);

                database.Close(ref error);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public virtual List<KeyValuePair<PropertySpec, object>> GetAutomaticProperties()
        {
            return new List<KeyValuePair<PropertySpec, object>>();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Read values from properties of the object (not from the DB) into the attributes
        /// </summary>
        /// <param name="attributes"></param>
        protected void GetAttributes(ref Attributes attributes)
        {
            // create if not available yet...
            if (attributes == null)
            {
                attributes = new Attributes();
            }

            foreach (var column in mTable.Columns)
            {

                if (column.Name != mTable.PrimaryKey)
                {
                    var value = GetPropertyByColumn(column.Name);

                    attributes.Add(new Attribute(column, value));
                }
                else if (!mTable.ReloadIdOnInsert)
                {
                    // If the id is not automatically calculated, we also have to add it...
                    attributes.Add(new Attribute(column, mId));
                }
            }
        }

        /// <summary>
        /// Set the values of the given attributes into the property value fields of the object
        /// </summary>
        /// <param name="attributes"></param>
        protected void SetAttributes(Attributes attributes)
        {
            // Set the values to the object
            foreach (var iterator in attributes)
            {
                var columnName = iterator.Key;
                // Id column is handled differently...
                if (columnName != mTable.PrimaryKey)
                {
                    var attribute = iterator.Value;
                    mValues[columnName] = attribute.Value;
                }
            }
        }

        /// <summary>
        /// If ReloadIdOnInsert is set to true for this object, then these columns are
        /// used to "find" the newly created object again
        /// </summary>
        /// <returns></returns>
        protected virtual List<Column> uniqueColumns()
        {
            // We should not get here if the ReloadIdOnInsert is set in the table
            if (mTable.ReloadIdOnInsert)
            {
                //throw new SystemException("Method uniqueColumns must be implemented for class " + this.GetType().ToString());
            }
            // Only the id is unique
            return null;
        }

        /// <summary>
        /// This will modify the database schema in the schema table and the "real" database table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="database"></param>
        /// <returns></returns>
        protected bool ModifyTableSchema(Table table, Database database)
        {
            Error error = null;
            var conn = database.Open(false, ref error);

            table.SchemaVersion = RequiredSchemaVersion();

            Table oldTable = null;
            var schema = new TableSchema();
            if (!schema.Load(database, table.Name, ref error))
            {
                // ignore this error...
                error = null;

                // There is no schema yet defined for this table... define the schema
                schema.Name = table.Name;
                schema.Database = database;
                schema.XmlSchema =  Serializer.ToXml(table);

                // ...but don't save it to the database yet...
            }
            else
            {
                // schema has been defined in database for this table, so read it...
                oldTable = Serializer.FromXml(schema.XmlSchema);

                // and update it... but don't save it to the database yet...
                schema.XmlSchema =  Serializer.ToXml(table);
            }

            // check if the table exists and evtually update the schema
            if (database.TableExists(table.Name, ref error))
            {
                database.UpdateTable(table, oldTable, ref error);
            }
            else
            {
                database.CreateTable(table, ref error);
            }

            // Update the schema only if the table has been successfully 
            if (error == null)
            {
                schema.Save(ref error);
               
                //Dummy1BaseData.gTable = table;        
                //TableSchema.InitTableSchema(database);
            }
            else if (oldTable != null)
            {
                // in case something went wrong, the table and the schema should match
                var xml =  Serializer.ToXml(oldTable);
                table = Serializer.FromXml(xml);
            }

            Error.Display("Table '" + table.Name + "' modify", error);

            database.Close(ref error);

            return (error == null);
        }

        /// <summary>
        /// This will create the database schema in the schema table. It is not required
        /// to override this, only for the TableSchema itself it could make sense.
        /// </summary>
        /// <param name="database"></param>
        /// <returns></returns>
        protected virtual Table CreateTableSchema(Database database)
        {
            Table table;

            Error error = null;
            var tableName = TableName();

            var schema = new TableSchema();
            if (!schema.Load(database, tableName, ref error))
            {
                // ignore this error...
                error = null;

                // There is no schema yet defined for this table... define the schema
                table = new Table(tableName);
                var reloadIdOnInsert = false;

                var primaryColumn = CreateIdColumn(ref reloadIdOnInsert);
                table.Columns.Add(primaryColumn);

                var sdbeesColumn = CreateSDBeesIdColumn(ref reloadIdOnInsert);
                table.Columns.Add(sdbeesColumn);

                table.PrimaryKey = primaryColumn.Name;
                table.ReloadIdOnInsert = reloadIdOnInsert;
                table.SchemaVersion = RequiredSchemaVersion();

                // Now create the schema information and make persistent
                schema.Name = tableName;
                schema.Database = database;
                schema.XmlSchema = Serializer.ToXml(table);

                schema.Save(ref error);
            }
            else
            {
                // schema has been defined in database for this table, so read it...
                table = Serializer.FromXml(schema.XmlSchema);
            }

            Error.Display("Table '" + tableName + "' modify", error);

            return table;
        }

        /// <summary>
        /// Check the current schema version from the table for this schema. Returns difference
        /// between version in database and required version, where negative value means database
        /// is older than the required version, positive value means database is newer than required
        /// by implementation, zero means versions match.
        /// </summary>
        /// <returns>Difference between version in database and required version.</returns>
        protected int CheckSchemaVersion()
        {
            return mTable.SchemaVersion - RequiredSchemaVersion();
        }

        /// <summary>
        /// Gets the schema version the current implementation requires. The version number is 100 for 1.00.
        /// Override if a different schema version is required than 100.
        /// </summary>
        /// <returns></returns>
        protected virtual int RequiredSchemaVersion()
        {
            return 100;
        }

        /// <summary>
        /// Name of the table for this class as stored in the database
        /// Override to specify the table name for this class...
        /// </summary>
        /// <returns></returns>

        //protected abstract string TableName();
        protected string TableName()
        {
            var sTablename = GetTableName;
            return sTablename.ToLower();

            //return "usrAECBuildings";
        }

        /// <summary>
        /// Database internal id. Only for database related tasks. Use m_IsSDBeesColumnName, whereever possible!
        /// </summary>
        public const string m_IdColumnName = "id";
        public const string m_IdColumnDisplayName = "Id";

        /// <summary>
        /// The sdbees internal id. Use this to guaranty same values across databases
        /// </summary>
        public const string m_IdSDBeesColumnName = "id_sdbees";
        public const string m_IdSDBeesColumnDisplayName = "Id SDBees";

        /// <summary>
        /// Override if a different id column is required...
        /// </summary>
        /// <param name="reloadIdOnInsert"></param>
        /// <returns></returns>
        protected virtual Column CreateIdColumn(ref bool reloadIdOnInsert)
        {
            var column = new Column
            {
                Name = m_IdColumnName,
                DisplayName = m_IdColumnDisplayName,
                Description = "the unique id for the database",
                Category = "Internal",
                Type = DbType.GuidString,
                Flags = (int) DbFlags.eAutoCreate,
                IsEditable = false,
                IsBrowsable = false
            };
            reloadIdOnInsert = false;

            return column;
        }

        /// <summary>
        /// Override if a different id column is required...
        /// </summary>
        /// <param name="reloadIdOnInsert"></param>
        /// <returns></returns>
        protected virtual Column CreateSDBeesIdColumn(ref bool reloadIdOnInsert)
        {
            var column = new Column
            {
                Name = m_IdSDBeesColumnName,
                DisplayName = m_IdSDBeesColumnDisplayName,
                Description = "the unique id for the smartdatabees internal use",
                Category = "Internal",
                Type = DbType.GuidString,
                Flags = (int) DbFlags.eAutoCreate,
                IsEditable = false,
                IsBrowsable = false
            };
            reloadIdOnInsert = false;

            return column;
        }

        /// <summary>
        /// Returns the column used as id (primary key)
        /// </summary>
        /// <returns></returns>
        protected virtual Column idColumn()
        {
            return mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(mTable.PrimaryKey));
        }

        #endregion
    }
}
