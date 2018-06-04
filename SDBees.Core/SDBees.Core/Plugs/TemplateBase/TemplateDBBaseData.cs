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
using System.ComponentModel;
using Carbon.Plugins;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.Plugs.TemplateBase
{
    /// <summary>
    /// Base class for persistent objects in TemplateTreenodes
    /// </summary>
    public abstract class TemplateDBBaseData : SDBees.DB.Object
    {
        #region Private Data Members

        private string mDescription;
        private string mDefault;
        private string mCategory;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/Set the name of this object (displayed in the tree view)
        /// </summary>
        public string Name
        {
            get { return (string)GetPropertyByColumn(m_NameColumnName); }
            set { SetPropertyByColumn(m_NameColumnName, value); }
        }


        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public TemplateDBBaseData() :
            this("", "", "")
        {
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="description"></param>
        /// <param name="defaultValue"></param>
        /// <param name="category"></param>
        public TemplateDBBaseData(string description, string defaultValue, string category)
        {
            mDescription = description;
            mDefault = defaultValue;
            mCategory = category;

            try
            {
                //Guid gTest = Guid.Empty;

                //object val = this.GetPropertyByColumn(DB.Object.m_IdSDBeesColumnName);
                //if (val != null)
                //{
                //    if (Guid.TryParse(val.ToString(), out gTest))
                //    {
                //        if (gTest == Guid.Empty)
                //        {
                //            gTest = Guid.NewGuid();
                //            this.SetPropertyByColumn(DB.Object.m_IdSDBeesColumnName, gTest);
                //        }
                //    }                    
                //}                
                //SetDefaults(SDBees.DB.SDBeesDBConnection.Current.Database);
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Public Methods
        public static BindingList<SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow> GetTreeNodePropertyRowBindingList(DB.Table tbl, SDBees.Plugs.TemplateBase.TemplatePlugin basePlugin)
        {
            BindingList<SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow> m_dataList = new BindingList<Plugs.TemplateTreeNode.TreenodePropertyRow>();
            Error error = null;

            SDBees.DB.SDBeesDBConnection.Current.MyDBManager.Database.Open(false, ref error);

            ArrayList objectIds = new ArrayList();
            int count = SDBees.DB.SDBeesDBConnection.Current.MyDBManager.Database.Select(tbl, tbl.PrimaryKey, ref objectIds, ref error);
            foreach (string item in objectIds)
            {
                Plugs.TemplateBase.TemplateDBBaseData baseData = basePlugin.CreateDataObject();
                if (baseData.Load(SDBees.DB.SDBeesDBConnection.Current.MyDBManager.Database, item, ref error))
                {
                    SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow propTableRow = new SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow(SDBees.DB.SDBeesDBConnection.Current.MyDBManager, baseData, item);
                    m_dataList.Add(propTableRow);
                    //baseData
                }
            }
            SDBees.DB.SDBeesDBConnection.Current.MyDBManager.Database.Close(ref error);

            return m_dataList;
        }

        /// <summary>
        /// Searches the given table for records with sdbeesid
        /// </summary>
        /// <param name="tbl">the table for the search</param>
        /// <param name="sdbeesid">the internal sdbees id to search for</param>
        /// <param name="_error">error provider</param>
        /// <param name="_lstDbIds">arraylist of id's for found records</param>
        /// <returns>true, if one or more records found. Otherwise false</returns>
        public static bool ObjectExistsInDbWithSDBeesId(DB.Table tbl, object sdbeesid, ref Error _error, ref ArrayList _lstDbIds)
        {
            bool exists = false;
            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(tbl.Columns[m_IdSDBeesColumnName], sdbeesid);
            string criteria = SDBees.DB.SDBeesDBConnection.Current.MyDBManager.Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref _error);

            int count = SDBees.DB.SDBeesDBConnection.Current.MyDBManager.Database.Select(tbl, tbl.PrimaryKey, criteria, ref _lstDbIds, ref _error);
            if (count > 0)
                exists = true;

            return exists;
        }

        public const string m_NameColumnName = "name";
        public const string m_NameDisplayName = "Name";
        //public const string m_NumberColumnName = "number";
        //public const string m_NumberColumnDisplayName = "Number";

        /// <summary>
        /// Initialize the table schema
        /// </summary>
        /// <param name="table"></param>
        /// <param name="database"></param>
        public override void InitTableSchema(ref Table table, Database database)
        {
            base.InitTableSchema(ref table, database);

            // Add the number column
            //this.AddColumn(new Column(m_NumberColumnName, DbType.eInt32, m_NumberColumnDisplayName, "Indexnumber for this element", mCategory, 20, "", 0), database);
            // Now add the name column
            this.AddColumn(new Column(m_NameColumnName, DbType.eString, m_NameDisplayName, mDescription, mCategory, 100, mDefault, 0), database);
        }

        /// <summary>
        /// Set the default value for this persistent object
        /// </summary>
        /// <param name="database"></param>
        public override void SetDefaults(Database database)
        {
            base.SetDefaults(database);

            this.Name = mDefault;
            this.SetPropertyByColumn(SDBees.DB.Object.m_IdSDBeesColumnName, Guid.NewGuid().ToString());
        }

        public static TemplateTreenode GetPluginForBaseData(TemplateDBBaseData baseData)
        {
            TemplateTreenode result = null;

            foreach (PluginDescriptor desc in SDBees.DB.SDBeesDBConnection.Current.MyPluginContext.PluginDescriptors)
            {
                TemplateTreenode temp = desc.PluginInstance as TemplateTreenode;
                if (temp != null)
                {
                    TemplateDBBaseData tempBaseData = temp.CreateDataObject();
                    if (tempBaseData.GetType() == baseData.GetType())
                    {
                        result = temp;
                        break;
                    }
                }
            }

            return result;
        }

        public bool IsNameUnique(string tablename, string value)
        {
            bool result = true;


            System.Data.DataTable tbl = SDBees.DB.SDBeesDBConnection.Current.GetDataTableForPlugin(tablename);
            if (tbl != null)
            {
                foreach (System.Data.DataRow row in tbl.Rows)
                {
                    if (row[m_NameColumnName].ToString() == value)
                    {
                        result = false;
                        System.Windows.Forms.MessageBox.Show("The object requires a unique name! Please modify the name to be unique and try it again.", "Naming", System.Windows.Forms.MessageBoxButtons.OK);

                        break;
                    }
                }
            }

            return result;
        }

        public bool IsNameUnique(string tablename)
        {
            return IsNameUnique(tablename, Name);
        }

        ///// <summary>
        ///// Erase this object persistently from the database
        ///// </summary>
        ///// <param name="error">Contains error information if this fails</param>
        ///// <returns>true if successful</returns>
        //public override bool Erase(ref Error error)
        //{
        //    bool success = base.Erase(ref error);

        //    if (success)
        //    {
        //        SDBees.Plugs.TemplateTreeNode.TemplateTreenode plugin = SDBees.Plugs.TemplateTreeNode.TemplateTreenode.Current as SDBees.Plugs.TemplateTreeNode.TemplateTreenode;
        //        SDBees.Plugs.TemplateTreeNode.TemplateTreenodeTag tag = plugin.MyTag();
        //        tag.NodeGUID = this.Id.ToString();
        //        tag.NodeName = this.Name;
        //        tag.NodeTypeOf = plugin.GetType().ToString();
        //        plugin.RaiseObjectDeleted(this, tag);
        //    }

        //    return success;
        //}

        #endregion

        #region Protected Methods

        /*
        protected override string TableName()
        {
            string sTablename = this.GetTableName;
            return sTablename.ToLower();

            //return "usrAECBuildings";
        }
         * */


        #endregion

        public bool IsManuallyCreated(ref Error _error)
        {
            bool result = true;

            ArrayList _lst = new ArrayList();
            if(SDBees.Core.Connectivity.ConnectivityManagerAlienBaseData.GetAlienIdsByDbId(this.GetPropertyByColumn(m_IdColumnName).ToString(), ref _error, ref _lst))
            {
                if (_lst.Count > 0)
                {
                    result = false;
                }
            }

            return result;
        }

        public virtual void SetReadWriteVisibility(ref Properties.PropertySpec ps, ref Error _error)
        {
            // Set all properties to read only if not manually created ...

            if (IsManuallyCreated(ref _error) == false)
            {
                ps.ReadOnlyProperty = true;
            }
        }

        public virtual bool CheckForUniqueName()
        {
            return false;
        }
    }
}
