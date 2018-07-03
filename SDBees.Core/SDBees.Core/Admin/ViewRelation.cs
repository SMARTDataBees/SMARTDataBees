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
using SDBees.Core.Connectivity;
using SDBees.Core.Model;
using SDBees.DB;
using SDBees.Plugs.TemplateBase;
using SDBees.Plugs.TemplateTreeNode;
using Attribute = SDBees.DB.Attribute;
using DbType = SDBees.DB.DbType;

namespace SDBees.Core.Admin
{
    /// <summary>
    /// Class representing relations between objects within a specific view
    /// </summary>
    public class ViewRelation  : TemplateDBBaseData
    {
        #region Private Data Members

        private static Table gTable;

        private static DataTable s_tableCache;

        #endregion

        #region Public Properties

        public static void FillCache()
        {
            Error error = null;

            SDBeesDBConnection.Current.Database.Open(true, ref error);

            s_tableCache = SDBeesDBConnection.Current.GetDataTableForPlugin(TableName);

            SDBeesDBConnection.Current.Database.Close(ref error);
        }

        public static void ClearCache()
        {
            s_tableCache = null;
        }

        /// <summary>
        /// The view for this relation
        /// </summary>
        public Guid ViewId //TODO: TH-Remove ViewId Relations, shouldn't be used anymore!!
        {
            get { return new Guid ((string) GetPropertyByColumn("view")); }
            set { SetPropertyByColumn("view", value.ToString()); }
        }

        /// <summary>
        /// The Guid of the parent in this relationship
        /// </summary>
        public Guid ParentId
        {
            get { return new Guid((string)GetPropertyByColumn("parent")); }
            set { SetPropertyByColumn("parent", value.ToString()); }
        }

        /// <summary>
        /// The Type of the parent in this relationship
        /// </summary>
        public string ParentType
        {
            get { return (string)GetPropertyByColumn("parent_type"); }
            set { SetPropertyByColumn("parent_type", value); }
        }

        /// <summary>
        /// The Guid of the child in this relationship
        /// </summary>
        public Guid ChildId
        {
            get { return new Guid((string)GetPropertyByColumn("child")); }
            set { SetPropertyByColumn("child", value.ToString()); }
        }

        /// <summary>
        /// The type of the child in this relationship
        /// </summary>
        public string ChildType
        {
            get { return (string)GetPropertyByColumn("child_type"); }
            set { SetPropertyByColumn("child_type", value); }
        }

        /// <summary>
        /// The name of the child in this relationship (this is redundant and only for performance reasons here)
        /// </summary>
		public string ChildName
		{
			get { return (string)GetPropertyByColumn("child_name"); }
			set { SetPropertyByColumn("child_name", value); }
		}

        public override string GetTableName
        {
            get { return TableName; }
        }

        public static string TableName
        {
            get { return "usrViewRelations"; }
        }
    

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ViewRelation()
        {
            Table = gTable;
        }

        #endregion

        #region Public Methods


        public static bool GetViewRelationsByRootId(SDBeesDocumentId docId, ref Error _error, ref ArrayList _existingObjectsWithoutAlienId)
        {
            var result = false;
            var ids = new ArrayList();

            if (ObjectExistsInDbWithSDBeesId(ConnectivityManagerDocumentBaseData.gTable, docId.Id, ref _error, ref ids))
            {
                var docdata = ConnectivityManagerDocumentBaseData.GetDocumentData(docId.Id, ref _error);

                // select all viewrels with parentid == rootid
                var firstDocid = docdata.GetPropertyByColumn(ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName).ToString();
                var _countWithoutAlienId = FindViewRelationByParentId(AdminView.Current.DBManager.Database, new Guid(firstDocid), ref _existingObjectsWithoutAlienId, ref _error);
                if (_countWithoutAlienId > 0)
                    result = true;
            }
            return result;
        }


        /// <summary>
        /// Gets all view relations for a specific parentid and childtype in a database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="viewId"></param>
        /// <param name="parentId"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int FindViewRelationForParentIdChildType(Database database,  Guid parentId, string childType, ref ArrayList objectIds, ref Error error)
        {
            int result; 

#if PROFILER
            SDBees.Profiler.Start("FindViewRelationForParentIdChildType");
#endif

            if (s_tableCache != null)
            {
                //string criteria = string.Format("{0} = ´{1}´ AND {2} = ´{3}´", m_ChildTypeColumnName, childType, m_ParentIdColumnName, parentId.ToString());
                //string criteria = string.Format("[{0}] = ´{1}´", m_ChildTypeColumnName, childType);
                //System.Data.DataRow[] dataRows = s_tableCache.Select(criteria);

                IEnumerable<DataRow> query = from r in s_tableCache.AsEnumerable() where r.Field<string>(m_ChildTypeColumnName) == childType && r.Field<string>(m_ParentIdColumnName) == parentId.ToString() select r;  

                var allowMultiple = false;

                if (objectIds == null) objectIds = new ArrayList();

                foreach (var dataRow in query)
                {
                    var vObject = dataRow[0];
                    if (vObject != DBNull.Value)
                    {
                        var strValue = vObject.ToString();
                        if ((!objectIds.Contains(strValue)))
                        {
                            objectIds.Add(strValue);
                        }
                    }
                }

                result = objectIds.Count;
            }
            else
            {
                var childColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ChildTypeColumnName));
                var attView = new Attribute(childColumn, childType);
                var criteria1 = database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error);

                var parentColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ParentIdColumnName));
                var attParent = new Attribute(parentColumn, parentId.ToString());
                var criteria2 = database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

                var criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

                //criteria += String.Format(" ORDER BY {0} ASC", m_ChildNameColumnName);

                result = Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
            }

#if PROFILER
            SDBees.Profiler.Stop();
#endif

            return result;
        }

        /// <summary>
        /// Gets view relation for a specific view, parent and child in a database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="viewId"></param>
        /// <param name="parentId"></param>
        /// <param name="childId"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int FindViewRelationForView(Database database, Guid viewId, Guid parentId, Guid childId, ref ArrayList objectIds, ref Error error)
        {
            var criterias = new ArrayList();
            var viewColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals("view"));
            var attView = new Attribute(viewColumn, viewId.ToString());
            criterias.Add(database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error));

            var parentColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ParentIdColumnName));
            var attParent = new Attribute(parentColumn, parentId.ToString());
            criterias.Add(database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error));

            var childColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ChildIdColumnName));
            var attChild = new Attribute(childColumn, childId.ToString());
            criterias.Add(database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref error));

            var criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Gets all view relations for a specific child in a database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="childId"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int FindViewRelationByChildId(Database database, Guid childId, ref ArrayList objectIds, ref Error error)
        {
            var childColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ChildIdColumnName));
            var attChild = new Attribute(childColumn, childId.ToString());
            var criteria = database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        internal static bool FindViewRelationByParentIdAndChildId(Database database, Guid guid1, Guid guid2, ref ArrayList objectIds, ref Error _error)
        {
            var criterias = new ArrayList();

            var parentColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ParentIdColumnName));
            var attParent = new Attribute(parentColumn, guid1);
            criterias.Add(database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error));

            var childColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ChildIdColumnName));
            var attChild = new Attribute(childColumn, guid2);
            criterias.Add(database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref _error));

            var criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref _error);

            if (Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error) > 0)
                return true;
            return false;
        }

        public static int FindViewRelationByChildIdParentType(Database database, Guid childId, string parentType, ref ArrayList objectIds, ref Error _error)
        {
            var criterias = new ArrayList();

            var parentColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ParentTypeColumnName));
            var attParent = new Attribute(parentColumn, parentType);
            criterias.Add(database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error));

            var childColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ChildIdColumnName));
            var attChild = new Attribute(childColumn, childId.ToString());
            criterias.Add(database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref _error));

            var criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref _error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
        }

        public static int FindViewRelationByChildTypeParentType(Database database, string childType, string parentType, ref ArrayList objectIds, ref Error _error)
        {
            var criterias = new ArrayList();

            var parentColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ParentTypeColumnName));
            var attParent = new Attribute(parentColumn, parentType);
            criterias.Add(database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error));

            var childColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ChildTypeColumnName));
            var attChild = new Attribute(childColumn, childType);
            criterias.Add(database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref _error));

            var criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref _error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
        }


        /// <summary>
        /// Gets all view relations for childs for a specific parent id in a database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="parentId"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int FindViewRelationByParentId(Database database, Guid parentId, ref ArrayList objectIds, ref Error error)
        {
            var parentColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ParentIdColumnName));
            var attParent = new Attribute(parentColumn, parentId.ToString());
            var criteria = database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Gets all view relations for a specific view and child in a database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="viewId"></param>
        /// <param name="childId"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int ChildReferencesForView(Database database, Guid viewId, Guid childId, ref ArrayList objectIds, ref Error error)
        {
            var viewColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals("view"));
            var attView = new Attribute(viewColumn, viewId.ToString());
            var criteria1 = database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error);

            var childColumn = gTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(m_ChildIdColumnName));
            var attChild = new Attribute(childColumn, childId.ToString());
            var criteria2 = database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref error);

            var criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Check if an object has a view relation in a specific view as a child
        /// </summary>
        /// <param name="database"></param>
        /// <param name="viewId"></param>
        /// <param name="childId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool ChildReferencedByView(Database database, Guid viewId, Guid childId, ref Error error)
        {
            var objectIds = new ArrayList();
            ChildReferencesForView(database, viewId, childId, ref objectIds, ref error);

            return (objectIds.Count > 0);
        }

        /// <summary>
        /// Updates all view relations for the given child id the the new name
        /// </summary>
        /// <param name="database"></param>
        /// <param name="newName"></param>
        /// <param name="childId"></param>
        /// <param name="error"></param>
        public static void UpdateViewRelationNames(Database database, string newName, Guid childId, ref Error error)
        {
            ArrayList objectIds = null;
            if (FindViewRelationByChildId(database, childId, ref objectIds, ref error) > 0)
            {
                foreach (var objectId in objectIds)
                {
                    var viewRel = new ViewRelation();
                    if (viewRel.Load(database, objectId, ref error))
                    {
                        if (viewRel.ChildName != newName)
                        {
                            viewRel.ChildName = newName;
                            viewRel.Save(ref error);

                            // raise an event...
                            AdminView.Current.RaiseViewRelationModified(viewRel, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a view relation from the database and erases the data object if it not referenced
        /// by any view and if desired
        /// </summary>
        /// <param name="deleteUnreferencedObjects">When true and the child object is not referenced by any view, the child object is erased</param>
        /// <param name="error"></param>
        public void Delete(bool deleteUnreferencedObjects, ref Error error)
        {
            Erase(ref error);

            if (error == null)
            {
                if (deleteUnreferencedObjects)
                {
                    var objectIds = new ArrayList();
                    if (0 == FindViewRelationByChildId(Database, ChildId, ref objectIds, ref error))
                    {
                        var plugin = TemplateTreenode.GetPluginForType(ChildType);
                        if (plugin != null)
                        {
                            plugin.DeleteDataObject(Database, ChildId);
                        }
                    }
                }

                // raise an event...
                AdminView.Current.RaiseViewRelationRemoved(this);
            }

        }

        /// <summary>
        /// Save the view relation persistently to the database, this will raise an event if successful
        /// </summary>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public override bool Save(ref Error error)
        {
            var WasNewObject = IsNewObject;

            var success = base.Save(ref error);

            if (WasNewObject && success)
            {
                // raise an event...
                AdminView.Current.RaiseViewRelationCreated(this);
            }

            return success;
        }

        public const string m_ParentIdColumnName = "parent";
        public const string m_ParentTypeColumnName = "parent_type";
        public const string m_ChildIdColumnName = "child";
        public const string m_ChildTypeColumnName = "child_type";
        public const string m_ChildNameColumnName = "child_name";

        public const string m_StartNodeValue = "start";
        /// <summary>
        /// Initialize the schema
        /// </summary>
        /// <param name="database"></param>
        public static void InitTableSchema(Database database)
        {
            var viewRelations = new ViewRelation();
            viewRelations.InitTableSchema(ref gTable, database);

            // Now add columns always required by this plugIn
            viewRelations.AddColumn(new Column("view", DbType.GuidString, "View", "View", "", 0, "", 0), database);
            viewRelations.AddColumn(new Column(m_ParentIdColumnName, DbType.GuidString, "Parent", "Parent Id", "", 0, "", (int)DbFlags.eAllowNull), database);
            viewRelations.AddColumn(new Column(m_ParentTypeColumnName, DbType.String, "Parent Type", "Parent Type", "", 80, "", (int)DbFlags.eAllowNull), database);
            viewRelations.AddColumn(new Column(m_ChildIdColumnName, DbType.GuidString, "Child", "Child Id", "", 0, "", 0), database);
            viewRelations.AddColumn(new Column(m_ChildTypeColumnName, DbType.String, "Child Type", "Child Type", "", 80, "", 0), database);
            viewRelations.AddColumn(new Column(m_ChildNameColumnName, DbType.String, "Child Name", "Child Name", "", 80, "", 0), database);
        }

        #endregion

        #region Protected Methods

        /*
        /// <summary>
        /// Returns the table name
        /// </summary>
        /// <returns></returns>
        protected override string TableName()
        {
            return "usrViewRelations";
        }
        */
        /// <summary>
        /// Get the type string
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

        private static int Select(Database database, Table table, string searchColumn, string criteria, ref ArrayList objectIds, ref Error _error)
        {
            objectIds = ViewCache.Instance.ViewRelations(criteria, ref _error);

            var result = objectIds == null ? 0 : objectIds.Count;

            return result;
        }

        #endregion
    }
}
