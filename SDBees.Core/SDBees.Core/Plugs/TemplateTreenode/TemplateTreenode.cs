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
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SDBees.Core.Admin;
using SDBees.Core.Global;
using SDBees.Core.Model;
using SDBees.Core.Properties;
using SDBees.DB;
using SDBees.Main.Window;
using SDBees.Plugs.Properties;
using SDBees.Plugs.TemplateBase;

namespace SDBees.Plugs.TemplateTreeNode
{
    /// <summary>
    /// Templateklasse für die Treenode basierenden Klassen
    /// </summary>
    public abstract class TemplateTreenode : TemplatePlugin
    {
        private UserControlTempTreenode m_myControl;
        private PropertyBag m_myBag;

        private string m_mnuItemCreate;
        private string m_mnuItemDelete;
        private string m_mnuItemSchema;

        private static Hashtable _hashPlugins;

        private static ViewAdminDelegatorBase mViewAdminDelegator;

        /// <summary>
        /// Der Konstruktor für den TemplateTreenode
        /// </summary>
        public TemplateTreenode()
        {
            try
            {
                if (_hashPlugins == null)
                {
                    _hashPlugins = new Hashtable();
                }
                var treenodeType = GetType().ToString();
                _hashPlugins.Add(treenodeType, this);

                m_myControl = new UserControlTempTreenode();
                m_myBag = new PropertyBag();

                m_mnuItemCreate = "Create";
                m_mnuItemDelete = "Delete";
                m_mnuItemSchema = "edit Schema ...";

                m_myControl.MyGrid().SelectedObject = m_myBag;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                //throw;
            }
        }

        /// <summary>
        /// Returns a list of the loaded plugins
        /// </summary>
        /// <returns></returns>
        public static List<TemplateTreenode> GetAllTreenodePlugins()
        {
            var result = new List<TemplateTreenode>();

            foreach (DictionaryEntry plugin in _hashPlugins)
            {
                result.Add((TemplateTreenode)plugin.Value);
            }

            return result;
        }

        /// <summary>
        /// Returns the TreenodeTemplate for the given type
        /// </summary>
        /// <param name="PluginType">Type as returned by GetType().ToString()</param>
        /// <returns>TemplateTreenode for the given GetType().ToString()</returns>
        public static TemplateTreenode GetPluginForType(string PluginType)
        {
            TemplateTreenode treenodePlugin = null;

            if (_hashPlugins.Contains(PluginType))
            {
                treenodePlugin = (TemplateTreenode)_hashPlugins[PluginType];
            }

            return treenodePlugin;
        }


        /// <summary>
        /// Enable or disable the manual parent relation linking for treenodes in ViewAdmin relation linker
        /// override this, if you don't want to support Relationlinker in your plugin
        /// </summary>
        public virtual bool AllowRelationLinkingAsSibling => true;

        public virtual bool AllowRelationLinkingAsChild { get; } = true;

        public virtual bool AllowEditingOfSchema()
        {
            var editingAllowed = true;

            try
            {
                var mytype = GetType().ToString();
                foreach (var t in SDBeesGlobalVars.GetEditSchemaDissallowList())
                {
                    if (!String.IsNullOrEmpty(t) && mytype.Contains(t))
                    {
                        editingAllowed = false;
                        break;
                    }
                }
            }
            catch (Exception)
            {

            }

            return editingAllowed;
        }



        /// <summary>
        /// The eventhandler for Creating an element of the plugin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void _mnuItemCreate_Click(object sender, EventArgs e)
        //{
        //    CreateNewObject();
        //    DoCreateSubTasks();
        //}

        //private void CreateNewObject()
        //{
        //    Error error = null;
        //    SDBees.Plugs.TemplateBase.TemplateDBBaseData baseData = CreateDataObject();
        //    baseData.SetDefaults(MyDBManager.Database);
        //    if (baseData.Name == "")
        //    {
        //        baseData.Name = "Unnamed";
        //    }

        //    System.Drawing.Point location = Control.MousePosition;

        //    string name = InputBox.Show("Description", "Name for the new object", baseData.Name, location.X, location.Y);
        //    if (name != "")
        //    {
        //        baseData.Name = name;
        //        baseData.Save(ref error);

        //        // parentType = "" und ID = Empty heisst an den aktuellen Knoten...
        //        string parentType = "";
        //        Guid parentId = Guid.Empty;

        //        RaiseObjectCreated(baseData, this.MyTag(), parentType, parentId);
        //    }
        //}

        TreenodePropertyRow m_propertyRow;
        public TreenodePropertyRow MyPropertyRow()
        {
            return m_propertyRow;
        }

        /// <summary>
        /// The Event for Treenode selection handling. Displays the UserControl in the
        /// Pluginarea of the main Dialog.
        /// Overwrite, if you want to handle it yourself
        /// </summary>
        /// <param name="selectedTag"></param>
        /// <param name="e"></param>
        /// <param name="parentTag"></param>
        public virtual void UpdatePropertyPage(TemplateTreenodeTag selectedTag, TemplateTreenodeTag parentTag)
        {
            if ((selectedTag != null) && (selectedTag.GetType() == typeof(TemplateTreenodeTag)))
            {
                try
                {
                    if (selectedTag.NodeTypeOf == GetType().ToString())
                    {
                        var tabPage = MainWindowApplication.Current.MyMainWindow.TheDialog.TabPagePlugin("");

                        // Get the standard Properties from Database
                        m_propertyRow = GetTreenodePropertyRowFromTag(selectedTag);

                        var _myControl = (IUserControlTempTreenode)MyUserControl;

                        // Das Control markieren...
                        //_myControl.Tag = propertyTable;
                        _myControl.MyGrid().SelectedObject = m_propertyRow;

                        tabPage.Controls.Clear();
                        tabPage.Controls.Add(MyUserControl);
                        MyUserControl.Dock = DockStyle.Fill;

                        // Show the Propertypage
                        MainWindowApplication.Current.MyMainWindow.TheDialog.MyTabControl().SelectedTab = tabPage;
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else if (selectedTag == null)
            {
                var tabPage = MainWindowApplication.Current.MyMainWindow.TheDialog.TabPagePlugin("");

                var _myControl = (IUserControlTempTreenode)MyUserControl;

                if (_myControl.MyGrid().SelectedObject != null)
                {
                    // Das Control markieren...
                    //_myControl.Tag = propertyTable;
                    _myControl.MyGrid().SelectedObject = null;

                    tabPage.Controls.Clear();
                    tabPage.Controls.Add(MyUserControl);
                    MyUserControl.Dock = DockStyle.Fill;

                    // Show the Propertypage
                    MainWindowApplication.Current.MyMainWindow.TheDialog.MyTabControl().SelectedTab = tabPage;
                }
            }
        }

        private TreenodePropertyRow GetTreenodePropertyRowFromTag(TemplateTreenodeTag selectedTag)
        {
            return m_propertyRow = new TreenodePropertyRow(MyDBManager, this, selectedTag);
            // Get automatic Properties by Plugin.
            // Each plugin should provide it's own implementation
        }

        /// <summary>
        /// Triggers the ViewAdmin to add the a relation of the given object to the given parent in the current view
        /// </summary>
        /// <returns></returns>
        public void AddRelation(Guid objectId, string name, string parentType, Guid parentId)
        {
            var Tag = MyTag();
            Tag.NodeGUID = objectId.ToString();
            Tag.NodeName = name;

            // Raise an event, the view admin should react to it...
            RaiseCreateViewRelation(Tag, parentType, parentId);
        }

        /// <summary>
        /// Returns the Default UserControl for the TemplateTreenode
        /// If you want to use your own control, you have to override this.
        /// </summary>
        /// <returns></returns>
        public virtual UserControl MyUserControl
        {
            get { return m_myControl; }
        }

        /// <summary>
        /// Creates a new node instance as child of the given node
        /// </summary>
        /// <param name="pluginType">the type of the new node (= plugin name)</param>
        /// <param name="name">the name of the new node</param>
        /// <param name="parentType">Type of the node the object should be added to</param>
        /// <param name="parentId">Id of the node the object should be added to</param>
        /// <returns></returns>
        public static TemplateDBBaseData AddNewChildNodeInstance(string pluginType, string name, string parentType, Guid parentId)
        {
            Error error = null;
            var nodeType = GetPluginForType(pluginType);
            var baseData = nodeType.CreateDataObject();
            baseData.SetDefaults(nodeType.MyDBManager.Database);
            if (name == "")
            {
                name = "Unnamed";
            }

            baseData.Name = name;
            if (baseData.Save(ref error))
            {
                nodeType.AddRelation(new Guid(baseData.Id.ToString()), name, parentType, parentId);
            }

            return baseData;
        }


        /// <summary>
        /// Find the icon for a given plugin type, the imageList must not be null,
        /// the image is added to the imagelist, if not already existing.
        /// </summary>
        /// <returns>image key to be used as ImageKey</returns>
        public static string getImageForPluginType(string PluginType, ImageList imageList)
        {
            var imageKey = "";

            var templTreenode = GetPluginForType(PluginType);
            if (templTreenode != null)
            {
                imageKey = PluginType;
                var treenodeIcon = templTreenode.GetIcon(new Size(16, 16)) ?? templTreenode.GetFailedIcon(new Size(16, 16));
                imageList?.Images.Add(imageKey, treenodeIcon);
            }

            return imageKey;
        }

        /// <summary>
        /// Find the icon for a given plugin type, the imageList must not be null,
        /// the image is added to the imagelist, if not already existing.
        /// </summary>
        /// <returns>image key to be used as ImageKey</returns>
        public static int getImageIndexForPluginType(string PluginType, ImageList imageList)
        {
            var imageIndex = -1;

            var templTreenode = GetPluginForType(PluginType);
            if (templTreenode != null)
            {
                var imageKey = PluginType;

                if (!imageList.Images.ContainsKey(imageKey))
                {
                    var treenodeIcon = templTreenode.GetIcon(new Size(16, 16));

                    if (treenodeIcon == null)
                    {
                        treenodeIcon = templTreenode.GetFailedIcon(new Size(16, 16));
                    }

                    imageList.Images.Add(imageKey, treenodeIcon);
                }

                imageIndex = imageList.Images.IndexOfKey(imageKey);
            }

            return imageIndex;
        }

        /// <summary>
        /// Each Treenode Plugin has to return its own tag
        /// Use the TemplateTreenodeTag for this!
        /// </summary>
        /// <returns></returns>
        public virtual TemplateTreenodeTag MyTag()
        {
            return new TemplateTreenodeTag {NodeTypeOf = GetType().ToString()};
        }

        /// <summary>
        /// Each Plugin has to override this Method to provide a SDBeesEntity for Connectivity
        /// </summary>
        /// <example>
        /// public override Core.Connectivity.SDBeesLink.SDBeesEntityDefinition GetEntityDefinition()
        /// {
        ///    return base.GetEntityDefinition(this.GetType());
        /// }
        /// </example>
        /// <returns></returns>
        public abstract SDBeesEntityDefinition GetEntityDefinition();


        /// <summary>
        /// Each Plugin could provide it's own implementation of SDBeesEntityDefinition for connectivity interface
        /// </summary>
        /// <returns></returns>
        public virtual SDBeesEntityDefinition GetEntityDefinition(Type AddinType)
        {
            var ent = new SDBeesEntityDefinition(AddinType);

            foreach (var col in MyTable().Columns)
            {
                var pdef = new SDBeesPropertyDefinition();
                pdef.Name = new SDBeesLabel(col.DisplayName);
                pdef.Id = new SDBeesPropertyDefinitionId(col.Name);
                //pdef.propertyType = 
                pdef.PropertyTypeConverter = col.UITypeConverter;
                pdef.PropertyUiTypeEditor = col.UITypeEditor;
                pdef.Editable = col.IsEditable;
                pdef.Browsable = col.IsBrowsable;

                ent.Properties.Add(pdef);
            }
            return ent;
        }

        /// <summary>
        /// Get the displayable name for this plugin
        /// </summary>
        /// <returns></returns>
        public virtual string GetDisplayName()
        {
            // TBD: this is a kludge an should be made abstract/overridden
            var name = CreateMenuItem;
            name = name.Substring(name.LastIndexOf(" ") + 1);

            return name;
        }

        /// <summary>
        /// Each Treenode Plugin can provide an own Icon, this will be the default if not provided
        /// </summary>
        /// <param name="size"></param>
        public virtual Icon GetStandardIcon(Size size)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var s = assembly.GetManifestResourceStream("SDBees.Plugs.Templates.Resources.StandardIcon.ico");
            var result = new Icon(s, size);
            s.Close();

            return result;
        }

        /// <summary>
        /// Each Treenode Plugin can provide an own Icon, this will be displayed if something went wrong
        /// </summary>
        /// <param name="size"></param>
        public virtual Icon GetFailedIcon(Size size)
        {
            //// TBD: nur für debugging zwecke...
            //List<string> resourceNames = new List<string>();
            //foreach (string resourceName in assembly.GetManifestResourceNames())
            //{
            //    resourceNames.Add(resourceName);
            //}

            var result = Resources.IconFailed;

            return result;
        }

        /// <summary>
        /// Each Treenode Plugin can provide an own Icon, this will be the default if not provided
        /// </summary>
        /// <param name="size"></param>
        public abstract Icon GetIcon(Size size);
        //{
        //Icon result = null;

        //Assembly assembly = Assembly.GetExecutingAssembly();
        //string location = assembly.Location;
        //FileInfo fileInfo = new FileInfo(location);
        //string typeName = this.GetType().ToString();
        //string imageFileName = fileInfo.DirectoryName + "\\Icons\\" + typeName + ".ico";

        //try
        //{
        //    result = new Icon(imageFileName, size);
        //}
        //catch(Exception ex)
        //{
        //    result = null;
        //}

        //    return result;
        //}

        /// <summary>
        /// Text for Treenode Plugin menuitem for creating an element
        /// </summary>
        public string CreateMenuItem
        {
            get { return m_mnuItemCreate; }
            set { m_mnuItemCreate = value; }
        }

        /// <summary>
        /// Text for Treenode Plugin menuitem for deleting an element
        /// </summary>
        public string DeleteMenuItem
        {
            get { return m_mnuItemDelete; }
            set { m_mnuItemDelete = value; }
        }

        /// <summary>
        /// Text for Treenode Plugin menuitem for editing the elements database schema
        /// </summary>
        public string EditSchemaMenuItem
        {
            get { return m_mnuItemSchema; }
            set { m_mnuItemSchema = value; }
        }

        public static ViewAdminDelegatorBase ViewAdminDelegator
        {
            get { return mViewAdminDelegator; }
            set { mViewAdminDelegator = value; }
        }

        public virtual void DeleteInstanceAndRelations(TemplateDBBaseData item, ref Error _error)
        {
            var relations = new ArrayList();

            var relationParents2Children = new ArrayList();

            if (0 < ViewRelation.FindViewRelationByChildId(MyDBManager.Database, new Guid(item.Id.ToString()), ref relationParents2Children, ref _error))
            {
                relations.AddRange(relationParents2Children);
            }

            var relationParent2Children = new ArrayList();

            if (0 < ViewRelation.FindViewRelationByParentId(MyDBManager.Database, new Guid(item.Id.ToString()), ref relationParent2Children, ref _error))
            {
                relations.AddRange(relationParent2Children);
            }

            foreach (var relation in relations)
            {
                var viewRelation = new ViewRelation();

                if (viewRelation.Load(MyDBManager.Database, new Guid(relation.ToString()), ref _error))
                {
                    viewRelation.Erase(ref _error);
                }
            }

            item.Erase(ref _error);
        }

        public virtual void DeleteInstancesAndRelations(List<TemplateDBBaseData> items, ref Error _error)
        {
            foreach (var item in items)
            {
                DeleteInstanceAndRelations(item, ref _error);
            }
        }

        #region Events

        public virtual void OnPropertyValueChanged(TreenodePropertyRow row, PropertyValueChangedEventArgs e)
        {
            // empty
        }

        public class BaseNotificationEventArgs
        {
            public TemplateTreenodeTag Tag;

            public BaseNotificationEventArgs(TemplateTreenodeTag tag)
            {
                Tag = tag;
            }
        }

        public class NotificationEventArgs : BaseNotificationEventArgs
        {
            public TemplateDBBaseData BaseData;
            public string ParentType;
            public Guid ParentId;

            public NotificationEventArgs(TemplateDBBaseData baseData, TemplateTreenodeTag tag)
                : base(tag)
            {
                BaseData = baseData;
                ParentType = "";
                ParentId = Guid.Empty;
            }
        }

        public class ViewRelationEventArgs : BaseNotificationEventArgs
        {
            public string ParentType;
            public Guid ParentId;

            public ViewRelationEventArgs(TemplateTreenodeTag tag, string parentType, Guid parentId)
                : base(tag)
            {
                ParentType = parentType;
                ParentId = parentId;
            }
        }

        public delegate void NotificationHandler(object sender, NotificationEventArgs args);
        public delegate void TreeViewEventHandler(object sender, TreeViewEventArgs args);
        public delegate void ViewRelationHandler(object sender, ViewRelationEventArgs args);

        public event NotificationHandler ObjectCreated;
        public event NotificationHandler ObjectModified;
        public event NotificationHandler ObjectDeleted;
        public event TreeViewEventHandler TreeViewAfterSelect;
        public event ViewRelationHandler CreateViewRelation;

        internal void RaiseObjectCreated(TemplateDBBaseData baseData, TemplateTreenodeTag Tag, string parentType, Guid parentId)
        {
            if (ObjectCreated != null)
            {
                Tag.NodeGUID = baseData.Id.ToString();
                Tag.NodeName = baseData.Name;

                var args = new NotificationEventArgs(baseData, Tag);
                args.ParentType = parentType;
                args.ParentId = parentId;

                ObjectCreated.Invoke(this, args);
            }
        }

        internal void RaiseObjectModified(TemplateDBBaseData baseData, TemplateTreenodeTag Tag)
        {
            if (ObjectModified != null)
            {
                var args = new NotificationEventArgs(baseData, Tag);
                ObjectModified.Invoke(this, args);
            }
        }

        internal void RaiseObjectDeleted(TemplateDBBaseData baseData, TemplateTreenodeTag Tag)
        {
            if (ObjectDeleted != null)
            {
                var args = new NotificationEventArgs(baseData, Tag);
                ObjectDeleted.Invoke(this, args);
            }
        }

        internal void RaiseCreateViewRelation(TemplateTreenodeTag Tag, string parentType, Guid parentId)
        {
            if (CreateViewRelation != null)
            {
                var args = new ViewRelationEventArgs(Tag, parentType, parentId);
                CreateViewRelation.Invoke(this, args);
            }
        }

        #endregion

        public virtual void DoCreateSubTasks(TemplateTreenodeTag newTag, TreeNode currentTreeNode)
        { }

        public virtual void DoDeleteSubTasks(TemplateTreenodeTag newTag, TreeNode currentTreeNode)
        { }

        public virtual bool IsNewChildInstanceAllowed(TreeNode currentTreeNode, string pluginType)
        {
            return true;
        }
    }
}
