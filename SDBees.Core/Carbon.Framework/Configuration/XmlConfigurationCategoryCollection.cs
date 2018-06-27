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
	/// Summary description for XmlConfigurationCategoryCollection.
	/// </summary>
	public class XmlConfigurationCategoryCollection : CollectionBase, ICloneable, ISupportsEditing, IXmlConfigurationElementEvents, ISupportInitialize
	{
	    private bool _hasChanges;
	    private bool _isBeingInitialized;

		#region Instance Constructors

	    #endregion

		#region Public Methods

		public int Add(XmlConfigurationCategory category)
		{
			if (Contains(category))
				throw new ArgumentException("ElementName already exists. ElementName in collection: " + category.ElementName + " ElementName being added: " + category.ElementName);
				
			category.Parent = this;
			category.BeforeEdit += OnBeforeEdit;
			category.Changed += OnChanged;			
			category.AfterEdit += OnAfterEdit;
			category.EditCancelled += OnEditCancelled;
			var index = InnerList.Add(category);
			OnChanged(this, new XmlConfigurationCategoryEventArgs(category, XmlConfigurationElementActions.Added));
			return index;
		}

		public void Add(XmlConfigurationCategory[] categories)
		{
			if (categories == null)
				throw new ArgumentNullException("categories");			

			foreach(var category in categories) 
			{
				try 
				{
					Add(category);
				}
				catch(Exception ex) 
				{
					Debug.WriteLine(ex);
				}
			}
		}

		public void Add(string elementName)
		{
			XmlConfigurationCategory category = null;

			if (!Contains(elementName))
			{
				category = new XmlConfigurationCategory(elementName);
				Add(category);
			}
		}

		public void Insert(int index, XmlConfigurationCategory category)
		{
			if (Contains(category))
				throw new ArgumentException("ElementName already exists. ElementName in collection: " + category.ElementName + " ElementName being added: " + category.ElementName);
			
			category.Parent = this;
			category.BeforeEdit += OnBeforeEdit;
			category.Changed += OnChanged;			
			category.AfterEdit += OnAfterEdit;
			category.EditCancelled += OnEditCancelled;
			InnerList.Insert(index, category);
			OnChanged(this, new XmlConfigurationCategoryEventArgs(category, XmlConfigurationElementActions.Added));			
		}

		public void Remove(XmlConfigurationCategory category)
		{
			if (Contains(category))
			{											
				category.BeforeEdit -= OnBeforeEdit;
				category.Changed -= OnChanged;			
				category.AfterEdit -= OnAfterEdit;
				category.EditCancelled -= OnEditCancelled;
				InnerList.Remove(category);
				OnChanged(this, new XmlConfigurationCategoryEventArgs(category, XmlConfigurationElementActions.Removed));
			}		
		}

		public void Remove(string elementName)
		{
			if (Contains(elementName))
				foreach(XmlConfigurationCategory category in InnerList)
					if (category.ElementName == elementName)
						Remove(category);					
		}

		public bool Contains(XmlConfigurationCategory category)
		{
			foreach(XmlConfigurationCategory c in InnerList)
				if (c.ElementName == category.ElementName)
					return true;
			
			return false;
		}

		public bool Contains(string elementName)
		{
			foreach(XmlConfigurationCategory c in InnerList)
				if (c.ElementName == elementName)
					return true;
			return false;
		}

		#endregion

		#region Public Properties

		public XmlConfigurationCategory this[int index]
		{
			get
			{
				return InnerList[index] as XmlConfigurationCategory;
			}
			set
			{
				InnerList[index] = value;
			}
		}

		public XmlConfigurationCategory this[string keyOrPath]
		{
			get
			{
				var categories = keyOrPath.Split(XmlConfiguration.CategoryPathSeparators);
				foreach(XmlConfigurationCategory category in InnerList)
				{
					if (category.ElementName == categories[0])
					{
						if (categories.Length == 1)
						{
							return category;
						}
					    keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);
					    return category.Categories[keyOrPath];
					}					
				}
				return null;
			}
		}

		public XmlConfigurationCategory this[string keyOrPath, bool createIfNotFound]
		{
			get
			{
				var categories = keyOrPath.Split(XmlConfiguration.CategoryPathSeparators);
				foreach(XmlConfigurationCategory category in InnerList)
				{
					if (category.ElementName == categories[0])
					{
						if (categories.Length == 1)
							return category;
					    keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);

					    var subCategory = category.Categories[keyOrPath];
						if (subCategory != null)
							return subCategory;
					    break;
					}
				}

				if (createIfNotFound)
					if (categories.Length > 0)
					{
						Add(categories[0]);
						var newCategory = this[categories[0]];
						if (categories.Length == 1)
							return newCategory;
					    keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);

					    return newCategory.Categories[keyOrPath, createIfNotFound];
					}
				return null;
			}
		}

		/// find category recursively by key/path/elementname
		public XmlConfigurationCategory FindCategory(string keyOrPath)
		{
			try
			{
				// chunk up the path into the separate categories
				var categories = keyOrPath.Split(XmlConfiguration.CategoryPathSeparators);
				
				// at the first level, look for the first category in our list, if we find it and it's the last one then return it
				foreach(XmlConfigurationCategory category in InnerList)
				{
					if (category.ElementName == categories[0])
					{
						if (categories.Length == 1)
						{
							return category;						
						}
					    // chomp the first category off
					    keyOrPath = string.Join(XmlConfiguration.DefaultPathSeparator, categories, 1, categories.Length - 1);
					    return category.FindCategory(keyOrPath);
					}
				}							
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}

		/// <summary>
		/// Gets or sets the full path to this option collection is a child
		/// </summary>
		[Browsable(false)]
		public string Fullpath
		{
			get
			{
			    if (Parent == null)
					return null;
			    return Parent.Fullpath;
			}
		}
		
		/// <summary>
		/// Gets or sets the element to which this collection is a child (Either a XmlConfiguration or a XmlConfigurationCategory)
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationElement Parent { get; set; }

	    [Browsable(false)]
		public XmlConfiguration Configuration
		{
			get
			{
				if (Parent != null)
				{
					if (Parent.GetElementType() == XmlConfigurationElementTypes.XmlConfiguration)
						return ((XmlConfiguration)Parent).Configuration;

					if (Parent.GetElementType() == XmlConfigurationElementTypes.XmlConfigurationCategory)
						return ((XmlConfigurationCategory)Parent).Configuration;
				}
				return null;
			}
		}

		#endregion

		#region ICloneable Members

		/// <summary>
		/// Clones this category collection
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			var clone = new XmlConfigurationCategoryCollection();
			clone.ResetBeforeEdit();
			clone.ResetChanged();
			clone.ResetAfterEdit();
			clone.ResetEditCancelled();
			clone.Parent = Parent;

			foreach(XmlConfigurationCategory category in InnerList)
			{
				var clonedCategory = category.Clone();				
				clonedCategory.Parent = clone;
				clone.Add(clonedCategory);
			}

			return clone;
		}

		#endregion

		#region ISupportsEditing Members

		public event XmlConfigurationElementCancelEventHandler BeforeEdit;
		public event XmlConfigurationElementEventHandler AfterEdit;
		public event XmlConfigurationElementEventHandler EditCancelled;

		public bool IsBeingEdited { get; protected set; }

	    public bool BeginEdit()
		{
			try
			{
				if (!IsBeingEdited)
				{									
					// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself
					IsBeingEdited = true;

					foreach(XmlConfigurationCategory category in InnerList)
					{
						try
						{
							category.BeginEdit();
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

		public bool EndEdit()
		{
			try
			{
				if (IsBeingEdited)
				{									
					// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself
					IsBeingEdited = false;
					
					foreach(XmlConfigurationCategory category in InnerList)
					{
						try
						{
							category.EndEdit();
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

		public virtual bool CancelEdit()
		{
			try
			{
				if (IsBeingEdited)
				{
					IsBeingEdited = false;

					foreach(XmlConfigurationCategory category in InnerList)
					{
						try
						{
							category.CancelEdit();
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
				//EventTracing.TraceMethodAndDelegate(this, this.BeforeEdit);

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
				//EventTracing.TraceMethodAndDelegate(this, this.AfterEdit);

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
				//EventTracing.TraceMethodAndDelegate(this, this.EditCancelled);

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
					if (invocationList != null)
					{
						foreach(var subscriber in invocationList)
							EditCancelled -= (XmlConfigurationElementEventHandler)subscriber;
					}
				}
			}
		}

		public bool HasChanges
		{
			get
			{
				var anyCategory = false;
				foreach(XmlConfigurationCategory category in InnerList)
					if (category.HasChanges)
						anyCategory = true;
				return _hasChanges || anyCategory;
			}
			set
			{
				_hasChanges = value;
			}
		}

		public void AcceptChanges()
		{
			foreach(XmlConfigurationCategory category in InnerList)
				category.AcceptChanges();				
			_hasChanges = false;
		}

		public bool ApplyChanges(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			var categories = editableObject as XmlConfigurationCategoryCollection;
			if (categories != null)
			{					
				foreach(XmlConfigurationCategory category in categories)
				{					
					var myCategory = this[category.ElementName];
					if (myCategory != null)
					{
						try
						{
							myCategory.ApplyChanges(category, actions);
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
			var categories = editableObject as XmlConfigurationCategoryCollection;
			if (categories != null)
			{	
				foreach(XmlConfigurationCategory category in categories)
				{					
					var myCategory = this[category.ElementName];
					if (myCategory != null)
					{
						try
						{
							myCategory.ApplyToSelf(category, actions);
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

			foreach(XmlConfigurationCategory category in InnerList)
				category.BeginInit();
		}

		public virtual void EndInit()
		{
			_isBeingInitialized = false;

			foreach(XmlConfigurationCategory category in InnerList)
				category.EndInit();
		}

		#endregion
	}
}
