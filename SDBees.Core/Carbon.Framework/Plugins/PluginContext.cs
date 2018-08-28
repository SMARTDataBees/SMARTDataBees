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
using System.Reflection;
using System.Windows.Forms;
using Carbon.Common;
using Carbon.Configuration.Providers;
using Carbon.Plugins.Providers;
using Carbon.UI;
using Carbon.UI.Providers;
using Timer = System.Threading.Timer;

namespace Carbon.Plugins
{
	/// <summary>
	/// Defines an application context in which Plugins are hosted.
	/// </summary>
	public sealed class PluginContext : DisposableObject
	{
		private bool _running;

	    //private Assembly _startingAssembly;
	    private Timer _gcTimer;

	    #region PluginContextException

		/// <summary>
		/// Defines the base PluginContext exception that is thrown by the PluginContext class.
		/// </summary>
		public abstract class PluginContextException : ApplicationException
		{
		    /// <summary>
			/// Initializes a new instance of the PluginContextException class
			/// </summary>
			/// <param name="context">The PluginContext around which the exception is based</param>
			/// <param name="message"></param>
			protected PluginContextException(PluginContext context, string message) : base(message)
			{
				ExistingContext = context;
			}

			/// <summary>
			/// Returns the PluginContext around which the exception is based
			/// </summary>
			public PluginContext ExistingContext { get; }
		}

		#endregion

		#region PluginContextAlreadyExistsException

		/// <summary>
		/// Defines an exception that is generated as a result of attempting
		/// to create more than one PluginContext per application.
		/// </summary>
		public sealed class PluginContextAlreadyExistsException : PluginContextException
		{			
			/// <summary>
			/// Initializes a new instance of the PluginContextAlreadyExistsException class
			/// </summary>
			/// <param name="context">The PluginContext that already exists</param>
			internal PluginContextAlreadyExistsException(PluginContext context) :
				base(context,
			        $"A PluginContext already exists for the AppDomain '{AppDomain.CurrentDomain.FriendlyName}'. Only one context can exist per application.")
			{

			}
		}

		#endregion

		#region PluginContextAlreadyRunningException

		/// <summary>
		/// Defines an exception that is throw by the PluginContext class if the Run method is called more than one time.
		/// </summary>
		public sealed class PluginContextAlreadyRunningException : PluginContextException 
		{
			/// <summary>
			/// Initializes a new instance of the PluginContextAlreadyRunningException class
			/// </summary>
			/// <param name="context">The PluginContext that is already running</param>
			internal PluginContextAlreadyRunningException(PluginContext context) : 
				base(context,
			        $"A PluginContext is already running for the AppDomain '{AppDomain.CurrentDomain.FriendlyName}'. Only one context can be run per application.")
			{

			}
		}

		#endregion

		#region My Public Events

		/// <summary>
		/// Fires when a Plugin is started
		/// </summary>
		public event EventHandler<PluginDescriptorEventArgs> PluginStarted;

		/// <summary>
		/// Fires when a Plugin is stopped
		/// </summary>
		public event EventHandler<PluginDescriptorEventArgs> PluginStopped;										

		/// <summary>
		/// Fires after all of the Plugins have been started
		/// </summary>
		public event EventHandler<PluginContextEventArgs> AfterPluginsStarted;
        
		/// <summary>
		/// Fires before all of the Plugins are stopped
		/// </summary>
		public event EventHandler<PluginContextEventArgs> BeforePluginsStopped;

		/// <summary>
		/// Fires when a restart is pending the current hosting engine's death. The bootstrap is waiting.
		/// </summary>
		public event EventHandler<PluginContextEventArgs> RestartPending;

		/// <summary>
		/// Fires when the PluginContext enters its main message loop.
		/// </summary>
		public event EventHandler<PluginContextEventArgs> EnteringMessageLoop;

		/// <summary>
		/// Fires when the PluginContext exits its main message loop.
		/// </summary>
		public event EventHandler<PluginContextEventArgs> ExitingMessageLoop;

		#endregion

		/// <summary>
		/// Initializes a new instance of the PluginContext class
		/// </summary>
		public PluginContext() 
		{
			AssertThisIsTheOnlyCreatedContext();

			// kick off the gc timer, call back in 5 minutes
			_gcTimer = new Timer(OnGarbageCollectorTimerCallback, this, 50000, 50000);

			// watch for unhandled exceptions
			AppDomain.CurrentDomain.UnhandledException += OnCurrentAppDomainUnhandledException;

			// log the carbon sections that must exist in the App.config file
			CarbonConfig.Initialize();						
		}

		/// <summary>
		/// Occurs when an unhandled exception is generated by the current app domain.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnCurrentAppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Log.WriteLine(e.ExceptionObject);
		}

		#region My Static Methods
		
		/// <summary>
		/// Returns the current PluginContext that is hosting the current application's Plugins
		/// </summary>
		public static PluginContext Current { get; private set; }

	    #endregion

		#region My Overrides
		
		/// <summary>
		/// Cleanup any managed resources
		/// </summary>
		protected override void DisposeOfManagedResources()
		{			
			base.DisposeOfManagedResources ();

			if (ConfigurationProviders != null)
			{
				ConfigurationProviders.Dispose();
				ConfigurationProviders = null;
			}

			if (PluginProviders != null)
			{
				PluginProviders.Dispose();
				PluginProviders = null;
			}

			if (WindowProviders != null)
			{
				WindowProviders.Dispose();
				WindowProviders = null;
			}

			if (PluginDescriptors != null)
			{
				PluginDescriptors.Dispose();
				PluginDescriptors = null;
			}
			
			if (InstanceManager != null)
			{
				InstanceManager.Dispose();
				InstanceManager = null;
			}

			if (_gcTimer != null)
			{
				_gcTimer.Dispose();
				_gcTimer = null;
			}

			CommandLineArgs = null;
			SplashProgressViewer = null;
		}

		#endregion

		#region My Public Properties

		/// <summary>
		/// Returns the Assembly that represents the starting executable
		/// </summary>
		public Assembly StartingAssembly { get; } = Assembly.GetEntryAssembly();

	    /// <summary>
		/// Returns the command line arguments that were passed to the starting executable at run time.
		/// </summary>
		public string[] CommandLineArgs { get; private set; }

	    /// <summary>
		/// Returns the application instance manager that should be used to receive notification
		/// of subsequent application instances and to retrieve their command line arguments.
		/// </summary>
		public InstanceManager InstanceManager { get; private set; }

	    /// <summary>
		/// Returns a collection of ConfigurationProviders loaded in the PluginContext
		/// </summary>
		public ConfigurationProviderCollection ConfigurationProviders { get; private set; }

	    ///// <summary>
		///// Returns a collection of EncryptionProviders loaded in the PluginContext
		///// </summary>
		//public EncryptionProviderCollection EncryptionProviders
		//{
		//    get
		//    {
		//        return _encryptionProviders;
		//    }
		//}

		/// <summary>
		/// Returns a collection of PluginProviders loaded in the PluginContext
		/// </summary>
		public PluginProviderCollection PluginProviders { get; private set; }

	    /// <summary>
		/// Returns a collection of WindowProviders loaded in the PluginContext
		/// </summary>
		public WindowProviderCollection WindowProviders { get; private set; }

	    /// <summary>
		/// Returns a collection of PluginDescriptors that is loaded in this PluginContext
		/// </summary>
		public PluginDescriptorCollection PluginDescriptors { get; private set; }

	    /// <summary>
		/// Returns the current PluginContext's ApplicationContext
		/// </summary>
		public PluginApplicationContext ApplicationContext { get; private set; }

	    ///// <summary>
        ///// Returns the IProgressViewer implementation that should be used during startup (aka. the splash _splashWindow)
        ///// </summary>
		public IProgressViewer SplashProgressViewer { get; private set; }

	    /// <summary>
		/// Returns the splash _splashWindow used by the plugin context.
		/// </summary>
		public Form SplashWindow { get; private set; }

	    /// <summary>
		/// Returns the current application version. First looks at the current folder, determines if it is the Debug, Release, or Current, and falls back upon the App.Config files, and then falls back upon the starting assembly version attribute.
		/// </summary>
		/// <returns></returns>
		public Version AppVersion
		{
			get
			{
				string[] folderNames = { "Debug", "Release", "Current" };
				try
				{
					var di = new DirectoryInfo(Application.StartupPath);
					var szVersion = di.Name;

					var useAssembly = false;
					foreach (var folderName in folderNames)
					{
						if (string.Compare(di.Name, folderName, true) == 0)
						{
							useAssembly = true;
							break;
						}
					}

					Version v = null;
					if (!useAssembly)
					{
						try
						{
							v = new Version(szVersion);
						}
						catch (Exception ex)
						{
							Log.WriteLine(ex);
							useAssembly = true;
						}
					}

					if (useAssembly)
					{
						try
						{
							
							szVersion = ConfigurationManager.AppSettings["AppVersion"];
							if (szVersion != null && szVersion.Length >= 0)
								v = new Version(szVersion);
						}
						catch (Exception ex) 
						{
							Log.WriteLine(ex);
						}

						// and yet, still as if being tortured by the gods, we have no version
						if (v == null)
						{
							// we must fall back upon the lonely executable that started it all, and hope to read a version from it
							szVersion = Current.StartingAssembly.GetName().Version.ToString();
						}
					}

					v = new Version(szVersion);
					return v;
				}
				catch (Exception ex)
				{
					Log.WriteLine(ex);
				}
				return null;
			}
		}

		/// <summary>
		/// Returns a flag indicating whether the PluginContext is inside it's main message loop or not.
		/// </summary>
		public bool IsInMessageLoop { get; private set; }

	    #endregion

		#region My Public Methods

		/// <summary>
		/// Begins a new application plugin context. Should only be called once.
		/// </summary>
		/// <param name="startingAssembly">The startingAssembly that started the CarbonRuntime</param>
		/// <param name="args">The command line arguments that were passed to the startingAssembly that started the CarbonRuntime</param>
		[STAThread]
		public void Run(Assembly startingAssembly, string[] args, bool silent = false)
		{
			AssertThisIsTheOnlyRunningContext();

			// create a new application context
			ApplicationContext = new PluginApplicationContext();	

			// save the command line args
			CommandLineArgs = args;

			if (CarbonConfig.SingleInstance)
			{
				// create a new instance manager, don't dispose of it just yet as we'll need to have our ui plugin 
				// grab it and listen for events until the plugin context is destroyed...
				InstanceManager = new InstanceManager(CarbonConfig.SingleInstancePort, CarbonConfig.SingleInstanceMutexName);

				// check to see if this one is the only instance running
				if (!InstanceManager.IsOnlyInstance)
				{						
					// if not, forward our command line, and then instruct the 
					InstanceManager.SendCommandLineToPreviousInstance(Current.CommandLineArgs);
					return;
				}
			}
							
			// load the Carbon core sub-system providers 
			WindowProviders = CarbonConfig.GetWindowProviders();
			ConfigurationProviders = CarbonConfig.GetConfigurationProviders();
			//_encryptionProviders = CarbonConfig.GetEncryptionProviders();
			PluginProviders = CarbonConfig.GetPluginProviders();
			
			// show the splash _splashWindow if the config specifies			
			if (!silent && CarbonConfig.ShowSplashWindow)
			{
				using (var splashWindowProvider = GetSplashWindowProvider())
				{
					SplashWindow = splashWindowProvider.CreateWindow(null);
				}				

#if DEBUG
#else
				_splashWindow.Show();
#endif
				SplashWindow.Refresh();				
				SplashProgressViewer = SplashWindow as IProgressViewer;
			}

			ProgressViewer.SetExtendedDescription(SplashProgressViewer, "Initializing Carbon Framework System Providers.");

			// start configuration providers
			ConfigurationProvidersManager.InstructConfigurationProvidersToLoad(SplashProgressViewer, ConfigurationProviders);

			// use the plugin manager to load the plugin types that the plugin providers want loaded
			using (var pluginTypes = PluginManager.LoadPluginTypes(SplashProgressViewer, PluginProviders))
			{
				// use the plugin manager to create descriptors for all of the plugins
				using (PluginDescriptors = PluginManager.CreatePluginDescriptors(SplashProgressViewer, pluginTypes))
				{
					// validate the plugin dependencies
					PluginManager.ValidatePluginDependencies(SplashProgressViewer, PluginDescriptors);

					// sort plugins to have the least dependent plugins first
					// NOTE: Always sort first because the dependencies are taken into account during instance construction!
					PluginDescriptors = PluginManager.Sort(PluginDescriptors, true);

					// create the plugins
					PluginManager.CreatePluginInstances(SplashProgressViewer, PluginDescriptors);

					// start plugins
					PluginManager.StartPlugins(SplashProgressViewer, PluginDescriptors);
			
					// if we are supposed to run a message loop, do it now
					if (CarbonConfig.RunApplicationContext)
					{
						OnEnteringMainMessageLoop(new PluginContextEventArgs(this));
						
						// run the plugin context's main message loop
						Application.Run(ApplicationContext);
						
						OnExitingMainMessageLoop(new PluginContextEventArgs(this));
					}

					// sort plugins to have the most dependent plugins first
					PluginDescriptors = PluginManager.Sort(PluginDescriptors, false);

					// stop plugins
					PluginManager.StopPlugins(null, PluginDescriptors);
				}
			}

			// stop configuration providers
			// start configuration providers
			ConfigurationProvidersManager.InstructConfigurationProvidersToSave(ConfigurationProviders);			
		}

		/// <summary>
		/// Restarts the application using the boostrapper
		/// </summary>
		public void RestartUsingBootstrap()
		{
			try
			{				
				// look at the startup directory
				var directory = new DirectoryInfo(Application.StartupPath);

				// jump to it's parent, that is going to be the download path for all updates (*.update)
				var bootstrapPath = Path.GetDirectoryName(directory.FullName);

				// start the bootstrap with the instructions to wait for us to quit
				var bootStrapFilename = Path.Combine(bootstrapPath, StartingAssembly.GetName().Name + ".exe");

				// format the bootstrap's command line and tell it to wait on this process
				var commandLine = $"/pid:{Process.GetCurrentProcess().Id} /wait";

				// start the bootstrapper
				Process.Start(bootStrapFilename, commandLine);

				EventManager.Raise(RestartPending, this, new PluginContextEventArgs(this));
			}
			catch (Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		#endregion		

        /// <summary>
        /// Asserts that there are no other created PluginContexts in the current application
        /// </summary>
        private void AssertThisIsTheOnlyCreatedContext()
		{
			// there can be only one per application
			if (Current != null)
			{
				throw new PluginContextAlreadyExistsException(Current);
			}
			
			// the first thing is to set the context so that it can be retrieved
			// anywhere in the application from here on using the PluginContext.Current property
			Current = this;
		}

		/// <summary>
		/// Asserts that there are no other running PluginContexts in the current application
		/// </summary>
		private void AssertThisIsTheOnlyRunningContext()
		{
			// there can be only one context running per application
			if (_running)
			{
				throw new PluginContextAlreadyRunningException(Current);
			}

			// set this so that future calls to the Run method will fail
			_running = true;
		}

		/// <summary>
		/// Returns the WindowProvider responsible for creating the Splash Window
		/// </summary>
		/// <returns></returns>
		private WindowProvider GetSplashWindowProvider()
		{			
			// ignore the setting if the splash _splashWindow provider's name is empty or equals "none"
			if (CarbonConfig.SplashWindowProviderName.Length == 0 ||
				String.Compare(CarbonConfig.SplashWindowProviderName, "None", StringComparison.OrdinalIgnoreCase) == 0)
				return null;

			// otherwise lookup the _splashWindow provider
			var provider = WindowProviders[CarbonConfig.SplashWindowProviderName];
			if (provider == null)
				throw new WindowProviderNotFoundException(CarbonConfig.SplashWindowProviderName);			
			return provider;
		}

		/// <summary>
		/// Occurs when the GC timer kicks off to help force garbage collection.
		/// </summary>
		private void OnGarbageCollectorTimerCallback(object state)
		{
			GC.Collect();
		}

		/// <summary>
		/// Raises the PluginStarted event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnPluginStarted(PluginDescriptorEventArgs e)
		{
			EventManager.Raise(PluginStarted, this, e);
		}

		/// <summary>
		/// Raises the PluginStopped event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnPluginStopped(PluginDescriptorEventArgs e)
		{
			EventManager.Raise(PluginStopped, this, e);
		}

		/// <summary>
		/// Raises the AfterPluginsStarted event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnAfterPluginsStarted(PluginContextEventArgs e)
		{
			ProgressViewer.SetExtendedDescription(SplashProgressViewer, "Plugins started. Application opening...");	

			EventManager.Raise(AfterPluginsStarted, this, e);
		}

		/// <summary>
		/// Raises the BeforePluginsStopped event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnBeforePluginsStopped(PluginContextEventArgs e)
		{
			EventManager.Raise(BeforePluginsStopped, this, e);			
		}

		/// <summary>
		/// Raises the RestartPending event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnRestartPending(PluginContextEventArgs e)
		{
			EventManager.Raise(RestartPending, this, e);			
		}

		/// <summary>
		/// Raises the EnteringMainMessageLoop event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnEnteringMainMessageLoop(PluginContextEventArgs e)
		{
			Log.WriteLine("Entering PluginContext Message Loop.");
			EventManager.Raise(EnteringMessageLoop, this, e);

			IsInMessageLoop = true;							
			SplashProgressViewer = null;			
		}

		/// <summary>
		/// Raises the ExitingMainMessageLoop event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnExitingMainMessageLoop(PluginContextEventArgs e)
		{
			IsInMessageLoop = false;

			Log.WriteLine("Exiting PluginContext Message Loop.");
			EventManager.Raise(ExitingMessageLoop, this, e);	
		}
	}
}