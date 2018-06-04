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

using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.Connectivity
{
    class ExternalConnection : SDBees.DB.Object
    {
        #region Private Data Members

        /// <summary>
        /// the table singleton for this class
        /// </summary>
        private static Table gTable = null;

        #endregion

        #region Public Properties

        /// <summary>
        /// the object/node/plugin type of the object
        /// </summary>
        public string ObjectType
        {
            get { return (string)GetProperty("object_type"); }
            set { SetProperty("object_type", value); }
        }

        /// <summary>
        /// the guid of the node object
        /// </summary>
        public string NodeId
        {
            get { return (string)GetProperty("node_id"); }
            set { SetProperty("node_id", value); }
        }

        /// <summary>
        /// the external id of the external object
        /// </summary>
        public string ExternalId
        {
            get { return (string)GetProperty("external_id"); }
            set { SetProperty("external_id", value); }
        }

        /// <summary>
        /// the external interface type 
        /// </summary>
        public string InterfaceType
        {
            get { return (string)GetProperty("interface_type"); }
            set { SetProperty("interface_type", value); }
        }

        public override string GetTableName
        {
            get { return "externalConnections"; }
        }


        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// default contructor
        /// </summary>
        public ExternalConnection()
        {
            base.Table = gTable;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the node object for this connection
        /// </summary>
        /// <returns></returns>
        public SDBees.Plugs.TemplateBase.TemplateDBBaseData getNodeObject(ref Error error)
        {
            SDBees.Plugs.TemplateBase.TemplateDBBaseData baseData = null;

            TemplateTreenode plugin = SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetPluginForType(ObjectType);
            if (plugin != null)
            {
                baseData = plugin.CreateDataObject();
                if (!baseData.Load(Database, NodeId, ref error))
                {
                    baseData = null;
                }
            }

            return baseData;
        }

        /// <summary>
        /// initializes the database table for the node type
        /// </summary>
        /// <param name="database">the database for the table</param>
        public static void InitTableSchema(Database database)
        {
            if (gTable == null)
            {
                ExternalConnection connection = new ExternalConnection();
                connection.InitTableSchema(ref gTable, database);

                // Now add columns always required by this plugIn
                connection.AddColumn(new Column("object_type", DbType.eString, "ObjectTyp", "Typ des verknüpften Objektes", "", 256, "", 0), database);
                connection.AddColumn(new Column("node_id", DbType.eGuidString, "Knoten Id", "Guid des SDBees Knoten", "", 256, "", 0), database);
                connection.AddColumn(new Column("interface_type", DbType.eString, "Interface Typ", "Typ des externen Interfaces", "", 256, "", 0), database);
                connection.AddColumn(new Column("external_id", DbType.eString, "Externer Id", "externer Id des externen Objektes", "", 256, "", 0), database);
            }
        }

        /// <summary>
        /// Find all persistent objects
        /// </summary>
        /// <param name="database"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int FindAllConnections(Database database, ref ArrayList objectIds, ref Error error)
        {
            return database.Select(gTable, gTable.PrimaryKey, ref objectIds, ref error);
        }

        /// <summary>
        /// Find all connections for a specific interface
        /// </summary>
        /// <param name="database">Database to search in</param>
        /// <param name="interfaceType">The interface type</param>
        /// <param name="objectIds">Will contain the found object ids of the connection objects</param>
        /// <param name="error">will be not null if errors occurred</param>
        /// <returns>Number of connections found</returns>
        public static int FindAllConnectionsForInterface(Database database, string interfaceType, ref ArrayList objectIds, ref Error error)
        {
            SDBees.DB.Attribute attribute = new SDBees.DB.Attribute(gTable.Columns["interface_type"], interfaceType);
            string criteria = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);

            return database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Find a connection by the external id
        /// </summary>
        /// <param name="database"></param>
        /// <param name="interfaceType"></param>
        /// <param name="externalId"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int FindConnectionByExternalId(Database database, string interfaceType, string externalId, ref ArrayList objectIds, ref Error error)
        {
            SDBees.DB.Attribute attribute = new SDBees.DB.Attribute(gTable.Columns["interface_type"], interfaceType);
            string criteria1 = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
            attribute = new SDBees.DB.Attribute(gTable.Columns["external_id"], externalId);
            string criteria2 = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);

            string criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

            return database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        /// <summary>
        /// Find a connection by the internal node id
        /// </summary>
        /// <param name="database"></param>
        /// <param name="interfaceType"></param>
        /// <param name="objectType"></param>
        /// <param name="nodeId"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static int FindConnectionByNodeId(Database database, string interfaceType, string objectType, string nodeId, ref ArrayList objectIds, ref Error error)
        {
            ArrayList criterias = new ArrayList();
            SDBees.DB.Attribute attribute = new SDBees.DB.Attribute(gTable.Columns["interface_type"], interfaceType);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));
            attribute = new SDBees.DB.Attribute(gTable.Columns["object_type"], objectType);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));
            attribute = new SDBees.DB.Attribute(gTable.Columns["node_id"], nodeId);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));

            string criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref error);

            return database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        #endregion

        #region Protected Methods

        /*
        protected override string TableName()
        {
            return "externalConnections";
        }
         * */

        #endregion

    }
}
