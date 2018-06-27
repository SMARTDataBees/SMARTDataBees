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
using System.Windows.Forms;
using Carbon.Common;
using FormCollection = Carbon.Common.FormCollection;

namespace Carbon.Plugins
{
	/// <summary>
	/// Provides the contextual information about an application thread.
	/// </summary>
	public sealed class PluginApplicationContext : ApplicationContext
	{		
		private FormCollection _topLevelWindows;

		/// <summary>
		/// Initializes a new instance of the SnapInApplicationContext class
		/// </summary>
		public PluginApplicationContext()
		{
            _topLevelWindows = new FormCollection();			
		}				

		/// <summary>
		/// Adds a Form to the SnapInHostingEngine's ApplicationContext as a top level form
		/// </summary>
		/// <param name="form"></param>
		public void AddTopLevelWindow(Form form)
		{
			if (form == null)
				throw new ArgumentNullException("form");

			// wire up to the form's closed event
			form.Closed += OnTopLevelWindowClosed;

			// add the window to the list
			lock (_topLevelWindows)
			{				
				_topLevelWindows.Add(form);
			}
		}

		/// <summary>
		/// Removes a Form from the SnapInHostingEngine's ApplicationContext as a top level form
		/// </summary>
		/// <param name="form"></param>
		public void RemoveTopLevelWindow(Form form)
		{
			if (form == null)
				throw new ArgumentNullException("form");

			// unwire from the form's closed event
			form.Closed -= OnTopLevelWindowClosed;

			// lock the window list
			lock(_topLevelWindows)
			{
				// remove the window from the list
				_topLevelWindows.Remove(form);
		
				// if that was the last top level window
				if (_topLevelWindows.Count == 0)
					// go ahead and exit the main thread
					ExitThread();
			}
		}

		/// <summary>
		/// Occurs when a top level window is closed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnTopLevelWindowClosed(object sender, EventArgs e)
		{
			try
			{
				// snag the sender of the event
				var form = (Form)sender;

				// remove the window from out list, if it's the last one this will exit the main thread
				RemoveTopLevelWindow(form);
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}            
		}
	}
}
