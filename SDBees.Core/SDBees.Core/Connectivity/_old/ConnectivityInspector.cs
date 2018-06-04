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
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.Connectivity
{
    internal class ConnectivityInspector : Inspector
    {
        #region Private Data Members

        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ConnectivityInspector(Database database)
            : base(database)
        {
        }

        #endregion

        #region Public Methods

        public override void InspectDatabase()
        {
            WriteMessage("Verbindungs Prüfung gestartet ...\r\n");

            string message;
            List<object> invalidObjects = new List<object>();

            ArrayList objectIds = null;
            Error error = null;
            ExternalConnection.FindAllConnections(Database, ref objectIds, ref error);

            int totalConnectionCount = objectIds.Count;

            if (totalConnectionCount > 0)
            {
                myProgressBar.Maximum = totalConnectionCount - 1;
                int invalidCount = 0;
                int index = 0;

                foreach (object objectId in objectIds)
                {
                    if (!ConnectionObjectValid(objectId, ref error))
                    {
                        invalidObjects.Add(objectId);
                        invalidCount++;
                    }

                    myProgressBar.Value = index;
                    index++;
                }

                if (invalidCount > 0)
                {
                    WriteMessage("\t" + invalidCount + " ungültige Verbindungen gefunden\r\n");
                }
            }

            message = "Es sind insgesamt " + totalConnectionCount + " Verbindungen zu externen Objekten in der Datenbank.\r\n";
            if (invalidObjects.Count > 0)
            {
                message += "Ungültige Verbindungen: " + invalidObjects.Count + "\r\n";
            }
            else
            {
                message += "Es wurden keine ungültigen Verbindungen in der Datenbank gefunden.\r\n";
            }

            WriteMessage(message);
        }

        #endregion

        #region Protected Methods

        private bool ConnectionObjectValid(object objectId, ref Error error)
        {
            bool isValid = true;

            ExternalConnection connection = new ExternalConnection();
            if (connection.Load(Database, objectId, ref error))
            {
                string pluginType = connection.ObjectType;
                string nodeId = connection.NodeId;

                TemplateTreenode plugin = TemplateTreenode.GetPluginForType(pluginType);
                if (plugin != null)
                {
                    isValid = plugin.ObjectExists(Database, nodeId, ref error);

                    if (!isValid && this.AutomaticFix && (error == null))
                    {
                        if (!connection.Erase(ref error))
                        {
                            // TBD: display a message? Cound the failed?
                        }
                    }
                }
            }

            return isValid;
        }

        #endregion
    }
}
