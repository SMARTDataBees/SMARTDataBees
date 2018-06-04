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

namespace Carbon.Common
{
	/// <summary>
	/// Provides a strongly-typed collection of Forms.
	/// This class is thread safe.
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	public sealed class FormCollection : DisposableCollection
	{
		#region FormAlreadyExistsException 

		/// <summary>
		/// Defines an exception that is generated when a Form is added to the collection
		/// and the Form already exists in the collection.
		/// </summary>
		public sealed class FormAlreadyExistsException : ApplicationException
		{
			private readonly Form _form;

			/// <summary>
			/// Initializes a new instance of the FormAlreadyExistsException class
			/// </summary>
			/// <param name="form">The Form that already exists in the collection</param>
			internal FormAlreadyExistsException(Form form) : 
				base(string.Format("The Form '{0}' already exists in the collection.", form.Text))
			{
				_form = form;
			}

			/// <summary>
			/// Returns the Form that caused the exception
			/// </summary>
			public Form Form
			{
				get
				{
					return _form;
				}
			}
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the FormCollection class
		/// </summary>
		public FormCollection()
		{
			
		}
		
		/// <summary>
		/// Adds a Form to the collection
		/// </summary>
		/// <param name="form"></param>
		public void Add(Form form)
		{
			if (this.Contains(form))
				throw new FormAlreadyExistsException(form);
			
			lock (base.SyncRoot)
			{
				base.InnerList.Add(form);
			}
		}

		/// <summary>
		/// Adds an array of Forms to the collection
		/// </summary>
		/// <param name="forms"></param>
		public void AddRange(Form[] forms)
		{
			foreach (Form form in forms)
				this.Add(form);
		}

		/// <summary>
		/// Removes a Form from the collection
		/// </summary>
		/// <param name="form"></param>
		public void Remove(Form form)
		{
			if (this.Contains(form))
				lock (base.SyncRoot)
				{
					base.InnerList.Remove(form);
				}
		}
		
		/// <summary>
		/// Determines if the specified Form exists in the collection
		/// </summary>
		/// <param name="form"></param>
		/// <returns></returns>
		public bool Contains(Form form)
		{
			if (form == null)
				throw new ArgumentNullException("form");

			lock (base.SyncRoot)
			{
				foreach(Form f in base.InnerList)
					if (f == form)
						return true;
			}
			return false;
		}

		/// <summary>
		/// Returns the Form at the specified index of the collection
		/// </summary>
		public Form this[int index]
		{
			get
			{
				lock (base.SyncRoot)
				{
					return base.InnerList[index] as Form;
				}
			}
		}
	}


}
