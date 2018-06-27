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

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace Carbon.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationCategory.
	/// </summary>
	[DefaultProperty("Options")]
	[TypeConverter(typeof(XmlConfigurationCategoryTypeConverter))]
    //[Designer(typeof(XmlConfigurationCategoryDesigner))]
	public class XmlConfigurationCategory : XmlConfigurationElement 
	{		
		protected XmlConfigurationOptionCollection _options;
		protected XmlConfigurationCategoryCollection _categories;
		protected XmlConfigurationCategoryCollection _parent;
//		private bool _isEndingEdit;

		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationCategory class
		/// </summary>
		public XmlConfigurationCategory()
		{			
			
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationCategory class
		/// </summary>
		public XmlConfigurationCategory(string elementName) : base(elementName)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationCategory class
		/// </summary>
		public XmlConfigurationCategory(string elementName, string description, string category, string displayName) : base(elementName, description, category, displayName)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationCategory class
		/// </summary>
		public XmlConfigurationCategory(XmlConfigurationElement element) : base(element)
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationCategory class
		/// </summary>
		public XmlConfigurationCategory(XmlConfigurationCategory category) : base(category)
		{
			_options = category.Options;
			_categories = category.Categories;
			_parent = category.Parent;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a collection of options (sub-options for this XmlConfigurationElement)
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		[Description("The collection of options contained in this category.")]
		[Category("Category Properties")]
		public XmlConfigurationOptionCollection Options
		{
			get
			{	
				if (_options == null)
				{
					_options = new XmlConfigurationOptionCollection();	
					_options.Parent = this;
					_options.BeforeEdit += OnBeforeEdit;
					_options.Changed += OnChanged;
					_options.AfterEdit += OnAfterEdit;
					_options.EditCancelled += OnEditCancelled;
					if (_isBeingInitialized)
						_options.BeginInit();
				}
				return _options;
			}
			set
			{
				_options = (XmlConfigurationOptionCollection)value.Clone();
				_options.Parent = this;
				_options.BeforeEdit += OnBeforeEdit;
				_options.Changed += OnChanged;
				_options.AfterEdit += OnAfterEdit;
				_options.EditCancelled += OnEditCancelled;
				if (_isBeingInitialized)
					_options.BeginInit();
			}
		}

		/// <summary>
		/// Gets or sets a collection of categories (sub-categories for this XmlConfigurationElement)
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		[Description("The collection of categories contained in this category.")]
		[Category("Category Properties")]
		public XmlConfigurationCategoryCollection Categories
		{
			get
			{			
				if (_categories == null)
				{
					_categories = new XmlConfigurationCategoryCollection();	
					_categories.Parent = this;
					_categories.BeforeEdit += OnBeforeEdit;
					_categories.Changed += OnChanged;			
					_categories.AfterEdit += OnAfterEdit;
					_categories.EditCancelled += OnEditCancelled;
					if (_isBeingInitialized)
						_categories.BeginInit();
				}
				return _categories;
			}
			set
			{
				_categories = (XmlConfigurationCategoryCollection)value.Clone();				
				_categories.Parent = this;
				_categories.BeforeEdit += OnBeforeEdit;
				_categories.Changed += OnChanged;			
				_categories.AfterEdit += OnAfterEdit;
				_categories.EditCancelled += OnEditCancelled;
				if (_isBeingInitialized)
					_categories.BeginInit();
			}
		}

		/// <summary>
		/// Gets or sets the collection to which this category is a child
		/// </summary>
		[Browsable(false)]
		public XmlConfigurationCategoryCollection Parent
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

		#endregion

		#region Public Methods

		/// <summary>
		/// Removes this category from the collection it belongs to
		/// </summary>
		public void Remove()
		{
			if (_parent != null)
				_parent.Remove(this);			
		}
	
		#endregion

		public override string Fullpath
		{
			get
			{
				if (_parent == null)
					return base.Fullpath;
			    var path = _parent.Fullpath;
			    return (path != null ? path + @"\" + base.Fullpath : base.Fullpath);
			}
		}

		public override bool HasChanges
		{
			get
			{
				var anyCategory = false; 
				var anyOption = false;
							
				if (_categories != null)
					anyCategory = _categories.HasChanges;

				if (_options != null)
					anyOption = _options.HasChanges;				

				return (base.HasChanges || anyCategory || anyOption);
			}
			set
			{
				base.HasChanges = value;
			}
		}

		public override void BeginInit()
		{
			base.BeginInit ();

			if (_options != null)
				_options.BeginInit();

			if (_categories != null)
				_categories.BeginInit();
		}

		public override void EndInit()
		{
			if (_options != null)
				_options.EndInit();

			if (_categories != null)
				_categories.EndInit();

			base.EndInit ();
		}


		public override bool BeginEdit()
		{
			base.BeginEdit();

			if (_options != null)
				_options.BeginEdit();

			if (_categories != null)
				_categories.BeginEdit();

			return true;;
		}

		public override bool EndEdit()
		{			
			if (_options != null)
				_options.EndEdit();

			if (_categories != null)
				_categories.EndEdit();				
		
			return base.EndEdit();							
		}

		public override bool CancelEdit()
		{						
			if (_options != null)
				_options.CancelEdit();

			if (_categories != null)
				_categories.CancelEdit();			
			
			return base.CancelEdit();;		
		}

		public override void AcceptChanges()
		{						
			if (_options != null)
				_options.AcceptChanges();
			
			if (_categories != null)
				_categories.AcceptChanges();
			
			base.AcceptChanges();
		}

//		protected override XmlConfigurationElement GetElementToEdit()
//		{
//			XmlConfigurationCategory category = (XmlConfigurationCategory)this.Clone();
//			category.Parent = this.Parent;
//			return (XmlConfigurationElement)category;
//		}

		public override bool ApplyChanges(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			if (base.ApplyChanges (editableObject, actions))
			{
				var category = editableObject as XmlConfigurationCategory;			
				if (category != null)
				{
					if (_categories != null)
						_categories.ApplyChanges(category.Categories, actions);
				}
				return true;
			}
			return false;
		}	

		public override bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			if (base.ApplyToSelf(editableObject, actions))
			{
				var category = editableObject as XmlConfigurationCategory;			
				if (category != null)
				{
					if (_options != null)
						_options.ApplyToSelf(category.Options, actions);					

					if (_categories != null)
						_categories.ApplyToSelf(category.Categories, actions);
				}
				return true;
			}
			return false;
		}	

		/// <summary>
		/// Returns a clone of this category
		/// </summary>
		/// <returns></returns>
		public new XmlConfigurationCategory Clone()
		{						
			XmlConfigurationCategory clone = null;
			var element = (XmlConfigurationElement)base.Clone();
			if (element != null)
			{
				clone = new XmlConfigurationCategory(element);
				clone.ResetBeforeEdit();
				clone.ResetChanged();
				clone.ResetAfterEdit();
				clone.ResetEditCancelled();
				
				if (_options != null)
					clone.Options = (XmlConfigurationOptionCollection)_options.Clone();

				if (_categories != null)
					clone.Categories = (XmlConfigurationCategoryCollection)_categories.Clone();
			}
			return clone;
		}

		public override XmlConfiguration Configuration
		{
			get
			{
				if (_parent != null)
					return _parent.Configuration;
				return base.Configuration;
			}
		}

		public XmlConfigurationCategory FindCategory(string keyOrPath)
		{			
			return Categories.FindCategory(keyOrPath);
		}
	}
}
