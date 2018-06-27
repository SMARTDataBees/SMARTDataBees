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
using System.Reflection;
using System.Text;

namespace Carbon.Configuration
{
	/// <summary>
	/// Provides methods for retrieving Enum value description and category attribute values.
	/// </summary>
	[DebuggerStepThrough]
	public static class EnumUtilities
	{
		public static string GetCombinedEnumValuesDescription(object value, Type t)
		{
			string description = null;
			try
			{
				description = value.ToString();

				var members = t.GetMembers(BindingFlags.Static | BindingFlags.Public);	

				var valuesSet = new ArrayList();
				var values = Enum.GetValues(t);
				foreach(var enumValue in values)
				{
					if (FlagsHelper.IsFlagSet((int)value, (int)enumValue))
					{
						valuesSet.Add(enumValue);
					}
				}

				var sb = new StringBuilder();
				var count = 0;
				foreach(var memberInfo in members)
				{
					foreach(var enumValue in valuesSet)
					{
						if (string.Compare(enumValue.ToString(), memberInfo.Name, false) == 0)
						{
							// get the custom attributes, specifically looking for the description attribute
							var attributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);							
							if (attributes != null)
							{									
								// who knows there may be more than one
								foreach(DescriptionAttribute attribute in attributes)
									sb.AppendFormat("{0}{1}", attribute.Description, (count > 0 ? ", " : null));												
								count++;
							}								
						}
					}
				}
				return sb.ToString();				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return description;
		}

		public static bool IsEnumFlags(Type enumType)
		{
			try
			{
				// get the custom attributes, specifically looking for the description attribute
				var attributes = enumType.GetCustomAttributes(typeof(FlagsAttribute), false);						
				if (attributes != null)
					foreach(Attribute a in attributes)
						if (a.GetType() == typeof(FlagsAttribute))
							return true;				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return false;
		}

		public static bool EnumValueHasAttribute(object value, Type enumType, Type attributeType)
		{
			try
			{
				// grab the public static members, which enum values are dynamically generated to be public static members
				var members = enumType.GetMembers(BindingFlags.Static | BindingFlags.Public);						

				// bool areFlags = EnumUtilities.IsEnumFlags(enumType);

				// loop thru the values of the enum until the correct value is found
				foreach(var memberInfo in members)
				{
//					object enumValueParsed = Enum.Parse(enumType, memberInfo.Name, true);					                    
//					bool match = (areFlags ? (((uint)enumValueParsed & (uint)value) == (uint)value) : (enumValueParsed == value));

					if (string.Compare(value.ToString(), memberInfo.Name, false) == 0)
					{
						// get the custom attributes, specifically looking for the description attribute
						var attributes = memberInfo.GetCustomAttributes(attributeType, false);						
						if (attributes != null)
							foreach(Attribute a in attributes)
								if (a.GetType() == attributeType)
                                    return true;
						return false;
					}
				}	
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return false;
		}

		public static string GetEnumValueDescription(object value, Type t)
		{
			try
			{
				// default it to the value's string representation
				var description = value.ToString();

				// grab the public static members, which enum values are dynamically generated to be public static members
				var members = t.GetMembers(BindingFlags.Static | BindingFlags.Public);		

				// loop thru the values of the enum until the correct value is found
				foreach(var memberInfo in members)
				{
					// if the name of the member matches the name of value, then we can assume we have found the correct member of the enum from which to extract the description
					if (string.Compare(value.ToString(), memberInfo.Name, false) == 0)
					{
						// get the custom attributes, specifically looking for the description attribute
						var attributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
						var sb = new StringBuilder();
					    // who knows there may be more than one
					    foreach(DescriptionAttribute attribute in attributes)
					        sb.AppendFormat("{0}", attribute.Description);
					    return sb.ToString();
					}
				}	
				return description;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}

		public static string GetEnumValueCategory(object value, Type t)
		{
			try
			{
				// default it to the value's string representation
				var description = "Misc";

				// grab the public static members, which enum values are dynamically generated to be public static members
				var members = t.GetMembers(BindingFlags.Static | BindingFlags.Public);		

				// loop thru the values of the enum until the correct value is found
				foreach(var memberInfo in members)
				{
					// if the name of the member matches the name of value, then we can assume we have found the correct member of the enum from which to extract the description
					if (string.Compare(value.ToString(), memberInfo.Name, false) == 0)
					{
						// get the custom attributes, specifically looking for the description attribute
						var attributes = memberInfo.GetCustomAttributes(typeof(CategoryAttribute), false);
						var sb = new StringBuilder();
						if (attributes != null)
						{						
							// who knows there may be more than one
							foreach(CategoryAttribute attribute in attributes)
								sb.AppendFormat("{0}", attribute.Category);												
						}	
						return sb.ToString();
					}
				}	
				return description;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}
	}
}
