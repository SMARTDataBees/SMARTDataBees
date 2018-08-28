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
using System.Data;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Global;
using SDBees.Core.Model;
using SDBees.DB.Forms;
using SDBees.DB.Generic;
using SDBees.DB.MySQL;
using SDBees.DB.SQLite;
using SDBees.GuiTools;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateBase;
using SDBees.Utils;
using SDBees.ViewAdmin;

namespace SDBees.DB
{
    [PluginName("DataBaseManager Plugin")]
    [PluginAuthors("Gamal Kira, Tim Hoffeller")]
    [PluginDescription("Plugin for the database management of SmartDataBees")]
    [PluginId("A4F00A51-D16F-4FB5-8799-51442374AF92")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(GlobalManager))]

    public sealed class SDBeesDBConnection : TemplatePlugin
    {
        private static SDBeesDBConnection m_theInstance;
        private PluginContext m_context;
        private ServerConfig m_ServerConfig;
        private Database m_database; // RALF : This database is absolutely needed here!
        private LogfileWriter mLogfileWriter;
        private TableInspector mInspector;

        /// <summary>
        /// Returns the one and only DataBaseManagerPlugin instance.
        /// </summary>
        public static SDBeesDBConnection Current
        {
            get
            {
                return m_theInstance;
            }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public SDBeesDBConnection()
        {
            m_theInstance = this;
            m_context = null;
            m_ServerConfig = null;
            m_database = null;

            SetupLogWriter();

            SDBeesGlobalVars.SetupInternalConfigurations(); //Configure SDBees internal configurations
        }

        private void SetupLogWriter()
        {
            var fileInfo = new FileInfo(Path.Combine(DirectoryTools.GetTempDir(), "SDBees.DB.Logfile.htm"));
            if (fileInfo.Exists)
            {
                fileInfo.Delete();
            }
            mLogfileWriter = new LogfileWriter(fileInfo.FullName);
            var dbCategory = new LogfileCategory("DB.Details");
            dbCategory.Flags = LogfileWriter.Flags.eShowDateTime | LogfileWriter.Flags.eTitleBold | LogfileWriter.Flags.eMessageItalic;
            dbCategory.Red = 0;
            dbCategory.Green = 192;
            dbCategory.Blue = 0;
            mLogfileWriter.AddCategory(dbCategory);
        }

        // The delegate procedure we are assigning to our object
        public delegate void SynchronizeHandler(object myObject, EventArgs myArgs);

        private event SynchronizeHandler OnDatabaseChangedHandler;

        private event SynchronizeHandler OnUpdateHandler;

        public void AddDatabaseChangedHandler(SynchronizeHandler handler)
        {
            OnDatabaseChangedHandler += handler;
        }

        public void RemoveDatabaseChangedHandler(SynchronizeHandler handler)
        {
            OnDatabaseChangedHandler -= handler;
        }

        public void AddUpdateHandler(SynchronizeHandler handler)
        {
            OnUpdateHandler += handler;
        }

        public void RemoveUpdateHandler(SynchronizeHandler handler)
        {
            OnUpdateHandler -= handler;
        }

        /// <summary>
        /// Returns the database this manager handles
        /// </summary>
        public Database Database
        {
            get { return m_database; }
        }

        /// <summary>
        /// Get the LogfileWriter to write to the database logfile
        /// </summary>
        public LogfileWriter LogfileWriter
        {
            get { return mLogfileWriter; }
        }

        public ServerConfigItem CurrentServerConfigItem
        {
            get { return m_ServerConfig.GetSelectedItem(); }
        }

        /// <summary>
        /// Occurs when the plugin starts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                m_context = context;

                StartMe(context, e);

                Console.WriteLine("DB Plugin starts\n");

                //CopyDemoDataset(); //Now handled by SDBees.Main

                /*
                bool doLogin = true; // TBD: Ralf - Set this to false!

                if (doLogin)
                {
                    SetDatabase();
                }
                */

                mInspector = new TableInspector(this);

                m_context.AfterPluginsStarted += OnAllPluginsStarted;

                var filenameDatabaseFolder = new DirectoryInfo(Path.Combine(ServerConfigHandler.GetConfigStorageFolder(), "Content\\"));

                if (!filenameDatabaseFolder.Exists)
                {
                    filenameDatabaseFolder.Create();
                }

                var filenameDatabase = Path.Combine(filenameDatabaseFolder.FullName, "Start.s3db");

                OpenProject(filenameDatabase, true, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //throw;
            }
        }

        private void OnAllPluginsStarted(object sender, EventArgs e)
        {
           //OnUpdate();
        }

        //public void OpenProjectOLD(SDBeesProjectId _inputId)
        //{
        //    SDBeesProjectId _id = new SDBeesProjectId();

        //    MyDBManager.SetDatabase();
        //    MyDBManager.ReloadPlugins();
        //   _id = MyDBManager.CurrentProjectId().ToString();
        //}

        string m_CurrentDbName = "";

        public string CurrentDbName
        {
            get { return m_CurrentDbName; }
        }

        public bool OpenProject(SDBeesProjectId projectId)
        {
            var result = false;

            // Find or create configuration.

            var conf = GetConfiguration();

            if (conf != null)
            {
                var item = conf.FindServerConfigItemByProjectGuid(projectId.ToString());

                if (item != null)
                {
                    Error error = null;
                    //m_CurrentDbName = item.ConfigItemName;

                    result = SetDatabase(item, "", false, ref error);
                }
            }

            return result;
        }

        public bool OpenProject(string filenameDatabase, bool createIfNotFound, bool force)
        {
            var result = false;

            // NOT NEEDED - Create database if it doesn't exist.

            /*
            if (!File.Exists(filenameDatabase))
            {
                string path = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "Content\\Start.s3db");

                FileInfo fileInfo = new FileInfo(path);

                fileInfo.CopyTo(filenameDatabase);
            }
            */

            // Find or create configuration.

            var conf = GetConfiguration();

            if (conf != null)
            {
                var item = conf.FindServerConfigItemByServerDatabasePath(filenameDatabase);

                if ((item == null) && createIfNotFound)
                {
                    item = new ServerConfigItem();

                    item.ProjectName = Path.GetFileName(filenameDatabase);
                    item.ProjectDescription = "";
                    item.ServerDatabasePath = filenameDatabase;
                    item.ServerTableCaching = false;
                    item.ServerType = Servers.SQLite;
                    item.ConfigItemName = Path.GetFileName(filenameDatabase);

                    conf.ConfigItems.Add(item);

                    ServerConfigHandler.SaveConfig(conf);

                    //m_CurrentDbName = item.ConfigItemName;
                }

                if (item != null)
                {
                    Error error = null;

                    result = SetDatabase(item, "", force, ref error);
                }
            }

            return result;
        }

        private bool SetDatabase(ServerConfigItem item, string password, bool force, ref Error error)
        {
            var result = false;

            if (force || !String.Equals(CurrentProjectId().ToString(), item.ProjectGuid))
            {
                if (item != null)
                {
                    if (item.ServerType == Servers.MySQL)
                    {
                        m_database = new MySqlDatabase();
                        m_database.Server = new MySqlServer(item, "pwd");
                    }
                    //else if (item.ServerType == Generic.Servers.MicrosoftSQL)
                    //{
                    //    m_database = new MsSqlDatabase();
                    //    m_database.Server = new MsSqlServer(item, "pwd");
                    //}
                    //else if (item.ServerType == Generic.Servers.MicrosoftSQLServerExpress)
                    //{
                    //    m_database = new DB.MicrosoftLocalDB.MsLocalDbDatabase();
                    //    m_database.Server = new DB.MicrosoftLocalDB.MsLocalDbServer(item, "pwd");
                    //}
                    else if (item.ServerType == Servers.SQLite)
                    {
                        m_database = new SQLiteDatabase();
                        m_database.UseGlobalCaching = false; // Use global caching!
                        m_database.Server = new SQLiteServer(item, "pwd");
                    }

                    m_database.Name = item.ServerDatabase;
                    m_database.Server.Name = item.ServerIpAdress;
                    m_database.User = item.UserName;
                    m_database.Port = item.ServerPort;
                    m_database.UseTableNameCaching = item.ServerTableCaching;
                    m_database.Password = password;
                    m_database.Server.SuperUser = item.UserName;
                    m_database.Server.Password = password;

                    // Create the required Tables...
                    TableSchema.InitTableSchema(m_database);

                    //m_database.Server.Init();

                    error = null;
                    var conn = m_database.Open(true, ref error);
                    if (conn == null)
                    {
                        m_database.Password = "";
                    }
                    else
                    {
                        m_database.Close(ref error);

                        m_ServerConfig.SelectedItemGuid = item.ConfigItemGuid;

                        InitTableSchema(ref SDBeesDBConnectionData.gTable, Database);

                        SetProjectInformations();

                        m_CurrentDbName = item.ConfigItemName;

                        OnDatabaseChanged();

                        result = true;
                    }
                }
            }
            else
            {
                result = true;
            }

            return result;
        }

        private void SetProjectInformations()
        {
            //Get current informations in db
            Error error = null;
            var _dbObjects = new ArrayList();

            if (FindAllObjects(Database, ref _dbObjects, ref error) == 0)
            {
                //First run, new project
                var dataObject = CreateDataObject() as SDBeesDBConnectionData;
                dataObject.SetDefaults(Database);
                dataObject.ProjectName = CurrentServerConfigItem.ProjectName;
                dataObject.ProjectDescription = CurrentServerConfigItem.ProjectDescription;
                dataObject.Save(ref error);
            }

            if (FindAllObjects(Database, ref _dbObjects, ref error) > 0)
            {
                var dataObject = CreateDataObject() as SDBeesDBConnectionData;

                if (dataObject.Load(Database, _dbObjects[0], ref error))
                {
                    if(!String.Equals(CurrentServerConfigItem.ProjectGuid, dataObject.GetPropertyByColumn(Object.m_IdColumnName).ToString()))
                        CurrentServerConfigItem.ProjectGuid = dataObject.GetPropertyByColumn(Object.m_IdColumnName).ToString();

                    if (!String.Equals(CurrentServerConfigItem.ProjectName, dataObject.ProjectName))
                        dataObject.ProjectName = CurrentServerConfigItem.ProjectName;

                    if (!String.Equals(CurrentServerConfigItem.ProjectDescription, dataObject.ProjectDescription))
                        dataObject.ProjectDescription = CurrentServerConfigItem.ProjectDescription;

                    dataObject.Save(ref error);
                }
            }

            SaveConfiguration();

            Error.Display("Can't create or handle project informations!", error);
        }

        public void OpenProject()
        {
            LoadConfiguration();

            if (m_ServerConfig != null)
            {
                var dlgLogin = new frmLogin();
                dlgLogin.Serverconfig = m_ServerConfig;

                var dlgRes = dlgLogin.ShowDialog(MyMainWindow.TheDialog);

                if (dlgRes == DialogResult.OK)
                {
                    SaveConfiguration();

                    var dbCategory = mLogfileWriter.GetCategory("DB.Details");
                    // Enable this to log actions in the database...
                    dbCategory.Enabled = false;

#if DEBUG
                    var m_logValue = false;
                    if (Boolean.TryParse(LogfileWriter.SDBeesLogLocalConfiguration().Options[LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue)
                        mLogfileWriter.Writeline("Info", "Database Plugin started", "DB.Details");
#endif

                    if (m_ServerConfig.GetSelectedItem() != null)
                    {
                        Error error = null;

                        if (!SetDatabase(m_ServerConfig.GetSelectedItem(), dlgLogin.Password, true, ref error))
                        {
                            m_database.Password = "";

                            // login failed... display error message...
                            var dlgError = new frmError("Can't open database!", "Login failed", error);
                            dlgError.ShowDialog();

                            dlgError.Dispose();
                        }

                        OnUpdate("Loading...");
                    }
                    else
                    {
                        var dlgError = new frmError("No server configuration available!", "Login failed", null);
                        dlgError.ShowDialog();

                        dlgError.Dispose();
                    }
                }
                else if (dlgRes == DialogResult.Cancel)
                    SaveConfiguration();
            }
        }

        private void OnDatabaseChanged()
        {
            if (OnDatabaseChangedHandler != null) 
                OnDatabaseChangedHandler(this, new EventArgs());

            //if (OnUpdateHandler != null) 
            //    OnUpdateHandler(this, new EventArgs());
        }

        // Progress bar functions ...

        protected iProgress GetProgressForm()
        {
            var myMainWindowHandle = MyMainWindow.TheDialog.Handle;

            var result = Progress.GetProgress(myMainWindowHandle);

            return result;
        }

        public void OnUpdate(string message)
        {
            if (OnUpdateHandler != null)
            {
                try
                {
                    ViewCache.Enable();

                    if (message != null)
                    {
                        using (var pf = GetProgressForm())
                        {
                            var caption = MainWindowApplication.Current.GetApplicationTitle();

                            pf.Set(caption, 100, message, message);

                            pf.IncrementTo(10);

                            Thread.Sleep(100);

                            pf.IncrementTo(30);

                            Thread.Sleep(200);

                            pf.IncrementTo(50);

                            OnUpdateHandler(this, new EventArgs());

                            pf.IncrementTo(80);

                            Thread.Sleep(200);

                            pf.IncrementTo(100);

                            Thread.Sleep(100);
                        }
                    }
                    else
                    {
                        OnUpdateHandler(this, new EventArgs());
                    }
                }
                finally
                {
                    ViewCache.Disable();
                }
            }
        }

//        public void SetDatabaseOLD()
//        {
//            LoadConfiguration();

//            if (m_ServerConfig != null)
//            {
//                frmLogin dlgLogin = new frmLogin();
//                dlgLogin.Serverconfig = m_ServerConfig;

//                DialogResult dlgRes = dlgLogin.ShowDialog();

//                if (dlgRes == System.Windows.Forms.DialogResult.OK ||
//                    dlgRes == DialogResult.Cancel)
//                {
//                    SaveConfiguration();

//                    LogfileCategory dbCategory = mLogfileWriter.GetCategory("DB.Details");
//                    // Enable this to log actions in the database...
//                    dbCategory.Enabled = false;

//#if DEBUG
//                    bool m_logValue = false;
//                    if (Boolean.TryParse(SDBees.GuiTools.LogfileWriter.SDBeesLogLocalConfiguration().Options[SDBees.GuiTools.LogfileWriter.m_LogSuccess, true].Value.ToString(), out m_logValue) && m_logValue == true)
//                        mLogfileWriter.Writeline("Info", "Database Plugin started", "DB.Details");
//#endif

//                    if (m_ServerConfig.GetSelectedItem() != null)
//                    {
//                        if (m_ServerConfig.GetSelectedItem().ServerType == Generic.Servers.MySQL)
//                        {
//                            m_database = new MySqlDatabase();
//                            m_database.Server = new MySqlServer(m_ServerConfig.GetSelectedItem(), "pwd");
//                        }
//                        //else if (m_ServerConfig.GetSelectedItem().ServerType == Generic.Servers.MicrosoftSQL)
//                        //{
//                        //    m_database = new MsSqlDatabase();
//                        //    m_database.Server = new MsSqlServer(m_ServerConfig.GetSelectedItem(), "pwd");
//                        //}
//                        //else if (m_ServerConfig.GetSelectedItem().ServerType == Generic.Servers.MicrosoftSQLServerExpress)
//                        //{
//                        //    m_database = new DB.MicrosoftLocalDB.MsLocalDbDatabase();
//                        //    m_database.Server = new DB.MicrosoftLocalDB.MsLocalDbServer(m_ServerConfig.GetSelectedItem(), "pwd");
//                        //}
//                        else if (m_ServerConfig.GetSelectedItem().ServerType == Generic.Servers.SQLite)
//                        {
//                            m_database = new DB.SQLite.SQLiteDatabase();
//                            m_database.Server = new DB.SQLite.SQLiteServer(m_ServerConfig.GetSelectedItem(), "pwd");
//                        }

//                        m_database.Name = m_ServerConfig.GetSelectedItem().ServerDatabase;
//                        m_database.Server.Name = m_ServerConfig.GetSelectedItem().ServerIpAdress;
//                        m_database.User = m_ServerConfig.GetSelectedItem().UserName;
//                        m_database.Port = m_ServerConfig.GetSelectedItem().ServerPort;
//                        m_database.UseTableNameCaching = m_ServerConfig.GetSelectedItem().ServerTableCaching;
//                        m_database.Password = dlgLogin.Password;
//                        m_database.Server.SuperUser = m_ServerConfig.GetSelectedItem().UserName;
//                        m_database.Server.Password = dlgLogin.Password;

//                        Error error = null;

//                        // Create the required Tables...
//                        TableSchema.InitTableSchema(m_database);

//                        m_database.Server.Init();
//#if false
//            // TBD: Gamal - die Dialoge erscheinen hinter dem Carbon ProcessViewer. Am besten erst sp�ter, d.h.
//            //              manuell das Einloggen initiieren.
//            if (!mDatabase.Server.DatabaseExists(mDatabase.Name, ref error))
//            {
//                DialogResult dlgRes = MessageBox.Show(this, "Die Datenbank '" + mDatabase.Name + "' existiert nicht, soll diese erstellt werden?", "Datenbank erstellen", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
//                if (dlgRes == DialogResult.Yes)
//                {
//                    if (!mDatabase.Server.CreateDatabase(mDatabase.Name, ref error))
//                    {
//                        Error.Display("Die Datenbank '" + mDatabase.Name + "' konnte nicht erstellt werden!", error);
//                    }
//                }
//                else
//                {
//                    return;
//                }
//            }
//#endif

//                        error = null;
//                        Connection conn = m_database.Open(true, ref error);
//                        if (conn == null)
//                        {
//                            m_database.Password = "";

//                            // login failed... display error message...
//                            frmError dlgError = new frmError("Can't open database!", "Login failed", error);
//                            dlgError.ShowDialog();

//                            dlgError.Dispose();
//                        }
//                        else
//                        {
//                            // login succeeded
//                            m_database.Close(ref error);
//                        }
//                    }
//                    else
//                    {
//                        frmError dlgError = new frmError("No server configuration available!", "Login failed", null);
//                        dlgError.ShowDialog();

//                        dlgError.Dispose();
//                    }
//                }
//            }
//        }

//        public void ReloadPlugins()
//        {
//            //m_context.RestartUsingBootstrap();
//            foreach (PluginDescriptor item in m_context.PluginDescriptors)
//            {
//                try
//                {
//                    if (item.PluginType != typeof(SDBeesDBConnection) &&
//                        item.PluginType != typeof(SDBees.Core.Connectivity.ConnectivityManager))
//                    {
//                        SDBees.Plugs.TemplateBase.TemplateBase plg = m_context.PluginDescriptors[item.PluginType].PluginInstance as SDBees.Plugs.TemplateBase.TemplateBase;
//                        if (plg != null)
//                            plg.ReStart(m_context);
//                    }
//                }
//                catch (Exception ex)
//                {
//                }
//            }
//        }

        public Guid CurrentProjectId()
        {
            var _temp = Guid.Empty;

            if(CurrentServerConfigItem != null)
            {
                Guid.TryParse(CurrentServerConfigItem.ProjectGuid, out _temp);
            }

            return _temp;
        }

        public string CurrentFilenameDatabase()
        {
            var result = "";

            if (CurrentServerConfigItem != null)
            {
                result = CurrentServerConfigItem.ServerDatabasePath;
            }

            return result;
        }

        private ServerConfig GetConfiguration()
        {
            if (m_ServerConfig == null) LoadConfiguration(false);

            return m_ServerConfig;
        }

        private void SaveConfiguration()
        {
            //Write the server configuration back to disk
            if (m_ServerConfig != null) ServerConfigHandler.SaveConfig(m_ServerConfig);
        }

        private void LoadConfiguration(bool createDefaults = true)
        {
            //Load server mandator config from user appdata
            m_ServerConfig = ServerConfigHandler.LoadConfig(createDefaults);
        }

        /*
        private void CopyDemoDataset()
        {
            const string m_DirDemo = "DemoData";
            DirectoryInfo dirSource = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), m_DirDemo));
            if (dirSource.Exists)
            {
                DirectoryInfo dirTarget = new DirectoryInfo(Path.Combine(DB.Generic.ServerConfigHandler.GetConfigStorageFolder(), m_DirDemo));
                if (!dirTarget.Exists)
                {
                    dirTarget.Create();
                    SDBees.Utils.DirectoryTools.DirectoryCopy(dirSource.FullName, dirTarget.FullName, true);
                }
            }            
        }
        */

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("DB Plugins stops\n");

            if (m_ServerConfig != null)
            {
                SaveConfiguration();
            }
        }

        public override Table MyTable()
        {
            return SDBeesDBConnectionData.gTable;
        }

        public override TemplateDBBaseData CreateDataObject()
        {
            return new SDBeesDBConnectionData();
        }

        public override TemplatePlugin GetPlugin()
        {
            return m_theInstance;
        }

        protected override void OnDatabaseChanged(object myObject, EventArgs myArgs)
        {
            // empty
        }

        #region Reporting
        /// <summary>
        /// Plugins can use this method to retrieve a DataTable for Reporting Tools
        /// </summary>
        /// <param name="sTableName">Name of the table to select the Data from</param>
        /// <returns></returns>
        public DataTable GetDataTableForPlugin(string sTableName)
        {
            DataTable _tbl = null;

            Error _error = null;

            try
            {
                Database.Open(true, ref _error);

                _tbl = Database.Connection.GetReadOnlyDataTable(sTableName);

            }
            finally
            {
                Database.Close(ref _error);
            }

            Error.Display("Error while receiving Datatable", _error);

            return _tbl;
        }

        public DataSet GetDataSetForAllTables()
        {
            DataSet _set = null;

            Error _error = null;

            try
            {
                Database.Open(true, ref _error);

                _set = Database.Connection.GetReadOnlyDataSet();
            }
            finally
            {
                Database.Close(ref _error);
            }

            Error.Display("Error while receiving Dataset", _error);

            return _set;
        }
        #endregion
    }

    public class SDBeesDBConnectionData : TemplateDBBaseData
    {
        #region Private Data Members

        internal static Table gTable;

        #endregion

        #region Public Methods
        public const string m_ProjectDescriptionColumnName = "description";
        public const string m_ProjectDBVersionColumnName = "sdbeesversion";

        public override void InitTableSchema(ref Table table, Database database)
        {
            base.InitTableSchema(ref table, database);
            //required columns
            AddColumn(new Column(m_ProjectDescriptionColumnName, DbType.String, "Description", "The project description", "ProjectInfo", 250, "this is a new project", 0), database);
            AddColumn(new Column(m_ProjectDBVersionColumnName, DbType.String, "dbVersion", "The version of SDBees", "ProjectDBVersion", 250, "this is the project version number", 0), database);
        }
        #endregion

        #region Public Properties
        public override string GetTableName
        {
            get { return "sdbDBManager"; }
        }

        /// <summary>
        /// Get/Set the project name of this object
        /// </summary>
        public string ProjectName
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        /// Get/Set the project description of this object
        /// </summary>
        public string ProjectDescription
        {
            get { return (string)GetPropertyByColumn(m_ProjectDescriptionColumnName); }
            set { SetPropertyByColumn(m_ProjectDescriptionColumnName, value); }
        }

        /// <summary>
        /// Get / Set the project version for this project
        /// </summary>
        public string ProjectVersion
        {
            get { return (string)GetPropertyByColumn(m_ProjectDBVersionColumnName); }
            set { SetPropertyByColumn(m_ProjectDescriptionColumnName, value); }
        }
        #endregion

        #region Constructor/Destructor

        public SDBeesDBConnectionData() :
            base("DBManager", "DBConnection", "General")
        {
            Table = gTable;
        }

        #endregion
    }
}
