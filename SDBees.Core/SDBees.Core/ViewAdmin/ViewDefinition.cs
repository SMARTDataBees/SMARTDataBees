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
    /// Database resident object representing a view definition, this means a parent/child relationship
    /// for a specific view
    /// </summary>
    public class ViewDefinition : TemplateDBBaseData //SDBees.DB.Object
    {
        #region Private Data Members

        private static Table gTable;

        #endregion

        #region Public Properties

        /// <summary>
        /// Guid for the view definition
        /// </summary>
        public string ViewId
        {
            get { return (string) GetPropertyByColumn(m_ViewPropIdColumnName); }
            set { SetPropertyByColumn(m_ViewPropIdColumnName, value); }
        }


        public string IdSdBees
        {
            get { return (string)GetPropertyByColumn(_IdSdBeesColumnName); }
            set { SetPropertyByColumn(_IdSdBeesColumnName, value); }
        }
      

        /// <summary>
        /// String representation of the type for the parent
        /// </summary>
        public string ParentType
        {
            get { return (string)GetPropertyByColumn(m_ParentTypeColumnName); }
            set { SetPropertyByColumn(m_ParentTypeColumnName, value); }
        }

        /// <summary>
        /// String representation of the type for the child
        /// </summary>
        public string ChildType
        {
            get { return (string)GetPropertyByColumn(m_ChildTypeColumnName); }
            set { SetPropertyByColumn(m_ChildTypeColumnName, value); }
        }

        /// <summary>
        /// Name of the child
        /// </summary>
        public string ChildName
        {
            get { return (string)GetPropertyByColumn(m_ChildNameColumnName); }
            set { SetPropertyByColumn(m_ChildNameColumnName, value); }
        }

        public override string GetTableName
        {
            get { return "usrViewDefinitions"; }
        }

      
        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ViewDefinition() : base("ViewRelations", "TestView", "General")
        {
            Table = gTable;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Find all given view definitions (parent/child relationships)
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="objectIds">Returned object ids of view definitions</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public static int FindAllViewDefinitions(Database database, ref ArrayList objectIds, ref Error error)
        {
          return Select(database, gTable, gTable.PrimaryKey, ref objectIds, ref error);
        }

        /// <summary>
        /// Find all given view definitions (parent/child relationships) for a specific view
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="objectIds">Returned object ids of view definitions</param>
        /// <param name="viewId">Guid of the required view</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public static int FindViewDefinitions(Database database, ref ArrayList objectIds, Guid viewId, ref Error error)
        {
            var attView = new Attribute(gTable.Columns[m_ViewPropIdColumnName], viewId.ToString());
            var criteria = database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Find all view definitions (parent/child relationships) for a specific view and with a certain child type
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="objectIds">Returned object ids of view definitions</param>
        /// <param name="viewId">Guid of the required view</param>
        /// <param name="childType">String representation of the type for the child</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public static int FindViewDefinitionByChildType(Database database, ref ArrayList objectIds, Guid viewId, string childType, ref Error error)
        {
            var attView = new Attribute(gTable.Columns[m_ViewPropIdColumnName], viewId.ToString());
            var criteria1 = database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error);
            var attChildType = new Attribute(gTable.Columns[m_ChildTypeColumnName], childType);
            var criteria2 = database.FormatCriteria(attChildType, DbBinaryOperator.eIsEqual, ref error);

            var criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Find all view definitions (parent/child relationships) for a specific view and with a certain parent type
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="objectIds">Returned object ids of view definitions</param>
        /// <param name="viewId">Guid of the required view</param>
        /// <param name="parentType">String representation of the type for the parent</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public static int FindViewDefinitionsByParentType(Database database, ref ArrayList objectIds, Guid viewId, string parentType, ref Error error)
        {
            var attView = new Attribute(gTable.Columns[m_ViewPropIdColumnName], viewId.ToString());
            var criteria1 = database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error);
            var attParentType = new Attribute(gTable.Columns[m_ParentTypeColumnName], parentType);
            var criteria2 = database.FormatCriteria(attParentType, DbBinaryOperator.eIsEqual, ref error);

            var criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Find all view definitions (parent/child relationships) for a specific view and with a certain child type and parent type
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="objectIds">Returned object ids of view definitions</param>
        /// <param name="viewId">Guid of the required view</param>
        /// <param name="parentType">String representation of the type for the parent</param>
        /// <param name="childType">String representation of the type for the child</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>Number of found entries</returns>
        public static int FindViewDefinitionsByTypes(Database database, ref ArrayList objectIds, Guid viewId, string parentType, string childType, ref Error error)
        {
            var criterias = new ArrayList();
            var attView = new Attribute(gTable.Columns[m_ViewPropIdColumnName], viewId.ToString());
            criterias.Add(database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error));
            var attParentType = new Attribute(gTable.Columns[m_ParentTypeColumnName], parentType);
            criterias.Add(database.FormatCriteria(attParentType, DbBinaryOperator.eIsEqual, ref error));
            var attChildType = new Attribute(gTable.Columns[m_ChildTypeColumnName], childType);
            criterias.Add(database.FormatCriteria(attChildType, DbBinaryOperator.eIsEqual, ref error));

            var criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Checks if a given view definitions with a given relationship exists
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="viewId">Guid of the required view</param>
        /// <param name="parentType">String representation of the type for the parent</param>
        /// <param name="childType">String representation of the type for the child</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>True if it exists</returns>
        public static bool ViewDefinitionExists(Database database, Guid viewId, string parentType, string childType, ref Error error)
        {
            var objectIds = new ArrayList();
            return (FindViewDefinitionsByTypes(database, ref objectIds, viewId, parentType, childType, ref error) > 0);
        }

        public const string m_ViewPropIdColumnName = "view";
        public const string m_ParentTypeColumnName = "parent_type";
        public const string m_ChildTypeColumnName = "child_type";
        public const string m_ChildNameColumnName = "child_name";
        public const string _IdSdBeesColumnName = "id_sdbees";

        

        /// <summary>
        /// Initialize the Table schema in the databse
        /// </summary>
        /// <param name="database"></param>
        public static void InitTableSchema(Database database)
        {
            var viewDefinition = new ViewDefinition();
            viewDefinition.InitTableSchema(ref gTable, database);

            // Now add columns always required by this plugIn
            viewDefinition.AddColumn(new Column(m_ViewPropIdColumnName, DbType.GuidString, "View", "View", "", 0, "", 0), database);
            viewDefinition.AddColumn(new Column(m_ParentTypeColumnName, DbType.String, "Parent Type", "Parent Type", "", 80, "", (int)DbFlags.eAllowNull), database);
            viewDefinition.AddColumn(new Column(m_ChildTypeColumnName, DbType.String, "Child Type", "Child Type", "", 80, "", 0), database);
            viewDefinition.AddColumn(new Column(m_ChildNameColumnName, DbType.String, "Child Name", "Child Name", "", 80, "", 0), database);
        }

        #endregion

        #region Protected Methods

        /*
        /// <summary>
        /// Returns the tablename
        /// </summary>
        /// <returns></returns>
        protected override string TableName()
        {
            return "usrViewDefinitions";
        }
        */
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

        #region Private Methods

        private static int Select(Database database, Table table, string searchColumn, ref ArrayList objectIds, ref Error _error)
        {
            var result = database.Select(table.Name, searchColumn, ref objectIds, ref _error);

            return result;
        }

        private static int Select(Database database, Table table, string searchColumn, string criteria, ref ArrayList objectIds, ref Error _error)
        {
            objectIds = ViewCache.Instance.ViewDefinitions(criteria, ref _error);

            var result = objectIds == null ? 0 : objectIds.Count;

            return result;
        }

        #endregion
      }
}
