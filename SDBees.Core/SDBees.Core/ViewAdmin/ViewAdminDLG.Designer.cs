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
namespace SDBees.ViewAdmin
{
  partial class ViewAdminDLG
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ViewAdminDLG));
            this.treeViewSystemConfig = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listViewHelper = new System.Windows.Forms.ListView();
            this.listViewPlugins = new System.Windows.Forms.ListView();
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.buttonNew = new System.Windows.Forms.ToolStripButton();
            this.buttonSave = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewSystemConfig
            // 
            this.treeViewSystemConfig.AllowDrop = true;
            this.treeViewSystemConfig.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewSystemConfig.HideSelection = false;
            this.treeViewSystemConfig.LabelEdit = true;
            this.treeViewSystemConfig.Location = new System.Drawing.Point(0, 0);
            this.treeViewSystemConfig.Name = "treeViewSystemConfig";
            this.treeViewSystemConfig.ShowNodeToolTips = true;
            this.treeViewSystemConfig.Size = new System.Drawing.Size(279, 369);
            this.treeViewSystemConfig.TabIndex = 0;
            this.treeViewSystemConfig.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewConfig_AfterLabelEdit);
            this.treeViewSystemConfig.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewConfig_ItemDrag);
            this.treeViewSystemConfig.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewConfig_DragDrop);
            this.treeViewSystemConfig.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewConfig_DragEnter);
            this.treeViewSystemConfig.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewConfig_DragOver);
            this.treeViewSystemConfig.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeViewConfig_KeyPress);
            this.treeViewSystemConfig.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeViewConfig_KeyUp);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewSystemConfig);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.listViewHelper);
            this.splitContainer1.Panel2.Controls.Add(this.listViewPlugins);
            this.splitContainer1.Size = new System.Drawing.Size(589, 369);
            this.splitContainer1.SplitterDistance = 279;
            this.splitContainer1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 215);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Treeview Helper";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Treeview Plugins";
            // 
            // listViewHelper
            // 
            this.listViewHelper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewHelper.CheckBoxes = true;
            this.listViewHelper.Enabled = false;
            this.listViewHelper.Location = new System.Drawing.Point(2, 234);
            this.listViewHelper.MultiSelect = false;
            this.listViewHelper.Name = "listViewHelper";
            this.listViewHelper.Size = new System.Drawing.Size(301, 132);
            this.listViewHelper.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewHelper.TabIndex = 1;
            this.toolTip1.SetToolTip(this.listViewHelper, "The list of the loaded treenode helper plugins");
            this.listViewHelper.UseCompatibleStateImageBehavior = false;
            this.listViewHelper.View = System.Windows.Forms.View.List;
            // 
            // listViewPlugins
            // 
            this.listViewPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewPlugins.Location = new System.Drawing.Point(3, 22);
            this.listViewPlugins.MultiSelect = false;
            this.listViewPlugins.Name = "listViewPlugins";
            this.listViewPlugins.Size = new System.Drawing.Size(300, 187);
            this.listViewPlugins.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listViewPlugins.TabIndex = 0;
            this.toolTip1.SetToolTip(this.listViewPlugins, "The list of the loaded treeview plugins");
            this.listViewPlugins.UseCompatibleStateImageBehavior = false;
            this.listViewPlugins.View = System.Windows.Forms.View.List;
            this.listViewPlugins.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewPlugins_ItemDrag);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonClose.Location = new System.Drawing.Point(12, 403);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(526, 403);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.UseAnimation = false;
            this.toolTip1.UseFading = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonNew,
            this.buttonSave});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(613, 31);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // buttonNew
            // 
            this.buttonNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonNew.Image = ((System.Drawing.Image)(resources.GetObject("buttonNew.Image")));
            this.buttonNew.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new System.Drawing.Size(26, 28);
            this.buttonNew.Text = "New View";
            this.buttonNew.ToolTipText = "Create a new View";
            this.buttonNew.Click += new System.EventHandler(this.buttonNew_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
            this.buttonSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(26, 28);
            this.buttonSave.Text = "Save View";
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // ViewAdminDLG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 438);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ViewAdminDLG";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ViewAdmin Manager";
            this.Load += new System.EventHandler(this.ViewAdminDLG_Load);
            this.Shown += new System.EventHandler(this.ViewAdminDLG_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TreeView treeViewSystemConfig;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.ListView listViewPlugins;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListView listViewHelper;
    private System.Windows.Forms.Button buttonClose;
    private System.Windows.Forms.Button buttonCancel;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.ToolStrip toolStrip1;
    private System.Windows.Forms.ToolStripButton buttonSave;
    private System.Windows.Forms.ToolStripButton buttonNew;
  }
}
