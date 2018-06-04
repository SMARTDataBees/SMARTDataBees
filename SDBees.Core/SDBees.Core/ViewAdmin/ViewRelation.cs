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
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;

namespace SDBees.ViewAdmin
{
    /// <summary>
    /// Class representing relations between objects within a specific view
    /// </summary>
    public class ViewRelation  : SDBees.Plugs.TemplateBase.TemplateDBBaseData
    {
        #region Private Data Members

        private static Table gTable = null;

        private static System.Data.DataTable s_tableCache = null;

        #endregion

        #region Public Properties

        public static void FillCache()
        {
            Error error = null;

            SDBeesDBConnection.Current.Database.Open(true, ref error);

            s_tableCache = SDBees.DB.SDBeesDBConnection.Current.GetDataTableForPlugin(SDBees.ViewAdmin.ViewRelation.TableName);

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
            base.Table = gTable;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all view relations in a database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        //public static int FindAllViewRelations(Database database, ref ArrayList objectIds, ref Error error)
        //{
        //    return database.Select(gTable, gTable.PrimaryKey, ref objectIds, ref error);
        //}

        /// <summary>
        /// Gets all view relations for a specific view and parent in a database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="viewId"></param>
        /// <param name="parentId"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        //public static int FindViewRelationForView(Database database, Guid viewId, Guid parentId, ref ArrayList objectIds, ref Error error)
        //{
        //    SDBees.DB.Attribute attView = new SDBees.DB.Attribute(gTable.Columns["view"], viewId.ToString());
        //    string criteria1 = database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error);
        //    SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns["parent"], parentId.ToString());
        //    string criteria2 = database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

        //    string criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

        //    return database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        //}

        public static bool GetViewRelationsByRootId(SDBees.Core.Model.SDBeesDocumentId docId, ref Error _error, ref ArrayList _existingObjectsWithoutAlienId)
        {
            bool result = false;
            int _countWithoutAlienId = -1;
            ArrayList _lstIds = new ArrayList();

            if (SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.ObjectExistsInDbWithSDBeesId(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.gTable, docId.Id, ref _error, ref _lstIds))
            {
                Plugs.TemplateBase.TemplateDBBaseData docdata = SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.GetDocumentData(docId.Id, ref _error);

                // select all viewrels with parentid == rootid
                string firstDocid = docdata.GetPropertyByColumn(SDBees.Core.Connectivity.ConnectivityManagerDocumentBaseData.m_DocumentRootColumnName).ToString();
                _countWithoutAlienId = FindViewRelationByParentId(ViewAdmin.Current.MyDBManager.Database, new Guid(firstDocid), ref _existingObjectsWithoutAlienId, ref _error);
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
            int result = 0; 

#if PROFILER
            SDBees.Profiler.Start("FindViewRelationForParentIdChildType");
#endif

            if (s_tableCache != null)
            {
                //string criteria = string.Format("{0} = ´{1}´ AND {2} = ´{3}´", m_ChildTypeColumnName, childType, m_ParentIdColumnName, parentId.ToString());
                //string criteria = string.Format("[{0}] = ´{1}´", m_ChildTypeColumnName, childType);
                //System.Data.DataRow[] dataRows = s_tableCache.Select(criteria);

                IEnumerable<System.Data.DataRow> query = from r in s_tableCache.AsEnumerable() where r.Field<string>(m_ChildTypeColumnName) == childType && r.Field<string>(m_ParentIdColumnName) == parentId.ToString() select r;  

                bool allowMultiple = false;

                if (objectIds == null) objectIds = new ArrayList();

                foreach (System.Data.DataRow dataRow in query)
                {
                    object vObject = dataRow[0];
                    if (vObject != DBNull.Value)
                    {
                        string strValue = vObject.ToString();
                        if (allowMultiple || (!objectIds.Contains(strValue)))
                        {
                            objectIds.Add(strValue);
                        }
                    }
                }

                result = objectIds.Count;
            }
            else
            {
                SDBees.DB.Attribute attView = new SDBees.DB.Attribute(gTable.Columns[m_ChildTypeColumnName], childType);
                string criteria1 = database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error);
                SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_ParentIdColumnName], parentId.ToString());
                string criteria2 = database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

                string criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

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
            ArrayList criterias = new ArrayList();
            SDBees.DB.Attribute attView = new SDBees.DB.Attribute(gTable.Columns["view"], viewId.ToString());
            criterias.Add(database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error));
            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_ParentIdColumnName], parentId.ToString());
            criterias.Add(database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error));
            SDBees.DB.Attribute attChild = new SDBees.DB.Attribute(gTable.Columns[m_ChildIdColumnName], childId.ToString());
            criterias.Add(database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref error));

            string criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref error);

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
            SDBees.DB.Attribute attChild = new SDBees.DB.Attribute(gTable.Columns[m_ChildIdColumnName], childId.ToString());
            string criteria = database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        internal static bool FindViewRelationByParentIdAndChildId(Database database, Guid guid1, Guid guid2, ref ArrayList objectIds, ref Error _error)
        {
            ArrayList criterias = new ArrayList();

            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_ParentIdColumnName], guid1);
            criterias.Add(database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error));
            SDBees.DB.Attribute attChild = new SDBees.DB.Attribute(gTable.Columns[m_ChildIdColumnName], guid2);
            criterias.Add(database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref _error));

            string criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref _error);

            if (Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error) > 0)
                return true;
            else
                return false;
        }

        public static int FindViewRelationByChildIdParentType(DB.Database database, Guid childId, string parentType, ref ArrayList objectIds, ref Error _error)
        {
            ArrayList criterias = new ArrayList();

            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_ParentTypeColumnName], parentType);
            criterias.Add(database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error));
            SDBees.DB.Attribute attChild = new SDBees.DB.Attribute(gTable.Columns[m_ChildIdColumnName], childId.ToString());
            criterias.Add(database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref _error));

            string criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref _error);

            return Select(database, gTable, gTable.PrimaryKey, criteria, ref objectIds, ref _error);
        }

        public static int FindViewRelationByChildTypeParentType(DB.Database database, string childType, string parentType, ref ArrayList objectIds, ref Error _error)
        {
            ArrayList criterias = new ArrayList();

            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_ParentTypeColumnName], parentType);
            criterias.Add(database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error));
            SDBees.DB.Attribute attChild = new SDBees.DB.Attribute(gTable.Columns[m_ChildTypeColumnName], childType);
            criterias.Add(database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref _error));

            string criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref _error);

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
            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(gTable.Columns[m_ParentIdColumnName], parentId.ToString());
            string criteria = database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

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
            SDBees.DB.Attribute attView = new SDBees.DB.Attribute(gTable.Columns["view"], viewId.ToString());
            string criteria1 = database.FormatCriteria(attView, DbBinaryOperator.eIsEqual, ref error);
            SDBees.DB.Attribute attChild = new SDBees.DB.Attribute(gTable.Columns[m_ChildIdColumnName], childId.ToString());
            string criteria2 = database.FormatCriteria(attChild, DbBinaryOperator.eIsEqual, ref error);

            string criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

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
            ArrayList objectIds = new ArrayList();
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
            if (ViewRelation.FindViewRelationByChildId(database, childId, ref objectIds, ref error) > 0)
            {
                foreach (object objectId in objectIds)
                {
                    ViewRelation viewRel = new ViewRelation();
                    if (viewRel.Load(database, objectId, ref error))
                    {
                        if (viewRel.ChildName != newName)
                        {
                            viewRel.ChildName = newName;
                            viewRel.Save(ref error);

                            // raise an event...
                            ViewAdmin.Current.RaiseViewRelationModified(viewRel, true);
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
            this.Erase(ref error);

            if (error == null)
            {
                if (deleteUnreferencedObjects)
                {
                    ArrayList objectIds = new ArrayList();
                    if (0 == ViewRelation.FindViewRelationByChildId(this.Database, this.ChildId, ref objectIds, ref error))
                    {
                        TemplateTreenode plugin = TemplateTreenode.GetPluginForType(this.ChildType);
                        if (plugin != null)
                        {
                            plugin.DeleteDataObject(Database, this.ChildId);
                        }
                    }
                }

                // raise an event...
                ViewAdmin.Current.RaiseViewRelationRemoved(this);
            }

        }

        /// <summary>
        /// Save the view relation persistently to the database, this will raise an event if successful
        /// </summary>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successful</returns>
        public override bool Save(ref Error error)
        {
            bool WasNewObject = this.IsNewObject;

            bool success = base.Save(ref error);

            if (WasNewObject && success)
            {
                // raise an event...
                ViewAdmin.Current.RaiseViewRelationCreated(this);
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
            ViewRelation viewRelations = new ViewRelation();
            viewRelations.InitTableSchema(ref gTable, database);

            // Now add columns always required by this plugIn
            viewRelations.AddColumn(new Column("view", SDBees.DB.DbType.eGuidString, "View", "View", "", 0, "", 0), database);
            viewRelations.AddColumn(new Column(m_ParentIdColumnName, SDBees.DB.DbType.eGuidString, "Parent", "Parent Id", "", 0, "", (int)DbFlags.eAllowNull), database);
            viewRelations.AddColumn(new Column(m_ParentTypeColumnName, SDBees.DB.DbType.eString, "Parent Type", "Parent Type", "", 80, "", (int)DbFlags.eAllowNull), database);
            viewRelations.AddColumn(new Column(m_ChildIdColumnName, SDBees.DB.DbType.eGuidString, "Child", "Child Id", "", 0, "", 0), database);
            viewRelations.AddColumn(new Column(m_ChildTypeColumnName, SDBees.DB.DbType.eString, "Child Type", "Child Type", "", 80, "", 0), database);
            viewRelations.AddColumn(new Column(m_ChildNameColumnName, SDBees.DB.DbType.eString, "Child Name", "Child Name", "", 80, "", 0), database);
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

            int result = objectIds == null ? 0 : objectIds.Count;

            return result;
        }

        #endregion
    }
}
