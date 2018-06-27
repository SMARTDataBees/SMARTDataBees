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
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Carbon.Common;
using Carbon.MultiThreading;
using Carbon.UI.Providers;

namespace Carbon.UI
{
	/// <summary>
	/// Summary description for AsyncWindowManager.
	/// </summary>
//	[System.Diagnostics.DebuggerStepThrough()]
	public class AsyncWindowManager : DisposableObject
	{
		private readonly WindowProvider _provider;
		private readonly Type _type;
		private readonly BackgroundThread _thread;
		private Form _window;
		private ManualResetEvent _createdEvent;
		private bool _failed;
		
		private const uint WM_CLOSE = 0x0010;

		[DllImport(@"User32.dll")]
		private static extern int PostMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
		
		[DllImport(@"User32.dll")]
		private static extern int SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        private delegate int WindowMessageCallback(uint uMsg, IntPtr wParam, IntPtr lParam);

		/// <summary>
		/// Occurs when the inner window is created
		/// </summary>
		public event AsyncWindowManagerEventHandler WindowCreated;

		/// <summary>
		/// Occurs when an Exception is encountered internally
		/// </summary>
		public event AsyncWindowManagerExceptionEventHandler Exception;

		/// <summary>
		/// Occurs when the inner window is closed
		/// </summary>
		public event AsyncWindowManagerEventHandler WindowClosed;

		/// <summary>
		/// Initializes a new instance of the AsyncWindowManager class
		/// </summary>
		/// <param name="provider">The WindowProvider to use to create the window instance</param>
		public AsyncWindowManager(WindowProvider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");

			_provider = provider;
			_type = null;
			_thread = new BackgroundThread();			
			_thread.Run += HandleThreadRun;
		}

		/// <summary>
		/// Initializes a new instance of the AsyncWindowManager class
		/// </summary>
		/// <param name="windowType">The Type to use to create the window instance</param>
		public AsyncWindowManager(Type windowType)
		{
			if (windowType == null)
				throw new ArgumentNullException("windowType");

			_provider = null;
			_type = windowType;
			_thread = new BackgroundThread();			
			_thread.Run += HandleThreadRun;
		}

		/// <summary>
		/// Close the Window when the instance is destroyed
		/// </summary>
		protected override void DisposeOfManagedResources()
		{		
			if (_window != null)
			{
				CloseWindow(false);
			}
		}

		/// <summary>
		/// Shows the Window asynchronously on a background thread
		/// </summary>
		/// <returns></returns>
		public void ShowAsynchronously()
		{			
			ShowAsynchronously(null);
		}

		/// <summary>
		/// Shows the Window asynchronously on a background thread with the specified owner
		/// </summary>
		/// <param name="owner">The owner of the form, when the dialog is shown modally</param>
		/// <returns></returns>
		public void ShowAsynchronously(IWin32Window owner)
		{
			_failed = false;
			_createdEvent = new ManualResetEvent(false);
			_thread.Start(true, new object[] {owner});
			_createdEvent.WaitOne();
		}
		
		/// <summary>
		/// Posts the specified message to the InnerWindow's message queue
		/// </summary>
		/// <param name="uMsg">The message to post to the window</param>
		/// <param name="wParam">Reserved for specific messages</param>
		/// <param name="lParam">Reserved for specific messages</param>
		/// <returns></returns>
		public int PostMessage(uint uMsg, IntPtr wParam, IntPtr lParam)
		{
            if (_window.InvokeRequired)
                return (int)_window.Invoke(new WindowMessageCallback(PostMessage), uMsg, wParam, lParam);
            
			if (_window.IsHandleCreated)
				return PostMessage(_window.Handle, uMsg, wParam, lParam);
			return 0;
		}

		/// <summary>
		/// Sends the specified message to the InnerWindow's message queue
		/// </summary>
		/// <param name="uMsg">The message to post to the window</param>
		/// <param name="wParam">Reserved for specific messages</param>
		/// <param name="lParam">Reserved for specific messages</param>
		/// <returns></returns>
		public int SendMessage(uint uMsg, IntPtr wParam, IntPtr lParam)
        {
            if (_window.InvokeRequired)
                return (int)_window.Invoke(new WindowMessageCallback(SendMessage), uMsg, wParam, lParam);
            
			if (_window.IsHandleCreated)
				return SendMessage(_window.Handle, uMsg, wParam, lParam);
			return 0;
		}

		/// <summary>
		/// Closes the InnerWindow
		/// </summary>
		/// <param name="asynchronously">True to post the message, false to send it.</param>
		public void CloseWindow(bool asynchronously)
		{
			if (asynchronously)
				PostMessage(WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
			else
				SendMessage(WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
		}

		/// <summary>
		/// Returns the Form that is being shown on the background thread
		/// </summary>
		public Form InnerWindow
		{
			get
			{
				return _window;
			}
		}

		/// <summary>
		/// Returns a flag that indicates if the Window failed to be created and shown
		/// </summary>
		public bool Failed
		{
			get
			{
				return _failed;
			}
		}

		/// <summary>
		/// The background thread's entry method
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void HandleThreadRun(object sender, BackgroundThreadStartEventArgs e)
		{
			try
			{
				// first try the Type var, if we can create one using Reflection we don't need the provider
				if (_type != null)
				{
					// make sure that the type specified inherits from Form or is directly a Form class
					TypeUtilities.AssertTypeIsSubclassOfBaseType(_type, typeof(Form));

					// create a new instance of the window using the Type specified
					_window = TypeUtilities.CreateInstanceOfType(_type, Type.EmptyTypes, new object[] {}) as Form;
				}
				else
				{
					// use the provider to create an instance of the window
					_window = _provider.CreateWindow(e.Args);
				}

				// make sure we have a window instance
				if (_window == null)
					throw new ArgumentNullException("InnerWindow", "No window instance was created.");

				// raise an event so that we can modify the window as needed before it is shown
				OnWindowCreated(this, new AsyncWindowManagerEventArgs(this));

				// release the start method by signalling we have created the window
				_createdEvent.Set();

				// see if an owner was specified
				IWin32Window owner = null;
				if (e.Args != null)
					if (e.Args.Length > 0)
						owner = (IWin32Window)e.Args[0];

				// show the window modally on this thread
				if (owner != null)
				{
					_window.Owner = owner as Form;
					_window.ShowDialog(owner);
				}
				else
				{
					_window.ShowDialog();
				}
			}
			catch(Exception ex)
			{
				_failed = true;
				Log.WriteLine(ex);
				OnException(this, new AsyncWindowManagerExceptionEventArgs(this, ex));
			}
			finally
			{
				_window = null;
				_createdEvent.Set();
			}
		}

		/// <summary>
		/// Raises the WindowCreated event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWindowCreated(object sender, AsyncWindowManagerEventArgs e)
		{
			try
			{
				if (WindowCreated != null)
					WindowCreated(sender, e);
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the Exception event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnException(object sender, AsyncWindowManagerExceptionEventArgs e)
		{
			try
			{
				if (Exception != null)
					Exception(sender, e);
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the WindowClosed event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWindowClosed(object sender, AsyncWindowManagerEventArgs e)
		{
			try
			{
				if (WindowClosed != null)
					WindowClosed(sender, e);				
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}
	}
}
