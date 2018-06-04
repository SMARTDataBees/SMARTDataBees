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
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
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
        private SDBees.DB.SDBeesDBConnection m_dbManager;
        private Database m_database;
        private string mNodeColumnName;
        private string mParentNodeColumnName;
        private string mDisplayColumnName;
        private string mFilter;
        private object mParentNullId;
        private List<MenuDefinition> mMenuDefinitions;
        protected ViewAdmin.ViewCache.ViewCacheImplementation mViewCache { get { return ViewAdmin.ViewCache.Instance; } }

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
        public DbTreeView(SDBees.DB.SDBeesDBConnection dbManager) : this(dbManager, null)
        {
            this.TreeViewNodeSorter = (IComparer)new TreeNodeComparer();
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public DbTreeView(Database database) : this(null, database)
        {
            this.TreeViewNodeSorter = (IComparer)new TreeNodeComparer();
        }

        public DbTreeView() : this(null, null)
        {
            this.TreeViewNodeSorter = (IComparer)new TreeNodeComparer();
        }

        /// <summary>
        /// Private constructor
        /// </summary>
        private DbTreeView(SDBees.DB.SDBeesDBConnection dbManager, Database database)
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
            this.HideSelection = false;
            this.FullRowSelect = false;

            // Create a context menu...
            this.ContextMenu = new ContextMenu();
            this.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);

            // Drag & Drop
            this.AllowDrop = false;
            this.ItemDrag += new ItemDragEventHandler(DbTreeView_ItemDrag);
            this.DragOver += new DragEventHandler(DbTreeView_DragOver);
            this.DragDrop += new DragEventHandler(DbTreeView_DragDrop);
            this.DragEnter += new DragEventHandler(DbTreeView_DragEnter);

            // Hotkeys
            this.KeyDown += new KeyEventHandler(DbTreeView_KeyDown);
        }


        private void DbTreeView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            try
            {
                // We must (single) select the node beneath the mouse position if it is not in a multi-select
                FixSelectionAtMousePosition();

                // Create the drag information that will be pass
                DbDragData dragData = new DbDragData();
                if (this.SelNodes.Count > 0)
                {
                    foreach (MWTreeNodeWrapper tnWrapper in this.SelNodes.Values)
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
                    Point targetPoint = this.PointToClient(new Point(e.X, e.Y));

                    // Select the node at the mouse position.
                    TreeNode tn = this.GetNodeAt(targetPoint);
                    SelectSingleNode(tn);

                    if (tn != null)
                    {
                        DbDragData dragData = (DbDragData)e.Data.GetData(typeof(DbDragData));

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

                    DbDragData dragData = (DbDragData)e.Data.GetData(typeof(DbDragData));

                    // Perform drag-and-drop, depending upon the effect.
                    if (e.Effect == DragDropEffects.Copy ||
                        e.Effect == DragDropEffects.Move)
                    {
                        string message = "";

                        foreach (object dragItem in dragData.DragItems)
                        {
                            if (typeof(TreeNode).IsInstanceOfType(dragItem))
                            {
                                TreeNode treeNode = (TreeNode)dragItem;

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
            if (this.SelNodes.Count == 1)
            {
                foreach (MWTreeNodeWrapper tnWrapper in this.SelNodes.Values)
                {
                    return tnWrapper.Node;
                }
            }

            if (this.SelNodes.Count > 1)
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
                FillHelper h = new FillHelper(Fill);
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
                        SDBees.ViewAdmin.ViewCache.Enable();

                        BeginUpdate();

                        FillStarting();

                        FillChildren(Nodes, mParentNullId, null, ref error);
                    }
                    finally
                    {
                        SDBees.ViewAdmin.ViewCache.Disable();
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
                    error = new Error("Object not setup correctly", 9999, this.GetType(), error);
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
            MenuDefinition menuDefinition = new MenuDefinition(menuItem, enableForMultiSelect, enableForSingleSelect, weight);
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
            string m_Sorting = "";

            if (Database.UseGlobalCaching == false)
            {
                //m_Sorting = String.Format(" ORDER BY {0} ASC", ViewAdmin.ViewRelation.m_ChildNameColumnName);
            }

            string criteriaViewDef = "";

            try
            {
                ArrayList criteriaLstViewDefs = new ArrayList();

                if (String.IsNullOrEmpty(parentType))
                {
                    criteriaLstViewDefs.Add(String.Format("parent_type='{0}'", ViewAdmin.ViewRelation.m_StartNodeValue));
                    if (String.IsNullOrEmpty(mFilter))
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
                    criteriaLstViewDefs.Add(String.Format("parent_type='{0}'", parentType));
                    if (String.IsNullOrEmpty(mFilter))
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
                ArrayList lstViewDefs = mViewCache.ViewDefinitions(criteriaViewDef, ref error);
                int numRelDefs = lstViewDefs.Count;

                for (int i = 0; i < numRelDefs; i++)
                {
                    ViewAdmin.ViewDefinition db = null;
                    if (mViewCache.ViewDefinition(lstViewDefs[i], out db, ref error))
                    {
                        SDBees.DB.Attribute attParent = new SDBees.DB.Attribute(mTable.Columns[mParentNodeColumnName], parentId.ToString());
                        string criteria = Database.FormatCriteria(attParent, DbBinaryOperator.eIsEqual, ref error);
                        if (Database.UseGlobalCaching == false)
                        {
                            criteria += m_Sorting;
                        }

                        ArrayList objectIds = mViewCache.ViewRelations(criteria, ref error);
                        int numEntries = objectIds.Count;

                        for (int index = 0; index < numEntries; index++)
                        {
                            ViewAdmin.ViewRelation viewRel = null;
                            if (mViewCache.ViewRelation(objectIds[index], out viewRel, ref error))
                            {
                                if (viewRel.ChildType == db.ChildType)
                                {
                                    TreeNode treeNode = CreateNode(objectIds[index], ref error);
                                    if (treeNode != null)
                                    {
                                        //ArrayList objectIdValidRels = null;
                                        //int numValidRelations = SDBees.ViewAdmin.ViewDefinitions.FindViewDefinitionsByParentType(mDatabase, ref objectIdValidRels, new Guid(mFilter), , ref error);
                                        nodes.Add(treeNode);

                                        NodeAdded(treeNode);

                                        // Recurse for this node
                                        object nextParentId = objectIds[index];
                                        if (mNodeColumnName != mTable.PrimaryKey)
                                        {
                                            SDBees.DB.Attribute attribute = new SDBees.DB.Attribute(mTable.Columns[mTable.PrimaryKey], nextParentId.ToString());
                                            string idCriteria = Database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
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

            SDBees.DB.Attribute attribute = new SDBees.DB.Attribute(mTable.Columns[mTable.PrimaryKey], nodeId.ToString());
            string idCriteria = Database.FormatCriteria(attribute, DbBinaryOperator.eIsEqual, ref error);
            object nodeLabel = Database.SelectSingle(mTable.Name, mDisplayColumnName, idCriteria, ref error);

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
            DragDropEffects result = DragDropEffects.None;

            if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                result = DragDropEffects.Move;
            }
            else
            {
                result = DragDropEffects.Copy;
            }

            object senderControl = dragData.SenderControl();

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
                foreach (object dragItem in dragData.DragItems)
                {
                    if (typeof(TreeNode).IsInstanceOfType(dragItem))
                    {
                        TreeNode treeNode = (TreeNode)dragItem;

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
            else if (parentNode == childNode)
            {
                return true;
            }
            else
            {
                return NodeIsDescendantOrSelf(childNode.Parent, parentNode);
            }
        }

        protected void FixSelectionAtMousePosition()
        {
            // We must (single) select the node beneath the mouse position if it is not in a multi-select
            Point mousePosition = this.PointToClient(Control.MousePosition);
            TreeNode currentNode = this.GetNodeAt(mousePosition);

            // check the current selection situation
            if (this.SelNodes.Count < 2)
            {
                // If a single node has been selected, then we should switch...
                this.SelectSingleNode(currentNode);
            }
            else if ((currentNode != null) && (this.SelNodes.Count > 1) && !this.IsTreeNodeSelected(currentNode))
            {
                // If multiple nodes have been selected but the one just right clicked is not in the selection
                // then we should switch the selection
                this.SelectSingleNode(currentNode);
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
            this.ContextMenu.MenuItems.Clear();

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
            bool multipleSelection = (this.SelNodes.Count > 1);
            bool singleSelection = (this.SelNodes.Count == 1);

            foreach (MenuDefinition menuDefinition in mMenuDefinitions)
            {
                // consider multiselect etc...
                if ((multipleSelection && menuDefinition.mEnableForMultiSelect) || (singleSelection && menuDefinition.mEnableForSingleSelect))
                {
                    this.ContextMenu.MenuItems.Add(menuDefinition.mMenuItem);
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
    };

    internal class TreeNodeComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            return SDBees.Core.Utils.SearchTools.Compare((x as TreeNode).Text, (y as TreeNode).Text);
        }
    }
}
