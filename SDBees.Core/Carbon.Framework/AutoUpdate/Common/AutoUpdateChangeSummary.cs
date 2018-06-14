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

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Provides a class that exposes a summary of changes that were logged during an AutoUpdate cycle.
	/// </summary>
	public sealed class AutoUpdateChangeSummary
	{
		private string _id;
		private string _title;
		private string _preview;
		private string _postedBy;
		private DateTime _datePosted;
		private AutoUpdateChangeTypes _type;
		//private static int MaxPreviewLength = 150;
		
		/// <summary>
		/// Initializes a new instance of the AutoUpdateChangeSummary class
		/// </summary>
		public AutoUpdateChangeSummary()
		{
			_id = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateChangeSummary class
		/// </summary>
		/// <param name="title">The title of the change</param>
		/// <param name="preview">A preview of the change</param>
		/// <param name="postedBy">Who posted the change</param>
		/// <param name="datePosted">The date it was posted</param>
		/// <param name="type">The type of change</param>
		public AutoUpdateChangeSummary(string id, string title, string preview, string postedBy, DateTime datePosted, AutoUpdateChangeTypes type)
		{
			_id = id;
			_title = title;
			Preview = preview;
			_postedBy = postedBy;
			_datePosted = datePosted;
			_type = type;
		}
		
		/// <summary>
		/// Gets or sets the id for this change
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}

		/// <summary>
		/// Gets or sets the title of the change
		/// </summary>
		public string Title
		{
			get
			{
				return _title;
			}
			set
			{
				_title = value;
			}
		}
		
		/// <summary>
		/// Gets or sets a preview of the description, usually about 150 words or less...
		/// </summary>
		public string Preview
		{
			get
			{
				return _preview;
			}
			set
			{
				//if (value != null)
				//{
				//    if (value.Length > MaxPreviewLength)
				//    {
				//        value = value.Substring(0, MaxPreviewLength);
				//        value += @"...";
				//    }
				//}

				_preview = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the person that posted the change
		/// </summary>
		public string PostedBy
		{
			get
			{
				return _postedBy;
			}
			set
			{
				_postedBy = value;
			}
		}

		/// <summary>
		/// Gets or sets the date the change was posted
		/// </summary>
		public DateTime DatePosted
		{
			get
			{
				return _datePosted;
			}
			set
			{
				_datePosted = value;
			}
		}

		/// <summary>
		/// Gets or sets the type of change made
		/// </summary>
		public AutoUpdateChangeTypes Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}
	}
}
