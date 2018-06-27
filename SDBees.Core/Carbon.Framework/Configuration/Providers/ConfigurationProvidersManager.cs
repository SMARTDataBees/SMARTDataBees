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
using System.IO;
using System.Windows.Forms;
using Carbon.Common;
using Carbon.UI;

namespace Carbon.Configuration.Providers
{
    /// <summary>
    /// Defines a class with methods used to manage Configuration Providers and Load/Save configuration files.
    /// </summary>
    public static class ConfigurationProvidersManager
    {
        private static Exception _lastException;
        private readonly static string ConfigurationWindowKey = "{a12210b1-7198-403f-b4c8-273197444a30}";

        /// <summary>
        /// Fires when the XmlConfigurationManager is enumerating configurations
        /// </summary>
        // public static event XmlConfigurationManagerEventHandler EnumeratingConfigurations;
		public static event EventHandler<XmlConfigurationManagerEventArgs> EnumeratingConfigurations;
        
        /// <summary>
        /// Allows a client to retrieve all of the configurations to be displayed in a configuration properties window, or for other purposes. All SnapIns should listen for the for the EnumeratingConfigurations event, and add any configurations to the event as needed.
        /// </summary>
        /// <param name="configurations">An array of configurations that can be initially added so that the event listeners may filter them.</param>
        /// <returns></returns>
        public static XmlConfiguration[] EnumConfigurations(params XmlConfiguration[] configurations)
        {
            if (configurations == null)
                configurations = new XmlConfiguration[] { };

            var e = new XmlConfigurationManagerEventArgs(configurations);
            OnEnumeratingConfigurations(null, e);

            return e.Configurations.ToArray();
        }
        
        /// <summary>
        /// Raises the EnumeratingConfigurations event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnEnumeratingConfigurations(object sender, XmlConfigurationManagerEventArgs e)
        {
            try
            {
                if (EnumeratingConfigurations != null)
					EnumeratingConfigurations(sender, e);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
            }
        }

        /// <summary>
        /// Shows a ConfigurationWindow as a modal dialog with the specified owner
        /// </summary>
        /// <param name="owner">The window that will own the dialog when it is shown</param>
        /// <returns></returns>
        public static DialogResult ShowConfigurationWindow(IWin32Window owner)
        {
            var result = DialogResult.Cancel;

            // create a new xml configuration properties window
            var window = new XmlConfigurationPropertiesWindow();

            // ask the window manager if we can show the window
            if (WindowManager.Current.CanShow(window))
            {
                // it can be shown, so now let us enumerate the available configurations
                var configurations = EnumConfigurations();

                // ask the window manager to track this window for us
                WindowManager.Current.BeginTrackingLifetime(window, ConfigurationWindowKey);

                // select the available configurations into the xml configuration properties window
                window.SelectedConfigurations = new XmlConfigurationCollection(configurations);

                // display a local warning if permissions aren't sufficient
                window.XmlConfigurationView.DisplayWarningIfLocalFilePermissionsAreInsufficient();

                // show the window modally
                result = (owner == null ? window.ShowDialog() : window.ShowDialog(owner));
            }

            return result;
        }

        /// <summary>
        /// Shows a ConfigurationWindow as a modal dialog with the specified owner
        /// </summary>
        /// <param name="owner">The window that will own the dialog when it is shown</param>
        /// <returns></returns>
        public static DialogResult ShowConfigurationWindow(IWin32Window owner, XmlConfigurationPropertiesWindow window)
        {
            var result = DialogResult.Cancel;

            // ask the window manager if we can show the window
            if (WindowManager.Current.CanShow(window))
            {
                // it can be shown, so now let us enumerate the available configurations
                var configurations = EnumConfigurations();

                // ask the window manager to track this window for us
                WindowManager.Current.BeginTrackingLifetime(window, ConfigurationWindowKey);

                // select the available configurations into the xml configuration properties window
                window.SelectedConfigurations = new XmlConfigurationCollection(configurations);

                // display a local warning if permissions aren't sufficient
                window.XmlConfigurationView.DisplayWarningIfLocalFilePermissionsAreInsufficient();

                // show the window modally
                result = (owner == null ? window.ShowDialog() : window.ShowDialog(owner));
            }

            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="configurationProviders"></param>
        public static void InstructConfigurationProvidersToLoad(IProgressViewer progressViewer, ConfigurationProviderCollection configurationProviders)
        {
            foreach (ConfigurationProvider provider in configurationProviders)
            {
                try
                {
                    provider.Load(progressViewer);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="configurationProviders"></param>
        public static void InstructConfigurationProvidersToSave(ConfigurationProviderCollection configurationProviders)
        {
            foreach (ConfigurationProvider provider in configurationProviders)
            {
                try
                {
                    provider.Save();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Writes a configuration using the specified encryption engine to the specified path
        /// </summary>
        /// <param name="encryptionEngine">The encryption engine to use while writing the configuration, null if no encryption is desired</param>
        /// <param name="configuration">The confiruration to write</param>
        /// <param name="path">The path to write it to</param>
        /// <returns></returns>
        public static bool WriteConfiguration(FileEncryptionEngine encryptionEngine, XmlConfiguration configuration, string path)
        {
            Stream stream = null;
            _lastException = null;

            try
            {
                if (configuration != null)
                {
                    if (configuration.HasUnpersistedChanges())
                    {
                        configuration.AcceptChanges();
                        stream = (encryptionEngine != null ? encryptionEngine.CreateEncryptorStream(path) : new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None));
                        var writer = new XmlConfigurationWriter();
                        writer.Write(configuration, stream, false);
                        configuration.SetHasUnpersistedChanges(false);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
                _lastException = ex;
            }
            finally
            {
                if (stream != null) 
                    stream.Close();
            }
            return false;
        }

        /// <summary>
        /// Reads a configuration using the specified encryption engine from the specified path
        /// </summary>
        /// <param name="encryptionEngine">The encryption engine to use while reading the configuration, null if no decryption is desired</param>
        /// <param name="configuration">The configuration to be read into</param>
        /// <param name="path">The path to be read from</param>
        /// <returns></returns>
        public static bool ReadConfiguration(FileEncryptionEngine encryptionEngine, out XmlConfiguration configuration, string path)
        {
            Stream stream = null;
            _lastException = null;

            try
            {
                configuration = new XmlConfiguration();
                stream = (encryptionEngine != null ? encryptionEngine.CreateDecryptorStream(path) : new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None));
                var reader = new XmlConfigurationReader();
                configuration = reader.Read(stream);
                configuration.Path = path;
                configuration.SetHasUnpersistedChanges(false);

                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
                _lastException = ex;
            }
            finally
            {
                if (stream != null) 
                    stream.Close();
            }
            configuration = null;
            return false;
        }

        /// <summary>
        /// Writes a configuration to a path using the specified encryption engine. Takes windows security into account and checks for write access before trying to write to the path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="configuration"></param>
        /// <param name="encryptionEngine"></param>
        /// <returns></returns>
        public static bool WriteConfiguration(bool verbose, string path, XmlConfiguration configuration, FileEncryptionEngine encryptionEngine)
        {
            try
            {
                Log.WriteLineIf(verbose, "Checking to see if the path '" + path + "' exists.");

                // if the file exists, we need to try and read it
                if (File.Exists(path))
                {
                    Log.WriteLineIf(verbose, "The path '" + path + "' exists.");

                    // but first see if we have permissions to read it
                    using (var right = new SecurityAccessRight(path))
                    {
                        Log.WriteLineIf(verbose, "Checking to see if the path '" + path + "' has write access.");

                        // if we don't have rights to the file						
                        if (!right.AssertWriteAccess())
                        {
                            Log.WriteLineIf(verbose, "The path '" + path + "' does not have write access.");
                            Log.WriteLineIf(verbose, "Prompting for user intervention for the path '" + path + "'.");

                            if (verbose)
                            {
                                // prompt to see what we should do about this
                                var result = ExceptionUtilities.DisplayException(
                                    null,
                                    "Write access denied - Unable to write to file",
                                    MessageBoxIcon.Error,
                                    MessageBoxButtons.AbortRetryIgnore,
                                    null,
                                    "Write access has been denied for the file '" + path + "'.",
                                    "Ignoring this exception may result in a loss of data if any options in this file were changed.");

                                switch (result)
                                {
                                    case DialogResult.Abort:
                                        Log.WriteLineIf(verbose, "Aborting attempt to write to the path '" + path + "' because of user intervention.");
                                        return false;

                                    case DialogResult.Retry:
                                        Log.WriteLineIf(verbose, "Retrying attempt to write to the path '" + path + "' because of user intervention.");
                                        return WriteConfiguration(verbose, path, configuration, encryptionEngine);

                                    case DialogResult.Ignore:
                                        Log.WriteLineIf(verbose, "Ignoring attempt to write to the path '" + path + "' because of user intervention.");
                                        return true;
                                    //break;					
                                };
                            }
                            else
                            {
                                // it failed, but we're not in verbose mode so who cares?
                                return true;
                            }
                        }
                        else
                        {
                            Log.WriteLineIf(verbose, "The path '" + path + "' has write access, preparing to write the configuration.");

                            // rights to write to the file
                            // ask the configuration engine to write our configuration file for us into our configuration 
                            if (!WriteConfiguration(encryptionEngine, configuration, path))
                            {
                                Log.WriteLineIf(verbose, "Failed to write the configuration, throwing exception from the last operation.");
                                throw _lastException;
                            }

                            // ensure that the configuration has no changes visible
                            if (configuration != null)
                            {
                                Log.WriteLineIf(verbose, "Succeeded in writing the configuration, accepting changes .");
                                configuration.AcceptChanges();
                            }

                            return true;
                        }
                    }
                }
                else
                {
                    Log.WriteLineIf(verbose, "The path '" + path + "' does not exist, preparing to write the configuration for the first time.");

                    // ask the configuration engine to write our configuration file for us into our configuration 
                    if (!WriteConfiguration(encryptionEngine, configuration, path))
                    {
                        Log.WriteLineIf(verbose, "Failed to write the configuration, throwing exception from the last operation.");
                        throw _lastException;
                    }

                    // ensure that the configuration has no changes visible
                    if (configuration != null)
                    {
                        Log.WriteLineIf(verbose, "Succeeded in writing the configuration, accepting changes .");
                        configuration.AcceptChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLineIf(verbose, "An unexpected exception was encountered while writing the configuration, dumping exception.");
                Log.WriteLine(ex);
                Log.WriteLineIf(verbose, "Prompting for user intervention for the path '" + path + "'.");

                if (verbose)
                {
                    // failed for some reason writing the file
                    // prompt to see what we should do about this
                    var result = ExceptionUtilities.DisplayException(
                        null,
                        "Exception encountered - Unable to write to file",
                        MessageBoxIcon.Error,
                        MessageBoxButtons.AbortRetryIgnore,
                        ex,
                        "An exception was encountered while trying to write to the file '" + path + "'.",
                        "Ignoring this exception may result in a loss of data if any options in this file were changed");

                    switch (result)
                    {
                        case DialogResult.Abort:
                            Log.WriteLineIf(verbose, "Aborting attempt to write to the path '" + path + "' because of user intervention.");
                            return false;

                        case DialogResult.Retry:
                            Log.WriteLineIf(verbose, "Retrying attempt to write to the path '" + path + "' because of user intervention.");
                            return WriteConfiguration(verbose, path, configuration, encryptionEngine);

                        case DialogResult.Ignore:
                            Log.WriteLineIf(verbose, "Ignoring attempt to write to the path '" + path + "' because of user intervention.");
                            return true;
                    };
                }
            }
            return true;
        }

        /// <summary>
        /// Reads or creates an XmlConfiguration from a name, path, and/or a handler function to provide structure to a new configuration.
        /// </summary>
        /// <param name="name">The name that will be given to the configuration</param>
        /// <param name="path">The path to the file where the configuration is stored</param>
        /// <param name="configuration">The configuration that will be returned after creation or reading has finished</param>
        /// <param name="encryptionEngine">The encryption engine to use when reading the file</param>
        /// <param name="handler">The event handler to call if structure is needed for a new configuration</param>
        /// <returns>True if a configuration was created or read</returns>
        public static bool ReadOrCreateConfiguration(
            bool verbose,
            string name,
            string path,
            out XmlConfiguration configuration,
            FileEncryptionEngine encryptionEngine,
            XmlConfigurationEventHandler handler)
        {
            configuration = null;

            Log.WriteLineIf(verbose, "Checking to see if the path '" + path + "' exists.");

            // if the file exists, we need to try and read it
            if (File.Exists(path))
            {
                Log.WriteLineIf(verbose, "The path '" + path + "' exists.");

                try
                {
                    // but first see if we have permissions to read it
                    using (var right = new SecurityAccessRight(path))
                    {
                        Log.WriteLineIf(verbose, "Checking to see if the path '" + path + "' has read access.");

                        // if we don't have rights to the file
                        if (!right.AssertReadAccess())
                        {
                            Log.WriteLineIf(verbose, "The path '" + path + "' does not have write access.");
                            Log.WriteLineIf(verbose, "Prompting for user intervention for the path '" + path + "'.");

                            // prompt to see what we should do about this
                            var result = ExceptionUtilities.DisplayException(
                                null,
                                "Read access denied - Unable to read from file",
                                MessageBoxIcon.Error,
                                MessageBoxButtons.AbortRetryIgnore,
                                null,
                                "Read access has been denied for the '" + name + "'.",
                                "Ignoring this exception will result in a default set of options to be loaded in the '" + name + "' for this application instance.",
                                "If the file has write access enabled, it will be overwritten with the loaded options when the application exits.",
                                "WARNING: Aborting this operation will exit the application!");

                            switch (result)
                            {
                                case DialogResult.Abort:
                                    Log.WriteLineIf(verbose, "Aborting attempt to read from the path '" + path + "' because of user intervention.");
                                    return false;

                                case DialogResult.Retry:
                                    Log.WriteLineIf(verbose, "Retrying attempt to read from the path '" + path + "' because of user intervention.");
                                    return ReadOrCreateConfiguration(verbose, name, path, out configuration, encryptionEngine, handler);

                                case DialogResult.Ignore:
                                    Log.WriteLineIf(verbose, "Ignoring attempt to read from the path '" + path + "' because of user intervention.");
                                    return true;
                                //break;					
                            };
                        }
                        else
                        {
                            Log.WriteLineIf(verbose, "The path '" + path + "' has read access, preparing to read the configuration.");

                            // rights to read the file
                            // ask the configuration engine to read our configuration file for us into our configuration 
                            if (!ReadConfiguration(encryptionEngine, out configuration, path))
                            {
                                Log.WriteLineIf(verbose, "Failed to write the configuration, throwing exception from the last operation.");
                                throw _lastException;
                            }

                            // ensure that the configuration has no changes visible
                            if (configuration != null)
                            {
                                Log.WriteLineIf(verbose, "Succeeded in reading the configuration, accepting changes .");
                                configuration.AcceptChanges();
                            }

                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLineIf(verbose, "An unexpected exception was encountered while reading the configuration, dumping exception.");
                    Log.WriteLine(ex);
                    Log.WriteLineIf(verbose, "Prompting for user intervention for the path '" + path + "'.");

                    // failed for some reason reading the file
                    // prompt to see what we should do about this
                    var result = ExceptionUtilities.DisplayException(
                        null,
                        "Exception encountered - Unable to read from file",
                        MessageBoxIcon.Error,
                        MessageBoxButtons.AbortRetryIgnore,
                        ex,
                        "An exception was encountered while trying to read '" + name + "'.",
                        "Ignoring this exception will result in a default set of options to be loaded in the '" + name + "' for this application instance.",
                        "If the file has write access enabled, it will be overwritten with the loaded options when the application exits.",
                        "WARNING: Aborting this operation will exit the application!");

                    switch (result)
                    {
                        case DialogResult.Abort:
                            Log.WriteLineIf(verbose, "Aborting attempt to read from the path '" + path + "' because of user intervention.");
                            return false;

                        case DialogResult.Retry:
                            Log.WriteLineIf(verbose, "Retrying attempt to read from the path '" + path + "' because of user intervention.");
                            return ReadOrCreateConfiguration(verbose, name, path, out configuration, encryptionEngine, handler);

                        case DialogResult.Ignore:
                            Log.WriteLineIf(verbose, "Ignoring attempt to read from the path '" + path + "' because of user intervention.");
                            break;
                    };
                }
            }
            else
            {
                Log.WriteLineIf(verbose, "The path '" + path + "' does not exist.");
            }

            // if for some reason the configuration hasn't been loaded yet
            if (configuration == null)
            {
                Log.WriteLineIf(verbose, "Creating new configuration named '" + name + "'.");
                configuration = new XmlConfiguration();
                configuration.ElementName = name;

                Log.WriteLineIf(verbose, "Checking for formatting callback for the configuration named '" + name + "'.");

                if (handler != null)
                {
                    Log.WriteLineIf(verbose, "Formatting callback found for the configuration named '" + name + "', calling formatting callback to apply structure to the configuration.");
                    try
                    {
                        var e = new XmlConfigurationEventArgs(configuration, XmlConfigurationElementActions.None);
                        handler(null, e);
                        configuration = e.Element;
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLineIf(verbose, "An unexpected exception was encountered while reading the configuration named '" + name + "', dumping exception.");
                        Log.WriteLine(ex);
                    }
                }
            }

            Log.WriteLineIf(verbose, "Setting the path for the configuration named '" + name + "' and accepting changes to the configuration.");

            // let the configuration know where it lives
            configuration.Path = path;

            // ensure that the configuration has no changes visible
            configuration.AcceptChanges();

            return true;
        }     
    }
}
