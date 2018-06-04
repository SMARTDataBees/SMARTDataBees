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

namespace SDBees.DB
{
    /// <summary>
    /// Class to wrap SQL Server Users
    /// </summary>
    public class User
    {
        #region Private Data Members

        private UserBaseData mBaseData;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Id of this User...
        /// </summary>
        public object Id
        {
            get { return mBaseData.Id; }
        }

        /// <summary>
        /// Name the user uses to login
        /// </summary>
        public string LoginName
        {
            get { return (string) mBaseData.GetPropertyByColumn("loginname"); }
            set { mBaseData.SetPropertyByColumn("loginname", value); }
        }

        /// <summary>
        /// Full name of the user
        /// </summary>
        public string Name
        {
            get { return (string)mBaseData.GetPropertyByColumn("name"); }
            set { mBaseData.SetPropertyByColumn("name", value); }
        }

        /// <summary>
        /// Description of the user
        /// </summary>
        public string Description
        {
            get { return (string)mBaseData.GetPropertyByColumn("description"); }
            set { mBaseData.SetPropertyByColumn("description", value); }
        }

        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email
        {
            get { return (string)mBaseData.GetPropertyByColumn("email"); }
            set { mBaseData.SetPropertyByColumn("email", value); }
        }

        /// <summary>
        /// Returns the base data for this object
        /// </summary>
        public SDBees.DB.Object BaseData
        {
            get { return mBaseData; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public User()
        {
            mBaseData = new UserBaseData();
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public User(Server server)
        {
            mBaseData = new UserBaseData();
            mBaseData.SetDefaults(server.SecurityDatabase);
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public User(Server server, object objectId)
        {
            mBaseData = new UserBaseData();

            Error error = null;
            mBaseData.Load(server.SecurityDatabase, objectId, ref error);

            Error.Display("Unable to load User", error);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Find a user by the login name
        /// </summary>
        /// <param name="server"></param>
        /// <param name="loginName"></param>
        /// <returns>The found user or null if not found</returns>
        public static User FindUser(Server server, string loginName)
        {
            User user = null;

            Error error = null;

            Database database = server.SecurityDatabase;

            UserBaseData baseData = new UserBaseData();
            Attribute attribute = new Attribute(baseData.Table.Columns["loginname"], loginName);
            string criteria = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);

            ArrayList values = new ArrayList();
            int numFound = database.Select(baseData.Table, baseData.Table.PrimaryKey, criteria, ref values, ref error);

            if (numFound == 1)
            {
                user = new User();
                user.Load(database, values[0], ref error);
            }

            Error.Display("FindUser failed!", error);

            return user;
        }

        /// <summary>
        /// Get all the login names on this server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="loginNames"></param>
        /// <returns></returns>
        public static int GetAllLogins(Server server, ref ArrayList loginNames)
        {
            int loginCount = 0;

            Error error = null;
            UserBaseData baseData = new UserBaseData();

            loginCount = server.SecurityDatabase.Select(baseData.Table, "loginname", ref loginNames, ref error);

            Error.Display("GetAllLogins failed!", error);

            return loginCount;
        }

        /// <summary>
        /// Get all the login names on this server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="loginNames"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int GetAllLogins(Server server, ref ArrayList loginNames, ref Error error)
        {
            int loginCount = 0;

            UserBaseData baseData = new UserBaseData();

            loginCount = server.SecurityDatabase.Select(baseData.Table, "loginname", ref loginNames, ref error);

            return loginCount;
        }

        /// <summary>
        /// Update the database access for this user to the server
        /// </summary>
        /// <param name="error">Error description if this function fails</param>
        /// <returns>true if successful</returns>
        public bool UpdateAccessRightsOnServer(ref Error error)
        {
            bool success = false;

            Server server = mBaseData.Database.Server;

            if (!server.RemoveAllPrivileges(LoginName, ref error))
                return false;

            if (!server.GrantDatabasePrivileges(LoginName, server.SecurityDatabase.Name, AccessFlags.SelectRows, ref error))
                return false;

            if (!server.GrantStandardPrivileges(LoginName, ref error))
                return false;

            ArrayList objectIds = null;
            if (AccessRights.GetAccessRightsForUserId(server, Id.ToString(), ref objectIds) == 0)
            {
                return true;
            }

            success = true;

            foreach (object objectId in objectIds)
            {
                AccessRights accessRight = new AccessRights(server, objectId);

                success = success && accessRight.UpdateAccessRightsOnServer(server, LoginName, ref error);
            }

            return success;
        }

        /// <summary>
        /// Save the user to the database
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Save(ref Error error)
        {
            bool success = false;

            if ((error == null) && (mBaseData.Database != null))
            {
                Server server = mBaseData.Database.Server;
                if (!server.UserExists(LoginName, ref error))
                {
                    server.CreateUser(this, ref error);
                }

                if (error == null)
                {
                    success = mBaseData.Save(ref error);
                }
            }

            return success;
        }

        /// <summary>
        /// Delete the user from the server
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Remove(ref Error error)
        {
            bool success = false;

            if ((error == null) && (mBaseData.Database != null))
            {
                Server server = mBaseData.Database.Server;
                if (server.UserExists(LoginName, ref error))
                {
                    server.RemoveUser(LoginName, ref error);
                }

                if (error == null)
                {
                    success = mBaseData.Erase(ref error);
                }
            }

            return success;
        }

        /// <summary>
        /// Set password for the user
        /// </summary>
        /// <param name="password"></param>
        /// <param name="error"></param>
        public bool SetPassword(string password, ref Error error)
        {
            bool success = false;

            if ((error == null) && (mBaseData != null) && (mBaseData.Database != null))
            {
                mBaseData.Database.Server.SetPassword(LoginName, password, ref error);

                success = (error == null);
            }

            return success;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Load a user from the database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="userId"></param>
        /// <param name="error"></param>
        /// <returns>true if successful</returns>
        protected bool Load(Database database, object userId, ref Error error)
        {
            mBaseData = new UserBaseData();
            return mBaseData.Load(database, userId, ref error);
        }

        #endregion
    }
}
