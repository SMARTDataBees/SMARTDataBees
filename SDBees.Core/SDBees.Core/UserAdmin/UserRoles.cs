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

namespace SDBees.UserAdmin
{
    public class RoleDefinitions : SDBees.DB.Object
    {
        #region Private Data Members

        private static Table gTable = null;

        #endregion

        #region Public Properties

        public Guid ViewId
        {
            get { return new Guid ((string) GetPropertyByColumn("role")); }
            set { SetPropertyByColumn("role", value); }
        }

        public Type ParentType
        {
            get { return Type.ReflectionOnlyGetType((string)GetPropertyByColumn("role_name"), false, false); }
            set { SetPropertyByColumn("role_name", value); }
        }

        public Type ChildType
        {
            get { return Type.ReflectionOnlyGetType((string)GetPropertyByColumn("role_description"), false, false); }
            set { SetPropertyByColumn("role_description", value); }
        }

        public override string GetTableName
        {
            get { return "usrUserRoleDefinitions"; }
        }

        #endregion

        #region Constructor/Destructor

        public RoleDefinitions()
        {
            base.Table = gTable;
        }

        #endregion

        #region Public Methods

        public static int FindAllRoleDefinitions(Database database, ref ArrayList objectIds, ref Error error)
        {
            return database.Select(gTable, gTable.PrimaryKey, ref objectIds, ref error);
        }


        public static void InitTableSchema(Database database)
        {
            RoleDefinitions viewDefinition = new RoleDefinitions();
            viewDefinition.InitTableSchema(ref gTable, database);

            // Now add columns always required by this plugIn
            viewDefinition.AddColumn(new Column("role", DbType.eGuidString, "Role", "Role", "", 0, "", 0), database);
            viewDefinition.AddColumn(new Column("role_name", DbType.eString, "Role Name", "Role Name", "", 80, "", (int)DbFlags.eAllowNull), database);
            viewDefinition.AddColumn(new Column("role_description", DbType.eString, "Role Descritption", "Role Descritption", "", 80, "", 0), database);
        }

        #endregion

        #region Protected Methods

        /*
        protected override string TableName()
        {
            return "usrUserRoleDefinitions";
        }
         * */
        protected string GetTypeString(Type value)
        {
            if (value == null)
                return "";

            return value.ToString();
        }

        #endregion
    }
}
