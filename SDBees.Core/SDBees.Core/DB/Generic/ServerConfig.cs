using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SDBees.DB.Generic
{
    [XmlRootAttribute("sdbeesserverconfiguration", Namespace = "", IsNullable = false)]
    public class ServerConfig
    {
        List<ServerConfigItem> mConfigItems;
        [XmlArray("configurations")]
        [XmlArrayItem(typeof(ServerConfigItem))]
        public List<ServerConfigItem> ConfigItems
        {
            get { return mConfigItems; }
            set { mConfigItems = value; }
        }

        public ServerConfig()
        {
            mConfigItems = new List<ServerConfigItem>();
        }

        string mVersion = "1.0.0.0";
        [XmlAttribute("configversion")]
        public string ConfigVersion
        {
            get { return this.mVersion; }
            set { this.mVersion = value; }
        }

        DateTime mConfigSaveDate = DateTime.Now;
        [XmlAttribute("configsavedate")]
        public DateTime ConfigSaveDate
        {
            get { return mConfigSaveDate; }
            set { mConfigSaveDate = value; }
        }

        string m_selectedItemGuid = "";
        [XmlAttribute("configlastselecteditem")]
        public string SelectedItemGuid
        {
            get { return m_selectedItemGuid; }
            set { m_selectedItemGuid = value; }
        }

        public ServerConfigItem GetSelectedItem()
        {
            ServerConfigItem res = null;

            if (!String.IsNullOrEmpty(this.SelectedItemGuid))
            {
                foreach (ServerConfigItem item in this.mConfigItems)
                {
                    if (item.ConfigItemGuid == this.SelectedItemGuid)
                    {
                        res = item;
                        break;
                    }
                }
            }
            else
            {
                // nothing set or selected before, so we use the first config in list
                foreach (ServerConfigItem item in this.mConfigItems)
                {
                    res = item;
                    this.SelectedItemGuid = item.ConfigItemGuid;
                    break;
                }
            }

            return res;
        }

        public void AddDefaultItem()
        {
            this.ConfigItems.Add(new ServerConfigItem());
        }

        public ServerConfigItem FindServerConfigItemByProjectGuid(string projectGuid)
        {
            ServerConfigItem result = null;

            foreach (ServerConfigItem item in mConfigItems)
            {
                if (item.ProjectGuid.Equals(projectGuid))
                {
                    result = item;

                    break;
                }
            }

            return result;
        }

        public ServerConfigItem FindServerConfigItemByServerDatabasePath(string serverDatabasePath)
        {
            ServerConfigItem result = null;

            foreach (ServerConfigItem item in mConfigItems)
            {
                if (item.ServerDatabasePath.Equals(serverDatabasePath))
                {
                    result = item;

                    break;
                }
            }

            return result;
        }

        internal void DeleteSelectedItem()
        {
            ServerConfigItem res = null;

            if (!String.IsNullOrEmpty(this.SelectedItemGuid))
            {
                foreach (ServerConfigItem item in this.mConfigItems)
                {
                    if (item.ConfigItemGuid == this.SelectedItemGuid)
                    {
                        res = item;
                        break;
                    }
                }
            }

            this.mConfigItems.Remove(res);
            this.SelectedItemGuid = null;
            this.GetSelectedItem();
        }
    }

    public class ServerConfigItem
    {
        public ServerConfigItem()
        {
            //m_ExchangeRules = new List<ExchangeRule>();
        }

        string m_ConfigName = "Database configuration name";
        [XmlAttribute("configname")]
        [Description("Insert name for this configuration."), Category("1 - Config")]
        public string ConfigItemName
        {
            get { return m_ConfigName; }
            set { m_ConfigName = value; }
        }

        string m_ProjectName = "ProjectName";
        [XmlAttribute("projectname")]
        [Description("Insert name for this project"), Category("2 - General")]
        public string ProjectName
        {
            get { return m_ProjectName; }
            set { m_ProjectName = value; }
        }

        string m_ProjectDescription = "Description";
        [XmlAttribute("projectdescription")]
        [Description("Insert an description for this project"), Category("2 - General")]
        public string ProjectDescription
        {
            get { return m_ProjectDescription; }
            set { m_ProjectDescription = value; }
        }

        string m_ProjectGuid = Guid.NewGuid().ToString();
        [XmlAttribute("projectid")]
        [ReadOnly(true), Description("the unique id for this project"), Category("2 - General")]
        public string ProjectGuid
        {
            get { return m_ProjectGuid; }
            set { m_ProjectGuid = value; }
        }

        string m_ConfigGuid = Guid.NewGuid().ToString();
        [XmlAttribute("configguid")]
        [Browsable(false)]
        public string ConfigItemGuid
        {
            get { return m_ConfigGuid; }
            set { m_ConfigGuid = value; }
        }

        string m_UserName = "root";
        [XmlAttribute("username")]
        [Description("Insert user name for database login."), Category("4 - Server")]
        public string UserName
        {
            get { return m_UserName; }
            set { m_UserName = value; }
        }

        Servers m_ServerType = Servers.SQLite;
        [XmlAttribute("servertype")]
        [Description("Select type of supported database servers"), Category("4 - Server")]
        public Servers ServerType
        {
            get { return m_ServerType; }
            set { m_ServerType = value; }
        }

        string m_ServerIpAdress = "127.0.0.1";
        [XmlAttribute("serveripadress")]
        [Description("Network name or ip adress for the database"), Category("4 - Server")]
        public string ServerIpAdress
        {
            get { return m_ServerIpAdress; }
            set { m_ServerIpAdress = value; }
        }

        string m_ServerDatabasePath = GetDefaultDatabase();
        private static string GetDefaultDatabase()
        {
            //Load expected window title
            bool replace = false;

            string _title = System.Configuration.ConfigurationManager.AppSettings["MainWindowTitle"];
            if (!String.IsNullOrEmpty(_title))
            {
                replace = true;
            }

            FileInfo inf = null;
            if(replace)
                inf = new FileInfo(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), _title + "\\DemoData\\DemoDb.s3db"));
            else
                inf = new FileInfo(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData), "SMARTDataBees\\DemoData\\DemoDb.s3db"));

            return inf.FullName;
        }

        [XmlAttribute("serverdatabasepath")]
        [Description("If you use a filebased database system, insert path to databasefile here!"), Category("4 - Server")]
        [EditorAttribute(typeof(SDBees.Core.DB.SQLite.SQLiteFilenameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string ServerDatabasePath
        {
            get { return m_ServerDatabasePath; }
            set { m_ServerDatabasePath = value; }
        }

        string m_Database = "sdbees";
        [XmlAttribute("serverdatabase")]
        [Description("Name of the database on server"), Category("4 - Server")]
        public string ServerDatabase
        {
            get { return m_Database; }
            set { m_Database = value; }
        }

        string m_SecureDatabase = "sdbees";
        [XmlAttribute("serversecuredatabase")]
        [Description("If you want to use a different database for security settings, enter the name here."), Category("4 - Server")]
        public string ServerSecureDatabase
        {
            get { return m_SecureDatabase; }
            set { m_SecureDatabase = value; }
        }

        string m_ServerPort = "3306";
        [XmlAttribute("serverport")]
        [Description("Network port for database server"), Category("4 - Server")]
        public string ServerPort
        {
            get { return m_ServerPort; }
            set { m_ServerPort = value; }
        }

        bool m_TableCaching = false;
        [XmlAttribute("servertablecaching")]
        [Description("Enable table caching for online databases?"), Category("4 - Server")]
        public bool ServerTableCaching
        {
            get { return m_TableCaching; }
            set { m_TableCaching = value; }
        }

        string m_EDMRootDirectory = "c:\\SDBees.EDM";
        [XmlAttribute("edmdirectory")]
        [EditorAttribute(typeof(System.Windows.Forms.Design.FolderNameEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Folder for the EDM root directory"), Category("3 - EDM")]
        public string EDMRootDirectory
        {
            get { return m_EDMRootDirectory; }
            set { m_EDMRootDirectory = value; }
        }
        //List<ExchangeRule> m_ExchangeRules;
        //[XmlArray("exchangerules")]
        //[XmlArrayItem(typeof(ExchangeRule))]
        //[Browsable(false)]
        //public List<ExchangeRule> ExchangeRules
        //{
        //    get { return m_ExchangeRules; }
        //    set { m_ExchangeRules = value; }
        //}
    }

    //public class ExchangeRule
    //{
    //    string mPath = "c:\\Temp\\Text.xml";
    //    [XmlAttribute("path")]
    //    public string Path
    //    {
    //        get { return mPath; }
    //        set { mPath = value; }
    //    }
    //}

    public static class ServerConfigHandler
    {
        static string _SDBeesStorageFolder = GetStorageFolder();
        static string _SDBeesMandatorFileName = GetStorageFolder() + "Mandators.dbconfig";

        private static string GetStorageFolder()
        {
            //Load expected window title
            string _title = System.Configuration.ConfigurationManager.AppSettings["MainWindowTitle"];
            if (!String.IsNullOrEmpty(_title))
            {
                return _title;
            }
            else
	        {
                return "SMARTDataBees";
	        }            
        }

        public static string GetConfigStorageFolder()
        {
            string mainfolder = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), _SDBeesStorageFolder);
            DirectoryInfo dirinf = new DirectoryInfo(mainfolder);
            if (!dirinf.Exists)
	        {
		        dirinf.Create();
	        }
            return dirinf.FullName;
        }

        private static string GetMandatorConfigFile()
        {
            return Path.Combine(GetConfigStorageFolder(), _SDBeesMandatorFileName);
        }

        public static ServerConfig LoadConfig(bool createDefaults = true)
        {
            ServerConfig mConfig = null;
            if (!System.IO.File.Exists(GetMandatorConfigFile()))
                CreateMandatorConfig(createDefaults);

            if (System.IO.File.Exists(GetMandatorConfigFile()))
            {
                try
                {
                    mConfig = SDBees.Utils.ObjectXmlSerializer.ObjectXMLSerializer<ServerConfig>.Load(GetMandatorConfigFile());
                }
                catch (System.Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
            return mConfig;
        }

        public static void SaveConfig(ServerConfig mConfig)
        {
            mConfig.ConfigSaveDate = DateTime.Now;
            SDBees.Utils.ObjectXmlSerializer.ObjectXMLSerializer<ServerConfig>.Save(mConfig, GetMandatorConfigFile(), Encoding.UTF8);
        }

        private static void CreateMandatorConfig(bool createDefaults = true)
        {
            ServerConfig cfg = new ServerConfig();
            cfg.ConfigSaveDate = DateTime.Now;

            if(createDefaults)
                cfg.AddDefaultItem();


            SDBees.Utils.ObjectXmlSerializer.ObjectXMLSerializer<ServerConfig>.Save(cfg, GetMandatorConfigFile(), Encoding.UTF8);
        }
    }
}
