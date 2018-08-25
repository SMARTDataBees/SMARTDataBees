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
using System.Drawing;
using System.Windows.Forms;
using Carbon.Plugins;
using SDBees.DB;
using SDBees.Plugs.Objects;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;

namespace SDBees.Core.Admin
{
    public partial class AdminDialog : Form
    {
        private readonly ViewAdmin _refViewAdmin;
        private readonly Hashtable _hashPlugins;
        private readonly Hashtable _hashHelper;

        private Hashtable _hashViews;
        private string _currentViewId;

        public ToolStripComboBox _cmbViewSelector;

        private bool _isConfigNodeSelected;

        public AdminDialog(ViewAdmin baseRef)
        {
            try
            {
                _refViewAdmin = baseRef;
                InitializeComponent();

                _hashPlugins = new Hashtable();
                _hashHelper = new Hashtable();
                _hashViews = new Hashtable();

                _currentViewId = null;

                _cmbViewSelector = new ToolStripComboBox
                {
                    Alignment = ToolStripItemAlignment.Right,
                    Sorted = true,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                var pluginListContextMenu = new ContextMenu();
                pluginListContextMenu.Popup += pluginListContextMenu_Popup;
                m_listViewPlugins.ContextMenu = pluginListContextMenu;

                _cmbViewSelector.SelectedIndexChanged += _cmbViewSelector_SelectedIndexChanged;
                if (_cmbViewSelector.Items.Count > 0)
                    _cmbViewSelector.SelectedIndex = 0;

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
            FillViewComboBox();
        }

        void pluginListContextMenu_Popup(object sender, EventArgs e)
        {
            m_listViewPlugins.ContextMenu.MenuItems.Clear();

            if (m_listViewPlugins.SelectedItems.Count == 1)
            {
                var lvi = m_listViewPlugins.SelectedItems[0];

                var menuEditSchema = new MenuItem("Edit schema of " + lvi.Text) {Tag = lvi.Tag};
                menuEditSchema.Click += OnEditSchema;
                m_listViewPlugins.ContextMenu.MenuItems.Add(menuEditSchema);
            }
        }

        void OnEditSchema(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var objPlug = (ObjectPlugin)menuItem.Tag;
            var plugin = TemplateTreenode.GetPluginForType(objPlug.PluginType);
            plugin.EditSchema();
        }


        private void ViewAdminDLG_Load(object sender, EventArgs e)
        {
            try
            {
                toolStrip1.Items.Add(_cmbViewSelector);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void _cmbViewSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Einlesen der Viewdaten
            var objView = (ObjectView)_hashViews[_cmbViewSelector.SelectedItem.ToString()];

            _currentViewId = objView.ViewGUID;
            if (_currentViewId != null)
                _refViewAdmin.CurrentViewId = new Guid(_currentViewId);
            else
                _refViewAdmin.CurrentViewId = Guid.Empty;

            LoadView(objView);

            //Füllen der Pluginliste

            //Löschen der Plugins aus der liste, die bereits in der view verwendet werden
        }

        private void FillPluginList()
        {
            m_listViewPlugins.SmallImageList = new ImageList();
            m_listViewPlugins.Items.Clear();

            foreach (ObjectPlugin obj in _hashPlugins.Values)
            {
                var imageKey = TemplateTreenode.getImageForPluginType(obj.PluginType, m_listViewPlugins.SmallImageList);

                var item = new ListViewItem(obj.PluginName);
                item.ImageKey = imageKey;
                item.Tag = obj;
                m_listViewPlugins.Items.Add(item);
            }
        }

        private void FillHelperList()
        {
            foreach (ObjectHelper obj in _hashHelper.Values)
            {
                var item = new ListViewItem(obj.PluginName) { Tag = obj };
                m_listViewHelper.Items.Add(item);
            }
        }

        private void FillViewComboBox()
        {
            FillHashViews();

            _cmbViewSelector.Items.Clear(); //Die Liste der vorhandenen Views löschen?

            foreach (string var in _hashViews.Keys)
            {
                var iSelectedItem = _cmbViewSelector.Items.Add(var);
            }
            var propView = new ViewProperty();
            Error error = null;
            propView.Load(_refViewAdmin.MyDBManager.Database, _refViewAdmin.CurrentViewId, ref error);
            var index = _cmbViewSelector.FindStringExact(propView.ViewName);
            if (index >= 0)
            {
                _cmbViewSelector.SelectedIndex = index;
            }
        }

        private void FillHashViews()
        {
            _hashViews.Clear();

            try
            {
                var ids = new ArrayList();
                Error error = null;
                ViewProperty.FindAllViewProperties(_refViewAdmin.MyDBManager.Database, ref ids, ref error);

                foreach (var id in ids)
                {
                    var propView = new ViewProperty();
                    propView.Load(_refViewAdmin.MyDBManager.Database, id, ref error);
                    var opbView = new ObjectView
                    {
                        ViewDescription = propView.ViewDescription,
                        ViewName = propView.ViewName,
                        ViewGUID = propView.Id.ToString()
                    };
                    if (_hashViews.ContainsKey(propView.ViewName) == false)
                        _hashViews.Add(propView.ViewName, opbView);
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
            foreach (PluginDescriptor plgDesc in _refViewAdmin.MyContext.PluginDescriptors)
            {
                try
                {
                    var t = plgDesc.PluginInstance.GetType();
                    if (t.BaseType == typeof(TemplateTreenodeHelper))
                    {
                        var helperInstance = (TemplateTreenodeHelper)plgDesc.PluginInstance;
                        Console.WriteLine(t.BaseType.ToString());
                        var objHLP = new ObjectHelper();
                        objHLP.PluginType = plgDesc.PluginType.ToString();
                        objHLP.PluginName = plgDesc.PluginName;
                        objHLP.PluginTabPageLabel = helperInstance.TabPageName();
                        objHLP.Plugin = plgDesc.PluginInstance;
                        if (!_hashHelper.ContainsKey(objHLP.PluginName))
                            _hashHelper.Add(objHLP.PluginName, objHLP);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex + Environment.NewLine + plgDesc);
                }
            }
        }

        private void InitPluginHash()
        {
            foreach (PluginDescriptor plgDesc in _refViewAdmin.MyContext.PluginDescriptors)
            {
                try
                {
                    var t = plgDesc.PluginInstance.GetType();
                    if (t.BaseType == typeof(TemplateTreenode))
                    {
                        //Console.WriteLine(t.BaseType.ToString());
                        var objPLG = new ObjectPlugin();
                        objPLG.PluginType = plgDesc.PluginType.ToString();
                        objPLG.PluginName = plgDesc.PluginName;
                        objPLG.Plugin = plgDesc.PluginInstance;
                        if (!_hashPlugins.ContainsKey(objPLG.PluginName))
                            _hashPlugins.Add(objPLG.PluginName, objPLG);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex + @"" + plgDesc);
                }
            }
        }

        private void buttonNew_Click(object sender, EventArgs e)
        {
            //Die alte View sichern?
            if (_cmbViewSelector.Items.Count > 0)
            {
                var result = MessageBox.Show(@"Should the current data be stored ? ", @"Save data ? ", MessageBoxButtons.YesNo);
                if (result == DialogResult.OK)
                    SaveView();
            }
           
            var dlgNewView = new ViewAdminAddNew(ref _hashViews);
            dlgNewView.ShowDialog();

            try
            {
                if (dlgNewView.DialogResult == DialogResult.OK)
                {

                    //if(this._hashViews.ContainsKey(_dlgNewView.ViewName))
                    var opbView = new ObjectView();
                    opbView.ViewDescription = dlgNewView.ViewDescription;
                    opbView.ViewName = dlgNewView.ViewName;
                    opbView.ViewGUID = dlgNewView.ViewGuid;
                    _hashViews.Add(dlgNewView.ViewName, opbView);


                    SaveViewProperty((ObjectView)_hashViews[dlgNewView.ViewName]);

                    m_listViewPlugins.Items.Clear(); //Die Liste der verfügbaren Plugins löschen
                    FillViewComboBox();
                    m_treeViewSystemConfig.Nodes.Clear(); //Die alte Treeview löschen
                    _cmbViewSelector.SelectedItem = dlgNewView.ViewName;
                    // die ID setzen
                    var objView = (ObjectView)_hashViews[dlgNewView.ViewName];
                    _currentViewId = objView.ViewGUID;
                   
                    FillPluginList();
                    FillHelperList();
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
                var dlgRes = MessageBox.Show(@"Should the data be stored?", @"Save data?", MessageBoxButtons.YesNo);

                if (dlgRes == DialogResult.Yes)
                    SaveView();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        #region SaveView
        private void SaveView()
        {
            //Was passiert mit vorhandenen Einträgen? ViewProperty wird aktualisiert
            // ViewRelations werden gelöscht
            DeleteViewRelations();

            if (m_treeViewSystemConfig.TopNode != null)
            {
                if (_cmbViewSelector.SelectedItem != null)
                    SaveViewProperty((ObjectView)_hashViews[_cmbViewSelector.SelectedItem.ToString()]);

                foreach (TreeNode trn in m_treeViewSystemConfig.Nodes)
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
            if (_currentViewId == null)
                return;

            Error error = null;
            var lstEraseIds = new ArrayList();
            var viewDef = new ViewDefinition();

            try
            {
                ViewDefinition.FindViewDefinitions(_refViewAdmin.MyDBManager.Database, ref lstEraseIds,
                    new Guid(_currentViewId), ref error);
                foreach (var var in lstEraseIds)
                {
                    viewDef.Load(_refViewAdmin.MyDBManager.Database, var, ref error);
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
                var lstViewProps = new ArrayList();
                var viewProp = new ViewProperty();
                if (_currentViewId != null)
                {
                    // Falls die view bereits existiert, laden...
                    viewProp.Load(_refViewAdmin.MyDBManager.Database, _currentViewId, ref error);

                   // Error.Display("View konnte nicht geladen werden", error);
                }
                else
                {
                    viewProp.SetDefaults(_refViewAdmin.MyDBManager.Database);
                }

                viewProp.ViewName = objView.ViewName;
                viewProp.ViewDescription = objView.ViewDescription;
                viewProp.Save(ref error);

                if (_currentViewId == null)
                {
                    _currentViewId = viewProp.Id.ToString();
                    objView.ViewGUID = _currentViewId;
                    _refViewAdmin.CurrentViewId = new Guid(_currentViewId);
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
                var viewDef = new ViewDefinition {Database = _refViewAdmin.MyDBManager.Database};
                var objTag = (ObjectPlugin)trn.Tag;
                viewDef.ParentType = tParentType;
                viewDef.ChildType = objTag.PluginType;
                viewDef.ChildName = objTag.PluginName;
                viewDef.ViewId = _currentViewId;
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
            var database = _refViewAdmin.MyDBManager.Database;
            Error error = null;
            database.Open(false, ref error);

            m_treeViewSystemConfig.Nodes.Clear();
            m_treeViewSystemConfig.ImageList = new ImageList();
            LoadViewStartDefinition(objView);

            database.Close(ref error);
        }

        private void LoadViewStartDefinition(ObjectView objView)
        {
            try
            {
                if (objView.ViewGUID == null)
                    return;

                var lstObjViewDefs = new ArrayList();
                Error error = null;

                ViewDefinition.FindViewDefinitionsByParentType(_refViewAdmin.MyDBManager.Database,
                  ref lstObjViewDefs, new Guid(objView.ViewGUID), ViewRelation.m_StartNodeValue, ref error);

                if (lstObjViewDefs.Count == 1)
                {
                    _currentViewId = objView.ViewGUID;
                    _refViewAdmin.CurrentViewId = new Guid(_currentViewId);
                }

                var viewDef = new ViewDefinition();
                foreach (var var in lstObjViewDefs)
                {
                    //Das Startteil herausfinden. Kann ja auch unsortiert sein ...
                    viewDef.Load(_refViewAdmin.MyDBManager.Database, var, ref error);
                    if (viewDef.ParentType == ViewRelation.m_StartNodeValue)
                    {
                        var imageKey = TemplateTreenode.getImageForPluginType(viewDef.ChildType, m_treeViewSystemConfig.ImageList);

                        var trnPLG = new TreeNode(viewDef.ChildName);
                        trnPLG.ImageKey = imageKey;
                        trnPLG.SelectedImageKey = imageKey;
                        var objPLG = new ObjectPlugin
                        {
                            PluginName = viewDef.ChildName,
                            PluginType = viewDef.ChildType,
                            ViewID = viewDef.ViewId,
                            ID = viewDef.Id.ToString()
                        };
                        trnPLG.Tag = objPLG;
                        m_treeViewSystemConfig.Nodes.Add(trnPLG);


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
            var lstChilds = new ArrayList();
            var iDefs = ViewDefinition.FindViewDefinitionsByParentType(_refViewAdmin.MyDBManager.Database, ref lstChilds,
              new Guid(objView.ViewGUID), sParentType, ref error);

            var viewDefSub = new ViewDefinition();
            foreach (var obj in lstChilds)
            {
                viewDefSub.Load(_refViewAdmin.MyDBManager.Database, obj, ref error);

                var imageKey = TemplateTreenode.getImageForPluginType(viewDefSub.ChildType, m_treeViewSystemConfig.ImageList);

                var trnPLG_SUB = new TreeNode(viewDefSub.ChildName);
                trnPLG_SUB.ImageKey = imageKey;
                trnPLG_SUB.SelectedImageKey = imageKey;
                var objPLG_SUB = new ObjectPlugin();
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
                var trvSelected = (TreeView)sender;
                var dropNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode", true);

                if (trvSelected.SelectedNode != null)
                {
                    var targetNode = trvSelected.SelectedNode;
                    dropNode.Remove();

                    targetNode.Nodes.Add(dropNode);
                    dropNode.EnsureVisible();
                    trvSelected.SelectedNode = dropNode;
                }
            }
            else if (e.Data.GetDataPresent("System.Windows.Forms.ListViewItem", true))
            {
                var trvSelected = (TreeView)sender;
                var lstItem = (ListViewItem)e.Data.GetData("System.Windows.Forms.ListViewItem", true);

                var obj = (ObjectPlugin)lstItem.Tag;
                var imageKey = TemplateTreenode.getImageForPluginType(obj.PluginType, trvSelected.ImageList);

                if (trvSelected.SelectedNode != null)
                {
                    //Es wird auf einen Viewnamen gedragd
                    var trnNew = new TreeNode(lstItem.Text);
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
                        var trnNew = new TreeNode(lstItem.Text);
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
                var trvSelected = (TreeView)sender;

                //Highlight der Nodes
                var pt = trvSelected.PointToClient(new Point(e.X, e.Y));
                var tnTarget = trvSelected.GetNodeAt(pt);

                if (tnTarget != trvSelected.SelectedNode) //) && (tnTarget.Tag.GetType() == typeof(ObjectPlugin)))
                {
                    trvSelected.SelectedNode = tnTarget;

                    //Check that the selected node is not the dropNode and
                    //also that it is not a child of the dropNode and
                    //therefore an invalid target
                    var dropNode = (TreeNode)e.Data.GetData("System.Windows.Forms.TreeNode", true);

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
                var trvSelected = (TreeView)sender;
                var pt = trvSelected.PointToClient(new Point(e.X, e.Y));
                var tnTarget = trvSelected.GetNodeAt(pt);
                if (tnTarget != null) //Das ListviewItem wird auf den Viewnamen gezogen
                {
                    trvSelected.SelectedNode = tnTarget;
                    e.Effect = DragDropEffects.Move;
                }
            }
        }

        private void treeViewConfig_ItemDrag(object sender, ItemDragEventArgs e)
        {
            var tn = (TreeNode)e.Item;
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
                var objView = (ObjectView)e.Node.Tag;
                _cmbViewSelector.Items.Remove(objView.ViewName);
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
                var dlgRes = MessageBox.Show("Do you really want to delete the selected node and the subnodes?", "Attention", MessageBoxButtons.YesNo);
                if (dlgRes == DialogResult.Yes)
                {
                    RemoveNodeAndAddBackToList(m_treeViewSystemConfig.SelectedNode);
                }
            }
            else if (e.KeyCode == Keys.F2)
            {
                m_treeViewSystemConfig.SelectedNode.BeginEdit();
            }
        }

        private void RemoveNodeAndAddBackToList(TreeNode trNode)
        {
            foreach (TreeNode trNodeTemp in trNode.Nodes)
            {
                RemoveNodeAndAddBackToList(trNodeTemp);
            }

            var obj = (ObjectPlugin)trNode.Tag;
            var imageKey = TemplateTreenode.getImageForPluginType(obj.PluginType, m_listViewPlugins.SmallImageList);

            var lstItem = new ListViewItem(trNode.Text);
            lstItem.ImageKey = imageKey;
            lstItem.Tag = trNode.Tag;
            m_listViewPlugins.Items.Add(lstItem);
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
