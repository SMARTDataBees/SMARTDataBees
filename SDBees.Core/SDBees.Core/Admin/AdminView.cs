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
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Global;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateMenue;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;

namespace SDBees.Core.Admin
{
    /// <summary>
    /// ViewAdmin
    /// Klasse für das Handling der Baumstrukturen im Treeview
    /// Steuert die Sichtbarkeit der Knoten und deren Anordnung
    /// </summary>
    [PluginName("ViewAdminManager Plugin")]
    [PluginAuthors("Tim Hoffeller, Gamal Kira")]
    [PluginDescription("Plugin for the View management of SmartDataBees")]
    [PluginId("5770E5C9-5386-4CCA-AC7F-FC7EB8A4B896")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBeesDBConnection))]
    [PluginDependency(typeof(MainWindowApplication))]
    [PluginDependency(typeof(GlobalManager))]

    public class AdminView : TemplateMenue
    {
        private readonly MenuItem _mnuItemViewAdmin;
        private readonly ToolStripComboBox _puViewSelector;
        private Guid _curViewId;
        private ViewRelationTreeView _treeView;
        private ViewRelationTreeDLG _viewRelDLG;
        private PluginContext _context;

        public Guid CurrentViewId
        {
            get => _curViewId;
            set => _curViewId = value;
        }

        public AdminView()
        {
            Current = this;
            _mnuItemViewAdmin = new MenuItem("View Admin ...");
            _mnuItemViewAdmin.Click += _mnuItemViewManager_Click;
            _puViewSelector = new ToolStripComboBox("View")
            {
                Alignment = ToolStripItemAlignment.Left,
                Sorted = true,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat,
                Width = MainWindowApplicationDLG.m_Splitterdistance,
                DropDownWidth = MainWindowApplicationDLG.m_Splitterdistance
            };

            _puViewSelector.SelectedIndexChanged += OnSelectedIndexChanged;
        }

        public ViewRelationTreeDLG ViewRelationWindow() => _viewRelDLG;

        private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
        {
            if ((_treeView != null) && (DBManager != null))
            {
                Error error = null;
                var viewId = ViewProperty.GetViewIdFromName(DBManager.Database, _puViewSelector.Text, ref error);
                if (viewId != Guid.Empty)
                {
                    _treeView.ViewId = viewId;
                    _curViewId = viewId;
                    OnViewSelectionChanged?.Invoke(_puViewSelector, new ViewSelectionArgs(_puViewSelector.Text, viewId));
                }
            }
        }

        // The delegate procedure we are assigning to our object
        public delegate void ViewSelectedChangedHandler(object myObject, ViewSelectionArgs myArgs);

        public event ViewSelectedChangedHandler OnViewSelectionChanged;

        /// <summary>
        /// Occurs when the plugin starts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                Console.WriteLine(@"Viewadmin starts");
                _context = context;
                StartMe(context, e);
                InitDatabase();

                AdminDialog = new AdminDialog(this);
                if (MainWindow != null)
                {
                    MainWindow.TheDialog.MenueAdmin().MenuItems.Add(_mnuItemViewAdmin);
                    _mnuItemViewAdmin.Visible = SDBeesGlobalVars.GetViewAdminDisplayment();
                    CreateTreeView();
                }

                if (DBManager != null)
                    _viewRelDLG = new ViewRelationTreeDLG(DBManager);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void SelectFirstTreeNode()
        {
            if (_treeView.SelectedNode == null)
            {
                if (0 < _treeView.Nodes.Count)
                {
                    var firstTreeNode = _treeView.Nodes[0];
                    _treeView.SelectNode(firstTreeNode, true);
                }
            }
        }

        public void UpdatePropertyPages(TreeNode selectedNode)
        {
            var database = DBManager.Database;
            Error error = null;
            database.Open(false, ref error);

            // Zunächst die TemplateTreenodes benachrichtigen...
            TemplateTreenodeTag selectedTag = null;
            TemplateTreenodeTag parentTag = null;
            if (selectedNode != null)
            {
                selectedTag = (TemplateTreenodeTag)selectedNode.Tag;

                if (selectedNode.Parent != null)
                    parentTag = (TemplateTreenodeTag) selectedNode.Parent.Tag;
            }

            var allTreenodePlugins = TemplateTreenode.GetAllTreenodePlugins();
            foreach (var treenodePlugin in allTreenodePlugins)
            {
                treenodePlugin?.UpdatePropertyPage(selectedTag, parentTag);
            }

            // Jetzt die TemplateTreenodeHelper benachrichtigen...
            var allHelperPlugins = TemplateTreenodeHelper.GetAllHelperPlugins();
            foreach (var helperPlugin in allHelperPlugins)
            {
                if (helperPlugin != null)
                {
                    var tabPage = MainWindow.TheDialog.TabPagePlugin(helperPlugin.GetType().ToString());
                    if (tabPage != null)
                    {
                        helperPlugin.UpdatePropertyPage(tabPage, _treeView.ViewId, selectedTag, parentTag);
                    }
                }
            }

            database.Close(ref error);
        }

        private void CreateTreeView()
        {
            if (DBManager != null)
            {
                // Create the required database tables...
                var database = DBManager.Database;

                Error error = null;
                database.Open(false, ref error);

                FillViewSelector();
                MainWindow.TheDialog.ToolStripMainWindow().Items.Add(_puViewSelector);
                _puViewSelector.Width = MainWindowApplicationDLG.m_Splitterdistance;

                _treeView = new ViewRelationTreeView(DBManager);
                _treeView.AfterSelect += _treeView_AfterSelect;
                _treeView.ViewSwitched += _treeView_ViewSwitched;

                MainWindow.TheDialog.SystemView().Controls.Clear();
                MainWindow.TheDialog.SystemView().Controls.Add(_treeView);
                _treeView.Dock = DockStyle.Fill;

                CreateTabPropertyPages();

                var allTreenodePlugins = TemplateTreenode.GetAllTreenodePlugins();
                foreach (var treenodePlugin in allTreenodePlugins)
                {
                    if (treenodePlugin != null)
                    {
                        treenodePlugin.ObjectCreated += treenodePlugin_ObjectCreated;
                        treenodePlugin.ObjectModified += treenodePlugin_ObjectModified;
                        treenodePlugin.CreateViewRelation += treenodePlugin_CreateViewRelation;
                    }
                }

                database.Close(ref error);
            }
        }

        private void _treeView_ViewSwitched(object sender, ViewRelationTreeView.NotificationEventArgs args)
        {
            if (_puViewSelector.Text != args.ViewName)
            {
                SelectViewByName(args.ViewName);
            }
        }

        private void treenodePlugin_ObjectModified(object sender, TemplateTreenode.NotificationEventArgs args)
        {
            Error error = null;
            ViewRelation.UpdateViewRelationNames(args.BaseData.Database, args.BaseData.Name, new Guid(args.Tag.NodeGUID), ref error);
        }

        private void treenodePlugin_DeleteObjectClicked(object sender, TemplateTreenode.NotificationEventArgs args)
        {
            _treeView.DeleteSelectedNodes();
        }

        private void treenodePlugin_ObjectCreated(object sender, TemplateTreenode.NotificationEventArgs args)
        {
            // _treeView.AddViewRelation(args.Tag, args.ParentType, args.ParentId);
        }

        void treenodePlugin_CreateViewRelation(object sender, TemplateTreenode.ViewRelationEventArgs args)
        {
            var newTag = args.Tag;

            var database = DBManager.Database;
            Error error = null;
            if (ViewRelation.ChildReferencedByView(database, _curViewId, new Guid(newTag.NodeGUID), ref error))
            {
                // This child is already referenced in this view and cannot be referenced again
                return;
            }

            if (!ViewDefinition.ViewDefinitionExists(database, _curViewId, args.ParentType, newTag.NodeTypeOf, ref error))
            {
                MessageBox.Show($@"Relation not guilty ({args.ParentType} / {newTag.NodeTypeOf})");
                return;
            }

            //Die Relation einfügen
            var viewrel = new ViewRelation();
            viewrel.SetDefaults(database);
            viewrel.ViewId = _curViewId;
            viewrel.ChildId = new Guid(newTag.NodeGUID);
            viewrel.ChildType = newTag.NodeTypeOf;
            viewrel.ChildName = newTag.NodeName;
            viewrel.ParentId = args.ParentId;
            viewrel.ParentType = args.ParentType;

            viewrel.Save(ref error);

            Error.Display($"Can't create relation", error);
        }

        private void CreateTabPropertyPages()
        {
            // Jetzt das TabControl für Eigenschaften bzw. EDM, ... einrichten
            MainWindow.TheDialog.MyTabControl().TabPages.Clear();

            // Eine TabPage für die Grunddaten ist immer da...
            var tabPageProperties = new TabPage
            {
                Name = "",
                Text = @"Properties"
            };
            // Grunddaten muss diesen Namen haben!!!
            MainWindow.TheDialog.MyTabControl().TabPages.Add(tabPageProperties);

            // Für jeden Helper eine TabPage hinzufügen...
            var allHelperPlugins = TemplateTreenodeHelper.GetAllHelperPlugins();

            foreach (var pluginHelper in allHelperPlugins)
            {
                // Wenn der Helper kein Label hat, dann möchte dieser nicht dargestellt werden.
                if (pluginHelper.TabPageName() != "")
                {
                    var tabPageHelper = new TabPage
                    {
                        Name = pluginHelper.GetType().ToString(),
                        Text = pluginHelper.TabPageName()
                    };
                    // Grunddaten muss einen leeren namen haben!!!
                    MainWindow.TheDialog.MyTabControl().TabPages.Add(tabPageHelper);
                }
            }
        }

        void _treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                ViewCache.Enable();

                UpdatePropertyPages(e.Node);
            }
            finally
            {
                ViewCache.Disable();
            }
        }

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
           
        }

        /// <summary>
        /// Returns the one and only ViewAdminManager Plugin instance.
        /// </summary>
        public static AdminView Current { get; private set; }

        /// <summary>
        /// The Context for the loaded Plugin
        /// </summary>
        public PluginContext MyContext
        {
            get { return _context; }
        }

        public AdminDialog AdminDialog { get; private set; }

        private void FillViewSelector()
        {
            if (_treeView != null) _treeView.ViewId = Guid.Empty;

            _puViewSelector.Items.Clear();

            var database = DBManager.Database;
            if (database != null)
            {
                ArrayList viewNames = null;
                Error error = null;

                ViewProperty.GetAllViewNames(database, ref viewNames, ref error);

                foreach (string viewName in viewNames)
                {
                    _puViewSelector.Items.Add(viewName);
                }

                if (0 < _puViewSelector.Items.Count) _puViewSelector.SelectedIndex = 0;
            }

            _puViewSelector.Width = MainWindowApplicationDLG.m_Splitterdistance;
            _puViewSelector.DropDownWidth = MainWindowApplicationDLG.m_Splitterdistance;
        }

        private void SelectViewByName(string viewName)
        {
            var index = _puViewSelector.FindStringExact(viewName);
            if (index >= 0)
            {
                _puViewSelector.SelectedIndex = index;
            }
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            InitDatabase();
        }

        protected override void OnUpdate(object sender, EventArgs e)
        {
            FillViewSelector();

            AdminDialog.RefreshView();
            SelectFirstTreeNode();
        }

        private void InitDatabase()
        {
            // Das Databaseplugin besorgen
            if (DBManager != null)
            {
                // Create the required database tables...
                var database = DBManager.Database;

                Error error = null;
                database.Open(false, ref error);

                ViewRelation.InitTableSchema(database);
                ViewDefinition.InitTableSchema(database);
                ViewProperty.InitTableSchema(database);

                database.Close(ref error);
            }
        }

        #region Events

        /// <summary>
        /// Arguments for ViewRelations related events
        /// </summary>
        public class ViewRelationEventArgs
        {
            /// <summary>
            /// The view relation that initiated the event
            /// </summary>
            public ViewRelation ViewRelation;
            public bool NameChanged;

            /// <summary>
            /// Standard constructor
            /// </summary>
            /// <param name="viewRel"></param>
            public ViewRelationEventArgs(ViewRelation viewRel)
            {
                ViewRelation = viewRel;
                NameChanged = false;
            }
        }

        /// <summary>
        /// Eventhandler delegate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void ViewRelationEventHandler(object sender, ViewRelationEventArgs args);

        /// <summary>
        /// Triggered when a view relation has been created
        /// </summary>
        public event ViewRelationEventHandler ViewRelationCreated;

        /// <summary>
        /// Triggered when a view relation has been modified
        /// </summary>
        public event ViewRelationEventHandler ViewRelationModified;

        /// <summary>
        /// Triggered when a view relation has been removed
        /// </summary>
        public event ViewRelationEventHandler ViewRelationRemoved;

        internal void RaiseViewRelationCreated(ViewRelation viewRel)
        {
            if (ViewRelationCreated != null)
            {
                var args = new ViewRelationEventArgs(viewRel);

                ViewRelationCreated.Invoke(this, args);
            }
        }

        internal void RaiseViewRelationModified(ViewRelation viewRel, bool NameChanged)
        {
            if (ViewRelationModified != null)
            {
                var args = new ViewRelationEventArgs(viewRel);
                args.NameChanged = NameChanged;
                ViewRelationModified.Invoke(this, args);
            }
        }

        internal void RaiseViewRelationRemoved(ViewRelation viewRel)
        {
            if (ViewRelationRemoved != null)
            {
                var args = new ViewRelationEventArgs(viewRel);
                ViewRelationRemoved.Invoke(this, args);
            }
        }

        #endregion

        #region MyEvents
        void _mnuItemViewManager_Click(object sender, EventArgs e)
        {
            AdminDialog.ShowDialog();

            // Refill the view list, but get the last setting first...
            var lastViewName = _puViewSelector.Text;

            FillViewSelector();

            if (string.IsNullOrEmpty(AdminDialog.CurrentViewIdentification) == false)
                SelectViewByName(lastViewName);
        }

        #endregion

    }
}
