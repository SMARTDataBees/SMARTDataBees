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
	partial class ViewAdminLinkHelperCtrl
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

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddObjects = new System.Windows.Forms.ToolStripMenuItem();
            this.m_listViewChilds = new System.Windows.Forms.ListView();
            this.m_listViewSibling = new System.Windows.Forms.ListView();
            this.contextMenuStripSibling = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuAddSiblingObjects = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripSibling.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddObjects});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(230, 26);
            // 
            // menuAddObjects
            // 
            this.menuAddObjects.Name = "menuAddObjects";
            this.menuAddObjects.Size = new System.Drawing.Size(229, 22);
            this.menuAddObjects.Text = "&Add to selected node as child";
            this.menuAddObjects.Click += new System.EventHandler(this.menuAddObjects_Click);
            // 
            // m_listViewChilds
            // 
            this.m_listViewChilds.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_listViewChilds.ContextMenuStrip = this.contextMenuStrip;
            this.m_listViewChilds.Location = new System.Drawing.Point(6, 19);
            this.m_listViewChilds.Name = "m_listViewChilds";
            this.m_listViewChilds.Size = new System.Drawing.Size(353, 190);
            this.m_listViewChilds.TabIndex = 1;
            this.m_listViewChilds.UseCompatibleStateImageBehavior = false;
            this.m_listViewChilds.View = System.Windows.Forms.View.List;
            this.m_listViewChilds.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView_MouseDoubleClick);
            // 
            // m_listViewSibling
            // 
            this.m_listViewSibling.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_listViewSibling.ContextMenuStrip = this.contextMenuStripSibling;
            this.m_listViewSibling.Enabled = false;
            this.m_listViewSibling.Location = new System.Drawing.Point(6, 19);
            this.m_listViewSibling.Name = "m_listViewSibling";
            this.m_listViewSibling.Size = new System.Drawing.Size(353, 194);
            this.m_listViewSibling.TabIndex = 2;
            this.m_listViewSibling.UseCompatibleStateImageBehavior = false;
            this.m_listViewSibling.View = System.Windows.Forms.View.List;
            this.m_listViewSibling.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewSibling_MouseDoubleClick);
            // 
            // contextMenuStripSibling
            // 
            this.contextMenuStripSibling.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuAddSiblingObjects});
            this.contextMenuStripSibling.Name = "contextMenuStripSibling";
            this.contextMenuStripSibling.Size = new System.Drawing.Size(149, 26);
            // 
            // menuAddSiblingObjects
            // 
            this.menuAddSiblingObjects.Name = "menuAddSiblingObjects";
            this.menuAddSiblingObjects.Size = new System.Drawing.Size(148, 22);
            this.menuAddSiblingObjects.Text = "&Add as sibling";
            this.menuAddSiblingObjects.Click += new System.EventHandler(this.menuAddSiblingObjects_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.m_listViewChilds);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(365, 215);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lower level:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new System.Drawing.Size(368, 444);
            this.splitContainer1.SplitterDistance = 222;
            this.splitContainer1.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.m_listViewSibling);
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(365, 219);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Same level:";
            // 
            // ViewAdminLinkHelperCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ViewAdminLinkHelperCtrl";
            this.Size = new System.Drawing.Size(368, 444);
            this.Load += new System.EventHandler(this.ViewAdminLinkHelperUserControl_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripSibling.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuAddObjects;
        private System.Windows.Forms.ListView m_listViewChilds;
        private System.Windows.Forms.ListView m_listViewSibling;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripSibling;
        private System.Windows.Forms.ToolStripMenuItem menuAddSiblingObjects;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox2;
	}
}
