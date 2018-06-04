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
namespace SDBees.DB.Forms
{
    partial class frmLogin
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            this.m_Password = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.m_buttonLogin = new System.Windows.Forms.Button();
            this.m_pictureBoxLogo = new System.Windows.Forms.PictureBox();
            this.m_propertyGridSelectedConfigItem = new System.Windows.Forms.PropertyGrid();
            this.m_buttonAddConfigItem = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.m_buttonDelete = new System.Windows.Forms.Button();
            this.m_splitContainer = new System.Windows.Forms.SplitContainer();
            this.m_groupBoxProjects = new System.Windows.Forms.GroupBox();
            this.m_listViewItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.m_imageListItems = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitContainer)).BeginInit();
            this.m_splitContainer.Panel1.SuspendLayout();
            this.m_splitContainer.Panel2.SuspendLayout();
            this.m_splitContainer.SuspendLayout();
            this.m_groupBoxProjects.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_Password
            // 
            this.m_Password.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_Password.Location = new System.Drawing.Point(138, 377);
            this.m_Password.Name = "m_Password";
            this.m_Password.PasswordChar = '*';
            this.m_Password.ReadOnly = true;
            this.m_Password.Size = new System.Drawing.Size(422, 20);
            this.m_Password.TabIndex = 1;
            this.m_Password.Visible = false;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(79, 377);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Passwort:";
            this.label4.Visible = false;
            // 
            // m_buttonLogin
            // 
            this.m_buttonLogin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_buttonLogin.Location = new System.Drawing.Point(485, 404);
            this.m_buttonLogin.Name = "m_buttonLogin";
            this.m_buttonLogin.Size = new System.Drawing.Size(75, 23);
            this.m_buttonLogin.TabIndex = 0;
            this.m_buttonLogin.Text = "Open";
            this.toolTip1.SetToolTip(this.m_buttonLogin, "Login with selected data");
            this.m_buttonLogin.UseVisualStyleBackColor = true;
            this.m_buttonLogin.Click += new System.EventHandler(this.bnLogin_Click);
            // 
            // m_pictureBoxLogo
            // 
            this.m_pictureBoxLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_pictureBoxLogo.Image = global::SDBees.Core.Properties.Resources.SDBees;
            this.m_pictureBoxLogo.Location = new System.Drawing.Point(21, 377);
            this.m_pictureBoxLogo.Name = "m_pictureBoxLogo";
            this.m_pictureBoxLogo.Size = new System.Drawing.Size(52, 50);
            this.m_pictureBoxLogo.TabIndex = 8;
            this.m_pictureBoxLogo.TabStop = false;
            // 
            // m_propertyGridSelectedConfigItem
            // 
            this.m_propertyGridSelectedConfigItem.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_propertyGridSelectedConfigItem.Location = new System.Drawing.Point(24, 49);
            this.m_propertyGridSelectedConfigItem.Name = "m_propertyGridSelectedConfigItem";
            this.m_propertyGridSelectedConfigItem.Size = new System.Drawing.Size(85, 250);
            this.m_propertyGridSelectedConfigItem.TabIndex = 0;
            this.m_propertyGridSelectedConfigItem.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.m_propertyGridSelectedConfigItem_PropertyValueChanged);
            // 
            // m_buttonAddConfigItem
            // 
            this.m_buttonAddConfigItem.Location = new System.Drawing.Point(9, 3);
            this.m_buttonAddConfigItem.Name = "m_buttonAddConfigItem";
            this.m_buttonAddConfigItem.Size = new System.Drawing.Size(33, 23);
            this.m_buttonAddConfigItem.TabIndex = 9;
            this.m_buttonAddConfigItem.Text = "+";
            this.toolTip1.SetToolTip(this.m_buttonAddConfigItem, "Create new configuration");
            this.m_buttonAddConfigItem.UseVisualStyleBackColor = true;
            this.m_buttonAddConfigItem.Click += new System.EventHandler(this.m_buttonAddConfigItem_Click);
            // 
            // m_buttonDelete
            // 
            this.m_buttonDelete.Location = new System.Drawing.Point(48, 3);
            this.m_buttonDelete.Name = "m_buttonDelete";
            this.m_buttonDelete.Size = new System.Drawing.Size(33, 23);
            this.m_buttonDelete.TabIndex = 12;
            this.m_buttonDelete.Text = "-";
            this.toolTip1.SetToolTip(this.m_buttonDelete, "Delete the current configuration");
            this.m_buttonDelete.UseVisualStyleBackColor = true;
            this.m_buttonDelete.Click += new System.EventHandler(this.m_buttonDelete_Click);
            // 
            // m_splitContainer
            // 
            this.m_splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_splitContainer.Location = new System.Drawing.Point(12, 12);
            this.m_splitContainer.Name = "m_splitContainer";
            // 
            // m_splitContainer.Panel1
            // 
            this.m_splitContainer.Panel1.Controls.Add(this.m_buttonDelete);
            this.m_splitContainer.Panel1.Controls.Add(this.m_groupBoxProjects);
            this.m_splitContainer.Panel1.Controls.Add(this.m_buttonAddConfigItem);
            // 
            // m_splitContainer.Panel2
            // 
            this.m_splitContainer.Panel2.Controls.Add(this.m_propertyGridSelectedConfigItem);
            this.m_splitContainer.Size = new System.Drawing.Size(548, 359);
            this.m_splitContainer.SplitterDistance = 396;
            this.m_splitContainer.TabIndex = 10;
            // 
            // m_groupBoxProjects
            // 
            this.m_groupBoxProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_groupBoxProjects.Controls.Add(this.m_listViewItems);
            this.m_groupBoxProjects.Location = new System.Drawing.Point(3, 32);
            this.m_groupBoxProjects.Name = "m_groupBoxProjects";
            this.m_groupBoxProjects.Size = new System.Drawing.Size(390, 324);
            this.m_groupBoxProjects.TabIndex = 11;
            this.m_groupBoxProjects.TabStop = false;
            this.m_groupBoxProjects.Text = "Projects";
            // 
            // m_listViewItems
            // 
            this.m_listViewItems.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.m_listViewItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_listViewItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.m_listViewItems.FullRowSelect = true;
            this.m_listViewItems.LargeImageList = this.m_imageListItems;
            this.m_listViewItems.Location = new System.Drawing.Point(6, 19);
            this.m_listViewItems.MultiSelect = false;
            this.m_listViewItems.Name = "m_listViewItems";
            this.m_listViewItems.ShowItemToolTips = true;
            this.m_listViewItems.Size = new System.Drawing.Size(378, 299);
            this.m_listViewItems.SmallImageList = this.m_imageListItems;
            this.m_listViewItems.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.m_listViewItems.StateImageList = this.m_imageListItems;
            this.m_listViewItems.TabIndex = 11;
            this.m_listViewItems.UseCompatibleStateImageBehavior = false;
            this.m_listViewItems.View = System.Windows.Forms.View.Details;
            this.m_listViewItems.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.m_listViewItems_ItemSelectionChanged);
            this.m_listViewItems.SelectedIndexChanged += new System.EventHandler(this.m_listViewItems_SelectedIndexChanged);
            this.m_listViewItems.DoubleClick += new System.EventHandler(this.m_listViewItems_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "name";
            this.columnHeader1.Width = 46;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "location";
            this.columnHeader2.Width = 500;
            // 
            // m_imageListItems
            // 
            this.m_imageListItems.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.m_imageListItems.ImageSize = new System.Drawing.Size(32, 32);
            this.m_imageListItems.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // frmLogin
            // 
            this.AcceptButton = this.m_buttonLogin;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 439);
            this.Controls.Add(this.m_splitContainer);
            this.Controls.Add(this.m_Password);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.m_pictureBoxLogo);
            this.Controls.Add(this.m_buttonLogin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmLogin";
            this.ShowIcon = false;
            this.Text = "SMARTDataBees Login";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLogin_FormClosing);
            this.Load += new System.EventHandler(this.Login_Load);
            ((System.ComponentModel.ISupportInitialize)(this.m_pictureBoxLogo)).EndInit();
            this.m_splitContainer.Panel1.ResumeLayout(false);
            this.m_splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitContainer)).EndInit();
            this.m_splitContainer.ResumeLayout(false);
            this.m_groupBoxProjects.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox m_Password;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button m_buttonLogin;
        private System.Windows.Forms.PictureBox m_pictureBoxLogo;
        private System.Windows.Forms.PropertyGrid m_propertyGridSelectedConfigItem;
        private System.Windows.Forms.Button m_buttonAddConfigItem;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.SplitContainer m_splitContainer;
        private System.Windows.Forms.GroupBox m_groupBoxProjects;
        private System.Windows.Forms.ListView m_listViewItems;
        private System.Windows.Forms.Button m_buttonDelete;
        private System.Windows.Forms.ImageList m_imageListItems;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}
