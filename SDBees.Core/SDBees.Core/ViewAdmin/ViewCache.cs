// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2016 by
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

using System.Collections;
using System.Collections.Generic;
using System.Data;
using SDBees.DB;

namespace SDBees.ViewAdmin
{
    public class ViewCache
    {
        #region Private Data Members

        private static ViewCacheImplementation s_viewCache;

        private static int s_enabled;

        #endregion

        #region Public Properties

        public static ViewCacheImplementation Instance
        {
            get
            {
                if (s_viewCache == null)
                {
#if false //PROFILER
                    System.Windows.Forms.MessageBox.Show("s_viewCache == null");
#endif

                    s_viewCache = CreateViewCacheImplementation(SDBeesDBConnection.Current.Database, false);
                }

                return s_viewCache;
            }
        }

        public static bool Enabled
        {
            get
            {
                return (s_viewCache != null) ? s_viewCache.Enabled : false;
            }
            set
            {
                if (s_viewCache != null)
                {
                    s_viewCache.Enabled = value;
                }
            }
        }

        public static void Enable()
        {
            if (s_enabled == 0)
            {
                var activateCache = true;

                s_viewCache = CreateViewCacheImplementation(SDBeesDBConnection.Current.Database, activateCache);

#if PROFILER
                //SDBees.Profiler.Enabled = true;

                SDBees.Profiler.Start("Enable");
#endif
            }

            s_enabled++;
        }

        public static void Disable()
        {
            --s_enabled;

            if (s_enabled == 0)
            {
#if PROFILER
                SDBees.Profiler.Log(s_viewCache.ToString());

                SDBees.Profiler.Stop();

                //SDBees.Profiler.RememberAndOpenNotepad();

                //SDBees.Profiler.Enabled = false;
#endif

                s_viewCache = null;
            }
        }

        private static ViewCacheImplementation CreateViewCacheImplementation(Database database, bool enabled)
        {
            return new ViewCacheImplementationOne(database, enabled);
        }

        #endregion

        #region Public Classes

        public abstract class ViewCacheImplementation
        {
            public ViewCacheImplementation(Database database, bool enabled)
            {
                mDatabase = database;

                mEnabled = enabled && (UseGlobalCaching == false);
            }

            public bool Enabled
            {
                get
                {
                    return mEnabled;
                }
                set
                {
                    mEnabled = value;

                    OnEnabled(mEnabled);
                }
            }

            public void Clear()
            {
                mViewDefinitionInstance = null;

                mViewRelationInstance = null;

                OnClear();
            }

            public abstract string ToString();

            public abstract ArrayList ViewDefinitions(string criteriaViewDef, ref Error error);

            public abstract bool ViewDefinition(object id, out ViewDefinition viewDefinition, ref Error error);

            public abstract ArrayList ViewRelations(string criteria, ref Error error);

            public abstract bool ViewRelation(object id, out ViewRelation viewRelation, ref Error error);

            public abstract object Parent(Table table, string columnName, string idCriteria, ref Error error);

            protected abstract void OnEnabled(bool enabled);

            protected abstract void OnClear();

            protected Database database
            {
                get { return mDatabase; }
            }

            protected bool UseGlobalCaching
            {
                get { return database != null ? database.UseGlobalCaching : false; }
            }

            protected Table ViewDefinitionTable
            {
                get
                {
                    if (mViewDefinitionInstance == null) mViewDefinitionInstance = new ViewDefinition();

                    return mViewDefinitionInstance.Table;
                }
            }

            protected Table ViewRelationTable
            {
                get
                {
                    if (mViewRelationInstance == null) mViewRelationInstance = new ViewRelation();

                    return mViewRelationInstance.Table;
                }
            }

            private bool mEnabled = true;

            private Database mDatabase;

            private ViewDefinition mViewDefinitionInstance;

            private ViewRelation mViewRelationInstance;
        }

        public class ViewCacheImplementationOne : ViewCacheImplementation
        {
            public ViewCacheImplementationOne(Database database, bool enabled) : base(database, enabled)
            {
                Clear();
            }

            public bool UseDataTable
            {
                get
                {
                    return Enabled;
                }
            }

            public override string ToString()
            {
                return mViewDefinitions.ToString() + " / " + mViewDefinition.ToString() + " / " + mViewRelations.ToString() + " / " + mViewRelation.ToString();
            }

            public override ArrayList ViewDefinitions(string criteriaViewDef, ref Error error)
            {
                var lstViewDefs = new ArrayList();

#if PROFILER
                SDBees.Profiler.Start("ViewCache.ViewDefinitions");
#endif

                var found = mViewDefinitions.TryGetValue(criteriaViewDef, out lstViewDefs);

                if (!found)
                {
#if PROFILER
                    SDBees.Profiler.Start("ViewCache.Select");
#endif

                    Select(database, ViewDefinitionTable, criteriaViewDef, ref lstViewDefs, ref error);

                    mViewDefinitions.Add(criteriaViewDef, lstViewDefs);

#if PROFILER
                    SDBees.Profiler.Stop();
#endif
                }

#if PROFILER
                SDBees.Profiler.Stop();
#endif

                return lstViewDefs;
            }

            public override bool ViewDefinition(object id, out ViewDefinition viewDefinition, ref Error error)
            {
                var result = false;

#if PROFILER
                SDBees.Profiler.Start("ViewCache.ViewDefinition");
#endif

                var found = mViewDefinition.TryGetValue(id, out viewDefinition);

                if (!found)
                {
#if PROFILER
                    SDBees.Profiler.Start("ViewCache.Load");
#endif

                    Object viewObject = new ViewDefinition();

                    result = Load(database, ref viewObject, id, ref error);

                    if (result)
                    {
                        viewDefinition = (ViewDefinition)viewObject;

                        mViewDefinition.Add(id, viewDefinition);
                    }
                    else
                    {
                        viewDefinition = new ViewDefinition();
                    }

#if PROFILER
                    SDBees.Profiler.Stop();
#endif
                }
                else
                {
                    result = true;
                }

#if PROFILER
                SDBees.Profiler.Stop();
#endif
                return result;
            }

            public override ArrayList ViewRelations(string criteria, ref Error error)
            {
                var objectIds = new ArrayList();

#if PROFILER
                SDBees.Profiler.Start("ViewCache.ViewRelations");
#endif

                var found = mViewRelations.TryGetValue(criteria, out objectIds);

                if (!found)
                {
#if PROFILER
                    SDBees.Profiler.Start("ViewCache.Select");
#endif

                    Select(database, ViewRelationTable, criteria, ref objectIds, ref error);

                    mViewRelations.Add(criteria, objectIds);

#if PROFILER
                    SDBees.Profiler.Stop();
#endif
                }

#if PROFILER
                SDBees.Profiler.Stop();
#endif

                return objectIds;
            }

            public override bool ViewRelation(object id, out ViewRelation viewRelation, ref Error error)
            {
                var result = false;

#if PROFILER
                SDBees.Profiler.Start("ViewCache.ViewRelation");
#endif

                var found = mViewRelation.TryGetValue(id, out viewRelation);

                if (!found)
                {
#if PROFILER
                    SDBees.Profiler.Start("ViewCache.Load");
#endif

                    Object viewObject = new ViewRelation();

                    result = Load(database, ref viewObject, id, ref error);

                    if (result)
                    {
                        viewRelation = (ViewRelation)viewObject;

                        mViewRelation.Add(id, viewRelation);
                    }
                    else
                    {
                        viewRelation = new ViewRelation();
                    }

#if PROFILER
                    SDBees.Profiler.Stop();
#endif
                }
                else
                {
                    result = true;
                }

#if PROFILER
                SDBees.Profiler.Stop();
#endif

                return result;
            }

            public override object Parent(Table table, string columnName, string idCriteria, ref Error error)
            {
                object result = null;

#if PROFILER
                SDBees.Profiler.Start("ViewCache.Parent");
#endif

                var found = mParent.TryGetValue(idCriteria, out result);

                if (!found)
                {
#if PROFILER
                    SDBees.Profiler.Start("ViewCache.SelectSingle");
#endif

                    result = SelectSingle(database, table, columnName, idCriteria, ref error);

                    mParent.Add(idCriteria, result);

#if PROFILER
                    SDBees.Profiler.Stop();
#endif
                }

#if PROFILER
                SDBees.Profiler.Stop();
#endif

                return result;
            }

            protected override void OnEnabled(bool enabled)
            {
                mViewDefinitions.Enabled = enabled;

                mViewDefinition.Enabled = enabled;

                mViewRelations.Enabled = enabled;

                mViewRelation.Enabled = enabled;

                mParent.Enabled = enabled;
            }

            protected override void OnClear()
            {
                mViewDefinitions.Clear(Enabled);

                mViewDefinition.Clear(Enabled);

                mViewRelations.Clear(Enabled);

                mViewRelation.Clear(Enabled);

                mParent.Clear(Enabled);
            }

            private int Select(Database database, Table table, string criteria, ref ArrayList values, ref Error error)
            {
                var result = 0;

                if (UseDataTable)
                {
                    values = new ArrayList();

                    var dataTable = GetDataTable(table);

                    var dataRows = dataTable.Select(criteria);

                    foreach (var dataRow in dataRows)
                    {
                        var viewObject = dataRow[table.PrimaryKey];

                        if (viewObject != null)
                        {
                            values.Add(viewObject);
                        }
                    }

                    result = values.Count;
                }
                else
                {
                    result = database.Select(table, table.PrimaryKey, criteria, ref values, ref error);
                }

                return result;
            }

            private bool Load(Database database, ref Object viewObject, object id, ref Error error)
            {
                return viewObject.Load(database, id, ref error);
            }

            private object SelectSingle(Database database, Table table, string searchColumn, string criteria, ref Error error)
            {
                if (UseDataTable)
                {
                    return database.SelectSingle(table.Name, searchColumn, criteria, ref error);
                }
                return database.SelectSingle(table.Name, searchColumn, criteria, ref error);
            }

            private DataTable GetDataTable(Table table)
            {
                DataTable dataTable = null;

                if (!m_map.TryGetValue(table, out dataTable))
                {
                    FillMaps();

                    dataTable = m_map[table];
                }

                return dataTable;
            }

            private void FillMaps()
            {
                Error error = null;

                database.Open(true, ref error);

                var viewDefinitionDataTable = SDBeesDBConnection.Current.GetDataTableForPlugin(new ViewDefinition().GetTableName);

                m_map.Add(ViewDefinitionTable, viewDefinitionDataTable);

                var viewRelationDataTable = SDBeesDBConnection.Current.GetDataTableForPlugin(new ViewRelation().GetTableName);

                m_map.Add(ViewRelationTable, viewRelationDataTable);

                database.Close(ref error);
            }

            private Dictionary<Table, DataTable> m_map = new Dictionary<Table, DataTable>();

            private Cache<string, ArrayList> mViewDefinitions = new Cache<string, ArrayList>("ViewDefinitions");

            private Cache<object, ViewDefinition> mViewDefinition = new Cache<object, ViewDefinition>("ViewDefinition");

            private Cache<string, ArrayList> mViewRelations = new Cache<string, ArrayList>("ViewRelations");

            private Cache<object, ViewRelation> mViewRelation = new Cache<object, ViewRelation>("ViewRelation");

            private Cache<string, object> mParent = new Cache<string, object>("Parent");
        }

        internal class Cache<TKey, TValue> where TValue : new()
        {
            public Cache(string name)
            {
                mName = name;

                Clear(true);
            }

            public bool Enabled
            {
                get
                {
                    return mEnabled;
                }
                set
                {
                    mEnabled = value;
                }
            }

            public void Clear(bool enabled)
            {
                mEnabled = enabled;

                mCache = null;

                mHits = 0;

                mMisses = 0;

                mPerformance = 0;
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                var result = false;

                if (mEnabled)
                {
                    if (Instance.TryGetValue(key, out value))
                    {
                        mHits += 1;

                        result = true;
                    }
                    else
                    {
                        mMisses += 1;
                    }
                }
                else
                {
                    value = new TValue();

                    mMisses += 1;
                }

                mPerformance = 100 * mHits / (mHits + mMisses);

                return result;
            }

            public void Add(TKey key, TValue value)
            {
                if (mEnabled) Instance.Add(key, value);
            }

            public string ToString()
            {
                return string.Format("{0} = ({1}, {2}, {3}, {4})", mName, mHits + mMisses, mHits, mMisses, mPerformance);
            }

            private Dictionary<TKey, TValue> Instance
            {
                get
                {
                    if (mCache == null) mCache = new Dictionary<TKey, TValue>();

                    return mCache;
                }
            }

            private bool mEnabled = true;

            private Dictionary<TKey, TValue> mCache;

            private string mName = "Cache";

            private int mHits;

            private int mMisses;

            private int mPerformance;
        }

        #endregion
    }
}
