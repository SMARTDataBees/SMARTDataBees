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

namespace Carbon.Configuration
{
	/// <summary>
	/// Summary description for ValuePropertyDescriptor.
	/// </summary>
	public class ValuePropertyDescriptor : PropertyDescriptor
	{
		private XmlConfigurationOption _option;

		public ValuePropertyDescriptor() : base("Value", null)
		{

		}

		public ValuePropertyDescriptor(XmlConfigurationOption option) : base("Value", null)
		{			
			_option = option;
		}

		public XmlConfigurationOption Option
		{
			get
			{
				return _option;
			}
			set
			{
				_option = value;
			}
		}

		public override Type ComponentType
		{
			get
			{
				return typeof(XmlConfigurationOption);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

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
		
		public override object GetValue(object component)
		{
			return _option.Value;
		}

		public override void SetValue(object component, object value)
		{
			_option.Value = value;
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

		public override void ResetValue(object component)
		{
			_option.Value = null;
		}

		public override bool CanResetValue(object component)
		{
			return true;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}

	}
}
