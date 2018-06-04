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

namespace SDBees.Plugs.TemplateBase
{
    internal class TableInspector : Inspector
    {
        #region Private Data Members

        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public TableInspector(SDBees.DB.SDBeesDBConnection dbManager)
            : base(dbManager)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Check and fix the database, this is a virtual method called
        /// by the framework
        /// </summary>
        public override void InspectDatabase()
        {
            string message;
            List<object> invalidObjects = new List<object>();
            List<object> unreferencedObjects = new List<object>();

            List<SDBees.Plugs.TemplateTreeNode.TemplateTreenode> plugins = SDBees.Plugs.TemplateTreeNode.TemplateTreenode.GetAllTreenodePlugins();

            int count = plugins.Count;

            if (count > 0)
            {
                WriteMessage("Tabellen Prüfung gestartet ...\r\n");

                myProgressBar.Maximum = count - 1;

                for (int index = 0; index < count; index++)
                {
                    SDBees.Plugs.TemplateTreeNode.TemplateTreenode plugin = plugins[index];
                    int invalidCount = 0;

                    WriteMessage("Prüfung " + plugin.GetType().ToString() + "\r\n");

                    Error error = null;

                    if (!DbTableValid(plugin, Database, AutomaticFix, ref error))
                    {
                        invalidObjects.Add(plugin);
                        invalidCount++;
                    }

                    if (invalidCount > 0)
                    {
                        WriteMessage("\t" + invalidCount + " fehlerhafte Tabellen gefunden\r\n");
                    }

                    myProgressBar.Value = index;
                }
            }

            message = "Es sind insgesamt " + count + " Plugins in der Datenbank.\r\n";
            if ((invalidObjects.Count > 0) || (unreferencedObjects.Count > 0))
            {
                message += "Fehlerhafte Tabellen: " + invalidObjects.Count + "\r\n";
            }
            else
            {
                message += "Es wurden keine Inkonsistenzen oder Fehler in der Datenbank gefunden.\r\n";
            }

            WriteMessage(message);
        }

        #endregion

        #region Protected Methods

        private bool DbTableValid(SDBees.Plugs.TemplateTreeNode.TemplateTreenode plugin, Database database, bool fix, ref Error error)
        {
            bool isValid = true;

            TemplateDBBaseData baseData = plugin.CreateDataObject();

            // test the xml schema...
            //string xmlTableDefinition = baseData.Table.writeXml();
            //WriteMessage("Table = :" + xmlTableDefinition);
            //WriteMessage("\r\n");

            List<string> columnsToFix = new List<string>();
            foreach (KeyValuePair<string, Column> iterator in baseData.Table.Columns)
            {
                Column column = iterator.Value;
                string columnName = column.Name;
                if (columnName != columnName.Trim())
                {
                    columnsToFix.Add(column.Name);
                    WriteMessage("FEHLER: Spaltenname '" + columnName + "' enthält Leerzeichen!\r\n");
                    isValid = false;
                }

                string cleanColumnName = database.MakeValidColumnName(columnName.Trim());
                if (columnName != cleanColumnName)
                {
                    if (!columnsToFix.Contains(columnName))
                    {
                        columnsToFix.Add(columnName);
                    }
                    WriteMessage("FEHLER: Spaltenname '" + columnName + "' enthält ungültige Zeichen!\r\n");
                    isValid = false;
                }
            }
            if (fix)
            {
                foreach (string columnName in columnsToFix)
                {
                    string oldColumnName = columnName;
                    string newColumnName = database.MakeValidColumnName(columnName.Trim());

                    baseData.RenameColumn(oldColumnName, newColumnName, database);
                }
            }

            string tableName = baseData.Table.Name;

            List<string> columnNames = null;
            List<string> columnsMissingInXML = new List<string>();
            columnNames = null;
            if (database.GetTableColumns(baseData.Table.Name, out columnNames, ref error) > 0)
            {
                foreach (string columnName in columnNames)
                {
                    if (!baseData.Table.Columns.ContainsKey(columnName))
                    {
                        columnsMissingInXML.Add(columnName);
                        WriteMessage("FEHLER: Spalte '" + columnName + "' nicht in XML Schema enthalten!\r\n");
                        isValid = false;
                    }
                }
            }
            if (fix)
            {
                foreach (string columnName in columnsMissingInXML)
                {
                    database.EraseColumn(tableName, columnName, ref error);
                }
            }

            TableSchema tableSchema = TableSchema.FindSchema(database, baseData.Table.Name, ref error);
            if (tableSchema != null)
            {
                //string xmlSchemaDefinition = tableSchema.XmlSchema;
                //WriteMessage("Schema = :" + xmlSchemaDefinition);
                //WriteMessage("\r\n");
            }
            else
            {
                WriteMessage("FEHLER: TableSchema für Tabelle " + baseData.Table.Name + " nicht gefunden!\r\n");

                isValid = false;
                if (fix)
                {
                    //TBD: create schema...
                }
            }


            return isValid;
        }

        #endregion
    }
}
