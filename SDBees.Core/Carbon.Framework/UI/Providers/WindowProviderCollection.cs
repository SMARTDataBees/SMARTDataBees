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

namespace Carbon.UI.Providers
{
	/// <summary>
	/// Provides a strongly-typed collection of WindowProviderCollection instances.
	/// This class is thread safe.
	/// </summary>
	public sealed class WindowProviderCollection : ProviderCollection
	{
        /// <summary>
        /// Initializes a new instance of the WindowProviderCollection class.
        /// </summary>
		internal WindowProviderCollection()
		{
			
		}

        /// <summary>
        /// Adds a provider to the collection.
        /// </summary>
        /// <param name="provider">The provider to add.</param>
		internal void Add(WindowProvider provider)
		{
			base.Add(provider);
		}

        /// <summary>
        /// Removes a provider from the collection.
        /// </summary>
        /// <param name="provider">The provider to remove.</param>
		internal void Remove(WindowProvider provider)
		{
			base.Remove(provider);
		}

        /// <summary>
        /// Determines if the provider exists in the collection.
        /// </summary>
        /// <param name="provider">The provider to look for.</param>
        /// <returns></returns>
		public bool Contains(WindowProvider provider)
		{
			return base.Contains(provider);
		}

        /// <summary>
        /// Returns the provider from the collection with the specified name.
        /// </summary>
		public new WindowProvider this[string name]
		{
			get
			{
				return (WindowProvider)base[name];
			}
		}
	}
}
