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

namespace SDBees.DB.MicrosoftLocalDB
{
    /// <summary>
    /// Class wrapping a Microsoft SQL Database
    /// </summary>
    public class MsLocalDbDatabase : Database
    {
        #region Constructor/Destructor

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        // This method creates a connection to the database...
        protected override Connection CreateConnection(bool bReadOnly, ref Error error)
        {
            MsLocalDbConnection connection = new MsLocalDbConnection(this);
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
