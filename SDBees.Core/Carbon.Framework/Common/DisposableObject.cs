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

namespace Carbon.Common
{
	/// <summary>
	/// Defines a class that is disposable.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public abstract class DisposableObject : IDisposable
	{
		private bool _disposed;
		private object _syncRoot = new object();
		
		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
            GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Gets a flag that indicates whether the object has been disposed
		/// </summary>
		[Browsable(false)]
		public bool Disposed
		{
			get
			{
				return _disposed;
			}
		}

		/// <summary>
		/// Returns an object that can be used to synchronize access to the collection
		/// </summary>
		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return _syncRoot;
			}
		}

		/// <summary>
		/// Override to dispose of managed resources
		/// </summary>
		protected virtual void DisposeOfManagedResources()
		{

		}
		
		/// <summary>
		/// Override to dispose of unmanaged resources
		/// </summary>
		protected virtual void DisposeOfUnManagedResources()
		{

		}
		
		/// <summary>
		/// Internal disposal function to manage this object's disposed state
		/// </summary>
		/// <param name="disposing"></param>
		private void Dispose(bool disposing)
		{			
			lock (SyncRoot)
			{
				if (!_disposed)
				{
					if (disposing)
					{
						// dispose of managed resources here
						DisposeOfManagedResources();
					}

					// dispose of unmanaged resources
					DisposeOfUnManagedResources();
					
					_disposed = true;
				}				
			}
		}		
	}
}
