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
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading;
using System.Windows.Forms;

namespace Carbon.Common
{
	//[System.Diagnostics.DebuggerStepThrough()]
	public sealed class InstanceManager : MarshalByRefObject, IDisposable
	{
		private Mutex _instance;
		private TcpChannel _channel;
	    private string _url;
		private readonly int _port;
		private ObjRef _marshalledObject;
		private bool _disposed;

		/// <summary>
		/// Fired when command line arguments are received from another application instance.
		/// </summary>
		public event EventHandler<InstanceManagerEventArgs> ReceivedCommandLineArgs;

		/// <summary>
		/// Initializes a new instance of the InstanceManager class.
		/// </summary>
		public InstanceManager(int port, string mutexName)
		{
			try
			{
				_port = port;
			    var executablePath = ExecutablePath;
				_url = $"tcp://127.0.0.1:{port}/{executablePath}";
				_instance = new Mutex(false, mutexName);

				// if this is the only instance then register a new channel on the specified port
				// and marshall this object instance on the channel
				if (IsOnlyInstance)
				{
					// create a tcp channel
					_channel = new TcpChannel(port);

					// register it
					ChannelServices.RegisterChannel(_channel, false);

					// marshal the instance manager
					_marshalledObject = RemotingServices.Marshal(this, executablePath);

					Log.WriteLine(_marshalledObject.URI);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					ReleaseInstance();

					_marshalledObject = null;
					_channel = null;
					_instance = null;
				    _url = null;
				}
				_disposed = true;
			}
		}
		#endregion

		private string ExecutablePath
		{
			get
			{
				var executablePath = Application.ExecutablePath.Replace("\\", "/");
				executablePath = executablePath.ToLower();
				return executablePath;
			}
		}

		/// <summary>
		/// Returns the object marshalled out on the remoting channel.
		/// </summary>
		public ObjRef MarshalledObject
		{
			get
			{
				return _marshalledObject;
			}
		}

		/// <summary>
		/// Returns the port that the remoting services is using to marshall objects.
		/// </summary>
		public int Port
		{
			get
			{
				return _port;
			}
		}

		/// <summary>
		/// Returns the remoting url where this object can be found.
		/// </summary>
		public string Url
		{
			get
			{
				return _url;
			}
		}

		/// <summary>
		/// Returns a flag indicating if this is the only instance of the InstanceManager running on the system.
		/// </summary>
		public bool IsOnlyInstance
		{
			get
			{
				return _instance.WaitOne(0, false);
			}
		}

		/// <summary>
		/// Releases the mutex and unregisters the tcp channel if it was registered.
		/// </summary>
		private void ReleaseInstance()
		{			
			if (_channel != null)
			{
				_instance.ReleaseMutex();

				ChannelServices.UnregisterChannel(_channel);
			}			
		}

		/// <summary>
		/// Sends the command line arguments to the previously running instance.
		/// </summary>
		/// <param name="args">The command line arguments to send.</param>
		/// <returns></returns>
		public bool SendCommandLineToPreviousInstance(string[] args)
		{
			var channel = new TcpChannel();

			try
			{
				ChannelServices.RegisterChannel(channel, false);

				var instance = Activator.GetObject(typeof(InstanceManager), _url);
				var instanceManager = instance as InstanceManager;
				if (instanceManager != null)
				{
					instanceManager.Run(args);					
				}
				return true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				MessageBox.Show(ex.ToString());
			}
			finally
			{
				ChannelServices.UnregisterChannel(channel);
			}
			return false;
		}

		/// <summary>
		/// Called by another instance of this class via remoting to raise an event on the original 
		/// application instance to signal that command line arguments have been received from another 
		/// subsequent application instance.
		/// </summary>
		/// <param name="args"></param>
		public void Run(string[] args)
		{
			EventManager.Raise(ReceivedCommandLineArgs, this, new InstanceManagerEventArgs(args));
		}
	}
}
