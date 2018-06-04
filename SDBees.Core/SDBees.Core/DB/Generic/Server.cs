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
using System.Configuration;

namespace SDBees.DB
{
    /// <summary>
    /// Base Class for wrapping SQL Servers
    /// </summary>
    public abstract class Server
    {
        #region Private Data Members

        private string mName;
        private string mVendor;
        private string mSystemSchemaName;
        private string mSuperUser;
        private string mPassword;
        private string mPort;
        private Database mSecurityDatabase;

        private DB.Generic.ServerConfigItem mSrvConfig;

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="vendor">Name of the Vendor (only for display)</param>
        /// <param name="systemSchemaName">Name of the schema table for this SQL Server</param>
        /// <param name="userName">Username for login</param>
        /// <param name="password">Password for login</param>
        public Server(string vendor, string systemSchemaName, DB.Generic.ServerConfigItem srvConfig, string password)
        {
            mSrvConfig = srvConfig;
            mVendor = vendor;
            mName = "";
            mSystemSchemaName = systemSchemaName;
            mSuperUser = srvConfig.UserName;
            mPassword = password;
            mPort = srvConfig.ServerPort;

            //mSecurityDatabase = GetDatabase("SDBsecurity");
			mSecurityDatabase = GetDatabase(srvConfig.ServerSecureDatabase);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of this SQL Server (Machine name or IP Number)
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Name of the Vendor of this SQL Server, only used for display
        /// </summary>
        public string Vendor
        {
            get { return mVendor; }
            set { mVendor = value; }
        }

        /// <summary>
        /// Name of the schema table of this Database server. This table contains all the
        /// information about the existing databases and their schema.
        /// </summary>
        public string SystemSchemaName
        {
            get { return mSystemSchemaName; }
            set { mSystemSchemaName = value; }
        }

        /// <summary>
        /// The login name of the super user to administer the database server
        /// </summary>
        public string SuperUser
        {
            get { return mSuperUser; }
            set
            {
                mSuperUser = value;

                if (mSecurityDatabase != null)
                {
                    mSecurityDatabase.User = value;
                }
            }
        }

        /// <summary>
        /// The password of the super user to administer the database server
        /// </summary>
        public string Password
        {
            get { return mPassword; }
            set
            {
                mPassword = value;

                if (mSecurityDatabase != null)
                {
                    mSecurityDatabase.Password = value;
                }
            }
        }

        /// <summary>
        /// The port to communicate with the SQL server
        /// </summary>
        public string Port
        {
            get { return mPort; }
            set { mPort = value; }
        }

        /// <summary>
        /// Security database containing all the login information and the access rights for SDBees
        /// </summary>
        public Database SecurityDatabase
        {
            get { return mSecurityDatabase; }
            set { mSecurityDatabase = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sollte zu beginn aufgerufen werden, damit die Security Datenbank etc. entsprechend
        /// eingerichtet werden kann.
        /// </summary>
        public void Init()
        {
            InitSecurity();
        }

        /// <summary>
        /// Each server instance must report the own serverconfigitem
        /// </summary>
        /// <returns></returns>
        public abstract SDBees.DB.Generic.ServerConfigItem GetServerConfigItem();

        /// <summary>
        /// Get the list of databases on this server
        /// </summary>
        /// <param name="databaseNames">returned list of strings representing the database names</param>
        /// <param name="includeSystem">When true also include system databases</param>
        /// <param name="error">Error description if this function fails</param>
        /// <returns>number of databases found</returns>
        public int GetDatabases(ref ArrayList databaseNames, bool includeSystem, ref Error error)
        {
            int numDatabases = 0;
            Database schemaDb = GetSystemSchemaDB();
            Connection conn = schemaDb.Open(true, ref error);
            if (conn != null)
            {
                // TBD: consider includeSystem
                numDatabases = schemaDb.Select("INFORMATION_SCHEMA.SCHEMATA", "SCHEMA_NAME", ref databaseNames, ref error);

                schemaDb.Close(ref error);
            }

            return numDatabases;
        }

        /// <summary>
        /// Gets the database with that name, it is not checked if it exists
        /// </summary>
        /// <param name="dbName"></param>
        /// <returns></returns>
        public Database GetDatabase(string dbName)
        {
            Database database = NewDatabase();
            database.Server = this;
            database.Name = dbName;
            database.Port = Port;
            database.User = SuperUser;
            database.Password = Password;
            database.UseTableNameCaching = (mSrvConfig.ServerTableCaching);
            database.UseGlobalCaching = false; // Don't Use global caching!

            return database;
        }

        /// <summary>
        /// Check if a database exists on this server
        /// </summary>
        /// <param name="databaseName">Name of the database</param>
        /// <param name="error">Error description if this function fails</param>
        /// <returns></returns>
        public bool DatabaseExists(string databaseName, ref Error error)
        {
            bool foundSchema = false;
            Database schemaDb = GetSystemSchemaDB();
            Connection conn = schemaDb.Open(true, ref error);
            if (conn != null)
            {
                string criteria = "SCHEMA_NAME = '" + databaseName + "'";
                ArrayList names = null;
                int numDatabases = schemaDb.Select("INFORMATION_SCHEMA.SCHEMATA", "SCHEMA_NAME", criteria, ref names, ref error);

                foundSchema = (numDatabases == 1);

                schemaDb.Close(ref error);
            }

            return foundSchema;
        }

        /// <summary>
        /// Create a database on this server
        /// </summary>
        /// <param name="databaseName">Name of the database</param>
        /// <param name="error">Error description if this function fails</param>
        /// <returns></returns>
        public bool CreateDatabase(string databaseName, ref Error error)
        {
            bool success = false;

            Database schemaDb = GetSystemSchemaDB();
            Connection conn = schemaDb.Open(false, ref error);
            if (conn != null)
            {
                string commandString = "CREATE DATABASE " + databaseName;

                success = conn.ExecuteCommand(commandString, ref error);

                schemaDb.Close(ref error);
            }

            return success;
        }

        /// <summary>
        /// Remove all access rights for a user from this server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public virtual bool RemoveAllPrivileges(string loginName, ref Error error)
        {
            return RevokeServerPrivileges(loginName, AccessFlags.All, ref error);
        }

        /// <summary>
        /// Update the server access for this user to the server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="accessMask">Bit mask of what is to grant, see AccessRights for details</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public abstract bool GrantServerPrivileges(string loginName, int accessMask, ref Error error);

        /// <summary>
        /// Update the server access for this user to the server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="accessMask">Bit mask of what is to revoke, see AccessRights for details</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public abstract bool RevokeServerPrivileges(string loginName, int accessMask, ref Error error);

        /// <summary>
        /// Update the database access for this user to the server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="databaseName">Name of the database to grant privileges for</param>
        /// <param name="accessMask">Bit mask of what is to grant, see AccessRights for details</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public abstract bool GrantDatabasePrivileges(string loginName, string databaseName, int accessMask, ref Error error);

        /// <summary>
        /// Grant standard privileges to the user on this server
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if successfule</returns>
        public virtual bool GrantStandardPrivileges(string loginName, ref Error error)
        {
            return true;
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
        public abstract bool RevokeDatabasePrivileges(string loginName, string databaseName, int accessMask, ref Error error);

        /// <summary>
        /// Check if the user has the privilege to grant user access
        /// </summary>
        /// <param name="loginName">Login name of the user</param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns>true if user has the grant privilege</returns>
        public abstract bool UserHasGrantPrivileges(string loginName, ref Error error);

        /// <summary>
        /// Check if there is a user with this login
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public abstract bool UserExists(string loginName, ref Error error);

        /// <summary>
        /// Create a user login
        /// </summary>
        /// <param name="user"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public abstract bool CreateUser(User user, ref Error error);

        /// <summary>
        /// Remove a user login from the server
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public abstract bool RemoveUser(string loginName, ref Error error);

        /// <summary>
        /// Set the password for this login
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public abstract bool SetPassword(string loginName, string password, ref Error error);

        /// <summary>
        /// Change the password for this login
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        /// <param name="error">Contains error information if this fails</param>
        /// <returns></returns>
        public abstract bool ChangePassword(string loginName, string oldPassword, string newPassword, ref Error error);

        #endregion

        #region Protected Methods

        /// <summary>
        /// Initialize the security Database. This will create the database if it doesn't exist
        /// and add all the required tables for users, groups and rights.
        /// </summary>
        protected void InitSecurity()
        {
            if (mSecurityDatabase == null)
            {
                throw new Exception("SecurityDatabase should be initialized in " + this.GetType().ToString() + "!");
            }

            Error error = null;
            if (!DatabaseExists(mSecurityDatabase.Name, ref error) && (error == null))
            {
                CreateDatabase(mSecurityDatabase.Name, ref error);
            }

            if (error == null)
            {
                // Verify that the required Tables are created/updated in the database
                TableSchema.InitTableSchema(mSecurityDatabase);

                UserBaseData.InitTableSchema(mSecurityDatabase);
                GroupBaseData.InitTableSchema(mSecurityDatabase);
                RightsBaseData.InitTableSchema(mSecurityDatabase);

                // TBD: Gamal - Die weiteren verschiedenen Security Tabellen erstellen...
            }

            Error.Display("InitSecurity Failed", error);
        }

        /// <summary>
        /// Derived classes should override this and return a matching database
        /// </summary>
        /// <returns></returns>
        protected abstract Database NewDatabase();

        /// <summary>
        /// Gets the database for the system schema
        /// </summary>
        /// <returns></returns>
        protected Database GetSystemSchemaDB()
        {
            return GetDatabase(mSystemSchemaName);
        }


        #endregion
    }
}
