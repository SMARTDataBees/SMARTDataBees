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
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace Carbon.Configuration 
{
	/// <summary>
	/// Summary description for XmlConfigurationOptionCollection.
	/// </summary>
	public class XmlConfigurationOptionCollection : CollectionBase, ICloneable, ISupportsEditing, IXmlConfigurationElementEvents, ISupportInitialize
	{
		private XmlConfigurationCategory _parent;
		private bool _hasChanges;
		protected bool _isBeingEdited;	
		private bool _isBeingInitialized;
		
		#region Instance Constructors

	    #endregion

		#region Public Methods

		/// <summary>
		/// Adds the option to this collection
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public int Add(XmlConfigurationOption option) 
		{				
			if (Contains(option))
				throw new ArgumentException("ElementName already exists. ElementName in collection: " + option.ElementName + " ElementName being added: " + option.ElementName);
			
			option.Parent = this;
			option.BeforeEdit += OnBeforeEdit;
			option.Changed += OnChanged;
			option.AfterEdit += OnAfterEdit;
			option.EditCancelled += OnEditCancelled;
			var index = InnerList.Add(option);
			OnChanged(this, new XmlConfigurationOptionEventArgs(option, XmlConfigurationElementActions.Added));			
			return index;
		}

		/// <summary>
		/// Adds the array of options to this collection
		/// </summary>
		/// <param name="options"></param>
		public void Add(XmlConfigurationOption[] options) 
		{
			if (options == null)
				throw new ArgumentNullException("options");			

			foreach(var opt in options) 
			{
				try 
				{
					Add(opt);
				}
				catch(Exception ex) 
				{
					Debug.WriteLine(ex);
				}
			}
		}

		/// <summary>
		/// Removes the option from this collection
		/// </summary>
		/// <param name="option"></param>
		public void Remove(XmlConfigurationOption option) 
		{
			if (Contains(option))
			{											
				option.BeforeEdit -= OnBeforeEdit;
				option.Changed -= OnChanged;
				option.AfterEdit -= OnAfterEdit;
				option.EditCancelled -= OnEditCancelled;
				InnerList.Remove(option);
				OnChanged(this, new XmlConfigurationOptionEventArgs(option, XmlConfigurationElementActions.Removed));
			}
		}

		/// <summary>
		/// Determines whether an option exists using the specified option's elementName
		/// </summary>
		/// <param name="option"></param>
		/// <returns></returns>
		public bool Contains(XmlConfigurationOption option) 
		{
			foreach(XmlConfigurationOption opt in InnerList)
				if (opt.ElementName == option.ElementName)
					return true;
			return false;
		}				

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the option at the specified index
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOption this[int index] 
		{
			get 
			{
				return InnerList[index] as XmlConfigurationOption;
			}
			set 
			{
				InnerList[index] = value;
			}
		}

		/// <summary>
		/// Gets the option using the specified elementName
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOption this[string elementName]
		{
			get
			{
				foreach(XmlConfigurationOption opt in InnerList)
					if (opt.ElementName == elementName)
						return opt;
				return null;
			}
		}

		/// <summary>
		/// Gets or adds the option using the specified elementName
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOption this[string elementName, bool createIfNotFound]
		{
			get
			{	
				var option = this[elementName];				
				
				if (option == null)
					if (createIfNotFound)
					{
						option = new XmlConfigurationOption(elementName, null);
						Add(option);
						option = this[elementName];
					}
				return option;			
			}
		}

		/// <summary>
		/// Gets or adds the option using the specified elementName and default value
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationOption this[string elementName, bool createIfNotFound, object defaultValue]
		{
			get
			{
				var option = this[elementName];				
				
				if (option == null)
					if (createIfNotFound)
					{
						option = new XmlConfigurationOption(elementName, defaultValue);
						Add(option);
						option = this[elementName];
					}
				return option;			
			}
		}

		/// <summary>
		/// Gets or sets the category of which this option collection is a child
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationCategory Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		/// <summary>
		/// Gets or sets the full path to this option collection is a child
		/// </summary>
		[Browsable(false)]
		public string Fullpath
		{
			get
			{
			    if (_parent == null)
					return null;
			    return _parent.Fullpath;
			}
		}
		
		[Browsable(false)]
		public XmlConfiguration Configuration
		{
			get
			{
				if (_parent != null)
					return _parent.Configuration;
				return null;
			}
		}

		#endregion

		#region ICloneable Members

		public object Clone()
		{
			var clone = new XmlConfigurationOptionCollection();
			clone.ResetBeforeEdit();
			clone.ResetChanged();
			clone.ResetAfterEdit();
			clone.ResetEditCancelled();

			foreach(XmlConfigurationOption option in InnerList)
			{
				var clonedOption = (XmlConfigurationOption)option.Clone();
				clonedOption.Parent = clone;
				clone.Add(clonedOption);
			} 

			return clone;
		}

		#endregion

		#region ISupportsEditing Members

		public event XmlConfigurationElementCancelEventHandler BeforeEdit;
		public event XmlConfigurationElementEventHandler AfterEdit;
		public event XmlConfigurationElementEventHandler EditCancelled;

		public bool IsBeingEdited
		{
			get
			{
				return _isBeingEdited;
			}
		}

		public bool BeginEdit()
		{
			try
			{
				// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself
				_isBeingEdited = true;

				foreach(XmlConfigurationOption option in InnerList)
				{
					try
					{
						option.BeginEdit();
					}
					catch(Exception ex)
					{
						Debug.WriteLine(ex);
					}
				}
				return true;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return false;
		}

		public bool EndEdit()
		{
			try
			{
				if (_isBeingEdited)
				{									
					// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself
					_isBeingEdited = false;
					
					foreach(XmlConfigurationOption option in InnerList)
					{
						try
						{
							option.EndEdit();
						}
						catch(Exception ex)
						{
							Debug.WriteLine(ex);
						}
					}
					return true;
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return false;
		}

		public virtual bool CancelEdit()
		{
			try
			{
				if (_isBeingEdited)
				{
					_isBeingEdited = false;

					foreach(XmlConfigurationOption option in InnerList)
					{
						try
						{
							option.CancelEdit();
						}
						catch(Exception ex)
						{
							Debug.WriteLine(ex);
						}
					}
				}
				return true;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return false;
		}

		public void OnBeforeEdit(object sender, XmlConfigurationElementCancelEventArgs e)
		{
			try
			{
				if (BeforeEdit != null)
					BeforeEdit(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void OnAfterEdit(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{
				if (AfterEdit != null)
					AfterEdit(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void OnEditCancelled(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{
				if (EditCancelled != null)
					EditCancelled(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void ResetBeforeEdit()
		{
			lock(this)
			{
				if (BeforeEdit != null)
				{
					var invocationList = BeforeEdit.GetInvocationList();
					if (invocationList != null)
					{
						foreach(var subscriber in invocationList)
							BeforeEdit -= (XmlConfigurationElementCancelEventHandler)subscriber;
					}
				}
			}
		}

		public void ResetAfterEdit()
		{
			lock(this)
			{
				if (AfterEdit != null)
				{
					var invocationList = AfterEdit.GetInvocationList();
					if (invocationList != null)
					{
						foreach(var subscriber in invocationList)
							AfterEdit -= (XmlConfigurationElementEventHandler)subscriber;
					}
				}
			}
		}

		public void ResetEditCancelled()
		{
			lock(this)
			{
				if (EditCancelled != null)
				{
					var invocationList = EditCancelled.GetInvocationList();
				    foreach(var subscriber in invocationList)
				        EditCancelled -= (XmlConfigurationElementEventHandler)subscriber;
				}
			}
		}

		public bool HasChanges
		{
			get
			{
				var anyOption = false;
				foreach(XmlConfigurationOption option in InnerList)
					if (option.HasChanges)
						anyOption = true;
				return _hasChanges || anyOption;
			}
			set
			{
				_hasChanges = value;
			}
		}

		public void AcceptChanges()
		{
			foreach(XmlConfigurationOption option in InnerList)
				option.AcceptChanges();
			_hasChanges = false;
		}

		public bool ApplyChanges(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			var options = editableObject as XmlConfigurationOptionCollection;
			if (options != null)
			{	
				foreach(XmlConfigurationOption option in options)
				{					
					var myOption = this[option.ElementName];
					if (myOption != null)
					{
						try
						{
							myOption.ApplyChanges(option, actions);
						}
						catch(Exception ex)
						{
							Debug.WriteLine(ex);
						}
					}
				}

			}
			return true;
		}

		public bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			var options = editableObject as XmlConfigurationOptionCollection;
			if (options != null)
			{	
				foreach(XmlConfigurationOption option in options)
				{					
					var myOption = this[option.ElementName];
					if (myOption != null)
					{
						try
						{
							myOption.ApplyToSelf(option, actions);
						}
						catch(Exception ex)
						{
							Debug.WriteLine(ex);
						}
					}
				}
			}
			return true;
		}

		#endregion

		#region IXmlConfigurationElementEvents Members

		public event XmlConfigurationElementEventHandler Changed;

		public void OnChanged(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{
				// no events during initialization
				if (_isBeingInitialized) return;

				_hasChanges = true;
				
				//EventTracing.TraceMethodAndDelegate(this, this.Changed);

				if (Changed != null)
					Changed(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public void ResetChanged()
		{
			lock(this)
			{
				if (Changed != null)
				{
					var invocationList = Changed.GetInvocationList();
					if (invocationList != null)
					{
						foreach(var subscriber in invocationList)
							Changed -= (XmlConfigurationElementEventHandler)subscriber;
					}
				}
			}
		}

		#endregion

		#region ISupportInitialize Members

		public virtual void BeginInit()
		{
			_isBeingInitialized = true;

			foreach(XmlConfigurationOption option in InnerList)
				option.BeginInit();
		}

		public virtual void EndInit()
		{
			_isBeingInitialized = false;

			foreach(XmlConfigurationOption option in InnerList)
				option.EndInit();
		}

		#endregion
	}
}
