// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2014 by
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
using System.Data;
using System.Windows.Forms;
using SDBees.DB;

namespace SDBees.Plugs.TemplateBase
{
    public abstract class TemplatePlugin : TemplateBase
    {
        /// <summary>
        /// Each database related Plugin has to return its own Database Table
        /// </summary>
        /// <returns></returns>
        public abstract Table MyTable();

        /// <summary>
        /// Each Treenode Plugin returns a .net Datatable for reporting functionalities
        /// </summary>
        /// <returns>The datatable</returns>
        public DataTable MyDataTable()
        {
            return MyDBManager.GetDataTableForPlugin(MyTable().Name);
        }

        /// <summary>
        /// Each Treenode Plugin has to create a default object
        /// </summary>
        /// <returns></returns>
        public abstract TemplateDBBaseData CreateDataObject();

        /// <summary>
        /// Gets the plugin this persistent object works with
        /// </summary>
        /// <returns></returns>
        public abstract TemplatePlugin GetPlugin();

        /// <summary>
        /// Gets the type of the plugin
        /// </summary>
        /// <returns></returns>
        public string ObjectType()
        {
            return GetPlugin().GetType().ToString();
        }

        ///// <summary>
        ///// Every Plugin has to override this
        ///// </summary>
        //public static abstract TemplateDB Current;

        /// <summary>
        /// delete method for all objects
        /// </summary>
        /// <returns></returns>
        public virtual bool DeleteDataObject(Database database, object objectId)
        {
            var dataObject = CreateDataObject();

            Error error = null;
            if (dataObject.Load(database, objectId, ref error))
            {
                dataObject.Erase(ref error);
            }

            Error.Display("Can't delete object!", error);

            return (error == null);
        }

        /// <summary>
        /// Find all persistent objects for this plugin
        /// </summary>
        /// <param name="database"></param>
        /// <param name="objectIds"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public int FindAllObjects(Database database, ref ArrayList objectIds, ref Error error)
        {
            var table = MyTable();
            if (table != null)
                return database.Select(table, table.PrimaryKey, ref objectIds, ref error);
            return -1;
        }

        /// <summary>
        /// Checks if an object exists in the database
        /// </summary>
        /// <param name="database"></param>
        /// <param name="objectId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool ObjectExists(Database database, object objectId, ref Error error)
        {
            var table = MyTable();
            ArrayList objectIds = null;
            var attribute = new Attribute(table.Columns[table.PrimaryKey], objectId.ToString());
            var criteria = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
            return (database.Select(table, table.PrimaryKey, criteria, ref objectIds, ref error) == 1);
        }

        bool m_editSchemaAllowed = true;
        /// <summary>
        /// Is it allowed for the addin to edit the schema?
        /// </summary>
        public bool EditSchemaAllowed
        {
            get { return m_editSchemaAllowed; }
            set { m_editSchemaAllowed = value; }
        }

        /// <summary>
        /// Displays a dialog to edit the schema of this plugin
        /// </summary>
        public void EditSchema()
        {
            if (m_editSchemaAllowed)
            {
                var tableEditor = new frmEditTable(MyDBManager);
                // TBD: besser wäre eine "lesbarer" Name...
                tableEditor.Text = GetType().ToString();
                tableEditor.Table = MyTable();

                if (tableEditor.ShowDialog() == DialogResult.OK)
                {
                    Object dataObject = CreateDataObject();
                    dataObject.ModifyTable(tableEditor.Table, MyDBManager.Database);
                }                
            }
            else
            {
                MessageBox.Show("Editing of the schema isn't allowed for this plugin!");
            }
        }

        /// <summary>
        /// Initialize the Tableschema for this Plugin
        /// </summary>
        /// <returns></returns>
        public void InitTableSchema(ref Table table, Database database)
        {
            Object baseData = CreateDataObject();
            baseData.InitTableSchema(ref table, database);
            //baseData.SetDefaults(database);
        }

        /// <summary>
        /// If a plugin have to react on Changes in the model (maybe some subitems have been changed), it can do so ;-)
        /// </summary>
        public virtual void Update()
        { }
    }
}
