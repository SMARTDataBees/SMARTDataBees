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

namespace Carbon.Common
{
	/// <summary>
	/// Provides methods for logging and diagnostic traces.
	/// </summary>	
	[DebuggerStepThrough]
	public sealed class Log
	{
		/// <summary>
		/// Returns the name of the category used during all Debug/Debug writes
		/// </summary>
		public static readonly string Category = "'Carbon'";

        /// <summary>
        /// Initializes a new instance of the Log class.
        /// </summary>
		private Log() 
        {
        
        }

		/// <summary>
		/// Writes the value to the trace listeners in the System.Diagnostics.Debug.Listeners collection.
		/// </summary>
		/// <param name="value">The value to write</param>
		public static void WriteLine(string value)
		{
			Debug.WriteLine(value, Category);
		}

		/// <summary>
		/// Writes the value of the object's System.Object.ToString method to the trace listeners in the System.Diagnostics.Debug.Listeners collection.
		/// </summary>
		/// <param name="value">The value to write</param>
		public static void WriteLine(object value)
		{			
			Debug.WriteLine(value, Category);
		}

        /// <summary>
        /// Writes a category name and the value of the object's Object.ToString() method to the trace listners in the System.Diagnostics.Debug.Listeners collection.
        /// </summary>
        /// <param name="condition">The condition if true will cause the value to be written.</param>
        /// <param name="value">The value to write</param>
        public static void WriteLineIf(bool condition, object value)
        {
            Debug.WriteLineIf(condition, value, Category);
        }

		/// <summary>
		/// Writes the formatted value to the trace listeners in the System.Diagnostics.Debug.Listeners collection.
		/// </summary>
		/// <param name="format">A string containing zero or more format items</param>
		/// <param name="args">An object array containing zero or more objects to format</param>
		public static void WriteLine(string format, params object[] args)
		{
			Debug.WriteLine(string.Format(format, args), Category);            
		}

        /// <summary>
        /// Increases the Indent level by one.
        /// </summary>
        public static void Indent()
        {
            Debug.Indent();
        }

        /// <summary>
        /// Decreases the Indent level by one.
        /// </summary>
        public static void Unindent()
        {
            Debug.Unindent();
        }
	}
}
