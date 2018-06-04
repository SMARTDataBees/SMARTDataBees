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
using System.Collections;
using System.Text;

namespace SDBees.DB.SQLite
{
    /// <summary>
    /// Class as a wrapper to SQLight server
    /// </summary>
    public class SQLiteServer : Server
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
        public SQLiteServer(DB.Generic.ServerConfigItem srcConfig, string password)
            : base("SQLight", "information_schema", srcConfig, password)
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
            // This doesn't change anything, just exit...
            if (accessMask == 0)
                return true;

            bool success = false;

            string options = "";
            string privileges = FormatServerPrivileges(accessMask, true, ref options);

            string commandString = "GRANT " + privileges + " ON *.* TO '" + loginName + "' " + options;

            Database sqlightDb = GetDatabase("sqlight");
            success = sqlightDb.ExecuteCommand(commandString, ref error);

            return success;
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
            // This doesn't change anything, just exit...
            if (accessMask == 0)
                return true;

            bool success = false;

            string options = "";
            string privileges = FormatServerPrivileges(accessMask, false, ref options);

            string commandString = "REVOKE " + privileges + " FROM '" + loginName + "' " + options;

            Database mysqlDb = GetDatabase("sqlight");
            success = mysqlDb.ExecuteCommand(commandString, ref error);

            return success;
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
            // This doesn't change anything, just exit...
            if (accessMask == 0)
                return true;

            bool success = false;

            string options = "";
            string privileges = FormatDatabasePrivileges(accessMask, true, ref options);

            string commandString = "GRANT " + privileges + " ON " + databaseName + ".* TO '" + loginName + "' " + options;

            Database mysqlDb = GetDatabase("sqlight");
            success = mysqlDb.ExecuteCommand(commandString, ref error);

            return success;
        }

        /// <summary>
        /// Grant standard privileges to the user on this server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public override bool GrantStandardPrivileges(string loginName, ref Error error)
        {
            return GrantDatabasePrivileges(loginName, "sqlight", AccessFlags.SelectRows, ref error);
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
            // This doesn't change anything, just exit...
            if (accessMask == 0)
                return true;

            bool success = false;

            string options = "";
            string privileges = FormatDatabasePrivileges(accessMask, false, ref options);

            string commandString = "REVOKE " + privileges + " ON " + databaseName + ".* FROM '" + loginName + "' " + options;

            Database mysqlDb = GetDatabase("sqlight");
            success = mysqlDb.ExecuteCommand(commandString, ref error);

            return success;
        }

        /// <summary>
        /// Check if the user has the privilege to grant user access
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if user has the grant privilege</returns>
        public override bool UserHasGrantPrivileges(string loginName, ref Error error)
        {
            bool result = false;

            Database mysqlDb = GetDatabase("sqlight");

            string criteria = "User = '" + loginName + "'";
            ArrayList values = new ArrayList();
            if (mysqlDb.Select("user", "grant_priv", criteria, ref values, ref error) > 0)
            {
                string value = (string)values[0];
                value = value.ToUpper();
                result = (value == "Y");
            }

            return result;
        }

        /// <summary>
        /// Check if there is a user with this login
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public override bool UserExists(string loginName, ref Error error)
        {
            Database mysqlDb = GetDatabase("sqlight");

            string criteria = "User = '" + loginName + "'";
            ArrayList values = new ArrayList();
            int numFound = mysqlDb.Select("user", "User", criteria, ref values, ref error);

            return numFound > 0;
        }

        /// <summary>
        /// Create a user login
        /// </summary>
        /// <param name="user"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public override bool CreateUser(User user, ref Error error)
        {
            bool success = false;

            Database mysqlDb = GetDatabase("sqlight");

            string sqlCommand = "CREATE USER '" + user.LoginName + "' IDENTIFIED BY 'password'";
            success = mysqlDb.ExecuteCommand(sqlCommand, ref error);

            return success;
        }

        /// <summary>
        /// Remove a user login from the server
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public override bool RemoveUser(string loginName, ref Error error)
        {
            bool success = false;

            Database mysqlDb = GetDatabase("sqlight");

            string sqlCommand = "DROP USER '" + loginName + "'";
            success = mysqlDb.ExecuteCommand(sqlCommand, ref error);

            return success;
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
            bool success = false;

            Database mysqlDb = GetDatabase("sqlight");

            string sqlCommand = "SET PASSWORD FOR '" + loginName + "' = PASSWORD('" + password + "')";
            success = mysqlDb.ExecuteCommand(sqlCommand, ref error);

            return success;
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
            bool success = false;

            Database mysqlDb = GetDatabase("sqlight");
            // Use the old password to verify that it's correct
            mysqlDb.Password = oldPassword;

            string sqlCommand = "SET PASSWORD FOR '" + loginName + "' = PASSWORD('" + newPassword + "')";
            success = mysqlDb.ExecuteCommand(sqlCommand, ref error);

            return success;
        }

        public override Generic.ServerConfigItem GetServerConfigItem()
        {
            return m_SrvConfig;
        }
        #endregion

        #region Protected Methods

        /// <summary>
        /// return a matching database (MySQL)
        /// </summary>
        /// <returns></returns>
        protected override Database NewDatabase()
        {
            return new SQLiteDatabase();
        }

        /// <summary>
        /// Format the flags for the GRANT or REVOKE command on servers
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="useGrantSyntax"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected string FormatServerPrivileges(int flags, bool useGrantSyntax, ref string options)
        {
            string result = "";
            options = "";

            if (flags == AccessFlags.All)
            {
                result += "ALL";
            }
            else
            {
                // Tabellen Rechte
                if ((flags & AccessFlags.CreateDatabase) != 0)
                {
                    result = AddCommaSeparated(result, "CREATE");
                }
                if ((flags & AccessFlags.EditDatabase) != 0)
                {
                    result = AddCommaSeparated(result, "ALTER");
                }
                if ((flags & AccessFlags.DeleteDatabase) != 0)
                {
                    result = AddCommaSeparated(result, "DROP");
                }

                // Benutzer Rechte
                if ((flags & (AccessFlags.CreateUser | AccessFlags.EditUser | AccessFlags.DeleteUser)) != 0)
                {
                    result = AddCommaSeparated(result, "CREATE USER");
                }
            }

            if ((flags & (AccessFlags.EditUser | AccessFlags.EditGroup)) != 0)
            {
                // TBD: Check this users rights????
                if (useGrantSyntax)
                {
                    options = AddCommaSeparated(options, "GRANT OPTION");
                }
                else
                {
                    result = AddCommaSeparated(result, "GRANT OPTION");
                }
            }

            if (options != "")
            {
                options = "WITH " + options;
            }

            return result;
        }

        /// <summary>
        /// Format the flags for the GRANT or REVOKE command on databases
        /// </summary>
        /// <param name="flags"></param>
        /// <param name="useGrantSyntax"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        protected string FormatDatabasePrivileges(int flags, bool useGrantSyntax, ref string options)
        {
            string result = "";
            options = "";

            if (flags == AccessFlags.All)
            {
                result += "ALL";
            }
            else
            {
                // Tabellen Rechte
                if ((flags & AccessFlags.CreateTable) != 0)
                {
                    result = AddCommaSeparated(result, "CREATE");
                }
                if ((flags & AccessFlags.EditTable) != 0)
                {
                    result = AddCommaSeparated(result, "ALTER");
                }
                if ((flags & AccessFlags.DeleteTable) != 0)
                {
                    result = AddCommaSeparated(result, "DROP");
                }

                // Benutzer Rechte
                if ((flags & (AccessFlags.CreateDbUser | AccessFlags.EditDbUser | AccessFlags.DeleteDbUser)) != 0)
                {
                    result = AddCommaSeparated(result, "CREATE USER");
                }

                // Zeilen Rechte
                if ((flags & AccessFlags.SelectRows) != 0)
                {
                    result = AddCommaSeparated(result, "SELECT");
                }
                if ((flags & AccessFlags.CreateRows) != 0)
                {
                    result = AddCommaSeparated(result, "INSERT");
                }
                if ((flags & AccessFlags.EditRows) != 0)
                {
                    result = AddCommaSeparated(result, "UPDATE");
                }
                if ((flags & AccessFlags.DeleteRows) != 0)
                {
                    result = AddCommaSeparated(result, "DELETE");
                }
            }

            if ((flags & (AccessFlags.EditDbUser | AccessFlags.EditDbGroup)) != 0)
            {
                // TBD: Check this users rights????
                if (useGrantSyntax)
                {
                    options = AddCommaSeparated(options, "GRANT OPTION");
                }
                else
                {
                    result = AddCommaSeparated(result, "GRANT OPTION");
                }
            }

            if (options != "")
            {
                options = "WITH " + options;
            }

            return result;
        }

        /// <summary>
        /// Adds a string to a complete string and separates with commas if required
        /// </summary>
        /// <param name="string1"></param>
        /// <param name="string2"></param>
        /// <returns></returns>
        protected string AddCommaSeparated(string string1, string string2)
        {
            string result = string1;
            if (result != "")
            {
                result += ", ";
            }
            result += string2;

            return result;
        }

        #endregion
    }
}
