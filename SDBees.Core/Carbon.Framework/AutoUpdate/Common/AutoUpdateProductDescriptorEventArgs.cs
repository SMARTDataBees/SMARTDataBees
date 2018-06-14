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
	/// Defines an EventArgs class that contains an AutoUpdateProductDescriptor as the context of the application.
	/// </summary>
	public class AutoUpdateProductDescriptorEventArgs : EventArgs 
	{
		private AutoUpdateProductDescriptor _productDescriptor;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateProductDescriptorEventArgs class.
		/// </summary>
		/// <param name="productDescriptor">The descriptor that is the context of the event.</param>
		public AutoUpdateProductDescriptorEventArgs(AutoUpdateProductDescriptor productDescriptor)
		{
			_productDescriptor = productDescriptor;
		}

		/// <summary>
		/// Gets or sets the product descriptor that is the context of the event.
		/// </summary>
		public AutoUpdateProductDescriptor ProductDescriptor
		{
			get
			{
				return _productDescriptor;
			}
			set
			{
				_productDescriptor = value;
			}
		}
	}

	public delegate void AutoUpdateProductDescriptorEventHandler(object sender, AutoUpdateProductDescriptorEventArgs e);
}
