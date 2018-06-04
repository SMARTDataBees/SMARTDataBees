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
using Carbon.UI;

namespace Carbon.Plugins.Providers
{
	/// <summary>
	/// Defines a class responsible for loading Plugins.
	/// </summary>
	public abstract class PluginProvider : Provider
	{		
        /// <summary>
        /// Initializes a new instance of the PluginProvider class.
        /// </summary>
        /// <param name="name">The name of the provider.</param>
		protected PluginProvider(string name) : 
            base(name)
		{
			
		}

		/// <summary>
		/// Returns a collection of Types that are defined as Carbon Plugins.
		/// </summary>
		/// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param></param>
		/// <returns></returns>
		public abstract TypeCollection LoadPluginTypes(IProgressViewer progressViewer);
	}
}
