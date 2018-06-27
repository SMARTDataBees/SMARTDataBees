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
	/// Provides a strongly-typed collection of Type instances.
	/// This class is thread safe.
	/// </summary>	
	[DebuggerStepThrough]
	[Serializable]
	public sealed class TypeCollection : DisposableCollection	
	{
		private bool _allowDuplicates;

		#region TypeAlreadyExistsException

		/// <summary>
		/// Summary description for TypeAlreadyExistsException.
		/// </summary>
		public sealed class TypeAlreadyExistsException : ApplicationException
		{
			private readonly Type _type;

			/// <summary>
			/// Initializes a new instance of the TypeAlreadyExistsException class
			/// </summary>
			/// <param name="type"></param>
			internal TypeAlreadyExistsException(Type type) : 
				base($"The Type '{type.FullName}' already exists in the collection.")
			{
				_type = type;
			}

			/// <summary>
			/// Returns the Type that already exists in the collection
			/// </summary>
			public Type Type
			{
				get
				{
					return _type;
				}
			}
		}

		#endregion

		public event EventHandler<TypeCollectionEventArgs> Changed;

		/// <summary>
		/// Initializes a new instance of the TypeCollection class
		/// </summary>
		public TypeCollection()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the TypeCollection class
		/// </summary>
		/// <param name="types">An array of Type objects to add to the collection</param>
		public TypeCollection(Type[] types)
		{
			if (types == null)
				throw new ArgumentNullException("types");
			
			AddRange(types);
		}

		/// <summary>
		/// Initializes a new instance of the TypeCollection class
		/// </summary>
		/// <param name="types">A TypeCollection filled with types to add to the collection</param>
		public TypeCollection(TypeCollection types)
		{
			AddRange(types);
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the collection allows duplicate items.
		/// </summary>
		public bool AllowDuplicates
		{
			get
			{
				return _allowDuplicates;
			}
			set
			{
				_allowDuplicates = value;
			}
		}

		/// <summary>
		/// Adds a Type to the collection
		/// </summary>
		/// <param name="type">The Type to add</param>
		public void Add(Type type)
		{
			if (!AllowDuplicates)
				if (Contains(type))
					throw new TypeAlreadyExistsException(type);

			lock (SyncRoot)
			{
				InnerList.Add(type);
				EventManager.Raise(Changed, this, new TypeCollectionEventArgs(type, ObjectActions.Added));
			}			
		}

		/// <summary>
		/// Adds an array of Types to the collection
		/// </summary>
		/// <param name="types">The array of Types to add</param>
		public void AddRange(Type[] types)
		{
			foreach (var type in types)
				Add(type);
		}

		/// <summary>
		/// Adds a collection of Types to the collection
		/// </summary>
		/// <param name="types">The collection of types to add</param>
		public void AddRange(TypeCollection types)
		{
			if (types == null)
				throw new ArgumentNullException("types");

			foreach (Type type in types)
			{				
				Add(type);
			}
		}

		/// <summary>
		/// Inserts a Type into the collection at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <param name="type"></param>
		public void Insert(int index, Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");

			if (Contains(type))
				throw new TypeAlreadyExistsException(type);

			lock (SyncRoot)
			{				
				InnerList.Insert(index, type);
				EventManager.Raise(Changed, this, new TypeCollectionEventArgs(type, ObjectActions.Added));
			}				
		}

		/// <summary>
		/// Determines if the Type exists in the collection
		/// </summary>
		/// <param name="type">The Type to look for</param>
		/// <returns></returns>
		public bool Contains(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			return (this[type.FullName] != null);
		}

		/// <summary>
		/// Removes a Type from the collection
		/// </summary>
		/// <param name="type">The Type to remove</param>
		public void Remove(Type type)
		{
			if (Contains(type))
				lock (SyncRoot)
				{
					InnerList.Remove(type);
					EventManager.Raise(Changed, this, new TypeCollectionEventArgs(type, ObjectActions.Changed));
				}
		}

		/// <summary>
		/// Returns the Type from the collection at the specified index.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Type this[int index]
		{
			get
			{
				lock (SyncRoot)
				{
					return InnerList[index] as Type;
				}
			}
		}

		/// <summary>
		/// Returns the Type from the collection with the specified full name
		/// </summary>
		public Type this[string fullName]
		{
			get
			{
				lock (SyncRoot)
				{
					foreach (Type t in InnerList)
						if (string.Compare(t.FullName, fullName, true) == 0)
							return t;
					return null;
				}				
			}
		}

		/// <summary>
		/// Converts the collection into an array of Types
		/// </summary>
		/// <returns></returns>
		public Type[] ToArray()
		{
			lock (SyncRoot)
			{
				return InnerList.ToArray(typeof(Type)) as Type[];
			}
		}
	}
}
