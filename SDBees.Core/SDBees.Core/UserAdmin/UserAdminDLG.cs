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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SDBees.DB;
using System.IO;
using System.Reflection;

namespace SDBees.UserAdmin
{
    public partial class UserAdminDLG : Form
    {
        private UserAdmin mParent;
        private ListView lvUsers;
        private DbTreeView tvGroups;

        public UserAdminDLG(UserAdmin parent)
        {
            mParent = parent;

            InitializeComponent();
        }

        private Server Server
        {
            get
            {
                return mParent.MyDBManager.Database.Server;
            }
        }

        private void UserAdminDLG_Load(object sender, EventArgs e)
        {
            CreateUserList();
            CreateGroupTree();
            CreateSecurityRightList();

            tabPageUsers.Controls.Add(lvUsers);
            tabPageGroups.Controls.Add(tvGroups);

            EnableControls();
        }

        private void CreateUserList()
        {
            lvUsers = new ListView();
            lvUsers.Dock = DockStyle.Fill;

            // Set the view to show details.
            lvUsers.View = View.Details;
            // Don't allow the user to edit item text.
            lvUsers.LabelEdit = false;
            // Don't allow the user to rearrange columns.
            lvUsers.AllowColumnReorder = false;
            // Select the item and sub items when selection is made.
            lvUsers.FullRowSelect = true;
            // Don't display grid lines.
            lvUsers.GridLines = false;
            // Sort the items in the list in ascending order.
            lvUsers.Sorting = SortOrder.Ascending;
            // Show the selection, even when focus is lost
            lvUsers.HideSelection = false;

            // Images...
            lvUsers.SmallImageList = new ImageList();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(SDBees.Core.Properties.Resources.User);
            lvUsers.SmallImageList.Images.Add(image);

            // Create columns for the items and sub items.
            lvUsers.Columns.Add("Benutzer", -2, HorizontalAlignment.Left);
            lvUsers.Columns.Add("Beschreibung", -2, HorizontalAlignment.Left);

            // Define the event handlers...
            lvUsers.SelectedIndexChanged += new EventHandler(lvUsers_SelectedIndexChanged);
            lvUsers.ContextMenuStrip = contextMenuStripUser;

            FillUserList();

            if (lvUsers.Items.Count > 0)
            {
                lvUsers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else
            {
                lvUsers.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void FillUserList()
        {
            lvUsers.Items.Clear();

            ListViewItem lviSelected = null;

            ArrayList loginNames = new ArrayList();
            int loginCount = User.GetAllLogins(Server, ref loginNames);

            foreach (string loginName in loginNames)
            {
                User user = User.FindUser(Server, loginName);

                ListViewItem lvItem = new ListViewItem(user.LoginName, 0);
                lvItem.SubItems.Add(user.Description);
                lvUsers.Items.Add(lvItem);

                if (lviSelected == null)
                {
                    lviSelected = lvItem;
                    lviSelected.Selected = true;
                    lviSelected.Focused = true;
                }
            }
        }

        private void CreateGroupTree()
        {
            tvGroups = new GroupTreeView(Server.SecurityDatabase);
            tvGroups.Dock = DockStyle.Fill;

            Error error = null;
            tvGroups.Fill(ref error);

            tvGroups.AfterSelect += new TreeViewEventHandler(tvGroups_AfterSelect);
            tvGroups.AfterSelNodesChanged += new EventHandler(tvGroups_AfterSelNodesChanged);
            tvGroups.MouseUp += new MouseEventHandler(tvGroups_MouseUp);

            Error.Display("Failed to fill Group tree", error);
        }

        private void CreateSecurityRightList()
        {
            // Set the view to show details.
            lvSecurityRights.View = View.Details;
            // Don't allow the user to edit item text.
            lvSecurityRights.LabelEdit = false;
            // Don't allow the user to rearrange columns.
            lvSecurityRights.AllowColumnReorder = false;
            // Select the item and sub items when selection is made.
            lvSecurityRights.FullRowSelect = true;
            // Don't display grid lines.
            lvSecurityRights.GridLines = false;
            // Sort the items in the list in ascending order.
            lvSecurityRights.Sorting = SortOrder.Ascending;
            // Show the selection, even when focus is lost
            lvSecurityRights.HideSelection = false;

            // Images...
            lvSecurityRights.SmallImageList = new ImageList();
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(SDBees.Core.Properties.Resources.SecurityAllow);
            lvSecurityRights.SmallImageList.Images.Add(image);
            image = new Bitmap(SDBees.Core.Properties.Resources.SecurityDeny);
            lvSecurityRights.SmallImageList.Images.Add(image);

            // Create columns for the items and sub items.
            lvSecurityRights.Columns.Add("Typ", -2, HorizontalAlignment.Left);
            lvSecurityRights.Columns.Add("Datenbank/Tabelle/Spalte", -2, HorizontalAlignment.Left);

            // Add event handlers
            lvSecurityRights.DoubleClick += new EventHandler(lvSecurityRights_DoubleClick);

            if (lvSecurityRights.Items.Count > 0)
            {
                lvSecurityRights.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
            else
            {
                lvSecurityRights.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        void lvSecurityRights_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Edit rights not implemented yet!");
        }

        private void FillSecurityRightList(string userOrGroupId)
        {
            lvSecurityRights.Items.Clear();

            ArrayList objectIds = null;
            int numAccessRights = AccessRights.GetAccessRightsForUserId(Server, userOrGroupId, ref objectIds);

            foreach (object objectId in objectIds)
            {
                AccessRights accessRights = new AccessRights(Server, objectId);

                int imageIndex = 0;
                if (accessRights.DeniedFlags != 0)
                {
                    imageIndex = 1;
                }

                ListViewItem lvItem = new ListViewItem(accessRights.Description(), imageIndex);
                lvItem.SubItems.Add(accessRights.Name);
                lvItem.Tag = objectId;

                lvSecurityRights.Items.Add(lvItem);
            }
        }

        private void EnableControls()
        {
            bool tabOnUsers = (tabUsersAndGroups.SelectedTab == this.tabPageUsers);
            int selectionCount = 0;

            if (tabOnUsers)
            {
                selectionCount = lvUsers.SelectedItems.Count;
            }
            else
            {
                selectionCount = tvGroups.SelNodes.Count;
            }

            mnuAddSecurityRight.Enabled = tabOnUsers && (selectionCount == 1);
            mnuRemoveSecurityRight.Enabled = tabOnUsers && (selectionCount == 1) && (lvSecurityRights.SelectedItems.Count > 0);
        }

        private void lvUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePropertiesAndRightsControls();
        }

        private void tvGroups_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdatePropertiesAndRightsControls();
        }

        private void tvGroups_AfterSelNodesChanged(object sender, EventArgs e)
        {
            UpdatePropertiesAndRightsControls();
        }

        private void tvGroups_MouseUp(object sender, MouseEventArgs e)
        {
            UpdatePropertiesAndRightsControls();
        }

        private void lvSecurityRights_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableControls();
        }

        private void mnuCreateUser_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult dlgres = DialogResult.Abort;
            string loginName = InputBox.Show("Login name", "Benutzer erzeugen", "benutzer1", ref dlgres);

            if (loginName != "")
            {
                Error error = null;
                User newUser = User.FindUser(Server, loginName);
                if ((newUser != null) || (Server.UserExists(loginName, ref error)))
                {
                    MessageBox.Show("Ein Benutzer mit dem login '" + loginName + "' existiert bereits!");
                }
                else
                {
                    newUser = new User(Server);
                    newUser.LoginName = loginName;
                    newUser.Name = loginName;

                    newUser.Save(ref error);

                    if (error == null)
                    {
                        ListViewItem lvItem = new ListViewItem(newUser.LoginName, 0);
                        lvItem.SubItems.Add(newUser.Description);
                        lvUsers.Items.Add(lvItem);

                        // Set the focus to this new Element

                        lvUsers.SelectedItems.Clear();
                        lvItem.Selected = true;
                        lvItem.Focused = true;
                    }
                    else
                    {
                        Error.Display("User Create failed!", error);
                    }
                }
            }
        }

        private void mnuDeleteUser_Click(object sender, EventArgs e)
        {
            bool listModified = false;

            ListView.SelectedListViewItemCollection selection = lvUsers.SelectedItems;

            if (selection.Count == 0)
                return;

            string message = "";
            if (selection.Count == 1)
            {
                message = "Soll der Benutzer '" + selection[0].Text + "' gelöscht werden?";
            }
            else
            {
                message = "Sollen " + selection.Count + " Benutzer gelöscht werden?";
            }
            if (MessageBox.Show(message, "Benutzer löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            foreach (ListViewItem lvItem in selection)
            {
                string loginName = lvItem.Text;
                User user = User.FindUser(Server, loginName);
                if (user != null)
                {
                    listModified = true;

                    Error error = null;
                    user.Remove(ref error);

                    if (error != null)
                    {
                        Error.Display("Remove User failed!", error);
                        break;
                    }
                }
            }

            if (listModified)
            {
                FillUserList();
            }
        }

        private void mnuSetPassword_Click(object sender, EventArgs e)
        {
            bool listModified = false;

            ListView.SelectedListViewItemCollection selection = lvUsers.SelectedItems;

            if (selection.Count != 1)
            {
                string message = "";
                if (selection.Count == 0)
                {
                    message = "Kein Benutzer ausgewählt!";
                }
                else
                {
                    message = "Es darf nur ein Benutzer ausgewählt sein!";
                }
                MessageBox.Show(message, "Passwort festlegen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            ListViewItem lvItem = selection[0];

            string loginName = lvItem.Text;
            User user = User.FindUser(Server, loginName);
            if (user != null)
            {
                System.Windows.Forms.DialogResult dlgres = DialogResult.Abort;
                string password = InputBox.Show("Passwort für Benutzer '" + loginName + "'", "Passwort festlegen", ref dlgres);

                if (password != "")
                {
                    Error error = null;
                    bool success = user.SetPassword(password, ref error);

                    if (error != null)
                    {
                        Error.Display("Remove User failed!", error);
                    }
                    else if (!success)
                    {
                        MessageBox.Show("Passwort konnte nicht festgelegt werden!", "Passwort festlegen", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                    else
                    {
                        MessageBox.Show("Passwort erfolgreich festgelegt!", "Passwort festlegen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }

            if (listModified)
            {
                FillUserList();
            }
        }

        private void mnuAddSecurityRight_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selection = lvUsers.SelectedItems;

            if (selection.Count == 0)
                return;

            User user = User.FindUser(Server, lvUsers.SelectedItems[0].Text);

            SecurityRightDLG dlg = new SecurityRightDLG(Server);
            AccessRights accessRights = new AccessRights(Server);
            accessRights.UserId = user.Id.ToString();
            dlg.AccessRights = accessRights;

            DialogResult dlgResult = dlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                string userId = "";

                Error error = null;
                accessRights.Save(ref error);

                Error.Display("Saving AccessRights failed!", error);

                if (user != null)
                {
                    // Now update the server....
                    error = null;
                    user.UpdateAccessRightsOnServer(ref error);

                    Error.Display("Failed to update the user privileges on the server", error);

                    userId = user.Id.ToString();
                }

                FillSecurityRightList(userId);
            }
        }

        private void mnuRemoveSecurityRight_Click(object sender, EventArgs e)
        {
            ListView.SelectedListViewItemCollection selection = lvSecurityRights.SelectedItems;

            if (selection.Count == 0)
                return;

            User user = User.FindUser(Server, lvUsers.SelectedItems[0].Text);

            bool listModified = false;

            string message = "";
            if (selection.Count == 1)
            {
                message = "Soll die Berechtigung '" + selection[0].Text + "' gelöscht werden?";
            }
            else
            {
                message = "Sollen " + selection.Count + " Berechtigungen gelöscht werden?";
            }
            if (MessageBox.Show(message, "Berechtigung löschen", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }

            foreach (ListViewItem lvItem in selection)
            {
                object objectId = lvItem.Tag;
                AccessRights accessRight = new AccessRights(Server, objectId);
                if (accessRight != null)
                {
                    listModified = true;

                    Error error = null;
                    accessRight.Remove(ref error);

                    if (error != null)
                    {
                        Error.Display("Remove AccessRight failed!", error);
                        break;
                    }
                }
            }

            if (listModified)
            {
                string userId = "";

                if (user != null)
                {
                    // Now update the server....
                    Error error = null;
                    user.UpdateAccessRightsOnServer(ref error);

                    Error.Display("Failed to update the user privileges on the server", error);

                    userId = user.Id.ToString();
                }

                FillSecurityRightList(userId);
            }
        }

        private void tabUsersAndGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePropertiesAndRightsControls();
        }

        private void UpdatePropertiesAndRightsControls()
        {
            bool tabOnUsers = (tabUsersAndGroups.SelectedTab == this.tabPageUsers);

            string userOrGroupId = "";

            if (tabOnUsers)
            {
                User user = null;

                if (lvUsers.SelectedItems.Count == 1)
                {
                    ListViewItem lvItem = lvUsers.SelectedItems[0];
                    string loginName = lvItem.Text;
                    user = User.FindUser(Server, loginName);
                }

                if (user != null)
                {
                    userOrGroupId = user.Id.ToString();

                    ObjectPropertyTable properties = new ObjectPropertyTable(user.BaseData);
                    pgProperties.SelectedObject = properties;
                }
                else
                {
                    pgProperties.SelectedObject = null;
                }
            }
            else
            {
                TreeNode tnSelected = tvGroups.SingleSelectedNode();

                Group group = null;
                if (tnSelected != null)
                {
                    string groupName = tnSelected.Text;
                    group = Group.FindGroup(Server, groupName);
                }

                if (group != null)
                {
                    userOrGroupId = group.Id.ToString();

                    ObjectPropertyTable properties = new ObjectPropertyTable(group.BaseData);
                    pgProperties.SelectedObject = properties;
                }
                else
                {
                    pgProperties.SelectedObject = null;
                }
            }

            FillSecurityRightList(userOrGroupId);

            EnableControls();
        }
    }
}
