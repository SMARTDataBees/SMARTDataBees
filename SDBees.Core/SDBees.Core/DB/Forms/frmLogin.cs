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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SDBees.DB;
using System.IO;

namespace SDBees.DB.Forms
{
	/// <summary>
	/// Form for logging into a SDBees database
	/// </summary>
	public partial class frmLogin : Form
	{
		#region Public Properties

		BindingSource m_binding = null;

		private DB.Generic.ServerConfig m_Serverconfig;
		/// <summary>
		/// Configurationfile for the available Servers
		/// </summary>
		public DB.Generic.ServerConfig Serverconfig
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
				this.Text = m_DialogTitle + " project manager";
			}
		}

		public string Password
		{
			get
			{
				return this.m_Password.Text;
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
					int index = m_Serverconfig.ConfigItems.IndexOf(m_Serverconfig.GetSelectedItem());
					SelectLastItem(m_Serverconfig.ConfigItems[index]);
				}
				catch (Exception ex)
				{

				}
			}
		}

		private void SelectLastItem(Generic.ServerConfigItem serverConfigItem)
		{
			foreach (ListViewItem item in this.m_listViewItems.Items)
			{
				Generic.ServerConfigItem sc = item.Tag as Generic.ServerConfigItem;
				if(sc.ConfigItemGuid == serverConfigItem.ConfigItemGuid)
				{
					this.m_listViewItems.Items[item.Index].Selected = true;
					this.m_listViewItems.Select();
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
			if (this.m_listViewItems.SelectedItems != null && this.m_listViewItems.SelectedItems.Count > 0)
			{
				DB.Generic.ServerConfigItem item = this.m_listViewItems.SelectedItems[0].Tag as DB.Generic.ServerConfigItem;
				m_Serverconfig.SelectedItemGuid = item.ConfigItemGuid;
				m_propertyGridSelectedConfigItem.SelectedObject = m_Serverconfig.GetSelectedItem();

				this.m_propertyGridSelectedConfigItem.Enabled = SDBees.Core.Global.SDBeesGlobalVars.GetLoginDlgPropertiesEnabled() == true ? true : false;
				if (this.m_propertyGridSelectedConfigItem.Enabled)
					this.m_propertyGridSelectedConfigItem.ExpandAllGridItems();
				else
					this.m_propertyGridSelectedConfigItem.CollapseAllGridItems();

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
				this.m_propertyGridSelectedConfigItem.Dock = DockStyle.Fill;

				//Load expected window title
				string _title = Main.Window.MainWindowApplication.Current.GetApplicationTitle();
				if (!String.IsNullOrEmpty(_title))
				{
					DialogTitle = _title;
				}

				// Load icon for maindialog
				this.Icon = Main.Window.MainWindowApplication.Current.GetApplicationIcon();

				string _icon = Main.Window.MainWindowApplication.Current.GetApplicationIconPath();
				if (!String.IsNullOrEmpty(_icon))
				{
					string path = Path.GetDirectoryName(this.GetType().Assembly.Location);
					string full = path + _icon;
					FileInfo fi = new FileInfo(full);
					if (fi.Exists)
					{
						Bitmap bmp = new Bitmap(fi.FullName);
						bmp = new Bitmap(bmp, new Size(this.m_pictureBoxLogo.Image.Width, this.m_pictureBoxLogo.Image.Height));
						this.m_pictureBoxLogo.Image = bmp;

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
			DB.Generic.ServerConfigItem item = new Generic.ServerConfigItem();

			OpenFileDialog dlgDatabase = new OpenFileDialog();

			//dlgDatabase.Icon = Main.Window.MainWindowApplication.Current.GetApplicationIcon(); //Geht leider nicht, da der OpenFiledialog kein Icon unterstützt ...

			dlgDatabase.AddExtension = true;
			dlgDatabase.SupportMultiDottedExtensions = true;
			dlgDatabase.ValidateNames = true;
			dlgDatabase.Filter = "Project files (*.s3db)|*.s3db";
			dlgDatabase.DefaultExt = "s3db";
			dlgDatabase.CheckFileExists = false;

			if (dlgDatabase.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
			{
				//if (File.Exists(dlgDatabase.FileName))
				//    MessageBox.Show("The database already exists! We will use it...", "Database creation", MessageBoxButtons.OK);

				item.ServerDatabasePath = dlgDatabase.FileName;

				item.ConfigItemName = Path.GetFileName(dlgDatabase.FileName);

				m_Serverconfig.SelectedItemGuid = item.ConfigItemGuid;
				m_Serverconfig.ConfigItems.Add(item);
			}
			this.FillListView();
			this.SetFromObject();
		}

		private void m_propertyGridSelectedConfigItem_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			this.FillListView();
			this.SetFromObject();
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
			if (this.m_listViewItems.SelectedItems != null && this.m_listViewItems.SelectedItems.Count > 0)
			{
				if (MessageBox.Show("Do you really want to delete the selected item? The projectfile won't be deleted!", "Project", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
				{
					m_Serverconfig.DeleteSelectedItem();
					FillListView();
					SetFromObject();
				}
			}
		}

		private void m_listViewItems_DoubleClick(object sender, EventArgs e)
		{
			if (this.m_listViewItems.SelectedItems != null && this.m_listViewItems.SelectedItems.Count > 0)
			{
				this.m_buttonLogin.PerformClick();
			}
		}

		private void Login_Load(object sender, EventArgs e)
		{
			FillListView();
			SetFromObject();
		}

		private void FillListView()
		{
			this.m_listViewItems.Items.Clear();

			foreach (Generic.ServerConfigItem item in m_Serverconfig.ConfigItems)
			{
				ListViewItem lvi = new ListViewItem( Path.GetFileName(item.ConfigItemName));
				lvi.Tag = item;
				lvi.ImageIndex = 0;
				lvi.SubItems.Add(item.ServerDatabasePath);
				lvi.SubItems.Add(item.ProjectDescription);
				lvi.ToolTipText = String.Format("{0}", item.ServerDatabasePath);

				m_listViewItems.Items.Add(lvi);
			}

			this.m_listViewItems.Sort();
			this.m_listViewItems.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		}

		private void bnLogin_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		#endregion
	}
}
