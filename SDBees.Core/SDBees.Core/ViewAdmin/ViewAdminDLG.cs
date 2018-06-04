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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Carbon;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;

using SDBees.Plugs.Objects;
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;

namespace SDBees.ViewAdmin
{
    public partial class ViewAdminDLG : Form
    {
        private ViewAdmin _refViewAdmin;
        internal Hashtable _hashPlugins;
        internal Hashtable _hashHelper;

        private Hashtable _hashViews;

        private string _sCurrentViewID;

        public ToolStripComboBox _cmbViewSelector;

        private bool _bConfigNodeSelected = false;
        private bool _bConfigNodeofInsertion = false;

        private int iLevelofInsertion = 0;

        public ViewAdminDLG(ViewAdmin baseRef)
        {
            try
            {
                this._refViewAdmin = baseRef;
                InitializeComponent();

                this._hashPlugins = new Hashtable();
                this._hashHelper = new Hashtable();
                this._hashViews = new Hashtable();

                this._sCurrentViewID = null;

                this._cmbViewSelector = new ToolStripComboBox();
                this._cmbViewSelector.Alignment = ToolStripItemAlignment.Right;
                this._cmbViewSelector.Sorted = true;
                this._cmbViewSelector.DropDownStyle = ComboBoxStyle.DropDownList;

                ContextMenu pluginListContextMenu = new ContextMenu();
                pluginListContextMenu.Popup += new EventHandler(pluginListContextMenu_Popup);
                this.listViewPlugins.ContextMenu = pluginListContextMenu;

                this._cmbViewSelector.SelectedIndexChanged += new EventHandler(_cmbViewSelector_SelectedIndexChanged);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void RefreshView()
        {
            InitPluginHash();
            InitHelperHash();

            FillPluginList();
            FillHelperList();

            //FillHashViews();

            FillViewComboBox();
        }

        void pluginListContextMenu_Popup(object sender, EventArgs e)
        {
            this.listViewPlugins.ContextMenu.MenuItems.Clear();

            if (this.listViewPlugins.SelectedItems.Count == 1)
            {
                ListViewItem lvi = this.listViewPlugins.SelectedItems[0];

                MenuItem menuEditSchema = new MenuItem("Edit Schema of " + lvi.Text);
                menuEditSchema.Tag = lvi.Tag;
                menuEditSchema.Click += new EventHandler(menuEditSchema_Click);
                this.listViewPlugins.ContextMenu.MenuItems.Add(menuEditSchema);
            }
        }

        void menuEditSchema_Click(object sender, EventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            ObjectPlugin objPlug = (ObjectPlugin)menuItem.Tag;

            TemplateTreenode plugin = TemplateTreenode.GetPluginForType(objPlug.PluginType);
            plugin.EditSchema();
        }

        private void SelectCorrespondingNode(TemplateTreenodeTag tag, TreeNode trn)
        {
            try
            {
                if (this._bConfigNodeSelected != true)
                {
                    ObjectPlugin tntag = (ObjectPlugin)trn.Tag;
                    if (tag.NodeTypeOf == tntag.PluginType)
                    {
                        this._bConfigNodeSelected = true;
                        this.treeViewSystemConfig.SelectedNode = trn;
                        this.iLevelofInsertion = GetLevel(this.treeViewSystemConfig.SelectedNode.FullPath);
                        //break;
                    }
                    else if (trn.Nodes.Count > 0)
                    {
                        foreach (TreeNode tr in trn.Nodes)
                        {
                            SelectCorrespondingNode(tag, tr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private int GetLevel(string p)
        {
            int iLevel = 0;
            string[] aPathSegments = p.Split('\\');
            iLevel = aPathSegments.Length - 1;
            return iLevel;
        }


        void _treeviewSystem_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        private void ViewAdminDLG_Load(object sender, EventArgs e)
        {
            try
            {
                this.toolStrip1.Items.Add(this._cmbViewSelector);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }

        void _cmbViewSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Einlesen der Viewdaten
            ObjectView objView = (ObjectView)this._hashViews[this._cmbViewSelector.SelectedItem.ToString()];

            this._sCurrentViewID = objView.ViewGUID;
            if (this._sCurrentViewID != null)
                this._refViewAdmin.CurrentViewId = new Guid(this._sCurrentViewID);
            else
                this._refViewAdmin.CurrentViewId = Guid.Empty;

            this.LoadView(objView);

            //Füllen der Pluginliste

            //Löschen der Plugins aus der liste, die bereits in der view verwendet werden
        }

        private void FillPluginList()
        {
            this.listViewPlugins.SmallImageList = new ImageList();

            foreach (ObjectPlugin obj in this._hashPlugins.Values)
            {
                string imageKey = TemplateTreenode.getImageForPluginType(obj.PluginType, this.listViewPlugins.SmallImageList);

                ListViewItem item = new ListViewItem(obj.PluginName);
                item.ImageKey = imageKey;
                item.Tag = obj;
                this.listViewPlugins.Items.Add(item);
            }
        }

        private void FillHelperList()
        {
            foreach (ObjectHelper obj in this._hashHelper.Values)
            {
                ListViewItem item = new ListViewItem(obj.PluginName);
                item.Tag = obj;
                this.listViewHelper.Items.Add(item);
            }
        }

        private void FillViewComboBox()
        {
            FillHashViews();

            _cmbViewSelector.Items.Clear(); //Die Liste der vorhandenen Views löschen?

            foreach (string var in this._hashViews.Keys)
            {
                int iSelectedItem = _cmbViewSelector.Items.Add(var);
            }
        }

        private void FillHashViews()
        {
            this._hashViews.Clear();

            try
            {
                ArrayList _idArray = new ArrayList();
                Error _error = null;
                ViewProperty.FindAllViewProperties(this._refViewAdmin.MyDBManager.Database, ref _idArray, ref _error);

                foreach (object var in _idArray)
                {
                    ViewProperty propView = new ViewProperty();
                    propView.Load(this._refViewAdmin.MyDBManager.Database, var, ref _error);
                    ObjectView opbView = new ObjectView();
                    opbView.ViewDescription = propView.ViewDescription;
                    opbView.ViewName = propView.ViewName;
                    opbView.ViewGUID = propView.Id.ToString();
                    this._hashViews.Add(propView.ViewName, opbView);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }

        private void InitHelperHash()
        {
            foreach (PluginDescriptor plgDesc in this._refViewAdmin.MyContext.PluginDescriptors)
            {
                try
                {
                    Type t = plgDesc.PluginInstance.GetType();
                    if (t.BaseType == typeof(SDBees.Plugs.TreenodeHelper.TemplateTreenodeHelper))
                    {
                        SDBees.Plugs.TreenodeHelper.TemplateTreenodeHelper helperInstance = (SDBees.Plugs.TreenodeHelper.TemplateTreenodeHelper)plgDesc.PluginInstance;
                        Console.WriteLine(t.BaseType.ToString());
                        ObjectHelper objHLP = new ObjectHelper();
                        objHLP.PluginType = plgDesc.PluginType.ToString();
                        objHLP.PluginName = plgDesc.PluginName;
                        objHLP.PluginTabPageLabel = helperInstance.TabPageName();
                        objHLP.Plugin = plgDesc.PluginInstance;
                        if (!this._hashHelper.ContainsKey(objHLP.PluginName))
                            this._hashHelper.Add(objHLP.PluginName, objHLP);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString() + "\n" + plgDesc.ToString());
                }
            }
        }

        private void InitPluginHash()
        {
            foreach (PluginDescriptor plgDesc in this._refViewAdmin.MyContext.PluginDescriptors)
            {
                try
                {
                    Type t = plgDesc.PluginInstance.GetType();
                    if (t.BaseType == typeof(SDBees.Plugs.TemplateTreeNode.TemplateTreenode))
                    {
                        //Console.WriteLine(t.BaseType.ToString());
                        ObjectPlugin objPLG = new ObjectPlugin();
                        objPLG.PluginType = plgDesc.PluginType.ToString();
                        objPLG.PluginName = plgDesc.PluginName;
                        objPLG.Plugin = plgDesc.PluginInstance;
                        if (!this._hashPlugins.ContainsKey(objPLG.PluginName))
                            this._hashPlugins.Add(objPLG.PluginName, objPLG);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString() + "\n" + plgDesc.ToString());
                }
            }
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            //Die alte View sichern?
            DialogResult dlgRes = DialogResult.None;
            if (_cmbViewSelector.Items.Count > 0)
            {
                dlgRes = MessageBox.Show("Soll die aktuelle View gespeichert werden?", "View Sichern?", MessageBoxButtons.OKCancel);
            }
            if (dlgRes == DialogResult.OK)
            {
                SaveView();
            }
            ViewAdminAddNew _dlgNewView = new ViewAdminAddNew(ref this._hashViews);
            _dlgNewView.ShowDialog();

            try
            {
                if (_dlgNewView.DialogResult == DialogResult.OK)
                {
                    this.listViewPlugins.Items.Clear(); //Die Liste der verfügbaren Plugins löschen
                    FillViewComboBox();
                    treeViewSystemConfig.Nodes.Clear(); //Die alte Treeview löschen
                    _cmbViewSelector.SelectedItem = _dlgNewView.ViewName;
                    // die ID setzen
                    ObjectView objView = (ObjectView)this._hashViews[_dlgNewView.ViewName];
                    this._sCurrentViewID = objView.ViewGUID;
                    if (this._sCurrentViewID != null)
                    {
                        this._refViewAdmin.CurrentViewId = new Guid(this._sCurrentViewID);
                    }

                    this.FillPluginList();
                    this.FillHelperList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult dlgRes = MessageBox.Show("Soll die aktuelle View gesichert werden?", "View Sichern", MessageBoxButtons.YesNo);

                if (dlgRes == DialogResult.Yes)
                {
                    SaveView();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region SaveView
        private void SaveView()
        {
            //Was passiert mit vorhandenen Einträgen? ViewProperty wird aktualisiert
            // ViewRelations werden gelöscht
            DeleteViewRelations();

            if (this.treeViewSystemConfig.TopNode != null)
            {
                this.SaveViewProperty((ObjectView)this._hashViews[this._cmbViewSelector.SelectedItem.ToString()]);

                foreach (TreeNode trn in this.treeViewSystemConfig.Nodes)
                {
                    SaveViewDefinition(trn, ViewRelation.m_StartNodeValue);
                }
            }
            else //Es ist eine leere View, der Treeview sollte leer sein. Sollen wir das zulassen? Nein
            {
                //this.SaveViewProperty((ObjectView)this._hashViews[this._cmbViewSelector.SelectedItem.ToString()]);
                MessageBox.Show("Die Viewdefiniton ist leer! Sie kann nicht gespeichert werden!");
            }
        }

        private void DeleteViewRelations()
        {
            if (this._sCurrentViewID == null)
                return;

            Error error = null;
            ArrayList lstEraseIds = new ArrayList();
            ViewDefinition viewDef = new ViewDefinition();

            try
            {
                ViewDefinition.FindViewDefinitions(this._refViewAdmin.MyDBManager.Database, ref lstEraseIds,
                    new Guid(this._sCurrentViewID), ref error);
                foreach (object var in lstEraseIds)
                {
                    viewDef.Load(this._refViewAdmin.MyDBManager.Database, var, ref error);
                    viewDef.Erase(ref error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SaveViewProperty(ObjectView objView)
        {
            try
            {
                Error error = null;
                ArrayList lstViewProps = new ArrayList();
                ViewProperty viewProp = new ViewProperty();
                if (this._sCurrentViewID != null)
                {
                    // Falls die view bereits existiert, laden...
                    viewProp.Load(this._refViewAdmin.MyDBManager.Database, this._sCurrentViewID, ref error);

                    Error.Display("View konnte nicht geladen werden", error);
                }
                else
                {
                    viewProp.SetDefaults(this._refViewAdmin.MyDBManager.Database);
                }

                viewProp.ViewName = objView.ViewName;
                viewProp.ViewDescription = objView.ViewDescription;
                viewProp.Save(ref error);

                if (this._sCurrentViewID == null)
                {
                    this._sCurrentViewID = viewProp.Id.ToString();
                    objView.ViewGUID = this._sCurrentViewID;
                    this._refViewAdmin.CurrentViewId = new Guid(this._sCurrentViewID);
                }

                if (error != null)
                    Error.Display("View Properties Erzeugung", error);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SaveViewDefinition(TreeNode trn, string tParentType)
        {
            try
            {
                foreach (TreeNode trNode in trn.Nodes)
                {
                    ObjectPlugin objPLGTemp = null;
                    if (trNode.Parent.Tag.GetType() == typeof(ObjectPlugin))
                    {
                        objPLGTemp = (ObjectPlugin)trNode.Parent.Tag;
                    }
                    SaveViewDefinition(trNode, objPLGTemp.PluginType);
                }

                Error error = null;
                ViewDefinition viewDef = new ViewDefinition();
                viewDef.Database = this._refViewAdmin.MyDBManager.Database;
                ObjectPlugin objTag = (ObjectPlugin)trn.Tag;
                //viewDef.Id = new Guid(this._sCurrentViewID);
                viewDef.ParentType = tParentType;
                viewDef.ChildType = objTag.PluginType;
                viewDef.ChildName = objTag.PluginName;
                viewDef.ViewId = this._sCurrentViewID;
                viewDef.Save(ref error);

                if (error != null)
                    Error.Display("View Definition Erzeugung", error);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }


        #endregion


        #region LoadView
        /// <summary>
        /// Loads the Views defined in the database
        /// </summary>
        private void LoadView(ObjectView objView)
        {
            Database database = this._refViewAdmin.MyDBManager.Database;
            Error error = null;
            database.Open(false, ref error);

            this.treeViewSystemConfig.Nodes.Clear();
            this.treeViewSystemConfig.ImageList = new ImageList();
            this.LoadViewStartDefinition(objView);

            database.Close(ref error);
        }

        private void LoadViewStartDefinition(ObjectView objView)
        {
            try
            {
                if (objView.ViewGUID == null)
                    return;

                ArrayList lstObjViewDefs = new ArrayList();
                Error error = null;

                ViewDefinition.FindViewDefinitionsByParentType(this._refViewAdmin.MyDBManager.Database,
                  ref lstObjViewDefs, new Guid(objView.ViewGUID), ViewRelation.m_StartNodeValue, ref error);

                if (lstObjViewDefs.Count == 1)
                {
                    this._sCurrentViewID = objView.ViewGUID;
                    this._refViewAdmin.CurrentViewId = new Guid(this._sCurrentViewID);
                }

                ViewDefinition viewDef = new ViewDefinition();
                foreach (object var in lstObjViewDefs)
                {
                    //Das Startteil herausfinden. Kann ja auch unsortiert sein ...
                    viewDef.Load(this._refViewAdmin.MyDBManager.Database, var, ref error);
                    if (viewDef.ParentType == ViewRelation.m_StartNodeValue)
                    {
                        string imageKey = TemplateTreenode.getImageForPluginType(viewDef.ChildType, this.treeViewSystemConfig.ImageList);

                        TreeNode trnPLG = new TreeNode(viewDef.ChildName);
                        trnPLG.ImageKey = imageKey;
                        trnPLG.SelectedImageKey = imageKey;
                        ObjectPlugin objPLG = new ObjectPlugin();
                        objPLG.PluginName = viewDef.ChildName;
                        objPLG.PluginType = viewDef.ChildType;
                        objPLG.ViewID = viewDef.ViewId;
                        objPLG.ID = viewDef.Id.ToString();
                        trnPLG.Tag = objPLG;
                        this.treeViewSystemConfig.Nodes.Add(trnPLG);


                        error = LoadViewSubDefinitions(objView, error, viewDef.ChildType, trnPLG);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }
        }

        private Error LoadViewSubDefinitions(ObjectView objView, Error error, string sParentType, TreeNode trnPLGParent)
        {
            ArrayList lstChilds = new ArrayList();
            int iDefs = ViewDefinition.FindViewDefinitionsByParentType(this._refViewAdmin.MyDBManager.Database, ref lstChilds,
              new Guid(objView.ViewGUID), sParentType, ref error);

            ViewDefinition viewDefSub = new ViewDefinition();
            foreach (object obj in lstChilds)
            {
                viewDefSub.Load(this._refViewAdmin.MyDBManager.Database, obj, ref error);

                string imageKey = TemplateTreenode.getImageForPluginType(viewDefSub.ChildType, this.treeViewSystemConfig.ImageList);

                TreeNode trnPLG_SUB = new TreeNode(viewDefSub.ChildName);
                trnPLG_SUB.ImageKey = imageKey;
                trnPLG_SUB.SelectedImageKey = imageKey;
                ObjectPlugin objPLG_SUB = new ObjectPlugin();
                objPLG_SUB.PluginName = viewDefSub.ChildName;
                objPLG_SUB.PluginType = viewDefSub.ChildType;
                objPLG_SUB.ViewID = viewDefSub.ViewId;
                objPLG_SUB.ID = viewDefSub.Id.ToString();
                trnPLG_SUB.Tag = objPLG_SUB;
                trnPLGParent.Nodes.Add(trnPLG_SUB);

                error = LoadViewSubDefinitions(objView, error, viewDefSub.ChildType, trnPLG_SUB);
            }
            return error;
        }

        #endregion


        #region DragDrop

        private void listViewPlugins_ItemDrag(object sender, ItemDragEventArgs e)
        {
            //Set the drag mode and initiate the DragDrop 
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void treeViewConfig_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", true))
            {
                TreeView trvSelected = (TreeView)sender;
                TreeNode dropNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode", true);

                if (trvSelected.SelectedNode != null)
                {
                    TreeNode targetNode = trvSelected.SelectedNode;
                    dropNode.Remove();

                    targetNode.Nodes.Add(dropNode);
                    dropNode.EnsureVisible();
                    trvSelected.SelectedNode = dropNode;
                }
            }
            else if (e.Data.GetDataPresent("System.Windows.Forms.ListViewItem", true))
            {
                TreeView trvSelected = (TreeView)sender;
                ListViewItem lstItem = (ListViewItem)e.Data.GetData("System.Windows.Forms.ListViewItem", true);

                ObjectPlugin obj = (ObjectPlugin)lstItem.Tag;
                string imageKey = TemplateTreenode.getImageForPluginType(obj.PluginType, trvSelected.ImageList);

                if (trvSelected.SelectedNode != null)
                {
                    //Es wird auf einen Viewnamen gedragd
                    TreeNode trnNew = new TreeNode(lstItem.Text);
                    trnNew.ImageKey = imageKey;
                    trnNew.SelectedImageKey = imageKey;
                    trnNew.Tag = lstItem.Tag;
                    trvSelected.SelectedNode.Nodes.Add(trnNew);
                    lstItem.Remove();
                }
                else
                {
                    if (trvSelected.Nodes.Count == 0)
                    {
                        TreeNode trnNew = new TreeNode(lstItem.Text);
                        trnNew.ImageKey = imageKey;
                        trnNew.Tag = lstItem.Tag;
                        trvSelected.Nodes.Add(trnNew);
                        lstItem.Remove();
                    }
                }
                trvSelected.ExpandAll();
            }
        }

        private void treeViewConfig_DragEnter(object sender, DragEventArgs e)
        {
            //See if there is a TreeNode being dragged
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", true))
            {
                //TreeNode found allow move effect
                e.Effect = DragDropEffects.Move;
            }

            else if (e.Data.GetDataPresent("System.Windows.Forms.ListViewItem", true))
            {
                //ListviewItem found allow move effect
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                //No TreeNode found, prevent move
                e.Effect = DragDropEffects.None;
            }
        }

        private void treeViewConfig_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("System.Windows.Forms.TreeNode", true))
            {
                //Die Treeview besorgen
                TreeView trvSelected = (TreeView)sender;

                //Highlight der Nodes
                Point pt = trvSelected.PointToClient(new Point(e.X, e.Y));
                TreeNode tnTarget = trvSelected.GetNodeAt(pt);

                if (tnTarget != trvSelected.SelectedNode) //) && (tnTarget.Tag.GetType() == typeof(ObjectPlugin)))
                {
                    trvSelected.SelectedNode = tnTarget;

                    //Check that the selected node is not the dropNode and
                    //also that it is not a child of the dropNode and
                    //therefore an invalid target
                    TreeNode dropNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode", true);

                    /*
                    while (tnTarget != null)
                    {
                      if (tnTarget == dropNode)
                      {
                        e.Effect = DragDropEffects.None;
                        return;
                      }
                      tnTarget = tnTarget.Parent;
                    }
                     * */
                    e.Effect = DragDropEffects.Move;
                }
            }
            else if (e.Data.GetDataPresent("System.Windows.Forms.ListViewItem", true)) //Es wird ein Listviewitem gedraged
            {
                TreeView trvSelected = (TreeView)sender;
                Point pt = trvSelected.PointToClient(new Point(e.X, e.Y));
                TreeNode tnTarget = trvSelected.GetNodeAt(pt);
                if (tnTarget != null) //Das ListviewItem wird auf den Viewnamen gezogen
                {
                    trvSelected.SelectedNode = tnTarget;
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void treeViewConfig_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode tn = (TreeNode)e.Item;
            if (tn.Tag.GetType() == typeof(ObjectPlugin))
            {
                //Set the drag node and initiate the DragDrop 
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        #endregion


        #region LabelEdit + Löschen
        private void treeViewConfig_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node.Tag.GetType() == typeof(ObjectView))
            {
                ObjectView objView = (ObjectView)e.Node.Tag;
                this._cmbViewSelector.Items.Remove(objView.ViewName);
                objView.ViewName = e.Label;
                e.Node.Tag = objView;
            }
            else if (e.Node.Tag.GetType() == typeof(ObjectPlugin))
            {
                e.CancelEdit = true;
            }
        }

        private void treeViewConfig_KeyPress(object sender, KeyPressEventArgs e)
        {
        }

        private void treeViewConfig_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DialogResult dlgRes = MessageBox.Show("Do you really want to delete the selected node and the subnodes?", "Attention", MessageBoxButtons.YesNo);
                if (dlgRes == DialogResult.Yes)
                {
                    RemoveNodeAndAddBackToList(this.treeViewSystemConfig.SelectedNode);
                }
            }
            else if (e.KeyCode == Keys.F2)
            {
                this.treeViewSystemConfig.SelectedNode.BeginEdit();
            }
        }

        private void RemoveNodeAndAddBackToList(TreeNode trNode)
        {
            foreach (TreeNode trNodeTemp in trNode.Nodes)
            {
                RemoveNodeAndAddBackToList(trNodeTemp);
            }

            ObjectPlugin obj = (ObjectPlugin)trNode.Tag;
            string imageKey = TemplateTreenode.getImageForPluginType(obj.PluginType, this.listViewPlugins.SmallImageList);

            ListViewItem lstItem = new ListViewItem(trNode.Text);
            lstItem.ImageKey = imageKey;
            lstItem.Tag = trNode.Tag;
            this.listViewPlugins.Items.Add(lstItem);
            trNode.Remove();
        }
        #endregion


        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveView();
        }

        private void ViewAdminDLG_Shown(object sender, EventArgs e)
        {
            RefreshView();
        }
    }
}
