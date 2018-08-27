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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SDBees.Core.Global;
using SDBees.DB.Generic;
using SDBees.Main.Window;

namespace SDBees.DB.Forms
{
	/// <summary>
	/// Form for logging into a SDBees database
	/// </summary>
	public partial class frmLogin : Form
	{
		#region Public Properties

		

		private ServerConfig m_Serverconfig;
		/// <summary>
		/// Configurationfile for the available Servers
		/// </summary>
		public ServerConfig Serverconfig
		{
			get { return m_Serverconfig; }
			set { m_Serverconfig = value; }
		}

		string m_DialogTitle = "SMARTDataBees";

		public string DialogTitle
		{
			set
			{
				m_DialogTitle = value;
				Text = m_DialogTitle + " project manager";
			}
		}

		public string Password
		{
			get
			{
				return m_Password.Text;
			}
		}

		#endregion

		#region Private Methods

		private void SetFromObject()
		{
			if (m_Serverconfig != null && m_Serverconfig.GetSelectedItem() != null)
			{
				//m_comboBoxConfigItems.DataSource = null;

				m_Password.Text = "";

				try
				{
					var index = m_Serverconfig.ConfigItems.IndexOf(m_Serverconfig.GetSelectedItem());
					SelectLastItem(m_Serverconfig.ConfigItems[index]);
				}
				catch (Exception ex)
				{

				}
			}
		}

		private void SelectLastItem(ServerConfigItem serverConfigItem)
		{
			foreach (ListViewItem item in m_listViewItems.Items)
			{
				var sc = item.Tag as ServerConfigItem;
				if(sc.ConfigItemGuid == serverConfigItem.ConfigItemGuid)
				{
					m_listViewItems.Items[item.Index].Selected = true;
					m_listViewItems.Select();
					break;
				}
			}
		}
		#endregion

		#region Constructor / Destructor

		/// <summary>
		/// Standard constructor
		/// </summary>
		public frmLogin()
		{
			InitializeComponent();

			SetupControls();
		}

		private void SetPropertyGridAndControls()
		{
			if (m_listViewItems.SelectedItems != null && m_listViewItems.SelectedItems.Count > 0)
			{
				var item = m_listViewItems.SelectedItems[0].Tag as ServerConfigItem;
				m_Serverconfig.SelectedItemGuid = item.ConfigItemGuid;
				m_propertyGridSelectedConfigItem.SelectedObject = m_Serverconfig.GetSelectedItem();

				m_propertyGridSelectedConfigItem.Enabled = SDBeesGlobalVars.GetLoginDlgPropertiesEnabled() ? true : false;
				if (m_propertyGridSelectedConfigItem.Enabled)
					m_propertyGridSelectedConfigItem.ExpandAllGridItems();
				else
					m_propertyGridSelectedConfigItem.CollapseAllGridItems();

				m_buttonDelete.Enabled = true;
			}
			else
			{
				m_propertyGridSelectedConfigItem.SelectedObject = null;
				m_buttonDelete.Enabled = false;
			}
		}

		private void SetupControls()
		{
			try
			{
				m_propertyGridSelectedConfigItem.Dock = DockStyle.Fill;

				//Load expected window title
				var _title = MainWindowApplication.Current.GetApplicationTitle();
				if (!String.IsNullOrEmpty(_title))
				{
					DialogTitle = _title;
				}

				// Load icon for maindialog
				Icon = MainWindowApplication.Current.GetApplicationIcon();

				var _icon = MainWindowApplication.Current.GetApplicationIconPath();
				if (!String.IsNullOrEmpty(_icon))
				{
					var path = Path.GetDirectoryName(GetType().Assembly.Location);
					var full = path + _icon;
					var fi = new FileInfo(full);
					if (fi.Exists)
					{
						var bmp = new Bitmap(fi.FullName);
						bmp = new Bitmap(bmp, new Size(m_pictureBoxLogo.Image.Width, m_pictureBoxLogo.Image.Height));
						m_pictureBoxLogo.Image = bmp;

						//Setup Imagelist for tile view
						m_imageListItems.Images.Add(bmp);
					}
				}

				SetPropertyGridAndControls();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		#endregion

		#region Eventhandlers

		private void frmLogin_FormClosing(object sender, FormClosingEventArgs e)
		{

		}

		private void m_buttonAddConfigItem_Click(object sender, EventArgs e)
		{
			var item = new ServerConfigItem();

			var dlgDatabase = new OpenFileDialog();

			//dlgDatabase.Icon = Main.Window.MainWindowApplication.Current.GetApplicationIcon(); //Geht leider nicht, da der OpenFiledialog kein Icon unterstützt ...

			dlgDatabase.AddExtension = true;
			dlgDatabase.SupportMultiDottedExtensions = true;
			dlgDatabase.ValidateNames = true;
			dlgDatabase.Filter = "Project files (*.s3db)|*.s3db";
			dlgDatabase.DefaultExt = "s3db";
			dlgDatabase.CheckFileExists = false;

			if (dlgDatabase.ShowDialog(this) == DialogResult.OK)
			{
				//if (File.Exists(dlgDatabase.FileName))
				//    MessageBox.Show("The database already exists! We will use it...", "Database creation", MessageBoxButtons.OK);

				item.ServerDatabasePath = dlgDatabase.FileName;

				item.ConfigItemName = Path.GetFileName(dlgDatabase.FileName);

				m_Serverconfig.SelectedItemGuid = item.ConfigItemGuid;
				m_Serverconfig.ConfigItems.Add(item);
			}
			FillListView();
			SetFromObject();
		}

		private void m_propertyGridSelectedConfigItem_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			FillListView();
			SetFromObject();
		}

		private void m_listViewItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetPropertyGridAndControls();
		}

		private void m_listViewItems_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			SetPropertyGridAndControls();
		}

		private void m_buttonDelete_Click(object sender, EventArgs e)
		{
			if (m_listViewItems.SelectedItems != null && m_listViewItems.SelectedItems.Count > 0)
			{
				if (MessageBox.Show("Do you really want to delete the selected item? The projectfile won't be deleted!", "Project", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					m_Serverconfig.DeleteSelectedItem();
					FillListView();
					SetFromObject();
				}
			}
		}

		private void m_listViewItems_DoubleClick(object sender, EventArgs e)
		{
			if (m_listViewItems.SelectedItems != null && m_listViewItems.SelectedItems.Count > 0)
			{
				m_buttonLogin.PerformClick();
			}
		}

		private void Login_Load(object sender, EventArgs e)
		{
			FillListView();
			SetFromObject();
		}

		private void FillListView()
		{
			m_listViewItems.Items.Clear();

			foreach (var item in m_Serverconfig.ConfigItems)
			{
				var lvi = new ListViewItem( Path.GetFileName(item.ConfigItemName));
				lvi.Tag = item;
				lvi.ImageIndex = 0;
				lvi.SubItems.Add(item.ServerDatabasePath);
				lvi.SubItems.Add(item.ProjectDescription);
				lvi.ToolTipText = String.Format("{0}", item.ServerDatabasePath);

				m_listViewItems.Items.Add(lvi);
			}

			m_listViewItems.Sort();
			m_listViewItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void bnLogin_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		#endregion
	}
}
