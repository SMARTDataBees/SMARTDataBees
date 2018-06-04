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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Reflection;
using System.Windows.Forms;

using Carbon.Configuration.Providers;
using Carbon.Plugins.Providers;
using Carbon.UI.Providers;

namespace Carbon.Common
{
	/// <summary>
	/// Provides methods for reading CarbonSettings from the App.config file.
	/// </summary>
	//[System.Diagnostics.DebuggerStepThrough()]
	public static class CarbonConfig
	{
		private static readonly AppSettingsReader _reader;

		private static string _carbonSettingsGroupName;
		private static string _carbonConfigurationProvidersSectionName;
		//private static string _carbonEncryptionProvidersSectionName;
		private static string _carbonPluginProvidersSectionName;
		private static string _carbonWindowProvidersSectionName;
		private static string _carbonSplashWindowProviderName;
		private static string _carbonAboutWindowProviderName;
		private static string _carbonOptionsWindowProviderName;
        private static string _carbonDocumentsAndSettingsRelativePath;
        private static bool _carbonRunApplicationContext;
        private static bool _verbose;
        private static bool _showSplashWindow;
		private static bool _singleInstance;
		private static int _singleInstancePort;
		private static string _singleInstanceMutexValue;
		private static string _autoUpdateWebServiceUrl;

		#region ConfigSettingMissingException

		/// <summary>
		/// Defines an Exception that is generated when a key is missing from the App.config file.
		/// </summary>
		public sealed class ConfigSettingMissingException : ApplicationException
		{
			private readonly string _key;

			/// <summary>
			/// Initializes a new instance of the ConfigSettingMissingException class
			/// </summary>
			/// <param name="key">The name of the key that is missing</param>
			internal ConfigSettingMissingException(string key)				
			{
				_key = key;
			}
			
			public override string Message
			{
				get
				{
					return string.Format("The setting '{0}' is missing. The setting is a critical setting that must exist. Please add the setting to the App.config file.", _key);
				}
			}

			/// <summary>
			/// Returns the setting key that is missing from the App.config file
			/// </summary>
			public string Key
			{
				get
				{
					return _key;
				}
			}
		}

		#endregion

		/// <summary>
		/// Initializes the reader used by the class to read AppSetting Keys from the App.Config file
		/// </summary>
		static CarbonConfig() 
		{ 
			_reader = new AppSettingsReader();
		}
				
		/// <summary>
		/// Returns the internal AppSettingsReader that is used to access keys in the 'appSettings' section of the App.config file
		/// </summary>
		public static AppSettingsReader Reader 
		{
			get
			{
				return _reader;				
			}
		}

		/// <summary>
		/// Initializes the CarbonConfig class
		/// </summary>
		internal static void Initialize()
		{			
			// load the path to the docs and settings
			_carbonDocumentsAndSettingsRelativePath = GetValueAsString("CarbonDocumentsAndSettingsRelativePath", true);

			// make sure the all users data path exists
			if (!Directory.Exists(AllUsersDataPath))
				Directory.CreateDirectory(AllUsersDataPath);

			// make sure the local users data path exists
			if (!Directory.Exists(LocalUsersDataPath))
				Directory.CreateDirectory(LocalUsersDataPath);

			// initialize the logging system, this is about the earliest possible time this can be loaded
			LogFileManager.Initialize();

			// log the user's data paths, these are critical to the entire application
			LogSetting("AllUsersDataPath", AllUsersDataPath);
			LogSetting("LocalUsersDataPath", LocalUsersDataPath);

			// load the name of the carbon settings section, from which all of the core providers will be loaded
			_carbonSettingsGroupName = GetValueAsString("CarbonSettingsGroupName", true);

			// load the name of the config providers section
			_carbonConfigurationProvidersSectionName = GetValueAsString("CarbonConfigurationProvidersSectionName", true);

			// load the name of the config providers section
			//_carbonEncryptionProvidersSectionName = GetValueAsString("CarbonEncryptionProvidersSectionName", true);

			// load the name of the config providers section
			_carbonPluginProvidersSectionName = GetValueAsString("CarbonPluginProvidersSectionName", true);

			// load the name of the config providers section
			_carbonWindowProvidersSectionName = GetValueAsString("CarbonWindowProvidersSectionName", true);

			// load the name of the config providers section
			_carbonSplashWindowProviderName = GetValueAsString("CarbonSplashWindowProviderName", true);

			// load the name of the config providers section
			_carbonAboutWindowProviderName = GetValueAsString("CarbonAboutWindowProviderName", true);

			// load the name of the config providers section
			_carbonOptionsWindowProviderName = GetValueAsString("CarbonOptionsWindowProviderName", true);

			// load the setting that determines whether the plugin context will run a message pump			           
			_carbonRunApplicationContext = GetValueAsBool("CarbonRunApplicationContext", true);

			// load the verbose mode setting
            _verbose = GetValueAsBool("CarbonVerbose", true);

			// load the setting that determines whether the plugin context will show a splash dialog
            _showSplashWindow = GetValueAsBool("CarbonShowSplashWindow", true);

			// load the setting that determines whether the plugin context will allow multiple instances of itself
			_singleInstance = GetValueAsBool("CarbonSingleInstance", true);

			// load the setting that determines what port will be used to communicate with previous plugin context instances
			_singleInstancePort = GetValueAsInt("CarbonSingleInstancePort", true);

			// load the setting that determines the name of the mutex used by the plugin context to detect multiple instances
			_singleInstanceMutexValue = GetValueAsString("CarbonSingleInstanceMutexName", true);

			// loads the default autoupdate url
			_autoUpdateWebServiceUrl = GetValueAsString("CarbonAutoUpdateWebServiceUrl", true);			
		}
		
		/// <summary>
		/// Gets the value for a specified key from the CarbonConfig property and returns an object of the specified type containing the value from the .config file.
		/// </summary>
		/// <param name="key">The key for which to get the value</param>
		/// <param name="type">The type of the object to return</param>
		/// <param name="throwExceptionIfMissing">A flag that determines if an Exception is thrown if the key is not found</param>
		/// <returns></returns>
		public static object GetValue(string key, Type type, bool throwExceptionIfMissing)
		{
			if (key == null || key == string.Empty)
				throw new ArgumentNullException("key");

			if (type == null)
				throw new ArgumentNullException("type");
			
			try
			{
				object value = _reader.GetValue(key, type);

				LogSetting(key, value.ToString());

				return value;
			}
			catch(InvalidOperationException)
			{
				Log.WriteLine("Misconfiguration detected. The setting '{0}' is missing from the App.Config.", key);

				if (throwExceptionIfMissing)
					throw new ConfigSettingMissingException(key);				
			}

			return string.Empty;
		}
		
		/// <summary>
		/// Gets the value for the specified key as an string
		/// </summary>
		/// <param name="key">The key for which to get the value</param>
		/// <param name="throwExceptionIfMissing">A flag that determines if an Exception is thrown if the key is not found</param>
		/// <returns></returns>
		public static string GetValueAsString(string key, bool throwExceptionIfMissing)
		{
			return (string)GetValue(key, typeof(string), throwExceptionIfMissing);
		}

		/// <summary>
		/// Gets the value for the specified key as a bool
		/// </summary>
		/// <param name="key">The key for which to get the value</param>
		/// <param name="throwExceptionIfMissing">A flag that determines if an Exception is thrown if the key is not found</param>
		/// <returns></returns>
		public static bool GetValueAsBool(string key, bool throwExceptionIfMissing)
		{
			return (bool)GetValue(key, typeof(bool), throwExceptionIfMissing);
		}

		/// <summary>
		/// Gets the value for the specified key as a int
		/// </summary>
		/// <param name="key">The key for which to get the value</param>
		/// <param name="throwExceptionIfMissing">A flag that determines if an Exception is thrown if the key is not found</param>
		/// <returns></returns>
		public static int GetValueAsInt(string key, bool throwExceptionIfMissing)
		{
			return (int)GetValue(key, typeof(int), throwExceptionIfMissing);
		}

		/// <summary>
		/// Gets the value for the specified key as a DateTime
		/// </summary>
		/// <param name="key">The key for which to get the value</param>
		/// <param name="throwExceptionIfMissing">A flag that determines if an Exception is thrown if the key is not found</param>
		/// <returns></returns>
		public static DateTime GetValueAsDateTime(string key, bool throwExceptionIfMissing)
		{
			return (DateTime)GetValue(key, typeof(DateTime), throwExceptionIfMissing);
		}
		
		/// <summary>
		/// Returns a collection of configuration providers defined in the App.config
		/// </summary>
		/// <returns></returns>
		public static ConfigurationProviderCollection GetConfigurationProviders()
		{
			// read the configuration providers section
			return (ConfigurationProviderCollection)ConfigurationManager.GetSection(GetCarbonConfigSectionName(_carbonConfigurationProvidersSectionName));
		}

		///// <summary>
		///// Returns a collection of encryption providers defined in the App.config
		///// </summary>
		///// <returns></returns>
		//public static EncryptionProviderCollection GetEncryptionProviders()
		//{
		//    // read the encryption providers section
		//    return (EncryptionProviderCollection)ConfigurationManager.GetSection(GetCarbonConfigSectionName(_carbonEncryptionProvidersSectionName));
		//}

		/// <summary>
		/// Returns a collection of plugin providers defined in the App.config
		/// </summary>
		/// <returns></returns>
		public static PluginProviderCollection GetPluginProviders()
		{
			// read the plugin providers section
            return (PluginProviderCollection)ConfigurationManager.GetSection(GetCarbonConfigSectionName(_carbonPluginProvidersSectionName));
		}

		/// <summary>
		/// Returns a collection of window providers defined in the App.config
		/// </summary>
		/// <returns></returns>
		public static WindowProviderCollection GetWindowProviders()
		{
            return (WindowProviderCollection)ConfigurationManager.GetSection(GetCarbonConfigSectionName(_carbonWindowProvidersSectionName));
		}			
		
		/// <summary>
		/// Returns the name of the WindowProvider that will create the Splash Window
		/// </summary>
		public static string SplashWindowProviderName
		{
			get
			{
				return _carbonSplashWindowProviderName;
			}
		}

        /// <summary>
        /// Returns a relative path (i.e., "CodeReflection/Carbon") that will be used to
        /// construct file paths using various Windows Documents and Settings folders.
        /// </summary>
        public static string CarbonDocumentsAndSettingsRelativePath
        {
            get
            {
                return _carbonDocumentsAndSettingsRelativePath;
            }
        }

        /// <summary>
        /// Returns a flag indicating whether the PluginContext will run a message loop
        /// using the ApplicationContext or exit immediately after starting all of the plugins available.
        /// </summary>
        public static bool RunApplicationContext
        {
            get
            {
                return _carbonRunApplicationContext;
            }
			set
			{
				_carbonRunApplicationContext = value;
			}
        }

        /// <summary>
        /// Returns the data path that should be used to store data common to all users.
        /// </summary>
        public static string AllUsersDataPath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonApplicationData), _carbonDocumentsAndSettingsRelativePath);                
            }
        }

        /// <summary>
        /// Returns the data path that should be used to store data specific to local users.
        /// </summary>
        public static string LocalUsersDataPath
        {
            get
            {
                return Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), _carbonDocumentsAndSettingsRelativePath);			
            }
        }

        /// <summary>
        /// Gets or sets a flag that indicates the verbosity level of the Carbon Runtime.
        /// </summary>
        public static bool Verbose
        {
            get
            {
                return _verbose;
            }
            set
            {
                _verbose = value;
            }
        }

        /// <summary>
        /// Returns a flag that indicates if the PluginContext should display a splash window.
        /// </summary>
        public static bool ShowSplashWindow
        {
            get
            {
                return _showSplashWindow;
            }
        }

		/// <summary>
		/// Gets or sets a flag that indicates whether the application should be single instance.
		/// </summary>
		public static bool SingleInstance
		{
			get
			{
				return _singleInstance;
			}
		}

		/// <summary>
		/// Gets or sets a port number that the application will use to register the instance manager if the application is marked as single instance.
		/// </summary>
		public static int SingleInstancePort
		{
			get
			{
				return _singleInstancePort;
			}
		}

		/// <summary>
		/// Gets or sets the 
		/// </summary>
		public static string SingleInstanceMutexName
		{
			get
			{
				return _singleInstanceMutexValue;
			}
		}

		/// <summary>
		/// Gets or sets the default AutoUpdate Web Service Url.
		/// </summary>
		public static string AutoUpdateWebServiceUrl
		{
			get
			{
				return _autoUpdateWebServiceUrl;
			}
		}

		/// <summary>
		/// Returns a flag that indicates whether the Control key was depressed during startup signalling that the user
		/// would like to have his/her data reset. Usually this will apply to items such as window positions and chrome elements.
		/// </summary>
		public static bool ShouldResetUserData
		{
			get
			{
				return KeyboardUtilities.IsKeyDown(Keys.ControlKey);
			}
		}

		#region My Private Methods

		/// <summary>
		/// Returns the name of a Carbon config section using the specified section name
		/// </summary>
		/// <param name="sectionName">The name of the config section in the CarbonSettings group</param>
		/// <returns></returns>
		private static string GetCarbonConfigSectionName(string sectionName)
		{			
			return string.Format("{0}/{1}", _carbonSettingsGroupName, sectionName);
		}

		/// <summary>
		/// Logs the name and value of the specified application setting.
		/// </summary>
		/// <param name="name">The name of the setting.</param>
		/// <param name="value">The value of the setting.</param>
		/// <returns></returns>
		private static void LogSetting(string name, string value)
		{
			Log.WriteLine("Logging AppSetting, '{0}' = '{1}'.", name, value);
		}

		#endregion
	}
}
