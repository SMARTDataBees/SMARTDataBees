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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.EDM
{
	public partial class EDMUserControl1 : UserControl
	{
		internal class TreeNodeTag
		{
			internal string mFullpath = "";
			internal bool mIsDirectory = false;
			internal EDMBaseData mBaseData = null;

			internal bool IsRootDirectory
			{
				get { return mBaseData != null; }
			}

			internal TreeNodeTag(string fullpath, bool isDirectory, EDMBaseData baseData)
			{
				mFullpath = fullpath;
				mIsDirectory = isDirectory;
				mBaseData = baseData;
			}
		};

		private TemplateTreenodeTag mTemplateTreenodeTag;

		List<EDMTreeNodeHelper> _allEDMHelperPlugins = null;

		public TemplateTreenodeTag TemplateTreenodeTag
		{
			get { return mTemplateTreenodeTag; }
			set { mTemplateTreenodeTag = value; }
		}

		public Database Database
		{
			get { return SDBees.DB.SDBeesDBConnection.Current.Database; }
		}

		public void Refresh()
		{
			FillFolderView();
		}

		public EDMUserControl1()
		{
			InitializeComponent();
		}

		private void EDMUserControl1_Load(object sender, EventArgs e)
		{
			// Attach an event handler for the ContextMenuStrip control's Opening event.
			contextMenu.Opening += new System.ComponentModel.CancelEventHandler(cms_Opening);

			if (!DesignMode)
			{
				FillFolderView();
			}

			//Die HelperPlugins für den EDM-Manager besorgen
			_allEDMHelperPlugins = EDMTreeNodeHelper.GetAllPlugins();
			if (this._allEDMHelperPlugins.Count > 0)
			{
				this.neuToolStripMenuItemPlugins.Visible = true;
			}
		}

		private void FillFolderView()
		{
			// Clear the TreeView each time the method is called.
			tvFolders.Nodes.Clear();

			if (mTemplateTreenodeTag != null)
			{
				// Is required for multiple purposes...
				EDMManager manager = EDMManager.Current;

				// Suppress repainting the TreeView until all the objects have been created.
				tvFolders.BeginUpdate();

				// Now analyze the TemplateTreenodeTag settings...
				ArrayList objectIds = null;
				Error error = null;
				if (manager.FindEDMDatasForPlugin(Database, PluginName, ref objectIds, ref error) > 0)
				{
					foreach (object objectId in objectIds)
					{
						EDMBaseData baseData = new EDMBaseData();
						baseData.Load(Database, objectId, ref error);

						if (error != null)
							break;

						string filespec = baseData.FileSpec;
						string folderPath = baseData.FullPathname;

						DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
						AddDirectory(folderInfo, tvFolders.Nodes, baseData);
					}
				}

				// Now the entries for this specific object
				objectIds = null;
				error = null;
				if (manager.FindEDMDatasForObject(Database, PluginName, mTemplateTreenodeTag.NodeGUID, ref objectIds, ref error) > 0)
				{
					foreach (object objectId in objectIds)
					{
						EDMBaseData baseData = new EDMBaseData();
						baseData.Load(Database, objectId, ref error);

						if (error != null)
							break;

						string filespec = baseData.FileSpec;
						string folderPath = baseData.FullPathname;

						DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
						AddDirectory(folderInfo, tvFolders.Nodes, baseData);
					}
				}

				// Begin repainting the TreeView.
				tvFolders.EndUpdate();
			}

			EnableControls();
		}
		
		private string PluginName
		{
			get { return mTemplateTreenodeTag.NodeTypeOf; }
		}

		public string CurrentSelectedPath
		{
			get{return tvFolders.SelectedNode.FullPath;}
		}

		private void AddDirectory(DirectoryInfo folderInfo, TreeNodeCollection nodes, EDMBaseData baseData)
		{
			// Create the folder if it does not exist...
			if (!Directory.Exists(folderInfo.FullName))
			{
				Directory.CreateDirectory(folderInfo.FullName);
			}

			TreeNode rootNode = new TreeNode(folderInfo.Name);
			rootNode.Tag = new TreeNodeTag(folderInfo.FullName, true, baseData);
			int imageIndex = AddImage(folderInfo.FullName);
			if (imageIndex >= 0)
			{
				rootNode.ImageIndex = imageIndex;
				rootNode.SelectedImageIndex = imageIndex;
			}
			nodes.Add(rootNode);

			// Add child directories recursively...
			DirectoryInfo[] subFolders = folderInfo.GetDirectories();

			// Display the names of the directories.
			foreach (DirectoryInfo subFolder in subFolders)
			{
				AddDirectory(subFolder, rootNode.Nodes, null);
			}

			// Add files...
			FileInfo[] files = folderInfo.GetFiles();

			// Display the names of the directories.
			foreach (FileInfo file in files)
			{
				AddFile(file, rootNode.Nodes);
			}
		}
   
		private void AddFile(FileInfo fileInfo, TreeNodeCollection nodes)
		{
			TreeNode rootNode = new TreeNode(fileInfo.Name);
			rootNode.Tag = new TreeNodeTag(fileInfo.FullName, false, null);
			int imageIndex = AddImage(fileInfo.FullName);
			if (imageIndex >= 0)
			{
				rootNode.ImageIndex = imageIndex;
				rootNode.SelectedImageIndex = imageIndex;
			}
			nodes.Add(rootNode);
		}
  
		private int AddImage(string fullPathName)
		{
			int imageIndex = -1;

			if (File.Exists(fullPathName) || Directory.Exists(fullPathName))
			{
				// Get Type Name
				SHFILEINFO info = ShellGetFileInfo.GetFileInfo(fullPathName);

				// Get ICON
				Icon fileIcon = System.Drawing.Icon.FromHandle(info.hIcon);

				if (tvFolders.ImageList == null)
				{
					tvFolders.ImageList = new ImageList();
				}
				tvFolders.ImageList.Images.Add(fileIcon);
				imageIndex = tvFolders.ImageList.Images.Count - 1;
			}

			return imageIndex;
		}

		private void EnableControls()
		{
			TreeNode node = tvFolders.SelectedNode;
			bool treenodeTagSelected = (mTemplateTreenodeTag != null);

			if (node != null)
			{
				TreeNodeTag tag = (TreeNodeTag)node.Tag;

				menuOpen.Enabled = true;
				menuAdd.Enabled = tag.mIsDirectory;
				menuDelete.Enabled = !tag.IsRootDirectory;

				menuPaste.Enabled = false;
				menuCopy.Enabled = false;

				menuLinkWithObject.Enabled = treenodeTagSelected;
				menuLinkWithPlugin.Enabled = treenodeTagSelected;
				menuRemoveLink.Enabled = tag.IsRootDirectory;
				menuCreateDirectory.Enabled = tag.mIsDirectory;

				if (tag.mIsDirectory)
				{
					foreach (EDMTreeNodeHelper treenodePlugin in _allEDMHelperPlugins)
					{
						if (treenodePlugin != null)
						{
							if (treenodePlugin.NewItemsMenue() != null)
							{
								this.neuToolStripMenuItemPlugins.DropDownItems.Add(treenodePlugin.NewItemsMenue());   
							}
						}
					}
				}
				else
				{
					//TBD: Das Handling für die registrierten Extensions
				}
			}
			else
			{
				menuOpen.Enabled = false;
				menuAdd.Enabled = false;
				menuDelete.Enabled = false;

				menuPaste.Enabled = false;
				menuCopy.Enabled = false;

				menuLinkWithObject.Enabled = treenodeTagSelected;
				menuLinkWithPlugin.Enabled = treenodeTagSelected;
				menuRemoveLink.Enabled = false;
				menuCreateDirectory.Enabled = false;
			}
		}

		private void ActionOpen(TreeNode selectedNode)
		{
			if (selectedNode != null)
			{
				TreeNodeTag tag = (TreeNodeTag)selectedNode.Tag;

				if (!tag.mIsDirectory && File.Exists(tag.mFullpath))
				{
					Process.Start(tag.mFullpath);
				}
			}
		}
	   
		private void ActionAdd(TreeNode selectedNode)
		{
			TreeNode node = selectedNode;
			if (node == null)
				return;

			TreeNodeTag tag = (TreeNodeTag)node.Tag;
			if (!tag.mIsDirectory)
				return;

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Dateien hinzufügen";
			dlg.CheckFileExists = true;

			// TBD: Directories could be classified for the specified folders in the EDM
			dlg.Filter = "Alle Dateien (*.*)|*.*";

			dlg.RestoreDirectory = true;
			dlg.Multiselect = true;

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				int numCopied = 0;

				foreach (string filename in dlg.FileNames)
				{
					FileInfo fileInfo = new FileInfo(filename);
					string destinationFilename = tag.mFullpath + "\\" + fileInfo.Name;

					bool copyFile = true;
					bool fileExists = File.Exists(destinationFilename);
					if (fileExists)
					{
						string msg = "Datei '" + fileInfo.Name + "' ist bereits vorhanden. Soll diese überschrieben werden?";
						if (MessageBox.Show(msg, "Datei überschreiben", MessageBoxButtons.YesNo) == DialogResult.No)
						{
							copyFile = false;
						}
					}

					if (copyFile)
					{
						File.Copy(filename, destinationFilename, true);
						numCopied++;

						if (!fileExists)
						{
							FileInfo destInfo = new FileInfo(destinationFilename);
							AddFile(destInfo, node.Nodes);
						}
					}
				}

				if ((numCopied > 0) && (!node.IsExpanded))
				{
					node.Expand();
				}
			}

		}
   
		private void ActionDelete(TreeNode selectedNode)
		{
			TreeNode node = selectedNode;
			if (node == null)
				return;

			TreeNodeTag tag = (TreeNodeTag)node.Tag;
			if (tag.IsRootDirectory)
			{
				MessageBox.Show("Hauptverzeichnisse dürfen nicht gelöscht werden.");
				return;
			}
			if (tag.mIsDirectory)
			{
				DirectoryInfo info = new DirectoryInfo(tag.mFullpath);

				string msg = "Soll das Verzeichnis '" + info.Name + "' mit allen Unterverzeichnissen gelöscht werden?";
				if (MessageBox.Show(msg, "Verzeichnis löschen", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					try
					{
						info.Delete(true);
						node.Remove();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Fehler beim Löschen");
					}
				}

				//MessageBox.Show("Verzeichnisse können noch nicht gelöscht werden.");
				//return;
			}
			else
			{
				FileInfo info = new FileInfo(tag.mFullpath);

				string msg = "Soll die Datei '" + info.Name + "' gelöscht werden?";
				if (MessageBox.Show(msg, "Datei löschen", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					try
					{
						info.Delete();
						node.Remove();
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Fehler beim Löschen");
					}
				}
			}
		}
	  
		private void ActionLinkWithObject()
		{
			string objectId = mTemplateTreenodeTag.NodeGUID;
			ActionLink(objectId);
		}
  
		private void ActionLinkWithPlugin()
		{
			string objectId = "";  // empty string means it's linked with the plugin
			ActionLink(objectId);
		}
  
		private void ActionLink(string objectId)
		{
			string rootDirectory = EDMManager.Current.RootDirectory;

			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.SelectedPath = rootDirectory;
			dlg.ShowNewFolderButton = true;

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				string folderName = dlg.SelectedPath;

				if (!EDMManager.Current.IsInRootDirectory(folderName))
				{
					MessageBox.Show("Verzeichnis ist nicht unter dem eingestellten EDM-Hauptverzeichnis");
				}
				else
				{
					string partialFoldername = EDMManager.Current.GetRelativePathname(folderName);

					EDMBaseData baseData = new EDMBaseData();
					baseData.SetDefaults(Database);
					baseData.Name = partialFoldername;
					baseData.PlugIn = mTemplateTreenodeTag.NodeTypeOf;
					baseData.ObjectId = objectId;
					Error error = null;
					baseData.Save(ref error);

					Error.Display("Failed to save EDM-Data", error);

					if (error == null)
					{
						DirectoryInfo folderInfo = new DirectoryInfo(folderName);
						AddDirectory(folderInfo, tvFolders.Nodes, baseData);
					}
				}
			}
		}
   
		private void ActionRemoveLink(TreeNode selectedNode)
		{
			TreeNodeTag tag = (TreeNodeTag)selectedNode.Tag;

			if (tag.mBaseData != null)
			{
				Error error = null;

				tag.mBaseData.Erase(ref error);
				if (error == null)
				{
					selectedNode.Remove();
				}
			}
		}

		private void ActionCreateDirectory(TreeNode selectedNode)
		{
			TreeNodeTag tag = (TreeNodeTag)selectedNode.Tag;

			if (tag.mIsDirectory)
			{
				System.Windows.Forms.DialogResult dlgres = DialogResult.Abort;
				string folderName = SDBees.DB.InputBox.Show("Name des neuen Verzeichnisses", "Verzeichnis erzeugen", ref dlgres);
				if (folderName != "")
				{
					string fullFolderName = tag.mFullpath + "\\" + folderName;

					try
					{
						Directory.CreateDirectory(fullFolderName);

						DirectoryInfo folderInfo = new DirectoryInfo(fullFolderName);
						AddDirectory(folderInfo, selectedNode.Nodes, null);

						if (!selectedNode.IsExpanded)
						{
							selectedNode.Expand();
						}
					}
					catch (Exception ex)
					{
						MessageBox.Show(ex.Message, "Verzeichnis Erzeugen");
					}
				}
			}
		}

		private void tvFolders_DoubleClick(object sender, EventArgs e)
		{
			ActionOpen(tvFolders.SelectedNode);
		}

		private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
		{
			EnableControls();
		}

		private void menuLinkWidthObject_Click(object sender, EventArgs e)
		{
			ActionLinkWithObject();
		}

		private void menuLinkWithPlugin_Click(object sender, EventArgs e)
		{
			ActionLinkWithPlugin();
		}

		private void menuRemoveLink_Click(object sender, EventArgs e)
		{
			ActionRemoveLink(tvFolders.SelectedNode);
		}

		private void menuCreateDirectory_Click(object sender, EventArgs e)
		{
			ActionCreateDirectory(tvFolders.SelectedNode);
		}

		private void menuAdd_Click(object sender, EventArgs e)
		{
			ActionAdd(tvFolders.SelectedNode);
		}

		private void menuDelete_Click(object sender, EventArgs e)
		{
			ActionDelete(tvFolders.SelectedNode);
		}

		private void menuOpen_Click(object sender, EventArgs e)
		{
			ActionOpen(tvFolders.SelectedNode);
		}

		private void menuPaste_Click(object sender, EventArgs e)
		{

		}

		private void menuCopy_Click(object sender, EventArgs e)
		{

		}

		private void menuRefresh_Click(object sender, EventArgs e)
		{
			FillFolderView();
		}

		private void tvFolders_KeyUp(object sender, KeyEventArgs e)
		{
			// TBD: ... delete ... etc ...
			if (e.KeyCode == Keys.Delete)
			{
				ActionDelete(tvFolders.SelectedNode);
			}
			else if (e.KeyCode == Keys.Enter)
			{
				ActionOpen(tvFolders.SelectedNode);
			}
			else if (e.KeyCode == Keys.Insert)
			{
				ActionAdd(tvFolders.SelectedNode);
			}
			else if (e.KeyCode == Keys.F5)
			{
				Refresh();
			}
		}

		// This event handler is invoked when the ContextMenuStrip
		// control's Opening event is raised. It will set the correct
		// selectedNode in the TreeView
		void cms_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{
			// Acquire references to the owning control and item.
			TreeView treeView = contextMenu.SourceControl as TreeView;

			// Set the correct TreeNode depending on the MousePosition
			// TBD: we need to igone this if the popup has been started from the Keyboard
			Point mousePosition = treeView.PointToClient(Control.MousePosition);
			TreeNode currentNode = treeView.GetNodeAt(mousePosition);
			treeView.SelectedNode = currentNode;


			// Set Cancel to false. 
			// It is optimized to true based on empty entry.
			e.Cancel = false;
		}

	}
}
