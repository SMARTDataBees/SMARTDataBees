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

namespace SDBees.Core.Admin
{
  partial class AdminDialog
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gel�scht werden sollen; andernfalls False.</param>
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
    /// Erforderliche Methode f�r die Designerunterst�tzung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor ge�ndert werden.
    /// </summary>
    private void InitializeComponent()
    {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdminDialog));
            this.PluginConfigurationTreeView = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.m_listViewHelper = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.AvailablePluginsListView = new System.Windows.Forms.ListView();
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
            // PluginConfigurationTreeView
            // 
            this.PluginConfigurationTreeView.AllowDrop = true;
            this.PluginConfigurationTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PluginConfigurationTreeView.HideSelection = false;
            this.PluginConfigurationTreeView.LabelEdit = true;
            this.PluginConfigurationTreeView.Location = new System.Drawing.Point(0, 0);
            this.PluginConfigurationTreeView.Name = "PluginConfigurationTreeView";
            this.PluginConfigurationTreeView.ShowNodeToolTips = true;
            this.PluginConfigurationTreeView.Size = new System.Drawing.Size(278, 369);
            this.PluginConfigurationTreeView.TabIndex = 0;
            this.PluginConfigurationTreeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeViewConfig_AfterLabelEdit);
            this.PluginConfigurationTreeView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeViewConfig_ItemDrag);
            this.PluginConfigurationTreeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeViewConfig_DragDrop);
            this.PluginConfigurationTreeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeViewConfig_DragEnter);
            this.PluginConfigurationTreeView.DragOver += new System.Windows.Forms.DragEventHandler(this.treeViewConfig_DragOver);
            this.PluginConfigurationTreeView.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeViewConfig_KeyPress);
            this.PluginConfigurationTreeView.KeyUp += new System.Windows.Forms.KeyEventHandler(this.treeViewConfig_KeyUp);
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
            this.splitContainer1.Panel1.Controls.Add(this.PluginConfigurationTreeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.m_listViewHelper);
            this.splitContainer1.Panel2.Controls.Add(this.AvailablePluginsListView);
            this.splitContainer1.Size = new System.Drawing.Size(589, 369);
            this.splitContainer1.SplitterDistance = 278;
            this.splitContainer1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 214);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Treeview helper";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Available plugins";
            // 
            // m_listViewHelper
            // 
            this.m_listViewHelper.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_listViewHelper.CheckBoxes = true;
            this.m_listViewHelper.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.m_listViewHelper.Location = new System.Drawing.Point(2, 234);
            this.m_listViewHelper.MultiSelect = false;
            this.m_listViewHelper.Name = "m_listViewHelper";
            this.m_listViewHelper.Size = new System.Drawing.Size(296, 132);
            this.m_listViewHelper.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.m_listViewHelper.TabIndex = 1;
            this.toolTip1.SetToolTip(this.m_listViewHelper, "The list of the loaded treenode helper plugins");
            this.m_listViewHelper.UseCompatibleStateImageBehavior = false;
            this.m_listViewHelper.View = System.Windows.Forms.View.List;
            // 
            // AvailablePluginsListView
            // 
            this.AvailablePluginsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AvailablePluginsListView.Location = new System.Drawing.Point(3, 22);
            this.AvailablePluginsListView.MultiSelect = false;
            this.AvailablePluginsListView.Name = "AvailablePluginsListView";
            this.AvailablePluginsListView.Size = new System.Drawing.Size(296, 187);
            this.AvailablePluginsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.AvailablePluginsListView.TabIndex = 0;
            this.toolTip1.SetToolTip(this.AvailablePluginsListView, "The list of the loaded treeview plugins");
            this.AvailablePluginsListView.UseCompatibleStateImageBehavior = false;
            this.AvailablePluginsListView.View = System.Windows.Forms.View.List;
            this.AvailablePluginsListView.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewPlugins_ItemDrag);
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
            this.buttonClose.Click += new System.EventHandler(this.OnClose);
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
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
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
            this.buttonNew.Text = "New view";
            this.buttonNew.ToolTipText = "Create a new view";
            this.buttonNew.Click += new System.EventHandler(this.OnNewConfiguration);
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonSave.Image = ((System.Drawing.Image)(resources.GetObject("buttonSave.Image")));
            this.buttonSave.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.buttonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(26, 28);
            this.buttonSave.Text = "Save view";
            this.buttonSave.Click += new System.EventHandler(this.OnSave);
            // 
            // AdminDialog
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
            this.Name = "AdminDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Plugin manager";
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

    private TreeView PluginConfigurationTreeView;
    private SplitContainer splitContainer1;
    private ListView AvailablePluginsListView;
    private Label label2;
    private Label label1;
    private ListView m_listViewHelper;
    private Button buttonClose;
    private Button buttonCancel;
    private ToolTip toolTip1;
    private ToolStrip toolStrip1;
    private ToolStripButton buttonSave;
    private ToolStripButton buttonNew;
        private ColumnHeader columnHeader1;
    }
}
