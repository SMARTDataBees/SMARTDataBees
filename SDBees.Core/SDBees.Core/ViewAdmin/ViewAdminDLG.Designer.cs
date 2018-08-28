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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SDBees.ViewAdmin
{
  partial class ViewAdminDLG
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private IContainer components = null;

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
            this.components = new Container();
            var resources = new ComponentResourceManager(typeof(ViewAdminDLG));
            this.m_treeViewSystemConfig = new TreeView();
            this.splitContainer1 = new SplitContainer();
            this.label2 = new Label();
            this.label1 = new Label();
            this.m_listViewHelper = new ListView();
            this.m_listViewPlugins = new ListView();
            this.buttonClose = new Button();
            this.buttonCancel = new Button();
            this.toolTip1 = new ToolTip(this.components);
            this.toolStrip1 = new ToolStrip();
            this.buttonNew = new ToolStripButton();
            this.buttonSave = new ToolStripButton();
            this.columnHeader1 = ((ColumnHeader)(new ColumnHeader()));
            ((ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_treeViewSystemConfig
            // 
            this.m_treeViewSystemConfig.AllowDrop = true;
            this.m_treeViewSystemConfig.Dock = DockStyle.Fill;
            this.m_treeViewSystemConfig.HideSelection = false;
            this.m_treeViewSystemConfig.LabelEdit = true;
            this.m_treeViewSystemConfig.Location = new Point(0, 0);
            this.m_treeViewSystemConfig.Margin = new Padding(4);
            this.m_treeViewSystemConfig.Name = "m_treeViewSystemConfig";
            this.m_treeViewSystemConfig.ShowNodeToolTips = true;
            this.m_treeViewSystemConfig.Size = new Size(371, 454);
            this.m_treeViewSystemConfig.TabIndex = 0;
            this.m_treeViewSystemConfig.AfterLabelEdit += new NodeLabelEditEventHandler(this.treeViewConfig_AfterLabelEdit);
            this.m_treeViewSystemConfig.ItemDrag += new ItemDragEventHandler(this.treeViewConfig_ItemDrag);
            this.m_treeViewSystemConfig.DragDrop += new DragEventHandler(this.treeViewConfig_DragDrop);
            this.m_treeViewSystemConfig.DragEnter += new DragEventHandler(this.treeViewConfig_DragEnter);
            this.m_treeViewSystemConfig.DragOver += new DragEventHandler(this.treeViewConfig_DragOver);
            this.m_treeViewSystemConfig.KeyPress += new KeyPressEventHandler(this.treeViewConfig_KeyPress);
            this.m_treeViewSystemConfig.KeyUp += new KeyEventHandler(this.treeViewConfig_KeyUp);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.splitContainer1.Location = new Point(16, 35);
            this.splitContainer1.Margin = new Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.m_treeViewSystemConfig);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Panel2.Controls.Add(this.m_listViewHelper);
            this.splitContainer1.Panel2.Controls.Add(this.m_listViewPlugins);
            this.splitContainer1.Size = new Size(785, 454);
            this.splitContainer1.SplitterDistance = 371;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new Point(3, 264);
            this.label2.Margin = new Padding(4);
            this.label2.Name = "label2";
            this.label2.Size = new Size(111, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Treeview Helper";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new Point(4, 4);
            this.label1.Margin = new Padding(4);
            this.label1.Name = "label1";
            this.label1.Size = new Size(115, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Treeview Plugins";
            // 
            // m_listViewHelper
            // 
            this.m_listViewHelper.Anchor = ((AnchorStyles)(((AnchorStyles.Bottom | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.m_listViewHelper.CheckBoxes = true;
            this.m_listViewHelper.Columns.AddRange(new ColumnHeader[] {
            this.columnHeader1});
            this.m_listViewHelper.Location = new Point(3, 288);
            this.m_listViewHelper.Margin = new Padding(4);
            this.m_listViewHelper.MultiSelect = false;
            this.m_listViewHelper.Name = "m_listViewHelper";
            this.m_listViewHelper.Size = new Size(396, 162);
            this.m_listViewHelper.Sorting = SortOrder.Ascending;
            this.m_listViewHelper.TabIndex = 1;
            this.toolTip1.SetToolTip(this.m_listViewHelper, "The list of the loaded treenode helper plugins");
            this.m_listViewHelper.UseCompatibleStateImageBehavior = false;
            this.m_listViewHelper.View = View.List;
            // 
            // m_listViewPlugins
            // 
            this.m_listViewPlugins.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.m_listViewPlugins.Location = new Point(4, 27);
            this.m_listViewPlugins.Margin = new Padding(4);
            this.m_listViewPlugins.MultiSelect = false;
            this.m_listViewPlugins.Name = "m_listViewPlugins";
            this.m_listViewPlugins.Size = new Size(396, 229);
            this.m_listViewPlugins.Sorting = SortOrder.Ascending;
            this.m_listViewPlugins.TabIndex = 0;
            this.toolTip1.SetToolTip(this.m_listViewPlugins, "The list of the loaded treeview plugins");
            this.m_listViewPlugins.UseCompatibleStateImageBehavior = false;
            this.m_listViewPlugins.View = View.List;
            this.m_listViewPlugins.ItemDrag += new ItemDragEventHandler(this.listViewPlugins_ItemDrag);
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
            this.buttonClose.DialogResult = DialogResult.OK;
            this.buttonClose.Location = new Point(16, 496);
            this.buttonClose.Margin = new Padding(4);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new Size(100, 28);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new EventHandler(this.buttonClose_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.buttonCancel.DialogResult = DialogResult.Cancel;
            this.buttonCancel.Location = new Point(701, 496);
            this.buttonCancel.Margin = new Padding(4);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(100, 28);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new EventHandler(this.buttonCancel_Click);
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
            this.toolStrip1.ImageScalingSize = new Size(32, 32);
            this.toolStrip1.Items.AddRange(new ToolStripItem[] {
            this.buttonNew,
            this.buttonSave});
            this.toolStrip1.Location = new Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new Size(817, 31);
            this.toolStrip1.TabIndex = 7;
            this.toolStrip1.Text = "toolStrip1";
            this.toolStrip1.ItemClicked += new ToolStripItemClickedEventHandler(this.toolStrip1_ItemClicked);
            // 
            // buttonNew
            // 
            this.buttonNew.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.buttonNew.Image = ((Image)(resources.GetObject("buttonNew.Image")));
            this.buttonNew.ImageScaling = ToolStripItemImageScaling.None;
            this.buttonNew.ImageTransparentColor = Color.Magenta;
            this.buttonNew.Name = "buttonNew";
            this.buttonNew.Size = new Size(26, 28);
            this.buttonNew.Text = "New View";
            this.buttonNew.ToolTipText = "Create a new View";
            this.buttonNew.Click += new EventHandler(this.buttonNew_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.buttonSave.Image = ((Image)(resources.GetObject("buttonSave.Image")));
            this.buttonSave.ImageScaling = ToolStripItemImageScaling.None;
            this.buttonSave.ImageTransparentColor = Color.Magenta;
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new Size(26, 28);
            this.buttonSave.Text = "Save View";
            this.buttonSave.Click += new EventHandler(this.buttonSave_Click);
            // 
            // ViewAdminDLG
            // 
            this.AutoScaleDimensions = new SizeF(8F, 16F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(817, 539);
            this.ControlBox = false;
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.splitContainer1);
            this.Icon = ((Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new Padding(4);
            this.Name = "ViewAdminDLG";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "ViewAdmin Manager";
            this.Load += new EventHandler(this.ViewAdminDLG_Load);
            this.Shown += new EventHandler(this.ViewAdminDLG_Shown);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private TreeView m_treeViewSystemConfig;
    private SplitContainer splitContainer1;
    private ListView m_listViewPlugins;
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
