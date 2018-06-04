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
using System.Data;
using System.Configuration;

namespace SDBees.DB.MicrosoftLocalDB
{
    /// <summary>
    /// Class to wrap a connection to a Microsoft SQL database
    /// </summary>
    public class MsLocalDbConnection : OleConnection
    {
        #region Public Methods
        public override System.Data.DataSet GetReadOnlyDataSet()
        {
            throw new NotImplementedException();
        }

        public override System.Data.DataTable GetReadOnlyDataTable(string sTablename)
        {
            System.Data.DataTable _dtTable = new DataTable();
            System.Data.DataSet _dtSet = new DataSet();
            Error _error = null;

            string sSelect = "SELECT * FROM `" + this.Database.Name + "`.`" + sTablename + "`";

            this.FillDataSet(sSelect, ref _dtSet, ref _error, sTablename);

            foreach (DataTable tbl in _dtSet.Tables)
            {
                if (_dtSet.Tables.Contains(sTablename))
                {
                    _dtTable = _dtSet.Tables[sTablename];
                }
                else
                {
                    _dtTable = null;
                }

            }

            return _dtTable;
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// Get the connection string...
        /// </summary>
        /// <param name="database"></param>
        /// <param name="bReadOnly"></param>
        /// <returns></returns>
        protected override string ConnectionString(Database database, bool bReadOnly)
        {
            //ConfigurationManager.AppSettings["DefaultSQLDatabase"];
            string tempCon = @"Server=(localdb)\v11.0;Integrated Security=true;AttachDbFileName=C:\MyFolder\MyData.mdf;Database=";
            string connectionString = "Provider=SQLOLEDB;Data Source=" + database.Server.Name + ";Initial Catalog=" + database.Name + ";User ID=" + database.User + ";password=" + database.Password + "";

            return connectionString;
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="database"></param>
        public MsLocalDbConnection(Database database)
            : base(database)
        {
        }

        #endregion

        #region overrides
        /// <summary>
        /// The internal LocalDB connection (read-only)
        /// </summary>
        public override object GetNativeConnection()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
