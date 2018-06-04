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
    /// Access type for access rights
    /// </summary>
    public enum AccessType
    {
        /// <summary>
        /// Server based access right
        /// </summary>
        Server = 0,

        /// <summary>
        /// Database based access rights
        /// </summary>
        Database = 1,

        /// <summary>
        /// Table based access rights
        /// </summary>
        Table = 2,

        /// <summary>
        /// Column based access rights
        /// </summary>
        Column = 3
    }

    /// <summary>
    /// Flag definitions
    /// </summary>
    public struct AccessFlags
    {
        #region Public Constants

        // Server Privileges:

        public const int CreateDatabase = 0x00000001;
        public const int EditDatabase   = 0x00000002;
        public const int DeleteDatabase = 0x00000004;
        public const int CreateUser     = 0x00000010;
        public const int EditUser       = 0x00000020;
        public const int DeleteUser     = 0x00000040;
        public const int CreateGroup    = 0x00000100;
        public const int EditGroup      = 0x00000200;
        public const int DeleteGroup    = 0x00000400;

        // Database Privileges:

        public const int CreateTable    = 0x00000001;
        public const int EditTable      = 0x00000002;
        public const int DeleteTable    = 0x00000004;
        public const int CreateDbUser   = 0x00000010;
        public const int EditDbUser     = 0x00000020;
        public const int DeleteDbUser   = 0x00000040;
        public const int CreateDbGroup  = 0x00000100;
        public const int EditDbGroup    = 0x00000200;
        public const int DeleteDbGroup  = 0x00000400;
        public const int SelectRows     = 0x00001000;
        public const int CreateRows     = 0x00002000;
        public const int EditRows       = 0x00004000;
        public const int DeleteRows     = 0x00008000;

        /// <summary>
        /// Allows the user to do anything for the given type of access
        /// </summary>
        public const int All = 0x7fffffff;

        #endregion
    };

    /// <summary>
    /// Class specifying the access rights for a user/group
    /// </summary>
    public class AccessRights
    {

        #region Private Data Members

        private RightsBaseData mBaseData;

        /// <summary>
        /// The object Id in the security database, null if not saved yet
        /// </summary>
        public object Id
        {
            get { return mBaseData.Id; }
            set { mBaseData.Id = value; }
        }

        /// <summary>
        /// Type of access
        /// </summary>
        public AccessType Type
        {
            get
            {
                int accessIndex = (int)mBaseData.GetPropertyByColumn("type");
                if (accessIndex == 0)
                    return AccessType.Server;
                if (accessIndex == 1)
                    return AccessType.Database;
                if (accessIndex == 2)
                    return AccessType.Table;
                if (accessIndex == 3)
                    return AccessType.Column;

                throw (new Exception("Unknown access type"));
            }
            set { mBaseData.SetPropertyByColumn("type", (int) value); }
        }

        /// <summary>
        /// Name of the resource this access rights references. This has different meanings depending
        /// on the type:
        /// - Server:     Server name
        /// - Database:   Database name
        /// - Table:      Database.Table name
        /// - Column:     Database.Table.Column name
        /// </summary>
        public string Name
        {
            get { return (string)mBaseData.GetPropertyByColumn("name"); }
            set { mBaseData.SetPropertyByColumn("name", value); }
        }

        /// <summary>
        /// User or group that has these rights assigned to
        /// </summary>
        public string UserId
        {
            get { return (string)mBaseData.GetPropertyByColumn("userid"); }
            set { mBaseData.SetPropertyByColumn("userid", value); }
        }

        /// <summary>
        /// Flags representing the rights explicitly granted
        /// </summary>
        public Int32 AllowedFlags
        {
            get { return (Int32)mBaseData.GetPropertyByColumn("allowflags"); }
            set { mBaseData.SetPropertyByColumn("allowflags", value); }
        }

        /// <summary>
        /// Flags representing the rights explicitly denied
        /// </summary>
        public Int32 DeniedFlags
        {
            get { return (Int32)mBaseData.GetPropertyByColumn("denyflags"); }
            set { mBaseData.SetPropertyByColumn("denyflags", value); }
        }

        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public AccessRights()
        {
            mBaseData = new RightsBaseData();

            // Default is database administrator
            Type = AccessType.Database;
            AllowedFlags = AccessFlags.All;
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public AccessRights(Server server)
            : this()
        {
            mBaseData.SetDefaults(server.SecurityDatabase);
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public AccessRights(Server server, object objectId)
            : this(server)
        {
            // load the object from the security database
            Error error = null;

            Load(server.SecurityDatabase, objectId, ref error);

            Error.Display("FindUser failed!", error);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a visible description of this right
        /// </summary>
        /// <returns></returns>
        public string Description()
        {
            string result = "Unknown";

            AccessType type = Type;
            if (type == AccessType.Server)
            {
                result = "Server Zugriff";
            }
            else if (type == AccessType.Database)
            {
                result = "Datenbank Zugriff";
            }
            else if (type == AccessType.Table)
            {
                result = "Tabellen Zugriff";
            }
            else if (type == AccessType.Column)
            {
                result = "Spalten Zugriff";
            }

            return result;
        }

        /// <summary>
        /// Save the access rights to the database
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Save(ref Error error)
        {
            bool success = false;

            if ((error == null) && (mBaseData.Database != null))
            {
                success = mBaseData.Save(ref error);
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
                success = mBaseData.Erase(ref error);
            }

            return success;
        }

        /// <summary>
        /// Update the access right on the server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="loginName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool UpdateAccessRightsOnServer(Server server, string loginName, ref Error error)
        {
            bool success = false;

            if (Type == AccessType.Server)
            {
                success = server.GrantServerPrivileges(loginName, AllowedFlags, ref error);
                success = success && server.RevokeServerPrivileges(loginName, DeniedFlags, ref error);
            }
            else if (Type == AccessType.Database)
            {
                success = server.GrantDatabasePrivileges(loginName, Name, AllowedFlags, ref error);
                success = success && server.RevokeDatabasePrivileges(loginName, Name, DeniedFlags, ref error);
            }
            else
            {
                // TBD: Table and column access...
                error = new Error("Not implemented yet", 1, this.GetType(), error);
            }

            return success;
        }

        /// <summary>
        /// Get all the defined access rights for the given user- or group-id
        /// </summary>
        /// <param name="server">Server to search in</param>
        /// <param name="userId">object id of the user or group</param>
        /// <param name="objectIds">returned object ids</param>
        /// <returns>number of defined access rights</returns>
        public static int GetAccessRightsForUserId(Server server, string userId, ref ArrayList objectIds)
        {
            int numFound = 0;

            Error error = null;

            Database database = server.SecurityDatabase;

            RightsBaseData baseData = new RightsBaseData();
            Attribute attribute = new Attribute(baseData.Table.Columns["userid"], userId);
            string criteria = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);

            numFound = database.Select(baseData.Table, baseData.Table.PrimaryKey, criteria, ref objectIds, ref error);

            Error.Display("GetAccessRightsForUserId failed!", error);

            return numFound;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Load a user from the database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="accessRightId"></param>
        /// <param name="error"></param>
        /// <returns>true if successful</returns>
        protected bool Load(Database database, object accessRightId, ref Error error)
        {
            mBaseData = new RightsBaseData();
            return mBaseData.Load(database, accessRightId, ref error);
        }

        #endregion
    }
}
