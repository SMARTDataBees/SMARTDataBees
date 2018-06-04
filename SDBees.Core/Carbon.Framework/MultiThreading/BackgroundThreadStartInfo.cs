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

namespace Carbon.MultiThreading
{
	/// <summary>
	/// Defines the starting information passed to a BackgroundThread when the thread is created or started
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	public sealed class BackgroundThreadStartInfo 
	{
        //private bool _allowThreadAbortExceptions;
		private object _stateObject;
		private object[] _args;
		private BackgroundThreadStartEventHandler _onRun;
		private BackgroundThreadEventHandler _onFinished;

		/// <summary>
		/// Initializes a new instance of the BackgroundThreadStartInfo class
		/// </summary>
		public BackgroundThreadStartInfo()
		{

		}

		/// <summary>
		/// Initializes a new instance of the BackgroundThreadStartInfo class
		/// </summary>
		/// <param name="args">An array of arguments to pass to the thread when it starts</param>
		/// <param name="onRun"></param>
		public BackgroundThreadStartInfo(object stateObject, object[] args, BackgroundThreadStartEventHandler onRun, BackgroundThreadEventHandler onFinished) 
		{			
            //_allowThreadAbortExceptions = allowThreadAbortExceptions;
			_stateObject = stateObject;
			_args = args;
			_onRun = onRun;
			_onFinished = onFinished;
		}
		
		#region My Public Properties

        ///// <summary>
        ///// Gets or sets a flag that indicates whether the thread will allow ThreadAbortExceptions to be thrown when the thread finishes or is stopped
        ///// </summary>
        //public bool AllowThreadAbortExceptions
        //{
        //    get
        //    {
        //        return _allowThreadAbortExceptions;
        //    }
        //    set
        //    {
        //        _allowThreadAbortExceptions = value;
        //    }
        //}

		/// <summary>
		/// Gets or sets an object that holds state information
		/// </summary>		
		public object StateObject
		{
			get
			{
				return _stateObject;
			}
			set
			{
				_stateObject = value;
			}
		}

		/// <summary>
		/// Returns an array of arguments to pass to the thread when it starts
		/// </summary>
		public object[] Args
		{
			get
			{
				return _args;
			}
			set
			{
				_args = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the callback method that will be called when the background thread starts
		/// </summary>
		public BackgroundThreadStartEventHandler Run
		{
			get
			{
				return _onRun;
			}
			set
			{
				_onRun = value;
			}
		}

		/// <summary>
		/// Gets or sets the callback method that will be called when the background thread finishes
		/// </summary>
		public BackgroundThreadEventHandler Finished
		{
			get
			{
				return _onFinished;
			}
			set
			{
				_onFinished = value;
			}
		}

		#endregion
	}
}
