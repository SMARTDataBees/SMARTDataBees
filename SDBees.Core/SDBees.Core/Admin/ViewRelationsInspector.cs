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
using SDBees.DB;
using SDBees.Plugs.TemplateBase;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.Core.Admin
{
    internal class ViewRelationsInspector : Inspector
    {

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ViewRelationsInspector(SDBeesDBConnection connection)
            : base(connection)
        {
        }



        public override void InspectDatabase()
        {
            var invalidObjects = new List<object>();
            var unreferencedObjects = new List<object>();

            var plugins = TemplateTreenode.GetAllTreenodePlugins();
            var count = plugins.Count;
            var totalObjectCount = 0;

            if (count > 0)
            {
                WriteMessage("Objekt Prüfung gestartet ...\r\n");

                myProgressBar.Maximum = count - 1;

                for (var index = 0; index < count; index++)
                {
                    var plugin = plugins[index];

                    ArrayList objectIds = null;
                    Error error = null;
                    var objectCount = plugin.FindAllObjects(Database, ref objectIds, ref error);
                    var invalidCount = 0;
                    var unreferencedCount = 0;

                    totalObjectCount += objectCount;

                    if (objectCount > 0)
                    {
                        WriteMessage($"Checking {objectCount} {plugin.GetType()}\r\n");
                    }

                    if (objectIds != null)
                        foreach (var objectId in objectIds)
                        {
                            if (objectId != null && !DbObjectValid(plugin, Database, objectId, AutomaticFix, ref error))
                            {
                                invalidObjects.Add(objectId);
                                invalidCount++;
                            }
                            if (objectId != null && !DbObjectReferenced(plugin, Database, objectId, DeleteUnreferenced, ref error))
                            {
                                unreferencedObjects.Add(objectId);
                                unreferencedCount++;
                            }
                        }

                    if (invalidCount > 0)
                    {
                        WriteMessage("\t" + invalidCount + " faulty elements found\r\n");
                    }

                    if (unreferencedCount > 0)
                    {
                        WriteMessage("\t" + unreferencedCount + " not referenced elements found\r\n");
                    }

                    myProgressBar.Value = index;
                }
            }

            var message = "Number of elements in the database: " + totalObjectCount + "\r\n";
            if ((invalidObjects.Count > 0) || (unreferencedObjects.Count > 0))
            {
                message += "Faulty elements: " + invalidObjects.Count + "\r\n";
                message += "Not referenced elements: " + unreferencedObjects.Count + "\r\n";
            }
            else
            {
                message += "Es wurden keine Inkonsistenzen oder Fehler in der Datenbank gefunden.\r\n";
            }

            WriteMessage(message);
        }


        private bool DbObjectValid(TemplatePlugin plugin, Database database, object objectId, bool fix, ref Error error)
        {
            var isValid = true;

            var baseData = plugin.CreateDataObject();
            if (baseData.Load(database, objectId, ref error))
            {
                var name = baseData.Name;

                ArrayList viewRelIds = null;
                var viewRelCount = ViewRelation.FindViewRelationByChildId(database, new Guid(objectId.ToString()), ref viewRelIds, ref error);

                if (viewRelCount > 0)
                {
                    foreach (var viewRelId in viewRelIds)
                    {
                        var viewRel = new ViewRelation();
                        if (viewRel.Load(database, viewRelId, ref error))
                        {
                            if (viewRel.ChildName != name)
                            {
                                isValid = false;

                                if (fix && (error == null))
                                {
                                    viewRel.ChildName = name;
                                    if (!viewRel.Save(ref error))
                                    {
                                        // TBD: display a message? Cound the failed?
                                    }
                                }
                            }
                        }
                    }
                }

            }

            return isValid;
        }

        private bool DbObjectReferenced(TemplateTreenode plugin, Database database, object objectId, bool delete, ref Error error)
        {
            ArrayList viewRelIds = null;
            var viewRelCount = ViewRelation.FindViewRelationByChildId(database, new Guid(objectId.ToString()), ref viewRelIds, ref error);

            var isReferenced = (viewRelCount > 0);

            if (!isReferenced && delete && (error == null))
            {
                if (!EraseDbObject(plugin, database, objectId, ref error))
                {
                    // TBD: display a message? Cound the failed?
                }
            }

            return isReferenced;
        }

        private bool EraseDbObject(TemplateTreenode plugin, Database database, object objectId, ref Error error)
        {
            var success = false;

            if (error == null)
            {
                var baseData = plugin.CreateDataObject();
                if (baseData.Load(database, objectId, ref error))
                    success = baseData.Erase(ref error);
            }

            return success;
        }

    }
}
