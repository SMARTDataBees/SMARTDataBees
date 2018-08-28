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
    partial class SecurityRightDLG
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
            this.label1 = new System.Windows.Forms.Label();
            this.puSecurityType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.puDatabase = new System.Windows.Forms.ComboBox();
            this.cbSystemAdministrator = new System.Windows.Forms.CheckBox();
            this.cbDatabaseAdministrator = new System.Windows.Forms.CheckBox();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOk = new System.Windows.Forms.Button();
            this.cbTableAdministrator = new System.Windows.Forms.CheckBox();
            this.puTable = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.puColumn = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Typ:";
            // 
            // puSecurityType
            // 
            this.puSecurityType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.puSecurityType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.puSecurityType.FormattingEnabled = true;
            this.puSecurityType.Location = new System.Drawing.Point(84, 6);
            this.puSecurityType.Name = "puSecurityType";
            this.puSecurityType.Size = new System.Drawing.Size(208, 21);
            this.puSecurityType.TabIndex = 1;
            this.puSecurityType.SelectedIndexChanged += new System.EventHandler(this.puSecurityType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Database:";
            // 
            // puDatabase
            // 
            this.puDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.puDatabase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.puDatabase.FormattingEnabled = true;
            this.puDatabase.Location = new System.Drawing.Point(84, 33);
            this.puDatabase.Name = "puDatabase";
            this.puDatabase.Size = new System.Drawing.Size(208, 21);
            this.puDatabase.TabIndex = 3;
            this.puDatabase.SelectedIndexChanged += new System.EventHandler(this.puDatabase_SelectedIndexChanged);
            // 
            // cbSystemAdministrator
            // 
            this.cbSystemAdministrator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSystemAdministrator.AutoSize = true;
            this.cbSystemAdministrator.Location = new System.Drawing.Point(299, 9);
            this.cbSystemAdministrator.Name = "cbSystemAdministrator";
            this.cbSystemAdministrator.Size = new System.Drawing.Size(120, 17);
            this.cbSystemAdministrator.TabIndex = 4;
            this.cbSystemAdministrator.Text = "&Server Administrator";
            this.cbSystemAdministrator.UseVisualStyleBackColor = true;
            // 
            // cbDatabaseAdministrator
            // 
            this.cbDatabaseAdministrator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDatabaseAdministrator.AutoSize = true;
            this.cbDatabaseAdministrator.Location = new System.Drawing.Point(298, 35);
            this.cbDatabaseAdministrator.Name = "cbDatabaseAdministrator";
            this.cbDatabaseAdministrator.Size = new System.Drawing.Size(135, 17);
            this.cbDatabaseAdministrator.TabIndex = 5;
            this.cbDatabaseAdministrator.Text = "&Database Administrator";
            this.cbDatabaseAdministrator.UseVisualStyleBackColor = true;
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(380, 212);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(75, 23);
            this.bnCancel.TabIndex = 6;
            this.bnCancel.Text = "&Abbruch";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOk
            // 
            this.bnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOk.Location = new System.Drawing.Point(298, 212);
            this.bnOk.Name = "bnOk";
            this.bnOk.Size = new System.Drawing.Size(75, 23);
            this.bnOk.TabIndex = 7;
            this.bnOk.Text = "&OK";
            this.bnOk.UseVisualStyleBackColor = true;
            this.bnOk.Click += new System.EventHandler(this.bnOk_Click);
            // 
            // cbTableAdministrator
            // 
            this.cbTableAdministrator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbTableAdministrator.AutoSize = true;
            this.cbTableAdministrator.Location = new System.Drawing.Point(298, 62);
            this.cbTableAdministrator.Name = "cbTableAdministrator";
            this.cbTableAdministrator.Size = new System.Drawing.Size(116, 17);
            this.cbTableAdministrator.TabIndex = 10;
            this.cbTableAdministrator.Text = "&Table Administrator";
            this.cbTableAdministrator.UseVisualStyleBackColor = true;
            // 
            // puTable
            // 
            this.puTable.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.puTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.puTable.FormattingEnabled = true;
            this.puTable.Location = new System.Drawing.Point(84, 60);
            this.puTable.Name = "puTable";
            this.puTable.Size = new System.Drawing.Size(208, 21);
            this.puTable.TabIndex = 9;
            this.puTable.SelectedIndexChanged += new System.EventHandler(this.puTable_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Table:";
            // 
            // puColumn
            // 
            this.puColumn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.puColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.puColumn.FormattingEnabled = true;
            this.puColumn.Location = new System.Drawing.Point(84, 87);
            this.puColumn.Name = "puColumn";
            this.puColumn.Size = new System.Drawing.Size(208, 21);
            this.puColumn.TabIndex = 12;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 94);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Column:";
            // 
            // SecurityRightDLG
            // 
            this.AcceptButton = this.bnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(467, 247);
            this.Controls.Add(this.puColumn);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbTableAdministrator);
            this.Controls.Add(this.puTable);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bnOk);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.cbDatabaseAdministrator);
            this.Controls.Add(this.cbSystemAdministrator);
            this.Controls.Add(this.puDatabase);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.puSecurityType);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(475, 280);
            this.Name = "SecurityRightDLG";
            this.Text = "Change &User rights";
            this.Load += new System.EventHandler(this.SecurityRightDLG_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private ComboBox puSecurityType;
        private Label label2;
        private ComboBox puDatabase;
        private CheckBox cbSystemAdministrator;
        private CheckBox cbDatabaseAdministrator;
        private Button bnCancel;
        private Button bnOk;
        private CheckBox cbTableAdministrator;
        private ComboBox puTable;
        private Label label3;
        private ComboBox puColumn;
        private Label label4;
    }
}
