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

namespace SDBees.DB
{
    public class RightsBaseData : Object
    {
        #region Private Data Members

        private static Table gTable;

        #endregion

        #region Public Properties
        public override string GetTableName
        {
            get { return "sdbRights"; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public RightsBaseData()
        {
            Table = gTable;
        }

        #endregion

        #region Public Methods

        internal static void InitTableSchema(Database database)
        {
            var baseData = new RightsBaseData();
            baseData.InitTableSchema(ref gTable, database);

            // Now add the name column
            baseData.AddColumn(new Column("type", DbType.Int32, "Type", "Type of the Security rights", "General", 0, "", 0), database);
            baseData.AddColumn(new Column("name", DbType.String, "Name", "Name of the server, database, table or column depending on the type", "General", 100, "", 0), database);
            baseData.AddColumn(new Column("userid", DbType.GuidString, "User/Group Id", "Id of the user or group", "General", 100, "", 0), database);
            baseData.AddColumn(new Column("allowflags", DbType.Int32, "Allow Flags", "Allowed operations depending on the type", "Security", 0, "", 0), database);
            baseData.AddColumn(new Column("denyflags", DbType.Int32, "Deny Flags", "Denied operations depending on the type", "Security", 0, "", 0), database);
        }

        public override void SetDefaults(Database database)
        {
            base.SetDefaults(database);
        }

        #endregion

        #region Protected Methods
        /*
        protected override string TableName()
        {
            return "sdbRights";
        }
         * */

        #endregion

    }
}
