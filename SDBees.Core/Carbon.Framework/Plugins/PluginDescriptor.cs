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
using System.Drawing;
using System.IO;
using Carbon.Common;
using Carbon.Plugins.Attributes;

namespace Carbon.Plugins
{
	/// <summary>
	/// Provides a class that describes a single Plugin instance using meta-data and contains the active instance of the Plugin type.
	/// </summary>
	public sealed class PluginDescriptor : DisposableObject
	{
		private readonly Type _type;
		private readonly Type[] _dependencies;
		private Plugin _plugin;		
		private bool _isMissingDependency;
		private bool _isCircularlyDependent;
		private bool _isDependentOnTypeThatIsCircularlyDependent;
		private bool _isDependentOnTypeThatIsMissingDependency;

		/// <summary>
		/// Initializes a new instance of the PluginDescriptor class
		/// </summary>
		/// <param name="type"></param>
		public PluginDescriptor(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			_type = type;
			_dependencies = ExtractTypesThatThisTypeDependsOn(_type);
		}

		#region My Private Methods
		
		/// <summary>
		/// Extracts the Types that the specified as dependencies via attributes on the specified Type.
		/// </summary>
		/// <param name="type">The Type that contains the attributes to read</param>
		/// <returns></returns>
		private Type[] ExtractTypesThatThisTypeDependsOn(Type type)
		{
			try
			{
				var value = _type.GetCustomAttributes(typeof(PluginDependencyAttribute), false);
				if (value != null)
				{
					var types = new TypeCollection();
					foreach (PluginDependencyAttribute attribute in value)
					{
						types.Add(attribute.Type);
					}
					return types.ToArray();
				}
			}
			catch(FileNotFoundException ex)
			{
                // if this fails, we know that there is a type listed in this assembly's
                // attributes, who's containing assembly couldn't be found, so this is therefore
                // missing a dependency. 
                IsMissingDependency = true;

				Log.WriteLine("Plugin: '{0}' is missing a dependency. Additional Info: '{1}'", PluginName, ex.Message);
			}
			return new Type[] {};
		}

		#endregion

		#region My Public Properties
		
		/// <summary>
		/// Returns the plugin's Type
		/// </summary>
		public Type PluginType
		{
			get
			{
				return _type;					
			}
		}

		/// <summary>
		/// Returns the object instance implementing the IPlugin interface created by the Type described by this PluginDescriptor
		/// </summary>
		public Plugin PluginInstance
		{
			get
			{
				return _plugin;
			}
		}

		/// <summary>
		/// Returns an array of strings that contain the names of the plugin's authors
		/// </summary>
		public string[] PluginAuthors
		{
			get
			{
				var value = _type.GetCustomAttributes(typeof(PluginAuthorsAttribute), false);
				if (value != null)
				{
					return ((PluginAuthorsAttribute)value[0]).Authors;
				}
				return new[] {string.Empty};
			}
		}

		/// <summary>
		/// Returns an array of Types that define external Plugin Types that this Plugin Type depends upon
		/// </summary>
		public Type[] PluginDependencies
		{
			get
			{
				return _dependencies;
			}
		}

		/// <summary>
		/// Returns the plugin's description
		/// </summary>
		public string PluginDescription
		{
			get
			{
				var value = _type.GetCustomAttributes(typeof(PluginDescriptionAttribute), false);
				if (value != null)
				{
					return ((PluginDescriptionAttribute)value[0]).Description;
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// Returns the plugin's Id
		/// </summary>
		public string PluginId
		{
			get
			{
				var value = _type.GetCustomAttributes(typeof(PluginIdAttribute), false);
				if (value != null)
				{
					return ((PluginIdAttribute)value[0]).Id;
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// Returns the plugin's image. (16 by 16 bitmap)
		/// </summary>
		public Image PluginImage
		{
			get
			{
				var value = _type.GetCustomAttributes(typeof(PluginImageAttribute), false);
				if (value != null)
				{
					return ((PluginImageAttribute)value[0]).GetImage(_type);
				}
				return null;
			}
		}

		/// <summary>
		/// Returns the plugin's manufacturer
		/// </summary>
		public string PluginManufacturer
		{
			get
			{
				var value = _type.GetCustomAttributes(typeof(PluginManufacturerAttribute), false);
				if (value != null)
				{
					return ((PluginManufacturerAttribute)value[0]).Manufacturer;
				}
				return string.Empty;
			}
		}

		/// <summary>
		/// Returns the plugin's name
		/// </summary>
		public string PluginName
		{
			get
			{
				var value = _type.GetCustomAttributes(typeof(PluginNameAttribute), false);
				if (value != null)
				{
					return ((PluginNameAttribute)value[0]).Name;
				}
				return _type.FullName;
			}
		}

		/// <summary>
		/// Returns the plugin's version
		/// </summary>
		public Version PluginVersion
		{
			get
			{
				var value = _type.GetCustomAttributes(typeof(PluginVersionAttribute), false);
				if (value != null)
				{
					return ((PluginVersionAttribute)value[0]).Version;
				}
				return new Version("0.0.0");
			}
		}

        public override string ToString()
        {
            return PluginName;
        }

		#endregion

		#region My Internal Methods

		/// <summary>
		/// Attaches a live instance of a class implementing the IPlugin interface to this PluginDescriptor object.
		/// </summary>
		/// <param name="plugin">The IPlugin interface that is described by this descriptor</param>
		internal void AttachPluginInstance(Plugin plugin)
		{
			_plugin = plugin;
		}

		/// <summary>
		/// Determines if this Plugin depends upon the PluginType described by the specified PluginDescriptor.
		/// </summary>
		/// <param name="descriptor">The PluginDescriptor to check against this PluginDescriptor's dependencies</param>
		/// <returns>true if this descriptor depends on the specified descriptor's Type</returns>
		internal bool DependsOn(PluginDescriptor descriptor)
		{			
			foreach(var type in PluginDependencies)
				if (type != Type.Missing && type == descriptor.PluginType)
					return true;
			return false;
		}

		#endregion

		#region My Internal Properties

		/// <summary>
		/// Gets or sets a flag that indicates if the Plugin is missing a Plugin dependency.
		/// </summary>
		internal bool IsMissingDependency
		{
			get
			{
				return _isMissingDependency;
			}
			set
			{
				_isMissingDependency = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the Plugin is circularly dependent with another Plugin
		/// </summary>
		internal bool IsCircularlyDependent
		{
			get
			{
				return _isCircularlyDependent;
			}
			set
			{
				_isCircularlyDependent = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the Plugin is dependent on a Plugin that is circularly dependent
		/// </summary>
		internal bool IsDependentOnTypeThatIsCircularlyDependent
		{
			get
			{
				return _isDependentOnTypeThatIsCircularlyDependent;
			}
			set
			{
				_isDependentOnTypeThatIsCircularlyDependent = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates if the Plugin is dependent on a Plugin that is missing a dependency
		/// </summary>
		internal bool IsDependentOnTypeThatIsMissingDependency
		{
			get
			{
				return _isDependentOnTypeThatIsMissingDependency;
			}
			set
			{
				_isDependentOnTypeThatIsMissingDependency = value;
			}
		}

		/// <summary>
		/// Returns a flag that indicates if the Plugin looks Ok to start.
		/// This flag is calculated by checking the Plugin's dependencies, and determining if it should be Ok to start,
		/// or whether it is impossible to start it because of a dependency problem.
		/// </summary>
		internal bool IsStartable
		{
			get
			{
                return !_isMissingDependency && !_isCircularlyDependent && !_isDependentOnTypeThatIsCircularlyDependent && !_isDependentOnTypeThatIsMissingDependency;
			}
		}

		#endregion

		/// <summary>
		/// Sorts an array of PluginDescriptors from least dependent first to most dependent last, or vice versa.
		/// </summary>
		/// <param name="descriptors">The array of descriptors to sort</param>
		/// <param name="leastDependentFirst">The order in which to sort them</param>
		/// <returns></returns>
		public static bool Sort(PluginDescriptor[] descriptors, bool leastDependentFirst)
		{
			try
			{
				// front to back - 1 
				for (var i = 0; i < descriptors.Length - 1; i++)
				{
					// front + 1 to back
					for (var j = i + 1; j < descriptors.Length; j++)
					{
						var dependsOn = descriptors[i].DependsOn(descriptors[j]);
						if ((leastDependentFirst ? dependsOn : !dependsOn))
						{
							// swap i with j, where i=1 and j=2
							var descriptor = descriptors[j];
							descriptors[j] = descriptors[i];
							descriptors[i] = descriptor;
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Log.WriteLine(ex);
			}
			return false;
		}
    }
}
