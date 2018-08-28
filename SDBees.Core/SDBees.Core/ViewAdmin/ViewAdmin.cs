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
using System.Collections.Specialized;
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Global;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateMenue;
using SDBees.Plugs.TemplateTreeNode;
using SDBees.Plugs.TreenodeHelper;
using Attribute = SDBees.DB.Attribute;
using Object = SDBees.DB.Object;

namespace SDBees.ViewAdmin
{
    /// <summary>
    /// ViewAdmin
    /// Klasse f�r das Handling der Baumstrukturen im Treeview
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

    public class ViewAdmin : TemplateMenue
    {
        private static ViewAdmin _theInstance;
        private readonly MenuItem _mnuItemViewAdmin;
        private readonly ToolStripComboBox _puViewSelector;
        private ViewAdminDLG _dlgViewAdmin;
        private Guid _curViewId;
        private ViewRelationTreeView _treeView;
        private ViewRelationTreeDLG _viewRelDLG;
        private PluginContext _context;

        public Guid CurrentViewId
        {
            get => _curViewId;
            set => _curViewId = value;
        }

        public ViewAdmin()
        {
            _theInstance = this;
            _mnuItemViewAdmin = new MenuItem("View Admin ...");
            _mnuItemViewAdmin.Click += _mnuItemViewManager_Click;
            //_mnuItem.

            _puViewSelector = new ToolStripComboBox("View");
            _puViewSelector.Alignment = ToolStripItemAlignment.Left;
            _puViewSelector.Sorted = true;
            _puViewSelector.DropDownStyle = ComboBoxStyle.DropDownList;
            _puViewSelector.FlatStyle = FlatStyle.Flat;
            _puViewSelector.Width = MainWindowApplicationDLG.m_Splitterdistance;
            _puViewSelector.DropDownWidth = MainWindowApplicationDLG.m_Splitterdistance;

            _puViewSelector.SelectedIndexChanged += _puViewSelector_SelectedIndexChanged;
        }

        public ViewRelationTreeDLG MyViewRelationWindow()
        {
            try
            {
                return _viewRelDLG;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        void _puViewSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((_treeView != null) && (MyDBManager != null))
            {
                Error error = null;
                var viewId = ViewProperty.GetViewIdFromName(MyDBManager.Database, _puViewSelector.Text, ref error);
                if (viewId != Guid.Empty)
                {
                    _treeView.ViewId = viewId;
                    _curViewId = viewId;
                }

                Error.Display("View " + _puViewSelector.Text + " not found in database!", error);

                //fire the event for view selection changed!
                if(OnViewSelectionChanged != null)
                    OnViewSelectionChanged(_puViewSelector, new ViewSelectArgs(_puViewSelector.Text, viewId));
            }
        }

        // The delegate procedure we are assigning to our object
        public delegate void ViewSelectedChangedHandler(object myObject, ViewSelectArgs myArgs);

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
                Console.WriteLine("Viewadmin starts\n");

                _context = context;

                StartMe(context, e);

                InitDatabase();

                if (MyDBManager != null)
                {
                    // creating an inspector is enough, it will register itself...
                    var inspector = new ViewRelationsInspector(MyDBManager);

                    // creating the view admin delegator is enough, it will register itself...
                    var delegator = new ViewAdminDelegator(MyDBManager);
                }

                _dlgViewAdmin = new ViewAdminDLG(this);

                //Setting up the menuitem
                if (MyMainWindow != null)
                {
                    //Adding the ViewAdmin window
                    MyMainWindow.TheDialog.MenueAdmin().MenuItems.Add(_mnuItemViewAdmin);

                    _mnuItemViewAdmin.Visible = SDBeesGlobalVars.GetViewAdminDisplayment() ? true : false;

                    //MenuItem mnuNewWindow = new MenuItem("New View Window...");
                    //mnuNewWindow.Click += new EventHandler(mnuNewWindow_Click);
                    //MyMainWindow.TheDialog.MenueAdmin().MenuItems.Add(mnuNewWindow);

                    //MenuItem mnuCheckDatabase = new MenuItem("Check database...");
                    //mnuCheckDatabase.Click += new EventHandler(mnuCheckDatabase_Click);
                    //MyMainWindow.TheDialog.MenueAdmin().MenuItems.Add(mnuCheckDatabase);

                    CreateTreeView();
                }

                if (MyDBManager != null)
                {
                    //create a new viewrelationwindow
                    _viewRelDLG = new ViewRelationTreeDLG(MyDBManager);
                    MyDBManager.AddUpdateHandler(ViewAdmin_OnUpdateHandler);
                    try
                    {
                        //this.MyContext.ApplicationContext.AddTopLevelWindow(_viewRelDLG);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        void ViewAdmin_OnUpdateHandler(object myObject, EventArgs myArgs)
        {
            //_treeView.RefreshView();

            //SelectFirstTreeNode();

            //_treeView.RefreshView();

            //_viewRelDLG.RefreshView();
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
            var database = MyDBManager.Database;
            Error error = null;
            database.Open(false, ref error);

            // Zun�chst die TemplateTreenodes benachrichtigen...
            TemplateTreenodeTag selectedTag = null;
            TemplateTreenodeTag parentTag = null;
            if (selectedNode != null)
            {
                selectedTag = (TemplateTreenodeTag)selectedNode.Tag;

                if (selectedNode.Parent != null)
                {
                    parentTag = (TemplateTreenodeTag)selectedNode.Parent.Tag;
                }

            }

            var allTreenodePlugins = TemplateTreenode.GetAllTreenodePlugins();
            foreach (var treenodePlugin in allTreenodePlugins)
            {
                if (treenodePlugin != null)
                {
                    treenodePlugin.UpdatePropertyPage(selectedTag, parentTag);
                }
            }

            // Jetzt die TemplateTreenodeHelper benachrichtigen...
            var allHelperPlugins = TemplateTreenodeHelper.GetAllHelperPlugins();
            foreach (var helperPlugin in allHelperPlugins)
            {
                if (helperPlugin != null)
                {
                    var tabPage = MyMainWindow.TheDialog.TabPagePlugin(helperPlugin.GetType().ToString());
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
            if (MyDBManager != null)
            {
                // Create the required database tables...
                var database = MyDBManager.Database;

                Error error = null;
                database.Open(false, ref error);

                FillViewSelector();
                MyMainWindow.TheDialog.ToolStripMainWindow().Items.Add(_puViewSelector);
                _puViewSelector.Width = MainWindowApplicationDLG.m_Splitterdistance;

                _treeView = new ViewRelationTreeView(MyDBManager);
                _treeView.AfterSelect += _treeView_AfterSelect;
                _treeView.ViewSwitched += _treeView_ViewSwitched;

                MyMainWindow.TheDialog.SystemView().Controls.Clear();
                MyMainWindow.TheDialog.SystemView().Controls.Add(_treeView);
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

            var database = MyDBManager.Database;
            Error error = null;
            if (ViewRelation.ChildReferencedByView(database, _curViewId, new Guid(newTag.NodeGUID), ref error))
            {
                // This child is already referenced in this view and cannot be referenced again
                return;
            }

            if (!ViewDefinition.ViewDefinitionExists(database, _curViewId, args.ParentType, newTag.NodeTypeOf, ref error))
            {
                MessageBox.Show("relation not guilty (" + args.ParentType + " / " + newTag.NodeTypeOf + ")");
                return;
            }

            //Die Relation einf�gen
            var viewrel = new ViewRelation();
            viewrel.SetDefaults(database);
            viewrel.ViewId = _curViewId;
            viewrel.ChildId = new Guid(newTag.NodeGUID);
            viewrel.ChildType = newTag.NodeTypeOf;
            viewrel.ChildName = newTag.NodeName;
            viewrel.ParentId = args.ParentId;
            viewrel.ParentType = args.ParentType;

            viewrel.Save(ref error);

            Error.Display("can't create relation", error);
        }

        private void CreateTabPropertyPages()
        {
            // Jetzt das TabControl f�r Eigenschaften bzw. EDM, ... einrichten
            MyMainWindow.TheDialog.MyTabControl().TabPages.Clear();

            // Eine TabPage f�r die Grunddaten ist immer da...
            var tabPageProperties = new TabPage();
            tabPageProperties.Name = ""; // Grunddaten muss diesen Namen haben!!!
            tabPageProperties.Text = "Properties";
            MyMainWindow.TheDialog.MyTabControl().TabPages.Add(tabPageProperties);

            // F�r jeden Helper eine TabPage hinzuf�gen...
            var allHelperPlugins = TemplateTreenodeHelper.GetAllHelperPlugins();

            foreach (var pluginHelper in allHelperPlugins)
            {
                // Wenn der Helper kein Label hat, dann m�chte dieser nicht dargestellt werden.
                if (pluginHelper.TabPageName() != "")
                {
                    var tabPageHelper = new TabPage();
                    tabPageHelper.Name = pluginHelper.GetType().ToString(); // Grunddaten muss einen leeren namen haben!!!
                    tabPageHelper.Text = pluginHelper.TabPageName();
                    MyMainWindow.TheDialog.MyTabControl().TabPages.Add(tabPageHelper);
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
            //context.SplashWindow.ShowDialog();
            //this.CloseHiddenWindow();
        }

        /// <summary>
        /// Returns the one and only ViewAdminManager Plugin instance.
        /// </summary>
        public static ViewAdmin Current
        {
            get { return _theInstance; }
        }

        /// <summary>
        /// The Context for the loaded Plugin
        /// </summary>
        public PluginContext MyContext
        {
            get { return _context; }
        }

        public ViewAdminDLG MyDialog
        {
            get { return _dlgViewAdmin; }
        }

        private void FillViewSelector()
        {
            if (_treeView != null) _treeView.ViewId = Guid.Empty;

            _puViewSelector.Items.Clear();

            var database = MyDBManager.Database;
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

            _dlgViewAdmin.RefreshView();
            SelectFirstTreeNode();
        }

        private void InitDatabase()
        {
            // Das Databaseplugin besorgen
            if (MyDBManager != null)
            {
                // Create the required database tables...
                var database = MyDBManager.Database;

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
            _dlgViewAdmin.ShowDialog();

            // Refill the view list, but get the last setting first...
            var lastViewName = _puViewSelector.Text;

            FillViewSelector();

            var lastViewInDialog = _dlgViewAdmin._cmbViewSelector.Text;
            if (lastViewInDialog != "")
            {
                lastViewName = lastViewInDialog;
            }
            SelectViewByName(lastViewName);
        }

        void mnuNewWindow_Click(object sender, EventArgs e)
        {
            var newWindow = new ViewRelationTreeDLG(MyDBManager);
            //newWindow.ViewId = _curViewId;

            newWindow.Show();
        }

        void mnuCheckDatabase_Click(object sender, EventArgs e)
        {
            var dialog = new CheckDatabaseDLG();

            dialog.ShowDialog();
        }


        #endregion

        public void SetUpPredefinedViewForPlugin(string typeOfPlugin, StringCollection viewStructure, string nameOfViewProperty)
        {
            //check, if View already exists
            Error error = null;

            var viewprop = new ViewProperty();
            var attViewProp = new Attribute(viewprop.Table.Columns["viewname"], nameOfViewProperty);
            var criteria = MyDBManager.Database.FormatCriteria(attViewProp, DbBinaryOperator.eIsEqual, ref error);

            ArrayList objectIds = null;
            var count = MyDBManager.Database.Select(viewprop.Table, viewprop.Table.PrimaryKey, criteria, ref objectIds, ref error);

            if (count > 0)
                return;

            // create a new viewprop
            viewprop.SetDefaults(MyDBManager.Database);
            viewprop.SetPropertyByColumn("viewname", nameOfViewProperty);
            viewprop.SetPropertyByColumn("viewdescription", String.Format("view automatically created by plugin {0}", typeOfPlugin));
            viewprop.Save(ref error);

            // define the viewrelations from structure
            foreach (var item in viewStructure)
            {
                var viewdef = new ViewDefinition();
                viewdef.SetDefaults(MyDBManager.Database);
                viewdef.SetPropertyByColumn("view", viewprop.GetPropertyByColumn(Object.m_IdColumnName));
                viewdef.SetPropertyByColumn("parent_type", item.Split(',')[0]);
                viewdef.SetPropertyByColumn("child_type", item.Split(',')[1]);

                foreach(PluginDescriptor desc in MyContext.PluginDescriptors)
                {
                    if (desc.PluginType.ToString() == item.Split(',')[1])
                    {
                        viewdef.SetPropertyByColumn("child_name", desc.PluginName);
                        break;
                    }
                }
                viewdef.Save(ref error);
            }
            //FillViewSelector();
        }
    }

    public class ViewSelectArgs : EventArgs
    {
        private string m_viewname;
        private Guid m_viewguid;

        public ViewSelectArgs(string viewname, Guid viewguid)
        {
            m_viewname = viewname;
            m_viewguid = viewguid;
        }

        public string ViewName
        {
            get
            {
                return m_viewname;
            }
        }

        public Guid ViewGuid
        {
            get { return m_viewguid; }
        }
    }
}
