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

using System.ComponentModel;
using System.Windows.Forms;

namespace SDBees.UserAdmin
{
    partial class UserAdminDLG
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(UserAdminDLG));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabUsersAndGroups = new System.Windows.Forms.TabControl();
            this.tabPageUsers = new System.Windows.Forms.TabPage();
            this.tabPageGroups = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.pgProperties = new System.Windows.Forms.PropertyGrid();
            this.lvSecurityRights = new System.Windows.Forms.ListView();
            this.contextMenuStripSecurityRights = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuAddSecurityRight = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRemoveSecurityRight = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripUser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuCreateUser = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteUser = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSetPassword = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabUsersAndGroups.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStripSecurityRights.SuspendLayout();
            this.contextMenuStripUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabUsersAndGroups);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(703, 469);
            this.splitContainer1.SplitterDistance = 262;
            this.splitContainer1.TabIndex = 0;
            // 
            // tabUsersAndGroups
            // 
            this.tabUsersAndGroups.Controls.Add(this.tabPageUsers);
            this.tabUsersAndGroups.Controls.Add(this.tabPageGroups);
            this.tabUsersAndGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabUsersAndGroups.Location = new System.Drawing.Point(0, 0);
            this.tabUsersAndGroups.Name = "tabUsersAndGroups";
            this.tabUsersAndGroups.SelectedIndex = 0;
            this.tabUsersAndGroups.Size = new System.Drawing.Size(262, 469);
            this.tabUsersAndGroups.TabIndex = 0;
            this.tabUsersAndGroups.SelectedIndexChanged += new System.EventHandler(this.tabUsersAndGroups_SelectedIndexChanged);
            // 
            // tabPageUsers
            // 
            this.tabPageUsers.Location = new System.Drawing.Point(4, 22);
            this.tabPageUsers.Name = "tabPageUsers";
            this.tabPageUsers.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUsers.Size = new System.Drawing.Size(254, 443);
            this.tabPageUsers.TabIndex = 0;
            this.tabPageUsers.Text = "Users";
            this.tabPageUsers.UseVisualStyleBackColor = true;
            // 
            // tabPageGroups
            // 
            this.tabPageGroups.Location = new System.Drawing.Point(4, 22);
            this.tabPageGroups.Name = "tabPageGroups";
            this.tabPageGroups.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGroups.Size = new System.Drawing.Size(254, 443);
            this.tabPageGroups.TabIndex = 1;
            this.tabPageGroups.Text = "Groups";
            this.tabPageGroups.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.pgProperties);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.lvSecurityRights);
            this.splitContainer2.Size = new System.Drawing.Size(437, 469);
            this.splitContainer2.SplitterDistance = 350;
            this.splitContainer2.TabIndex = 0;
            // 
            // pgProperties
            // 
            this.pgProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pgProperties.Location = new System.Drawing.Point(0, 0);
            this.pgProperties.Name = "pgProperties";
            this.pgProperties.Size = new System.Drawing.Size(437, 350);
            this.pgProperties.TabIndex = 0;
            // 
            // lvSecurityRights
            // 
            this.lvSecurityRights.ContextMenuStrip = this.contextMenuStripSecurityRights;
            this.lvSecurityRights.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvSecurityRights.Location = new System.Drawing.Point(0, 0);
            this.lvSecurityRights.Name = "lvSecurityRights";
            this.lvSecurityRights.Size = new System.Drawing.Size(437, 115);
            this.lvSecurityRights.TabIndex = 0;
            this.lvSecurityRights.UseCompatibleStateImageBehavior = false;
            this.lvSecurityRights.SelectedIndexChanged += new System.EventHandler(this.lvSecurityRights_SelectedIndexChanged);
            // 
            // contextMenuStripSecurityRights
            // 
            this.contextMenuStripSecurityRights.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuAddSecurityRight,
            this.mnuRemoveSecurityRight});
            this.contextMenuStripSecurityRights.Name = "contextMenuStripSecurityRights";
            this.contextMenuStripSecurityRights.Size = new System.Drawing.Size(211, 48);
            // 
            // mnuAddSecurityRight
            // 
            this.mnuAddSecurityRight.Name = "mnuAddSecurityRight";
            this.mnuAddSecurityRight.Size = new System.Drawing.Size(210, 22);
            this.mnuAddSecurityRight.Text = "Berechtigung &Hinzufügen";
            this.mnuAddSecurityRight.Click += new System.EventHandler(this.mnuAddSecurityRight_Click);
            // 
            // mnuRemoveSecurityRight
            // 
            this.mnuRemoveSecurityRight.Name = "mnuRemoveSecurityRight";
            this.mnuRemoveSecurityRight.Size = new System.Drawing.Size(210, 22);
            this.mnuRemoveSecurityRight.Text = "Berechtigung &Entfernen";
            this.mnuRemoveSecurityRight.Click += new System.EventHandler(this.mnuRemoveSecurityRight_Click);
            // 
            // contextMenuStripUser
            // 
            this.contextMenuStripUser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCreateUser,
            this.mnuDeleteUser,
            this.mnuSetPassword});
            this.contextMenuStripUser.Name = "contextMenuStripUser";
            this.contextMenuStripUser.Size = new System.Drawing.Size(176, 70);
            // 
            // mnuCreateUser
            // 
            this.mnuCreateUser.Name = "mnuCreateUser";
            this.mnuCreateUser.Size = new System.Drawing.Size(175, 22);
            this.mnuCreateUser.Text = "Benutzer &Erzeugen";
            this.mnuCreateUser.Click += new System.EventHandler(this.mnuCreateUser_Click);
            // 
            // mnuDeleteUser
            // 
            this.mnuDeleteUser.Name = "mnuDeleteUser";
            this.mnuDeleteUser.Size = new System.Drawing.Size(175, 22);
            this.mnuDeleteUser.Text = "Benutzer &Löschen";
            this.mnuDeleteUser.Click += new System.EventHandler(this.mnuDeleteUser_Click);
            // 
            // mnuSetPassword
            // 
            this.mnuSetPassword.Name = "mnuSetPassword";
            this.mnuSetPassword.Size = new System.Drawing.Size(175, 22);
            this.mnuSetPassword.Text = "&Password festlegen";
            this.mnuSetPassword.Click += new System.EventHandler(this.mnuSetPassword_Click);
            // 
            // UserAdminDLG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 469);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "UserAdminDLG";
            this.Text = "SMARTDataBees user handling";
            this.Load += new System.EventHandler(this.UserAdminDLG_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabUsersAndGroups.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStripSecurityRights.ResumeLayout(false);
            this.contextMenuStripUser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SplitContainer splitContainer1;
        private TabControl tabUsersAndGroups;
        private TabPage tabPageUsers;
        private TabPage tabPageGroups;
        private SplitContainer splitContainer2;
        private PropertyGrid pgProperties;
        private ContextMenuStrip contextMenuStripUser;
        private ToolStripMenuItem mnuCreateUser;
        private ToolStripMenuItem mnuDeleteUser;
        private ToolStripMenuItem mnuSetPassword;
        private ListView lvSecurityRights;
        private ContextMenuStrip contextMenuStripSecurityRights;
        private ToolStripMenuItem mnuAddSecurityRight;
        private ToolStripMenuItem mnuRemoveSecurityRight;
    }
}
