// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
//
// Copyright (C) 2007 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// #EndHeader# ================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Interop;
using Autodesk.AutoCAD.Interop.Common;
using Autodesk.AEC.Interop.Schedule;
using Autodesk.AEC.Interop.AreaCalculation;
using Autodesk.AEC.Interop.Base;

using SDBees.Plugs.TemplateTreenode;
using SDBees.Main.Window;

using Carbon;
using Carbon.Plugins.Attributes;
using Carbon.Plugins;


namespace SDBees.Connectivity
{

    /// <summary>
    /// Autocad Connectivity
    /// </summary>
    [PluginName("ConnectivityInterfaceAutocad Plugin")]
    [PluginAuthors("Eberhard Michaelis")]
    [PluginDescription("Plugin for ConnectivityInterfaceAutocad")]
    [PluginId("FB136184-1E35-4c9f-AA2B-899DE0E96C8D")]
    [PluginManufacturer("G.E.M. Team Solutions")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]

    public class ConnectivityInterfaceAutocad : ConnectivityInterface
    {
        #region Private Data Members

        private IAecScheduleApplication mScheduleApp;
        private IAcadApplication mIAcadApp;
        private Autodesk.AutoCAD.Interop.AcadApplication mAcadApp;
        private IAcadDocument mDocument;
        private static ConnectivityInterfaceAutocad _theInstance = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public static ConnectivityInterfaceAutocad Current
        {
            get { return _theInstance; }
        }

        #region Public Properties
        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// default Constructor does nothing
        /// </summary>
        public ConnectivityInterfaceAutocad()
        {
            _theInstance = this;
            mDocument = null;
            mScheduleApp = null;
            mIAcadApp = null;
            mAcadApp = null;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Imports objects from an external document and creates nodes as children of the currentTag
        /// in the given view
        /// </summary>
        /// <param name="viewId"></param>
        /// <param name="currentTag"></param>
        /// <returns></returns>
        public override bool ImportObjects(Guid viewId, TemplateTreenodeTag currentTag)
        {
            bool success = true;

            // This might be a length process, so entertain the user meanwhile :-)
            ProgressTool progressTool = new ProgressTool();
            progressTool.StartActiveProcess(true, true);

            progressTool.WriteStatus("Zeichnung wird analysiert...");

            int numCreated = 0;
            int numUpdated = 0;
            int totalObjects = 0;

            string dwgName = "";
            List<string> filter = new List<string>();
            List<string> externalIds = null;
            List<ImportRule> rules = null;

            TemplateTreenode.ViewAdminDelegator.GetChildTypes(viewId, currentTag.NodeTypeOf, out filter);
            GetExternalIdsFromContainer(dwgName, filter, out externalIds, out rules);

            // We should get the focus back now...
            MyMainWindow.TheDialog.Activate();

            if ((externalIds != null) && (rules != null))
            {
                totalObjects += externalIds.Count;

                progressTool.ProgressBar.Maximum = totalObjects;


                for (int index = 0; index < externalIds.Count; index++)
                {
                    string externalId = externalIds[index];
                    string sdbeesType = rules[index].InternalObjectType;
                    string defaultName = rules[index].DefaultName;

                    TemplateTreenode plugin = TemplateTreenode.GetPluginForType(sdbeesType);
                    if (plugin != null)
                    {
                        TemplateTreenodeBaseData baseData = FindConnectedSDBeesNodeAndUpdate(externalId, true);

                        if (baseData == null)
                        {
                            string parentType = currentTag.NodeTypeOf;
                            Guid parentId = new Guid(currentTag.NodeGUID);

                            // TBD: die view Id sollte übergeben werden...
                            CreateSDBeesNodeAndConnection(sdbeesType, defaultName, externalId, true, parentType, parentId);

                            numCreated++;
                        }
                        else
                        {
                            numUpdated++;
                        }
                    }


                    progressTool.ProgressBar.Value = index + 1;
                    progressTool.WriteStatus(numCreated + " erzeugt, " + numUpdated + " aktualisiert...");
                }
            }

            // that's enough of entertainment...
            progressTool.EndActiveProcess();

            string message = "";
            if (mDocument != null)
            {
                message = "Es wurden insgesamt " + totalObjects + " externe Objekt bearbeitet.\r\n";
                message += "\t" + numCreated + " Objekte wurden erzeugt.\r\n";
                message += "\t" + numUpdated + " Objekte wurden aktualisiert.\r\n";
            }
            else
            {
                message = "Kein Dokument aktiv!";
            }

            MessageBox.Show(message);

            return success;
        }

        /// <summary>
        /// returns a list of menu items to add to the current popup Menu, called by the popup managing class
        /// </summary>
        /// <param name="nodes">the selected nodes</param>
        /// <param name="menuItemsToAdd">the menu items to add</param>
        /// <returns>true if menu has to be added</returns>
        public override bool GetPopupMenuItems(List<TemplateTreenodeTag> nodes, out List<MenuItem> menuItemsToAdd)
        {
            bool retVal = false;
            menuItemsToAdd = null;
            if (nodes.Count == 1)
            {
            }
            return retVal;
        }


        /// <summary>
        /// returns the external interface type (e.g. Acad0001, Ifc00001, ...)
        /// </summary>
        /// <returns>the 8 character interface type code</returns>
        public override string InterfaceType()
        {
            return "Acad0001";
        }

        /// <summary>
        /// returns the external interface name (e.g. Autodesk Architectural Desktop 2006, ...)
        /// </summary>
        /// <returns>the readable name, also used in the menu</returns>
        public override string InterfaceName()
        {
            return "Autodesk Architectural Desktop 2006";
        }

        /// <summary>
        /// exports the parameters from the give SDBees node
        /// </summary>
        /// <param name="externalId">the external id of the external target object</param>
        /// <param name="nodeObject">the source SDBees node object that contains the parameters</param>
        /// <returns>true, if successful</returns>
        public override bool ExportAttributes(string externalId, TemplateTreenodeBaseData nodeObject)
        {
            try
            {
                if (!loadDocument(""))
                {
                    return false;
                }

                SDBees.DB.Database database = MyDBManager.Database;
                List<string> propNames = nodeObject.GetPropertyNames();
                foreach (string propName in propNames)
                {
                    SDBees.DB.Column column = null;
                    nodeObject.Table.Columns.TryGetValue(propName, out column);

                    SDBees.DB.Error error = null;
                    PropertySetAssignment assignment = PropertySetAssignment.FindByColumn(database, InterfaceType(), nodeObject.ObjectType(), column.Name, ref error);

                    if (assignment == null)
                    {
                        // We must use a combination for propertSet/propertyName that is not yet used...
                        string propertyName = propName;
                        string propertySetName = nodeObject.GetPlugin().GetDisplayName() + ".SMARTDataBees";
                        int index = 0;
                        while (PropertySetAssignment.FindByPropertySet(database, InterfaceType(), nodeObject.ObjectType(), propertySetName, propertyName, ref error) != null)
                        {
                            index++;
                            propertyName = propName + index.ToString();
                        }

                        // Create new assignment for this...
                        assignment = new PropertySetAssignment();
                        assignment.SetDefaults(database);
                        assignment.PlugIn = nodeObject.ObjectType();
                        assignment.InterfaceType = InterfaceType();
                        assignment.PropertyName = propertyName;
                        assignment.PropertySetName = propertySetName;
                        assignment.ColumnName = propName;

                        if (!assignment.Save(ref error))
                        {
                            assignment = null;
                            SDBees.DB.Error.Display("Konnte keine Zuordnung zwischen SMARTDataBees und ADT Eigenschaften erstellen!", error);
                        }
                    }

                    // add the property set definition and property definition, if missing
                    if (assignment != null)
                    {
                        string propertySetName = assignment.PropertySetName;
                        string propertyName = assignment.PropertyName;

                        AecSchedulePropertySetDefs propSetDefs = (AecSchedulePropertySetDefs)mScheduleApp.PropertySetDefs(mDocument.Database);
                        if (!propSetDefs.Has(propertySetName))
                        {
                            propSetDefs.Add(propertySetName);
                        }
                        AecSchedulePropertySetDef propSetDef = propSetDefs.Item(propertySetName);
                        AecSchedulePropertyDef propDef = null;
                        try
                        {
                            propDef = propSetDef.PropertyDefs.Item(propertyName);
                        }
                        catch
                        {
                            propDef = null;
                        }

                        if (propDef == null)
                        {
                            AecSchedulePropertyDefType acadPropType = AcadPropertyType(column.Type);
                            propDef = propSetDef.PropertyDefs.Add(propertyName);
                            propDef.Type = acadPropType;
                        }

                        // export the property
                        AecSchedulePropertySets propSets = mScheduleApp.PropertySets((AcadObject)AcadObject(externalId));
                        AecSchedulePropertySet propSet = propSets.Item(propertySetName);
                        if (propSet == null)
                        {
                            propSets.Add(propSetDef);
                            propSet = propSets.Item(propertySetName);
                        }
                        AecScheduleProperty prop = propSet.Properties.Item(propertyName);

                        // Automatic properties will crash AutoCAD and SDBees if we try to set them...
                        if (!prop.Automatic && propSetDef.Writeable)
                        {
                            prop.Value = AcadPropertyValue(column.Type, nodeObject.GetProperty(propName));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return true;
        }

        /// <summary>
        /// imports the parameters from the external linked obect
        /// </summary>
        /// <param name="externalId">the external id of the external source object</param>
        /// <param name="nodeObject">the target SDBees node object that will receive the parameters</param>
        /// <returns>true, if succesfull</returns>
        public override bool ImportAttributes(string externalId, TemplateTreenodeBaseData nodeObject)
        {
            try
            {
                AcadObject acadObject = (AcadObject)AcadObject(externalId);
                IAecSchedulePropertySets propSets = mScheduleApp.PropertySets(acadObject);
                foreach (IAecSchedulePropertySet propSet in propSets)
                {
                    ImportPropertySet(propSet, nodeObject);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            } 
            return true;
        }

        /// <summary>
        /// Get the information relevant to display to the user for this connection
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="objectType"></param>
        /// <param name="nodeId"></param>
        /// <returns>Message to display</returns>
        public override string GetDisplayInformation(string externalId, string objectType, string nodeId)
        {
            string dwgName = AcadDocumentName(externalId);
            string handle = AcadHandle(externalId);
            string message = "Verbindung zu: \r\n";
            message += "Zeichnung: " + dwgName + "\r\n";
            message += "Handle:    " + handle + "\r\n";
            message += "\r\n";

            SDBees.DB.Database database = MyDBManager.Database;
            ArrayList objectIds = null;
            SDBees.DB.Error error = null;
            if (PropertySetAssignment.FindAllByPlugin(database, InterfaceType(), objectType, ref objectIds, ref error) > 0)
            {
                message += "SMARTDataBees - PropertySet Eigenschaftszuordnung:\r\n";
                message += "--------------------------------------------------\r\n";

                foreach (object objectId in objectIds)
                {
                    PropertySetAssignment assignment = new PropertySetAssignment();
                    if (assignment.Load(database, objectId, ref error))
                    {
                        string description = assignment.ColumnName + " -> " + assignment.PropertySetName + " | " + assignment.PropertyName;

                        message += description + "\r\n";
                    }
                }
            }

            SDBees.DB.Error.Display("Konnte Eigenschaftszuordnung nicht bestimmen", error);

            return message;
        }

        /// <summary>
        /// Creates an external object of the same type as the node-object and links it with the node-object
        /// </summary>
        /// <param name="externalId">the external id of the created external object</param>
        /// <param name="nodeObject">SDBees node object to link with</param>
        /// <param name="exportAttributes">if true, parameters are exported</param>
        /// <returns>true, if successful</returns>
        public override bool CreateExternalObjectAndConnection(out string externalId, TemplateTreenodeBaseData nodeObject, bool exportAttributes)
        {
            externalId = "";
            return false;
        }

        /// <summary>
        /// Lets the user select an external object interactively
        /// </summary>
        /// <returns>the external id of the selected object</returns>
        public override bool SelectExternalObjectInteractively(out string externalId)
        {
            bool retval = false;
            externalId = "";

            if (loadDocument(""))
            {
                SetForegroundWindow(new IntPtr(mAcadApp.HWND));

                object obj;
                object pickPoint;
                string prompt = "Element auswählen";

                try
                {
                    mDocument.Utility.GetEntity(out obj, out pickPoint, prompt);
                    if (obj != null)
                    {
                        externalId = ExternalId((IAcadObject)obj);
                        retval = true;
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return retval;
        }

        /// <summary>
        /// Lets the user select multiple external objects interactively
        /// </summary>
        /// <param name="externalIds"></param>
        /// <returns></returns>
        public override bool SelectExternalObjectsInteractively(out List<string> externalIds)
        {
            bool retval = false;
            externalIds = new List<string>();

            if (loadDocument("") && (mDocument != null))
            {
                SetForegroundWindow(new IntPtr(mAcadApp.HWND));

                try
                {
                    List<AcadEntity> entities = null;
                    if (SelectEntitiesInteractively("Elemente auswählen", out entities))
                    {
                        retval = true;

                        foreach (AcadEntity entity in entities)
                        {
                            string externalId = ExternalId((IAcadObject) entity);
                            externalIds.Add(externalId);
                        }

                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return retval;
        }

        /// <summary>
        /// Highlights external objects
        /// </summary>
        /// <param name="externalIds"></param>
        /// <returns></returns>
        public override bool HighlightExternalObject(List<string> externalIds)
        {
            bool retval = true;

            if ((externalIds != null) && (externalIds.Count > 0))
            {
                // Verify that the application is initialized...
                loadDocument("");

                SetForegroundWindow(new IntPtr(mAcadApp.HWND));

                foreach (string externalId in externalIds)
                {
                    IAcadEntity acadEntity = (IAcadEntity)AcadObject(externalId);

                    if (acadEntity != null)
                    {
                    	try
                    	{
	                        object minPoint = null;
	                        object maxPoint = null;
	                        acadEntity.GetBoundingBox(out minPoint, out maxPoint);
	                        
	                        double[] arminPoint = (double[])minPoint;
	                        double[] armaxPoint = (double[])maxPoint;
	                        
	                        double dXMin = arminPoint[0];	                        
	                        string sMinXTemp = dXMin.ToString();
	                        string sMinX = sMinXTemp.Replace(',','.');
	                        
	                        double dYMin = arminPoint[1];
	                        string sMinYTemp = dYMin.ToString();
	                        string sMinY = sMinYTemp.Replace(',','.');
	                        
	                        string sMin = sMinX + "," + sMinY;
	                        
	                        double dXMax = armaxPoint[0];	                        
	                        string sMaxXTemp = dXMax.ToString();
	                        string sMaxX = sMaxXTemp.Replace(',','.');
	                        
	                        double dYMax = armaxPoint[1];
	                        string sMaxYTemp = dYMax.ToString();
	                        string sMaxY = sMaxYTemp.Replace(',','.');
	                        string sMax = sMaxX + "," + sMaxY;
	                        	
	                        mAcadApp.ActiveDocument.SendCommand("zoom " + sMin + " " + sMax + " ");
	                        
	                        mAcadApp.ActiveDocument.SendCommand("zoom 0.5x ");
	                    	
	                        acadEntity.Highlight(true);
                    	}
                    	catch(System.Exception ex)
                    	{
                    		ex.ToString();
                    	}
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// get external ids of objects in the external container
        /// </summary>
        /// <param name="containerId">the id of the container (filename, ...)</param>
        /// <param name="filter">all objects of the types that are listed in this array are returned, if empty all objects are returned</param>
        /// <param name="externalIds">the ids of the found objects</param>
        /// <param name="rules">The matching rules found for the external ids (in sync with externalIds</param>
        /// <returns>true if successful</returns>
        public override bool GetExternalIdsFromContainer(string containerId, List<string> filter, out List<string> externalIds, out List<ImportRule> rules)
        {
            bool retVal = false;
            externalIds = null;
            rules = null;
            retVal = loadDocument(containerId);
            if (retVal && (mDocument != null))
            {
                List<AcadEntity> eintities = null;
                if (SelectEntitiesInteractively("Elemente wählen", out eintities))
                {
                    externalIds = new List<string>();
                    rules = new List<ImportRule>();
                    // AcadModelSpace modelspace = mDocument.Database.ModelSpace;
                    // foreach (IAcadEntity entity in modelspace)
                    foreach (AcadEntity entity in eintities)
                    {
                        ImportRule rule = FindMatchingRule(entity, filter);
                        if ((filter == null) || ((rule != null) && filter.Contains(rule.InternalObjectType)))
                        {
                            // string externalId = mDocument.FullName + "[%-%]" + entity.Handle.ToString();
                            string externalId = ExternalId(entity);
                            externalIds.Add(externalId);
                            rules.Add(rule);
                        }
                    }
                }
            }
            return retVal;
        }

        /// <summary>
        /// Find a rule for the matching combination
        /// </summary>
        /// <param name="externalId"></param>
        /// <param name="objectType"></param>
        /// <param name="nodeId"></param>
        /// <returns>A rule if found or null if combination not valid</returns>
        public override ImportRule FindValidRule(string externalId, string objectType, string nodeId)
        {
            IAcadEntity acadObject = (IAcadEntity)AcadObject(externalId);

            List<string> objectTypes = new List<string>();
            objectTypes.Add(objectType);
            ImportRule rule = FindMatchingRule(acadObject, objectTypes);

            if ((rule != null) && (rule.InternalObjectType != objectType))
            {
                rule = null;
            }

            return rule;
        }

        #endregion


        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Start(Carbon.Plugins.PluginContext context, Carbon.Plugins.PluginDescriptorEventArgs e)
        {
            base.Start(context, e);
            Console.WriteLine("Autocad Connectivity Plugin starts\n");

            this.StartMe(context, e);

            SDBees.DB.Database database = this.MyDBManager.Database;
            PropertySetAssignment.InitTableSchema(database);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(Carbon.Plugins.PluginContext context, Carbon.Plugins.PluginDescriptorEventArgs e)
        {
            Console.WriteLine("Autocad Connectivity Plugin stops\n");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propSetDef"></param>
        /// <param name="propertySetName"></param>
        /// <param name="prop"></param>
        /// <param name="nodeObject"></param>
        /// <returns></returns>
        protected bool ImportProperty (IAecSchedulePropertySetDef propSetDef, string propertySetName, IAecScheduleProperty prop, TemplateTreenodeBaseData nodeObject)
        {
            SDBees.DB.Database database = MyDBManager.Database;

            IAecSchedulePropertyDef propDef = propSetDef.PropertyDefs.Item(prop.Name);

            SDBees.DB.Error error = null;

            PropertySetAssignment assignment = PropertySetAssignment.FindByPropertySet(database, InterfaceType(), nodeObject.ObjectType(), propertySetName, prop.Name, ref error);
            if (assignment == null)
            {
                // Create new assignment for this...
                assignment = new PropertySetAssignment();
                assignment.SetDefaults(database);
                assignment.PlugIn = nodeObject.ObjectType();
                assignment.InterfaceType = InterfaceType();
                assignment.PropertyName = prop.Name;
                assignment.PropertySetName = propertySetName;
                assignment.ColumnName = database.MakeValidColumnName(prop.Name + propertySetName);

                // add number if column name already exists...
                int index = 0;
                string baseName = assignment.ColumnName;
                while (nodeObject.Table.Columns.ContainsKey(assignment.ColumnName))
                {
                    index++;
                    assignment.ColumnName = baseName + index.ToString();
                }

                if (!assignment.Save(ref error))
                {
                    SDBees.DB.Error.Display("Konnte keine Zuordnung zwischen SMARTDataBees und ADT Eigenschaften erstellen!", error);
                }
            }

            string sdBeesPropertyName = assignment.ColumnName;
            sdBeesPropertyName = nodeObject.Database.MakeValidColumnName(sdBeesPropertyName);

            SDBees.DB.Column column = null;
            nodeObject.Table.Columns.TryGetValue(sdBeesPropertyName, out column);

            int size = 50;
            SDBees.DB.DbType columnType = SDBees.DB.DbType.eUnknown;
            if (column != null)
            {
                columnType = column.Type;
                size = column.Size;
            }
            object valueDefault;
            object value = null;

            object propertyValue = null;
            try
            {
                propertyValue = prop.Value;
            }
            catch(Exception)
            {
                propertyValue = "*INVALID*";
            }

            if (propDef.Automatic)
            {
                value = SDBeesPropertyValue(AecSchedulePropertyDefType.aecSchedulePropertyTypeText, ref columnType, out valueDefault, ref size, propertyValue.ToString());
            }
            else
            {
                value = SDBeesPropertyValue(propDef.Type, ref columnType, out valueDefault, ref size, propertyValue);
            }
            if (column == null)
            {
                string sdBeesPropertyDisplayName = prop.Name + ":" + propertySetName;
                string sdBeesDescription = propDef.Description;
                column = new SDBees.DB.Column(sdBeesPropertyName, columnType, sdBeesPropertyDisplayName, sdBeesDescription, "Autocad Eigenschaften", size, valueDefault.ToString(), 0);
                column.Editable = !propDef.Automatic && propSetDef.Writeable;
                nodeObject.AddColumn(column, nodeObject.Database);
            }
            nodeObject.SetProperty(sdBeesPropertyName, value);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propSet"></param>
        /// <param name="nodeObject"></param>
        /// <returns></returns>
        protected bool ImportPropertySet (IAecSchedulePropertySet propSet, TemplateTreenodeBaseData nodeObject)
        {            
            IAecSchedulePropertySetDef propSetDef = mScheduleApp.PropertySetDefs(mDocument.Database).Item(propSet.Name);
            foreach (IAecScheduleProperty prop in propSet.Properties)
            {
                ImportProperty(propSetDef, propSet.Name, prop, nodeObject);
            }
            return true;
        }


        /// <summary>
        /// Get the Acad Application object, create a new session if none available
        /// </summary>
        /// <returns></returns>
        protected bool getApplications()
        {
            bool retval = false;
            try
            {
                if (mIAcadApp == null)
                {
                    mIAcadApp = (IAcadApplication)Marshal.GetActiveObject("AutoCAD.Application.16.2");
                    mAcadApp = (Autodesk.AutoCAD.Interop.AcadApplication)mIAcadApp;

                    RegisterEvents();
                }
            }
            catch
            {
                try
                {
                    // This might be a length process, so entertain the user meanwhile :-)
                    ProgressTool progressTool = new ProgressTool();
                    progressTool.StartActiveProcess(true, false);

                    progressTool.WriteStatus("AutoCAD wird gestartet...");

                    mIAcadApp = new Autodesk.AutoCAD.Interop.AcadApplication();
                    mAcadApp = (Autodesk.AutoCAD.Interop.AcadApplication)mIAcadApp;
                    mAcadApp.Visible = true;

                    RegisterEvents();

                    progressTool.EndActiveProcess();
                }
                catch
                {
                    mAcadApp = null;
                    mIAcadApp = null;
                    mScheduleApp = null;
                }
            }
            if (mAcadApp != null)
            {
                mScheduleApp = (IAecScheduleApplication)mAcadApp.GetInterfaceObject("AecX.AecScheduleApplication.4.7");
                if (mScheduleApp != null)
                {
                    retval = true;
                }
            }
            return retval;
        }

        private void RegisterEvents()
        {
            if (mAcadApp != null)
            {
                mAcadApp.BeginQuit += new _DAcadApplicationEvents_BeginQuitEventHandler(mAcadApp_BeginQuit);
                mAcadApp.EndOpen += new _DAcadApplicationEvents_EndOpenEventHandler(mAcadApp_EndOpen);

                if (mAcadApp.Documents.Count > 0)
                {
                    Autodesk.AutoCAD.Interop.AcadDocuments acadDocs = mAcadApp.Documents;

                    foreach (IAcadDocument iAcadDoc in acadDocs)
                    {
                        Autodesk.AutoCAD.Interop.AcadDocument acadDoc = (Autodesk.AutoCAD.Interop.AcadDocument)iAcadDoc;
                        acadDoc.BeginDocClose += new _DAcadDocumentEvents_BeginDocCloseEventHandler(acadDoc_BeginDocClose);
                    }
                }
            }
        }

        private void acadDoc_BeginDocClose(ref bool Cancel)
        {
            mDocument = null;
        }

        private void mAcadApp_EndOpen(string FileName)
        {
            mDocument = null;
            if (mAcadApp != null)
            {
                Autodesk.AutoCAD.Interop.AcadDocuments acadDocs = mAcadApp.Documents;

                foreach (IAcadDocument iAcadDoc in acadDocs)
                {
                    if (iAcadDoc.FullName == FileName)
                    {
                        mDocument = iAcadDoc;

                        Autodesk.AutoCAD.Interop.AcadDocument acadDoc = (Autodesk.AutoCAD.Interop.AcadDocument)iAcadDoc;
                        acadDoc.BeginDocClose += new _DAcadDocumentEvents_BeginDocCloseEventHandler(acadDoc_BeginDocClose);
                        
                        break;
                    }
                }

            }
        }

        private void mAcadApp_BeginQuit(ref bool Cancel)
        {
            mDocument = null;
            mAcadApp = null;
            mIAcadApp = null;
        }

        private void ActiveDocument_BeginClose()
        {
            mDocument = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentName">the full document name or "" to load the current document</param>
        /// <returns></returns>
        protected bool loadDocument (string documentName)
        {
            bool retVal = true;
            if ((mDocument == null) || (mDocument.FullName != documentName))
            {
                if (getApplications())
                {
                    if (documentName == "")
                    {
                        if (mAcadApp.Documents.Count > 0)
                        {
                            mDocument = mAcadApp.ActiveDocument;
                        }
                    }
                    else
                    {
                        Autodesk.AutoCAD.Interop.AcadDocuments acadDocs = mAcadApp.Documents;

                        foreach (IAcadDocument acadDoc in acadDocs)
                        {
                            if (acadDoc.FullName == documentName)
                            {
                                mDocument = acadDoc;
                                mDocument.Activate();
                                break;
                            }
                        }
                        if ((mDocument == null) || (mDocument.FullName != documentName))
                        {
                            try
                            {
                                object readOnly = false;
                                object password = "";
                                mDocument = acadDocs.Open(documentName, readOnly, password);
                            }
                            catch
                            {
                                retVal = false;
                            }
                        }
                    }
                }
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected AecSchedulePropertyDefType AcadPropertyType (SDBees.DB.DbType type)
        {
            switch (type)
            {
                case SDBees.DB.DbType.eBinary :
                case SDBees.DB.DbType.eBoolean : return AecSchedulePropertyDefType.aecSchedulePropertyTypeTrueFalse;
                case SDBees.DB.DbType.eByte :
                case SDBees.DB.DbType.eDecimal :
                case SDBees.DB.DbType.eInt16 :
                case SDBees.DB.DbType.eInt32 :
                case SDBees.DB.DbType.eInt64 : return AecSchedulePropertyDefType.aecSchedulePropertyTypeInteger;
                case SDBees.DB.DbType.eDouble :
                case SDBees.DB.DbType.eSingle : return AecSchedulePropertyDefType.aecSchedulePropertyTypeReal;
                default : return AecSchedulePropertyDefType.aecSchedulePropertyTypeText;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected object AcadPropertyValue (SDBees.DB.DbType type, object value)
        {
            object retval = null;
            //System.Int32 iVal = 0;
            //double dVal = 0.0;
            //System.String sVal = "";
            //bool bVal = false;
            switch (type)
            {
                case SDBees.DB.DbType.eBinary: 
                    // bVal = ((int)value != 0);
                    retval = System.Convert.ChangeType(value, typeof(System.String));
                    break;
                case SDBees.DB.DbType.eBoolean:
                    // bVal = (bool)value;
                    retval = System.Convert.ChangeType(value, typeof(System.Boolean));
                    break;
                case SDBees.DB.DbType.eByte:
                case SDBees.DB.DbType.eDecimal:
                case SDBees.DB.DbType.eInt16:
                case SDBees.DB.DbType.eInt32:
                case SDBees.DB.DbType.eInt64:
                    // iVal = (long)value;
                    retval = System.Convert.ChangeType(value, typeof(System.Int32));
                    break;
                case SDBees.DB.DbType.eDouble:
                case SDBees.DB.DbType.eSingle:
                    // dVal = (double)value;
                    retval = System.Convert.ChangeType(value, typeof(System.Double));
                    break;
                default: 
                    // sVal = value.ToString();
                    retval = System.Convert.ChangeType(value, typeof(System.String));
                    break;
            }
            return retval;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected SDBees.DB.DbType SDBeesPropertyType (AecSchedulePropertyDefType type)
        {
            switch (type)
            {
                case AecSchedulePropertyDefType.aecSchedulePropertyTypeAutoIncrement :
                case AecSchedulePropertyDefType.aecSchedulePropertyTypeInteger : return SDBees.DB.DbType.eInt64;
                case AecSchedulePropertyDefType.aecSchedulePropertyTypeReal : return SDBees.DB.DbType.eDouble;
                case AecSchedulePropertyDefType.aecSchedulePropertyTypeTrueFalse: return SDBees.DB.DbType.eBoolean;
                default: return SDBees.DB.DbType.eText;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acadType"></param>
        /// <param name="sdbeesType"></param>
        /// <param name="valueDefault"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        protected object SDBeesPropertyValue(AecSchedulePropertyDefType acadType, ref SDBees.DB.DbType sdbeesType, out object valueDefault, ref int size, object value)
        {
            object retval = null;
            valueDefault = "";
            Int64 iVal = 0;
            double dVal = 0.0;
            string sVal = "";
            bool bVal = false;
            switch (acadType)
            {
                case AecSchedulePropertyDefType.aecSchedulePropertyTypeAutoIncrement:
                case AecSchedulePropertyDefType.aecSchedulePropertyTypeInteger:
                    iVal = (Int64)System.Convert.ChangeType(value, typeof(Int64));
                    // iVal = (Int64)value;
                    if (sdbeesType == SDBees.DB.DbType.eUnknown)
                    {
                        sdbeesType = SDBees.DB.DbType.eInt64;
                        valueDefault = 0;
                    }
                    break;
                case AecSchedulePropertyDefType.aecSchedulePropertyTypeReal:
                    dVal = (double)value;
                    if (sdbeesType == SDBees.DB.DbType.eUnknown)
                    {
                        sdbeesType = SDBees.DB.DbType.eDouble;
                        valueDefault = 0.0;
                    }
                    break;
                case AecSchedulePropertyDefType.aecSchedulePropertyTypeTrueFalse:
                    bVal = (bool) value;
                    if (sdbeesType == SDBees.DB.DbType.eUnknown)
                    {
                        sdbeesType = SDBees.DB.DbType.eBoolean;
                        valueDefault = false;
                    }
                    break;
                default:
                    sVal = value.ToString();
                    if (sdbeesType == SDBees.DB.DbType.eUnknown)
                    {
                        sdbeesType = SDBees.DB.DbType.eString;
                        valueDefault = "default";
                        size = 255;
                    }
                    break;

            }

            switch (sdbeesType)
            {
                case SDBees.DB.DbType.eBinary:
                    retval = bVal ? 1 : 0;
                    break;
                case SDBees.DB.DbType.eBoolean:
                    retval = bVal;
                    break;
                case SDBees.DB.DbType.eByte:
                case SDBees.DB.DbType.eDecimal:
                case SDBees.DB.DbType.eInt16:
                case SDBees.DB.DbType.eInt32:
                case SDBees.DB.DbType.eInt64:
                    retval = iVal;
                    break;
                case SDBees.DB.DbType.eDouble:
                case SDBees.DB.DbType.eSingle:
                    retval = dVal;
                    break;
                case SDBees.DB.DbType.eDate:
                case SDBees.DB.DbType.eDateTime:
                    DateTime tVal = DateTime.Parse(sVal);
                    retval = tVal;
                    break;
                default:
                    retval = sVal;
                    break;
            }

            return retval;
        }

        /// <summary>
        /// Returns the rules this object would use to converted...
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="objectTypes"></param>
        /// <returns></returns>
        protected ImportRule FindMatchingRule(IAcadEntity entity, List<string> objectTypes)
        {
            ImportRule retval = null;

            if (entity != null)
            {
                string acadType = entity.ObjectName;

                foreach (ImportRule rule in ImportRules.Items)
                {
                    if ((rule.ExternalObjectType == acadType) && objectTypes.Contains(rule.InternalObjectType))
                    {
                        // Check the criteria...
                        NameValueCollection properties = new NameValueCollection();
                        properties.Add("Style", GetEntityStyle(entity));
                        properties.Add("Layer", GetEntityLayer(entity));
                        properties.Add("Color", entity.color.ToString());
                        properties.Add("Linetype", GetEntityLinetype(entity));
                        if (rule.Matches(properties))
                        {
                            retval = rule;
                            break;
                        }
                    }
                }
            }

            return retval;
        }

        /// <summary>
        /// Get the style name of an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns>Stylename or "" if not applicable</returns>
        protected string GetEntityStyle(IAcadEntity entity)
        {
            string result = "";

            // check different kinds of entities...
            string acadType = entity.ObjectName;

            switch (acadType)
            {
                case "AecDbArea":
                    AecArea area = (AecArea)entity;
                    result = area.Style.Name;
                    break;

                case "AecDbMassElem":
                    AecMassElement massElem = (AecMassElement)entity;
                    result = massElem.StyleName;
                    break;

                case "AecDbAreaGroup":
                    AecAreaGroup group = (AecAreaGroup)entity;
                    result = group.Style.Name;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Gets the layer name of an entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected string GetEntityLayer(IAcadEntity entity)
        {
            string result = entity.Layer;

            return result;
        }

        /// <summary>
        /// Gets the color of an entity and resolves bylayer
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected string GetEntityColor(IAcadEntity entity)
        {
            string result = entity.color.ToString();

            // TBD: consider byLayer/byBlock

            return result;
        }

        /// <summary>
        /// Gets the linetype name of an entity and resolves bylayer
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        protected string GetEntityLinetype(IAcadEntity entity)
        {
            string result = entity.Linetype.ToString();

            // TBD: consider byLayer/byBlock

            return result;
        }

        /// <summary>
        /// Lets the user select multiple external objects interactively
        /// </summary>
        /// <param name="prompt"></param>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool SelectEntitiesInteractively(string prompt, out List<AcadEntity> entities)
        {
            bool retval = false;
            entities = new List<AcadEntity>();

            if (mDocument != null)
            {
                SetForegroundWindow(new IntPtr(mAcadApp.HWND));

                try
                {
                    AcadSelectionSet ss = mDocument.SelectionSets.Add("ss01");

                    object oMissing01 = System.Reflection.Missing.Value;
                    object oMissing02 = System.Reflection.Missing.Value;

                    // TBD: display prompt
                    ss.SelectOnScreen(oMissing01, oMissing02);

                    if (ss.Count > 0)
                    {
                        foreach (AcadEntity entity in ss)
                        {
                            entities.Add(entity);
                        }

                        retval = true;
                    }

                    ss.Delete();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compositeName"></param>
        /// <returns></returns>
        protected string PropertySetName (string compositeName)
        {
            int colonIndex = compositeName.IndexOf(':');

            if (colonIndex < 0)
            {
                return "";
            }
            return compositeName.Substring(colonIndex + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compositeName"></param>
        protected string PropertyName (string compositeName)
        {
            int colonIndex = compositeName.IndexOf(':');

            if (colonIndex < 0)
            {
                return compositeName;
            }

            return compositeName.Substring(0, colonIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalId"></param>
        /// <returns></returns>
        protected string AcadDocumentName (string externalId)
        {
            string pathname = externalId.Substring(0, externalId.IndexOf("[%-%]"));

            pathname = MakeFullPathname(pathname);

            return pathname;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalId"></param>
        /// <returns></returns>
        protected string AcadHandle(string externalId)
        {
            string handle = externalId.Substring(externalId.IndexOf("[%-%]") + 5);

            return handle;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="externalId"></param>
        /// <returns></returns>
        protected IAcadObject AcadObject(string externalId)
        {
            IAcadObject result = null;

            if (loadDocument(AcadDocumentName(externalId)))
            {
                string handle = AcadHandle(externalId);
                result = (IAcadObject)mDocument.Database.HandleToObject(handle);
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="acadObject"></param>
        /// <returns></returns>
        protected string ExternalId (IAcadObject acadObject)
        {
            //assert(acadObject.Database == mDocument.Database);
            return MakeRelativePathname(mDocument.FullName) + "[%-%]" + acadObject.Handle.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="acadObject"></param>
        /// <returns></returns>
        protected string AcadHandle (IAcadObject acadObject)
        {
            return acadObject.Handle.ToString();
        }

        #endregion

        #region 

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion
    }
}
