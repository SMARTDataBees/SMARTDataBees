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

namespace SDBees.Core.Admin
{
    public partial class ViewCache
    {
       
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
                return _viewDefinitions.ToString() + " / " + _viewDefinition.ToString() + " / " + _viewRelations.ToString() + " / " + _viewRelation.ToString();
            }

            public override ArrayList ViewDefinitions(string criteriaViewDef, ref Error error)
            {
                var lstViewDefs = new ArrayList();

#if PROFILER
                SDBees.Profiler.Start("ViewCache.ViewDefinitions");
#endif

                var found = _viewDefinitions.TryGetValue(criteriaViewDef, out lstViewDefs);

                if (!found)
                {
#if PROFILER
                    SDBees.Profiler.Start("ViewCache.Select");
#endif

                    Select(database, ViewDefinitionTable, criteriaViewDef, ref lstViewDefs, ref error);

                    _viewDefinitions.Add(criteriaViewDef, lstViewDefs);

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

                var found = _viewDefinition.TryGetValue(id, out viewDefinition);

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

                        _viewDefinition.Add(id, viewDefinition);
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

                var found = _viewRelations.TryGetValue(criteria, out objectIds);

                if (!found)
                {
#if PROFILER
                    SDBees.Profiler.Start("ViewCache.Select");
#endif

                    Select(database, ViewRelationTable, criteria, ref objectIds, ref error);

                    _viewRelations.Add(criteria, objectIds);

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

                var found = _viewRelation.TryGetValue(id, out viewRelation);

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

                        _viewRelation.Add(id, viewRelation);
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

                var found = _parent.TryGetValue(idCriteria, out result);

                if (!found)
                {
#if PROFILER
                    SDBees.Profiler.Start("ViewCache.SelectSingle");
#endif

                    result = SelectSingle(database, table, columnName, idCriteria, ref error);

                    _parent.Add(idCriteria, result);

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
                _viewDefinitions.Enabled = enabled;

                _viewDefinition.Enabled = enabled;

                _viewRelations.Enabled = enabled;

                _viewRelation.Enabled = enabled;

                _parent.Enabled = enabled;
            }

            protected override void OnClear()
            {
                _viewDefinitions.Clear(Enabled);

                _viewDefinition.Clear(Enabled);

                _viewRelations.Clear(Enabled);

                _viewRelation.Clear(Enabled);

                _parent.Clear(Enabled);
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
                if (!_map.TryGetValue(table, out var dataTable))
                {
                    FillMaps();
                    dataTable = _map[table];
                }

                return dataTable;
            }

            private void FillMaps()
            {
                Error error = null;

                database.Open(true, ref error);

                var viewDefinitionDataTable = SDBeesDBConnection.Current.GetDataTableForPlugin(new ViewDefinition().GetTableName);

                _map.Add(ViewDefinitionTable, viewDefinitionDataTable);

                var viewRelationDataTable = SDBeesDBConnection.Current.GetDataTableForPlugin(new ViewRelation().GetTableName);

                _map.Add(ViewRelationTable, viewRelationDataTable);

                database.Close(ref error);
            }

            private readonly Dictionary<Table, DataTable> _map = new Dictionary<Table, DataTable>();

            private readonly Cache<string, ArrayList> _viewDefinitions = new Cache<string, ArrayList>("ViewDefinitions");

            private readonly Cache<object, ViewDefinition> _viewDefinition = new Cache<object, ViewDefinition>("ViewDefinition");

            private readonly Cache<string, ArrayList> _viewRelations = new Cache<string, ArrayList>("ViewRelations");

            private readonly Cache<object, ViewRelation> _viewRelation = new Cache<object, ViewRelation>("ViewRelation");

            private readonly Cache<string, object> _parent = new Cache<string, object>("Parent");
        }
    }
}
