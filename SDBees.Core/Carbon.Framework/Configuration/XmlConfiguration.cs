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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms.Design;

namespace Carbon.Configuration
{
	/// <summary>
	/// Summary description for XmlConfiguration.
	/// </summary>
	[DefaultProperty("Categories")]	
	[TypeConverter(typeof(XmlConfigurationTypeConverter))]
	public class XmlConfiguration : XmlConfigurationElement
	{				
		/// <summary>
		/// The configuration will have unsaved changes by default until someone saves it or explicitly sets this to false
		/// </summary>
		protected bool _hasUnpersistedChanges = true;

		/// <summary>
		/// The path where this configuration is persisted
		/// </summary>
		protected string _path = string.Empty;

		/// <summary>
		/// The collection of categories contained in this configuration
		/// </summary>
		protected XmlConfigurationCategoryCollection _categories;

		/// <summary>
		/// Gets an array of valid path separators used by the configuration classes
		/// </summary>
		public static readonly char[] CategoryPathSeparators = {'\\', '/'};

		/// <summary>
		/// Gets the default path separator: a backslash
		/// </summary>
		public static readonly string DefaultPathSeparator = "\\";

		public event EventHandler TimeToSave;

		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the XmlConfiguration class
		/// </summary>
		public XmlConfiguration()
		{
		
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		/// <param name="element">The element to base this option on</param>
		public XmlConfiguration(XmlConfigurationElement element) : base(element)
		{

		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOption class
		/// </summary>
		/// <param name="option">The option to base this option on</param>
		public XmlConfiguration(XmlConfiguration configuration) : base(configuration)
		{
			_path = configuration.Path;
			_categories = configuration.Categories;			
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets a collection of categories (sub-categories for this XmlConfigurationElement)
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
		[Description("The collection of categories contained in this configuration.")]
		[Category("Configuration Properties")]
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
					_categories.Changed += Categories_Changed;
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
				_categories.Changed += Categories_Changed;
				if (_isBeingInitialized)
					_categories.BeginInit();
			}
		}

		/// <summary>
		/// Gets or sets the path where this configuration is saved (Example: 'C:\MyApp\MyConfig.xml')
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
		[Description("The location of the file.")]
		[Category("Configuration Properties")]
		public string Path
		{
			get => _path;
		    set
			{
				try
				{
					if (string.Compare(_path, value) == 0)
						return;
				}
				catch(Exception){ /*screw it*/ }

				_path = value;
				OnChanged(this, new XmlConfigurationEventArgs(this, XmlConfigurationElementActions.Changed));
			}
		}

		/// <summary>
		/// Determines whether changes have been made since the last time the object was persisted
		/// </summary>
		/// <returns></returns>
		public bool HasUnpersistedChanges()
		{
			return _hasUnpersistedChanges;
		}

		/// <summary>
		/// Sets whether changes have been made since the last time the object was persisted, used internally by the ConfigurationEngine class
		/// </summary>
		/// <param name="value"></param>
		public void SetHasUnpersistedChanges(bool value)
		{
			_hasUnpersistedChanges = value;
		}

		#endregion

		public override bool HasChanges
		{
			get
			{
				var anyCategory = false;
				
				// check this configuration's categories recursively for changes
				if (_categories != null)
					anyCategory = _categories.HasChanges;

				return base.HasChanges || anyCategory;
			}
			set
			{
				base.HasChanges = value;
			}
		}



		public void TraceCategories()
		{
			TraceCategories(_categories);
		}

		internal void TraceCategories(XmlConfigurationCategoryCollection categories)
		{
			Debug.WriteLine(categories.Fullpath);
			foreach(XmlConfigurationCategory category in categories)
				TraceCategories(category.Categories);
		}

		/// <summary>
		/// Writes the contents of this configuration to a string in the format of an XmlDocument
		/// </summary>
		/// <returns></returns>
		public string ToXml()
		{
			var stream = new MemoryStream();
			var writer = new XmlConfigurationWriter();
			writer.Write(this, stream, true);			
			var xml = Encoding.ASCII.GetString(stream.GetBuffer());
			stream.Close();
			return xml;
		}

		/// <summary>
		/// Clones this configuration
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			XmlConfiguration clone = null;
			var element = (XmlConfigurationElement)base.Clone();
			if (element != null)
			{
				clone = new XmlConfiguration(element);
				clone.ResetBeforeEdit();
				clone.ResetChanged();
				clone.ResetAfterEdit();
				clone.Path = Path;
				clone.SetHasUnpersistedChanges(HasUnpersistedChanges());
				clone.Categories =  (XmlConfigurationCategoryCollection)Categories.Clone();
			}	
			return clone;
		}
		
		public override void BeginInit()
		{
			base.BeginInit ();

			if (_categories != null)
				_categories.BeginInit();
		}

		public override void EndInit()
		{
			if (_categories != null)
				_categories.EndInit();

			base.EndInit ();
		}

		public override bool BeginEdit()
		{
			if (base.BeginEdit())
			{
				if (_categories != null)
					_categories.BeginEdit();
				
				return true;
			}
			return false;
		}

		public override bool CancelEdit()
		{
			if (_categories != null)
				_categories.CancelEdit();

			return base.CancelEdit ();
		}

		public override bool EndEdit()
		{			
			if (_categories != null)
				_categories.EndEdit();
			
			var result = base.EndEdit();						
			AcceptChanges();
			ItIsNowTimeToSave();
			return result;
		}
		
		protected override XmlConfigurationElement GetElementToEdit()
		{
			var configuration = (XmlConfiguration)Clone();
			return configuration;
		}

		public override void AcceptChanges()
		{
			if (_categories != null)
				_categories.AcceptChanges();
			base.AcceptChanges();
		}


		public override bool ApplyChanges(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			if (base.ApplyChanges (editableObject, actions))
			{
				var configuration = editableObject as XmlConfiguration;			
				if (configuration != null)
				{
					if (_isBeingEdited)
						BeginInit();									
										
					if (_categories != null)
						_categories.ApplyChanges(configuration.Categories, actions);

					if (_isBeingEdited)
						EndInit();
				}
				return true;
			}
			return false;
		}

		public override bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			if (base.ApplyToSelf(editableObject, actions))
			{
				var configuration = editableObject as XmlConfiguration;			
				if (configuration != null)
				{																				
					if (_categories != null)
						_categories.ApplyToSelf(configuration.Categories, actions);
				}
				return true;
			}

			ItIsNowTimeToSave();

			return false;
		}


		public override XmlConfiguration Configuration
		{
			get
			{
				return this;
			}
		}

		#region Static Methods

		public static string DescribeElementEnteringEdit(XmlConfigurationElementCancelEventArgs e)
		{
			try
			{
				string elementType = null;
				var et = e.Element.GetElementType();
				switch(et)
				{
				case XmlConfigurationElementTypes.XmlConfiguration:				elementType = "configuration"; break;
				case XmlConfigurationElementTypes.XmlConfigurationCategory:		elementType = "category"; break;		
				case XmlConfigurationElementTypes.XmlConfigurationElement:		elementType = "element"; break;
				case XmlConfigurationElementTypes.XmlConfigurationOption:		elementType = "option"; break;
				};
				
				return
				    $"The {elementType} '{e.Element.Fullpath}' is entering edit mode at {DateTime.Now.ToLongTimeString()} on {DateTime.Now.ToLongDateString()}. The current user is {Environment.UserName}.";				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}

		public static string DescribeElementChanging(XmlConfigurationElementEventArgs e)
		{
			try
			{
				string elementType = null;
				var et = e.Element.GetElementType();
				switch(et)
				{
				case XmlConfigurationElementTypes.XmlConfiguration:				elementType = "configuration"; break;
				case XmlConfigurationElementTypes.XmlConfigurationCategory:		elementType = "category"; break;		
				case XmlConfigurationElementTypes.XmlConfigurationElement:		elementType = "element"; break;
				case XmlConfigurationElementTypes.XmlConfigurationOption:		elementType = "option"; break;
				};
				
				return string.Format("The {5} '{0}' was '{1}' in the '{2}' configuration at {3} on {4}. The {5} is{6}being edited. The current user is {7}.", e.Element.Fullpath, e.Action.ToString(), e.Element.Configuration.DisplayName, DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString(), elementType, (e.Element.IsBeingEdited ? " " : " not "), Environment.UserName);				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}

		public static string DescribeElementLeavingEdit(XmlConfigurationElementEventArgs e)
		{
			try
			{
				string elementType = null;
				var et = e.Element.GetElementType();
				switch(et)
				{
				case XmlConfigurationElementTypes.XmlConfiguration:				elementType = "configuration"; break;
				case XmlConfigurationElementTypes.XmlConfigurationCategory:		elementType = "category"; break;		
				case XmlConfigurationElementTypes.XmlConfigurationElement:		elementType = "element"; break;
				case XmlConfigurationElementTypes.XmlConfigurationOption:		elementType = "option"; break;
				};
				
				return
				    $"The {elementType} '{e.Element.Fullpath}' is leaving edit mode at {DateTime.Now.ToLongTimeString()} on {DateTime.Now.ToLongDateString()}. The current user is {Environment.UserName}.";				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}

		public static string DescribeElementCancellingEdit(XmlConfigurationElementEventArgs e)
		{
			try
			{
				string elementType = null;
				var et = e.Element.GetElementType();
				switch(et)
				{
				case XmlConfigurationElementTypes.XmlConfiguration:				elementType = "configuration"; break;
				case XmlConfigurationElementTypes.XmlConfigurationCategory:		elementType = "category"; break;		
				case XmlConfigurationElementTypes.XmlConfigurationElement:		elementType = "element"; break;
				case XmlConfigurationElementTypes.XmlConfigurationOption:		elementType = "option"; break;
				};
				
				return
				    $"The {elementType} '{e.Element.Fullpath}' has cancelled edit mode at {DateTime.Now.ToLongTimeString()} on {DateTime.Now.ToLongDateString()}. The current user is {Environment.UserName}.";				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}

		#endregion

		/// <summary>
		/// Searches the tree of categories for the matching category by the specified key or path
		/// </summary>
		/// <param name="keyOrPath">The key or path of combined keys that uniquely identifies the category</param>
		/// <returns></returns>
		public XmlConfigurationCategory FindCategory(string keyOrPath)
		{
			// chunk up the path into the separate categories
			var categories = keyOrPath.Split(CategoryPathSeparators);
			
			// strip the config key from the path
			if (categories[0] == ElementName)
				keyOrPath = string.Join(DefaultPathSeparator, categories, 1, categories.Length - 1);

			return Categories.FindCategory(keyOrPath);
		}

		/// <summary>
		/// Occurs when a category has changed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Categories_Changed(object sender, XmlConfigurationElementEventArgs e)
		{
			if (!e.Element.IsBeingEdited)
			{
				_hasUnpersistedChanges = true;
			}
		}

		/// <summary>
		/// Fires the TimeToSave event
		/// </summary>
		public void ItIsNowTimeToSave()
		{
			// raise the event that it is time to save
			OnTimeToSave(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raises the TimeToSave event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void OnTimeToSave(object sender, EventArgs e)
		{
			try
			{
				if (TimeToSave != null)
					TimeToSave(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
	}
}
