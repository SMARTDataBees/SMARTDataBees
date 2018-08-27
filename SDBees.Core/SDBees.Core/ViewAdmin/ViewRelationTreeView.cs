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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SDBees.DB;
using SDBees.GuiTools;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateBase;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;
using Attribute = SDBees.DB.Attribute;

namespace SDBees.ViewAdmin
{
    internal class ViewRelationTreeView : DbTreeView
    {
        #region Private Data Members

        private Guid m_ViewId;
        private ViewProperty m_ViewProperties;
        private Hashtable m_HashObjectIdToTreeNode;

        #endregion

        #region Public Properties

        internal Guid ViewId
        {
            get { return m_ViewId; }
            set
            {
                if (m_ViewId != value)
                {
                    m_ViewId = value;

                    if (m_ViewId == Guid.Empty)
                    {
                        m_ViewProperties = null;
                    }
                    else
                    {
                        // update the filter...
                        var table = Table;
                        Error error = null;

                        //Filter only the current view?
                        var attribute = new Attribute(table.Columns["view"], m_ViewId.ToString());
                        Filter = Database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);

                        m_ViewProperties = new ViewProperty();
                        m_ViewProperties.Load(Database, m_ViewId, ref error);

                        Error.Display("Cannot format criteria", error);

                        RefreshView();

                        RaiseViewSwitched(m_ViewProperties.ViewName);
                    }
                }
            }
        }

        internal string ViewName
        {
            get
            {
                var name = "";

                if (m_ViewProperties != null)
                {
                    name = m_ViewProperties.ViewName;
                }

                return name;
            }
        }

        #endregion

        #region Constructor/Destructor

        internal ViewRelationTreeView(SDBeesDBConnection dbManager) : base(dbManager)
        {
            var baseData = new ViewRelation();
            baseData.SetDefaults(dbManager.Database);

            m_HashObjectIdToTreeNode = new Hashtable();

            Table = baseData.Table;
            NodeColumnName = "child";
            ParentNodeColumnName = "parent";
            DisplayColumnName = "child_name";
            ParentNullId = Guid.Empty;
            ImageList = new ImageList();

            m_ViewId = Guid.Empty;
            m_ViewProperties = null;

            // Events...
            KeyDown += ViewRelationTreeView_KeyDown;

            ViewAdmin.Current.ViewRelationCreated += Current_ViewRelationCreated;
            ViewAdmin.Current.ViewRelationModified += Current_ViewRelationModified;
            ViewAdmin.Current.ViewRelationRemoved += Current_ViewRelationRemoved;
            ViewAdmin.Current.OnViewSelectionChanged += Current_ViewSelectionChanged;

            //this.MultiSelect = TreeViewMultiSelect.NoMulti;
        }

        internal ViewRelationTreeView()
        {
            HideSelection = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Deletes all the selected nodes, does nothing when no nodes are selected...
        /// </summary>
        public void DeleteSelectedNodes()
        {
            var selectedTreeNodes = new List<TreeNode>();

            if (SelNodes.Count > 1)
            {
                foreach (MWTreeNodeWrapper tnWrapper in SelNodes.Values)
                {
                    selectedTreeNodes.Add(tnWrapper.Node);
                }
            }
            else if (SelNodes.Count == 1)
            {
                var treeNode = SingleSelectedNode();

                if (treeNode != null)
                {
                    selectedTreeNodes.Add(treeNode);
                }
            }

            var numLastReferences = 0;
            var numNodesWithChildren = 0;
            var treeNodesToDelete = new List<TreeNode>();
            foreach (var treeNode in selectedTreeNodes)
            {
                var isLastReference = false;
                if (CanRemoveViewRelation(treeNode, ref isLastReference))
                {
                    treeNodesToDelete.Add(treeNode);

                    if (isLastReference)
                    {
                        numLastReferences++;
                    }
                }
                else
                {
                    numNodesWithChildren++;
                }
            }

            if (numNodesWithChildren > 0)
            {
                MessageBox.Show(numNodesWithChildren + " objects have childs and can't be deleted!");
                return;
            }

            var answerRemove = MessageBox.Show("Would you really like to remove the objects?", "Remove objects", MessageBoxButtons.YesNo);
            if (answerRemove == DialogResult.No)
                return;

            var deleteUnreferenced = false;

            if (numLastReferences > 0)
            {
                var answerDelete = MessageBox.Show(numLastReferences + " objects no longer referenced, would you like to delete them??", "Remove objects", MessageBoxButtons.YesNo);

                deleteUnreferenced = (answerDelete == DialogResult.Yes);
            }

            TreeNode toBeSelectedNode = null;

            if (selectedTreeNodes.Count == 1)
            {
                toBeSelectedNode = selectedTreeNodes[0].Parent;
            }
            else
            {
                if (0 < Nodes.Count)
                {
                    toBeSelectedNode = Nodes[0];
                }
            }

            // This might be a length process, so entertain the user meanwhile :-)
            var progressTool = new ProgressTool();
            progressTool.StartActiveProcess(true, true);

            progressTool.WriteStatus("Objects will be deleted...");

            progressTool.ProgressBar.Maximum = treeNodesToDelete.Count;
            progressTool.ProgressBar.Value = 0;

            foreach (var treeNode in treeNodesToDelete)
            {
                ViewRelationRemove(treeNode, deleteUnreferenced);

                progressTool.ProgressBar.Value++;
            }

            if (toBeSelectedNode != null)
            {
                SelectNode(toBeSelectedNode, true);
            }

            progressTool.EndActiveProcess();


        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates the TreeNode for a certain node
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected override TreeNode CreateNode(object nodeId, ref Error error)
        {
            TreeNode treeNode = null;

            ViewRelation viewRel = null;
            if (mViewCache.ViewRelation(nodeId, out viewRel, ref error))
            {
                treeNode = CreateNodeForViewRel(viewRel);
            }

            return treeNode;
        }

        /// <summary>
        /// Create a node for the given ViewRelation...
        /// </summary>
        /// <param name="viewRel"></param>
        /// <returns></returns>
        protected TreeNode CreateNodeForViewRel(ViewRelation viewRel)
        {
            TreeNode treeNode = null;

            if (viewRel != null)
            {
                var imageIndex = TemplateTreenode.getImageIndexForPluginType(viewRel.ChildType, ImageList);

                treeNode = new TreeNode(viewRel.ChildName);
                treeNode.ImageIndex = imageIndex;
                treeNode.SelectedImageIndex = imageIndex;

                var tagRel = new TemplateTreenodeTag();
                tagRel.NodeGUID = viewRel.ChildId.ToString();
                tagRel.NodeName = viewRel.ChildName;
                tagRel.NodeTypeOf = viewRel.ChildType;

                treeNode.Tag = tagRel;
            }

            return treeNode;
        }

        /// <summary>
        /// Get the Tag that should be set with a tree node item. This is called in CreateNode and
        /// the default implementations returns the nodeId.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        protected override object CreateTreeNodeTag(object nodeId)
        {
            throw new Exception("CreateTreeNodeTag should not be called...");
        }

        /// <summary>
        /// Notification function that filling will start now...
        /// </summary>
        protected override void FillStarting()
        {
            m_HashObjectIdToTreeNode.Clear();
#if DEBUG
            Debug.Print("FillStarting(" + ViewName + ")");
#endif
            //base.FillStarting();
        }

        protected override void FillEnded()
        {
            base.FillEnded();

            //if (this.Nodes.Count > 0)
            //    this.SelectedNode = this.Nodes[0];
        }

        /// <summary>
        /// Notification function that a tree node has been added
        /// </summary>
        /// <param name="treenode"></param>
        protected override void NodeAdded(TreeNode treenode)
        {
            var tnTag = (TemplateTreenodeTag)treenode.Tag;
            try
            {
                if (tnTag != null)
                {
                    if (!m_HashObjectIdToTreeNode.ContainsKey(tnTag.NodeGUID))
                    {
                        m_HashObjectIdToTreeNode.Add(tnTag.NodeGUID, treenode);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Called during DragOver Event for derived classes to control dragging depending on special cases. The default
        /// implementation takes care of a couple of things, so this should be called first. Then this method will check
        /// specific behavior for this class.
        /// </summary>
        /// <param name="sender">sender object originally passed by .NET framework</param>
        /// <param name="e">Event args originally passed by .NET framework</param>
        /// <param name="treeNodeToDrop">Tree node currently dragged over</param>
        /// <param name="dragData">Drag information containing information about the objects to be dragged</param>
        /// <returns></returns>
        protected override DragDropEffects GetDragOverEffect(object sender, DragEventArgs e, TreeNode treeNodeToDrop, DbDragData dragData)
        {
            var result = base.GetDragOverEffect(sender, e, treeNodeToDrop, dragData);

            if (m_ViewProperties == null)
            {
                result = DragDropEffects.None;
            }

            if (result != DragDropEffects.None)
            {
                // The base class would allow this operation, now add our limitations...

                // 1. Check that this matches the view specification
                var tnTag = (TemplateTreenodeTag)treeNodeToDrop.Tag;

                ArrayList viewDefs = null;
                Error error = null;
                m_ViewProperties.GetChildren(tnTag.NodeTypeOf, ref viewDefs, ref error);

                var allowedChildTypes = new Hashtable();
                foreach (ViewDefinition viewDef in viewDefs)
                {
                    allowedChildTypes.Add(viewDef.ChildType, null);
                }

                foreach (var dragItem in dragData.DragItems)
                {
                    if (typeof(TreeNode).IsInstanceOfType(dragItem))
                    {
                        var treeNode = (TreeNode)dragItem;
                        var tnDragTag = (TemplateTreenodeTag)treeNode.Tag;

                        if (!allowedChildTypes.ContainsKey(tnDragTag.NodeTypeOf))
                        {
                            result = DragDropEffects.None;
                            break;
                        }
                    }
                }

                // 2. If the dragging is between different tree views, but these tree views show the same
                //    view, then the operation should only be a move...
                var senderControl = dragData.SenderControl();
                if ((senderControl != this) && (result == DragDropEffects.Copy))
                {
                    if (typeof(ViewRelationTreeView).IsInstanceOfType(senderControl))
                    {
                        var viewRelTreeView = (ViewRelationTreeView)senderControl;

                        if ((viewRelTreeView.ViewId == ViewId) && ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move))
                        {
                            result = DragDropEffects.Move;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is called in the popup event of the context menu and allows dynamic creation of menu items
        /// The default implementation creates the menu items defined in the menu definition, override to
        /// support special behavior.
        /// </summary>
        protected override void CreateContextMenu()
        {
            // add the default context menus...
            base.CreateContextMenu();

            Error error = null;
            var multipleSelection = (SelNodes.Count > 1);
            var singleSelection = (SelNodes.Count == 1);
            var noSelection = (SelNodes.Count == 0);
            var curViewName = ViewName;

            AddFunctionalPluginMenuItems();

            if (singleSelection)
            {
                var tn = SingleSelectedNode();
                var tagRel = (TemplateTreenodeTag)tn.Tag;

                if (m_ViewProperties != null)
                {
                    // Create menu entries for creating children...
                    AddCreateChildMenuItems(tagRel.NodeTypeOf, ref error);

                    // Create Menu entries for this node
                    var plugin = TemplateTreenode.GetPluginForType(tagRel.NodeTypeOf);
                    if (plugin != null)
                    {
                        AddDeleteMenuItem(plugin);
                        AddEditSchemaMenuItem(plugin);
                    }
                }
            }
            else if (noSelection)
            {
                AddCreateChildMenuItems(ViewRelation.m_StartNodeValue, ref error);
            }
            else if (multipleSelection)
            {
                //AddDeleteMenuItem(null);
            }

            ContextMenu.MenuItems.Add(new MenuItem("-"));

            var menuViewRefresh = new MenuItem("Update view");
            // Add a separator
            // menuViewRefresh.Break = true;
            menuViewRefresh.Enabled = (curViewName != "");
            menuViewRefresh.Click += menuViewRefresh_Click;

            ContextMenu.MenuItems.Add(menuViewRefresh);

            var menuViewFlyout = new MenuItem("Change view");
            // Add a separator
            // menuViewFlyout.Break = true;

            ContextMenu.MenuItems.Add(menuViewFlyout);

            ArrayList viewNames = null;
            var numViews = ViewProperty.GetAllViewNames(Database, ref viewNames, ref error);

            if (numViews > 0)
            {
                foreach (string viewName in viewNames)
                {
                    var menuView = new MenuItem(viewName);
                    menuView.Enabled = (curViewName != viewName);
                    menuView.Checked = (curViewName == viewName);
                    menuView.Click += menuView_Click;

                    menuViewFlyout.MenuItems.Add(menuView);
                }
            }
            else
            {
                var menuView = new MenuItem("No views definied");
                menuView.Enabled = false;

                menuViewFlyout.MenuItems.Add(menuView);
            }
        }

        private void AddEditSchemaMenuItem(TemplateTreenode plugin)
        {
            if (plugin.AllowEditingOfSchema())
            {
                var menuEditSchema = new MenuItem(plugin.EditSchemaMenuItem);
                menuEditSchema.Click += menuEditSchema_Click;
                ContextMenu.MenuItems.Add(menuEditSchema);
            }
        }

        private void AddDeleteMenuItem(TemplateTreenode plugin)
        {
            MenuItem menuDeleteNode = null;

            if (plugin == null)
                menuDeleteNode = new MenuItem("Delete Object");
            else
                menuDeleteNode = new MenuItem(plugin.DeleteMenuItem);

            menuDeleteNode.Click += menuDeleteNode_Click;
            ContextMenu.MenuItems.Add(menuDeleteNode);

            menuDeleteNode.Click += menuDeleteNode_Click;
            ContextMenu.MenuItems.Add(menuDeleteNode);
        }

        protected void AddFunctionalPluginMenuItems()
        {
            //Funktionale MenuItems hinzufügen
            if (SelNodes.Count > 0)
            {
                var tagList = new List<TemplateTreenodeTag>();
                foreach (MWTreeNodeWrapper tnWrapper in SelNodes.Values)
                {
                    tagList.Add((TemplateTreenodeTag) tnWrapper.Node.Tag);
                }

                var helperList = TemplateTreenodeHelper.GetAllHelperPlugins();
                if (helperList.Count > 0)
                {
                    var createSeperator = false;
                    foreach (var helper in helperList)
                    {
                        List<MenuItem> menuItemsToAdd = null;
                        if (helper.GetPopupMenuItems(tagList, out menuItemsToAdd))
                        {
                            foreach (var menuItem in menuItemsToAdd)
                            {
                                ContextMenu.MenuItems.Add(menuItem);
                            }
                            createSeperator = true;
                        }
                    }

                    if(createSeperator)
                        ContextMenu.MenuItems.Add(new MenuItem("-"));                    
                }
            }
        }

        protected void AddCreateChildMenuItems(string parentType, ref Error error)
        {
            ArrayList viewDefs = null;
            if ((m_ViewProperties != null) && (m_ViewProperties.GetChildren(parentType, ref viewDefs, ref error) > 0))
            {
                foreach (ViewDefinition viewDef in viewDefs)
                {
                    var pluginChild = TemplateTreenode.GetPluginForType(viewDef.ChildType);
                    if (pluginChild != null)
                    {
                        var menuCreateChild = new MenuItem(pluginChild.CreateMenuItem);
                        menuCreateChild.Click += menuCreateChild_Click;
                        menuCreateChild.Tag = viewDef.ChildType;

                        ContextMenu.MenuItems.Add(menuCreateChild);
                    }
                }
            }

        }

        /// <summary>
        /// Adds a new viewrelation for a childnode
        /// </summary>
        /// <param name="theTag">Tag for the child node</param>
        /// <param name="treeNode">the parent treenode</param>
        protected void ViewRelationAdd(TemplateTreenodeTag theTag, TreeNode treeNode)
        {
            try
            {
                //Die Relation einfügen
                Error error = null;
                var viewrel = new ViewRelation();
                viewrel.SetDefaults(Database);
                viewrel.ChildId = new Guid(theTag.NodeGUID);
                //viewrel.
                viewrel.ChildType = theTag.NodeTypeOf;
                viewrel.ChildName = theTag.NodeName;
                viewrel.ViewId = m_ViewId;

                if (ViewRelation.ChildReferencedByView(Database, m_ViewId, viewrel.ChildId, ref error))
                {
                    MessageBox.Show("Object already referenced in view! No second relation created!");
                    return;
                }

                // Die Knotenliste an die das neue Objekt angehängt werden soll muss noch bestimmt werden
                TreeNodeCollection nodes = null;

                if (treeNode == null) // Einen obersten Knoten einfügen
                {
                    viewrel.ParentType = null;
                    nodes = Nodes;

                    //Parentid ist automatisch 000
                }
                else //Es gibt bereits Knoten im Treeview
                {
                    //Den Tag des Parent besorgen. Parent ist der Selektierte Knoten
                    var tagSelected = (TemplateTreenodeTag)treeNode.Tag;
                    if (tagSelected.NodeTypeOf == viewrel.ChildType) //Es wird der gleiche Typ eingefügt, also auf dem gleichen Niveau
                    {
                        var parentTreeNode = treeNode.Parent;
                        if (parentTreeNode == null)
                        {
                            // oberster Knoten...
                            nodes = Nodes;
                            viewrel.ParentType = null;
                        }
                        else
                        {
                            // nachbar Knoten...
                            nodes = parentTreeNode.Nodes;

                            // ParentType bestimmen...
                            var tagParent = (TemplateTreenodeTag)parentTreeNode.Tag;
                            viewrel.ParentId = new Guid(tagParent.NodeGUID);
                            viewrel.ParentType = tagParent.NodeTypeOf;
                        }
                    }
                    else if (ViewDefinition.ViewDefinitionExists(Database, m_ViewId, tagSelected.NodeTypeOf, viewrel.ChildType, ref error))
                    {
                        //Es wird ein Child eingefügt
                        nodes = treeNode.Nodes;

                        viewrel.ParentId = new Guid(tagSelected.NodeGUID);
                        viewrel.ParentType = tagSelected.NodeTypeOf;
                    }
                }

                if (nodes != null)
                {
                    var testParentType = viewrel.ParentType;
                    if (viewrel.ParentType == null)
                        testParentType = ViewRelation.m_StartNodeValue;

                    if (!ViewDefinition.ViewDefinitionExists(Database, m_ViewId, testParentType, viewrel.ChildType, ref error))
                    {
                        MessageBox.Show("Can't insert object.");
                        return;
                    }

                    viewrel.Save(ref error);
                }
                else
                {
                    MessageBox.Show("No suitable element for insertion found!");
                }

                Error.Display("Error on creating relation", error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        protected bool CanRemoveViewRelation(TreeNode treeNode, ref bool isLastReference)
        {
            var result = false;

            if (treeNode.Nodes.Count == 0)
            {
                var selectedTag = (TemplateTreenodeTag)treeNode.Tag;

                Error error = null;
                var objectIds = new ArrayList();
                isLastReference = (ViewRelation.FindViewRelationByChildId(Database, new Guid(selectedTag.NodeGUID), ref objectIds, ref error) <= 1);

                result = true;
            }

            return result;
        }

        protected void ViewRelationRemove(TreeNode treeNode, bool deleteUnreferenced)
        {
            var objectName = treeNode.Text;
            var selectedTag = (TemplateTreenodeTag)treeNode.Tag;
            var database = Database;

            if (treeNode.Nodes.Count > 0)
            {
                MessageBox.Show("The object '" + objectName + "' has childs and can't be deleted!");
            }
            else
            {
                var viewRel = GetViewRelationFromNode(treeNode);

                if (viewRel != null)
                {
                    Error error = null;
                    viewRel.Delete(deleteUnreferenced, ref error);
                }
            }
        }

        protected ViewRelation GetViewRelationFromNode(TreeNode currentNode)
        {
            if (currentNode == null)
                return null;

            ViewRelation result = null;

            try
            {
                var objectName = currentNode.Text;
                var selectedTag = (TemplateTreenodeTag)currentNode.Tag;

                var parentId = new Guid();
                var childId = new Guid(selectedTag.NodeGUID);

                var parentNode = currentNode.Parent;
                if (parentNode != null)
                {
                    var parentTag = (TemplateTreenodeTag)parentNode.Tag;
                    parentId = new Guid(parentTag.NodeGUID);
                }
                var objectIds = new ArrayList();
                Error error = null;
                ViewRelation.FindViewRelationByParentIdAndChildId(Database, parentId, childId, ref objectIds, ref error);

                if (error == null)
                {
                    if (objectIds.Count > 1)
                    {
                        MessageBox.Show("More than one Viewrelation found!");
                    }
                    else if(objectIds.Count == 0)
                    {
                        MessageBox.Show("No viewrelation found, very strange!");
                    }
                    else
                    {
                        result = new ViewRelation();
                        result.Load(Database, objectIds[0], ref error);
                    }
                }

                if (error != null)
                {
                    Error.Display("ViewRelation not found", error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                result = null;
            }

            return result;
        }

        internal TemplateTreenodeTag m_selTag;

        protected void menuCreateChild_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;

            var plugin = TemplateTreenode.GetPluginForType((string)menuItem.Tag);
            if (plugin != null)
            {
                Error error = null;
                var baseData = plugin.CreateDataObject();
                baseData.SetDefaults(Database);
                if (String.IsNullOrEmpty(baseData.Name))
                {
                    baseData.Name = "Unnamed";
                }

                var currentTreeNode = SingleSelectedNode();
                if (plugin.IsNewChildInstanceAllowed(currentTreeNode, menuItem.Tag.ToString()))
                {
                    var location = MousePosition;
                    string name;
                    var proceed = false;

                    do
                    {
                        var dlgres = DialogResult.Abort;

                        name = InputBox.Show("Naming", "Name for the new object", baseData.Name, location.X, location.Y, ref dlgres);

                        if (dlgres == DialogResult.Cancel)
                        {
                            proceed = false;
                            break;
                        }

                        if (!String.IsNullOrEmpty(name))
                        {
                            baseData.Name = name;

                            if (baseData.CheckForUniqueName())
                            {
                                if (baseData.IsNameUnique(baseData.GetTableName))
                                    proceed = true;
                                else
                                    proceed = false;
                            }
                            else
                                proceed = true;
                        }

                    } while (!proceed);

                    if (proceed)
                    {
                        baseData.Save(ref error);

                        var newTag = plugin.MyTag();
                        newTag.NodeGUID = baseData.Id.ToString();
                        newTag.NodeName = baseData.Name;

                        ViewRelationAdd(newTag, currentTreeNode);

                        plugin.DoCreateSubTasks(newTag, currentTreeNode);

                        m_selTag = newTag;

                        RefreshView();

                        try
                        {
                            if (m_lastNode != null)
                            {
                                SelectNode(m_lastNode, true);

                                //this.SelectedNode.Expand();
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        public override void RefreshView()
        {
            base.RefreshView();

            ReselectLastNode();
            //this.ExpandAll();
            if(Nodes != null)
            {
                foreach (TreeNode item in Nodes)
                {
                    item.Expand();
                }
            }
        }

        private void ReselectLastNode()
        {
            if(m_selTag != null)
            {
                foreach (TreeNode item in Nodes)
                {
                    if (CheckTag(item))
                        break;
                    ParseSubNodes(item);
                }
            }
        }

        private void ParseSubNodes(TreeNode item)
        {
            foreach (TreeNode subitems in item.Nodes)
            {
                if (CheckTag(subitems))
                    break;
                ParseSubNodes(subitems);
            }
        }

        TreeNode m_lastNode;

        private bool CheckTag(TreeNode item)
        {
            var curTag = item.Tag as TemplateTreenodeTag;
            if(curTag != null && curTag.NodeGUID == m_selTag.NodeGUID)
            {
                m_lastNode = item;
                //item.Expand();
                return true;
            }
            return false;
        }

        protected void menuEditSchema_Click(object sender, EventArgs e)
        {
            var currentTreeNode = SingleSelectedNode();

            if (currentTreeNode != null)
            {
                var tagRel = (TemplateTreenodeTag)currentTreeNode.Tag;
                var plugin = TemplateTreenode.GetPluginForType(tagRel.NodeTypeOf);

                if (plugin != null)
                {
                    plugin.EditSchema();
                }
            }
        }

        protected void menuDeleteNode_Click(object sender, EventArgs e)
        {
            DeleteSelectedNodes();
        }

        protected void menuViewRefresh_Click(object sender, EventArgs e)
        {
            RefreshView();
        }

        protected void menuView_Click(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var viewName = menuItem.Text;
            Error error = null;
            var viewId = ViewProperty.GetViewIdFromName(Database, viewName, ref error);

            if ((error == null) && (viewId != null))
            {
                ViewId = viewId;
            }
        }

        protected void ViewRelationTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.Delete)
            {
                DeleteSelectedNodes();
            }
        }

        private void Current_ViewRelationCreated(object sender, ViewAdmin.ViewRelationEventArgs args)
        {
            TreeNode parentTreeNode = null;
            TreeNodeCollection nodes = null;
            if (args.ViewRelation.ParentId != Guid.Empty)
            {
                var key = args.ViewRelation.ParentId.ToString();

                if (m_HashObjectIdToTreeNode.ContainsKey(key))
                {
                    parentTreeNode = (TreeNode)m_HashObjectIdToTreeNode[key];

                    if (parentTreeNode != null)
                    {
                        nodes = parentTreeNode.Nodes;
                    }
                }
            }
            else
            {
                nodes = Nodes;
            }

            if (nodes != null)
            {
                var treeNode = CreateNodeForViewRel(args.ViewRelation);

                nodes.Add(treeNode);

                NodeAdded(treeNode);

                // Falls dieser als Kind eingefügt wurde, expandieren, damit sichtbar...
                if (parentTreeNode != null)
                {
                    if (!parentTreeNode.IsExpanded)
                    {
                        parentTreeNode.Expand();
                    }
                }
            }
        }

        private void Current_ViewRelationModified(object sender, ViewAdmin.ViewRelationEventArgs args)
        {
            var key = args.ViewRelation.ChildId.ToString();

            if (m_HashObjectIdToTreeNode.ContainsKey(key))
            {
                var treeNode = (TreeNode)m_HashObjectIdToTreeNode[key];

                if (treeNode != null)
                {
                    if (args.NameChanged)
                    {
                        try
                        {
                            treeNode.Text = args.ViewRelation.ChildName;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
        }

        private void Current_ViewRelationRemoved(object sender, ViewAdmin.ViewRelationEventArgs args)
        {
            var key = args.ViewRelation.ChildId.ToString();

            if (m_HashObjectIdToTreeNode.ContainsKey(key))
            {
                var treeNode = (TreeNode)m_HashObjectIdToTreeNode[key];

                if (treeNode != null)
                {
                    ClearSelNodes();

                    treeNode.Remove();
                    m_HashObjectIdToTreeNode.Remove(key);
                }
            }
        }

        private void Current_ViewSelectionChanged(object myObject, ViewSelectArgs myArgs)
        {
            //Guid value = myArgs.ViewGuid;

            //m_ViewId = value;

            //// update the filter...
            //Table table = this.Table;
            //Error error = null;

            ////Filter only the current view?
            //SDBees.DB.Attribute attribute = new SDBees.DB.Attribute(table.Columns["view"], m_ViewId.ToString());
            //this.Filter = Database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);

            //m_ViewProperties = new ViewProperty();
            //m_ViewProperties.Load(Database, m_ViewId, ref error);

            //Error.Display("Cannot format criteria", error);

            //RefreshView();

            //RaiseViewSwitched(m_ViewProperties.ViewName);
        }

        #endregion

        #region Events

        public class NotificationEventArgs
        {
            public string ViewName;

            public NotificationEventArgs(string viewName)
            {
                ViewName = viewName;
            }
        }
        public delegate void NotificationHandler(object sender, NotificationEventArgs args);

        public event NotificationHandler ViewSwitched;

        internal void RaiseViewSwitched(string viewName)
        {
            if (ViewSwitched != null)
            {
                var args = new NotificationEventArgs(viewName);
                ViewSwitched.Invoke(this, args);
            }
        }

        #endregion
    }
}
