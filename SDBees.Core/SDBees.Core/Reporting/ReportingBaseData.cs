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

namespace SDBees.Reporting
{
    class ReportingBaseData : Object
    {
        #region Private Data Members

        private static Table gTable;

        #endregion

        #region Public Properties

        public static Table Table
        {
            get { return gTable; }
        }

        public override string GetTableName
        {
            get { return "usrReporting"; }
        }


        #endregion

        #region Constructor/Destructor

        public ReportingBaseData()
        {
            base.Table = gTable;
        }

        #endregion

        #region Public Methods

        public static int FindAllReportings(Database database, ref ArrayList roomIds, ref Error error)
        {
            return database.Select(gTable, gTable.PrimaryKey, ref roomIds, ref error);
        }

        public static void InitTableSchema(Database database)
        {
            var baseData = new ReportingBaseData();
            baseData.InitTableSchema(ref gTable, database);

            // Now add columns always required by this plugIn
        }


        #endregion

        #region Protected Methods
        /*
        protected override string TableName()
        {
            return "usrReporting";
        }
         */

        #endregion

    }
}
