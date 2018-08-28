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
using System.Reflection;

namespace Carbon.Configuration
{
	/// <summary>
	/// Defines a PropertyDescriptor that describes an XmlConfigurationOption. To be used at runtime by the XmlConfigurationPropertiesWindow for displaying an XmlConfigurationOption.
	/// </summary>
	public class XmlConfigurationOptionPropertyDescriptor: PropertyDescriptor
	{
		private XmlConfigurationOption _option;

		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOptionPropertyDescriptor class
		/// </summary>
		/// <param name="option">The option to describe</param>
		public XmlConfigurationOptionPropertyDescriptor(XmlConfigurationOption option) : base(option.DisplayName, null)
		{			
			_option = option;
		}

		/// <summary>
		/// Gets the XmlConfigurationOption that is described by this PropertyDescriptor
		/// </summary>
		public XmlConfigurationOption Option
		{
			get
			{
				return _option;
			}
		}

//		public override TypeConverter Converter
//		{
//			get
//			{
//				return new XmlConfigurationOptionTypeConverter();
//			}
//		}
		 
		public override string DisplayName
		{
			get
			{
				return _option.DisplayName;
			}
		}

		public override string Category
		{
			get
			{
				return _option.Category;
			}
		}

		public override string Description
		{
			get
			{
				return _option.Description;
			}
		}

		/// <summary>
		/// Gets the type of component that this PropertyDescriptor is desribing
		/// </summary>
		public override Type ComponentType
		{
			get
			{
				return typeof(XmlConfigurationOption);
			}
		}

		/// <summary>
		/// Gets a flag that indicates whether this option is readonly
		/// </summary>
		public override bool IsReadOnly
		{
			get
			{
				return _option.Readonly;
			}
		}

		public override object GetEditor(Type editorBaseType)
		{
			try
			{
				if (_option.EditorAssemblyQualifiedName != null && _option.EditorAssemblyQualifiedName != string.Empty)
				{
					var t = Type.GetType(_option.EditorAssemblyQualifiedName);
					if (t != null)
					{										
						var ci = t.GetConstructor(Type.EmptyTypes);
						var editor = ci.Invoke(null);
						if (editor != null)
							return editor;												
					}					
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}

			return base.GetEditor (editorBaseType);
		}

        public override TypeConverter Converter
        {
            get
            {
                try
                {
                    if (_option.TypeConverterAssemblyQualifiedName != null && _option.TypeConverterAssemblyQualifiedName != string.Empty)
                    {
                        var t = Type.GetType(_option.TypeConverterAssemblyQualifiedName);
                        if (t != null)
                        {
                            var ci = t.GetConstructor(Type.EmptyTypes);
                            var converter = ci.Invoke(null);
                            if (converter != null)
                                return (TypeConverter)converter;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return base.Converter;
            }
        }

        /// <summary>
        /// Gets the type that should be displayed in the PropertyGrid, determined from the type contained in the Value property of the XmlConfigurationOption instance.
        /// </summary>
        public override Type PropertyType
		{
			get
			{
                var t = XmlConfigurationOptionTypeUtilities.GetType(_option);
				if (t != null)
					return t;

				return Type.Missing as Type;
			}
		}
		
		/// <summary>
		/// Gets the value of the option
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override object GetValue(object component)
		{
			if (component != null)
			{
				var t = component.GetType();
				if (t != null)
				{
//					System.Diagnostics.Debug.WriteLine("GetValue('" + t.FullName + "' EditMode=" + _option.IsBeingEdited.ToString() + ")");
					
					if (t == typeof(XmlConfigurationOption))
					{
						return _option.Value;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Sets the value of the option
		/// </summary>
		/// <param name="component"></param>
		/// <param name="value"></param>
		public override void SetValue(object component, object value)
		{
			if (component != null)
			{
				var t = component.GetType();
				if (t != null)
				{
//					System.Diagnostics.Debug.WriteLine("\tSetValue('" + t.FullName + "' EditMode=" + _option.IsBeingEdited.ToString() + ")");
					if (t == typeof(XmlConfigurationOption))
					{
						_option.Value = value;
						//						this.OnValueChanged(_option, System.EventArgs.Empty);
					}
				}
			}

			//			XmlConfigurationOption option = component as XmlConfigurationOption;
			//			IComponentChangeService changeSvc = null;
			//			PropertyDescriptor prop = null;
			//			object oldValue = option.Value;
			//
			//			if (option != null && option.Site != null)
			//			{
			//				changeSvc = (IComponentChangeService)option.Site.GetService(typeof(IComponentChangeService));
			//				if (changeSvc != null)
			//				{
			//					prop = TypeDescriptor.GetProperties(option)["Value"];
			//					try
			//					{
			//						changeSvc.OnComponentChanging(option, prop);
			//					}
			//					catch(Exception ex)
			//					{
			//						Debug.WriteLine(ex);
			//						return;
			//					}
			//				}
			//			}
			//
			//			option.Value = value;
			//			if (changeSvc != null)
			//			{
			//				changeSvc.OnComponentChanged(option, prop, oldValue, value); 
			//			}
		}

		/// <summary>
		/// Indicates the option's value should be reset
		/// </summary>
		/// <param name="component"></param>
		public override void ResetValue(object component)
		{
//			_option.Value = null;
		}

		/// <summary>
		/// Inicates whether the option's value can be reset
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override bool CanResetValue(object component)
		{
			return false;
		}

		/// <summary>
		/// Indicates whether the option's value should be serialized
		/// </summary>
		/// <param name="component"></param>
		/// <returns></returns>
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
