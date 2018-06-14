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
	/// Defines an exception that can be thrown when a Provider cannot be found.
	/// </summary>
	[DebuggerStepThrough]
	public class ProviderNotFoundException : ApplicationException
	{
		private readonly string _name;

		/// <summary>
		/// Initializes a new instance of the ProviderNotFoundException class
		/// </summary>
		/// <param name="name">The name of the provider that could not be found</param>
		public ProviderNotFoundException(string name) : 
			base($"No provider could be found with the name '{name}'.\nPlease refer to the log files for exception details.")
		{
			_name = name;
		}

		/// <summary>
		/// Returns the name of the provider that could not be found
		/// </summary>
		public string ProviderName
		{
			get
			{
				return _name;
			}
		}
	}
}
