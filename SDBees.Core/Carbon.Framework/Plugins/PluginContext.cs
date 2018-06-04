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
using Carbon.UI.Providers;
using Carbon.UI;

namespace Carbon.Plugins
{
	/// <summary>
	/// Defines an application context in which Plugins are hosted.
	/// </summary>
	public sealed class PluginContext : DisposableObject
	{
		private bool _running;
		private static PluginContext _context;
		private PluginApplicationContext _appContext;		
		private ConfigurationProviderCollection _configurationProviders;
		//private EncryptionProviderCollection _encryptionProviders;
		private PluginProviderCollection _pluginProviders;
		private WindowProviderCollection _windowProviders;
		private PluginDescriptorCollection _pluginDescriptors;		
		//private Assembly _startingAssembly;
		private string[] _commandLineArgs;
		private InstanceManager _instanceManager;
		private IProgressViewer _progressViewer;
		private System.Threading.Timer _gcTimer;
		private bool _isInMessageLoop;
		private Form _splashWindow;

		#region PluginContextException

		/// <summary>
		/// Defines the base PluginContext exception that is thrown by the PluginContext class.
		/// </summary>
		public abstract class PluginContextException : ApplicationException
		{
			private readonly PluginContext _context;

			/// <summary>
			/// Initializes a new instance of the PluginContextException class
			/// </summary>
			/// <param name="context">The PluginContext around which the exception is based</param>
			/// <param name="message"></param>
			protected PluginContextException(PluginContext context, string message) : base(message)
			{
				_context = context;
			}

			/// <summary>
			/// Returns the PluginContext around which the exception is based
			/// </summary>
			public PluginContext ExistingContext
			{
				get
				{
					return _context;
				}
			}
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
				base(context, string.Format("A PluginContext already exists for the AppDomain '{0}'. Only one context can exist per application.", AppDomain.CurrentDomain.FriendlyName))
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
				base(context, string.Format("A PluginContext is already running for the AppDomain '{0}'. Only one context can be run per application.", AppDomain.CurrentDomain.FriendlyName))
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
			this.AssertThisIsTheOnlyCreatedContext();

			// kick off the gc timer, call back in 5 minutes
			_gcTimer = new System.Threading.Timer(this.OnGarbageCollectorTimerCallback, this, 50000, 50000);

			// watch for unhandled exceptions
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(OnCurrentAppDomainUnhandledException);

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
		public static PluginContext Current
		{
			get
			{
				return _context;
			}
		}

		#endregion

		#region My Overrides
		
		/// <summary>
		/// Cleanup any managed resources
		/// </summary>
		protected override void DisposeOfManagedResources()
		{			
			base.DisposeOfManagedResources ();

			if (_configurationProviders != null)
			{
				_configurationProviders.Dispose();
				_configurationProviders = null;
			}

			//if (_encryptionProviders != null)
			//{
			//    _encryptionProviders.Dispose();
			//    _encryptionProviders = null;
			//}

			if (_pluginProviders != null)
			{
				_pluginProviders.Dispose();
				_pluginProviders = null;
			}

			if (_windowProviders != null)
			{
				_windowProviders.Dispose();
				_windowProviders = null;
			}

			if (_pluginDescriptors != null)
			{
				_pluginDescriptors.Dispose();
				_pluginDescriptors = null;
			}
			
			if (_instanceManager != null)
			{
				_instanceManager.Dispose();
				_instanceManager = null;
			}

			if (_gcTimer != null)
			{
				_gcTimer.Dispose();
				_gcTimer = null;
			}

			_commandLineArgs = null;
			_progressViewer = null;
		}

		/// <summary>
		/// Cleanup any unmanaged resources
		/// </summary>
		protected override void DisposeOfUnManagedResources()
		{
			base.DisposeOfUnManagedResources ();
		}

		#endregion

		#region My Public Properties

		/// <summary>
		/// Returns the Assembly that represents the starting executable
		/// </summary>
		public Assembly StartingAssembly
		{
			get
			{
				return Assembly.GetEntryAssembly();
			}
		}

		/// <summary>
		/// Returns the command line arguments that were passed to the starting executable at run time.
		/// </summary>
		public string[] CommandLineArgs
		{
			get
			{
				return _commandLineArgs;
			}
		}

		/// <summary>
		/// Returns the application instance manager that should be used to receive notification
		/// of subsequent application instances and to retrieve their command line arguments.
		/// </summary>
		public InstanceManager InstanceManager
		{
			get
			{
				return _instanceManager;
			}
		}

		/// <summary>
		/// Returns a collection of ConfigurationProviders loaded in the PluginContext
		/// </summary>
		public ConfigurationProviderCollection ConfigurationProviders
		{
			get
			{
				return _configurationProviders;
			}
		}

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
		public PluginProviderCollection PluginProviders
		{
			get
			{
				return _pluginProviders;
			}
		}

		/// <summary>
		/// Returns a collection of WindowProviders loaded in the PluginContext
		/// </summary>
		public WindowProviderCollection WindowProviders
		{
			get
			{
				return _windowProviders;
			}
		}

		/// <summary>
		/// Returns a collection of PluginDescriptors that is loaded in this PluginContext
		/// </summary>
		public PluginDescriptorCollection PluginDescriptors
		{
			get
			{
				return _pluginDescriptors;
			}
		}

		/// <summary>
		/// Returns the current PluginContext's ApplicationContext
		/// </summary>
		public PluginApplicationContext ApplicationContext
		{
			get
			{
				return _appContext;
			}
		}
		
        ///// <summary>
        ///// Returns the IProgressViewer implementation that should be used during startup (aka. the splash _splashWindow)
        ///// </summary>
		public IProgressViewer SplashProgressViewer
		{
			get
			{
				return _progressViewer;
			}
		}

		/// <summary>
		/// Returns the splash _splashWindow used by the plugin context.
		/// </summary>
		public Form SplashWindow
		{
			get
			{
				return _splashWindow;
			}
		}

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
					DirectoryInfo di = new DirectoryInfo(Application.StartupPath);
					string szVersion = di.Name;

					bool useAssembly = false;
					foreach (string folderName in folderNames)
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
						AppSettingsReader reader = new AppSettingsReader();
						try
						{
							
							szVersion = System.Configuration.ConfigurationManager.AppSettings["AppVersion"];
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
							szVersion = PluginContext.Current.StartingAssembly.GetName().Version.ToString();
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
		public bool IsInMessageLoop
		{
			get
			{
				return _isInMessageLoop;
			}
		}

		#endregion

		#region My Public Methods

		/// <summary>
		/// Begins a new application plugin context. Should only be called once.
		/// </summary>
		/// <param name="startingAssembly">The startingAssembly that started the CarbonRuntime</param>
		/// <param name="args">The command line arguments that were passed to the startingAssembly that started the CarbonRuntime</param>
		[STAThread()]
		public void Run(Assembly startingAssembly, string[] args, bool silent = false)
		{
			this.AssertThisIsTheOnlyRunningContext();

			// create a new application context
			_appContext = new PluginApplicationContext();	

			// save the command line args
			_commandLineArgs = args;

			if (CarbonConfig.SingleInstance)
			{
				// create a new instance manager, don't dispose of it just yet as we'll need to have our ui plugin 
				// grab it and listen for events until the plugin context is destroyed...
				_instanceManager = new InstanceManager(CarbonConfig.SingleInstancePort, CarbonConfig.SingleInstanceMutexName);

				// check to see if this one is the only instance running
				if (!_instanceManager.IsOnlyInstance)
				{						
					// if not, forward our command line, and then instruct the 
					_instanceManager.SendCommandLineToPreviousInstance(PluginContext.Current.CommandLineArgs);
					return;
				}
			}
							
			// load the Carbon core sub-system providers 
			_windowProviders = CarbonConfig.GetWindowProviders();
			_configurationProviders = CarbonConfig.GetConfigurationProviders();
			//_encryptionProviders = CarbonConfig.GetEncryptionProviders();
			_pluginProviders = CarbonConfig.GetPluginProviders();
			
			// show the splash _splashWindow if the config specifies			
			if (!silent && CarbonConfig.ShowSplashWindow)
			{
				using (WindowProvider splashWindowProvider = this.GetSplashWindowProvider())
				{
					_splashWindow = splashWindowProvider.CreateWindow(null);
				}				

#if DEBUG
#else
				_splashWindow.Show();
#endif
				_splashWindow.Refresh();				
				_progressViewer = _splashWindow as IProgressViewer;
			}

			ProgressViewer.SetExtendedDescription(_progressViewer, "Initializing Carbon Framework System Providers.");

			// start configuration providers
			ConfigurationProvidersManager.InstructConfigurationProvidersToLoad(_progressViewer, _configurationProviders);

			// use the plugin manager to load the plugin types that the plugin providers want loaded
			using (TypeCollection pluginTypes = PluginManager.LoadPluginTypes(_progressViewer, _pluginProviders))
			{
				// use the plugin manager to create descriptors for all of the plugins
				using (_pluginDescriptors = PluginManager.CreatePluginDescriptors(_progressViewer, pluginTypes))
				{
					// validate the plugin dependencies
					PluginManager.ValidatePluginDependencies(_progressViewer, _pluginDescriptors);

					// sort plugins to have the least dependent plugins first
					// NOTE: Always sort first because the dependencies are taken into account during instance construction!
					_pluginDescriptors = PluginManager.Sort(_pluginDescriptors, true);

					// create the plugins
					PluginManager.CreatePluginInstances(_progressViewer, _pluginDescriptors);

					// start plugins
					PluginManager.StartPlugins(_progressViewer, _pluginDescriptors);
			
					// if we are supposed to run a message loop, do it now
					if (CarbonConfig.RunApplicationContext)
					{
						this.OnEnteringMainMessageLoop(new PluginContextEventArgs(this));
						
						// run the plugin context's main message loop
						Application.Run(this.ApplicationContext);
						
						this.OnExitingMainMessageLoop(new PluginContextEventArgs(this));
					}

					// sort plugins to have the most dependent plugins first
					_pluginDescriptors = PluginManager.Sort(_pluginDescriptors, false);

					// stop plugins
					PluginManager.StopPlugins(null, _pluginDescriptors);
				}
			}

			// stop configuration providers
			// start configuration providers
			ConfigurationProvidersManager.InstructConfigurationProvidersToSave(_configurationProviders);			
		}

		/// <summary>
		/// Restarts the application using the boostrapper
		/// </summary>
		public void RestartUsingBootstrap()
		{
			try
			{				
				// look at the startup directory
				DirectoryInfo directory = new DirectoryInfo(System.Windows.Forms.Application.StartupPath);

				// jump to it's parent, that is going to be the download path for all updates (*.update)
				string bootstrapPath = Path.GetDirectoryName(directory.FullName);

				// start the bootstrap with the instructions to wait for us to quit
				string bootStrapFilename = Path.Combine(bootstrapPath, this.StartingAssembly.GetName().Name + ".exe");

				// format the bootstrap's command line and tell it to wait on this process
				string commandLine = string.Format("/pid:{0} /wait", Process.GetCurrentProcess().Id.ToString());

				// start the bootstrapper
				Process.Start(bootStrapFilename, commandLine);

				EventManager.Raise<PluginContextEventArgs>(this.RestartPending, this, new PluginContextEventArgs(this));
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
			if (_context != null)
			{
				throw new PluginContextAlreadyExistsException(_context);
			}
			
			// the first thing is to set the context so that it can be retrieved
			// anywhere in the application from here on using the PluginContext.Current property
			_context = this;
		}

		/// <summary>
		/// Asserts that there are no other running PluginContexts in the current application
		/// </summary>
		private void AssertThisIsTheOnlyRunningContext()
		{
			// there can be only one context running per application
			if (_running)
			{
				throw new PluginContextAlreadyRunningException(_context);
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
				string.Compare(CarbonConfig.SplashWindowProviderName, "None", true) == 0)
				return null;

			// otherwise lookup the _splashWindow provider
			WindowProvider provider = this.WindowProviders[CarbonConfig.SplashWindowProviderName];
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
			EventManager.Raise<PluginDescriptorEventArgs>(this.PluginStarted, this, e);
		}

		/// <summary>
		/// Raises the PluginStopped event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnPluginStopped(PluginDescriptorEventArgs e)
		{
			EventManager.Raise<PluginDescriptorEventArgs>(this.PluginStopped, this, e);
		}

		/// <summary>
		/// Raises the AfterPluginsStarted event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnAfterPluginsStarted(PluginContextEventArgs e)
		{
			ProgressViewer.SetExtendedDescription(_progressViewer, "Plugins started. Application opening...");	

			EventManager.Raise<PluginContextEventArgs>(this.AfterPluginsStarted, this, e);
		}

		/// <summary>
		/// Raises the BeforePluginsStopped event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnBeforePluginsStopped(PluginContextEventArgs e)
		{
			EventManager.Raise<PluginContextEventArgs>(this.BeforePluginsStopped, this, e);			
		}

		/// <summary>
		/// Raises the RestartPending event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnRestartPending(PluginContextEventArgs e)
		{
			EventManager.Raise<PluginContextEventArgs>(this.RestartPending, this, e);			
		}

		/// <summary>
		/// Raises the EnteringMainMessageLoop event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnEnteringMainMessageLoop(PluginContextEventArgs e)
		{
			Log.WriteLine("Entering PluginContext Message Loop.");
			EventManager.Raise<PluginContextEventArgs>(this.EnteringMessageLoop, this, e);

			_isInMessageLoop = true;							
			_progressViewer = null;			
		}

		/// <summary>
		/// Raises the ExitingMainMessageLoop event.
		/// </summary>
		/// <param name="e"></param>
		internal void OnExitingMainMessageLoop(PluginContextEventArgs e)
		{
			_isInMessageLoop = false;

			Log.WriteLine("Exiting PluginContext Message Loop.");
			EventManager.Raise<PluginContextEventArgs>(this.ExitingMessageLoop, this, e);	
		}
	}
}