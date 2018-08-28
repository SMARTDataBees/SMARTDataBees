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

namespace SDBees.DB
{
    /// <summary>
    /// Class to wrap SQL Server Groups
    /// </summary>
    public class Group
    {
        #region Private Data Members

        private GroupBaseData mBaseData;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Id of this Group...
        /// </summary>
        public object Id
        {
            get { return mBaseData.Id; }
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
        public string ParentId
        {
            get { return (string)mBaseData.GetPropertyByColumn("parentid"); }
            set { mBaseData.SetPropertyByColumn("parentid", value); }
        }

        /// <summary>
        /// Returns the base data for this object
        /// </summary>
        public Object BaseData
        {
            get { return mBaseData; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Group()
        {
            mBaseData = new GroupBaseData();
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Group(Server server)
        {
            mBaseData = new GroupBaseData();
            mBaseData.SetDefaults(server.SecurityDatabase);
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Group(Server server, object objectId)
        {
            mBaseData = new GroupBaseData();

            Error error = null;
            mBaseData.Load(server.SecurityDatabase, objectId, ref error);

            Error.Display("Unable to load Group", error);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Find a user by the login name
        /// </summary>
        /// <param name="server"></param>
        /// <param name="name"></param>
        /// <returns>The found user or null if not found</returns>
        public static Group FindGroup(Server server, string name)
        {
            Group group = null;

            Error error = null;

            var database = server.SecurityDatabase;

            var baseData = new GroupBaseData();
            var attribute = new Attribute(baseData.Table.Columns["name"], name);
            var criteria = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);

            var values = new ArrayList();
            var numFound = database.Select(baseData.Table, baseData.Table.PrimaryKey, criteria, ref values, ref error);

            if (numFound == 1)
            {
                group = new Group();
                group.Load(database, values[0], ref error);
            }

            Error.Display("FindGroup failed!", error);

            return group;
        }

        /// <summary>
        /// Get all the group names on this server
        /// </summary>
        /// <param name="server"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        public static int GetAllGroups(Server server, ref ArrayList names)
        {
            var count = 0;

            Error error = null;
            var baseData = new GroupBaseData();

            count = server.SecurityDatabase.Select(baseData.Table, "name", ref names, ref error);

            Error.Display("GetAllGroups failed!", error);

            return count;
        }

        /// <summary>
        /// Update the database access for this user to the server
        /// </summary>
        /// <param name="error">Error description if this function fails</param>
        /// <returns>true if successful</returns>
        public bool UpdateAccessRightsOnServer(ref Error error)
        {
            var success = false;

            // TBD: get all members (and derived...) and update their rights
            //ArrayList objectIds = null;
            //if (AccessRights.GetAccessRightsForUserId(server, Id.ToString(), ref objectIds) == 0)
            //{
            //    return true;
            //}

            //success = true;

            //foreach (object objectId in objectIds)
            //{
            //    AccessRights accessRight = new AccessRights(server, objectId);

            //    success = success && accessRight.UpdateAccessRightsOnServer(server, LoginName, ref error);
            //}

            return success;
        }

        /// <summary>
        /// Load a user from the database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="groupId"></param>
        /// <param name="error"></param>
        /// <returns>true if successful</returns>
        public bool Load(Database database, object groupId, ref Error error)
        {
            mBaseData = new GroupBaseData();
            return mBaseData.Load(database, groupId, ref error);
        }

        /// <summary>
        /// Save the user to the database
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool Save(ref Error error)
        {
            var success = false;

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
            var success = false;

            if ((error == null) && (mBaseData.Database != null))
            {
                success = mBaseData.Erase(ref error);
            }

            return success;
        }

        #endregion

        #region Protected Methods

        #endregion
    }
}
