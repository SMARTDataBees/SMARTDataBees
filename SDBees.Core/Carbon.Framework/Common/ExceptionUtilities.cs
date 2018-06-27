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
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Carbon.Common
{
	/// <summary>
	/// Provides methods for displaying exceptions that the application encounters in a standard format.
	/// </summary>
	[DebuggerStepThrough]
	public static class ExceptionUtilities
	{
		/// <summary>
		/// Displays a message box containing information about an exception, for the currently executing application, plus optional additional information.
		/// </summary>
		/// <param name="owner">The window owning the dialog</param>
		/// <param name="caption">The caption to display on the dialog</param>
		/// <param name="icon">The icon to display on the dialog</param>
		/// <param name="buttons">The buttons to display on the dialog</param>
		/// <param name="ex">The exception to display on the dialog</param>
		/// <param name="infoLines">Optional additional information to display on the dialog</param>
		/// <returns>The result of the dialog</returns>
		public static DialogResult DisplayException(IWin32Window owner, string caption, MessageBoxIcon icon, MessageBoxButtons buttons, Exception ex, params string[] infoLines)
		{
			var hasAdditionalInfo = false;
			var sb = new StringBuilder();
			
			// begin with the application information that generated the exception
			sb.Append(
			    $"The application '{Path.GetFileName(Application.ExecutablePath)}' has encountered the following exception or condition.\n\n");
			
			// append the additional information if any was supplied
			if (infoLines != null)
			{
				hasAdditionalInfo = true;
				sb.Append("Additional Information:\n\n");
				foreach(var line in infoLines)
					sb.Append($"{line}\n");
			}
						
			if (ex != null)
			{
				// append the information contained in the exception
				sb.Append($"{(hasAdditionalInfo ? "\n" : null)}Exception Information:\n\n");
				sb.Append(ex);
			}

			// display a message and return the result
			return MessageBox.Show(owner, sb.ToString(), caption, buttons, icon);
		}
	}
}
