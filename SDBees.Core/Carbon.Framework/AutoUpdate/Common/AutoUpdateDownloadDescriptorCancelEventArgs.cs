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
	/// Defines a cancellable EventArg class that contains an AutoUpdateDownloadDescriptor as the context.
	/// </summary>
	public sealed class AutoUpdateDownloadDescriptorCancelEventArgs : AutoUpdateDownloadDescriptorEventArgs
	{
		private bool _cancel;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateDownloadDescriptorCancelEventArgs class.
		/// </summary>
		/// <param name="cancel">A flag that indicates whether the event should be cancelled.</param>
		/// <param name="updateDescriptor">The AutoUpdateDownloadDescriptor that is the context of the event.</param>
		public AutoUpdateDownloadDescriptorCancelEventArgs(bool cancel, AutoUpdateDownloadDescriptor downloadDescriptor) 
			: base(downloadDescriptor)
		{
			_cancel = cancel;
		}

		/// <summary>
		/// Gets or sets a flag that indicates whether the event should be cancelled or not.
		/// </summary>
		public bool Cancel
		{
			get
			{
				return _cancel;
			}
			set
			{
				_cancel = value;
			}
		}
	}

	public delegate void AutoUpdateDownloadDescriptorCancelEventHandler(object sender, AutoUpdateDownloadDescriptorCancelEventArgs e);
}
