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

namespace Carbon.Configuration
{
	/// <summary>
	/// Provides methods for working with bit masks.
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	public static class FlagsHelper
	{
		/// <summary>
		/// Checks a bitmask to see if a particular flag is set on or off
		/// </summary>
		/// <param name="value">The bitmask to check</param>
		/// <param name="flag">The flag whose state is in question</param>
		/// <returns></returns>
		public static bool IsFlagSet(int value, int flag) 
		{
			return (bool)((value & flag) == flag);
		}

		/// <summary>
		/// Enables a mask
		/// </summary>
		/// <param name="value"></param>
		/// <param name="flag"></param>
		public static void Enable(int value, int flag)
		{
			value |= flag;
		}

		/// <summary>
		/// Disables a mask
		/// </summary>
		/// <param name="value"></param>
		/// <param name="flag"></param>
		public static void Disable(int value, int flag)
		{
			if (!FlagsHelper.IsFlagSet(value, flag))
				value ^= flag;
		}
	}
}
