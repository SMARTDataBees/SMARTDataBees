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
namespace SDBees.EDM
{
  partial class EDMUserControl1
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
		this.tvFolders = new System.Windows.Forms.TreeView();
		this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuLinkFlyout = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLinkWithObject = new System.Windows.Forms.ToolStripMenuItem();
		this.menuLinkWithPlugin = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
		this.menuRemoveLink = new System.Windows.Forms.ToolStripMenuItem();
		this.menuCreateDirectory = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
		this.menuOpen = new System.Windows.Forms.ToolStripMenuItem();
		this.menuAdd = new System.Windows.Forms.ToolStripMenuItem();
		this.menuDelete = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.menuPaste = new System.Windows.Forms.ToolStripMenuItem();
		this.menuCopy = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.menuRefresh = new System.Windows.Forms.ToolStripMenuItem();
		this.neuToolStripMenuItemPlugins = new System.Windows.Forms.ToolStripMenuItem();
		this.contextMenu.SuspendLayout();
		this.SuspendLayout();
		// 
		// tvFolders
		// 
		this.tvFolders.ContextMenuStrip = this.contextMenu;
		this.tvFolders.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tvFolders.Location = new System.Drawing.Point(0, 0);
		this.tvFolders.Name = "tvFolders";
		this.tvFolders.Size = new System.Drawing.Size(432, 189);
		this.tvFolders.TabIndex = 0;
		this.tvFolders.DoubleClick += new System.EventHandler(this.tvFolders_DoubleClick);
		this.tvFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFolders_AfterSelect);
		this.tvFolders.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tvFolders_KeyUp);
		// 
		// contextMenu
		// 
		this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
		this.contextMenu.Size = new System.Drawing.Size(187, 242);
		// 
		// menuLinkFlyout
		// 
		this.menuLinkFlyout.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuLinkWithObject,
            this.menuLinkWithPlugin,
            this.toolStripSeparator4,
            this.menuRemoveLink});
		this.menuLinkFlyout.Name = "menuLinkFlyout";
		this.menuLinkFlyout.Size = new System.Drawing.Size(186, 22);
		this.menuLinkFlyout.Text = "&Verbindung";
		// 
		// menuLinkWithObject
		// 
		this.menuLinkWithObject.Name = "menuLinkWithObject";
		this.menuLinkWithObject.Size = new System.Drawing.Size(133, 22);
		this.menuLinkWithObject.Text = "Zu &Objekt";
		this.menuLinkWithObject.Click += new System.EventHandler(this.menuLinkWidthObject_Click);
		// 
		// menuLinkWithPlugin
		// 
		this.menuLinkWithPlugin.Name = "menuLinkWithPlugin";
		this.menuLinkWithPlugin.Size = new System.Drawing.Size(133, 22);
		this.menuLinkWithPlugin.Text = "Zu &Plugin";
		this.menuLinkWithPlugin.Click += new System.EventHandler(this.menuLinkWithPlugin_Click);
		// 
		// toolStripSeparator4
		// 
		this.toolStripSeparator4.Name = "toolStripSeparator4";
		this.toolStripSeparator4.Size = new System.Drawing.Size(130, 6);
		// 
		// menuRemoveLink
		// 
		this.menuRemoveLink.Name = "menuRemoveLink";
		this.menuRemoveLink.Size = new System.Drawing.Size(133, 22);
		this.menuRemoveLink.Text = "&Entfernen";
		this.menuRemoveLink.Click += new System.EventHandler(this.menuRemoveLink_Click);
		// 
		// menuCreateDirectory
		// 
		this.menuCreateDirectory.Name = "menuCreateDirectory";
		this.menuCreateDirectory.Size = new System.Drawing.Size(186, 22);
		this.menuCreateDirectory.Text = "Ve&rzeichnis erzeugen";
		this.menuCreateDirectory.Click += new System.EventHandler(this.menuCreateDirectory_Click);
		// 
		// toolStripSeparator3
		// 
		this.toolStripSeparator3.Name = "toolStripSeparator3";
		this.toolStripSeparator3.Size = new System.Drawing.Size(183, 6);
		// 
		// menuOpen
		// 
		this.menuOpen.Name = "menuOpen";
		this.menuOpen.Size = new System.Drawing.Size(186, 22);
		this.menuOpen.Text = "&Öffnen";
		this.menuOpen.Click += new System.EventHandler(this.menuOpen_Click);
		// 
		// menuAdd
		// 
		this.menuAdd.Name = "menuAdd";
		this.menuAdd.Size = new System.Drawing.Size(186, 22);
		this.menuAdd.Text = "&Hinzufügen";
		this.menuAdd.Click += new System.EventHandler(this.menuAdd_Click);
		// 
		// menuDelete
		// 
		this.menuDelete.Name = "menuDelete";
		this.menuDelete.Size = new System.Drawing.Size(186, 22);
		this.menuDelete.Text = "&Löschen";
		this.menuDelete.Click += new System.EventHandler(this.menuDelete_Click);
		// 
		// toolStripSeparator1
		// 
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
		// 
		// menuPaste
		// 
		this.menuPaste.Name = "menuPaste";
		this.menuPaste.Size = new System.Drawing.Size(186, 22);
		this.menuPaste.Text = "&Einfügen";
		this.menuPaste.Click += new System.EventHandler(this.menuPaste_Click);
		// 
		// menuCopy
		// 
		this.menuCopy.Name = "menuCopy";
		this.menuCopy.Size = new System.Drawing.Size(186, 22);
		this.menuCopy.Text = "&Kopieren";
		this.menuCopy.Click += new System.EventHandler(this.menuCopy_Click);
		// 
		// toolStripSeparator2
		// 
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(183, 6);
		// 
		// menuRefresh
		// 
		this.menuRefresh.Name = "menuRefresh";
		this.menuRefresh.Size = new System.Drawing.Size(186, 22);
		this.menuRefresh.Text = "&Aktualisieren";
		this.menuRefresh.Click += new System.EventHandler(this.menuRefresh_Click);
		// 
		// neuToolStripMenuItemPlugins
		// 
		this.neuToolStripMenuItemPlugins.Name = "neuToolStripMenuItemPlugins";
		this.neuToolStripMenuItemPlugins.Size = new System.Drawing.Size(186, 22);
		this.neuToolStripMenuItemPlugins.Text = "Plugins";
		this.neuToolStripMenuItemPlugins.Visible = false;
		// 
		// EDMUserControl1
		// 
		this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
		this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Controls.Add(this.tvFolders);
		this.Name = "EDMUserControl1";
		this.Size = new System.Drawing.Size(432, 189);
		this.Load += new System.EventHandler(this.EDMUserControl1_Load);
		this.contextMenu.ResumeLayout(false);
		this.ResumeLayout(false);

    }

    #endregion

      private System.Windows.Forms.TreeView tvFolders;
      private System.Windows.Forms.ContextMenuStrip contextMenu;
      private System.Windows.Forms.ToolStripMenuItem menuOpen;
      private System.Windows.Forms.ToolStripMenuItem menuAdd;
      private System.Windows.Forms.ToolStripMenuItem menuDelete;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
      private System.Windows.Forms.ToolStripMenuItem menuPaste;
      private System.Windows.Forms.ToolStripMenuItem menuCopy;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
      private System.Windows.Forms.ToolStripMenuItem menuRefresh;
      private System.Windows.Forms.ToolStripMenuItem menuLinkFlyout;
      private System.Windows.Forms.ToolStripMenuItem menuLinkWithObject;
      private System.Windows.Forms.ToolStripMenuItem menuLinkWithPlugin;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
      private System.Windows.Forms.ToolStripMenuItem menuCreateDirectory;
      private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
      private System.Windows.Forms.ToolStripMenuItem menuRemoveLink;
	  private System.Windows.Forms.ToolStripMenuItem neuToolStripMenuItemPlugins;
  }
}
