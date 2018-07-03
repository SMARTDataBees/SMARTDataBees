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
using System.Windows.Forms;
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.Core.Admin
{
	public partial class ViewAdminLinkHelperCtrl : UserControl
	{
        internal class ListBoxTag
        {
            internal Guid mChildId;
            internal string mChildType;

            internal ListBoxTag(Guid childId, string childType)
            {
                mChildId = childId;
                mChildType = childType;
            }
        }

        private ViewAdminLinkHelper mParent;
        private TemplateTreenodeTag mTemplateTreenodeTag;
        private TemplateTreenodeTag mParentTemplateTreenodeTag;
        private Guid mViewId;

        public TemplateTreenodeTag TemplateTreenodeTag
        {
            get { return mTemplateTreenodeTag; }
            set { mTemplateTreenodeTag = value; }
        }

        public ListView ListSibling
        { get { return m_listViewSibling; } }

        public ListView ListChilds
        { get { return m_listViewChilds; } }

        public TemplateTreenodeTag ParentTemplateTreenodeTag
        {
            get { return mParentTemplateTreenodeTag; }
            set { mParentTemplateTreenodeTag = value; }
        }

        public Database Database
        {
            get
            {
                return mParent.DBManager.Database;
            }
        }

        public Guid ViewId
        {
            get { return mViewId; }
            set { mViewId = value; }
        }

        public void UpdateView()
        {
            FillListControl();
        }

        public ViewAdminLinkHelperCtrl(ViewAdminLinkHelper parent)
		{
            mParent = parent;

			InitializeComponent();
		}

        private void ViewAdminLinkHelperUserControl_Load(object sender, EventArgs e)
        {
            FillListControl();
        }

        private void ActionAddObject(string name, ListBoxTag tag, string parentType, Guid parentId)
        {
            var viewRel = new ViewRelation();
            viewRel.SetDefaults(Database);
            viewRel.ViewId = mViewId;
            viewRel.ParentType = parentType;
            viewRel.ParentId = parentId;
            viewRel.ChildType = tag.mChildType;
            viewRel.ChildId = tag.mChildId;
            viewRel.ChildName = name;

            Error error = null;
            viewRel.Save(ref error);

            Error.Display("Can't create relation", error);
        }

        private void ActionAddSelectedObjects(bool topLevel)
        {
            ListView lv = null;
            var parentType = ViewRelation.m_StartNodeValue;
            var parentId = Guid.Empty;
            if (topLevel)
            {
                lv = m_listViewSibling;
                if (mParentTemplateTreenodeTag != null)
                {
                    parentType = mParentTemplateTreenodeTag.NodeTypeOf;
                    parentId = new Guid(mParentTemplateTreenodeTag.NodeGUID);
                }
            }
            else
            {
                lv = m_listViewChilds;
                if (mTemplateTreenodeTag != null)
                {
                    parentType = mTemplateTreenodeTag.NodeTypeOf;
                    parentId = new Guid(mTemplateTreenodeTag.NodeGUID);
                }
            }
            foreach (ListViewItem lvi in lv.SelectedItems)
            {
                ActionAddObject(lvi.Text, (ListBoxTag)lvi.Tag, parentType, parentId);
            }

            // Liste aktualisieren...
            FillListControl();
        }

        private void FillListControl()
        {
            Error _error = null;
            AdminView.Current.DBManager.Database.Open(true, ref _error);

            ListSibling.Enabled = true;
            ListChilds.Enabled = true;

            FillListControlSibling();
            FillListControlChildren();

            AdminView.Current.DBManager.Database.Close(ref _error);
        }

        private void FillListControlChildren()
        {
            m_listViewChilds.Items.Clear();
            m_listViewChilds.SmallImageList = new ImageList();

            if (mTemplateTreenodeTag != null)
            {
                // Zunächst die PlugIn-Typen bestimmen, die unter dem aktuellen Knoten eingefügt werden können...
                Error error = null;
                var objectIds = new ArrayList();
                ViewDefinition.FindViewDefinitionsByParentType(Database, ref objectIds, mViewId, mTemplateTreenodeTag.NodeTypeOf, ref error);

                m_listViewChilds.BeginUpdate();

                foreach (var objectId in objectIds)
                {
                    var viewDef = new ViewDefinition();
                    if (viewDef.Load(Database, objectId, ref error))
                    {
                        // Jetzt aus dem PlugIn alle persistenten Objekte bestimmen...
                        var treenodePlugin = TemplateTreenode.GetPluginForType(viewDef.ChildType);

                        if (treenodePlugin.AllowRelationLinkingAsChild)
                        {
                            m_listViewChilds.Enabled = true;
                            var childIds = new ArrayList();
                            treenodePlugin.FindAllObjects(Database, ref childIds, ref error);

                            foreach (string childIdString in childIds)
                            {
                                // Prüfen ob dieses Objekt bereits in dieser View referenziert wird...
                                var childId = new Guid(childIdString);
                                var _lstChildRelations = new ArrayList();
                                var iRes = ViewRelation.FindViewRelationByChildIdParentType(Database, new Guid(childIdString), viewDef.ParentType, ref _lstChildRelations, ref error);

                                if (iRes <= 0)
                                {
                                    var baseData = treenodePlugin.CreateDataObject();
                                    if (baseData.Load(Database, childId, ref error))
                                    {
                                        var imageKey = TemplateTreenode.getImageForPluginType(viewDef.ChildType, m_listViewChilds.SmallImageList);

                                        var lvi = m_listViewChilds.Items.Add(baseData.Name);
                                        lvi.ImageKey = imageKey;
                                        lvi.Tag = new ListBoxTag(childId, viewDef.ChildType);
                                    }

                                }
                            }                            
                        }
                        else
                        {
                            m_listViewChilds.Enabled = false;
                        }
                    }
                }

                m_listViewChilds.EndUpdate();
            }
        }

        private void FillListControlSibling()
        {
            m_listViewSibling.Items.Clear();
            m_listViewSibling.SmallImageList = new ImageList();

            m_listViewSibling.BeginUpdate();

            // Zunächst die PlugIn-Typen bestimmen, die parallel zum aktuellen Knoten eingefügt werden können...
            Error error = null;
            var objectIds = new ArrayList();
            var parentType = ViewRelation.m_StartNodeValue;
            if (mParentTemplateTreenodeTag != null)
            {
                parentType = mParentTemplateTreenodeTag.NodeTypeOf;
            }
            ViewDefinition.FindViewDefinitionsByParentType(Database, ref objectIds, mViewId, parentType, ref error);

            foreach (var objectId in objectIds)
            {
                var viewDef = new ViewDefinition();
                if (viewDef.Load(Database, objectId, ref error))
                {
                    // Jetzt aus dem PlugIn alle persistenten Objekte bestimmen...
                    var treenodePlugin = TemplateTreenode.GetPluginForType(viewDef.ChildType);

                    if (treenodePlugin.AllowRelationLinkingAsSibling)
                    {
                        m_listViewSibling.Enabled = true;

                        var childIds = new ArrayList();
                        treenodePlugin.FindAllObjects(Database, ref childIds, ref error);

                        foreach (string childIdString in childIds)
                        {
                            // Prüfen ob dieses Objekt bereits in dieser View referenziert wird...
                            var childId = new Guid(childIdString);
                            var _lstChildRelations = new ArrayList();
                            if (ViewRelation.FindViewRelationByChildIdParentType(Database, childId, parentType, ref _lstChildRelations, ref error) <= 0)
                            {
                                var baseData = treenodePlugin.CreateDataObject();
                                if (baseData.Load(Database, childId, ref error))
                                {
                                    var imageKey = TemplateTreenode.getImageForPluginType(viewDef.ChildType, m_listViewSibling.SmallImageList);

                                    var lvi = m_listViewSibling.Items.Add(baseData.Name);
                                    lvi.ImageKey = imageKey;
                                    lvi.Tag = new ListBoxTag(childId, viewDef.ChildType);
                                }
                            }
                        }                        
                    }
                    else
                    {
                        m_listViewSibling.Enabled = false;
                    }
                }
            }

            m_listViewSibling.EndUpdate();
        }

        private void menuAddObjects_Click(object sender, EventArgs e)
        {
            ActionAddSelectedObjects(false);
        }

        private void listView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ActionAddSelectedObjects(false);
        }

        private void menuAddSiblingObjects_Click(object sender, EventArgs e)
        {
            ActionAddSelectedObjects(true);
        }

        private void listViewSibling_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ActionAddSelectedObjects(true);
        }
	}
}
