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
using System.Windows.Forms;
using SDBees.DB;
using SDBees.Plugs.Properties;
using SDBees.Plugs.TemplateBase;

namespace SDBees.Plugs.TemplateTreeNode
{
    public class TreenodePropertyRow : PropertyRow
    {
        private TemplateTreenode m_Treenode;
        private TemplateDBBaseData m_baseData;
        private TemplateTreenodeTag m_Tag;
        private SDBeesDBConnection m_dbManager;
        private Hashtable m_MapDisplayToDbName;

        private List<PropertySpecEventArgs> m_updates = new List<PropertySpecEventArgs>();

        public TreenodePropertyRow(SDBeesDBConnection dbManager, TemplateTreenode treenode, TemplateTreenodeTag tag)
        {
            m_dbManager = dbManager;
            m_Treenode = treenode;
            m_Tag = tag;

            UpdateProperties();
        }

        public TreenodePropertyRow(SDBeesDBConnection dbManager, string typePlugin, string guidInstance)
        {
            m_dbManager = dbManager;
            m_Treenode = TemplateTreenode.GetPluginForType(typePlugin);
            m_Tag = new TemplateTreenodeTag { NodeGUID = guidInstance };

            UpdateProperties();
        }

        public TreenodePropertyRow(SDBeesDBConnection dbManager, TemplateDBBaseData baseData, string guidInstance)
        {
            m_dbManager = dbManager;
            m_Treenode = null;
            m_Tag = new TemplateTreenodeTag { NodeGUID = guidInstance };
            m_baseData = baseData;

            UpdateProperties();
        }

        public SDBeesDBConnection MyDBManager
        {
            get
            {
                return m_dbManager;
            }
        }

        public string GetType()
        {
            return m_baseData?.GetType().ToString() ?? "";
        }

        public object Id => m_baseData?.Id;

        public string GetPropertyString(string displayName)
        {
            return this[displayName].ToString();
        }

        public void SetPropertyString(string displayName, string value, bool updateProperties)
        {
            SetPropertyManually(new PropertySpecEventArgs(
                new PropertySpec(displayName, typeof(string)), value, updateProperties));
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
                    var table = m_baseData.Table;

                    Properties.Clear();
                    m_MapDisplayToDbName = new Hashtable();

                    // Dieses ist nur zum testen und sollte aus dem "DB.Object" gefüllt werden.
                    foreach (var column in table.Columns)
                    {
                        var columnType = column.GetTypeForColumn();
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
                            ps.ReadOnlyProperty = column.IsEditable ? false : true;
                            ps.BrowsableProperty = column.IsBrowsable ? true : false;

                            //ps.Attributes = new List<System.Attribute>();
                            //if (!column.Editable)
                            //    ps.ReadOnlyProperty = true; //.Attributes.Add(new ReadOnlyAttribute(true));

                            //if (!column.Browsable)
                            //    ps.BrowsableProperty = false; //.Attributes.Add(new BrowsableAttribute(false));
                            //else
                            //    ps.BrowsableProperty

                            m_baseData.SetReadWriteVisibility(ref ps, ref _error);

                            Properties.Add(ps);
                            this[column.DisplayName] = m_baseData.GetPropertyByColumn(column.Name);

                            m_MapDisplayToDbName.Add(column.DisplayName, column.Name);
                        }
                    }

                    //Fill the automatic properties
                    foreach (var sp in m_baseData.GetAutomaticProperties())
                    {
                        try
                        {
                            Properties.Add(sp.Key);
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
            var oldValue = this[e.Property.Name];

            if (CheckIsNotEqualValue(oldValue, e.Value))
            {
                // Call base class first to update the value...
                base.OnSetValue(e);

                // Change the persistent value...
                Error error = null;

                if (m_baseData == null)
                {
                    m_baseData = m_Treenode.CreateDataObject();
                    m_baseData.Load(m_dbManager.Database, m_Tag.NodeGUID, ref error);
                }
                if ((m_baseData != null) && (error == null))
                {
                    var proceed = false;
                    if (m_baseData.CheckForUniqueName())
                    {
                        proceed = m_baseData.IsNameUnique(m_baseData.GetTableName, e.Value.ToString());
                    }
                    else
                        proceed = true;

                    if (proceed)
                    {
                        var columnName = (string)m_MapDisplayToDbName[e.Property.Name];
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
                    UpdateProperties();

                if ((error == null))
                {
                    m_Treenode?.RaiseObjectModified(m_baseData, m_Tag);
                }
            }
        }

        /// <summary>
        /// This member overrides PropertyTable.OnSetValue.
        /// </summary>
        protected void OnSetValue(List<PropertySpecEventArgs> events)
        {
            var updateProperties = false;

            var oldValues = new Dictionary<PropertySpecEventArgs, object>();

            foreach (var e in events)
            {
                var oldValue = this[e.Property.Name];

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
                m_baseData = m_Treenode.CreateDataObject();
                m_baseData.Load(m_dbManager.Database, m_Tag.NodeGUID, ref error);
            }

            if ((m_baseData != null) && (error == null))
            {
                foreach (var iterator in oldValues)
                {
                    var e = iterator.Key;

                    var columnName = (string)m_MapDisplayToDbName[e.Property.Name];

                    m_baseData.SetPropertyByColumn(columnName, e.Value);
                }

                m_baseData.Save(ref error);

                Error.Display("Can't change values in database!", error);

                if (error != null)
                {
                    foreach (var iterator in oldValues)
                    {
                        var e = iterator.Key;

                        var oldValue = iterator.Value;

                        this[e.Property.Name] = oldValue;
                    }
                }

                if (updateProperties) UpdateProperties();

                if ((error == null))
                {
                    m_Treenode?.RaiseObjectModified(m_baseData, m_Tag);
                }
            }
            else
            {
                Error.Display("Can't change value in database!", error);
            }
        }

        public bool Buffered
        {
            get => m_updates != null;
            set => m_updates = value ? new List<PropertySpecEventArgs>() : null;
        }

        public void SetPropertyManually(PropertySpecEventArgs e)
        {
            if (m_updates == null)
            {
                OnSetValue(e);
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
                OnSetValue(m_updates);

                m_updates.Clear();
            }
        }

        private bool CheckIsNotEqualValue(object oldValue, object p)
        {
            if(oldValue == null)
                return true;
            if (!oldValue.Equals(p))
                return true;
            return false;
        }
    }
}
