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

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Provides a strongly-typed collection of AutoUpdateChangeSummary instances.
	/// This class is thread-safe.
	/// </summary>
	public class AutoUpdateChangeSummaryList : DisposableCollection 		
	{
	    /// <summary>
		/// Adds a change summary to the collection.
		/// </summary>
		/// <param name="summary">The collection to add.</param>
		public void Add(AutoUpdateChangeSummary summary)
		{
			if (summary == null)
				throw new ArgumentNullException("summary");

			if (Contains(summary))
				throw new AutoUpdateChangeSummaryAlreadyExistsException(summary);

			lock (SyncRoot)
			{
				InnerList.Add(summary);
			}
		}

		/// <summary>
		/// Adds an array of change summaries to the collection.
		/// </summary>
		/// <param name="summaries">The change summaries to add to the collection.</param>
		public void AddRange(AutoUpdateChangeSummary[] summaries)
		{
			if (summaries == null)
				throw new ArgumentNullException("summaries");

			foreach(var summary in summaries)
				Add(summary);
		}	

		/// <summary>
		/// Removes the specified change summary from the collection.
		/// </summary>
		/// <param name="summary">The change summary to remove.</param>
		public void Remove(AutoUpdateChangeSummary summary)
		{
			if (summary == null)
				throw new ArgumentNullException("summary");

			if (Contains(summary))
			{
				lock (SyncRoot)
				{
					InnerList.Remove(summary);
				}
			}
		}

		/// <summary>
		/// Determines if the specified change summary exists in the collection.
		/// </summary>
		/// <param name="summary">The change summary to check for.</param>
		/// <returns></returns>
		public bool Contains(AutoUpdateChangeSummary summary)
		{
			if (summary == null)
				throw new ArgumentNullException("summary");

			lock (SyncRoot)
			{
				foreach (AutoUpdateChangeSummary existingSummary in InnerList)
					if (string.Compare(existingSummary.Id, summary.Id, true) == 0)
						return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the change summary from the collection at the specified index in the collection.
		/// </summary>
		/// <param name="index">The index of the change summary to return from the collection.</param>
		/// <returns></returns>
		public AutoUpdateChangeSummary this[int index]
		{
			get
			{
				lock (SyncRoot)
				{
					return InnerList[index] as AutoUpdateChangeSummary;
				}
			}
		}

		/// <summary>
		/// Returns the change summary from the collection with the specified id.
		/// </summary>
		/// <param name="id">The id of the change summary to return from the collection.</param>
		/// <returns></returns>
		public AutoUpdateChangeSummary this[string id]
		{
			get
			{
				lock (SyncRoot)
				{
					foreach (AutoUpdateChangeSummary existingSummary in InnerList)
						if (string.Compare(existingSummary.Id, id, true) == 0)
							return existingSummary;
				}

				return null;
			}
		}
	}
}
