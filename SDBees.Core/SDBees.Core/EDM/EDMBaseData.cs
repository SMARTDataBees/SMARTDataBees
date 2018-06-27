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

using System.Collections;
using SDBees.DB;

namespace SDBees.EDM
{
    class EDMBaseData : Object
    {
        #region Private Data Members

        private static Table gTable;

        #endregion

        #region Public Properties

        public static Table Table
        {
            get { return gTable; }
        }

        public string Name
        {
            get { return (string)GetPropertyByColumn("folder"); }
            set { SetPropertyByColumn("folder", value); }
        }

        public string PlugIn
        {
            get { return (string)GetPropertyByColumn("plugin"); }
            set { SetPropertyByColumn("plugin", value); }
        }

        public string ObjectId
        {
            get { return (string)GetPropertyByColumn("object_id"); }
            set { SetPropertyByColumn("object_id", value); }
        }

        public string FileSpec
        {
            get { return (string)GetPropertyByColumn("filespec"); }
            set { SetPropertyByColumn("filespec", value); }
        }

        public string FullPathname
        {
            get { return EDMManager.Current.RootDirectory + "\\" + Name; }
            set
            {
                // TBD: ...
            }
        }

        public override string GetTableName
        {
            get { return "usrEDM"; }
        }


        #endregion

        #region Constructor/Destructor

        public EDMBaseData()
        {
            base.Table = gTable;
        }

        #endregion

        #region Public Methods

        public static int FindAllRooms(Database database, ref ArrayList roomIds, ref Error error)
        {
            return database.Select(gTable, gTable.PrimaryKey, ref roomIds, ref error);
        }

        public static void InitTableSchema(Database database)
        {
            var baseData = new EDMBaseData();
            baseData.InitTableSchema(ref gTable, database);

            // Now add columns always required by this plugIn
            baseData.AddColumn(new Column("folder", DbType.String, "Verzeichnis", "Verzeichnis für dieses Plugin", "", 256, "", 0), database);
            baseData.AddColumn(new Column("plugin", DbType.String, "Plugin", "Plugin für dieses Verzeichnis", "", 256, "", 0), database);
            baseData.AddColumn(new Column("object_id", DbType.GuidString, "Objekt Id", "Objekt für dieses Verzeichnis", "", 256, "", 0), database);
            baseData.AddColumn(new Column("filespec", DbType.String, "Dateiinfo", "Dateispezifikation für dieses Verzeichnis", "", 256, "Alle Dateien (*.*)|*.*", 0), database);
        }


        #endregion

        #region Protected Methods
        /*
        protected override string TableName()
        {
            return "usrEDM";
        }
         * */

        #endregion

    }
}
