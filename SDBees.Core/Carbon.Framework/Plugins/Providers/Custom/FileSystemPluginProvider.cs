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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Carbon.Common;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using Carbon.UI;

namespace Carbon.Plugins.Providers.Custom
{
	/// <summary>
	/// Provides a PluginProvider that loads plugins from the application's startup path.
	/// </summary>
	public sealed class FileSystemPluginProvider : PluginProvider
	{
		#region TypeLoader

		/// <summary>
		/// Provides a class that can load Types from assemblies in the application's startup path.
		/// </summary>
		internal sealed class TypeLoader : MarshalByRefObject
		{	
			/// <summary>
			/// 
			/// </summary>
			public TypeLoader() {}

			/// <summary>
			/// Searches for plugins in the application's startup path
			/// </summary>
			/// <param name="viewer"></param>
			/// <returns>null if no plugins were found</returns>
			private TypeCollection InternalSearchForPluginTypes(IProgressViewer progressViewer)
			{				
				TypeCollection types = null;

                try
                {
                    // starting in the startup path
                    DirectoryInfo directoryInfo = new DirectoryInfo(Application.StartupPath);

                    // look for all the dlls
                    FileInfo[] files = directoryInfo.GetFiles("*.dll", SearchOption.AllDirectories);

                    // see if we can find any plugins defined in each assembly
                    foreach (FileInfo file in files)
                    {
                        // try and load the assembly
                        Assembly assembly = this.LoadAssembly(file.FullName);
                        if (assembly != null)
                        {
                            ProgressViewer.SetExtendedDescription(progressViewer, string.Format("Searching for plugins. Searching '{0}'...", assembly.GetName().Name));

                            // see if the assembly has any plugins defined in it
                            TypeCollection typesInAssembly = this.LoadPluginTypesFromAssembly(assembly);
                            if (typesInAssembly != null)
                            {
                                if (types == null)
                                    types = new TypeCollection();

                                // add the types defined as plugins to the master list
                                types.AddRange(typesInAssembly);
                            }
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                }

				return types;
			}

			/// <summary>
			/// Loads an Assembly from the specified filename
			/// </summary>
			/// <param name="filename">The name of the file to load as an assembly</param>
			/// <returns>null if the file is not a valid .NET assembly</returns>
			private Assembly LoadAssembly(string filename)
			{
				Assembly assembly = null;
				try
				{	
				    if(File.Exists(filename))
				    	assembly = Assembly.LoadFrom(filename);
				}
				catch(BadImageFormatException)
				{
					/*
					 * HACK: Normally you would never eat an exception, however 
					 * unmanaged dlls may be included in our results which will
					 * throw this exception indicating that the dll is not a 
					 * .NET assembly, in which case it will not contain plugins
					 * and is probably just some external dll that our app 
					 * references. In this case just ignore the exception.
					 * */

					/*
					 * SIDENOTE: I really wish there was a way to easily specify a different
					 * extension for the plugins, because then we wouldn't have to 
					 * scan for dlls, and potentially try and load dlls that aren't
					 * assemblies. Unfortunately it's a real pain to do so with build
					 * events, and the pdb files get jacked up when they are supposed
					 * to contain info for some dll that is no longer there. 
					 * */
				}
				catch(Exception ex)
				{
					Log.WriteLine(ex);					
				}
				return assembly;
			}

			/// <summary>
			/// Loads a TypeCollection with the plugins defined in the assembly
			/// </summary>
			/// <param name="assembly">The assembly to check for plugin definitions</param>
			/// <returns></returns>
			private TypeCollection LoadPluginTypesFromAssembly(Assembly assembly)
			{                
				TypeCollection types = null;
                
                try
                {
                    object[] value = assembly.GetCustomAttributes(typeof(PluginDefinitionAttribute), false);
    				
                    if (value != null)
                    {
                        PluginDefinitionAttribute[] attributes = (PluginDefinitionAttribute[])value;
    					
                        foreach (PluginDefinitionAttribute attribute in attributes)
                        {
                            if (types == null)
                                types = new TypeCollection();

                            types.Add(attribute.Type);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.WriteLine(ex);
                }

				return types;
			}

			/// <summary>
			/// Searches for plugin types from assemblies in the application's startup path in a second AppDomain
			/// </summary>
			/// <param name="viewer"></param>
			/// <returns></returns>
			internal static TypeCollection SearchForPluginTypes(IProgressViewer progressViewer)
			{
                TypeCollection types = null;

                try
                {
                    // create a new appdomain where we'll try and load the plugins
                    AppDomain domain = AppDomain.CreateDomain(Guid.NewGuid().ToString());

                    // create an instance of the plugin loader in the new appdomain
                    TypeLoader loader = (TypeLoader)domain.CreateInstanceFromAndUnwrap(
                        Assembly.GetExecutingAssembly().Location,
                        typeof(TypeLoader).FullName);

                    // use the loader to search for plugins inside the second appdomain
                    types = loader.InternalSearchForPluginTypes(progressViewer);

                    // unload the appdomain
                    AppDomain.Unload(domain);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(ex.Message);
                }				

				// return the plugin descriptors that were found
				return types;
			}
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the FileSystemPluginProvider class
		/// </summary>
		/// <param name="name">The name of the provider</param>
		public FileSystemPluginProvider(string name) : base(name)
		{

		}

		/// <summary>
		/// Loads the plugin types this provider brings to the system.
		/// </summary>
		/// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
		/// <returns></returns>
		public override TypeCollection LoadPluginTypes(IProgressViewer progressViewer)
		{
            TypeCollection types = null;

            try
            {
                ProgressViewer.SetExtendedDescription(progressViewer, "Search for plugins...");

                types = TypeLoader.SearchForPluginTypes(progressViewer);
            }
            catch (Exception ex)
            {

            }
            
            return types;
		}
	}	
}
