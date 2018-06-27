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
using Carbon.Common;
using Carbon.UI;

namespace Carbon.Configuration.Providers
{
	/// <summary>
	/// Defines a class that is responsible for creating and managing configuration files.
	/// </summary>
	public abstract class ConfigurationProvider : Provider 
	{
        private XmlConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the ConfigurationProvider class.
        /// </summary>
        /// <param name="name">The name of the Provider.</param>
		protected ConfigurationProvider(string name) : base(name)
		{
	
		}

        /// <summary>
        /// Returns the name applied to the configuration.
        /// </summary>
        public abstract string ConfigurationName { get;}

        /// <summary>
        /// Returns the full path to the configuration file.
        /// </summary>
        public abstract string FullPath { get; }

        /// <summary>
        /// Returns the configuration provided by this ConfigurationProvider. This should be created
        /// when the Load method is called.
        /// </summary>
        public XmlConfiguration Configuration
        {
            get
            {
                return _configuration;
            }
            private set
            {
                _configuration = value;
            }
        }

        /// <summary>
        /// Returns the default configuration for this provider with the default format.
        /// (i.e., Categories and Options already created)
        /// </summary>
        protected virtual XmlConfiguration DefaultConfiguration 
        {
            get
            {
                var configuration = new XmlConfiguration();
                configuration.ElementName = ConfigurationName;                
                return configuration;
            }
        }

        /// <summary>
        /// Occurs when the default configuration is needed to initialize a new configuration from memory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void FormatConfiguration(object sender, XmlConfigurationEventArgs e)
        {
            e.Element = DefaultConfiguration;
        }

        /// <summary>
        /// Called when the provider needs to load or save the configuration. By default this method returns null, 
        /// and the configuration files are in cleartext on disk.
        /// </summary>
        /// <returns></returns>
        protected virtual FileEncryptionEngine GetEncryptionEngine()
        {
            return null;
        }
        
        /// <summary>
        /// Called when the provider should load the configuration it is managing.
        /// </summary>
        public virtual void Load(IProgressViewer progressViewer)
        {
            // read or create the local user configuration
            ProgressViewer.SetExtendedDescription(progressViewer, $"Loading '{ConfigurationName}' configuration...");
            
            ConfigurationProvidersManager.ReadOrCreateConfiguration(
                CarbonConfig.Verbose, 
                ConfigurationName, 
                FullPath, 
                out _configuration, 
                GetEncryptionEngine(),
                FormatConfiguration);

            if (Configuration != null)
            {
                Configuration.TimeToSave += OnConfigurationTimeToSave;

                // by default we'll add this to the list so that the configuration shows up in the options dialog
                ConfigurationProvidersManager.EnumeratingConfigurations += OnConfigurationProvidersManagerEnumeratingConfigurations;
            }
        }
                
        /// <summary>
        /// Called when the provider should save the configuration it is managing.
        /// </summary>
        public virtual void Save()
        {
            ConfigurationProvidersManager.WriteConfiguration(
                CarbonConfig.Verbose, 
                FullPath, 
                Configuration,
                GetEncryptionEngine());
        }

        /// <summary>
        /// Occurs when it is time to save the configuration.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConfigurationTimeToSave(object sender, EventArgs e)
        {
            Save();
        }

        /// <summary>
        /// Occurs when the configuration providers manager enumerates configuration files to display them.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnConfigurationProvidersManagerEnumeratingConfigurations(object sender, XmlConfigurationManagerEventArgs e)
        {
            e.Configurations.Add(Configuration);
        }
	}
}
