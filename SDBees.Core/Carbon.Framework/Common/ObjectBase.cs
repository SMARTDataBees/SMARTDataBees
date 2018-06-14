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
using System.Drawing;
using System.Runtime.Serialization;

namespace Carbon.Common
{
	/// <summary>
	/// Defines the base object for all 'Named' objects that exist in the Carbon.Common namespace.
	/// This class is thread-safe, serializable, and cloneable.
	/// </summary>
	[DebuggerStepThrough]
	[Serializable]
	public class ObjectBase : DisposableObject, ICloneable, ISerializable
	{
		private string _name;
		private string _description;
		private Image _smallImage;
		private Image _largeImage;
		
		public event EventHandler<ObjectCancelEventArgs> BeforeChanged;
		public event EventHandler<ObjectEventArgs> AfterChanged;

		/// <summary>
		/// Initializes a new instance of the ObjectBase class.
		/// </summary>
		public ObjectBase() :
			this(string.Empty, string.Empty)
		{

		}

		/// <summary>
		/// Initializes a new instance of the ObjectBase class.
		/// </summary>
		/// <param name="name">A name to assign to the object.</param>
		public ObjectBase(string name) :
			this(name, string.Empty)
		{

		}

		/// <summary>
		/// Initializes a new instance of the ObjectBase class.
		/// </summary>
		/// <param name="name">A name to assign to the object.</param>
		/// <param name="description">An optional comment to describe the object.</param>
		public ObjectBase(string name, string description)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}

			if (name == string.Empty)
			{
				throw new ArgumentException("The name cannot be an emtpy string.", "name");
			}

			_name = name;
			_description = description;

			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the ObjectBase class.
		/// </summary>
		/// <param name="element">The object to copy.</param>
		public ObjectBase(ObjectBase element) :
			this(element.Name, element.Description)
		{
			if (element.SmallImage != null)
			{
				_smallImage = (Image)element.SmallImage.Clone();
			}

			if (element.LargeImage != null)
			{
				_largeImage = (Image)element.LargeImage.Clone();
			}
		}

		/// <summary>
		/// Provides an overridable function to initialize properties of the object during construction.
		/// Any overridden methods MUST call to the base class first.
		/// </summary>
		protected virtual void Initialize()
		{

		}

		/// <summary>
		/// Override the disposal of managed resources and destroy the images if needed.
		/// </summary>
		protected override void DisposeOfManagedResources()
		{
			lock (SyncRoot)
			{				
				/*
				 * Images are notorious memory leaks, so be vigilant in their destruction.
				 * */

				if (_smallImage != null)
				{
					_smallImage.Dispose();
					_smallImage = null;
				}

				if (_largeImage != null)
				{
					_largeImage.Dispose();
					_largeImage = null;
				}

				// dispose the base last
				base.DisposeOfManagedResources();
			}
		}

		/// <summary>
		/// Gets or sets the name of the object.
		/// </summary>
		public string Name
		{
			get
			{
				lock (SyncRoot)
				{
					return _name;
				}
			}
			set
			{
				lock (SyncRoot)
				{
					var e = new ObjectCancelEventArgs(this, ObjectActions.Changed, false);
					OnBeforeChanged(this, e);
					if (e.Cancel)
					{
						Debug.WriteLine("The property 'Name' could not be changed because the 'BeforeChanged' was cancelled.");
						return;
					}

					_name = value;

					OnAfterChanged(this, new ObjectEventArgs(this, ObjectActions.Changed));
				}
			}
		}

		/// <summary>
		/// Gets or sets a comment used to describe the object.
		/// </summary>
		public string Description
		{
			get
			{
				lock (SyncRoot)
				{
					return _description;
				}
			}
			set
			{
				lock (SyncRoot)
				{
					var e = new ObjectCancelEventArgs(this, ObjectActions.Changed, false);
					OnBeforeChanged(this, e);
					if (e.Cancel)
					{
						Debug.WriteLine("The property 'Description' could not be changed because the 'BeforeChanged' was cancelled.");
						return;
					}

					_description = value;

					OnAfterChanged(this, new ObjectEventArgs(this, ObjectActions.Changed));
				}
			}
		}

		/// <summary>
		/// Gets or sets the small image associated with this object.
		/// </summary>
		public Image SmallImage
		{
			get
			{
				lock (SyncRoot)
				{
					return _smallImage;
				}
			}
			set
			{
				lock (SyncRoot)
				{
					var e = new ObjectCancelEventArgs(this, ObjectActions.Changed, false);
					OnBeforeChanged(this, e);
					if (e.Cancel)
					{
						Debug.WriteLine("The property 'SmallImage' could not be changed because the 'BeforeChanged' was cancelled.");
						return;
					}

					_smallImage = value;

					OnAfterChanged(this, new ObjectEventArgs(this, ObjectActions.Changed));
				}
			}
		}

		/// <summary>
		/// Gets or sets the large image associated with this object.
		/// </summary>
		public Image LargeImage
		{
			get
			{
				lock (SyncRoot)
				{
					return _largeImage;
				}
			}
			set
			{
				lock (SyncRoot)
				{
					var e = new ObjectCancelEventArgs(this, ObjectActions.Changed, false);
					OnBeforeChanged(this, e);
					if (e.Cancel)
					{
						Debug.WriteLine("The property 'LargeImage' could not be changed because the 'BeforeChanged' was cancelled.");
						return;
					}

					_largeImage = value;

					OnAfterChanged(this, new ObjectEventArgs(this, ObjectActions.Changed));
				}
			}
		}

		//protected virtual void ResetChanged()
		//{
		//    lock (this)
		//    {
		//        if (this.AfterChanged != null)
		//        {
		//            Delegate[] invocationList = this.AfterChanged.GetInvocationList();
		//            if (invocationList != null)
		//            {
		//                foreach (Delegate subscriber in invocationList)
		//                {
		//                    this.AfterChanged -= (EventHandler<ObjectEventArgs>)subscriber;
		//                }
		//            }
		//        }
		//    }
		//}

		/// <summary>
		/// Raises the BeforeChanged event. The event will be cancelled if any exceptions are caught while raising the event.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The EventArgs for the event.</param>
		protected virtual void OnBeforeChanged(object sender, ObjectCancelEventArgs e)
		{
			try
			{
				// required for thread affinity
				var handler = BeforeChanged;
				if (handler != null)
				{
					handler(sender, e);
				}				
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);

				// capture the exception and cancel the change here
				e.Cancel = true;
			}				
		}

		/// <summary>
		/// Raises the AfterChanged event.
		/// </summary>
		/// <param name="sender">The sender of the event.</param>
		/// <param name="e">The EventArgs for the event.</param>
		protected virtual void OnAfterChanged(object sender, ObjectEventArgs e)
		{
			EventManager.Raise(AfterChanged, sender, e);
		}

		#region ICloneable Members

		/// <summary>
		/// Returns a shallow copy of this object.
		/// </summary>
		/// <returns></returns>
		public virtual object Clone()
		{
			return MemberwiseClone();
		}

		#endregion

		#region ISerializable Members

		public ObjectBase(SerializationInfo info, StreamingContext context)
		{
			_name = (string)info.GetValue("Name", typeof(string));
			_description = (string)info.GetValue("Description", typeof(string));
			_smallImage = info.GetValue("SmallImage", typeof(Image)) as Image;
			_largeImage = info.GetValue("LargeImage", typeof(Image)) as Image;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", _name, typeof(string));
			info.AddValue("Description", _description, typeof(string));
			info.AddValue("SmallImage", _smallImage, typeof(Image));
			info.AddValue("LargeImage", _largeImage, typeof(Image));
		}

		#endregion
	}
}
