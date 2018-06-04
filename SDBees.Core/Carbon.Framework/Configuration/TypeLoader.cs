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
using System.IO;

namespace Carbon.Configuration
{
	/// <summary>
	/// Summary description for XmlConfigurationOptionTypeUtilities.
	/// </summary>
	[Serializable()]
	internal sealed class XmlConfigurationOptionTypeUtilities
	{
		public XmlConfigurationOptionTypeUtilities()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Loads a Type specified by it's name, using the specified assembly name as the source
		/// </summary>
		/// <param name="typename">The name of the type (may be full or partial)</param>
		/// <param name="assemblyname">The name of the assembly (may be full or partial, may include the entire path or not)</param>
		/// <returns></returns>
		public Type LoadType(string typename, string assemblyname)
		{
			try
			{
				Type t = null;
				Assembly a = this.LoadAssembly(assemblyname);
				if (a != null)
				{
					t = a.GetType(typename, false, true);					
					if (t != null)
						return t;
				}			
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}

		/// <summary>
		/// Loads the specified assembly into the current appdomain
		/// </summary>
		/// <param name="assemblyname">The name of the assembly (may be full or partial, may include the entire path or not)</param>
		/// <returns></returns>
		public Assembly LoadAssembly(string assemblyname)
		{			
			Assembly assembly = null;
			try
			{
				FileInfo file = new FileInfo(assemblyname);
                //if (!file.Exists)
                //    assembly = Assembly.LoadWithPartialName(assemblyname);				
                //else
					assembly = Assembly.LoadFrom(assemblyname);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}			
			return assembly;
		}

		public static Type LoadTypeInSeparateAppDomain(string typename, string assemblyname)
		{
			try
			{
				/// first try loading the type the easy way
				Type t = Type.GetType(typename, false, true);
				if (t == null)
				{
					/// it didn't load, so try loading the reference and then finding it
					/// but do it in a separate app domain so that we don't incur the wrath of the overhead monkey
					AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());
					XmlConfigurationOptionTypeUtilities loader = (XmlConfigurationOptionTypeUtilities)domain.CreateInstanceFromAndUnwrap(System.Reflection.Assembly.GetExecutingAssembly().Location, typeof(XmlConfigurationOptionTypeUtilities).FullName);
					t = loader.LoadType(typename, assemblyname);
					AppDomain.Unload(domain);					
				}
				return t;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}

		public static Type GetType(XmlConfigurationOption option)
		{		
			try
			{
				Type t = null;
				if (option.ValueAssemblyQualifiedName != null)
				{			
					t = Type.GetType(option.ValueAssemblyQualifiedName, false, true);
					if (t != null)
						return t;					
				}

				object value = option.Value;
				if ((value != null) && (((string)value) != string.Empty))
				{
					t = value.GetType();
					if (t != null)
						return t;
				}
			}
			catch(Exception ex)
			{
                Debug.WriteLine(ex);
			}

            // this is questionable. Mark -12/4/05. I believe that will error out. Apparently it never has been hit.
            return Type.Missing as Type; 
		}	

	}
}
