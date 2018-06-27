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

using System.Collections;
using System.Windows.Forms;

namespace SDBees.DB
{
    /// <summary>
    /// This class is passed around when dragging db resident information
    /// </summary>
    public class DbDragData
    {
        #region Private Data Members

        private ArrayList mDragItems;

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the list of items that are to be dragged. The items will be
        /// either a TreeNode, a ListViewItem or some other Item with a DbTreeNodeTag atttached
        /// </summary>
        public ArrayList DragItems
        {
            get { return mDragItems; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public DbDragData()
        {
            mDragItems = new ArrayList();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the tree node tag from the DragItems at a certain index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DbTreeNodeTag TreeNodeTagAt(int index)
        {
            DbTreeNodeTag result = null;

            if ((0 <= index) && (index < mDragItems.Count))
            {
                var dragObject = mDragItems[index];

                if (typeof(TreeNode).IsInstanceOfType(dragObject))
                {
                    var treeNode = (TreeNode)dragObject;
                    result = (DbTreeNodeTag)treeNode.Tag;
                }
            }

            return result;
        }

        /// <summary>
        /// Determine the originating sender control... (TreeView, etc...)
        /// </summary>
        /// <returns></returns>
        public object SenderControl()
        {
            object result = null;

            if (mDragItems.Count > 0)
            {
                var dragObject = mDragItems[0];

                if (typeof(TreeNode).IsInstanceOfType(dragObject))
                {
                    var treeNode = (TreeNode)dragObject;
                    result = treeNode.TreeView;
                }
            }

            return result;
        }

        #endregion

        #region Protected Methods

        #endregion
    }
}
