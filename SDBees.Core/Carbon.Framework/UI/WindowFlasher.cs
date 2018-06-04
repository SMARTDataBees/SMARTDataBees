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
using System.Runtime.InteropServices;

namespace Carbon.UI
{
    /*
	 * Wow. This is one of the very first .NET Win32 Interop classes that I wrote. 
	 * Absolutely horrible use of data type aliasing. When I get time, come back and recode this
	 * and bring it up to standards. Ah the things you learn with time... :P
	 * */

	using UINT	= System.UInt32;
	using HWND	= System.IntPtr;
	using DWORD = System.Int32;

	/// <summary>
	/// Provides a class for flashing the title bar of a window.
	/// </summary>
	public static class WindowFlasher
	{
		private enum BOOL
		{
			FALSE = 0,
			TRUE  = 1
		}

		private struct FLASHINFO
		{			
			public UINT  cbSize;
			public HWND  hwnd;
			public DWORD dwFlags;
			public UINT  uCount;
			public DWORD dwTimeout;
		}

		[DllImport("User32")]
		private static extern BOOL FlashWindowEx(ref FLASHINFO pfwi);

		private const DWORD FLASHW_STOP			= 0;
		private const DWORD FLASHW_CAPTION		= 0x00000001;
		private const DWORD FLASHW_TRAY			= 0x00000002;
		private const DWORD FLASHW_ALL			= (FLASHW_CAPTION | FLASHW_TRAY);
		private const DWORD FLASHW_TIMER		= 0x00000004;
		private const DWORD FLASHW_TIMERNOFG	= 0x0000000C;
				
		/// <summary>
		/// Flashes the window specified the number of times specified.
		/// </summary>
		/// <param name="hWnd">The window to flash.</param>
		/// <param name="flashCount">The number of times to flash it.</param>
		/// <returns></returns>
		public static bool FlashWindow(HWND hWnd, int flashCount)
		{
			FLASHINFO fwi;
			fwi.cbSize		= (UINT)Marshal.SizeOf(typeof(FLASHINFO));	// size
			fwi.hwnd		= hWnd;									// the window to flash
			fwi.dwFlags		= FLASHW_ALL;							// flash the caption and tray
			fwi.uCount		= (UINT)flashCount;							// how many times to flash it
			fwi.dwTimeout	= 0;									// use the default cursor blink rate

			return (FlashWindowEx(ref fwi) == BOOL.TRUE);
		}

		/// <summary>
		/// Flashes the window specified until it receives focus.
		/// </summary>
		/// <param name="hWnd">The window to flash.</param>
		/// <param name="flashUntilForeground">A flag that indicates if the window will flash 5 times, or until it receives focus.</param>
		/// <returns></returns>
		public static bool FlashWindow(HWND hWnd, bool flashUntilForeground)
		{
			FLASHINFO fwi;
			fwi.cbSize		= (UINT)Marshal.SizeOf(typeof(FLASHINFO));	// size
			fwi.hwnd		= hWnd;									// the window to flash
			fwi.dwFlags		= (flashUntilForeground ? FLASHW_ALL | FLASHW_TIMERNOFG : FLASHW_ALL);							// flash the caption and tray
			fwi.uCount		= 5;									// how many times to flash it
			fwi.dwTimeout	= 0;									// use the default cursor blink rate

			return (FlashWindowEx(ref fwi) == BOOL.TRUE);
		}
	}
}
