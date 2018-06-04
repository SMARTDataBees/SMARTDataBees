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
using System.Collections.Generic;
using System.Text;

namespace SDBees.DB
{
    public class RightsBaseData : SDBees.DB.Object
    {
        #region Private Data Members

        private static Table gTable = null;

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
            base.Table = gTable;
        }

        #endregion

        #region Public Methods

        internal static void InitTableSchema(Database database)
        {
            RightsBaseData baseData = new RightsBaseData();
            baseData.InitTableSchema(ref gTable, database);

            // Now add the name column
            baseData.AddColumn(new Column("type", DbType.eInt32, "Type", "Type of the Security rights", "General", 0, "", 0), database);
            baseData.AddColumn(new Column("name", DbType.eString, "Name", "Name of the server, database, table or column depending on the type", "General", 100, "", 0), database);
            baseData.AddColumn(new Column("userid", DbType.eGuidString, "User/Group Id", "Id of the user or group", "General", 100, "", 0), database);
            baseData.AddColumn(new Column("allowflags", DbType.eInt32, "Allow Flags", "Allowed operations depending on the type", "Security", 0, "", 0), database);
            baseData.AddColumn(new Column("denyflags", DbType.eInt32, "Deny Flags", "Denied operations depending on the type", "Security", 0, "", 0), database);
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
