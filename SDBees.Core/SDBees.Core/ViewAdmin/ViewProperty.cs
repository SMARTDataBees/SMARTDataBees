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
using SDBees.DB;
using SDBees.Plugs.TemplateBase;
using Attribute = SDBees.DB.Attribute;

namespace SDBees.ViewAdmin
{
    /// <summary>
    /// Database resident object representing the view properties. For each view there is
    /// one view property in the database. This class supports caching of view definitions
    /// </summary>
    public class ViewProperty : TemplateDBBaseData //SDBees.DB.Object
    {
        #region Private Data Members

        private static Table gTable;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of the view
        /// </summary>
        public string ViewName
        {
            get { return (string)GetPropertyByColumn("viewname"); }
            set { SetPropertyByColumn("viewname", value); }
        }

        public Guid ViewId
        {
            get { return new Guid(Id.ToString()); }
            set { Id = value.ToString(); }
        }

        /// <summary>
        /// Description of the view
        /// </summary>
        public string ViewDescription
        {
            get { return (string)GetPropertyByColumn("viewdescription"); }
            set { SetPropertyByColumn("viewdescription", value); }
        }


        public string idSdBees
        {
            get { return (string)GetPropertyByColumn("id_sdbees"); }
            set { SetPropertyByColumn("id_sdbees", value); }
        }


        public override string GetTableName
        {
            get { return "usrViewProperties"; }
        }


        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public ViewProperty()
        {
            Table = gTable;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Find all available views and return their object ids
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="objectIds">Returned object ids of the view properties</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found views</returns>
        public static int FindAllViewProperties(Database database, ref ArrayList objectIds, ref Error error)
        {
            return database.Select(gTable, gTable.PrimaryKey, ref objectIds, ref error);
        }

        /// <summary>
        /// Find all available views and return their names
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="viewNames">Returned names of the view properties</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found views</returns>
        public static int GetAllViewNames(Database database, ref ArrayList viewNames, ref Error error)
        {
            return database.Select(gTable, "viewname", ref viewNames, ref error);
        }

        /// <summary>
        /// Find a view property by its name and load it from the database
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="name">Name of the view</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>ViewProperties object containing all the information from the database</returns>
        public static ViewProperty FindView(Database database, string name, ref Error error)
        {
            ViewProperty result = null;

            var objectId = GetViewIdFromName(database, name, ref error);

            if (objectId != Guid.Empty)
            {
                result = new ViewProperty();
                if (!result.Load(database, objectId, ref error))
                {
                    result = null;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the id of a view by its name
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="name">Name of the view</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Guid (object id) of the view</returns>
        public static Guid GetViewIdFromName(Database database, string name, ref Error error)
        {
            var result = Guid.Empty;

            var attribute = new Attribute(gTable.Columns["viewname"], name);
            var criteria = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
            ArrayList objectIds = null;
            if (database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error) > 0)
            {
                result = new Guid((string)objectIds[0]);
            }

            return result;
        }

        /// <summary>
        /// Get all the children for a specific type
        /// </summary>
        /// <param name="parentType">String representation of the parent type</param>
        /// <param name="viewDefinitions">Returned array of view definitions</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public int GetChildren(string parentType, ref ArrayList viewDefinitions, ref Error error)
        {
            viewDefinitions = new ArrayList();

            if ((Database != null) && (Id != null))
            {
                ArrayList objectIds = null;
                var numChildren = ViewDefinition.FindViewDefinitionsByParentType(Database, ref objectIds, ViewId, parentType, ref error);

                foreach (var objectId in objectIds)
                {
                    var viewDef = new ViewDefinition();
                    if ((error == null) && viewDef.Load(Database, objectId, ref error))
                    {
                        viewDefinitions.Add(viewDef);
                    }
                }
            }
            else
            {
                error = new Error("GetChildren failed on non-persistent ViewProperties object", 9999, GetType(), error);
            }

            return viewDefinitions.Count;
        }

        /// <summary>
        /// Get all the parents for a specific type
        /// </summary>
        /// <param name="childType">String representation of the child type</param>
        /// <param name="viewDefinitions">Returned array of view definitions</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries, should always be 0 or 1</returns>
        public int GetParents(string childType, ref ArrayList viewDefinitions, ref Error error)
        {
            viewDefinitions.Clear();

            if ((Database != null) && (Id != null))
            {
                ArrayList objectIds = null;
                var numParents = ViewDefinition.FindViewDefinitionByChildType(Database, ref objectIds, ViewId, childType, ref error);

                foreach (var objectId in objectIds)
                {
                    var viewDef = new ViewDefinition();
                    if ((error == null) && viewDef.Load(Database, objectId, ref error))
                    {
                        viewDefinitions.Add(viewDef);
                    }
                }
            }
            else
            {
                error = new Error("GetParents failed on non-persistent ViewProperties object", 9999, GetType(), error);
            }

            return viewDefinitions.Count;
        }

        /// <summary>
        /// Initialize the table schema
        /// </summary>
        /// <param name="database"></param>
        public static void InitTableSchema(Database database)
        {
            var viewProperties = new ViewProperty(); ;
            viewProperties.InitTableSchema(ref gTable, database);

            // Now add columns always required by this plugIn
            viewProperties.AddColumn(new Column("viewname", DbType.String, "View Name", "View Name", "", 80, "", 0), database);
            viewProperties.AddColumn(new Column("viewdescription", DbType.String, "View Description", "View Description", "", 256, "", 0), database);
        }

        #endregion

        #region Protected Methods

        /*
        /// <summary>
        /// Get the table name
        /// </summary>
        /// <returns></returns>
        protected override string TableName()
        {
            return "usrViewProperties";
        }
         * */

        /// <summary>
        /// Returns the type as a string, "" for null
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string GetTypeString(Type value)
        {
            if (value == null)
                return "";

            return value.ToString();
        }

        #endregion
    }
}
