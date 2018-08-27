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
	partial class ViewAdminLinkHelperCtrl
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

		#region Vom Komponenten-Designer generierter Code

		/// <summary> 
		/// Erforderliche Methode für die Designerunterstützung. 
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new Container();
            this.contextMenuStrip = new ContextMenuStrip(this.components);
            this.menuAddObjects = new ToolStripMenuItem();
            this.m_listViewChilds = new ListView();
            this.m_listViewSibling = new ListView();
            this.contextMenuStripSibling = new ContextMenuStrip(this.components);
            this.menuAddSiblingObjects = new ToolStripMenuItem();
            this.groupBox1 = new GroupBox();
            this.splitContainer1 = new SplitContainer();
            this.groupBox2 = new GroupBox();
            this.contextMenuStrip.SuspendLayout();
            this.contextMenuStripSibling.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new ToolStripItem[] {
            this.menuAddObjects});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new Size(230, 26);
            // 
            // menuAddObjects
            // 
            this.menuAddObjects.Name = "menuAddObjects";
            this.menuAddObjects.Size = new Size(229, 22);
            this.menuAddObjects.Text = "&Add to selected node as child";
            this.menuAddObjects.Click += new EventHandler(this.menuAddObjects_Click);
            // 
            // m_listViewChilds
            // 
            this.m_listViewChilds.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.m_listViewChilds.ContextMenuStrip = this.contextMenuStrip;
            this.m_listViewChilds.Location = new Point(6, 19);
            this.m_listViewChilds.Name = "m_listViewChilds";
            this.m_listViewChilds.Size = new Size(353, 190);
            this.m_listViewChilds.TabIndex = 1;
            this.m_listViewChilds.UseCompatibleStateImageBehavior = false;
            this.m_listViewChilds.View = View.List;
            this.m_listViewChilds.MouseDoubleClick += new MouseEventHandler(this.listView_MouseDoubleClick);
            // 
            // m_listViewSibling
            // 
            this.m_listViewSibling.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.m_listViewSibling.ContextMenuStrip = this.contextMenuStripSibling;
            this.m_listViewSibling.Enabled = false;
            this.m_listViewSibling.Location = new Point(6, 19);
            this.m_listViewSibling.Name = "m_listViewSibling";
            this.m_listViewSibling.Size = new Size(353, 194);
            this.m_listViewSibling.TabIndex = 2;
            this.m_listViewSibling.UseCompatibleStateImageBehavior = false;
            this.m_listViewSibling.View = View.List;
            this.m_listViewSibling.MouseDoubleClick += new MouseEventHandler(this.listViewSibling_MouseDoubleClick);
            // 
            // contextMenuStripSibling
            // 
            this.contextMenuStripSibling.Items.AddRange(new ToolStripItem[] {
            this.menuAddSiblingObjects});
            this.contextMenuStripSibling.Name = "contextMenuStripSibling";
            this.contextMenuStripSibling.Size = new Size(149, 26);
            // 
            // menuAddSiblingObjects
            // 
            this.menuAddSiblingObjects.Name = "menuAddSiblingObjects";
            this.menuAddSiblingObjects.Size = new Size(148, 22);
            this.menuAddSiblingObjects.Text = "&Add as sibling";
            this.menuAddSiblingObjects.Click += new EventHandler(this.menuAddSiblingObjects_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.m_listViewChilds);
            this.groupBox1.Location = new Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new Size(365, 215);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Lower level:";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = DockStyle.Fill;
            this.splitContainer1.Location = new Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Size = new Size(368, 444);
            this.splitContainer1.SplitterDistance = 222;
            this.splitContainer1.TabIndex = 4;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) 
            | AnchorStyles.Left) 
            | AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.m_listViewSibling);
            this.groupBox2.Location = new Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new Size(365, 219);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Same level:";
            // 
            // ViewAdminLinkHelperCtrl
            // 
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "ViewAdminLinkHelperCtrl";
            this.Size = new Size(368, 444);
            this.Load += new EventHandler(this.ViewAdminLinkHelperUserControl_Load);
            this.contextMenuStrip.ResumeLayout(false);
            this.contextMenuStripSibling.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

        private ContextMenuStrip contextMenuStrip;
        private ToolStripMenuItem menuAddObjects;
        private ListView m_listViewChilds;
        private ListView m_listViewSibling;
        private ContextMenuStrip contextMenuStripSibling;
        private ToolStripMenuItem menuAddSiblingObjects;
        private GroupBox groupBox1;
        private SplitContainer splitContainer1;
        private GroupBox groupBox2;
	}
}
