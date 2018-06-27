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
using System.Data;
using System.Data.SQLite;

namespace SDBees.DB.SQLite
{
    /// <summary>
    /// Class to wrap a MySQL database
    /// </summary>
    public class SQLiteDatabase : Database
    {
        #region Constructor/Destructor

        #endregion

        #region Public Methods
        #endregion

        #region Protected Methods

        /// <summary>
        /// This method creates a connection to the database...
        /// </summary>
        /// <param name="bReadOnly"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected override Connection CreateConnection(bool bReadOnly, ref Error error)
        {
            var connection = new SQLiteConnection(this);
            if (!connection.Open(this, bReadOnly, ref error))
            {
                // somehow failed...
                connection = null;
            }

            return connection;
        }

        #endregion

        #region Overrides
        public override int TableNames(ref ArrayList names, ref Error error)
        {
            Error _error = null;

            var select = "SELECT * FROM sqlite_master;";

            names = GetTables(ref _error);

            Error.Display("Error while fetching table names", _error);

            return names.Count;
        }

        #endregion


        private ArrayList GetTables(ref Error _error)
        {
            var list = new ArrayList();

            // executes query that select names of all tables in master table of the database
            var query = "SELECT name FROM sqlite_master " +
                    "WHERE type = 'table'" +
                    "ORDER BY 1";
            try
            {

                var table = GetDataTable(query, ref _error);

                // Return all table names in the ArrayList
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        list.Add(row.ItemArray[0].ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }

        private DataTable GetDataTable(string sql, ref Error _error)
        {
            try
            {
                var dt = new DataTable();
                SDBeesDBConnection.Current.Database.Open(true, ref _error);

                using (var cmd = new SQLiteCommand(sql, SDBeesDBConnection.Current.Database.Connection.GetNativeConnection() as System.Data.SQLite.SQLiteConnection))
                {
                    using (var rdr = cmd.ExecuteReader())
                    {
                        dt.Load(rdr);
                    }
                }

                SDBeesDBConnection.Current.Database.Close(ref _error);
                return dt;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
