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

namespace Carbon.Common
{
	/// <summary>
	/// Summary description for DisposableCollection.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public abstract class DisposableCollection : CollectionBase, IDisposable
	{
		private bool _disposed;
		
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
		public object SyncRoot
		{
			get
			{
				return InnerList.SyncRoot;
			}
		}

		/// <summary>
		/// Override to dispose of managed resources
		/// </summary>
		protected virtual void DisposeOfManagedResources()
		{
			lock (SyncRoot)
			{
				foreach (var obj in InnerList)
				{
					var disposable = obj as IDisposable;
					if (disposable != null)
						disposable.Dispose();
				}

				InnerList.Clear();
			}
		}
		
		/// <summary>
		/// Override to dispose of unmanaged resources
		/// </summary>
		protected virtual void DisposeOfUnManagedResources()
		{

		}

		/// <summary>
		/// Sorts the items in the collection using the specified comparer
		/// </summary>
		/// <param name="comparer">The IComparer implementation to use when sorting. -or- null to use the IComparable implementation of the items in the collection</param>
		public virtual void Sort(IComparer comparer)
		{
			lock (SyncRoot)
			{
				InnerList.Sort(comparer);
			}
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
						// dispose of managed resources here
						DisposeOfManagedResources();

					// dispose of unmanaged resources
					DisposeOfUnManagedResources();
				}
				_disposed = true;
			}
		}
	}
}
