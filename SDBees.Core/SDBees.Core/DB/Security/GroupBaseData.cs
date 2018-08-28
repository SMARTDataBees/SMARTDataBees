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

namespace SDBees.DB
{
    public class GroupBaseData : Object
    {
        #region Private Data Members

        private static Table gTable;

        #endregion

        #region Public Properties

        public string Name 
        {
            get { return (string) GetPropertyByColumn("name"); }
            set { SetPropertyByColumn("name", value); }
        }

        public string Description
        {
            get { return (string)GetPropertyByColumn("description"); }
            set { SetPropertyByColumn("description", value); }
        }

        public string ParentId
        {
            get { return (string)GetPropertyByColumn("parentid"); }
            set { SetPropertyByColumn("parentid", value); }
        }

        public override string GetTableName
        {
            get { return "sdbGroups"; }
        }


        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public GroupBaseData()
        {
            Table = gTable;
        }

        #endregion

        #region Public Methods

        internal static void InitTableSchema(Database database)
        {
            var baseData = new GroupBaseData();
            baseData.InitTableSchema(ref gTable, database);

            // Now add the name column
            var uniqueFlags = (int)DbFlags.eUnique;
//            baseData.AddColumn(new Column("name", DbType.eString, "Name", "Name of the group", "General", 50, "Unknown Group", uniqueFlags), database);
            baseData.AddColumn(new Column("name", DbType.String, "Name", "Name of the group", "General", 50, "Unknown Group", 0), database);
            baseData.AddColumn(new Column("description", DbType.String, "Description", "Description of the group", "General", 255, "", 0), database);
            baseData.AddColumn(new Column("parentid", DbType.GuidString, "Parent Id", "Id of the parent of the group", "General", 0, "", 0), database);
            baseData.Table.Columns["parentid"].Editable = false;

            CreateDefaultGroups(database);
        }

        public override void SetDefaults(Database database)
        {
            base.SetDefaults(database);

            ParentId = Guid.Empty.ToString();
        }

        #endregion

        #region Protected Methods

        /*
        protected override string TableName()
        {
            return "sdbGroups";
        }
         * */

        private static void CreateDefaultGroups(Database database)
        {
            Error error = null;
            ArrayList objectIds = null;
            var numFound = database.Select(gTable, gTable.PrimaryKey, ref objectIds, ref error);

            if (numFound == 0)
            {
                var rootGroup = new GroupBaseData();
                rootGroup.SetDefaults(database);

                rootGroup.Name = "Alle Benutzer";
                rootGroup.Description = "Jeder Benutzer ist in dieser Gruppe.";

                rootGroup.Save(ref error);

                if (error == null)
                {
                    var administrators = new GroupBaseData();
                    administrators.SetDefaults(database);

                    administrators.Name = "Administratoren";
                    administrators.Description = "Administratoren haben alle Rechte auf dem Server.";
                    administrators.ParentId = rootGroup.Id.ToString();

                    administrators.Save(ref error);

                    var users = new GroupBaseData();
                    users.SetDefaults(database);

                    users.Name = "Benutzer";
                    users.Description = "Benutzer haben eingeschränkte Rechte und sind somit meist zu verwenden.";
                    users.ParentId = rootGroup.Id.ToString();

                    users.Save(ref error);
                }
            }

            Error.Display("Cannot create default groups", error);
        }

        #endregion

    }
}
