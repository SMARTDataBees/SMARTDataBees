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
using SDBees.DB;

namespace SDBees.Core.Admin
{
    public partial class ViewCache
    {
       
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

    }
}
