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
using System.Text;
using System.Drawing;
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
            GroupBaseData baseData = new GroupBaseData();

            this.Table = baseData.Table;
            this.NodeColumnName = baseData.Table.PrimaryKey;
            this.ParentNodeColumnName = "parentid";
            this.DisplayColumnName = "name";
            this.ParentNullId = Guid.Empty;

            MenuItem menuCreateGroup = new MenuItem("Neue Gruppe erzeugen");
            menuCreateGroup.Click += new EventHandler(menuCreateGroup_Click);
            this.AddContextMenuItem(menuCreateGroup, false, true, -1);

            MenuItem menuDeleteGroup = new MenuItem("Gruppe(n) löschen");
            menuDeleteGroup.Click += new EventHandler(menuDeleteGroup_Click);
            this.AddContextMenuItem(menuDeleteGroup, true, true, -1);

            MenuItem menuEditSchema = new MenuItem("Schema bearbeiten");
            menuEditSchema.Click += new EventHandler(menuEditSchema_Click);
            this.AddContextMenuItem(menuEditSchema, true, true, -1);
        }

        void menuCreateGroup_Click(object sender, EventArgs e)
        {
            if (this.SelNodes.Count == 1)
            {
                TreeNode selectedNode = this.SingleSelectedNode();

                Error error = null;
                Group group = new Group(this.Database.Server);

                System.Drawing.Point location = Control.MousePosition;

                System.Windows.Forms.DialogResult dlgres = DialogResult.Abort;
                string name = InputBox.Show("Bezeichnung", "Name für neue Gruppe", group.Name, location.X, location.Y, ref dlgres);
                if (name != "")
                {
                    group.Name = name;
                    group.ParentId = selectedNode.Tag.ToString();
                    group.Save(ref error);

                    TreeNode treeNode = CreateNode(group.Id, ref error);
                    if (treeNode != null)
                    {
                        selectedNode.Nodes.Add(treeNode);

                        this.ClearSelNodes();
                        this.SelectNode(treeNode, true);
                    }
                }
            }
        }

        void menuDeleteGroup_Click(object sender, EventArgs e)
        {
            string message = "";
            int selectionCount = this.SelNodes.Count;
            if (selectionCount == 0)
            {
                return;
            }
            else if (selectionCount == 1)
            {
                TreeNode treeNode = SingleSelectedNode();
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
            foreach (MWTreeNodeWrapper tnWrapper in this.SelNodes.Values)
            {
                TreeNode treeNode = tnWrapper.Node;
                DeleteRecursiveGroups(treeNode, ref error);
            }

            this.Fill(ref error);

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

                object groupId = treeNode.Tag;
                if (groupId != null)
                {
                    Group group = new Group(Database.Server);
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
