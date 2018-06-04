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
using System.Collections;

using Carbon.Common;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Provides a strongly-typed collection of AutoUpdateDownloader instances.
	/// This class is thread-safe.
	/// </summary>
	public class AutoUpdateDownloaderList : DisposableCollection 
	{
		/// <summary>
		/// Initializes a new instance of the AutoUpdateDownloaderList class
		/// </summary>
		public AutoUpdateDownloaderList()
		{
			
		}

		/// <summary>
		/// Adds a downloader to the collection.
		/// </summary>
		/// <param name="downloader">The downloader to add to the collection.</param>
		public void Add(AutoUpdateDownloader downloader)
		{
			if (downloader ==  null)
				throw new ArgumentNullException("downloader");

			if (this.Contains(downloader))
				throw new AutoUpdateDownloaderAlreadyExistsException(downloader);

			lock (base.SyncRoot)
			{
				base.InnerList.Add(downloader);
			}
		}

		/// <summary>
		/// Adds an array of downloaders to the collection.
		/// </summary>
		/// <param name="downloaders">The downloaders to add to the collection.</param>
		public void AddRange(AutoUpdateDownloader[] downloaders)
		{
			if (downloaders == null)
				throw new ArgumentNullException("downloaders");

			foreach(AutoUpdateDownloader downloader in downloaders)
				this.Add(downloader);
		}

		/// <summary>
		/// Removes the downloader from the collection.
		/// </summary>
		/// <param name="downloader">The downloader to remove.</param>
		public void Remove(AutoUpdateDownloader downloader)
		{
			if (downloader ==  null)
				throw new ArgumentNullException("downloader");

			if (this.Contains(downloader))
				base.InnerList.Remove(downloader);
		}

		/// <summary>
		/// Determines if the specified downloader exists in the collection.
		/// </summary>
		/// <param name="downloader">The downloader to check for.</param>
		/// <returns></returns>
		public bool Contains(AutoUpdateDownloader downloader)
		{
			if (downloader ==  null)
				throw new ArgumentNullException("downloader");

			lock (base.SyncRoot)
			{
				foreach (AutoUpdateDownloader existingDownloader in base.InnerList)
					if (string.Compare(existingDownloader.Id, downloader.Id, true) == 0)
						return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the downloader from the collection at the specified index.
		/// </summary>
		/// <param name="index">The index of the downloader to return.</param>
		/// <returns></returns>
		public AutoUpdateDownloader this[int index]
		{
			get
			{
				lock (base.SyncRoot)
				{
					return base.InnerList[index] as AutoUpdateDownloader;
				}
			}
		}

		/// <summary>
		/// Returns the downloader from the collection with the specified id.
		/// </summary>
		/// <param name="id">The id of the downloader to return.</param>
		/// <returns></returns>
		public AutoUpdateDownloader this[string id]
		{
			get
			{
				lock (base.SyncRoot)
				{
					foreach (AutoUpdateDownloader existingDownloader in base.InnerList)
						if (string.Compare(existingDownloader.Id, id, true) == 0)
							return existingDownloader;
				}

				return null;
			}
		}
	}
}
