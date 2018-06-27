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
using System.Diagnostics;

namespace Carbon.Configuration
{
	/// <summary>
	/// The base class for elements in the Core.Configuration namespace. Provides a basic set of properties to describe the element.
	/// </summary>
//	[DesignTimeVisible(false)]
	[DefaultProperty("ElementName")]
	[TypeConverter(typeof(XmlConfigurationElementTypeConverter))] 
	public class XmlConfigurationElement : Component, ICloneable, ISupportInitialize, ISupportsEditing, IXmlConfigurationElementEvents
	{
		protected string _elementName;
		protected string _description;
		protected string _category;
		protected string _displayName;
		protected bool _hidden;
		protected bool _readonly;
		protected bool _persistent;
		protected bool _hasChanges;
		protected bool _isBeingInitialized;
		protected bool _isBeingEdited;		
		protected XmlConfigurationElement _editableProxy;
		protected bool _isEditableProxy;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		protected Container components;
		
		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}

		#endregion

		private void SetDefaultValues()
		{
			_elementName = Guid.NewGuid().ToString();
			//			_description = null;
			//			_category = null;
			//			_displayName = null;
			//			_hidden = false;	
			//			_readonly = false;
			_persistent = true;
			//			_hasChanges = false;
			//			_isBeingInitialized = false;
			//			_isBeingEdited = false;		
			//			_editableProxy = null;
			//			_isEditableProxy = false;
		}

		#region Instance Constructors

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		/// <param name="container"></param>
		public XmlConfigurationElement(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
			SetDefaultValues();
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		public XmlConfigurationElement()
		{
			InitializeComponent();	
			SetDefaultValues();
		}


		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		/// <param elementName="elementName">The elementName of this element</param>
		public XmlConfigurationElement(string elementName) : this()
		{
			_elementName = elementName;
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		/// <param elementName="elementName">A elementName for this element</param>
		/// <param elementName="description">A description for this element</param>
		/// <param elementName="category">A category for this element</param>
		/// <param elementName="displayName">A display name for this element</param>
		public XmlConfigurationElement(string elementName, string description, string category, string displayName) : this(elementName)
		{
			_description = description;
			_category = category;
			_displayName = displayName;
		}

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationElement class
		/// </summary>
		/// <param name="element"></param>
		public XmlConfigurationElement(XmlConfigurationElement element) : this(element.ElementName)
		{			
			_description = element.Description;
			_category = element.Category;
			_displayName = element.DisplayName;
			_hidden = element.Hidden;
			_readonly = element.Readonly;
			_persistent = element.Persistent;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets the elementName of this XmlConfigurationElement
		/// </summary>			
		[Description("A elementName that uniquely identifies this element in a collection. Also when combined in a recursive collection will form a path that can be used to refer to this element. Example: \"Environment\\General\\MyOption\"")]
		[Category("Element Properties")]	
		public virtual string ElementName
		{
			get	
			{ 
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.ElementName;

				return _elementName; 
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.ElementName = value;
						return;
					}
				}

				if (_elementName == value)
					return;

//				_hasChanges = true;
				_elementName = value;
				OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets the description of this XmlConfigurationElement
		/// </summary>
//		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Description("The description text displayed at runtime in the property grid for this option.")]
		[Category("Element Properties")]
		public virtual string Description
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Description;

				return _description;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{							
						_editableProxy.Description = value;
						return;
					}
				}
				
				if (_description == value)
					return;

//				_hasChanges = true;
				_description = value;
				OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets the category of this XmlConfigurationElement
		/// </summary>
		[Description("The category in which this option will be grouped when displayed in the property grid with the other options in the same collection.")]
		[Category("Element Properties")]
		public virtual string Category
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Category;

				return _category;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.Category = value;
					}
				}
				
				if (_category == value)
					return;

//				_hasChanges = true;
				_category = value;				
				OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets the display elementName of this XmlConfigurationElement
		/// </summary>
		[Description("The text displayed at runtime in the property grid for the name of this option.")]
		[Category("Element Properties")]
		public virtual string DisplayName
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.DisplayName;
								
				if (_displayName == null || _displayName == string.Empty)
					return _elementName;

				return _displayName;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.DisplayName = value;
						return;
					}
				}
				
				if (_displayName == value)
					return;

//				_hasChanges = true;
				_displayName = value;
				OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets whether this option is hidden in the default editor.
		/// </summary>
		[Description("A flag that indicates whether this option will be displayed at runtime in the property grid.")]
		[Category("Element Properties")]
		public virtual bool Hidden
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Hidden;

				return _hidden;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.Hidden = value;
						return;
					}
				}
				
				if (_hidden == value)
					return;

//				_hasChanges = true;
				_hidden = value;
				OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets whether this option is readonly in the default editor.
		/// </summary>
		[Description("A flag that indicates whether this option will be readonly at runtime in the property grid.")]
		[Category("Element Properties")]
		public virtual bool Readonly
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Readonly;

				return _readonly;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.Readonly = value;
						return;
					}
				}
				
				if (_readonly == value)
					return;

//				_hasChanges = true;
				_readonly = value;
				OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets whether this option is persisted when the option is saved.
		/// </summary>
		[Description("A flag that describes whether this option is persistent (ie. written to the configuration file) or volatile.")]
		[Category("Element Properties")]
		public virtual bool Persistent
		{
			get
			{
				if (_isBeingEdited)
					if (_editableProxy != null)
						return _editableProxy.Persistent;

				return _persistent;
			}
			set
			{
				if (_isBeingEdited)
				{
					if (_editableProxy != null)
					{
						_editableProxy.Persistent = value;
						return;
					}
				}
				
				if (_persistent == value)
					return;

//				_hasChanges = true;
				_persistent = value;
				OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));				
			}
		}

		/// <summary>
		/// Gets or sets the full path to this option
		/// </summary>
		[Browsable(false)]
//		[Description("The full path of keys combined to form a path to this element. This property exists for all objects that inherit from XmlConfigurationElement.")]
//		[Category("Element Properties")]
		public virtual string Fullpath
		{
			get
			{								
				return _elementName;
			}
//			set
//			{
//				_fullpath = value;				
//			}
		}

		[Browsable(false)]
		public virtual XmlConfiguration Configuration
		{
			get
			{				
				return null;
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Determines the type of element that this object is.
		/// </summary>
		/// <returns></returns>
		public XmlConfigurationElementTypes GetElementType()
		{
			var thisType = GetType();
			if (thisType != null)
			{
				if (thisType == typeof(XmlConfigurationElement))
					return XmlConfigurationElementTypes.XmlConfigurationElement;

				if (thisType == typeof(XmlConfigurationOption))
					return XmlConfigurationElementTypes.XmlConfigurationOption;

				if (thisType == typeof(XmlConfigurationCategory))
					return XmlConfigurationElementTypes.XmlConfigurationCategory;

				if (thisType == typeof(XmlConfiguration))
					return XmlConfigurationElementTypes.XmlConfiguration;
			};
			return XmlConfigurationElementTypes.Null;
		}

		protected virtual XmlConfigurationElement GetElementToEdit()
		{
			return (XmlConfigurationElement)Clone();
		}

		#endregion

		#region ICloneable Members

//		public virtual object Clone()
//		{
//			XmlConfigurationElement clone = new XmlConfigurationElement();
//			
//			clone.ElementName = _elementName;
//			clone.Description = _description;
//			clone.Category = _category;
//			clone.DisplayName = _displayName;
//			clone.Hidden = _hidden;
//			clone.Readonly = _readonly;
//			clone.Persistent = _persistent;
////			clone.Fullpath = _fullpath;
//			clone.AcceptChanges();
//			
//			return clone;
//		}
		
		public virtual object Clone()
		{
			var clone = CloningEngine.Clone(this, CloningEngine.DefaultBindingFlags);
			return clone;
		}

		#endregion

		#region ISupportInitialize Members

		public virtual void BeginInit()
		{
			_isBeingInitialized = true;
		}

		public virtual void EndInit()
		{
			_isBeingInitialized = false;
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
				return _isBeingEdited || _isEditableProxy;
			}
		}

		public virtual bool BeginEdit()
		{
			try
			{
				if (!_isBeingEdited)
				{
					// Raise BeforeEdit event and provide a means of cancellation
					var e = new XmlConfigurationElementCancelEventArgs(this, false);
					OnBeforeEdit(this, e);
					if (e.Cancel)
						return false;
					
					// place the element in edit mode and clone ourself so that future changes will be redirected to the clone and not to ourself					
					_editableProxy = GetElementToEdit();		
					if (_editableProxy != null)
					{
						_editableProxy.Changed += OnChanged;
						_editableProxy._isEditableProxy = true;
						_isBeingEdited = true;
						return true;
					}					
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return false;
		}

		public virtual bool EndEdit()
		{
			var success = false;
			try
			{
				if (_isBeingEdited)
				{		
					_isBeingEdited = false;

					BeginInit();

					// apply the changes
					ApplyChanges(_editableProxy, SupportedEditingActions.Synchronize);

					EndInit();

					// destroy clone's event handler
					if (_editableProxy != null)
					{
						_editableProxy.Changed -= OnChanged;
						_editableProxy = null;
					}

					try
					{
						// make sure to kick this off so that no we are getting all events out
						if (HasChanges)
							OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));
					}
					catch (Exception ex)
					{
						Debug.WriteLine(ex);
					}
						
					// reset the haschanges flag and accept the current changes
					AcceptChanges();
				}
				success = true;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			finally
			{
				// raise the AfterEdit event
				OnAfterEdit(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.None));										
			}
			return success;			
		}

		public virtual bool CancelEdit()
		{
			try
			{
				if (_isBeingEdited)
				{
					_isBeingEdited = false;

					// destroy clone's event handler
					if (_editableProxy != null)
					{
						_editableProxy.Changed -= OnChanged;
						_editableProxy = null;
					}

					// should not this accept changes? just like end edit?
					
					// raise the AfterEdit event
					OnEditCancelled(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.None));										
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
				    foreach(var subscriber in invocationList)
				        BeforeEdit -= (XmlConfigurationElementCancelEventHandler)subscriber;
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
				    foreach(var subscriber in invocationList)
				        AfterEdit -= (XmlConfigurationElementEventHandler)subscriber;
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

		public virtual bool HasChanges
		{
			get
			{						
				return _hasChanges;
			}
			set
			{
				_hasChanges = value;
			}
		}

		public virtual void AcceptChanges() => HasChanges = false;

	    public virtual bool ApplyChanges(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			if (actions == SupportedEditingActions.None)
				return true;

			var element = editableObject as XmlConfigurationElement;			
			if (element != null)
			{
				// do we match in full paths?
				if (string.Compare(Fullpath, element.Fullpath, true) == 0)
				{
					// does the element have changes, if not we don't need to bother
					if (element.HasChanges)
					{				
						// yes so apply it's changed features
						ElementName = element.ElementName;
						Description = element.Description;
						Category = element.Category;
						DisplayName = element.DisplayName;
						Hidden = element.Hidden;
						Readonly = element.Readonly;
						Persistent = element.Persistent;
					}
					return true;
				}
			}


			return false;
		}

		public virtual bool ApplyToSelf(ISupportsEditing editableObject, SupportedEditingActions actions)
		{
			var element = editableObject as XmlConfigurationElement;			
			if (element != null)
			{
				// if this fullpath matches the element's full path, then these may apply to each other
				if (string.Compare(Fullpath, element.Fullpath, true) == 0)
				{
					if (element.HasChanges)
					{
						ElementName = element.ElementName;
						Description = element.Description;
						Category = element.Category;
						DisplayName = element.DisplayName;
						Hidden = element.Hidden;
						Readonly = element.Readonly;
						Persistent = element.Persistent;
					}
					return true;
				}
			}
			return false;
		}

		#endregion

		#region IXmlConfigurationElementEvents Members

		public event XmlConfigurationElementEventHandler Changed;

		public virtual void OnChanged(object sender, XmlConfigurationElementEventArgs e)
		{
			try
			{							
				// no events during initialization or editing
				if (_isBeingInitialized/*|| _isBeingEdited */) return;	

				// are we ourselves changing? if not then our sub options and sub categories don't change us!!!
				if (string.Compare(Fullpath, e.Element.Fullpath, true) == 0)
				{					
					_hasChanges = true;
				}											

				//EventTracing.TraceMethodAndDelegate(this, this.Changed);

				if (Changed != null)
					Changed(sender, e);

			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		public virtual void ResetChanged()
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

		public virtual void TriggerChange()
		{
			OnChanged(this, new XmlConfigurationElementEventArgs(this, XmlConfigurationElementActions.Changed));
		}
	}
}
