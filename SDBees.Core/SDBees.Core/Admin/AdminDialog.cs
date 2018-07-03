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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Carbon.Plugins;
using SDBees.DB;
using SDBees.Plugs.Objects;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;
using View = SDBees.Plugs.Objects.View;

namespace SDBees.Core.Admin
{
    public partial class AdminDialog : Form
    {
        private readonly AdminView _adminView;
        private readonly Hashtable _plugins;
        private readonly Hashtable _hashHelper;

        private IList<View> _views;
        public string CurrentViewIdentification { get; private set; }

        private readonly ToolStripComboBox _viewSelector;

        public AdminDialog(AdminView adminView)
        {
            try
            {
                InitializeComponent();

                _adminView = adminView;
                _plugins = new Hashtable();
                _views = new List<View>();
                _hashHelper = new Hashtable();
                CurrentViewIdentification = null;

                _viewSelector = new ToolStripComboBox
                {
                    Alignment = ToolStripItemAlignment.Right,
                    Sorted = true,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                var pluginListContextMenu = new ContextMenu();
                pluginListContextMenu.Popup += pluginListContextMenu_Popup;
                AvailablePluginsListView.ContextMenu = pluginListContextMenu;

                _viewSelector.SelectedIndexChanged += OnViewSelectionChanged;
               
                if (_viewSelector.Items.Count > 0)
                    _viewSelector.SelectedIndex = 0;

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
            AvailablePluginsListView.ContextMenu.MenuItems.Clear();

            if (AvailablePluginsListView.SelectedItems.Count == 1)
            {
                var lvi = AvailablePluginsListView.SelectedItems[0];

                var menuEditSchema = new MenuItem("Edit schema of " + lvi.Text) { Tag = lvi.Tag };
                menuEditSchema.Click += OnEditSchema;
                AvailablePluginsListView.ContextMenu.MenuItems.Add(menuEditSchema);
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
                toolStrip1.Items.Add(_viewSelector);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void OnViewSelectionChanged(object sender, EventArgs e)
        {
            var selectedViewNam = _viewSelector.SelectedItem.ToString();
            var view = _views.FirstOrDefault(vw => vw.Name.Equals(selectedViewNam));

            CurrentViewIdentification = view?.Identification;
            _adminView.CurrentViewId = CurrentViewIdentification != null ? new Guid(CurrentViewIdentification) : Guid.Empty;
          
            LoadView(view);
        }

        private void FillPluginList()
        {
            AvailablePluginsListView.SmallImageList = new ImageList();
            AvailablePluginsListView.Items.Clear();

            foreach (ObjectPlugin obj in _plugins.Values)
            {
                var imageKey = TemplateTreenode.getImageForPluginType(obj.PluginType, AvailablePluginsListView.SmallImageList);

                var item = new ListViewItem(obj.PluginName);
                item.ImageKey = imageKey;
                item.Tag = obj;
                AvailablePluginsListView.Items.Add(item);
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
            _viewSelector.Items.Clear();
            _views = GetViews(_adminView.DBManager.Database);

            if (_views != null)
                foreach (var view in _views)
                    _viewSelector.Items.Add(view.Name);

            if (_views != null)
            {
                var viewForSelection = _views.FirstOrDefault(vw => vw.Identification.Equals(_adminView.CurrentViewId + ""));
                var index = _views.IndexOf(viewForSelection);
                if (index >= 0)
                    _viewSelector.SelectedIndex = index;
            }


            //var propView = new ViewProperty();
            //Error error = null;
            //propView.Load(_adminView.MyDBManager.Database, _adminView.CurrentViewId, ref error);
            //var index = _viewSelector.FindStringExact(propView.ViewName);
            //if (index >= 0)
            //{
            //    _viewSelector.SelectedIndex = index;
            //}
        }


        private IList<View> GetViews(Database database)
        {
            var views = new List<View>();
            try
            {
                var ids = new ArrayList();
                Error error = null;
                ViewProperty.FindAllViewProperties(database, ref ids, ref error);

                foreach (var id in ids)
                {
                    var viewProperty = new ViewProperty();
                    viewProperty.Load(_adminView.DBManager.Database, id, ref error);
                    var opbView = new View
                    {
                        Description = viewProperty.ViewDescription,
                        Name = viewProperty.ViewName,
                        Identification = viewProperty.Id.ToString()
                    };


                    if (views.FirstOrDefault(vw => vw.Name.Equals(opbView.Name)) == null)
                        views.Add(opbView);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                //throw;
            }

            return views;
        }




        private void InitHelperHash()
        {
            foreach (PluginDescriptor plgDesc in _adminView.MyContext.PluginDescriptors)
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
            foreach (PluginDescriptor plgDesc in _adminView.MyContext.PluginDescriptors)
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
                        if (!_plugins.ContainsKey(objPLG.PluginName))
                            _plugins.Add(objPLG.PluginName, objPLG);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex + @"" + plgDesc);
                }
            }
        }

        private void OnNewConfiguration(object sender, EventArgs eventArgs)
        {
            //Die alte View sichern?
            if (_viewSelector.Items.Count > 0)
            {
                var result = MessageBox.Show(@"Should the current data be stored? ", @"Save data ? ", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    var selectedViewName = _viewSelector.SelectedItem.ToString();
                    var view = _views.FirstOrDefault(vw => vw.Name.Equals(selectedViewName));
                    Save(view, PluginConfigurationTreeView.Nodes);

                    CurrentViewIdentification = view.Identification;
                    _adminView.CurrentViewId = new Guid(CurrentViewIdentification);
                }
            }


            var dlgNewView = new AddPluginConfigurationForm();
            try
            {
                if (dlgNewView.ShowDialog() == DialogResult.OK)
                {
                    var view = new View
                    {
                        Description = dlgNewView.View.Description,
                        Name = dlgNewView.View.Name,
                        Identification = dlgNewView.View.Identification
                    };


                    Save(view, PluginConfigurationTreeView.Nodes);

                    _views.Add(view);
                    CurrentViewIdentification = view.Identification;
                    _adminView.CurrentViewId = new Guid(CurrentViewIdentification);


                    AvailablePluginsListView.Items.Clear();
                    FillViewComboBox();
                    PluginConfigurationTreeView.Nodes.Clear(); //Die alte Treeview löschen
                    _viewSelector.SelectedItem = dlgNewView.View.Name;


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

        private void OnClose(object sender, EventArgs e)
        {
            try
            {
                var dlgRes = MessageBox.Show(@"Should the data be stored?", @"Save data?", MessageBoxButtons.YesNo);

                if (dlgRes == DialogResult.Yes)
                {
                    var selectedViewName = _viewSelector.SelectedItem.ToString();
                    var view = _views.FirstOrDefault(vw => vw.Name.Equals(selectedViewName));
                    Save(view, PluginConfigurationTreeView.Nodes);
                }

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
        private void Save(View view, IEnumerable nodes)
        {
            if (view == null)
                return;

            //Was passiert mit vorhandenen Einträgen? ViewProperty wird aktualisiert
            // ViewRelations werden gelöscht
            DeleteViewRelations();

            //if (PluginConfigurationTreeView.TopNode != null)
            //{

            SaveViewProperty(view);

            foreach (TreeNode node in nodes)
                SaveViewDefinition(node, ViewRelation.m_StartNodeValue);
            //}
            //else //Es ist eine leere View, der Treeview sollte leer sein. Sollen wir das zulassen? Nein
            //{
            //    //this.SaveViewProperty((ObjectView)this._hashViews[this._cmbViewSelector.SelectedItem.ToString()]);
            //    MessageBox.Show("Die Viewdefiniton ist leer! Sie kann nicht gespeichert werden!");
            //}
        }

        private void DeleteViewRelations()
        {
            if (CurrentViewIdentification == null)
                return;

            Error error = null;
            var lstEraseIds = new ArrayList();
            var viewDef = new ViewDefinition();

            try
            {
                ViewDefinition.FindViewDefinitions(_adminView.DBManager.Database, ref lstEraseIds,
                    new Guid(CurrentViewIdentification), ref error);
                foreach (var var in lstEraseIds)
                {
                    viewDef.Load(_adminView.DBManager.Database, var, ref error);
                    viewDef.Erase(ref error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SaveViewProperty(View objView)
        {
            try
            {
                Error error = null;

                var viewProp = new ViewProperty();
                if (objView.Identification.Equals(CurrentViewIdentification))
                    viewProp.Load(_adminView.DBManager.Database, objView.Identification, ref error);
                else
                    viewProp.SetDefaults(_adminView.DBManager.Database);


                viewProp.ViewName = objView.Name;
                viewProp.ViewDescription = objView.Description;
                viewProp.Save(ref error);


                if (error != null)
                    Error.Display("View properties save error", error);

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
                var viewDef = new ViewDefinition { Database = _adminView.DBManager.Database };
                var objTag = (ObjectPlugin)trn.Tag;
                viewDef.ParentType = tParentType;
                viewDef.ChildType = objTag.PluginType;
                viewDef.ChildName = objTag.PluginName;
                viewDef.ViewId = CurrentViewIdentification;
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
        private void LoadView(View objView)
        {
            var database = _adminView.DBManager.Database;
            Error error = null;
            database.Open(false, ref error);

            PluginConfigurationTreeView.Nodes.Clear();
            PluginConfigurationTreeView.ImageList = new ImageList();
            LoadViewStartDefinition(objView);

            database.Close(ref error);
        }

        private void LoadViewStartDefinition(View objView)
        {
            try
            {
                if (objView.Identification == null)
                    return;

                var lstObjViewDefs = new ArrayList();
                Error error = null;

                ViewDefinition.FindViewDefinitionsByParentType(_adminView.DBManager.Database,
                  ref lstObjViewDefs, new Guid(objView.Identification), ViewRelation.m_StartNodeValue, ref error);

                if (lstObjViewDefs.Count == 1)
                {
                    CurrentViewIdentification = objView.Identification;
                    _adminView.CurrentViewId = new Guid(CurrentViewIdentification);
                }

                var viewDef = new ViewDefinition();
                foreach (var var in lstObjViewDefs)
                {
                    //Das Startteil herausfinden. Kann ja auch unsortiert sein ...
                    viewDef.Load(_adminView.DBManager.Database, var, ref error);
                    if (viewDef.ParentType == ViewRelation.m_StartNodeValue)
                    {
                        var imageKey = TemplateTreenode.getImageForPluginType(viewDef.ChildType, PluginConfigurationTreeView.ImageList);

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
                        PluginConfigurationTreeView.Nodes.Add(trnPLG);


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

        private Error LoadViewSubDefinitions(View objView, Error error, string sParentType, TreeNode trnPLGParent)
        {
            var lstChilds = new ArrayList();
            var iDefs = ViewDefinition.FindViewDefinitionsByParentType(_adminView.DBManager.Database, ref lstChilds,
              new Guid(objView.Identification), sParentType, ref error);

            var viewDefSub = new ViewDefinition();
            foreach (var obj in lstChilds)
            {
                viewDefSub.Load(_adminView.DBManager.Database, obj, ref error);

                var imageKey = TemplateTreenode.getImageForPluginType(viewDefSub.ChildType, PluginConfigurationTreeView.ImageList);

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
            if (e.Node.Tag.GetType() == typeof(View))
            {
                var objView = (View)e.Node.Tag;
                _viewSelector.Items.Remove(objView.Name);
                objView.Name = e.Label;
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
                    RemoveNodeAndAddBackToList(PluginConfigurationTreeView.SelectedNode);
                }
            }
            else if (e.KeyCode == Keys.F2)
            {
                PluginConfigurationTreeView.SelectedNode.BeginEdit();
            }
        }

        private void RemoveNodeAndAddBackToList(TreeNode trNode)
        {
            foreach (TreeNode trNodeTemp in trNode.Nodes)
            {
                RemoveNodeAndAddBackToList(trNodeTemp);
            }

            var obj = (ObjectPlugin)trNode.Tag;
            var imageKey = TemplateTreenode.getImageForPluginType(obj.PluginType, AvailablePluginsListView.SmallImageList);

            var lstItem = new ListViewItem(trNode.Text);
            lstItem.ImageKey = imageKey;
            lstItem.Tag = trNode.Tag;
            AvailablePluginsListView.Items.Add(lstItem);
            trNode.Remove();
        }
        #endregion


        private void OnSave(object sender, EventArgs e)
        {
            var view = GetSelectedView();
            if (view != null)
                Save(view, PluginConfigurationTreeView.Nodes);
        }

        private void ViewAdminDLG_Shown(object sender, EventArgs e)
        {
            RefreshView();

        }

        private View GetSelectedView()
        {
            var selectedViewName = _viewSelector.SelectedItem.ToString();
            return _views.FirstOrDefault(vw => vw.Name.Equals(selectedViewName));
        }

    }
}
