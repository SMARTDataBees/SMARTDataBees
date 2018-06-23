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
using System.Reflection;
using System.Windows.Forms;
using SDBees.Core.Properties;
using SDBees.DB;

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
            lvUsers = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                LabelEdit = false,
                AllowColumnReorder = false,
                FullRowSelect = true,
                GridLines = false,
                Sorting = SortOrder.Ascending,
                HideSelection = false,
                SmallImageList = new ImageList()
            };

            // Set the view to show details.
            // Don't allow the user to edit item text.
            // Don't allow the user to rearrange columns.
            // Select the item and sub items when selection is made.
            // Don't display grid lines.
            // Sort the items in the list in ascending order.
            // Show the selection, even when focus is lost

            // Images...
            var assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(Resources.User);
            lvUsers.SmallImageList.Images.Add(image);

            // Create columns for the items and sub items.
            lvUsers.Columns.Add("Benutzer", -2, HorizontalAlignment.Left);
            lvUsers.Columns.Add("Beschreibung", -2, HorizontalAlignment.Left);

            // Define the event handlers...
            lvUsers.SelectedIndexChanged += lvUsers_SelectedIndexChanged;
            lvUsers.ContextMenuStrip = contextMenuStripUser;

            FillUserList();

            lvUsers.AutoResizeColumns(lvUsers.Items.Count > 0
                ? ColumnHeaderAutoResizeStyle.ColumnContent
                : ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void FillUserList()
        {
            lvUsers.Items.Clear();

            ListViewItem lviSelected = null;

            var loginNames = new ArrayList();
            var loginCount = User.GetAllLogins(Server, ref loginNames);

            foreach (string loginName in loginNames)
            {
                var user = User.FindUser(Server, loginName);

                var lvItem = new ListViewItem(user.LoginName, 0);
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
            tvGroups = new GroupTreeView(Server.SecurityDatabase) {Dock = DockStyle.Fill};

            Error error = null;
            tvGroups.Fill(ref error);

            tvGroups.AfterSelect += tvGroups_AfterSelect;
            tvGroups.AfterSelNodesChanged += tvGroups_AfterSelNodesChanged;
            tvGroups.MouseUp += tvGroups_MouseUp;

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
            var assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(Resources.SecurityAllow);
            lvSecurityRights.SmallImageList.Images.Add(image);
            image = new Bitmap(Resources.SecurityDeny);
            lvSecurityRights.SmallImageList.Images.Add(image);

            // Create columns for the items and sub items.
            lvSecurityRights.Columns.Add("Typ", -2, HorizontalAlignment.Left);
            lvSecurityRights.Columns.Add("Datenbank/Tabelle/Spalte", -2, HorizontalAlignment.Left);

            // Add event handlers
            lvSecurityRights.DoubleClick += lvSecurityRights_DoubleClick;

            lvSecurityRights.AutoResizeColumns(lvSecurityRights.Items.Count > 0
                ? ColumnHeaderAutoResizeStyle.ColumnContent
                : ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        void lvSecurityRights_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("Edit rights not implemented yet!");
        }

        private void FillSecurityRightList(string userOrGroupId)
        {
            lvSecurityRights.Items.Clear();

            ArrayList objectIds = null;
            var numAccessRights = AccessRights.GetAccessRightsForUserId(Server, userOrGroupId, ref objectIds);

            foreach (var objectId in objectIds)
            {
                var accessRights = new AccessRights(Server, objectId);

                var imageIndex = 0;
                if (accessRights.DeniedFlags != 0)
                {
                    imageIndex = 1;
                }

                var lvItem = new ListViewItem(accessRights.Description(), imageIndex);
                lvItem.SubItems.Add(accessRights.Name);
                lvItem.Tag = objectId;

                lvSecurityRights.Items.Add(lvItem);
            }
        }

        private void EnableControls()
        {
            var tabOnUsers = (tabUsersAndGroups.SelectedTab == tabPageUsers);
            var selectionCount = 0;

            selectionCount = tabOnUsers ? lvUsers.SelectedItems.Count : tvGroups.SelNodes.Count;

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
            var dlgres = DialogResult.Abort;
            var loginName = InputBox.Show("Login name", "Benutzer erzeugen", "benutzer1", ref dlgres);

            if (loginName != "")
            {
                Error error = null;
                var newUser = User.FindUser(Server, loginName);
                if ((newUser != null) || (Server.UserExists(loginName, ref error)))
                {
                    MessageBox.Show("Ein Benutzer mit dem login '" + loginName + "' existiert bereits!");
                }
                else
                {
                    newUser = new User(Server)
                    {
                        LoginName = loginName,
                        Name = loginName
                    };

                    newUser.Save(ref error);

                    if (error == null)
                    {
                        var lvItem = new ListViewItem(newUser.LoginName, 0);
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
            var listModified = false;

            var selection = lvUsers.SelectedItems;

            if (selection.Count == 0)
                return;

            var message = "";
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
                var loginName = lvItem.Text;
                var user = User.FindUser(Server, loginName);
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
            var listModified = false;

            var selection = lvUsers.SelectedItems;

            if (selection.Count != 1)
            {
                var message = "";
                message = selection.Count == 0 ? "Kein Benutzer ausgewählt!" : "Es darf nur ein Benutzer ausgewählt sein!";
                MessageBox.Show(message, "Passwort festlegen", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            var lvItem = selection[0];

            var loginName = lvItem.Text;
            var user = User.FindUser(Server, loginName);
            if (user != null)
            {
                var dlgres = DialogResult.Abort;
                var password = InputBox.Show("Passwort für Benutzer '" + loginName + "'", "Passwort festlegen", ref dlgres);

                if (password != "")
                {
                    Error error = null;
                    var success = user.SetPassword(password, ref error);

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
            var selection = lvUsers.SelectedItems;

            if (selection.Count == 0)
                return;

            var user = User.FindUser(Server, lvUsers.SelectedItems[0].Text);

            var dlg = new SecurityRightDLG(Server);
            var accessRights = new AccessRights(Server) {UserId = user.Id.ToString()};
            dlg.AccessRights = accessRights;

            var dlgResult = dlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                var userId = "";

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
            var selection = lvSecurityRights.SelectedItems;

            if (selection.Count == 0)
                return;

            var user = User.FindUser(Server, lvUsers.SelectedItems[0].Text);

            var listModified = false;

            var message = "";
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
                var objectId = lvItem.Tag;
                var accessRight = new AccessRights(Server, objectId);
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
                var userId = "";

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
            var tabOnUsers = (tabUsersAndGroups.SelectedTab == tabPageUsers);

            var userOrGroupId = "";

            if (tabOnUsers)
            {
                User user = null;

                if (lvUsers.SelectedItems.Count == 1)
                {
                    var lvItem = lvUsers.SelectedItems[0];
                    var loginName = lvItem.Text;
                    user = User.FindUser(Server, loginName);
                }

                if (user != null)
                {
                    userOrGroupId = user.Id.ToString();

                    var properties = new ObjectPropertyTable(user.BaseData);
                    pgProperties.SelectedObject = properties;
                }
                else
                {
                    pgProperties.SelectedObject = null;
                }
            }
            else
            {
                var tnSelected = tvGroups.SingleSelectedNode();

                Group group = null;
                if (tnSelected != null)
                {
                    var groupName = tnSelected.Text;
                    group = Group.FindGroup(Server, groupName);
                }

                if (group != null)
                {
                    userOrGroupId = group.Id.ToString();

                    var properties = new ObjectPropertyTable(group.BaseData);
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
