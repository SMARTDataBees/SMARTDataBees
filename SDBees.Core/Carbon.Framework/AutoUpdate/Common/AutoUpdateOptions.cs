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
using System.ComponentModel;
using System.Diagnostics;

using Carbon.Common;
using Carbon.Configuration;
using Carbon.Configuration.Providers.Custom;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Provides a class that defines options that can be used to control the behavior
	/// of an AutoUpdateManager for the purposes of downloading updates.
	/// </summary>
	public class AutoUpdateOptions : DisposableObject, ISupportInitialize
	{
		private static DefaultAutoUpdateOptions _defaultOptions;
		private bool _initializing;
		private bool _automaticallyCheckForUpdates;
		private bool _automaticallyDownloadUpdates;
		private bool _automaticallyInstallUpdates;
		private bool _automaticallySwitchToNewVersion;
		private bool _automaticallyUpdateAlternatePath;
		private string _downloadPath;
		private string _alternatePath;
		private string _webServiceUrl;

		public readonly static string ConfigurationCategoryName = "AutoUpdate";

		/// <summary>
		/// Fired when an AutoUpdate option changes. 
		/// Only fires when the option is set and the options instance is not being initialized via ISupportInitialize interface.
		/// </summary>
		public event EventHandler<AutoUpdateOptionsEventArgs> Changed;

		/// <summary>
		/// Initializes the static members of the AutoUpdateOptions class.
		/// </summary>
		static AutoUpdateOptions()
		{
			_defaultOptions = new DefaultAutoUpdateOptions();
		}

		/// <summary>
		/// Returns the default AutoUpdate options.
		/// </summary>
		public static DefaultAutoUpdateOptions DefaultOptions
		{
			get
			{
				return _defaultOptions;
			}
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateOptions class
		/// </summary>
		public AutoUpdateOptions() 
			: base()
		{
			this.BeginInit();

			this.DownloadPath = string.Empty;
			this.AlternatePath = string.Empty;
			this.WebServiceUrl = string.Empty;

			this.EndInit();
		}

		protected override void DisposeOfManagedResources()
		{
			lock (base.SyncRoot)
			{
				this.Changed = null;
			}

			base.DisposeOfManagedResources();
		}

		#region ISupportInitialize Members

		public void BeginInit()
		{
			lock (base.SyncRoot)
			{
				_initializing = true;
			}
		}

		public void EndInit()
		{
			lock (base.SyncRoot)
			{
				_initializing = false;
			}
		}

		#endregion


		/// <summary>
		/// Monitors the all users configuration for changes to the AutoUpdate options.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void HandleConfigurationChanged(object sender, XmlConfigurationElementEventArgs e)
		{
			// bail if the element is being edited
			if (e.Element.IsBeingEdited)
				return;

			// we only care about options
			if (e.Element.GetElementType() == XmlConfigurationElementTypes.XmlConfigurationOption)
			{
				string[] optionNames = Enum.GetNames(typeof(AutoUpdateOptionNames));

				foreach (string name in optionNames)
				{
					string fullPath = string.Format("{0}\\{1}\\{2}", AllUsersConfigurationProvider.Current.ConfigurationName, ConfigurationCategoryName, name);

					if (string.Compare(e.Element.Fullpath, fullPath, true) == 0)
					{
						AutoUpdateOptionNames optionName = (AutoUpdateOptionNames)Enum.Parse(typeof(AutoUpdateOptionNames), name);

						try
						{
							this.BeginInit();

							XmlConfigurationOption option = (XmlConfigurationOption)e.Element;

							// read the value back out into our copy
							switch (optionName)
							{
								case AutoUpdateOptionNames.AutomaticallyCheckForUpdates:
									this.AutomaticallyCheckForUpdates = Convert.ToBoolean(option.Value);
									break;

								case AutoUpdateOptionNames.AutomaticallyDownloadUpdates:
									this.AutomaticallyDownloadUpdates = Convert.ToBoolean(option.Value);
									break;

								case AutoUpdateOptionNames.AutomaticallyInstallUpdates:
									this.AutomaticallyInstallUpdates = Convert.ToBoolean(option.Value);
									break;

								case AutoUpdateOptionNames.AutomaticallySwitchToNewVersion:
									this.AutomaticallySwitchToNewVersion = Convert.ToBoolean(option.Value);
									break;

								case AutoUpdateOptionNames.AutomaticallyUpdateAlternatePath:
									this.AutomaticallyUpdateAlternatePath = Convert.ToBoolean(option.Value);
									break;

								case AutoUpdateOptionNames.AlternatePath:
									this.AlternatePath = Convert.ToString(option.Value);
									break;

								case AutoUpdateOptionNames.WebServiceUrl:
									this.WebServiceUrl = Convert.ToString(option.Value);
									break;
							};
						}
						catch (Exception ex)
						{
							Debug.WriteLine(ex);
						}
						finally
						{
							this.EndInit();
						}

						// raise the changed event to signal that this option was changed
						EventManager.Raise<AutoUpdateOptionsEventArgs>(this.Changed, this, new AutoUpdateOptionsEventArgs(this, optionName));
						break;
					}
				}
			}
		}

		#region My Public Properties

		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should automatically check for updates
		/// </summary>
		public bool AutomaticallyCheckForUpdates
		{
			get
			{
				return _automaticallyCheckForUpdates;
			}
			set
			{
				_automaticallyCheckForUpdates = value;

				if (!_initializing)
				{
					Save(this, AutoUpdateOptions.DefaultOptions, AutoUpdateOptionNames.AutomaticallyCheckForUpdates);
				}
			}
		}

		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should automatically download updates
		/// </summary>
		public bool AutomaticallyDownloadUpdates
		{
			get
			{
				return _automaticallyDownloadUpdates;
			}
			set
			{
				_automaticallyDownloadUpdates = value;

				if (!_initializing)
				{
					Save(this, AutoUpdateOptions.DefaultOptions, AutoUpdateOptionNames.AutomaticallyDownloadUpdates);
				}
			}
		}

		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should automatically install updates
		/// </summary>
		public bool AutomaticallyInstallUpdates
		{
			get
			{
				return _automaticallyInstallUpdates;
			}
			set
			{
				_automaticallyInstallUpdates = value;

				if (!_initializing)
				{
					Save(this, AutoUpdateOptions.DefaultOptions, AutoUpdateOptionNames.AutomaticallyInstallUpdates);
				}
			}
		}

		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should automatically switch to the new version after installing
		/// </summary>
		public bool AutomaticallySwitchToNewVersion
		{
			get
			{
				return _automaticallySwitchToNewVersion;
			}
			set
			{
				_automaticallySwitchToNewVersion = value;

				if (!_initializing)
				{
					Save(this, AutoUpdateOptions.DefaultOptions, AutoUpdateOptionNames.AutomaticallySwitchToNewVersion);
				}
			}
		}
		
		/// <summary>
		/// Gets or sets a flag to indicate whether the engine should copy the .update to the alternate path after it installs the update
		/// </summary>
		public bool AutomaticallyUpdateAlternatePath
		{
			get
			{
				return _automaticallyUpdateAlternatePath;
			}
			set
			{
				_automaticallyUpdateAlternatePath = value;

				if (!_initializing)
				{
					Save(this, AutoUpdateOptions.DefaultOptions, AutoUpdateOptionNames.AutomaticallyUpdateAlternatePath);
				}
			}
		}

		/// <summary>
		/// Gets or sets the Unc path where the updates will be downloaded on the local system
		/// </summary>
		public string DownloadPath
		{
			get
			{
				return _downloadPath;
			}
			set
			{
				_downloadPath = value;
			}
		}

		/// <summary>
		/// Gets or sets a Unc path where updates may be downloaded or uploaded
		/// </summary>
		public string AlternatePath
		{
			get
			{
				return _alternatePath;
			}
			set
			{
				_alternatePath = value;

				if (!_initializing)
				{
					Save(this, AutoUpdateOptions.DefaultOptions, AutoUpdateOptionNames.AlternatePath);
				}
			}
		}

		/// <summary>
		/// Gets or sets the Url where the AutoUpdate web service can be located (Ex: http://mbelles-desktop/AutoUpdate/AutoUpdateWebService.asmx")
		/// </summary>
		public string WebServiceUrl
		{
			get
			{
				return _webServiceUrl;
			}
			set
			{
				_webServiceUrl = value;

				if (!_initializing)
				{
					Save(this, AutoUpdateOptions.DefaultOptions, AutoUpdateOptionNames.WebServiceUrl);
				}
			}
		}

		#endregion

		#region My Public Static Methods

		/// <summary>
		/// Loads the AutoUpdate options given a set of default options.
		/// </summary>
		/// <param name="defaultOptions">The default options to use if the options do not exist in the configuration files.</param>
		/// <returns></returns>
		public static AutoUpdateOptions Load(AutoUpdateOptions defaultOptions)
		{
			if (defaultOptions == null)
			{
				throw new ArgumentNullException("defaultOptions");
			}
			AutoUpdateOptions options = new AutoUpdateOptions();

			try
			{
				options.BeginInit();

				options.AutomaticallyCheckForUpdates = Convert.ToBoolean(Load(defaultOptions, AutoUpdateOptionNames.AutomaticallyCheckForUpdates));
				options.AutomaticallyDownloadUpdates = Convert.ToBoolean(Load(defaultOptions, AutoUpdateOptionNames.AutomaticallyDownloadUpdates));
				options.AutomaticallyInstallUpdates  = Convert.ToBoolean(Load(defaultOptions, AutoUpdateOptionNames.AutomaticallyInstallUpdates));
				options.AutomaticallySwitchToNewVersion = Convert.ToBoolean(Load(defaultOptions, AutoUpdateOptionNames.AutomaticallySwitchToNewVersion));
				options.AutomaticallyUpdateAlternatePath = Convert.ToBoolean(Load(defaultOptions, AutoUpdateOptionNames.AutomaticallyUpdateAlternatePath));
				options.AlternatePath = Convert.ToString(Load(defaultOptions, AutoUpdateOptionNames.AlternatePath));
				options.WebServiceUrl = Convert.ToString(Load(defaultOptions, AutoUpdateOptionNames.WebServiceUrl));
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				options.EndInit();
			}

			return options;
		}

		/// <summary>
		/// Loads the value of the option specified. Creates the option if it does not exist, and writes the default value to the option.
		/// </summary>
		/// <param name="defaultOptions">The default options to use to create the option if the option does not exist.</param>
		/// <param name="optionName">The name of the option to load.</param>
		/// <returns></returns>
		public static object Load(AutoUpdateOptions defaultOptions, AutoUpdateOptionNames optionName)
		{
			if (defaultOptions == null)
			{
				throw new ArgumentNullException("defaultOptions");
			}

			try
			{
				XmlConfiguration configuration = AllUsersConfigurationProvider.Current.Configuration;
				XmlConfigurationCategory category = configuration.Categories[ConfigurationCategoryName, true];
				XmlConfigurationOption option = null;

				switch (optionName)
				{				
					case AutoUpdateOptionNames.AutomaticallyCheckForUpdates:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							option = category.Options[optionName.ToString(), true, defaultOptions.AutomaticallyCheckForUpdates];
							option.DisplayName = @"Automatically Check for Updates";
							option.Category = @"General";
							option.Description = @"Determines whether the AutoUpdateManager automatically checks for updates on startup.";
						}
						break;

					case AutoUpdateOptionNames.AutomaticallyDownloadUpdates:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							option = category.Options[optionName.ToString(), true, defaultOptions.AutomaticallyDownloadUpdates];
							option.DisplayName = @"Automatically Download Updates";
							option.Category = @"General";
							option.Description = @"Determines whether the AutoUpdateManager automatically downloads available updates without prompting.";
						}
						break;

					case AutoUpdateOptionNames.AutomaticallyInstallUpdates:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							option = category.Options[optionName.ToString(), true, defaultOptions.AutomaticallyInstallUpdates];
							option.DisplayName = @"Automatically Install Updates";
							option.Category = @"General";
							option.Description = @"Determines whether the AutoUpdateManager automatically installs the updates after downloading.";
						}
						break;

					case AutoUpdateOptionNames.AutomaticallySwitchToNewVersion:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							option = category.Options[optionName.ToString(), true, defaultOptions.AutomaticallySwitchToNewVersion];
							option.DisplayName = @"Automatically Switch to Newest Version";
							option.Category = @"General";
							option.Description = @"Determines whether the AutoUpdateManager automatically switches to the newest version after installation.";
						}
						break;

					case AutoUpdateOptionNames.AutomaticallyUpdateAlternatePath:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							option = category.Options[optionName.ToString(), true, defaultOptions.AutomaticallyUpdateAlternatePath];
							option.DisplayName = @"Automatically Update Alternate Path";
							option.Category = @"General";
							option.Description = @"Determines whether the AutoUpdateManager automatically creates backup copies of the .Manifest and .Update files after installation.";
						}
						break;

					case AutoUpdateOptionNames.AlternatePath:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							option = category.Options[optionName.ToString(), true, defaultOptions.AlternatePath];
							option.DisplayName = @"Alternate Download Path";
							option.Category = @"General";
							option.Description = @"This alternate path (url or unc path) will be checked first before attempting to use the web service url to locate updates.";
							option.EditorAssemblyQualifiedName = typeof(FolderBrowserUITypeEditor).AssemblyQualifiedName;
						}
						break;

					case AutoUpdateOptionNames.WebServiceUrl:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							option = category.Options[optionName.ToString(), true, defaultOptions.WebServiceUrl];
							option.DisplayName = @"Web Service Url";
							option.Category = @"General";
							option.Description = @"The url specifying where the AutoUpdate Web Service can be located.";
						}
						break;
				};

				return option.Value;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return null;
		}

		/// <summary>
		/// Saves the AutoUpdate options specified.
		/// </summary>
		/// <param name="options">The options to retrieve the value of the option to save.</param>
		/// <param name="optionToSave">The name of the option to save.</param>
		public static void Save(AutoUpdateOptions options, AutoUpdateOptions defaultOptions, AutoUpdateOptionNames optionName)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (defaultOptions == null)
			{
				throw new ArgumentNullException("defaultOptions");
			}

			try
			{
				XmlConfiguration configuration = AllUsersConfigurationProvider.Current.Configuration;
				XmlConfigurationCategory category = configuration.Categories[ConfigurationCategoryName, true];
				XmlConfigurationOption option = null;

				// this could be refactored again
				// into another method, but i'm in a massive hurry today

				switch (optionName)
				{
					case AutoUpdateOptionNames.AutomaticallyCheckForUpdates:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							Load(defaultOptions, optionName);
							option = category.Options[optionName.ToString()];
						}
						option.Value = options.AutomaticallyCheckForUpdates;
						break;

					case AutoUpdateOptionNames.AutomaticallyDownloadUpdates:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							Load(defaultOptions, optionName);
							option = category.Options[optionName.ToString()];
						}
						option.Value = options.AutomaticallyDownloadUpdates;
						break;

					case AutoUpdateOptionNames.AutomaticallyInstallUpdates:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							Load(defaultOptions, optionName);
							option = category.Options[optionName.ToString()];
						}
						option.Value = options.AutomaticallyInstallUpdates;
						break;

					case AutoUpdateOptionNames.AutomaticallySwitchToNewVersion:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							Load(defaultOptions, optionName);
							option = category.Options[optionName.ToString()];
						}
						option.Value = options.AutomaticallySwitchToNewVersion;
						break;

					case AutoUpdateOptionNames.AutomaticallyUpdateAlternatePath:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							Load(defaultOptions, optionName);
							option = category.Options[optionName.ToString()];
						}
						option.Value = options.AutomaticallyUpdateAlternatePath;
						break;

					case AutoUpdateOptionNames.AlternatePath:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							Load(defaultOptions, optionName);
							option = category.Options[optionName.ToString()];
						}
						option.Value = options.AlternatePath;
						break;

					case AutoUpdateOptionNames.WebServiceUrl:
						if ((option = category.Options[optionName.ToString()]) == null)
						{
							Load(defaultOptions, optionName);
							option = category.Options[optionName.ToString()];
						}
						option.Value = options.WebServiceUrl;
						break;
				};
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Saves all of the AutoUpdate options.
		/// </summary>
		/// <param name="options"></param>
		public static void Save(AutoUpdateOptions options, AutoUpdateOptions defaultOptions)
		{
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (defaultOptions == null)
			{
				throw new ArgumentNullException("defaultOptions");
			}

			try
			{
				Save(options, defaultOptions, AutoUpdateOptionNames.AutomaticallyCheckForUpdates);
				Save(options, defaultOptions, AutoUpdateOptionNames.AutomaticallyDownloadUpdates);
				Save(options, defaultOptions, AutoUpdateOptionNames.AutomaticallyInstallUpdates);
				Save(options, defaultOptions, AutoUpdateOptionNames.AutomaticallySwitchToNewVersion);
				Save(options, defaultOptions, AutoUpdateOptionNames.AutomaticallyUpdateAlternatePath);
				Save(options, defaultOptions, AutoUpdateOptionNames.AlternatePath);
				Save(options, defaultOptions, AutoUpdateOptionNames.WebServiceUrl);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		#endregion
	}
}