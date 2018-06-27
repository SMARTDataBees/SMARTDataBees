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

namespace SDBees.EDM
{
  partial class EDMUserControl1
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
		this.tvFolders = new TreeView();
		this.contextMenu = new ContextMenuStrip(this.components);
		this.menuLinkFlyout = new ToolStripMenuItem();
		this.menuLinkWithObject = new ToolStripMenuItem();
		this.menuLinkWithPlugin = new ToolStripMenuItem();
		this.toolStripSeparator4 = new ToolStripSeparator();
		this.menuRemoveLink = new ToolStripMenuItem();
		this.menuCreateDirectory = new ToolStripMenuItem();
		this.toolStripSeparator3 = new ToolStripSeparator();
		this.menuOpen = new ToolStripMenuItem();
		this.menuAdd = new ToolStripMenuItem();
		this.menuDelete = new ToolStripMenuItem();
		this.toolStripSeparator1 = new ToolStripSeparator();
		this.menuPaste = new ToolStripMenuItem();
		this.menuCopy = new ToolStripMenuItem();
		this.toolStripSeparator2 = new ToolStripSeparator();
		this.menuRefresh = new ToolStripMenuItem();
		this.neuToolStripMenuItemPlugins = new ToolStripMenuItem();
		this.contextMenu.SuspendLayout();
		this.SuspendLayout();
		// 
		// tvFolders
		// 
		this.tvFolders.ContextMenuStrip = this.contextMenu;
		this.tvFolders.Dock = DockStyle.Fill;
		this.tvFolders.Location = new Point(0, 0);
		this.tvFolders.Name = "tvFolders";
		this.tvFolders.Size = new Size(432, 189);
		this.tvFolders.TabIndex = 0;
		this.tvFolders.DoubleClick += new EventHandler(this.tvFolders_DoubleClick);
		this.tvFolders.AfterSelect += new TreeViewEventHandler(this.tvFolders_AfterSelect);
		this.tvFolders.KeyUp += new KeyEventHandler(this.tvFolders_KeyUp);
		// 
		// contextMenu
		// 
		this.contextMenu.Items.AddRange(new ToolStripItem[] {
            this.menuLinkFlyout,
            this.menuCreateDirectory,
            this.toolStripSeparator3,
            this.menuOpen,
            this.menuAdd,
            this.menuDelete,
            this.toolStripSeparator1,
            this.menuPaste,
            this.menuCopy,
            this.toolStripSeparator2,
            this.menuRefresh,
            this.neuToolStripMenuItemPlugins});
		this.contextMenu.Name = "contextMenu";
		this.contextMenu.Size = new Size(187, 242);
		// 
		// menuLinkFlyout
		// 
		this.menuLinkFlyout.DropDownItems.AddRange(new ToolStripItem[] {
            this.menuLinkWithObject,
            this.menuLinkWithPlugin,
            this.toolStripSeparator4,
            this.menuRemoveLink});
		this.menuLinkFlyout.Name = "menuLinkFlyout";
		this.menuLinkFlyout.Size = new Size(186, 22);
		this.menuLinkFlyout.Text = "&Verbindung";
		// 
		// menuLinkWithObject
		// 
		this.menuLinkWithObject.Name = "menuLinkWithObject";
		this.menuLinkWithObject.Size = new Size(133, 22);
		this.menuLinkWithObject.Text = "Zu &Objekt";
		this.menuLinkWithObject.Click += new EventHandler(this.menuLinkWidthObject_Click);
		// 
		// menuLinkWithPlugin
		// 
		this.menuLinkWithPlugin.Name = "menuLinkWithPlugin";
		this.menuLinkWithPlugin.Size = new Size(133, 22);
		this.menuLinkWithPlugin.Text = "Zu &Plugin";
		this.menuLinkWithPlugin.Click += new EventHandler(this.menuLinkWithPlugin_Click);
		// 
		// toolStripSeparator4
		// 
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		this.toolStripSeparator4.Size = new Size(130, 6);
		// 
		// menuRemoveLink
		// 
		this.menuRemoveLink.Name = "menuRemoveLink";
		this.menuRemoveLink.Size = new Size(133, 22);
		this.menuRemoveLink.Text = "&Entfernen";
		this.menuRemoveLink.Click += new EventHandler(this.menuRemoveLink_Click);
		// 
		// menuCreateDirectory
		// 
		this.menuCreateDirectory.Name = "menuCreateDirectory";
		this.menuCreateDirectory.Size = new Size(186, 22);
		this.menuCreateDirectory.Text = "Ve&rzeichnis erzeugen";
		this.menuCreateDirectory.Click += new EventHandler(this.menuCreateDirectory_Click);
		// 
		// toolStripSeparator3
		// 
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		this.toolStripSeparator3.Size = new Size(183, 6);
		// 
		// menuOpen
		// 
		this.menuOpen.Name = "menuOpen";
		this.menuOpen.Size = new Size(186, 22);
		this.menuOpen.Text = "&Öffnen";
		this.menuOpen.Click += new EventHandler(this.menuOpen_Click);
		// 
		// menuAdd
		// 
		this.menuAdd.Name = "menuAdd";
		this.menuAdd.Size = new Size(186, 22);
		this.menuAdd.Text = "&Hinzufügen";
		this.menuAdd.Click += new EventHandler(this.menuAdd_Click);
		// 
		// menuDelete
		// 
		this.menuDelete.Name = "menuDelete";
		this.menuDelete.Size = new Size(186, 22);
		this.menuDelete.Text = "&Löschen";
		this.menuDelete.Click += new EventHandler(this.menuDelete_Click);
		// 
		// toolStripSeparator1
		// 
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new Size(183, 6);
		// 
		// menuPaste
		// 
		this.menuPaste.Name = "menuPaste";
		this.menuPaste.Size = new Size(186, 22);
		this.menuPaste.Text = "&Einfügen";
		this.menuPaste.Click += new EventHandler(this.menuPaste_Click);
		// 
		// menuCopy
		// 
		this.menuCopy.Name = "menuCopy";
		this.menuCopy.Size = new Size(186, 22);
		this.menuCopy.Text = "&Kopieren";
		this.menuCopy.Click += new EventHandler(this.menuCopy_Click);
		// 
		// toolStripSeparator2
		// 
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new Size(183, 6);
		// 
		// menuRefresh
		// 
		this.menuRefresh.Name = "menuRefresh";
		this.menuRefresh.Size = new Size(186, 22);
		this.menuRefresh.Text = "&Aktualisieren";
		this.menuRefresh.Click += new EventHandler(this.menuRefresh_Click);
		// 
		// neuToolStripMenuItemPlugins
		// 
		this.neuToolStripMenuItemPlugins.Name = "neuToolStripMenuItemPlugins";
		this.neuToolStripMenuItemPlugins.Size = new Size(186, 22);
		this.neuToolStripMenuItemPlugins.Text = "Plugins";
		this.neuToolStripMenuItemPlugins.Visible = false;
		// 
		// EDMUserControl1
		// 
		this.AutoScaleDimensions = new SizeF(6F, 13F);
		this.AutoScaleMode = AutoScaleMode.Font;
		this.Controls.Add(this.tvFolders);
		this.Name = "EDMUserControl1";
		this.Size = new Size(432, 189);
		this.Load += new EventHandler(this.EDMUserControl1_Load);
		this.contextMenu.ResumeLayout(false);
		this.ResumeLayout(false);

    }

    #endregion

      private TreeView tvFolders;
      private ContextMenuStrip contextMenu;
      private ToolStripMenuItem menuOpen;
      private ToolStripMenuItem menuAdd;
      private ToolStripMenuItem menuDelete;
      private ToolStripSeparator toolStripSeparator1;
      private ToolStripMenuItem menuPaste;
      private ToolStripMenuItem menuCopy;
      private ToolStripSeparator toolStripSeparator2;
      private ToolStripMenuItem menuRefresh;
      private ToolStripMenuItem menuLinkFlyout;
      private ToolStripMenuItem menuLinkWithObject;
      private ToolStripMenuItem menuLinkWithPlugin;
      private ToolStripSeparator toolStripSeparator3;
      private ToolStripMenuItem menuCreateDirectory;
      private ToolStripSeparator toolStripSeparator4;
      private ToolStripMenuItem menuRemoveLink;
	  private ToolStripMenuItem neuToolStripMenuItemPlugins;
  }
}
