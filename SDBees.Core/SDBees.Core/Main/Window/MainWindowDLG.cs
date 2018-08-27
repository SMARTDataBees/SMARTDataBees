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
using System.Reflection;
using System.Windows.Forms;
using Carbon.Configuration;
using Carbon.Configuration.Providers;

namespace SDBees.Main.Window
{
    public sealed class MainWindowApplicationDLG : Form
    {
        private MainMenu m_mainMenu;
        private MenuItem m_menuItemProject;
        private MenuItem m_menuItemTools;
        private MenuItem m_menuItemHelp;
        private StatusStrip m_statusBar;
        private GroupBox m_groupBoxSystemView;
        private GroupBox m_groupBox;
        private SplitContainer m_splitContainer;
        private ToolStrip m_toolStrip;
        private ToolTip m_toolTip;
        private MenuItem m_menuItemAdmin;
        private MenuItem m_optionsMenueItem;
        private MenuItem m_menuWebpage;
        private IContainer m_components;
        private TabControl m_tabControlMain;
        private ToolStripProgressBar m_toolStripProgressBar;

        private MainWindowApplication m_thePlugin;
        private Label m_label;
        private MenuItem m_menuItemClose;
        private ToolStripStatusLabel m_toolStripStatusLabel;
        private MenuItem m_menuInfo;
        private IContainer components;
        //private MWTreeView m_myViewTree;

        //public MWTreeView MyViewTree
        //{
        //    get { return m_myViewTree; }
        //    set { m_myViewTree = value; }
        //}

        //private static MainWindowApplicationDLG _theInstance;

        //protected bool _exitApplicationThreadOnClose = true;
        //protected bool _noPromptsOnClose;
        //public const string WindowPositioningEngineKey = "SMARTDataBees";
        //[DllImport("user32.dll")]
        //static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        //[DllImport("user32.dll")]
        //static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        //[DllImport("user32.dll")]
        //static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

        //internal const uint SC_CLOSE = 0xF060;
        //internal const uint MF_GRAYED = 0x00000001;
        //internal const uint MF_BYCOMMAND = 0x00000000;


        /// <summary>
        /// Initializes a new instance of the SystemTrayApplicationPlugin class.
        /// </summary>
        public MainWindowApplicationDLG(MainWindowApplication mainWND)
        {
            //_theInstance = this;
            InitializeComponent();

            m_thePlugin = mainWND;

            //m_myViewTree = new MWTreeView();

            //IntPtr hMenu = Process.GetCurrentProcess().MainWindowHandle;
            //IntPtr hSystemMenu = GetSystemMenu(hMenu, false);

            //EnableMenuItem(hSystemMenu, SC_CLOSE, MF_GRAYED);
            //RemoveMenu(hSystemMenu, SC_CLOSE, MF_BYCOMMAND);

            FormatElements();

            if(MainWindowApplication.Current.MyDBManager != null)
                MainWindowApplication.Current.MyDBManager.AddDatabaseChangedHandler(MainWindowApplicationDLG_OnDatabaseChangedHandler);

            var mnuLoginDatabase = new MenuItem("Project manager ...");
            mnuLoginDatabase.Click += mnuLoginDatabase_Click;
            MenuProject().MenuItems.Add(0, mnuLoginDatabase);
        }

        protected override void OnShown(EventArgs e)
        {
            MainWindowApplication.Current.MyDBManager.OnUpdate("Updating...");
        }

        void mnuLoginDatabase_Click(object sender, EventArgs e)
        {
            MainWindowApplication.Current.MyDBManager.OpenProject();
        }

        void MainWindowApplicationDLG_OnDatabaseChangedHandler(object myObject, EventArgs myArgs)
        {
            SetWindowsTitle();
        }

        public const int m_Splitterdistance = 120;

        private void FormatElements()
        {
            MyTabControl().Dock = DockStyle.Fill;

            try
            {
                //Load expected window title
                SetWindowsTitle();

                // Load icon for maindialog
                SetWindowsIcon();

                m_splitContainer.SplitterDistance = m_Splitterdistance;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetWindowsIcon()
        {
            Icon = MainWindowApplication.Current.GetApplicationIcon();
        }

        private void SetWindowsTitle()
        {
            var dbName = "";
            if(MainWindowApplication.Current.MyDBManager != null)
            {
                dbName = MainWindowApplication.Current.MyDBManager.CurrentDbName;
            }

            var version = Assembly.GetEntryAssembly().GetName().Version;

            var _title = MainWindowApplication.Current.GetApplicationTitle();
            if (!String.IsNullOrEmpty(_title))
            {
                if (version.Revision == 0)
                {
                    Text = String.Format("{0} {1}.{2}.{3} - {4}", _title, version.Major, version.Minor, version.Build, dbName);
                }
                else
                {
                    Text = String.Format("{0} {1}.{2}.{3}.{4} - {5}", _title, version.Major, version.Minor, version.Build, version.Revision, dbName);
                }
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.m_mainMenu = new System.Windows.Forms.MainMenu(this.components);
            this.m_menuItemProject = new System.Windows.Forms.MenuItem();
            this.m_menuItemClose = new System.Windows.Forms.MenuItem();
            this.m_menuItemTools = new System.Windows.Forms.MenuItem();
            this.m_menuItemAdmin = new System.Windows.Forms.MenuItem();
            this.m_optionsMenueItem = new System.Windows.Forms.MenuItem();
            this.m_menuItemHelp = new System.Windows.Forms.MenuItem();
            this.m_menuWebpage = new System.Windows.Forms.MenuItem();
            this.m_menuInfo = new System.Windows.Forms.MenuItem();
            this.m_statusBar = new System.Windows.Forms.StatusStrip();
            this.m_toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.m_toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.m_groupBoxSystemView = new System.Windows.Forms.GroupBox();
            this.m_label = new System.Windows.Forms.Label();
            this.m_groupBox = new System.Windows.Forms.GroupBox();
            this.m_tabControlMain = new System.Windows.Forms.TabControl();
            this.m_splitContainer = new System.Windows.Forms.SplitContainer();
            this.m_toolStrip = new System.Windows.Forms.ToolStrip();
            this.m_toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.m_statusBar.SuspendLayout();
            this.m_groupBoxSystemView.SuspendLayout();
            this.m_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitContainer)).BeginInit();
            this.m_splitContainer.Panel1.SuspendLayout();
            this.m_splitContainer.Panel2.SuspendLayout();
            this.m_splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_mainMenu
            // 
            this.m_mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.m_menuItemProject,
            this.m_menuItemTools,
            this.m_menuItemAdmin,
            this.m_menuItemHelp});
            // 
            // m_menuItemProject
            // 
            this.m_menuItemProject.Index = 0;
            this.m_menuItemProject.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.m_menuItemClose});
            this.m_menuItemProject.Text = "Project";
            // 
            // m_menuItemClose
            // 
            this.m_menuItemClose.Index = 0;
            this.m_menuItemClose.Text = "Exit";
            this.m_menuItemClose.Click += new System.EventHandler(this.menuItemClose_Click);
            // 
            // m_menuItemTools
            // 
            this.m_menuItemTools.Index = 1;
            this.m_menuItemTools.Text = "Tools";
            // 
            // m_menuItemAdmin
            // 
            this.m_menuItemAdmin.Index = 2;
            this.m_menuItemAdmin.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.m_optionsMenueItem});
            this.m_menuItemAdmin.Text = "Admin";
            // 
            // m_optionsMenueItem
            // 
            this.m_optionsMenueItem.Index = 0;
            this.m_optionsMenueItem.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
            this.m_optionsMenueItem.Text = "Options";
            this.m_optionsMenueItem.Click += new System.EventHandler(this.m_optionsMenueItem_Click);
            // 
            // m_menuItemHelp
            // 
            this.m_menuItemHelp.Index = 3;
            this.m_menuItemHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.m_menuWebpage,
            this.m_menuInfo});
            this.m_menuItemHelp.Text = "Help";
            // 
            // m_menuWebpage
            // 
            this.m_menuWebpage.Index = 0;
            this.m_menuWebpage.Text = "Webpage ...";
            this.m_menuWebpage.Click += new System.EventHandler(this.menuWebPage_Click);
            // 
            // m_menuInfo
            // 
            this.m_menuInfo.Index = 1;
            this.m_menuInfo.Text = "&Info";
            this.m_menuInfo.Click += new System.EventHandler(this.menuInfo_Click);
            // 
            // m_statusBar
            // 
            this.m_statusBar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_toolStripStatusLabel,
            this.m_toolStripProgressBar});
            this.m_statusBar.Location = new System.Drawing.Point(0, 486);
            this.m_statusBar.Name = "m_statusBar";
            this.m_statusBar.Size = new System.Drawing.Size(735, 36);
            this.m_statusBar.TabIndex = 0;
            this.m_statusBar.Resize += new System.EventHandler(this._statusBar_Resize);
            // 
            // m_toolStripStatusLabel
            // 
            this.m_toolStripStatusLabel.AutoSize = false;
            this.m_toolStripStatusLabel.Name = "m_toolStripStatusLabel";
            this.m_toolStripStatusLabel.Size = new System.Drawing.Size(720, 31);
            this.m_toolStripStatusLabel.Spring = true;
            this.m_toolStripStatusLabel.Text = "Ready...";
            this.m_toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // m_toolStripProgressBar
            // 
            this.m_toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.m_toolStripProgressBar.Name = "m_toolStripProgressBar";
            this.m_toolStripProgressBar.Size = new System.Drawing.Size(144, 30);
            this.m_toolStripProgressBar.Visible = false;
            // 
            // m_groupBoxSystemView
            // 
            this.m_groupBoxSystemView.AutoSize = true;
            this.m_groupBoxSystemView.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_groupBoxSystemView.Controls.Add(this.m_label);
            this.m_groupBoxSystemView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_groupBoxSystemView.Location = new System.Drawing.Point(0, 0);
            this.m_groupBoxSystemView.Margin = new System.Windows.Forms.Padding(2);
            this.m_groupBoxSystemView.Name = "m_groupBoxSystemView";
            this.m_groupBoxSystemView.Padding = new System.Windows.Forms.Padding(2);
            this.m_groupBoxSystemView.Size = new System.Drawing.Size(122, 459);
            this.m_groupBoxSystemView.TabIndex = 1;
            this.m_groupBoxSystemView.TabStop = false;
            this.m_groupBoxSystemView.Text = "Hierarchy";
            // 
            // m_label
            // 
            this.m_label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_label.AutoSize = true;
            this.m_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_label.Location = new System.Drawing.Point(12, 185);
            this.m_label.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.m_label.Name = "m_label";
            this.m_label.Size = new System.Drawing.Size(182, 25);
            this.m_label.TabIndex = 0;
            this.m_label.Text = "No view selected!";
            this.m_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_groupBox
            // 
            this.m_groupBox.AutoSize = true;
            this.m_groupBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.m_groupBox.Controls.Add(this.m_tabControlMain);
            this.m_groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_groupBox.Location = new System.Drawing.Point(0, 0);
            this.m_groupBox.Margin = new System.Windows.Forms.Padding(2);
            this.m_groupBox.Name = "m_groupBox";
            this.m_groupBox.Padding = new System.Windows.Forms.Padding(2);
            this.m_groupBox.Size = new System.Drawing.Size(582, 459);
            this.m_groupBox.TabIndex = 2;
            this.m_groupBox.TabStop = false;
            this.m_groupBox.Text = "Details";
            // 
            // m_tabControlMain
            // 
            this.m_tabControlMain.Location = new System.Drawing.Point(104, 76);
            this.m_tabControlMain.Margin = new System.Windows.Forms.Padding(2);
            this.m_tabControlMain.Name = "m_tabControlMain";
            this.m_tabControlMain.SelectedIndex = 0;
            this.m_tabControlMain.ShowToolTips = true;
            this.m_tabControlMain.Size = new System.Drawing.Size(342, 240);
            this.m_tabControlMain.TabIndex = 1;
            // 
            // m_splitContainer
            // 
            this.m_splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_splitContainer.Location = new System.Drawing.Point(14, 32);
            this.m_splitContainer.Margin = new System.Windows.Forms.Padding(2);
            this.m_splitContainer.Name = "m_splitContainer";
            // 
            // m_splitContainer.Panel1
            // 
            this.m_splitContainer.Panel1.Controls.Add(this.m_groupBoxSystemView);
            // 
            // m_splitContainer.Panel2
            // 
            this.m_splitContainer.Panel2.Controls.Add(this.m_groupBox);
            this.m_splitContainer.Size = new System.Drawing.Size(708, 459);
            this.m_splitContainer.SplitterDistance = 122;
            this.m_splitContainer.TabIndex = 3;
            // 
            // m_toolStrip
            // 
            this.m_toolStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.m_toolStrip.Location = new System.Drawing.Point(0, 0);
            this.m_toolStrip.Name = "m_toolStrip";
            this.m_toolStrip.Size = new System.Drawing.Size(735, 25);
            this.m_toolStrip.TabIndex = 4;
            this.m_toolStrip.Text = "toolStrip1";
            this.m_toolStrip.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.m_toolStrip_ItemClicked);
            // 
            // MainWindowApplicationDLG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(735, 522);
            this.Controls.Add(this.m_toolStrip);
            this.Controls.Add(this.m_splitContainer);
            this.Controls.Add(this.m_statusBar);
            this.HelpButton = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Menu = this.m_mainMenu;
            this.Name = "MainWindowApplicationDLG";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SMARTDataBees";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindowApplicationDLG_FormClosing);
            this.Load += new System.EventHandler(this.MainWindowApplicationDLG_Load);
            this.Shown += new System.EventHandler(this.MainWindowApplicationDLG_Shown);
            this.m_statusBar.ResumeLayout(false);
            this.m_statusBar.PerformLayout();
            this.m_groupBoxSystemView.ResumeLayout(false);
            this.m_groupBoxSystemView.PerformLayout();
            this.m_groupBox.ResumeLayout(false);
            this.m_splitContainer.Panel1.ResumeLayout(false);
            this.m_splitContainer.Panel1.PerformLayout();
            this.m_splitContainer.Panel2.ResumeLayout(false);
            this.m_splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitContainer)).EndInit();
            this.m_splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_components != null)
                {
                    m_components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        public MenuItem MenuProject()
        {
            return m_menuItemProject;
        }

        /// <summary>
        /// Returns the Admin-Menue
        /// </summary>
        public MenuItem MenueAdmin()
        {
            return m_menuItemAdmin;
        }

        private delegate void InvokeHelper();
        /// <summary>
        /// Shows the main dialog. Use this instead of ShowDialog diectly! Threadsave
        /// </summary>
        [STAThread]
        public void ShowMainDialog()
        {
            if (this != null)
            {
                if(InvokeRequired)
                {
                    InvokeHelper d = ShowMainDialog;
                    Invoke(d);
                }
                else 
                {
                    if (!Visible)
                    {
                        ShowDialog();
                    }
                }
            }
        }

        /// <summary>
        /// Returns the Tools-Menue
        /// </summary>
        public MenuItem MenueTools()
        {
            return m_menuItemTools;
        }

        /// <summary>
        /// Returns the Help menu
        /// </summary>
        /// <returns></returns>
        public MenuItem MenueHelp()
        {
            return m_menuItemHelp;
        }

        public GroupBox SystemView()
        {
            return m_groupBoxSystemView;
        }

        /// <summary>
        /// Returns the main ToolStrip
        /// </summary>
        public ToolStrip ToolStripMainWindow()
        {
            return m_toolStrip;
        }

        public TabControl MyTabControl()
        {
            return m_tabControlMain;
        }

        /// <summary>
        /// Returns the TabPage for the PluginControl, if typeName is "", then its the basic properties Tabpage
        /// </summary>
        public TabPage TabPagePlugin(string typeName)
        {
            foreach (TabPage tabPage in MyTabControl().TabPages)
            {
                if (tabPage.Name == typeName)
                {
                    return tabPage;
                }
            }

            return null;
        }



        /// <summary>
        /// Returns the Main Form
        /// </summary>
        public MainWindowApplicationDLG MyForm
        {
            get { return this; }
        }

        /// <summary>
        /// Get the Statusbar
        /// </summary>
        public ToolStripProgressBar ProgressBar
        {
            get { return m_toolStripProgressBar; }
        }

        /// <summary>
        /// Display a message in the status bar
        /// </summary>
        /// <param name="message"></param>
        public void WriteStatus(string message)
        {
            m_toolStripStatusLabel.Text = message;
            m_toolStripStatusLabel.Invalidate();

            Application.DoEvents();
        }

        #region Events

        private void MainWindowApplicationDLG_FormClosing(object sender, FormClosingEventArgs e)
        {
            //sending ReturnFromEdit
            //if (SDBees.Core.Connectivity.ConnectivityManager.Current.MyExternalPluginService != null)
            //    SDBees.Core.Connectivity.ConnectivityManager.Current.MyExternalPluginService.ReturnFromEdit(null);
        }

        private void m_optionsMenueItem_Click(object sender, EventArgs e)
        {
            var confWindow = new XmlConfigurationPropertiesWindow();
            confWindow.Text = "Options";
            confWindow.XmlConfigurationView.TabPageXmlHide();

            if(ConfigurationProvidersManager.ShowConfigurationWindow(this, confWindow) == DialogResult.OK)
            {

            }
        }

        private void MainWindowApplicationDLG_Load(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            //Laden der Einstellungen
            /*
            Carbon.Configuration.XmlConfiguration _xmlConf = new Carbon.Configuration.XmlConfiguration();
            Carbon.Configuration.FileEncryptionEngine _fileEncrypt = new Carbon.Configuration.FileEncryptionEngine();
            Carbon.Configuration.XmlConfigurationCategoryEventArgs _evtArgs = new Carbon.Configuration.XmlConfigurationCategoryEventArgs(
            Carbon.Configuration.XmlConfigurationCategoryEventHandler _evtHandler = 
              new Carbon.Configuration.XmlConfigurationCategoryEventHandler(
            Carbon.Configuration.Providers.ConfigurationProvidersManager.ReadOrCreateConfiguration(false, "MainWindow", "c:\\Temp",
                                                                             _xmlConf, _fileEncrypt
             * */
            //Anlegen der Menüs
            //SetMenues();

            //Anlegen der Buttons
        }

        /// <summary>
        /// Calls the SMARTDataBees Webpage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuWebPage_Click(object sender, EventArgs e)
        {
            MainWindowApplication.Current.ShowWebpage();

            ////Load expected window title
            //string _webpage = System.Configuration.ConfigurationManager.AppSettings["Webpage"];
            //if (!String.IsNullOrEmpty(_webpage))
            //{
            //    Process.Start(_webpage);
            //}
        }

        /// <summary>
        /// Display the info (about) dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuInfo_Click(object sender, EventArgs e)
        {
            MainWindowApplication.Current.ShowInfo();

            //Assembly assembly1 = Assembly.GetExecutingAssembly();
            //Assembly assembly2 = Assembly.GetEntryAssembly();

            //AboutBox dlg = new AboutBox(assembly2);

            //dlg.ShowDialog();
        }

        private void MainWindowApplicationDLG_Shown(object sender, EventArgs e)
        {
#if _DEBUG
      Console.WriteLine("Hauptfenster wird angezeigt");
#endif
        }

        private void _treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //Event an alle Registrierten Treenode-Plugins schicken, damit das Control angezeigt werden kann

            //Event an alle Treenode-Helper schicken, um die relevanten Daten zum Treenode-Plugin zu setzen
        }

        private void _viewSelector_Click(object sender, EventArgs e)
        {

        }

        private void menuItemClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _statusBar_Resize(object sender, EventArgs e)
        {
            var statusBarWidth = m_statusBar.ClientSize.Width;
            var progressBarWidth = m_toolStripProgressBar.Width;
            if (statusBarWidth > progressBarWidth)
            {
                m_toolStripStatusLabel.Width = statusBarWidth - progressBarWidth - 15;
            }
        }

        #endregion

        private void m_toolStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }

    /// <summary>
    /// Class für die Events des MainWindow
    /// </summary>
    /// 
    public delegate void SampleEventDelegate(object sender, EventArgs e);

    public class MainWindowApplicationDLGEvents
    {
        public event SampleEventDelegate SampleEvent;
    }
}
