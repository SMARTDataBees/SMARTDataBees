// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
//
// Copyright (C) 2007 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// #EndHeader# ================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using SDBees.DB;


namespace SDBees.Connectivity
{
    internal class PropertySetAssignment : SDBees.DB.Object
    {
        #region Private Data Members

        private static Table gTable = null;

        #endregion

        #region Public Properties

        public string PlugIn
        {
            get { return (string)GetProperty("plugin"); }
            set { SetProperty("plugin", value); }
        }

        public string PropertyName
        {
            get { return (string)GetProperty("prop_name"); }
            set { SetProperty("prop_name", value); }
        }

        public string PropertySetName
        {
            get { return (string)GetProperty("propset_name"); }
            set { SetProperty("propset_name", value); }
        }

        public string ColumnName
        {
            get { return (string)GetProperty("column_name"); }
            set { SetProperty("column_name", value); }
        }

        public string InterfaceType
        {
            get { return (string)GetProperty("interface_type"); }
            set { SetProperty("interface_type", value); }
        }

        public override string GetTableName
        {
            get { return "usrADTPropSetAssign"; }
        }


        #endregion

        #region Constructor/Destructor

        public PropertySetAssignment()
        {
            base.Table = gTable;
        }

        #endregion

        #region Public Methods

        public static int FindAllAssignments(Database database, ref ArrayList objectIds, ref Error error)
        {
            return database.Select(gTable, gTable.PrimaryKey, ref objectIds, ref error);
        }

        public static int FindAllByPlugin(Database database, string interfaceType, string pluginName, ref ArrayList objectIds, ref Error error)
        {
            SDBees.DB.Attribute attribute = null;
            attribute = new SDBees.DB.Attribute(gTable.Columns["interface_type"], interfaceType);
            string criteria1 = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
            attribute = new SDBees.DB.Attribute(gTable.Columns["plugin"], pluginName);
            string criteria2 = database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);

            string criteria = database.FormatCriteria(criteria1, criteria2, DbBooleanOperator.eAnd, ref error);

            return database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error);
        }

        public static PropertySetAssignment FindByColumn(Database database, string interfaceType, string pluginName, string columnName, ref Error error)
        {
            PropertySetAssignment result = null;

            SDBees.DB.Attribute attribute = null;
            ArrayList criterias = new ArrayList();
            attribute = new SDBees.DB.Attribute(gTable.Columns["interface_type"], interfaceType);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));
            attribute = new SDBees.DB.Attribute(gTable.Columns["plugin"], pluginName);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));
            attribute = new SDBees.DB.Attribute(gTable.Columns["column_name"], columnName);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));

            string criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref error);

            ArrayList objectIds = null;
            if (database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error) > 0)
            {
                if (objectIds.Count > 1)
                {
                    error = new Error("Multiple assignments per column found for '" + columnName + "'", 9999, typeof(PropertySetAssignment), error);
                }
                else
                {
                    result = new PropertySetAssignment();
                    if (!result.Load(database, objectIds[0], ref error))
                    {
                        result = null;
                    }
                }
            }

            return result;
        }

        public static PropertySetAssignment FindByPropertySet(Database database, string interfaceType, string pluginName, string propertySetName, string propertyName, ref Error error)
        {
            PropertySetAssignment result = null;

            SDBees.DB.Attribute attribute = null;
            ArrayList criterias = new ArrayList();
            attribute = new SDBees.DB.Attribute(gTable.Columns["interface_type"], interfaceType);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));
            attribute = new SDBees.DB.Attribute(gTable.Columns["plugin"], pluginName);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));
            attribute = new SDBees.DB.Attribute(gTable.Columns["propset_name"], propertySetName);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));
            attribute = new SDBees.DB.Attribute(gTable.Columns["prop_name"], propertyName);
            criterias.Add(database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error));

            string criteria = database.FormatCriteria(criterias, DbBooleanOperator.eAnd, ref error);

            ArrayList objectIds = null;
            if (database.Select(gTable, gTable.PrimaryKey, criteria, ref objectIds, ref error) > 0)
            {
                if (objectIds.Count > 1)
                {
                    string message = "Multiple assignments per property set found for '" + propertySetName + "/" + propertyName + "'";
                    error = new Error(message, 9999, typeof(PropertySetAssignment), error);
                }
                else
                {
                    result = new PropertySetAssignment();
                    if (!result.Load(database, objectIds[0], ref error))
                    {
                        result = null;
                    }
                }
            }

            return result;
        }

        public static void InitTableSchema(Database database)
        {
            PropertySetAssignment baseData = new PropertySetAssignment();
            baseData.InitTableSchema(ref gTable, database);

            // Now add columns always required by this plugIn
            baseData.AddColumn(new Column("plugin", DbType.eString, "Plugin", "Plugin für diese Zuweisung", "", 256, "", 0), database);
            baseData.AddColumn(new Column("prop_name", DbType.eString, "Property Name", "Name der Eigenschaft in ADT", "", 256, "", 0), database);
            baseData.AddColumn(new Column("propset_name", DbType.eString, "PropertySet Name", "Name des Eigenschaftssatzes in ADT", "", 256, "", 0), database);
            baseData.AddColumn(new Column("column_name", DbType.eString, "Spalten Name", "Name der Eigenschaftsspalte in SMARTDataBees", "", 256, "", 0), database);
            baseData.AddColumn(new Column("interface_type", DbType.eString, "Schnittstelle", "Typ der Schnittstelle", "", 256, "", 0), database);
        }


        #endregion

        #region Protected Methods
        /*
        protected override string TableName()
        {
            return "usrADTPropSetAssign";
        }
         * */

        #endregion
    }
}
