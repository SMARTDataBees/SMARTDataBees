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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Carbon.AutoUpdate.Common;
using Carbon.AutoUpdate.Common.Xml;
using Carbon.MultiThreading;
using Carbon.Plugins;
using Carbon.UI;
using ICSharpCode.SharpZipLib.Zip;

namespace Carbon.AutoUpdate
{	
    /// <summary>
    /// Provides a class for managing and implementing the AutoUpdate process. 
    /// </summary>
    public class AutoUpdateManager 
    {
        /// <summary>
        /// The options used to control the behaviour of the manager and downloaders
        /// </summary>
        private AutoUpdateOptions _options;			

        /// <summary>
        /// The list of downloaders the manager will try to use to download later versions of the specified product
        /// </summary>
		private AutoUpdateDownloaderList _downloaders;

        /// <summary>
        /// The description of the product to be updated
        /// </summary>
		private AutoUpdateProductDescriptor _productToUpdate;

        /// <summary>
        /// The background thread upon which all work in the auto update process will occur
        /// </summary>
		private BackgroundThread _thread;

		private const string MY_TRACE_CATEGORY = @"'AutoUpdateManager'";

        #region My Public Events

        /// <summary>
        /// Occurs when the manager has started with its auto update process
        /// </summary>
        public event AutoUpdateManagerEventHandler AutoUpdateProcessStarted;

        /// <summary>
        /// Occurs before the manager queries the downloaders for their latest version
        /// </summary>
        public event AutoUpdateManagerCancelEventHandler BeforeQueryForLatestVersion;	
		
        /// <summary>
        /// Occurs after the manager has queried the downloaders and determined the latest version available 
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler AfterQueryForLatestVersion;	

        /// <summary>
        /// Occurs when no later version is available for download
        /// </summary>
        public event AutoUpdateManagerEventHandler NoLaterVersionAvailable;								

        /// <summary>
        /// Occurs before the manager downloads an update
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorCancelEventHandler BeforeDownload;			

        /// <summary>
        /// Occurs after the manager has downloaded an update
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler AfterDownload;				

        /// <summary>
        /// Occurs before the manager installs an update
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorCancelEventHandler BeforeInstall;			

        /// <summary>
        /// Occurs after the manager has installed an update
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler AfterInstall;					

        /// <summary>
        /// Occurs before the manager attempts to copy the update to the alternate path
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorCancelEventHandler BeforeUpdateAlternatePath;

        /// <summary>
        /// Occurs after the manager has updated the alternate path
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler AfterUpdateAlternatePath;

        /// <summary>
        /// Occurs before the manager switches to the latest version
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorCancelEventHandler BeforeSwitchToLatestVersion;		

        /// <summary>
        /// Occurs when the manager is ready to allow the bootstrap to switch to the latest version
        /// </summary>
        public event AutoUpdateManagerWithDownloadDescriptorEventHandler SwitchToLatestVersion;

        /// <summary>
        /// Occurs when the manager encounters an exception
        /// </summary>
        public event AutoUpdateExceptionEventHandler Exception;

        /// <summary>
        /// Occurs when the manager is finished with it's auto update process
        /// </summary>
        public event AutoUpdateManagerEventHandler AutoUpdateProcessEnded;									

        #endregion

        /// <summary>
        /// Initializes a new instance of the AutoUpdateManager class
        /// </summary>
        /// <param name="options">The options that will control the behaviour of the engine</param>
        public AutoUpdateManager(AutoUpdateOptions options) 
        {
            // we can't do anything without options to control our behavior

            /*
             * the default options will be used
             * to update the current hosting engine 
             * and download into the bootstrap directory along side the other versions of this hosting engine
             * */
            _options = options ?? throw new ArgumentNullException("options");					
            _productToUpdate = AutoUpdateProductDescriptor.FromAssembly(PluginContext.Current.StartingAssembly, PluginContext.Current.AppVersion);			
            _downloaders = new AutoUpdateDownloaderList();
            _downloaders.AddRange(CreateDownloadersForInternalUse());
            if (_options.DownloadPath == null || _options.DownloadPath == string.Empty)
                _options.DownloadPath = GetBootstrapPath();		
        }

        /// <summary>
        /// Initializes a new instance of the AutoUpdateManager class
        /// </summary>
        /// <param name="options">The options that will control the behaviour of the engine</param>
        /// <param name="productToUpdate">A product descriptor that will be used as the product to find updates for</param>
        public AutoUpdateManager(AutoUpdateOptions options, AutoUpdateProductDescriptor productToUpdate)
        {
			if (options == null)
			{
				throw new ArgumentNullException("options");
			}

			if (productToUpdate == null)
			{
				throw new ArgumentNullException("productToUpdate");
			}

            /*
             * however if we wanted to not do the norm
             * and create an update engine that could update another app
             * then we are all about it, makes no never mind at all
             * */
            _options = options;
            _productToUpdate = productToUpdate;
            _downloaders = new AutoUpdateDownloaderList();
            _downloaders.AddRange(CreateDownloadersForInternalUse());
            if (_options.DownloadPath == null || _options.DownloadPath == string.Empty)
                _options.DownloadPath = GetBootstrapPath();		
        }	

        #region My Public Properties

        /// <summary>
        /// Gets or sets the options used by the AutoUpdateManager
        /// </summary>
        public AutoUpdateOptions Options
        {
            get
            {
                return _options;
            }
            set
            {
                _options = value ?? throw new ArgumentNullException($"options");
                if (string.IsNullOrEmpty(_options.DownloadPath))
                    _options.DownloadPath = GetBootstrapPath();
            }
        }
		
        /// <summary>
        /// Gets or sets the product descriptor that describes the product that the autoupdate engine will try and find updates for
        /// </summary>
        public AutoUpdateProductDescriptor ProductToUpdate
        {
            get => _productToUpdate;
            set => _productToUpdate = value ?? throw new ArgumentNullException(@"productToUpdate");
        }
		
        /// <summary>
        /// Gets or sets the list of downloads the manager will use to find and download updates for the product specified
        /// </summary>
        public AutoUpdateDownloaderList Downloaders
        {
            get
            {
                return _downloaders;
            }
            set
            {
				if (value == null)
				{
					throw new ArgumentNullException("Downloaders");
				}

                _downloaders = value;
            }
        }

        /// <summary>
        /// Returns a flag that indicates whether the engine is running (ie. checking, downloading, intalling... that sort of thing)
        /// </summary>
        public bool IsRunning
        {
            get
            {
                if (_thread == null)
                    return false;

                return _thread.IsRunning;
            }
        }

        #endregion 
		
        #region My Public Methods

        /// <summary>
        /// Asyncronously begins a background thread which maintains a list of AutoUpdateManagers to check for available updates
        /// </summary>
        public void BeginCheckingForUpdates()
        {	           
            // if the thread is null reset it
            if (_thread == null)
            {
                // each instance of the engine will use a background thread to perform it's work
                _thread = new BackgroundThread();
                _thread.Run += OnThreadRun;
                _thread.Finished += OnThreadFinished;	
				//_thread.AllowThreadAbortException = true;
            }

            // if the thread is not running
            if (!_thread.IsRunning)
                // start it up
                _thread.Start(true, new object[] {});			
        }

        /// <summary>
        /// Syncronously ends a previous call that began the check for updates
        /// </summary>
        public void EndCheckingForUpdates()
        {
            // if the thread is running 
			if (_thread == null)
				return;

            if (_thread.IsRunning)
                // stut it down
                _thread.Stop();
        }
						
        #endregion

        #region My Protected Options		

        /// <summary>
        /// Returns the product's bootstrap path, which will be where all .update files are downloaded and installed from.
        /// </summary>
        /// <returns></returns>
        public virtual string GetBootstrapPath()
        {
            // look at the startup directory
            var directory = new DirectoryInfo(Application.StartupPath);
	                
            // jump to it's parent, that is going to be the download path for all updates (*.update)
            return Path.GetDirectoryName(directory.FullName);
        }

        /// <summary>
        /// Returns an array of downloads that this instance of the AutoUpdateManager 
        /// </summary>
        /// <returns></returns>
        protected virtual AutoUpdateDownloader[] CreateDownloadersForInternalUse()
        {
            // create an array of downloaders 
            return new AutoUpdateDownloader[] {new UncPathAutoUpdateDownloader(), new HttpAutoUpdateDownloader()};
        }

        /// <summary>
        /// Sorts the available downloads and returns the newest download descriptor as the one that should be downloaded
        /// </summary>
        /// <param name="updates"></param>
        /// <returns></returns>
        protected virtual AutoUpdateDownloadDescriptor SelectTheDownloadWithTheNewestUpdate(AutoUpdateDownloadDescriptor[] downloadDescriptors)
        {
            try
            {
                // if there are no downloads
                if (downloadDescriptors == null)
                    // then simply say so
                    return null;

                if (downloadDescriptors.Length > 0)
                {
                    // otherwise, sort them into descending order with the newest version at index zero.
                    downloadDescriptors = AutoUpdateDownloadDescriptor.Sort(downloadDescriptors);
				
                    // simply return the first one
                    return downloadDescriptors[0];
                }
            }
            catch(ThreadAbortException)
            {

            }

            // otherwise don't
            return null;
        }

        /// <summary>
        /// Installs the .update file specified by decrypting it and then unziping the contents to a new versioned directory (ie. 1.0.0.1)
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="downloadDescriptor"></param>
        /// <returns></returns>
        protected virtual bool InstallUpdate(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor)
        {
            string zipFilename = null;
            try
            {
                Debug.WriteLine($"Preparing to install update from '{downloadDescriptor.DownloadedPath}'.", MY_TRACE_CATEGORY);

                // decrypt the .update file first				
                if (!DecryptToZip(progressViewer, downloadDescriptor, out zipFilename))
                    return false;

                // then unzip the .zip file
                if (Unzip(progressViewer, downloadDescriptor, zipFilename))
                {	// delete the zip file
                    File.Delete(zipFilename);
                    return true;
                }
            }
            catch(ThreadAbortException)
            {
                try
                {
                    // delete the zip file
                    File.Delete(zipFilename);
                }
                catch(Exception ex)
                {
					Debug.WriteLine(ex);
                }
            }

            return false;
        }

        /// <summary>
        /// Decrypts the .update file to a .zip file using the name of the .update file as a base in the same directory as the .update file
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="downloadDescriptor"></param>
        /// <param name="zipFilename"></param>
        /// <returns></returns>
        protected virtual bool DecryptToZip(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor, out string zipFilename)
        {
            zipFilename = null;
            try
            {						
                ProgressViewer.SetExtendedDescription(progressViewer, "Parsing update...");

                // save the path to the update
                var updateFilename = downloadDescriptor.DownloadedPath;

                // format the .zip file
                zipFilename = Path.Combine(Path.GetDirectoryName(updateFilename), Path.GetFileName(updateFilename).Replace(Path.GetExtension(updateFilename), null) + ".zip");
				
				/*
				 * Dev Note: Versions 1 & 2 encrypted the zip files. Version 3 in Carbon will not.
				 * Just copy the update to the zip location.
				 **/
				File.Copy(updateFilename, zipFilename);

				//// it is rijndael encrypted
				//RijndaelEncryptionEngine ee = new RijndaelEncryptionEngine();
								
				//// decrypt the .update file to a .zip file
				//Debug.WriteLine(string.Format("Converting the update into an archive.\n\tThe archive will exist at '{0}'.", zipFilename), MY_TRACE_CATEGORY);
				//ee.Decrypt(updateFilename, zipFilename);				
				
                return true;
            }
            catch(ThreadAbortException)
            {

            }
            return false;
        }

        /// <summary>
        /// Runs the specified file if it exists
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="file"></param>
        protected virtual void RunFileIfFound(IProgressViewer progressViewer, string file)
        {
            try
            {
                var fileExists = File.Exists(file);
                Debug.WriteLine(
                    $"Preparing to start a process.\n\tThe file '{file}' {(fileExists ? "exists" : "does not exist")}.", MY_TRACE_CATEGORY);				

                if (fileExists)
                {
                    ProgressViewer.SetExtendedDescription(progressViewer,
                        $"Creating process '{Path.GetFileName(file)}'...");

                    var processStartInfo = new ProcessStartInfo
                    {
                        FileName = file,
                        WorkingDirectory = new FileInfo(file).DirectoryName
                    };


                    var p = Process.Start(processStartInfo);
                    p?.WaitForExit();
                }	
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                ProgressViewer.SetExtendedDescription(progressViewer, null);
            }
        }

        /// <summary>
        /// Unzips the .zip file to a directory of it's own using the name of the .zip file as a base
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="zipFilename"></param>
        /// <returns></returns>
		protected virtual bool Unzip(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor, string zipFilename)
		{
			ZipInputStream zipStream = null;
			FileStream fs = null;
			string newVersionPath = null;

			try
			{
				// calculate the rootname 
				//string rootName = Path.GetFileName(zipFilename).Replace(Path.GetExtension(zipFilename), null);

				//// the text to remove includes the name of the product and a dash
				//string prependedTextToRemove = string.Format("{0}-", downloadDescriptor.Manifest.Product.Name);

				//// remove that and we're left with a version
				//rootName = rootName.Replace(prependedTextToRemove, null);

				// extract here
				var rootPath = Path.GetDirectoryName(zipFilename);

				// the destination where the files will be unzipped for the new version
				//newVersionPath = Path.Combine(rootPath, rootName);
				newVersionPath = Path.Combine(rootPath, downloadDescriptor.Manifest.Product.Version.ToString());

				// make sure the directory where the new version will be extracted exists
				var folderExists = Directory.Exists(newVersionPath);
				Debug.WriteLine(
				    $"Confirming the new version's path.\n\tThe folder '{newVersionPath}' {(folderExists ? "already exists" : "does not exist")}.", MY_TRACE_CATEGORY);
				if (!folderExists)
				{
					Debug.WriteLine($"Creating the new verion's folder '{newVersionPath}'.", MY_TRACE_CATEGORY);
					Directory.CreateDirectory(newVersionPath);
				}

				// try and find the postbuildevent.bat file
				var postBuildFile = Path.Combine(rootPath, "PostBuildEvent.bat");

				// open the zip file using a zip input stream
				Debug.WriteLine($"Opening the archive '{zipFilename}' for reading.", MY_TRACE_CATEGORY);
				zipStream = new ZipInputStream(File.OpenRead(zipFilename));

				// ready each zip entry
				ZipEntry zipEntry;
				while ((zipEntry = zipStream.GetNextEntry()) != null)
				{
					try
					{
						var zipEntryFilename = Path.Combine(rootPath, zipEntry.Name);
						zipEntryFilename = zipEntryFilename.Replace("/", "\\");

						// trace the entry to where it is going
						Debug.WriteLine($"Extracting '{zipEntry.Name}' to '{zipEntryFilename}'.", MY_TRACE_CATEGORY);

						if (zipEntry.IsDirectory)
						{
							Debug.WriteLine($"Creating the folder '{zipEntryFilename}'.", MY_TRACE_CATEGORY);
							Directory.CreateDirectory(zipEntryFilename);
						}
						else
						{
							ProgressViewer.SetExtendedDescription(progressViewer, "Extracting " + zipEntry.Name + "...");

							// make sure the directory exists
							var fi = new FileInfo(zipEntryFilename);
							var di = fi.Directory;
							if (!Directory.Exists(di.FullName))
								Directory.CreateDirectory(di.FullName);
							fi = null;
							di = null;

							// create each file
							fs = File.Create(zipEntryFilename);
							var size = 2048;
							var data = new byte[size];
							while (true)
							{
								size = zipStream.Read(data, 0, data.Length);
								if (size > 0)
									fs.Write(data, 0, size);
								else
									break;
							}
							// close the extracted file
							fs.Close();
							fs = null;
						}
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
				}

				// close the zip stream
				zipStream.Close();

				RunFileIfFound(progressViewer, postBuildFile);

				return true;
			}
			catch (ThreadAbortException)
			{
				try
				{
					// make sure the streams are closed
					if (zipStream != null)
						zipStream.Close();

					if (fs != null)
						fs.Close();

					// delete the root folder of the new install upon cancellation
					Directory.Delete(newVersionPath, true);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				// make sure the streams are closed
				if (zipStream != null)
					zipStream.Close();

				if (fs != null)
					fs.Close();
			}

			return false;
		}

        /// <summary>
        /// Adjusts the url where the .update can be found, using the product descriptor and the alternate download path 
        /// </summary>
        /// <remarks>
        /// By default the url probably points to some web url. When the update is copied to the alternate download path
        /// we want the download to occur from that path, so the url in the manifest must be manipulated to point the download
        /// to the update in the alternate path.
        /// </remarks>
        /// <param name="options"></param>
        /// <param name="downloadDescriptor"></param>
        public virtual void AdjustUrlOfUpdateInManifest(AutoUpdateOptions options, AutoUpdateDownloadDescriptor downloadDescriptor)
        {
            /*
             * we're supposed to be adjusting the the url where the update can be downloaded
             * to point to the alternate path
             * where ideally the manifest file will reside alongside the update
             * 
             * the problem is that the manifest file contains a url to where the update was originally downloaded
             * most likely some web server somewhere, which after the manifest is copied to some network file share
             * we won't want to use, more likely in an effort to keep the downloaders from going over the web
             * we'll try and tweak the url to just be the unc path of the download as it resides next to the 
             * manifest in the alternate download path
             * 
             * */

            try
            {

                // if there's no alternate path, don't worry about adjusting anything
                // as nothing is going to be copied anyways
                if (options.AlternatePath == null || options.AlternatePath == string.Empty)
                    // if there
                    return;

                // redirect the url of the update to the alternate location
                downloadDescriptor.Manifest.UrlOfUpdate = string.Format("{0}\\{1}\\{1}-{2}.Update", options.AlternatePath, downloadDescriptor.Manifest.Product.Name, downloadDescriptor.Manifest.Product.Version);			
            }
            catch(ThreadAbortException)
            {

            }
        }

        /// <summary>
        /// Creates a copy of the manifest file in the alternate path
        /// </summary>
        /// <param name="downloadDescriptor"></param>
        public virtual void CreateCopyOfManifestInAlternatePath(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor)
        {
            try
            {
                // if the alternate path is not set, then we don't have to do this
                if (downloadDescriptor.Options.AlternatePath == null ||	downloadDescriptor.Options.AlternatePath == string.Empty)
                    return;

                // format a path to the product's alternate path
                var altPath = Path.Combine(downloadDescriptor.Options.AlternatePath, downloadDescriptor.Manifest.Product.Name);

                // if the path doesn't exist, just bail, we don't create alternate paths
                var folderExists = Directory.Exists(altPath);
                Debug.WriteLine(
                    $"Confirming the product's 'Alternate Download Path' folder.\n\tThe folder '{altPath}' {(folderExists ? "already exists" : "does not exist")}.", MY_TRACE_CATEGORY);
                if (!folderExists)
                {
                    Debug.WriteLine($"Creating the product's 'Alternate Download Path' folder at '{altPath}'.", MY_TRACE_CATEGORY);
                    Directory.CreateDirectory(altPath);
                }
				
                // format a path to the file in the alternate path
                var dstPath = Path.Combine(altPath,
                    $"{downloadDescriptor.Manifest.Product.Name}-{downloadDescriptor.Manifest.Product.Version}.manifest");

                var fileExists = File.Exists(dstPath);
                Debug.WriteLine(
                    $"Preparing to copy the manifest to the product's 'Alternate Download Path' folder.\n\tThe file '{dstPath}' {(fileExists ? "already exists" : "does not exist")}.", MY_TRACE_CATEGORY);
							
                // otherwise write the manifest to the alternate path
                ProgressViewer.SetExtendedDescription(progressViewer, "Creating a backup copy of the manifest file.");
                Debug.WriteLine($"Copying the manifest to '{dstPath}'.", MY_TRACE_CATEGORY);
                XmlAutoUpdateManifestWriter.Write(downloadDescriptor.Manifest, dstPath, Encoding.UTF8);				
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Creates a copy of the update file in the alternate path
        /// </summary>
        /// <param name="progressViewer"></param>
        /// <param name="downloadDescriptor"></param>
        /// <returns></returns>
        public virtual void CreateCopyOfUpdateInAlternatePath(IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor)
        {				 													
            try
            {
                // if the alternate path is not set, then we don't have to do this
                if (downloadDescriptor.Options.AlternatePath == null ||	downloadDescriptor.Options.AlternatePath == string.Empty)
                    return;

                // take the alternate path
                var altPath = Path.Combine(downloadDescriptor.Options.AlternatePath, downloadDescriptor.Manifest.Product.Name);

                // see if the folder exists
                var folderExists = Directory.Exists(altPath);				
                Debug.WriteLine(
                    $"Confirming the product's 'Alternate Download Path' folder.\n\tThe folder '{altPath}' {(folderExists ? "already exists" : "does not exist")}.", MY_TRACE_CATEGORY);
                if (!folderExists)
                {
                    Debug.WriteLine($"Creating the product's 'Alternate Download Path' folder at '{altPath}'.", MY_TRACE_CATEGORY);
                    Directory.CreateDirectory(altPath);
                }
	
                // format the backup filename from the alternate path, and the url where the update
                var dstPath = Path.Combine(altPath, Path.GetFileName(downloadDescriptor.Manifest.UrlOfUpdate));

                // see if the file already exists
                var fileExists = File.Exists(dstPath);
                Debug.WriteLine(
                    $"Preparing to copy the update to the product's 'Alternate Download Path' folder.\n\tThe file '{dstPath}' {(fileExists ? "already exists" : "does not exist")}.", MY_TRACE_CATEGORY);
				
                // copy the .update we downloaded to the backup location in the alternate path directory
                ProgressViewer.SetExtendedDescription(progressViewer, "Creating a backup copy of the update file.");
                Debug.WriteLine($"Copying the update to '{dstPath}'.", MY_TRACE_CATEGORY);				
                File.Copy(downloadDescriptor.DownloadedPath, dstPath, false);								
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }		

        /// <summary>
        /// Occurs when the engine's background thread runs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ea"></param>
        protected virtual void OnThreadRun(object sender, BackgroundThreadStartEventArgs threadStartEventArgs)
        {
            var installed = false;
            var finalized = false;
            AutoUpdateDownloadDescriptor recommendedUpdateDescriptor = null;

            try
            {
                /*
                 * Raise the AutoUpdateProcessStarted event
                 * */
                var startedArgs = new AutoUpdateManagerEventArgs(this, null);
                OnAutoUpdateProcessStarted(this, startedArgs);

                #region Step 1. QueryForLatestVersion

                /*
                 * Raise the BeforeQueryForLatestVersion event
                 * */
                var beforeQueryArgs = new AutoUpdateManagerCancelEventArgs(this, startedArgs.ProgressViewer, false);
                OnBeforeQueryForLatestVersion(this, beforeQueryArgs);

                // create an array list to hold all of the available updates
                var listOfAvailableDownloads = new ArrayList();				
				
                // use the downloaders to check for downloads
                foreach(AutoUpdateDownloader downloader in _downloaders)
                {
                    try
                    {
                        // query the latest update available for the specified product
                        AutoUpdateDownloadDescriptor updateAvailable;						
						
                        // if the downloader finds an update is available
                        if (downloader.QueryLatestVersion(beforeQueryArgs.ProgressViewer, _options, _productToUpdate, out updateAvailable))														
                            // add it to the list of downloads available
                            listOfAvailableDownloads.Add(updateAvailable);						
                    }
                    catch(ThreadAbortException ex)
                    {
                        throw new Exception(ex.Message, ex);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                }

                // create a simple array of the updates that are available for download
                var availableDownloads = listOfAvailableDownloads.ToArray(typeof(AutoUpdateDownloadDescriptor)) as AutoUpdateDownloadDescriptor[];

                // sort and select the download that contains the newest version
                recommendedUpdateDescriptor = SelectTheDownloadWithTheNewestUpdate(availableDownloads);

                /*
                 * Raise the AfterQueryForLatestVersion event
                 * */
                var afterQueryArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeQueryArgs.ProgressViewer, recommendedUpdateDescriptor);
                OnAfterQueryForLatestVersion(this, afterQueryArgs);

                // if the manager could not find a suitable recomendation for downloading, we're done 
                if (recommendedUpdateDescriptor == null)
                {
                    /*
                     * Raise the NoLaterVersionAvailable event
                     * */
                    OnNoLaterVersionAvailable(this, new AutoUpdateManagerEventArgs(this, afterQueryArgs.ProgressViewer));
                    return;
                }
				
                // or if the product to update is newer or equal to the version of the recommended update picked for downloading
                if (_productToUpdate.Version >= recommendedUpdateDescriptor.Manifest.Product.Version)
                {
                    /*
                     * Raise the NoLaterVersionAvailable event
                     * */
                    OnNoLaterVersionAvailable(this, new AutoUpdateManagerEventArgs(this, afterQueryArgs.ProgressViewer));
                    return;
                }
                
                #endregion

                #region Step 2. Download

                /*
                 * Create the path including the filename where the .Update file will be downloaded to locally
                 * (ex: "C:\Program Files\Razor\1.0.0.0.update")				  
                 * */
                recommendedUpdateDescriptor.DownloadedPath = Path.Combine(_options.DownloadPath, Path.GetFileName(recommendedUpdateDescriptor.Manifest.UrlOfUpdate));

                /*
                 * Raise the BeforeDownload event
                 * */
                var beforeDownloadArgs = new AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(this, afterQueryArgs.ProgressViewer, recommendedUpdateDescriptor, false);
                OnBeforeDownload(this, beforeDownloadArgs);
				
                // bail if the download was cancelled
                if (beforeDownloadArgs.Cancel)
                    return;				
				
                // use the downloader that found the update to download it
                // the update may be available via some proprietary communications channel (http, ftp, or Unc paths)
                var downloaded = recommendedUpdateDescriptor.Downloader.Download(beforeDownloadArgs.ProgressViewer, recommendedUpdateDescriptor);

                /*
                 * Raise the AfterDownload event
                 * */
                var afterDownloadArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeDownloadArgs.ProgressViewer, recommendedUpdateDescriptor);
                afterDownloadArgs.OperationStatus = downloaded;
                OnAfterDownload(this, afterDownloadArgs);
				
                // if the download failed bail out
                if (!downloaded)
                    return;

                #endregion

                #region Step 3. Install

                /*
                 * Raise the BeforeInstall event
                 * */
                var beforeInstallArgs = new AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(this, afterDownloadArgs.ProgressViewer, recommendedUpdateDescriptor, false);
                OnBeforeInstall(this, beforeInstallArgs);

                // if the installation was not cancelled
                if (!beforeInstallArgs.Cancel)
                {				
                    // install the update
                    installed = InstallUpdate(beforeInstallArgs.ProgressViewer, recommendedUpdateDescriptor);

                    // if the update was installed, now is the time to finalize the installation
                    if (installed)
                    {
                        // all the downloader to finalize the install, there may be things to do after the installation is complete
                        // depending upon the source of the download, and again since it's plugable only the downloader will know how to deal with it
                        // and by default it will just delete the downloaded .update file
                        finalized = recommendedUpdateDescriptor.Downloader.FinalizeInstallation(beforeInstallArgs.ProgressViewer, recommendedUpdateDescriptor);
                    }    															
                }

                /*
                 * Raise the AfterInstall event
                 * */				
                var afterInstallArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeInstallArgs.ProgressViewer, recommendedUpdateDescriptor);				
                afterInstallArgs.OperationStatus = installed && finalized;																	
                OnAfterInstall(this, afterInstallArgs);
				
                #endregion

                #region Step 4. Update Alternate Path

                /*
                 * Raise the X event
                 * */
                var beforeUpdateAltPathArgs = new AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(this, afterInstallArgs.ProgressViewer, recommendedUpdateDescriptor, false);
                OnBeforeUpdateAlternatePath(this, beforeUpdateAltPathArgs);

                if (!beforeUpdateAltPathArgs.Cancel)
                {
                    // copy the manifest & the update there 
                    AdjustUrlOfUpdateInManifest(_options, recommendedUpdateDescriptor);
                    CreateCopyOfManifestInAlternatePath(beforeUpdateAltPathArgs.ProgressViewer, recommendedUpdateDescriptor);
                    CreateCopyOfUpdateInAlternatePath(beforeUpdateAltPathArgs.ProgressViewer, recommendedUpdateDescriptor);
                }
				
                // delete the downloaded .update file, don't leave it laying around
                File.Delete(recommendedUpdateDescriptor.DownloadedPath);

                var afterUpdateAltPathArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeUpdateAltPathArgs.ProgressViewer, recommendedUpdateDescriptor);
                OnAfterUpdateAlternatePath(this, afterUpdateAltPathArgs);

                #endregion

                #region Step 5. Switch to Latest Version

                if (installed)
                {
                    /*
                     * Raise the BeforeSwitchToLatestVersion event
                     * */
                    var beforeSwitchedArgs = new AutoUpdateManagerWithDownloadDescriptorCancelEventArgs(this, afterUpdateAltPathArgs.ProgressViewer, recommendedUpdateDescriptor, false);
                    OnBeforeSwitchToLatestVersion(this, beforeSwitchedArgs);

                    // if switching to the latest version was not cancelled
                    if (!beforeSwitchedArgs.Cancel)
                    {
                        /*
                        * Raise the SwitchToLatestVersion event
                        * */
                        var switchToArgs = new AutoUpdateManagerWithDownloadDescriptorEventArgs(this, beforeSwitchedArgs.ProgressViewer, recommendedUpdateDescriptor);
                        OnSwitchToLatestVersion(this, switchToArgs);

                        // the rest should be history because the AutoUpdateSnapIn should catch that event and switch to the latest version using the bootstrap
                    }
                }

                #endregion
            }
            catch(ThreadAbortException)
            {
                Debug.WriteLine("The AutoUpdateManager has encountered a ThreadAbortException.\n\tThe auto-update thread has been aborted.", MY_TRACE_CATEGORY);

                try
                {
                    // delete the downloaded .update file, don't leave it laying around
                    File.Delete(recommendedUpdateDescriptor.DownloadedPath);
                }
                catch(Exception ex)
                {
					Debug.WriteLine(ex);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
                OnException(this, new AutoUpdateExceptionEventArgs(ex));				
            }
        }

        /// <summary>
        /// Occurs when the thread is finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnThreadFinished(object sender, BackgroundThreadEventArgs e)
        {
            // signal that the process has ended
            OnAutoUpdateProcessEnded(this, new AutoUpdateManagerEventArgs(this, null));
        }		
		
        #endregion

        #region My Event Raising Virtual Methods
		
        /// <summary>
        /// Raises the AutoUpdateProcessStarted event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAutoUpdateProcessStarted(object sender, AutoUpdateManagerEventArgs e)
        {
            try
            {
                Debug.WriteLine($"Starting auto-update process at '{DateTime.Now.ToString()}'.", MY_TRACE_CATEGORY);

                AutoUpdateProcessStarted?.Invoke(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeQueryForLatestVersion event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeQueryForLatestVersion(object sender, AutoUpdateManagerCancelEventArgs e)
        {
            try
            {
                BeforeQueryForLatestVersion?.Invoke(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AfterQueryForLatestVersion event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterQueryForLatestVersion(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                AfterQueryForLatestVersion?.Invoke(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the NoLaterVersionAvailable event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnNoLaterVersionAvailable(object sender, AutoUpdateManagerEventArgs e)
        {
            try
            {
                if (NoLaterVersionAvailable != null)
                    NoLaterVersionAvailable(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeDownload event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeDownload(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
        {
            try
            {
                BeforeDownload?.Invoke(sender, e);

                // cancel it if it's not supposed to download automatically
                if (!e.OverrideOptions && !e.Cancel)
                    if (!_options.AutomaticallyDownloadUpdates)
                        e.Cancel = true;
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AfterDownload event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterDownload(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                AfterDownload?.Invoke(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeInstall event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeInstall(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
        {
            try
            {
                BeforeInstall?.Invoke(sender, e);

                // cancel if it's not supposed to install automatically
                if (!e.OverrideOptions && !e.Cancel)
                    if (!_options.AutomaticallyInstallUpdates)
                        e.Cancel = true;
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AfterInstall event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterInstall(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                AfterInstall?.Invoke(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeUpdateAlternatePath event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeUpdateAlternatePath(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
        {
            try
            {
                BeforeUpdateAlternatePath?.Invoke(sender, e);

                // cancel if it's not supposed to update the alternate path automatically
                if (!e.OverrideOptions && !e.Cancel)
                    if (!_options.AutomaticallyUpdateAlternatePath)
                        e.Cancel = true;
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AfterInstall event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAfterUpdateAlternatePath(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                AfterUpdateAlternatePath?.Invoke(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the BeforeSwitchToLatestVersion event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnBeforeSwitchToLatestVersion(object sender, AutoUpdateManagerWithDownloadDescriptorCancelEventArgs e)
        {
            try
            {
                BeforeSwitchToLatestVersion?.Invoke(sender, e);

                // cancel if it's not supposed to switch to the latest version automatically
                if (!e.OverrideOptions && !e.Cancel)
                    if (!_options.AutomaticallySwitchToNewVersion)
                        e.Cancel = true;
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the SwitchToLatestVersion event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnSwitchToLatestVersion(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e)
        {
            try
            {
                if (SwitchToLatestVersion != null)
                    SwitchToLatestVersion(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the Exception event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnException(object sender, AutoUpdateExceptionEventArgs e)
        {
            try
            {
                if (Exception != null)
                    Exception(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
				Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Raises the AutoUpdateProcessEnded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnAutoUpdateProcessEnded(object sender, AutoUpdateManagerEventArgs e)
        {
            try
            {
                Debug.WriteLine($"Ending auto-update process at '{DateTime.Now.ToString()}'.", MY_TRACE_CATEGORY);

                if (AutoUpdateProcessEnded != null)
                    AutoUpdateProcessEnded(sender, e);
            }
            catch(ThreadAbortException)
            {

            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        #endregion				
    }	
}
