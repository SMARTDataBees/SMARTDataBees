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
using SDBees.Core.Admin;
using SDBees.Core.Utils;
using SDBees.GuiTools;

namespace SDBees.DB
{
    /// <summary>
    /// 
    /// </summary>
    public class DbTreeView : MWTreeView
    {
        #region Private Data Members

        private Table mTable;
        private SDBeesDBConnection m_dbManager;
        private Database m_database;
        private string mNodeColumnName;
        private string mParentNodeColumnName;
        private string mDisplayColumnName;
        private string mFilter;
        private object mParentNullId;
        private List<MenuDefinition> mMenuDefinitions;
        protected ViewCache.ViewCacheImplementation mViewCache { get { return ViewCache.Instance; } }

        #endregion

        #region Public Properties

        /// <summary>
        /// The Table this tree is stored in
        /// </summary>
        public Table Table
        {
            get { return mTable; }
            set { mTable = value; }
        }

        /// <summary>
        /// The Database to display the tree from
        /// </summary>
        public Database Database
        {
            get
            {
                return (m_dbManager != null) ? m_dbManager.Database : m_database;
            }
        }

        /// <summary>
        /// The column name of the objects in the tree
        /// </summary>
        public string NodeColumnName
        {
            get { return mNodeColumnName; }
            set { mNodeColumnName = value; }
        }

        /// <summary>
        /// The column name to find the parent relationship of the tree in
        /// This should be the same type as the NodeColumnName
        /// </summary>
        public string ParentNodeColumnName
        {
            get { return mParentNodeColumnName; }
            set { mParentNodeColumnName = value; }
        }

        /// <summary>
        /// The name of the column of which the value should be displayed as the tree node label.
        /// If the method CreateNode is not overridden, the this should be set.
        /// </summary>
        public string DisplayColumnName
        {
            get { return mDisplayColumnName; }
            set { mDisplayColumnName = value; }
        }

        /// <summary>
        /// An SQL formatted filter to generally filter the data for the tree
        /// </summary>
        public string Filter
        {
            get { return mFilter; }
            set { mFilter = value; }
        }

        /// <summary>
        /// This is the value in the parent column for root nodes
        /// </summary>
        public object ParentNullId
        {
            get { return mParentNullId; }
            set { mParentNullId = value; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public DbTreeView(SDBeesDBConnection dbManager) : this(dbManager, null)
        {
            TreeViewNodeSorter = new TreeNodeComparer();
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public DbTreeView(Database database) : this(null, database)
        {
            TreeViewNodeSorter = new TreeNodeComparer();
        }

        public DbTreeView() : this(null, null)
        {
            TreeViewNodeSorter = new TreeNodeComparer();
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private DbTreeView(SDBeesDBConnection dbManager, Database database)
        {
            if (dbManager != null)
                m_dbManager = dbManager;
            else
                m_dbManager = SDBeesDBConnection.Current;

            if (database != null)
                m_database = database;
            else
                m_database = SDBeesDBConnection.Current.Database;

            InitInternal();
        }

        private void InitInternal()
        {
            mTable = null;
            mNodeColumnName = "";
            mParentNodeColumnName = "";
            mDisplayColumnName = "";
            mFilter = "";
            mParentNullId = null;
            mMenuDefinitions = new List<MenuDefinition>();

            // typical settings of the control
            HideSelection = false;
            FullRowSelect = false;

            // Create a context menu...
            ContextMenu = new ContextMenu();
            ContextMenu.Popup += ContextMenu_Popup;

            // Drag & Drop
            AllowDrop = false;
            ItemDrag += DbTreeView_ItemDrag;
            DragOver += DbTreeView_DragOver;
            DragDrop += DbTreeView_DragDrop;
            DragEnter += DbTreeView_DragEnter;

            // Hotkeys
            KeyDown += DbTreeView_KeyDown;
        }


        private void DbTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                // We must (single) select the node beneath the mouse position if it is not in a multi-select
                FixSelectionAtMousePosition();

                // Create the drag information that will be pass
                var dragData = new DbDragData();
                if (SelNodes.Count > 0)
                {
                    foreach (MWTreeNodeWrapper tnWrapper in SelNodes.Values)
                    {
                        dragData.DragItems.Add(tnWrapper.Node);
                    }
                }
                else if (SelectedNode != null)
                {
                    dragData.DragItems.Add(SelectedNode);
                }

                // Move the dragged node when the left mouse button is used.
                if ((e.Button == MouseButtons.Left) || (e.Button == MouseButtons.Right))
                {
                    DoDragDrop(dragData, DragDropEffects.Move | DragDropEffects.Copy);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DbTreeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                e.Effect = DragDropEffects.None;

                // Ensure that the list item index is contained in the data.
                if (e.Data.GetDataPresent(typeof(DbDragData)))
                {
                    // Retrieve the client coordinates of the mouse position.
                    var targetPoint = PointToClient(new Point(e.X, e.Y));

                    // Select the node at the mouse position.
                    var tn = GetNodeAt(targetPoint);
                    SelectSingleNode(tn);

                    if (tn != null)
                    {
                        var dragData = (DbDragData)e.Data.GetData(typeof(DbDragData));

                        e.Effect = GetDragOverEffect(sender, e, tn, dragData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DbTreeView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.AllowedEffect;
        }

        private void DbTreeView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                // Ensure that the list item index is contained in the data.
                if (e.Data.GetDataPresent(typeof(DbDragData)))
                {

                    var dragData = (DbDragData)e.Data.GetData(typeof(DbDragData));

                    // Perform drag-and-drop, depending upon the effect.
                    if (e.Effect == DragDropEffects.Copy ||
                        e.Effect == DragDropEffects.Move)
                    {
                        var message = "";

                        foreach (var dragItem in dragData.DragItems)
                        {
                            if (typeof(TreeNode).IsInstanceOfType(dragItem))
                            {
                                var treeNode = (TreeNode)dragItem;

                                if (message != "")
                                {
                                    message += ", ";
                                }
                                message += treeNode.Text;
                            }
                        }

                        MessageBox.Show("DragDrop " + dragData.DragItems.Count + " Elements (" + message + ") TBD...");

                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        void DbTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (char)Keys.F5)
            {
                RefreshView();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return the single selected TreeNode, no matter if this is set to single or multiple selection.
        /// null if nothing or multiple items are selected.
        /// </summary>
        /// <returns></returns>
        public TreeNode SingleSelectedNode()
        {
            if (SelNodes.Count == 1)
            {
                foreach (MWTreeNodeWrapper tnWrapper in SelNodes.Values)
                {
                    return tnWrapper.Node;
                }
            }

            if (SelNodes.Count > 1)
                return null;

            if (SelectedNode != null)
                return SelectedNode;

            return null;
        }

        /// <summary>
        /// Select a single node, clear all others if this allows multiple selection
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns>true if successful</returns>
        public bool SelectSingleNode(TreeNode treeNode)
        {
            if (treeNode == SingleSelectedNode())
            {
                // This is already selected, nothing to do...
                return true;
            }
            ClearSelNodes();
            return SelectNode(treeNode, true);
        }

        private delegate void FillHelper(ref Error error);

        /// <summary>
        /// Fill the tree view from the database
        /// </summary>
        public virtual void Fill(ref Error error)
        {
            if (InvokeRequired)
            {
                FillHelper h = Fill;
                BeginInvoke(h);
            }
            else
            {
                FillIntern(ref error);
            }
        }


        /// <summary>
        /// Fill the tree view from the database
        /// </summary>
        private void FillIntern(ref Error error)
        {
            try
            {
                ClearSelNodes();

                if(Nodes != null)
                    Nodes.Clear();

                if ((mTable != null) && (Database != null) && (mNodeColumnName != "") && (mParentNodeColumnName != ""))
                {
                    Database.Open(true, ref error);

                    try
                    {
                        ViewCache.Enable();

                        BeginUpdate();

                        FillStarting();

                        FillChildren(Nodes, mParentNullId, null, ref error);
                    }
                    finally
                    {
                        ViewCache.Disable();
                    }

                    FillEnded();

                    //ExpandAll();

                    //foreach (TreeNode item in this.Nodes)
                    //{
                    //    item.Expand();
                    //}

                    EndUpdate();

                    Database.Close(ref error);

                    Update();
                }
                else
                {
                    error = new Error("Object not setup correctly", 9999, GetType(), error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Refresh the current view from the database
        /// </summary>
        public virtual void RefreshView()
        {
            Error error = null;
            Fill(ref error);

            Error.Display("Cannot fill view tree!", error);
        }


        /// <summary>
        /// This resets the list of menu item definitions set so far
        /// </summary>
        public virtual void ClearContextMenuDefinition()
        {
            mMenuDefinitions.Clear();
        }

        /// <summary>
        /// Adds a MenuItem to the Context menu
        /// </summary>
        /// <param name="menuItem">MenuItem to add</param>
        /// <param name="enableForMultiSelect">enable this menu item if multiple items are selected</param>
        /// <param name="enableForSingleSelect">enable this menu item if only one item is selected</param>
        /// <param name="weight">Gives a weight for the menu item for the display order, higher weights come after lower weights</param>
        public virtual void AddContextMenuItem(MenuItem menuItem, bool enableForMultiSelect, bool enableForSingleSelect, int weight)
        {
            // TBD: consider the weight...
            var menuDefinition = new MenuDefinition(menuItem, enableForMultiSelect, enableForSingleSelect, weight);
            mMenuDefinitions.Add(menuDefinition);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Fill the children into a node collection
        /// </summary>
        /// <param name="nodes">Node collection to add the child nodes to</param>
        /// <param name="parentId">Id of the parent</param>
        /// <param name="error">ref to the error handler</param>
        protected virtual void FillChildren(TreeNodeCollection nodes, object parentId, string parentType, ref Error error)
        {
            var m_Sorting = "";

            if (Database.UseGlobalCaching == false)
            {
                //m_Sorting = String.Format(" ORDER BY {0} ASC", ViewAdmin.ViewRelation.m_ChildNameColumnName);
            }

            var criteriaViewDef = "";

            try
            {
                var criteriaLstViewDefs = new ArrayList();

                if (string.IsNullOrEmpty(parentType))
                {
                    criteriaLstViewDefs.Add(string.Format("parent_type='{0}'", ViewRelation.m_StartNodeValue));
                    if (string.IsNullOrEmpty(mFilter))
                    {
                        criteriaViewDef = criteriaLstViewDefs[0].ToString();
                    }
                    else
                    {
                        criteriaLstViewDefs.Add(mFilter);
                        criteriaViewDef = Database.FormatCriteria(criteriaLstViewDefs, DbBooleanOperator.eAnd, ref error);
                    }
                }
                else
                {
                    criteriaLstViewDefs.Add(string.Format("parent_type='{0}'", parentType));
                    if (string.IsNullOrEmpty(mFilter))
                    {
                        criteriaViewDef = criteriaLstViewDefs[0].ToString();
                    }
                    else
                    {
                        criteriaLstViewDefs.Add(mFilter);
                        criteriaViewDef = Database.FormatCriteria(criteriaLstViewDefs, DbBooleanOperator.eAnd, ref error);
                    }
                }

                //Load the viewDef
                var lstViewDefs = mViewCache.ViewDefinitions(criteriaViewDef, ref error);
                var numRelDefs = lstViewDefs.Count;

                for (var i = 0; i < numRelDefs; i++)
                {
                    ViewDefinition db = null;
                    if (mViewCache.ViewDefinition(lstViewDefs[i], out db, ref error))
                    {
                        var column = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(mTable));
                        var attParent = new Attribute(column, parentId.ToString());
                        var criteria = Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);
                        if (Database.UseGlobalCaching == false)
                        {
                            criteria += m_Sorting;
                        }

                        var objectIds = mViewCache.ViewRelations(criteria, ref error);
                        var numEntries = objectIds.Count;

                        for (var index = 0; index < numEntries; index++)
                        {
                            ViewRelation viewRel = null;
                            if (mViewCache.ViewRelation(objectIds[index], out viewRel, ref error))
                            {
                                if (viewRel.ChildType == db.ChildType)
                                {
                                    var treeNode = CreateNode(objectIds[index], ref error);
                                    if (treeNode != null)
                                    {
                                        //ArrayList objectIdValidRels = null;
                                        //int numValidRelations = SDBees.ViewAdmin.ViewDefinitions.FindViewDefinitionsByParentType(mDatabase, ref objectIdValidRels, new Guid(mFilter), , ref error);
                                        nodes.Add(treeNode);

                                        NodeAdded(treeNode);

                                        // Recurse for this node
                                        var nextParentId = objectIds[index];
                                        if (mNodeColumnName != mTable.PrimaryKey)
                                        {
                                            var primaryKeyColumn = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(mTable.PrimaryKey));
                                            var attribute = new Attribute(primaryKeyColumn, nextParentId.ToString());
                                            var idCriteria = Database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
                                            nextParentId = mViewCache.Parent(mTable, mNodeColumnName, idCriteria, ref error);
                                        }

                                        if (error == null)
                                        {
                                            FillChildren(treeNode.Nodes, nextParentId, db.ChildType, ref error);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + criteriaViewDef);
            }
        }
        //protected virtual void FillChildren(TreeNodeCollection nodes, object parentId, string parentType, ref Error error)
        //{
        //    string criteriaViewDef = "";
        //    ArrayList criteriaLstViewDefs = new ArrayList();

        //    if (String.IsNullOrEmpty(parentType))
        //    {
        //        criteriaLstViewDefs.Add(String.Format("parent_type='{0}'", "start"));
        //        criteriaLstViewDefs.Add(mFilter);
        //        criteriaViewDef = mDatabase.FormatCriteria(criteriaLstViewDefs, DbBooleanOperator.eAnd, ref error);
        //    }
        //    else
        //    {
        //        criteriaLstViewDefs.Add(String.Format("parent_type='{0}'", parentType));
        //        criteriaLstViewDefs.Add(mFilter);
        //        criteriaViewDef = mDatabase.FormatCriteria(criteriaLstViewDefs, DbBooleanOperator.eAnd, ref error);
        //    }

        //    //Load the viewDef
        //    ArrayList lstViewDefs = mViewCache.ViewDefinitions(criteriaViewDef, ref error);
        //    int numRelDefs = lstViewDefs.Count;

        //    for (int i = 0; i < numRelDefs; i++)
        //    {
        //        ViewAdmin.ViewDefinition db = null;
        //        if (mViewCache.ViewDefinition(lstViewDefs[i], out db, ref error))
        //        {
        //            SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(mTable.Columns[mParentNodeColumnName], parentId.ToString());
        //            string criteria = mDatabase.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);

        //            if (mFilter != "")
        //            {
        //                //criteria = mDatabase.FormatCriteria(mFilter, criteria, DbBooleanOperator.eAnd, ref error);
        //            }

        //            ArrayList objectIds = mViewCache.ViewRelations(criteria, ref error);
        //            int numEntries = objectIds.Count;

        //            for (int index = 0; index < numEntries; index++)
        //            {
        //                ViewAdmin.ViewRelation viewRel = null;
        //                if (mViewCache.ViewRelation(objectIds[index], out viewRel, ref error))
        //                {
        //                    if (viewRel.ChildType == db.ChildType)
        //                    {
        //                        int maxcount = 1;

        //                        if (String.IsNullOrEmpty(parentType))
        //                        {
        //                            maxcount = 10;
        //                        }

        //                        for (int count = 0; count < maxcount; count++)
        //                        {
        //                            TreeNode treeNode = CreateNode(objectIds[index], ref error);
        //                            if (treeNode != null)
        //                            {
        //                                //ArrayList objectIdValidRels = null;
        //                                //int numValidRelations = SDBees.ViewAdmin.ViewDefinitions.FindViewDefinitionsByParentType(mDatabase, ref objectIdValidRels, new Guid(mFilter), , ref error);
        //                                nodes.Add(treeNode);

        //                                NodeAdded(treeNode);

        //                                //// Recurse for this node
        //                                object nextParentId = objectIds[index];
        //                                if (mNodeColumnName != mTable.PrimaryKey)
        //                                {
        //                                    SDBees.DB.Attribute attribute = new SDBees.DB.Attribute(mTable.Columns[mTable.PrimaryKey], nextParentId.ToString());
        //                                    string idCriteria = mDatabase.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
        //                                    nextParentId = mViewCache.Parent(idCriteria, ref error);
        //                                }

        //                                mStopwatch1.Stop();

        //                                if (error == null)
        //                                {
        //                                    FillChildren(treeNode.Nodes, nextParentId, db.ChildType, ref error);
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        /// <summary>
        /// Creates the TreeNode for a certain node
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected virtual TreeNode CreateNode (object nodeId, ref Error error)
        {
            TreeNode treeNode = null;
            var column = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(mTable.PrimaryKey));
            var attribute = new Attribute(column, nodeId.ToString());
            var idCriteria = Database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
            var nodeLabel = Database.SelectSingle(mTable.Name, mDisplayColumnName, idCriteria, ref error);

            if (nodeLabel != null)
            {
                treeNode = new TreeNode(nodeLabel.ToString());
                treeNode.Tag = CreateTreeNodeTag(nodeId);
            }

            return treeNode;
        }

        /// <summary>
        /// Get the Tag that should be set with a tree node item. This is called in CreateNode and
        /// the default implementations returns the nodeId.
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        protected virtual object CreateTreeNodeTag(object nodeId)
        {
            return new DbTreeNodeTag(nodeId);
        }

        /// <summary>
        /// Notification function that filling will start now...
        /// </summary>
        protected virtual void FillStarting()
        {
            // empty...
        }

        /// <summary>
        /// Notification function that filling will start now...
        /// </summary>
        protected virtual void FillEnded()
        {
            // empty...
        }

        /// <summary>
        /// Notification function that a tree node has been added
        /// </summary>
        /// <param name="treenode"></param>
        protected virtual void NodeAdded(TreeNode treenode)
        {
            // empty...
        }

        /// <summary>
        /// Called during DragOver Event for derived classes to control dragging depending on special cases. The default
        /// implementation will move within the same tree view, and copy between different tree views. This also takes
        /// care that within the same tree view a none effect will be returned if a node is tried to be dragged to itself
        /// or a descendant, in multiple select scenarios if any node is dragged to a child.
        /// In future this will also consider keyboard input like CTRL or SHIFT.
        /// </summary>
        /// <param name="sender">sender object originally passed by .NET framework</param>
        /// <param name="e">Event args originally passed by .NET framework</param>
        /// <param name="treeNodeToDrop">Tree node currently dragged over</param>
        /// <param name="dragData">Drag information containing information about the objects to be dragged</param>
        /// <returns></returns>
        protected virtual DragDropEffects GetDragOverEffect(object sender, DragEventArgs e, TreeNode treeNodeToDrop, DbDragData dragData)
        {
            var result = DragDropEffects.None;

            if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                result = DragDropEffects.Move;
            }
            else
            {
                result = DragDropEffects.Copy;
            }

            var senderControl = dragData.SenderControl();

            if (senderControl != this)
            {
                // This is dragged into a different tree view, check if copy is allowed - this will be the default
                if ((e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
                {
                    result = DragDropEffects.Copy;
                }
            }
            else
            {
                // This is dragged within the same tree view, check that its not dragged to a child of the selection
                foreach (var dragItem in dragData.DragItems)
                {
                    if (typeof(TreeNode).IsInstanceOfType(dragItem))
                    {
                        var treeNode = (TreeNode)dragItem;

                        if (NodeIsDescendantOrSelf(treeNodeToDrop, treeNode))
                        {
                            result = DragDropEffects.None;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Determine if a node is in the list of descendants of another node
        /// </summary>
        /// <param name="childNode"></param>
        /// <param name="parentNode"></param>
        /// <returns>true if childNode is the parentNode or the parentNode is in the parent list</returns>
        protected bool NodeIsDescendantOrSelf(TreeNode childNode, TreeNode parentNode)
        {
            if ((parentNode == null) || (childNode == null))
            {
                return false;
            }
            if (parentNode == childNode)
            {
                return true;
            }
            return NodeIsDescendantOrSelf(childNode.Parent, parentNode);
        }

        protected void FixSelectionAtMousePosition()
        {
            // We must (single) select the node beneath the mouse position if it is not in a multi-select
            var mousePosition = PointToClient(MousePosition);
            var currentNode = GetNodeAt(mousePosition);

            // check the current selection situation
            if (SelNodes.Count < 2)
            {
                // If a single node has been selected, then we should switch...
                SelectSingleNode(currentNode);
            }
            else if ((currentNode != null) && (SelNodes.Count > 1) && !IsTreeNodeSelected(currentNode))
            {
                // If multiple nodes have been selected but the one just right clicked is not in the selection
                // then we should switch the selection
                SelectSingleNode(currentNode);
            }
        }

        /// <summary>
        /// Handler called when context menu is about to be displayed... we will fill the menu items dynamically here.
        /// To control the menu items either use AddContextMenuItems once, or override CreateContextMenu to dynamically
        /// add menu items when clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ContextMenu_Popup(object sender, EventArgs e)
        {
            ContextMenu.MenuItems.Clear();

            // We must (single) select the node beneath the mouse position if it is not in a multi-select
            FixSelectionAtMousePosition();

            CreateContextMenu();
        }

        /// <summary>
        /// This is called in the popup event of the context menu and allows dynamic creation of menu items
        /// The default implementation creates the menu items defined in the menu definition, override to
        /// support special behavior.
        /// </summary>
        protected virtual void CreateContextMenu()
        {
            var multipleSelection = (SelNodes.Count > 1);
            var singleSelection = (SelNodes.Count == 1);

            foreach (var menuDefinition in mMenuDefinitions)
            {
                // consider multiselect etc...
                if ((multipleSelection && menuDefinition.mEnableForMultiSelect) || (singleSelection && menuDefinition.mEnableForSingleSelect))
                {
                    ContextMenu.MenuItems.Add(menuDefinition.mMenuItem);
                }
            }
        }

        #endregion
    }

    internal class MenuDefinition
    {
        internal MenuItem mMenuItem;
        internal bool mEnableForMultiSelect;
        internal bool mEnableForSingleSelect;
        internal int mWeight;

        public MenuDefinition(MenuItem menuItem, bool enableForMultiSelect, bool enableForSingleSelect, int weight)
        {
            mMenuItem = menuItem;
            mEnableForMultiSelect = enableForMultiSelect;
            mEnableForSingleSelect = enableForSingleSelect;
            mWeight = weight;
        }
    }

    internal class TreeNodeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return SearchTools.Compare((x as TreeNode).Text, (y as TreeNode).Text);
        }
    }
}
