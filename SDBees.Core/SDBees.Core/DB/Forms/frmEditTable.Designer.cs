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

namespace SDBees.DB
{
    /// <summary>
    /// Form to edit a Database Table
    /// </summary>
    partial class frmEditTable
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbColumns = new System.Windows.Forms.ListBox();
            this.pgProperties = new System.Windows.Forms.PropertyGrid();
            this.bnDeleteColumn = new System.Windows.Forms.Button();
            this.bnAddColumn = new System.Windows.Forms.Button();
            this.bnCancel = new System.Windows.Forms.Button();
            this.bnOk = new System.Windows.Forms.Button();
            this.bnExport = new System.Windows.Forms.Button();
            this.bnImport = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lbColumns);
            this.groupBox1.Controls.Add(this.pgProperties);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(488, 392);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Eigenschaften:";
            // 
            // lbColumns
            // 
            this.lbColumns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lbColumns.FormattingEnabled = true;
            this.lbColumns.Location = new System.Drawing.Point(6, 19);
            this.lbColumns.Name = "lbColumns";
            this.lbColumns.Size = new System.Drawing.Size(189, 355);
            this.lbColumns.TabIndex = 0;
            this.lbColumns.SelectedIndexChanged += new System.EventHandler(this.lbColumns_SelectedIndexChanged);
            // 
            // pgProperties
            // 
            this.pgProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pgProperties.Location = new System.Drawing.Point(201, 19);
            this.pgProperties.Name = "pgProperties";
            this.pgProperties.Size = new System.Drawing.Size(281, 355);
            this.pgProperties.TabIndex = 1;
            // 
            // bnDeleteColumn
            // 
            this.bnDeleteColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnDeleteColumn.Location = new System.Drawing.Point(507, 60);
            this.bnDeleteColumn.Name = "bnDeleteColumn";
            this.bnDeleteColumn.Size = new System.Drawing.Size(126, 23);
            this.bnDeleteColumn.TabIndex = 3;
            this.bnDeleteColumn.Text = "Eigenschaft &löschen";
            this.bnDeleteColumn.UseVisualStyleBackColor = true;
            this.bnDeleteColumn.Click += new System.EventHandler(this.bnDeleteColumn_Click);
            // 
            // bnAddColumn
            // 
            this.bnAddColumn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnAddColumn.Location = new System.Drawing.Point(507, 31);
            this.bnAddColumn.Name = "bnAddColumn";
            this.bnAddColumn.Size = new System.Drawing.Size(126, 23);
            this.bnAddColumn.TabIndex = 1;
            this.bnAddColumn.Text = "&Neue Eigenschaft";
            this.bnAddColumn.UseVisualStyleBackColor = true;
            this.bnAddColumn.Click += new System.EventHandler(this.bnAddColumn_Click);
            // 
            // bnCancel
            // 
            this.bnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.bnCancel.Location = new System.Drawing.Point(507, 381);
            this.bnCancel.Name = "bnCancel";
            this.bnCancel.Size = new System.Drawing.Size(126, 23);
            this.bnCancel.TabIndex = 4;
            this.bnCancel.Text = "Abbrechen";
            this.bnCancel.UseVisualStyleBackColor = true;
            // 
            // bnOk
            // 
            this.bnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOk.Location = new System.Drawing.Point(507, 352);
            this.bnOk.Name = "bnOk";
            this.bnOk.Size = new System.Drawing.Size(126, 23);
            this.bnOk.TabIndex = 5;
            this.bnOk.Text = "OK";
            this.bnOk.UseVisualStyleBackColor = true;
            this.bnOk.Click += new System.EventHandler(this.bnOk_Click);
            // 
            // bnExport
            // 
            this.bnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnExport.Location = new System.Drawing.Point(507, 118);
            this.bnExport.Name = "bnExport";
            this.bnExport.Size = new System.Drawing.Size(126, 23);
            this.bnExport.TabIndex = 6;
            this.bnExport.Text = "&Export";
            this.bnExport.UseVisualStyleBackColor = true;
            this.bnExport.Click += new System.EventHandler(this.bnExport_Click);
            // 
            // bnImport
            // 
            this.bnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnImport.Location = new System.Drawing.Point(507, 147);
            this.bnImport.Name = "bnImport";
            this.bnImport.Size = new System.Drawing.Size(126, 23);
            this.bnImport.TabIndex = 7;
            this.bnImport.Text = "&Import";
            this.bnImport.UseVisualStyleBackColor = true;
            this.bnImport.Click += new System.EventHandler(this.bnImport_Click);
            // 
            // frmEditTable
            // 
            this.AcceptButton = this.bnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.bnCancel;
            this.ClientSize = new System.Drawing.Size(645, 416);
            this.Controls.Add(this.bnImport);
            this.Controls.Add(this.bnExport);
            this.Controls.Add(this.bnOk);
            this.Controls.Add(this.bnCancel);
            this.Controls.Add(this.bnDeleteColumn);
            this.Controls.Add(this.bnAddColumn);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "frmEditTable";
            this.ShowIcon = false;
            this.Text = "Eigenschaften Editor";
            this.Load += new System.EventHandler(this.frmEditTable_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private GroupBox groupBox1;
        private Button bnDeleteColumn;
        private Button bnAddColumn;
        private ListBox lbColumns;
        private PropertyGrid pgProperties;
        private Button bnCancel;
        private Button bnOk;
        private Button bnExport;
        private Button bnImport;
    }
}
