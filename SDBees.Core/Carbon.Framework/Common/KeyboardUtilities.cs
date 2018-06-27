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

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Carbon.Common
{
    /// <summary>
    /// Provides methods for working with the keyboard.
	/// </summary>
	[DebuggerStepThrough]
    public static class KeyboardUtilities
    {
        [DllImport("User32.dll")]
        private extern static short GetAsyncKeyState(int key);

        /// <summary>
        /// Determines if a particular key is depressed on the keyboard
        /// </summary>
        /// <param name="key">The key whos state will be determined to be down or not</param>
        /// <returns></returns>
        public static bool IsKeyDown(Keys key)
        {
            if (GetAsyncKeyState((int)key) < 0)
                return true;
            return false;
        }
    }
}
