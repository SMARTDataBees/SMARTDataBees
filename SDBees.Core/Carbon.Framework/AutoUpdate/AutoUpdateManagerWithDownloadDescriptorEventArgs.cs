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
using System.Collections.Generic;
using System.Text;

using Carbon.AutoUpdate.Common;
using Carbon.UI;

namespace Carbon.AutoUpdate
{
	/// <summary>
	/// Defines an EventArgs class that combines an AutoUpdateDownloadDescriptor with AutoUpdateManagerEventArgs.
	/// </summary>
	public class AutoUpdateManagerWithDownloadDescriptorEventArgs : AutoUpdateManagerEventArgs
	{
		private AutoUpdateDownloadDescriptor _downloadDescriptor;
		private bool _operationStatus;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateManagerWithDownloadDescriptorEventArgs class.
		/// </summary>
		/// <param name="manager">The AutoUpdateManager that is handling the update.</param>
		/// <param name="progressViewer">The IProgressViewer that can be used to display progress about the update.</param>
		/// <param name="downloadDescriptor">The AutoUpdateDownloadDescriptor that describes teh update available.</param>
		public AutoUpdateManagerWithDownloadDescriptorEventArgs(AutoUpdateManager manager, IProgressViewer progressViewer, AutoUpdateDownloadDescriptor downloadDescriptor)
			: base(manager, progressViewer)
		{
			_downloadDescriptor = downloadDescriptor;
		}

		/// <summary>
		/// Returns an AutoUpdateDownloadDescriptor that describes teh update available.
		/// </summary>
		public AutoUpdateDownloadDescriptor DownloadDescriptor
		{
			get
			{
				return _downloadDescriptor;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates the overall status of the update operation.
		/// </summary>
		public bool OperationStatus
		{
			get
			{
				return _operationStatus;
			}
			set
			{
				_operationStatus = value;
			}
		}
	}

	public delegate void AutoUpdateManagerWithDownloadDescriptorEventHandler(object sender, AutoUpdateManagerWithDownloadDescriptorEventArgs e);

}
