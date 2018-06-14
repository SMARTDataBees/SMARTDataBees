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

namespace SDBees.DB.MySQL
{
    /// <summary>
    /// Class to wrap a MySQL database
    /// </summary>
    public class MySqlDatabase : Database
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
            var connection = new MySqlConnection(this);
            if (!connection.Open(this, bReadOnly, ref error))
            {
                // somehow failed...
                connection = null;
            }

            return connection;
        }

        #endregion
    }
}
