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
	/// Defines an EventArgs class containing an ObjectBase instance as the context.
	/// </summary>
	[DebuggerStepThrough]
	public class ObjectEventArgs : EventArgs 
	{
		private object _context;
		private ObjectActions _action;

		/// <summary>
		/// Initializes a new instance of the ObjectEventArgs class.
		/// </summary>
		/// <param name="context">The context of the event.</param>
		/// <param name="action">The action being taken on the object.</param>
		public ObjectEventArgs(object context, ObjectActions action)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}

			_context = context;
			_action = action;
		}

		/// <summary>
		/// Returns the context of the event.
		/// </summary>
		public object Context
		{
			get
			{
				return _context;
			}
			set
			{
				_context = value;
			}
		}

		/// <summary>
		/// Returns the action being taken on the context.
		/// </summary>
		public ObjectActions Action
		{
			get
			{
				return _action;
			}
		}
	}
}
