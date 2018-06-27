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
using SDBees.Plugs.Properties;

namespace SDBees.DB
{
    /// <summary>
    /// Class to display properties of an Object in a PropertyGrid
    /// </summary>
    public class ObjectPropertyTable : PropertyRow
    {
        private Object mDbObject;
        private Hashtable mMapDisplayToDbName;

        public ObjectPropertyTable(Object dataObject)
        {
            if ((dataObject == null) || (dataObject.Database == null))
            {
                throw (new Exception("Object must be given and persistent"));
            }

            mDbObject = dataObject;

            UpdateProperties();
        }

        protected void UpdateProperties()
        {
            // Fill the property bag from the database table...
            Error error = null;
            var table = mDbObject.Table;

            Properties.Clear();
            mMapDisplayToDbName = new Hashtable();

            // Dieses ist nur zum testen und sollte aus dem "DB.Object" gefüllt werden.
            foreach (var column in table.Columns)
            {
               
                var columnType = column.GetTypeForColumn();
                PropertySpec ps = null;
                if ((column.SelectionList != null) && (typeof(string) == columnType))
                {
                    ps = new PropertySpecListbox(column.DisplayName, typeof(string), column.Category, column.Description, null, column.SelectionList);
                }
                else
                {
                    ps = new PropertySpec(column.DisplayName, columnType, column.Category, column.Description, null);
                }
                if (ps != null)
                {
                    if (!column.IsEditable)
                    {
                        ps.ReadOnlyProperty = true;
                    }
                    if(!column.IsBrowsable)
                    {
                        ps.BrowsableProperty = false;
                    }
                    Properties.Add(ps);
                    this[column.DisplayName] = mDbObject.GetPropertyByColumn(column.Name);

                    mMapDisplayToDbName.Add(column.DisplayName, column.Name);
                }
            }
        }

        /// <summary>
        /// This member overrides PropertyTable.OnSetValue.
        /// </summary>
        protected override void OnSetValue(PropertySpecEventArgs e)
        {
            // TBD: First check if this is allowed...

            // Remember the old value...
            var oldValue = this[e.Property.Name];

            // Call base class first to update the value...
            base.OnSetValue(e);

            // Change the persistent value...
            Error error = null;
            if (mDbObject.Load(mDbObject.Database, mDbObject.Id, ref error) && (error == null))
            {
                var columnName = (string) mMapDisplayToDbName[e.Property.Name];
                mDbObject.SetPropertyByColumn(columnName, e.Value);
                mDbObject.Save(ref error);
            }

            Error.Display("Konnte Wert nicht in Datenbank ändern.", error);

            if (error != null)
            {
                // Alten Wert wieder herstellen...
                this[e.Property.Name] = oldValue;
            }

            // Refresh the property grid
            UpdateProperties();

            if (error == null)
            {
                // raise an Event that the value has changed...
                RaisePropertyValueModified(mDbObject, e.Property.Name);
            }
        }

        #region Events

        public class NotificationEventArgs
        {
            public Object DbObject;
            public string ColumnName;

            public NotificationEventArgs(Object dbObject, string columnName)
            {
                DbObject = dbObject;
                ColumnName = columnName;
            }
        }
        public delegate void NotificationHandler(object sender, NotificationEventArgs args);

        public event NotificationHandler PropertyValueModified;

        internal void RaisePropertyValueModified(Object dbObject, string columnName)
        {
            if (PropertyValueModified != null)
            {
                var args = new NotificationEventArgs(dbObject, columnName);
                PropertyValueModified.Invoke(this, args);
            }
        }

        #endregion
    }

}
