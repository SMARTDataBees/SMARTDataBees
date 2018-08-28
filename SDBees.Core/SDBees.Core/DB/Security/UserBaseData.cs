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
    internal class UserBaseData : Object
    {
        #region Private Data Members

        private static Table gTable;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name the user uses to login
        /// </summary>
        public string LoginName 
        {
            get { return (string) GetPropertyByColumn("loginname"); }
            set { SetPropertyByColumn("loginname", value); }
        }

        /// <summary>
        /// Full name of the user
        /// </summary>
        public string Name
        {
            get { return (string)GetPropertyByColumn("name"); }
            set { SetPropertyByColumn("name", value); }
        }

        public string Description
        {
            get { return (string)GetPropertyByColumn("description"); }
            set { SetPropertyByColumn("description", value); }
        }

        public string Email
        {
            get { return (string)GetPropertyByColumn("email"); }
            set { SetPropertyByColumn("email", value); }
        }

        public override string GetTableName
        {
            get { return "sdbUsers"; }
        }


        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public UserBaseData()
        {
            Table = gTable;
        }

        #endregion

        #region Public Methods

        internal static void InitTableSchema(Database database)
        {
            var baseData = new UserBaseData();
            baseData.InitTableSchema(ref gTable, database);

            // Now add the name column
            var loginFlags = (int)DbFlags.eUnique;
//            baseData.AddColumn(new Column("loginname", DbType.eString, "Login Name", "Login Name", "General", 50, "Unknown", loginFlags), database);
            baseData.AddColumn(new Column("loginname", DbType.String, "Login Name", "Login Name", "General", 50, "Unknown", 0), database);
            baseData.AddColumn(new Column("name", DbType.String, "Name", "Full name of the user", "General", 100, "", 0), database);
            baseData.AddColumn(new Column("description", DbType.String, "Description", "Description of the user", "General", 255, "", 0), database);
            baseData.AddColumn(new Column("email", DbType.String, "Email Address", "Email Address for communication", "General", 255, "", 0), database);
            baseData.Table.Columns["loginname"].Editable = false;
        }

        /// <summary>
        /// Sets the default for this user
        /// </summary>
        /// <param name="database"></param>
        public override void SetDefaults(Database database)
        {
            base.SetDefaults(database);
        }

        #endregion

    }
}
