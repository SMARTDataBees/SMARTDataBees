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
using SDBees.Plugs.Properties;
using System.Windows.Forms;
using SDBees.DB;
using System.ComponentModel;

namespace SDBees.Plugs.TemplateTreeNode
{
    public class TreenodePropertyRow : PropertyRow
    {
        private TemplateTreenode m_Treenode;
        private Plugs.TemplateBase.TemplateDBBaseData m_baseData;
        private TemplateTreenodeTag m_Tag;
        private SDBees.DB.SDBeesDBConnection m_dbManager;
        private Hashtable m_MapDisplayToDbName;

        private List<PropertySpecEventArgs> m_updates = new List<PropertySpecEventArgs>();

        public TreenodePropertyRow(SDBees.DB.SDBeesDBConnection dbManager, TemplateTreenode treenode, TemplateTreenodeTag tag)
        {
            m_dbManager = dbManager;
            m_Treenode = treenode;
            m_Tag = tag;

            UpdateProperties();
        }

        public TreenodePropertyRow(SDBees.DB.SDBeesDBConnection dbManager, string typePlugin, string guidInstance)
        {
            m_dbManager = dbManager;
            m_Treenode = TemplateTreenode.GetPluginForType(typePlugin);
            m_Tag = new TemplateTreenodeTag() { NodeGUID = guidInstance };

            UpdateProperties();
        }

        public TreenodePropertyRow(SDBees.DB.SDBeesDBConnection dbManager, Plugs.TemplateBase.TemplateDBBaseData baseData, string guidInstance)
        {
            m_dbManager = dbManager;
            m_Treenode = null;
            m_Tag = new TemplateTreenodeTag() { NodeGUID = guidInstance };
            m_baseData = baseData;

            UpdateProperties();
        }

        public SDBees.DB.SDBeesDBConnection MyDBManager
        {
            get
            {
                return m_dbManager;
            }
        }

        public string GetType()
        {
            return (m_baseData != null) ? m_baseData.GetType().ToString() : "";
        }

        public object Id
        {
            get
            {
                return m_baseData != null ? m_baseData.Id : null;
            }
        }

        public string GetPropertyString(string displayName)
        {
            return this[displayName].ToString();
        }

        public void SetPropertyString(string displayName, string value, bool updateProperties)
        {
            SetPropertyManually(new Plugs.Properties.PropertySpecEventArgs(
                new Plugs.Properties.PropertySpec(displayName, typeof(string)), value, updateProperties));
        }

        public void UpdateProperties()
        {
            try
            {
                // Fill the property bag from the database table...
                Error _error = null;
                if (m_Treenode != null)
                {
                    m_baseData = m_Treenode.CreateDataObject();
                    m_baseData.Load(m_dbManager.Database, m_Tag.NodeGUID, ref _error);
                }

                if (m_baseData != null)
                {
                    Table table = m_baseData.Table;

                    this.Properties.Clear();
                    m_MapDisplayToDbName = new Hashtable();

                    // Dieses ist nur zum testen und sollte aus dem "DB.Object" gefüllt werden.
                    foreach (KeyValuePair<string, Column> iterator in table.Columns)
                    {
                        Column column = iterator.Value;
                        Type columnType = column.GetTypeForColumn();
                        PropertySpec ps = null;
                        if ((column.SelectionList != null) && (columnType == typeof(string)))
                        {
                            ps = new PropertySpecListbox(column.DisplayName, columnType, column.Category, column.Description, null, column.SelectionList);
                        }
                        else if ((!String.IsNullOrEmpty(column.UITypeConverter)) && (!String.IsNullOrEmpty(column.UITypeEditor)))
                        {
                            ps = new PropertySpec(column.DisplayName, columnType, column.Category, column.Description, null, column.UITypeEditor, column.UITypeConverter);
                        }
                        else if ((!String.IsNullOrEmpty(column.UITypeConverter)) && (String.IsNullOrEmpty(column.UITypeEditor)))
                        {
                            ps = new PropertySpec(column.DisplayName, columnType, column.Category, column.Description, null, column.UITypeConverter);
                        }
                        else
                        {
                            ps = new PropertySpec(column.DisplayName, columnType, column.Category, column.Description, null);
                        }
                        if (ps != null)
                        {
                            ps.ReadOnlyProperty = (column.Editable == true) ? false : true;
                            ps.BrowsableProperty = (column.Browsable == true) ? true : false;

                            //ps.Attributes = new List<System.Attribute>();
                            //if (!column.Editable)
                            //    ps.ReadOnlyProperty = true; //.Attributes.Add(new ReadOnlyAttribute(true));

                            //if (!column.Browsable)
                            //    ps.BrowsableProperty = false; //.Attributes.Add(new BrowsableAttribute(false));
                            //else
                            //    ps.BrowsableProperty

                            m_baseData.SetReadWriteVisibility(ref ps, ref _error);

                            this.Properties.Add(ps);
                            this[column.DisplayName] = m_baseData.GetPropertyByColumn(column.Name);

                            m_MapDisplayToDbName.Add(column.DisplayName, column.Name);
                        }
                    }

                    //Fill the automatic properties
                    foreach (KeyValuePair<PropertySpec, object> sp in m_baseData.GetAutomaticProperties())
                    {
                        try
                        {
                            this.Properties.Add(sp.Key);
                            this[sp.Key.Name] = sp.Value;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public virtual void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
        {
            m_Treenode.OnPropertyValueChanged(this, e);
        }

        /// <summary>
        /// This member overrides PropertyTable.OnSetValue.
        /// </summary>
        protected override void OnSetValue(PropertySpecEventArgs e)
        {
            // TBD: First check if this is allowed...

            // Remember the old value...
            object oldValue = this[e.Property.Name];

            if (CheckIsNotEqualValue(oldValue, e.Value))
            {
                // Call base class first to update the value...
                base.OnSetValue(e);

                // Change the persistent value...
                Error error = null;

                if (m_baseData == null)
                {
                    this.m_baseData = m_Treenode.CreateDataObject();
                    m_baseData.Load(m_dbManager.Database, m_Tag.NodeGUID, ref error);
                }
                if ((m_baseData != null) && (error == null))
                {
                    bool proceed = false;
                    if (m_baseData.CheckForUniqueName())
                    {
                        proceed = m_baseData.IsNameUnique(m_baseData.GetTableName, e.Value.ToString());
                    }
                    else
                        proceed = true;

                    if (proceed)
                    {
                        string columnName = (string)m_MapDisplayToDbName[e.Property.Name];
                        m_baseData.SetPropertyByColumn(columnName, e.Value);
                        m_baseData.Save(ref error);
                    }
                }

                Error.Display("Can't change value in database!", error);

                if (error != null)
                {
                    // Alten Wert wieder herstellen...
                    this[e.Property.Name] = oldValue;
                }

                // Refresh the property grid
                if (e.UpdateProperties)
                    this.UpdateProperties();

                if ((error == null) && (m_Treenode != null))
                {
                    m_Treenode.RaiseObjectModified(m_baseData, m_Tag);
                }
            }
        }

        /// <summary>
        /// This member overrides PropertyTable.OnSetValue.
        /// </summary>
        protected void OnSetValue(List<PropertySpecEventArgs> events)
        {
            bool updateProperties = false;

            Dictionary<PropertySpecEventArgs, object> oldValues = new Dictionary<PropertySpecEventArgs, object>();

            foreach (PropertySpecEventArgs e in events)
            {
                object oldValue = this[e.Property.Name];

                if (CheckIsNotEqualValue(oldValue, e.Value))
                {
                    oldValues[e] = oldValue;

                    base.OnSetValue(e);

                    if (e.UpdateProperties) updateProperties = true;
                }
            }

            Error error = null;

            if (m_baseData == null)
            {
                this.m_baseData = m_Treenode.CreateDataObject();
                m_baseData.Load(m_dbManager.Database, m_Tag.NodeGUID, ref error);
            }

            if ((m_baseData != null) && (error == null))
            {
                foreach (KeyValuePair<PropertySpecEventArgs, object> iterator in oldValues)
                {
                    PropertySpecEventArgs e = iterator.Key;

                    string columnName = (string)m_MapDisplayToDbName[e.Property.Name];

                    m_baseData.SetPropertyByColumn(columnName, e.Value);
                }

                m_baseData.Save(ref error);

                Error.Display("Can't change values in database!", error);

                if (error != null)
                {
                    foreach (KeyValuePair<PropertySpecEventArgs, object> iterator in oldValues)
                    {
                        PropertySpecEventArgs e = iterator.Key;

                        object oldValue = iterator.Value;

                        this[e.Property.Name] = oldValue;
                    }
                }

                if (updateProperties) this.UpdateProperties();

                if ((error == null) && (m_Treenode != null))
                {
                    m_Treenode.RaiseObjectModified(m_baseData, m_Tag);
                }
            }
            else
            {
                Error.Display("Can't change value in database!", error);
            }
        }

        public bool Buffered
        {
            get
            {
                return m_updates != null;
            }
            set
            {
                if (value)
                {
                    m_updates = new List<PropertySpecEventArgs>();
                }
                else
                {
                    m_updates = null;
                }
            }

        }

        public void SetPropertyManually(PropertySpecEventArgs e)
        {
            if (m_updates == null)
            {
                this.OnSetValue(e);
            }
            else
            {
                m_updates.Add(e);
            }
        }

        public void flush()
        {
            if (m_updates != null)
            {
                this.OnSetValue(m_updates);

                m_updates.Clear();
            }
        }

        private bool CheckIsNotEqualValue(object oldValue, object p)
        {
            if(oldValue == null)
                return true;
            if (!oldValue.Equals(p))
                return true;
            else
                return false;
        }
    }
}
