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
    /// Class wrapping a Microsoft SQL Server
    /// </summary>
    public class MsLocalDbServer : Server
    {
        #region Private Data Members
        DB.Generic.ServerConfigItem m_SrvConfig;
        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// <param name="userName">Username for login</param>
        /// <param name="password">Password for login</param>
        /// </summary>
        public MsLocalDbServer(DB.Generic.ServerConfigItem srcConfig, string password)
            : base("MicrosoftSQL", "master", srcConfig, password)
        {
            m_SrvConfig = srcConfig;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Update the server access for this user to the server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="accessMask">Bit mask of what is to grant, see AccessRights for details</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public override bool GrantServerPrivileges(string loginName, int accessMask, ref Error error)
        {
            // TBD: Implement this...
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Update the server access for this user to the server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="accessMask">Bit mask of what is to revoke, see AccessRights for details</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public override bool RevokeServerPrivileges(string loginName, int accessMask, ref Error error)
        {
            // TBD: Implement this...
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Update the database access for this user to the server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="databaseName">Name of the database to grant privileges for</param>
        /// <param name="accessMask">Bit mask of what is to grant, see AccessRights for details</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public override bool GrantDatabasePrivileges(string loginName, string databaseName, int accessMask, ref Error error)
        {
            // TBD: Implement this...
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Update the database access for this user to the server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="databaseName">Name of the database to revoke privileges from</param>
        /// <param name="grant">grant if true, revoke if false</param>
        /// <param name="accessMask">Bit mask of what is to revoke, see AccessRights for details</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public override bool RevokeDatabasePrivileges(string loginName, string databaseName, int accessMask, ref Error error)
        {
            // TBD: Implement this...
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Check if the user has the privilege to grant user access
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if user has the grant privilege</returns>
        public override bool UserHasGrantPrivileges(string loginName, ref Error error)
        {
            // TBD: Implement this...
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Check if there is a user with this login
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public override bool UserExists(string loginName, ref Error error)
        {
            // TBD: Implement this...
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Create a user login
        /// </summary>
        /// <param name="user"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public override bool CreateUser(User user, ref Error error)
        {
            // TBD: sp_addlogin verwenden
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Remove a user login from the server
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public override bool RemoveUser(string loginName, ref Error error)
        {
            // TBD: sp_droplogin verwenden
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Set the password for this login
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public override bool SetPassword(string loginName, string password, ref Error error)
        {
            // TBD: sp_password verwenden
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        /// <summary>
        /// Change the password for this login
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public override bool ChangePassword(string loginName, string oldPassword, string newPassword, ref Error error)
        {
            // TBD: sp_password verwenden
            error = new Error("Not implemented yet", 1, this.GetType(), error);
            return false;
        }

        public override Generic.ServerConfigItem GetServerConfigItem()
        {
            return m_SrvConfig;
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// return a matching database (Microsoft SQL)
        /// </summary>
        /// <returns></returns>
        protected override Database NewDatabase()
        {
            return new MsLocalDbDatabase();
        }

        #endregion
    }
}
