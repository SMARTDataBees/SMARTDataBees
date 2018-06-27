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
using System.Runtime.Serialization;

namespace Carbon.Common
{
	/// <summary>
	/// Defines the base object for all 'Keyed' objects that exist in the Carbon.Common namespace.
	/// This class is thread-safe, serializable, and cloneable.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public class KeyedObject : ObjectBase, ISerializable	
	{
		private string _key;

		/// <summary>
		/// Initializes a new instance of the KeyedObject class.
		/// </summary>
		public KeyedObject()
		{

		}

		/// <summary>
		/// Initializes a new instance of the KeyedObject class.
		/// </summary>
		/// <param name="name">A name to assign to the object.</param>
		public KeyedObject(string name)
			: base(name)
		{

		}

		/// <summary>
		/// Initializes a new instance of the KeyedObject class.
		/// </summary>
		/// <param name="name">A name to assign to the object.</param>
		/// <param name="description">An optional comment to describe the object.</param>
		public KeyedObject(string name, string description)
			: base(name, description)
		{

		}

		/// <summary>
		/// Initializes a new instance of the KeyedObject class.
		/// </summary>
		/// <param name="name">A name to assign to the object.</param>
		/// <param name="description">An optional comment to describe the object.</param>
		/// <param name="key">A key to assign to the object</param>
		public KeyedObject(string name, string description, string key)
			: base(name, description)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			if (key == string.Empty)
			{
				throw new ArgumentException("The key cannot be an emtpy string.");
			}

			_key = key;
		}

		/// <summary>
		/// Initializes a new instance of the KeyedObject class.
		/// </summary>
		/// <param name="element">The object to copy.</param>
		public KeyedObject(KeyedObject element)
			: base(element)
		{
			_key = element.Key;
		}

		/// <summary>
		/// Override the base class initialization to create a new unique key.
		/// </summary>
		protected override void Initialize()
		{
			// always call the base class first
			base.Initialize();

			_key = Guid.NewGuid().ToString();
		}

		/// <summary>
		/// Returns a shallow copy of this object.
		/// </summary>
		/// <returns></returns>
		public new virtual object Clone()
		{			
			return MemberwiseClone();
		}

		/// <summary>
		/// Gets or sets a key that may be used to uniquely identify this object.
		/// </summary>
		public string Key
		{
			get
			{
				lock (SyncRoot)
				{
					return _key;
				}
			}
			set
			{
				lock (SyncRoot)
				{
					var e = new KeyedObjectCancelEventArgs(this, ObjectActions.Changed, false);
					OnBeforeChanged(this, e);
					if (e.Cancel)
					{
						Debug.WriteLine("The property 'Key' could not be changed because the 'BeforeChanged' was cancelled.");
						return;
					}

					_key = value;

					OnAfterChanged(this, new KeyedObjectEventArgs(this, ObjectActions.Changed));
				}
			}
		}

		#region ISerializable Members

		public KeyedObject(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			_key = (string)info.GetValue("Key", typeof(string));
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) 			
		{
			base.GetObjectData(info, context);

			info.AddValue("Key", _key, typeof(string));
		}

		#endregion
	}
}
