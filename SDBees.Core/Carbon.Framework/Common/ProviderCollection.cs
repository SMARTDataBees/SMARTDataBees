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

namespace Carbon.Common
{
	/// <summary>
	/// Defines a strongly-typed collection of Provider instances.
	/// This class is thread-safe.
	/// </summary>
	[DebuggerStepThrough]
	public abstract class ProviderCollection : DisposableCollection
	{
		#region ProviderAlreadyExistsException

		/// <summary>
		/// Defines an exception that is generated when a Provider is added to the collection and another
		/// Provider already exists in the collection using the same name. The Providers should be uniquely
		/// named in the App.config file so that Carbon can refer to them by name.
		/// </summary>
		public sealed class ProviderAlreadyExistsException : ApplicationException
		{
			private readonly string _name;
			
			/// <summary>
			/// Initializes a new instance of the ProviderAlreadyExistsException class
			/// </summary>
			/// <param name="name">The name of the Provider that already exists in the collection</param>
			internal ProviderAlreadyExistsException(string name) : 
				base($"A Provider with the name '{name}' already exists in the collection.")
			{
				_name = name;
			}

			/// <summary>
			/// Returns the name of the Provider that already exists in the collection
			/// </summary>
			public string ProviderName
			{
				get
				{
					return _name;
				}
			}
		}

		#endregion

		protected override void DisposeOfManagedResources()
		{
			base.DisposeOfManagedResources();

			lock (SyncRoot)
			{
				foreach (Provider provider in InnerList)
					provider.Dispose();
				InnerList.Clear();
			}
		}

		/// <summary>
		/// Adds the Provider to the collection
		/// </summary>
		/// <param name="provider">The provider to add</param>
		internal void Add(Provider provider)
		{
			if (Contains(provider))
				throw new ProviderAlreadyExistsException(provider.Name);

			lock (SyncRoot)
			{
				InnerList.Add(provider);
			}
		}

		/// <summary>
		/// Removes the Provider from the collection
		/// </summary>
		/// <param name="provider"></param>
		protected void Remove(Provider provider)
		{
			if (Contains(provider))
				lock (SyncRoot)
				{
					InnerList.Remove(provider);
				}
		}

		/// <summary>
		/// Determines if the collection contains another Provider with the same name
		/// </summary>
		/// <param name="provider">The Provider to look for</param>
		/// <returns></returns>
		protected bool Contains(Provider provider)
		{
			if (provider == null)
				throw new ArgumentNullException("provider");
			return (this[provider.Name] != null);
		}

		/// <summary>
		/// Returns the Provider from collection that has the specified name
		/// </summary>
		protected Provider this[string name]
		{
			get
			{
				lock (SyncRoot)
				{
					foreach (Provider provider in InnerList)
						if (string.Compare(provider.Name, name, true) == 0)
							return provider;
					return null;
				}
			}
		}
	}
}
