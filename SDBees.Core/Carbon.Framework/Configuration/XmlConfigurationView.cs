//	============================================================================
//
//  .,-:::::   :::.    :::::::..   :::::::.      ...   :::.    :::.
//	,;;;'````'   ;;`;;   ;;;;``;;;;   ;;;'';;'  .;;;;;;;.`;;;;,  `;;;
//	[[[         ,[[ '[[,  [[[,/[[['   [[[__[[\.,[[     \[[,[[[[[. '[[
//	$$$        c$$$cc$$$c $$$$$$c     $$""""Y$$$$$,     $$$$$$ "Y$c$$
//	`88bo,__,o, 888   888,888b "88bo,_88o,,od8P"888,_ _,88P888    Y88
//	"YUMMMMMP"YMM   ""` MMMM   "W" ""YUMMMP"   "YMMMMMP" MMM     YM
//
//	============================================================================
//
//	This file is a part of the Carbon Framework.
//
//	Copyright (C) 2005 Mark (Code6) Belles 
//
//	This library is free software; you can redistribute it and/or
//	modify it under the terms of the GNU Lesser General Public
//	License as published by the Free Software Foundation; either
//	version 2.1 of the License, or (at your option) any later version.
//
//	This library is distributed in the hope that it will be useful,
//	but WITHOUT ANY WARRANTY; without even the implied warranty of
//	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//	Lesser General Public License for more details.
//
//	You should have received a copy of the GNU Lesser General Public
//	License along with this library; if not, write to the Free Software
//	Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
//	============================================================================

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Carbon.Common;

namespace Carbon.Configuration
{
    /// <summary>
    /// Summary description for XmlConfigurationView.
    /// </summary>
    public class XmlConfigurationView : UserControl
    {
        /// <summary>
        /// Defines the indexes of the images associated with categories displayed in the treeview
        /// </summary>
        public enum CategoryImageIndexes
        {
            /// <summary>
            /// No image
            /// </summary>
            NoImage = 3,

            /// <summary>
            /// The category is unselected with sub categories (displayed as a closed folder)
            /// </summary>
            UnselectedWithSubCategories = 0,

            /// <summary>
            /// The category is selected with sub categories (displayed as an open folder)
            /// </summary>
            SelectedWithSubCategories = 1,

            /// <summary>
            /// The category is unselected without sub categories (displayed with no image)
            /// </summary>
            UnselectedWithoutSubCategories = NoImage,

            /// <summary>
            /// The category is selected without sub categories (displayed with an arrow)
            /// </summary>
            SelectedWithoutSubCategories = 2
        }

        /// <summary>
        /// The configurations that are currently selected into the control
        /// </summary>
        private XmlConfigurationCollection _selectedConfigurations;
        private bool _placeElementsIntoEditMode;

        public event XmlConfigurationElementEventHandler ConfigurationChanged;

        /// <summary>
        /// The various controls that are used to display the configurations
        /// </summary>
        private TabControl tabControlMain;
        private TabPage tabPagePropertyPages;
        private Panel rootPanel;
        private Panel optionsPanel;
        private Splitter _splitter;
        private Panel categoriesPanel;
        private TreeView _treeView;
        private TabPage tabPageXml;
        private TabControl tabControlXmlViews;
        private ImageList _imageList;
        private ContextMenu _contextMenu;
        private Label _labelCategory;
        private PropertyGrid _propertyGrid;
        private IContainer components;

        /// <summary>
        /// Initializes a new instance of the XmlConfigurationView class
        /// </summary>
        public XmlConfigurationView()
        {
            InitializeComponent();

            _selectedConfigurations = new XmlConfigurationCollection();

            // property grid
            _propertyGrid.HelpVisible = true;
            _propertyGrid.ToolbarVisible = true;

            // splitter
            _splitter.MouseEnter += OnMouseEnterSplitter;
            _splitter.MouseLeave += OnMouseLeaveSplitter;

            // treeview
            _treeView.AfterSelect += OnAfterNodeSelected;
            _treeView.ImageList = _imageList;

            _placeElementsIntoEditMode = true;

            _contextMenu.Popup += OnGridContextMenuPoppedUp;
            //			this.ClearNodes();
            //			this.ClearXmlTabPages();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlConfigurationView));
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPagePropertyPages = new System.Windows.Forms.TabPage();
            this.rootPanel = new System.Windows.Forms.Panel();
            this.optionsPanel = new System.Windows.Forms.Panel();
            this._propertyGrid = new System.Windows.Forms.PropertyGrid();
            this._contextMenu = new System.Windows.Forms.ContextMenu();
            this._labelCategory = new System.Windows.Forms.Label();
            this._splitter = new System.Windows.Forms.Splitter();
            this.categoriesPanel = new System.Windows.Forms.Panel();
            this._treeView = new System.Windows.Forms.TreeView();
            this.tabPageXml = new System.Windows.Forms.TabPage();
            this.tabControlXmlViews = new System.Windows.Forms.TabControl();
            this._imageList = new System.Windows.Forms.ImageList(this.components);
            this.tabControlMain.SuspendLayout();
            this.tabPagePropertyPages.SuspendLayout();
            this.rootPanel.SuspendLayout();
            this.optionsPanel.SuspendLayout();
            this.categoriesPanel.SuspendLayout();
            this.tabPageXml.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPagePropertyPages);
            this.tabControlMain.Controls.Add(this.tabPageXml);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Multiline = true;
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(470, 260);
            this.tabControlMain.TabIndex = 5;
            // 
            // tabPagePropertyPages
            // 
            this.tabPagePropertyPages.Controls.Add(this.rootPanel);
            this.tabPagePropertyPages.Location = new System.Drawing.Point(4, 22);
            this.tabPagePropertyPages.Name = "tabPagePropertyPages";
            this.tabPagePropertyPages.Size = new System.Drawing.Size(462, 234);
            this.tabPagePropertyPages.TabIndex = 0;
            this.tabPagePropertyPages.Text = "Property Pages";
            // 
            // rootPanel
            // 
            this.rootPanel.Controls.Add(this.optionsPanel);
            this.rootPanel.Controls.Add(this._splitter);
            this.rootPanel.Controls.Add(this.categoriesPanel);
            this.rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootPanel.Location = new System.Drawing.Point(0, 0);
            this.rootPanel.Name = "rootPanel";
            this.rootPanel.Size = new System.Drawing.Size(462, 234);
            this.rootPanel.TabIndex = 1;
            // 
            // optionsPanel
            // 
            this.optionsPanel.Controls.Add(this._propertyGrid);
            this.optionsPanel.Controls.Add(this._labelCategory);
            this.optionsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsPanel.Location = new System.Drawing.Point(205, 0);
            this.optionsPanel.Name = "optionsPanel";
            this.optionsPanel.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.optionsPanel.Size = new System.Drawing.Size(257, 234);
            this.optionsPanel.TabIndex = 4;
            // 
            // _propertyGrid
            // 
            this._propertyGrid.BackColor = System.Drawing.SystemColors.Control;
            this._propertyGrid.ContextMenu = this._contextMenu;
            this._propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this._propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this._propertyGrid.Location = new System.Drawing.Point(3, 20);
            this._propertyGrid.Name = "_propertyGrid";
            this._propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this._propertyGrid.Size = new System.Drawing.Size(254, 214);
            this._propertyGrid.TabIndex = 5;
            // 
            // _labelCategory
            // 
            this._labelCategory.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this._labelCategory.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._labelCategory.Dock = System.Windows.Forms.DockStyle.Top;
            this._labelCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._labelCategory.ForeColor = System.Drawing.SystemColors.HighlightText;
            this._labelCategory.Location = new System.Drawing.Point(3, 0);
            this._labelCategory.Name = "_labelCategory";
            this._labelCategory.Size = new System.Drawing.Size(254, 20);
            this._labelCategory.TabIndex = 4;
            // 
            // _splitter
            // 
            this._splitter.BackColor = System.Drawing.SystemColors.Control;
            this._splitter.Location = new System.Drawing.Point(200, 0);
            this._splitter.Name = "_splitter";
            this._splitter.Size = new System.Drawing.Size(5, 234);
            this._splitter.TabIndex = 3;
            this._splitter.TabStop = false;
            // 
            // categoriesPanel
            // 
            this.categoriesPanel.Controls.Add(this._treeView);
            this.categoriesPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.categoriesPanel.Location = new System.Drawing.Point(0, 0);
            this.categoriesPanel.Name = "categoriesPanel";
            this.categoriesPanel.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.categoriesPanel.Size = new System.Drawing.Size(200, 234);
            this.categoriesPanel.TabIndex = 2;
            // 
            // _treeView
            // 
            this._treeView.BackColor = System.Drawing.SystemColors.Window;
            this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._treeView.Location = new System.Drawing.Point(0, 0);
            this._treeView.Name = "_treeView";
            this._treeView.ShowLines = false;
            this._treeView.Size = new System.Drawing.Size(197, 234);
            this._treeView.Sorted = true;
            this._treeView.TabIndex = 0;
            // 
            // tabPageXml
            // 
            this.tabPageXml.Controls.Add(this.tabControlXmlViews);
            this.tabPageXml.Location = new System.Drawing.Point(4, 22);
            this.tabPageXml.Name = "tabPageXml";
            this.tabPageXml.Size = new System.Drawing.Size(462, 234);
            this.tabPageXml.TabIndex = 1;
            this.tabPageXml.Text = "Xml";
            this.tabPageXml.Visible = false;
            // 
            // tabControlXmlViews
            // 
            this.tabControlXmlViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlXmlViews.Location = new System.Drawing.Point(0, 0);
            this.tabControlXmlViews.Name = "tabControlXmlViews";
            this.tabControlXmlViews.SelectedIndex = 0;
            this.tabControlXmlViews.Size = new System.Drawing.Size(462, 234);
            this.tabControlXmlViews.TabIndex = 0;
            // 
            // _imageList
            // 
            this._imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_imageList.ImageStream")));
            this._imageList.TransparentColor = System.Drawing.Color.Transparent;
            this._imageList.Images.SetKeyName(0, "");
            this._imageList.Images.SetKeyName(1, "");
            this._imageList.Images.SetKeyName(2, "");
            this._imageList.Images.SetKeyName(3, "");
            this._imageList.Images.SetKeyName(4, "");
            this._imageList.Images.SetKeyName(5, "");
            this._imageList.Images.SetKeyName(6, "");
            this._imageList.Images.SetKeyName(7, "");
            // 
            // XmlConfigurationView
            // 
            this.Controls.Add(this.tabControlMain);
            this.Name = "XmlConfigurationView";
            this.Size = new System.Drawing.Size(470, 260);
            this.tabControlMain.ResumeLayout(false);
            this.tabPagePropertyPages.ResumeLayout(false);
            this.rootPanel.ResumeLayout(false);
            this.optionsPanel.ResumeLayout(false);
            this.categoriesPanel.ResumeLayout(false);
            this.tabPageXml.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        #region Overrides

        /// <summary>
        /// Override the parent changing event, and bind to the parent forms
        /// </summary>
        /// <param name="e"></param>
        protected override void OnParentChanged(EventArgs e)
        {
            if (ParentForm != null)
                ParentForm.Closed += OnParentFormClosed;

            base.OnParentChanged(e);
        }

        #endregion

        #region Public Methods
        public void TabPageXmlHide()
        {
            if (tabControlMain.TabPages.Contains(tabPageXml))
                tabControlMain.TabPages.Remove(tabPageXml);
        }

        public void TabPageXmlShow()
        {
            ShowTabPage(tabPageXml, tabControlMain.TabPages.Count);
        }

        private delegate void AddConfigurationInvoker(XmlConfiguration configuration);

        /// <summary>
        /// Adds the specified configuration to the selected configurations displayed by this control
        /// </summary>
        /// <param name="configuration"></param>
        public void AddConfiguration(XmlConfiguration configuration)
        {
            if (InvokeRequired)
            {
                Invoke(new AddConfigurationInvoker(AddConfiguration), configuration);
                return;
            }

            try
            {
                if (configuration != null)
                {
                    configuration.Changed += OnConfigurationChanged;
                    if (_placeElementsIntoEditMode)
                        configuration.BeginEdit();
                    _selectedConfigurations.Add(configuration);
                    AddNodesForCategories(_treeView, null, configuration.Categories);
                    AddXmlTabForConfiguration(configuration);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private delegate void RemoveConfigurationInvoker(XmlConfiguration configuration, bool keepLocationIfPossible);

        /// <summary>
        /// Removes the specified configuration from the selected configurations diplayed by this control, optionally attempts to restore the location
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="keepLocationIfPossible"></param>
        public void RemoveConfiguration(XmlConfiguration configuration, bool keepLocationIfPossible)
        {
            if (InvokeRequired)
            {
                Invoke(new RemoveConfigurationInvoker(RemoveConfiguration), configuration, keepLocationIfPossible);
                return;
            }

            try
            {
                if (configuration != null)
                {
                    if (_selectedConfigurations.Contains(configuration))
                    {
                        string path = null;
                        var n = _treeView.SelectedNode;
                        if (n != null)
                            path = n.FullPath;

                        _selectedConfigurations.Remove(configuration);

                        RefreshDisplay(path, keepLocationIfPossible);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private delegate bool SelectPathInvoker(string path);

        /// <summary>
        /// Attempts to select the node specified by the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool SelectPath(string path)
        {
            if (InvokeRequired)
            {
                return (bool)Invoke(new SelectPathInvoker(SelectPath), path);
            }

            try
            {
                return SelectPathFromNodes(_treeView.Nodes, path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return false;
        }

        /// <summary>
        /// Refreshes the categories and the selected category and the options contained therein
        /// </summary>
        public void RefreshDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(RefreshDisplay), new object[] { });
                return;
            }

            try
            {
                string path = null;
                var n = _treeView.SelectedNode;
                if (n != null)
                    path = n.FullPath;
                RefreshDisplay(path, true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private delegate void RefreshDisplayInvoker(string path, bool keepLocationIfPossible);

        /// <summary>
        /// Refreshes the display, and conditionally selects the specified path if possible
        /// </summary>
        /// <param name="path"></param>
        /// <param name="keepLocationIfPossible"></param>
        public void RefreshDisplay(string path, bool keepLocationIfPossible)
        {
            if (InvokeRequired)
            {
                Invoke(new RefreshDisplayInvoker(RefreshDisplay), path, keepLocationIfPossible);
                return;
            }

            try
            {
                ClearNodes();
                ClearXmlTabPages();

                foreach (XmlConfiguration configuration in _selectedConfigurations)
                {
                    AddNodesForCategories(_treeView, null, configuration.Categories);
                    AddXmlTabForConfiguration(configuration);
                }

                if (keepLocationIfPossible)
                    SelectPath(path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Refreshes the Xml view of the selected configurations
        /// </summary>
        public void RefreshXmlDisplay()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(RefreshXmlDisplay), new object[] { });
                return;
            }

            try
            {
                ClearXmlTabPages();

                foreach (XmlConfiguration configuration in _selectedConfigurations)
                    AddXmlTabForConfiguration(configuration);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Applies the changes of the selected configuration to the original configuration
        /// </summary>
        public void ApplyChanges()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ApplyChanges), new object[] { });
                return;
            }

            foreach (XmlConfiguration configuration in _selectedConfigurations)
            {
                if (configuration.IsBeingEdited)
                    configuration.EndEdit();

                if (_placeElementsIntoEditMode)
                    configuration.BeginEdit();
            }
        }

        /// <summary>
        /// Cancels the changes to the selected configuration
        /// </summary>
        public void CancelEdit()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(CancelEdit), new object[] { });
                return;
            }

            foreach (XmlConfiguration configuration in _selectedConfigurations)
                if (configuration.IsBeingEdited)
                    configuration.CancelEdit();
        }

        /// <summary>
        /// Displays a warning if any of the current configurations do not have sufficient security access permissions for write access on the local computer for the current user
        /// </summary>
        public void DisplayWarningIfLocalFilePermissionsAreInsufficient()
        {
            if (_selectedConfigurations == null)
                return;

            try
            {
                var anyAreDeniedWriteAccess = false;
                foreach (XmlConfiguration configuration in _selectedConfigurations)
                {
                    var path = configuration.Path;
                    if (path != null && path != string.Empty)
                    {
                        if (File.Exists(path))
                        {
                            using (var right = new SecurityAccessRight(path))
                            {
                                var noWrite = right.AssertWriteAccess();
                                if (!noWrite)
                                    anyAreDeniedWriteAccess = true;
                            }
                        }
                        else
                        {
                            var dir = Path.GetDirectoryName(path);
                            using (var right = new SecurityAccessRight(dir))
                            {
                                var noWrite = right.AssertWriteAccess();
                                if (!noWrite)
                                    anyAreDeniedWriteAccess = true;
                            }
                        }
                    }
                }

                if (anyAreDeniedWriteAccess)
                {
                    ExceptionUtilities.DisplayException(
                        this,
                        "Security restriction detected - Write access is denied to one or more configuration files",
                        MessageBoxIcon.Information,
                        MessageBoxButtons.OK,
                        null,
                        "One or more of the configuration files that store the options you are about to configure, has been denied write access for the current user '" + Environment.UserName + "'.",
                        "You may continue and make changes to the options as normal, however some options may not be saved when the appliation exits.",
                        "Please contact your system administrator for questions regarding Windows security and access rights.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region Private Methods


        private void ShowTabPage(TabPage tp, int index)
        {
            if (tabControlMain.TabPages.Contains(tp)) return;
            InsertTabPage(tp, index);
        }


        private void InsertTabPage(TabPage tabpage, int index)
        {
            if (index < 0 || index > tabControlMain.TabCount)
                throw new ArgumentException("Index out of Range.");
            tabControlMain.TabPages.Add(tabpage);
            if (index < tabControlMain.TabCount - 1)
            {
                do
                {
                    SwapTabPages(tabpage, (tabControlMain.TabPages[tabControlMain.TabPages.IndexOf(tabpage) - 1]));
                }
                while (tabControlMain.TabPages.IndexOf(tabpage) != index);
            }
            tabControlMain.SelectedTab = tabpage;
        }

        private void SwapTabPages(TabPage tp1, TabPage tp2)
        {
            if (tabControlMain.TabPages.Contains(tp1) == false || tabControlMain.TabPages.Contains(tp2) == false)
                throw new ArgumentException("TabPages must be in the TabControls TabPageCollection.");

            var Index1 = tabControlMain.TabPages.IndexOf(tp1);
            var Index2 = tabControlMain.TabPages.IndexOf(tp2);
            tabControlMain.TabPages[Index1] = tp2;
            tabControlMain.TabPages[Index2] = tp1;

            //Uncomment the following section to overcome bugs in the Compact Framework
            //tabControl1.SelectedIndex = tabControl1.SelectedIndex; 
            //string tp1Text, tp2Text;
            //tp1Text = tp1.Text;
            //tp2Text = tp2.Text;
            //tp1.Text=tp2Text;
            //tp2.Text=tp1Text;
        }
        /// <summary>
        /// Clears the nodes from the treeview
        /// </summary>
        private void ClearNodes()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ClearNodes), new object[] { });
                return;
            }

            try
            {
                _treeView.Nodes.Clear();
                _propertyGrid.SelectedObject = null;
                _labelCategory.Text = null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Clears the tab pages from the xml tab control
        /// </summary>
        private void ClearXmlTabPages()
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(ClearXmlTabPages), new object[] { });
                return;
            }

            try
            {
                foreach (TabPage page in tabControlXmlViews.TabPages)
                    tabControlXmlViews.TabPages.Remove(page);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private delegate bool SelectPathFromNodesInvoker(TreeNodeCollection nodes, string path);

        /// <summary>
        /// Recursively expands and selects nodes down to the last node found by searching the specified path
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool SelectPathFromNodes(TreeNodeCollection nodes, string path)
        {
            if (InvokeRequired)
            {
                return (bool)Invoke(new SelectPathFromNodesInvoker(SelectPathFromNodes), nodes, path);
            }

            var paths = path.Split('\\');

            if (paths.Length > 0)
            {
                if (nodes != null)
                {
                    foreach (TreeNode n in nodes)
                    {
                        if (string.Compare(n.Text, paths[0], true) == 0)
                        {
                            if (paths.Length == 1)
                            {
                                n.Expand();
                                _treeView.SelectedNode = n;
                                return true;
                            }
                            n.Expand();
                            path = string.Join("\\", paths, 1, paths.Length - 1);
                            return SelectPathFromNodes(n.Nodes, path);
                        }
                    }
                }
            }
            return false;
        }

        private delegate void AddNodesForCategoriesInvoker(TreeView tree, TreeNodeCollection nodes, XmlConfigurationCategoryCollection categories);

        /// <summary>
        /// Recursively adds nodes for the specified categories
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="nodes"></param>
        /// <param name="categories"></param>
        private void AddNodesForCategories(TreeView tree, TreeNodeCollection nodes, XmlConfigurationCategoryCollection categories)
        {
            if (InvokeRequired)
            {
                Invoke(new AddNodesForCategoriesInvoker(AddNodesForCategories), tree, nodes, categories);
                return;
            }

            tree.BeginUpdate();
            try
            {
                foreach (XmlConfigurationCategory category in categories)
                {
                    if (!category.Hidden)
                    {
                        // try and find an existing node that we can merge with
                        var n = FindNodeForCategory(nodes, category);

                        if (n == null)
                            n = AddNodeForCategory(tree, nodes, category);

                        if (n != null)
                        {
                            if (!n.IsBoundToCategory(category))
                                n.BindToCategory(category);

                            AddNodesForCategories(tree, n.Nodes, category.Categories);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                tree.EndUpdate();
            }
        }

        private delegate CategoryTreeNode FindNodeForCategoryInvoker(TreeNodeCollection nodes, XmlConfigurationCategory category);

        /// <summary>
        /// Finds an existing node for the specified category
        /// </summary>
        /// <param name="nodes"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private CategoryTreeNode FindNodeForCategory(TreeNodeCollection nodes, XmlConfigurationCategory category)
        {
            if (InvokeRequired)
            {
                return Invoke(new FindNodeForCategoryInvoker(FindNodeForCategory), nodes, category) as CategoryTreeNode;
            }

            if (nodes == null)
                // asume root
                nodes = _treeView.Nodes;

            if (nodes != null)
            {
                if (category != null)
                {
                    foreach (CategoryTreeNode n in nodes)
                    {
                        //						if (n.IsBoundToCategory(category))
                        //							return n;
                        if (string.Compare(n.Text, category.DisplayName, true) == 0)
                            return n;
                    }
                }
            }
            return null;
        }

        private delegate CategoryTreeNode AddNodeForCategoryInvoker(TreeView tree, TreeNodeCollection nodes, XmlConfigurationCategory category);

        /// <summary>
        /// Adds a category node into the treeview for the specified category
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="nodes"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private CategoryTreeNode AddNodeForCategory(TreeView tree, TreeNodeCollection nodes, XmlConfigurationCategory category)
        {
            if (InvokeRequired)
            {
                return Invoke(new AddNodeForCategoryInvoker(AddNodeForCategory), tree, nodes, category) as CategoryTreeNode;
            }

            var isRootCategory = (nodes == null);

            if (nodes == null)
                if (tree != null)
                    nodes = tree.Nodes;

            var n = new CategoryTreeNode(category.DisplayName);
            n.BindToCategory(category);
            nodes.Add(n);

            if (isRootCategory)
            {
                n.ImageIndex = (int)CategoryImageIndexes.UnselectedWithSubCategories;
                n.SelectedImageIndex = (int)CategoryImageIndexes.SelectedWithSubCategories;
            }
            else
            {
                // ok, off the root, now base images on whether the category has sub categories or not, 
                // and whether the category is selected
                if (category.Categories.Count > 0)
                {
                    n.ImageIndex = (int)CategoryImageIndexes.UnselectedWithSubCategories;
                    n.SelectedImageIndex = (int)CategoryImageIndexes.SelectedWithSubCategories;
                }
                else
                {
                    n.ImageIndex = (int)CategoryImageIndexes.NoImage;
                    n.SelectedImageIndex = (int)CategoryImageIndexes.SelectedWithoutSubCategories;
                }
            }

            return n;
        }

        private delegate void AddXmlTabForConfigurationInvoker(XmlConfiguration configuration);

        /// <summary>
        /// Adds an Xml tab for the specified configuration
        /// </summary>
        /// <param name="configuration"></param>
        private void AddXmlTabForConfiguration(XmlConfiguration configuration)
        {
            if (InvokeRequired)
            {
                Invoke(new AddXmlTabForConfigurationInvoker(AddXmlTabForConfiguration), configuration);
                return;
            }

            try
            {
                tabControlXmlViews.SuspendLayout();

                var view = new XmlConfigurationXmlBehindViewer();
                view.Xml = configuration.ToXml();

                var page = new TabPage(configuration.ElementName);
                page.Controls.Add(view);
                view.Parent = page;
                view.Dock = DockStyle.Fill;

                tabControlXmlViews.TabPages.Add(page);
                tabControlXmlViews.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private delegate void SelectPropertyDescriptorForNodeIntoPropertyGridInvoker(CategoryTreeNode node);

        /// <summary>
        /// Selects the node's type descriptor into the property grid
        /// </summary>
        /// <param name="n"></param>
        private void SelectPropertyDescriptorForNodeIntoPropertyGrid(CategoryTreeNode n)
        {
            if (InvokeRequired)
            {
                Invoke(new SelectPropertyDescriptorForNodeIntoPropertyGridInvoker(SelectPropertyDescriptorForNodeIntoPropertyGrid), n);
                return;
            }

            try
            {
                _propertyGrid.SelectedObject = null;
                _labelCategory.Text = null;

                if (n != null)
                {
                    if (n.Categories.Count > 0)
                    {

                        var td = new XmlConfigurationOptionCollectionTypeDescriptor(n.Categories);
                        if (td != null)
                            _propertyGrid.SelectedObject = td;

                        foreach (DictionaryEntry entry in n.Categories)
                        {
                            var category = entry.Value as XmlConfigurationCategory;
                            if (category != null)
                            {
                                _labelCategory.Text = category.DisplayName;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion

        #region Public Properties

        public bool PlaceElementsIntoEditMode
        {
            get
            {
                return _placeElementsIntoEditMode;
            }
            set
            {
                _placeElementsIntoEditMode = value;
            }
        }


        /// <summary>
        /// Gets or sets the selected configuration for the Configuration Window. 
        /// This configuration will reflect any and all changes made using the gui.
        /// Use this.OriginalConfiguration to obtain the original configuration unchanged.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public XmlConfigurationCollection SelectedConfigurations
        {
            get
            {
                return _selectedConfigurations;
            }
            set
            {
                ClearNodes();
                ClearXmlTabPages();
                _selectedConfigurations = new XmlConfigurationCollection();

                if (value != null)
                {
                    foreach (XmlConfiguration configuration in value)
                        AddConfiguration(configuration);
                }
            }
        }

        #endregion

        #region Private Event Handlers and Event Invokers

        /// <summary>
        /// Occurs when a configuration changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConfigurationChanged(object sender, XmlConfigurationElementEventArgs e)
        {
            try
            {
                if (ConfigurationChanged != null)
                    ConfigurationChanged(sender, e);

                //				System.Diagnostics.Debug.WriteLine(XmlConfiguration.DescribeElementChanging(e));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Occurs when our parent form closes, we should unwire from the configuration events		
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnParentFormClosed(object sender, EventArgs e)
        {
            if (_selectedConfigurations != null)
                foreach (XmlConfiguration configuration in _selectedConfigurations)
                    if (_placeElementsIntoEditMode)
                        configuration.Changed -= OnConfigurationChanged;
        }

        /// <summary>
        /// Occurs when the mouse enters the splitter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseEnterSplitter(object sender, EventArgs e)
        {
            _splitter.BackColor = SystemColors.ControlDark;
        }

        /// <summary>
        /// Occurs when the mouse leaves the splitter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseLeaveSplitter(object sender, EventArgs e)
        {
            _splitter.BackColor = SystemColors.Control;
        }

        /// <summary>
        /// Occurs after a tree node is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAfterNodeSelected(object sender, TreeViewEventArgs e)
        {
            SelectPropertyDescriptorForNodeIntoPropertyGrid(e.Node as CategoryTreeNode);
        }

        #endregion

        /// <summary>
        /// Occurs when the "Has changes" menu item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnToggleOptionHasChangesClicked(object sender, EventArgs e)
        {
            // grab one of our custom menu items
            var menuItem = sender as XmlConfigurationOptionPropertyDescriptorMenuItem;
            if (menuItem != null)
            {
                // and the option it points to
                var option = menuItem.Option;
                if (option != null)
                {
                    // toggle the check
                    option.HasChanges = !menuItem.Checked;

                    // if changed, then trigger the changed event for the option
                    if (option.HasChanges)
                        option.TriggerChange();
                }
            }
        }

        /// <summary>
        /// Occurs when the context menu is popped up for the property grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGridContextMenuPoppedUp(object sender, EventArgs e)
        {
            // grab the grid
            var grid = _propertyGrid;
            if (grid != null)
            {
                // grab the selected item
                if (grid.SelectedGridItem != null)
                {
                    var item = grid.SelectedGridItem;
                    if (item != null)
                    {
                        // grab the descriptor as one of our option descriptors
                        var descriptor = item.PropertyDescriptor as XmlConfigurationOptionPropertyDescriptor;
                        if (descriptor != null)
                        {
                            var option = descriptor.Option;
                            if (option != null)
                            {
                                // construct a new menu item for it
                                var menuItem = new XmlConfigurationOptionPropertyDescriptorMenuItem("Has changes", OnToggleOptionHasChangesClicked, option);
                                // determine its checked state
                                menuItem.Checked = option.HasChanges;
                                // rinse and repeat								
                                _contextMenu.MenuItems.Clear();
                                _contextMenu.MenuItems.Add(menuItem);
                            }
                        }
                    }
                }
            }
        }
    }
}
