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
	/// Provides a generic method that can raise any event that uses an EventHandler based delegate.
	/// </summary>
	public static class EventManager
	{
		/// <summary>
		/// Calls the specified delegate, using traditional event raising methodologies.
		/// </summary>
		/// <typeparam name="TEventArgs">The EventArgs Type that will be provided to the generic method.</typeparam>
		/// <param name="handler">The delegate to call.</param>
		/// <param name="sender">The sender to the delegate..</param>
		/// <param name="e">The EventArgs to the delegate.</param>
		public static void Raise<TEventArgs>(EventHandler<TEventArgs> handler, object sender, TEventArgs e) where TEventArgs : EventArgs 
		{
			try
			{				
				// thread affinity is achieved by the passing of the delegate to this function
				if (handler != null)
				{
					handler(sender, e);
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
