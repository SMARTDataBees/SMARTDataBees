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
using System.Windows.Forms;
using SDBees.DB;
using SDBees.GuiTools;

namespace SDBees.UserAdmin
{
    internal class GroupTreeView : DbTreeView
    {
        #region Private Data Members

        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        internal GroupTreeView(Database database) : base(database)
        {
            var baseData = new GroupBaseData();

            Table = baseData.Table;
            NodeColumnName = baseData.Table.PrimaryKey;
            ParentNodeColumnName = "parentid";
            DisplayColumnName = "name";
            ParentNullId = Guid.Empty;

            var menuCreateGroup = new MenuItem("Neue Gruppe erzeugen");
            menuCreateGroup.Click += menuCreateGroup_Click;
            AddContextMenuItem(menuCreateGroup, false, true, -1);

            var menuDeleteGroup = new MenuItem("Gruppe(n) löschen");
            menuDeleteGroup.Click += menuDeleteGroup_Click;
            AddContextMenuItem(menuDeleteGroup, true, true, -1);

            var menuEditSchema = new MenuItem("Schema bearbeiten");
            menuEditSchema.Click += menuEditSchema_Click;
            AddContextMenuItem(menuEditSchema, true, true, -1);
        }

        void menuCreateGroup_Click(object sender, EventArgs e)
        {
            if (SelNodes.Count == 1)
            {
                var selectedNode = SingleSelectedNode();

                Error error = null;
                var group = new Group(Database.Server);

                var location = MousePosition;

                var dlgres = DialogResult.Abort;
                var name = InputBox.Show("Bezeichnung", "Name für neue Gruppe", group.Name, location.X, location.Y, ref dlgres);
                if (name != "")
                {
                    group.Name = name;
                    group.ParentId = selectedNode.Tag.ToString();
                    group.Save(ref error);

                    var treeNode = CreateNode(group.Id, ref error);
                    if (treeNode != null)
                    {
                        selectedNode.Nodes.Add(treeNode);

                        ClearSelNodes();
                        SelectNode(treeNode, true);
                    }
                }
            }
        }

        void menuDeleteGroup_Click(object sender, EventArgs e)
        {
            var message = "";
            var selectionCount = SelNodes.Count;
            if (selectionCount == 0)
            {
                return;
            }
            if (selectionCount == 1)
            {
                var treeNode = SingleSelectedNode();
                if (treeNode == null)
                    return;

                message = "Soll die Gruppe '" + treeNode.Text + "' mit allen Untergruppen gelöscht werden?";
            }
            else
            {
                message = "Sollen " + selectionCount + " Gruppen mit allen Untergruppen gelöscht werden?";
            }
            if (MessageBox.Show(message, "Gruppen löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            Error error = null;
            foreach (MWTreeNodeWrapper tnWrapper in SelNodes.Values)
            {
                var treeNode = tnWrapper.Node;
                DeleteRecursiveGroups(treeNode, ref error);
            }

            Fill(ref error);

            Error.Display("Fehler beim Löschen von Gruppen aufgetreten!", error);
        }

        void menuEditSchema_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Edit Schema");
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        /// <summary>
        /// Deletes the group and all subgroups in the database, but does not change the tree
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="error"></param>
        protected void DeleteRecursiveGroups(TreeNode treeNode, ref Error error)
        {
            if ((treeNode != null) && (error == null))
            {
                foreach (TreeNode subTreeNode in treeNode.Nodes)
                {
                    DeleteRecursiveGroups(subTreeNode, ref error);
                }

                var groupId = treeNode.Tag;
                if (groupId != null)
                {
                    var group = new Group(Database.Server);
                    Error loadError = null;
                    if (group.Load(Database, groupId, ref loadError))
                    {
                        // Root group must not be deleted...
                        if (group.ParentId != Guid.Empty.ToString())
                        {
                            group.Remove(ref error);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
