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
using System.Reflection;

namespace Carbon.Configuration
{
	/// <summary>
	/// Provides a means of standardized cloning. 
	/// The object being cloned must contain a parameterless constructor.
	/// Also if the object is a derived class, only internal or protected fields in the base class will be set during cloning.
	/// That of course assumes the DefaultBindingFlags are used.
	/// </summary>
	public class CloningEngine
	{
		/// <summary>
		/// The default binding flags that will be used 99% of the time for object cloning. The flags include BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
		/// </summary>
		public static BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		/// <summary>
		/// Uses reflection to clone the object instance specified, and sets the fields from the instance on the clone using the binding flags specified.
		/// </summary>
		/// <param name="instance">The object instance to clone</param>
		/// <param name="bindingFlags">The binding flags that specify which fields will be set on the clone</param>
		/// <returns></returns>
		public static object Clone(object instance, BindingFlags bindingFlags)
		{						
			try
			{				
				Type t = instance.GetType();
				if (t != null)
				{
					FieldInfo[] fields = t.GetFields(bindingFlags);
					if (fields != null)
					{
						ConstructorInfo ci = t.GetConstructor(Type.EmptyTypes);
						if (ci != null)
						{
							object clone = ci.Invoke(null);
							if (clone != null)
							{
								foreach(FieldInfo fi in fields)
								{
									try
									{
//  									System.Diagnostics.Debug.WriteLine(fi.Name.ToString());
										fi.SetValue(clone, fi.GetValue(instance));
									}
									catch(Exception ex)
									{
										Debug.WriteLine(ex);
									}
								}
								return clone;
							}														
						}
					}					
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}
	}
}
