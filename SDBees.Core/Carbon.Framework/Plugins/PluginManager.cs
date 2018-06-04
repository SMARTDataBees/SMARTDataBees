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

using Carbon.Common;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using Carbon.Plugins.Providers;
using Carbon.UI;

namespace Carbon.Plugins
{
	/// <summary>
	/// Provides methods for managing Plugins.
	/// </summary>
//	[System.Diagnostics.DebuggerStepThrough()]
	internal static class PluginManager
	{		
		#region My Public Static Methods

		/// <summary>
		/// Uses the PluginProviders specified to load Plugin Types that will be used to create Plugins.
		/// </summary>
		/// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
		/// <param name="pluginProviders">The collection of PluginProviders that will be used to load the Plugin Types from their various sources.</param>
		/// <returns></returns>
		public static TypeCollection LoadPluginTypes(IProgressViewer progressViewer, PluginProviderCollection pluginProviders)
		{
			TypeCollection pluginTypes = new TypeCollection();
			foreach (PluginProvider provider in pluginProviders)
			{
				try
				{
					Log.WriteLine("Loading Plugin Types. PluginProvider: '{0}'.", provider.Name);

					TypeCollection types = provider.LoadPluginTypes(progressViewer);
					if (types != null)
					{
						pluginTypes.AddRange(types);
					}
				}
				catch(Exception ex)
				{
					Log.WriteLine(ex);
				}
			}
			return pluginTypes;
		}

		/// <summary>
		/// Sorts the collection of PluginDescriptors according to their dependency chain.
		/// </summary>
		/// <param name="descriptorCollection">The collection of descriptors to sort.</param>
		/// <param name="leastDependentFirst">A flag that determines how the descriptors are sorted.</param>
		/// <returns></returns>
		public static PluginDescriptorCollection Sort(PluginDescriptorCollection descriptorCollection, bool leastDependentFirst)
		{
			Log.WriteLine("Sorting PluginDescriptor Collection. LeastDependentFirst: '{0}'.", leastDependentFirst.ToString());

			PluginDescriptor[] descriptors = descriptorCollection.ToArray();
			PluginDescriptor.Sort(descriptors, leastDependentFirst);

			descriptorCollection.Clear();
			descriptorCollection.Add(descriptors);

			return descriptorCollection;
		}

		/// <summary>
		/// Creates PluginDescriptors from each Plugin Type specified.
		/// </summary>
		/// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
		/// <param name="types">The collection of Plugin Types to create descriptors for.</param>
		/// <returns></returns>
		public static PluginDescriptorCollection CreatePluginDescriptors(IProgressViewer progressViewer, TypeCollection types)
		{
			PluginDescriptorCollection descriptors = new PluginDescriptorCollection();
			foreach (Type type in types)
			{
				try
				{
					string message = string.Format("Creating PluginDescriptor, Type: '{0}'.", type.FullName);
                    ProgressViewer.SetExtendedDescription(progressViewer, message);
					Log.WriteLine(message);

                    PluginDescriptor descriptor = new PluginDescriptor(type);

                    descriptors.Add(descriptor);
				}
				catch(Exception ex)
				{
					Log.WriteLine(ex);
				}
			}
			return descriptors;
		}

		/// <summary>
		/// Validates the dependencies for each of the PluginDescriptors
		/// </summary>
		/// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
		/// <param name="descriptors">The collection of PluginDescriptors that describe the Plugins to be loaded.</param>
		public static void ValidatePluginDependencies(IProgressViewer progressViewer, PluginDescriptorCollection descriptors)
		{			
			/*
			 * Validation Phases
			 * Phase 1: Direct		(First level dependencies)
			 * Phase 2: Indirect	(Second level dependencies. i.e., dependencies of dependencies. Requires that Phase 1 already executed)
			 * Phase 3: Extended	(Provider Validation)
			 * */

			// Phase 1: Checks descriptors for missing dependencies and circular references. (direct)
			foreach (PluginDescriptor descriptor in descriptors)
			{
				try
				{
					// check for missing dependencies
//					MarkDescriptorIfMissingDependency(descriptor, descriptors);
					if (!descriptor.IsMissingDependency)
					{
						// check for circular references between plugins (direct, does not check dependency chains)
						MarkDescriptorIfCircularlyDependent(descriptor, descriptors);						
					}
				}
				catch(Exception ex)
				{
					Log.WriteLine(ex);
				}
			}

			// Phase 2: Checks depencencies for missing dependencies and circular references. (indirect)
			foreach (PluginDescriptor descriptor in descriptors)
			{
				try
				{
					// 
					if (!descriptor.IsMissingDependency && !descriptor.IsCircularlyDependent)
					{
						MarkDescriptorIfDependenciesAreMissingDependencyOrAreCircularlyDependent(descriptor, descriptors);
					}					
				}
				catch(Exception ex)
				{
					Log.WriteLine(ex);
				}
			}

			// Phase 3: Allow for Provider based validation?	
		
			/*
			 * Here we have an extension point. 
			 * If we created another provider who's sole purpose was to validate a PluginDescriptor,
			 * we could move this logic away from the core, and allow for validation to be extended.
			 * Possible reasons for doing this would be to prevent Plugins from being loaded based 
			 * on some other criteria. We could provide descriptions of why a particular descriptor failed validation.
			 * */
		}

        /// <summary>
        /// Creates instances of the Plugin type defined by each PluginDescriptor.
        /// </summary>
        /// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
        /// <param name="descriptors">The collection of PluginDescriptors that describe the Plugins to be loaded.</param>
        public static void CreatePluginInstances(IProgressViewer progressViewer, PluginDescriptorCollection descriptors)
        {
			Log.WriteLine("Creating Plugins. # of Plugins: '{0}'.", descriptors.Count.ToString());

			foreach (PluginDescriptor descriptor in descriptors)
			{
				if (descriptor.IsStartable)
				{
					if (AreDependenciesCreated(descriptor, descriptors))
					{
						CreatePluginInstance(progressViewer, descriptor);
					}
				}
			}
        }
        
        /// <summary>
        /// Starts the plugins defined in the collection that have been created.
        /// </summary>
        /// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
        /// <param name="descriptors">The collection of PluginDescriptors that describe the Plugins to be loaded.</param>
        public static void StartPlugins(IProgressViewer progressViewer, PluginDescriptorCollection descriptors)
        {
			Log.WriteLine("Starting Plugins. # of Plugins: '{0}'.", descriptors.Count.ToString());

            // start all of the plugins
			foreach (PluginDescriptor descriptor in descriptors)
			{
				if (descriptor.PluginInstance != null)
				{
					StartPlugin(progressViewer, descriptor);
				}
				else
				{
					Log.WriteLine(string.Format("Skipped Plugin: '{0}' was not created.", descriptor.PluginName));
				}
			}
            
          // fire the AfterPluginsStarted event of the PluginContext 
			    PluginContext.Current.OnAfterPluginsStarted(new PluginContextEventArgs(PluginContext.Current));			
        }

        /// <summary>
        /// Stops the plugins defined in the collection that have been created.
        /// </summary>
        /// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
        /// <param name="descriptors">The collection of PluginDescriptors that describe the Plugins to be loaded.</param>
        public static void StopPlugins(IProgressViewer progressViewer, PluginDescriptorCollection descriptors)
        {
			Log.WriteLine("Stopping Plugins. # of Plugins: '{0}'.", descriptors.Count.ToString());

            // fire the BeforePluginsStopped event of the PluginContext
			PluginContext.Current.OnBeforePluginsStopped(new PluginContextEventArgs(PluginContext.Current));
			
            // stop all of the plugins
			foreach (PluginDescriptor descriptor in descriptors)
			{
				if (descriptor.PluginInstance != null)
				{
					StopPlugin(progressViewer, descriptor);
				}
				else
				{
					Log.WriteLine(string.Format("Skipped Plugin: '{0}' was not created.", descriptor.PluginName));
				}
			}
        }

		#endregion

		#region My Private Static Methods

//		/// <summary>
//		/// Marks the descriptor if it is missing a dependency
//		/// </summary>
//		/// <param name="descriptor">The descriptor to check</param>
//		/// <param name="descriptors">The collection of PluginDescriptors to check against</param>
//		private static void MarkDescriptorIfMissingDependency(PluginDescriptor descriptor, PluginDescriptorCollection descriptors)
//		{
//            // we may already know that 
//            if (descriptor.IsMissingDependency)
//                return;
//
//			// check each Type the Plugin depends upon to determine if it is missing
//			foreach(Type type in descriptor.PluginDependencies)
//			{
//				// if the dependency Type is equivalent to Missing, then there is an Assembly reference missing
//				if ((object)type == Type.Missing)
//				{
//					// we know the plugin can't be started now so we're done with this one
//					descriptor.IsMissingDependency = true;
//					return;
//				}
//
//				// check each Type the Plugin depends upon to determine if the Type is listed somewhere in the other descriptors
//				// it is possible the Type reference could be valid because the correct Assembly is loaded, but the Plugin that is
//				// required isn't being exported as a Plugin yet. Sometimes the developers forget to export their plugins.
//				bool found = false;
//				foreach(PluginDescriptor otherDescriptor in descriptors)
//				{
//					// if some other descriptor's PluginType is the Type of this dependency
//					if (otherDescriptor.PluginType == type)
//					{
//						// and we aren't comparing the same descriptor
//						if (otherDescriptor != descriptor)
//						{
//							// then this particular dependency is exported and is in the list of descriptors 
//							found = true;
//							break;
//						}
//					}					
//				}
//				
//				// if the Type wasn't found in the list of descriptors then this descriptor's Plugin is missing a dependency
//				if (!found)
//				{
//					// we know the plugin can't be started now so we're done with this one
//					descriptor.IsMissingDependency = true;
//					return;
//				}
//			}
//		}

		/// <summary>
		/// Marks a descriptor if is is circularly dependent with any other descriptor
		/// </summary>
		/// <param name="descriptor">The descriptor to check</param>
		/// <param name="descriptors">The collection of PluginDescriptors to check against</param>
		private static void MarkDescriptorIfCircularlyDependent(PluginDescriptor descriptor, PluginDescriptorCollection descriptors)
		{
			// check each dependency in that descriptor depends on
			foreach(Type type in descriptor.PluginDependencies)
			{
				// against all the other descriptors
				foreach(PluginDescriptor otherDescriptor in descriptors)
				{
					// when we find a descriptor that describes the Type the first descriptor needs
					if (otherDescriptor.PluginType == type)
					{
						// it better not depend on the first
						if (otherDescriptor.DependsOn(descriptor))
						{
							// if it does, it's a circular dependency which we cannot have
							descriptor.IsCircularlyDependent = true;
							return;
						}
					}
				}
			}			
		}

		/// <summary>
		/// Marks a descriptor if it has dependencies that themselves are missing dependencies or are circularly dependent
		/// </summary>
		/// <param name="descriptor">The descriptor to check</param>
		/// <param name="descriptors">The collection of PluginDescriptors to check against</param>
		private static void MarkDescriptorIfDependenciesAreMissingDependencyOrAreCircularlyDependent(PluginDescriptor descriptor, PluginDescriptorCollection descriptors)
		{
			// check each dependency in that descriptor depends on
			foreach(Type type in descriptor.PluginDependencies)
			{
				// against all the other descriptors
				foreach(PluginDescriptor otherDescriptor in descriptors)
				{
					// when we find a descriptor that describes the Type the first descriptor needs
					if (otherDescriptor.PluginType == type)
					{
						// the other dependency better not be missing a dependency
						if (otherDescriptor.IsMissingDependency)
						{
							// if it does, the whole chain is jacked 
							descriptor.IsDependentOnTypeThatIsMissingDependency = true;
							return;
						}
						
						// the other dependency better not be circularly dependent
						if (otherDescriptor.IsCircularlyDependent)
						{
							// if it does, the whole chain is jacked 
							descriptor.IsDependentOnTypeThatIsCircularlyDependent = true;
							return;
						}
					}
				}
			}
		}
        
        /// <summary>
        /// Determines if the dependencies for a PluginDescriptor are created.
        /// </summary>
        /// <param name="descriptor">The descriptor to check dependencies for.</param>
        /// <param name="descriptors">The collection of PluginDescriptor(s) to check against.</param>
        /// <returns></returns>
        private static bool AreDependenciesCreated(PluginDescriptor descriptor, PluginDescriptorCollection descriptors)
        {
            foreach (Type type in descriptor.PluginDependencies)
                if (descriptors[type].PluginInstance == null)
                    return false;            
            return true;
        }

        /// <summary>
        /// Creates an instance of the Type described by the PluginDescriptor and asserts that it derives from Plugin.
        /// </summary>
        /// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
        /// <param name="descriptor">The PluginDescriptor that contains the Type to create.</param>
        private static void CreatePluginInstance(IProgressViewer progressViewer, PluginDescriptor descriptor)
        {            
            try
            {
                TypeUtilities.AssertTypeIsSubclassOfBaseType(descriptor.PluginType, typeof(Plugin));

				string message = string.Format("Creating Plugin: '{0}'.", descriptor.PluginName);
				ProgressViewer.SetExtendedDescription(progressViewer, message);
				Log.WriteLine(message);

                Plugin plugin = (Plugin)TypeUtilities.CreateInstanceOfType(descriptor.PluginType, Type.EmptyTypes, new object[] {});
                
                descriptor.AttachPluginInstance(plugin);
            }
            catch(Exception ex)
            {
                Log.WriteLine(ex);
            }
        }

        /// <summary>
        /// Starts the specified plugin.
        /// </summary>
        /// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
        /// <param name="descriptor">The descriptor that contains the plugin to start.</param>
        private static void StartPlugin(IProgressViewer progressViewer, PluginDescriptor descriptor)
        {			
            ProgressViewer.SetExtendedDescription(progressViewer, string.Format("Starting Plugin: '{0}'.", descriptor.PluginName));

            // start the plugin
            descriptor.PluginInstance.OnStart(PluginContext.Current, new PluginDescriptorEventArgs(descriptor));			
        }

        /// <summary>
        /// Stops the specified plugin.
        /// </summary>
        /// <param name="progressViewer">The callback object implementing IProgressViewer that will be used to monitor progress.</param>
        /// <param name="descriptor">The descriptor that contains the plugin to stop.</param>
        private static void StopPlugin(IProgressViewer progressViewer, PluginDescriptor descriptor)
        {
			ProgressViewer.SetExtendedDescription(progressViewer, string.Format("Stopping Plugin: '{0}'.", descriptor.PluginName));

            // stop the plugin
            descriptor.PluginInstance.OnStop(PluginContext.Current, new PluginDescriptorEventArgs(descriptor));
        }

		#endregion
	}
}
