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

using Carbon.Common;
using Carbon.Common.Attributes;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Provides a class that describes a product for the purposes of automatic updates.
	/// </summary>
	public sealed class AutoUpdateProductDescriptor
	{
		private string _name;
		private Version _version;
		private bool _requiresRegistration;
		private string _id;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateProductDescriptor class
		/// </summary>
		public AutoUpdateProductDescriptor()
		{

		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateProductDescriptor class
		/// </summary>
		/// <param name="name">The name of the product.</param>
		/// <param name="version">The version of the product.</param>
		public AutoUpdateProductDescriptor(string name, Version version)
		{
			_name = name;
			_version = version;				
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateProductDescriptor class
		/// </summary>
		/// <param name="name">The name of the product.</param>
		/// <param name="version">The version of the product.</param>
		/// <param name="requiresRegistration">A flag that indicates whether the product requires registration.</param>
		/// <param name="id">The unique identifier assigned to the product.</param>
		public AutoUpdateProductDescriptor(string name, Version version, bool requiresRegistration, string id) : this(name, version)
		{
			_requiresRegistration = requiresRegistration;
			_id = id;
		}

		/// <summary>
		/// Creates a descriptor that describes the product from it's assembly and version.
		/// </summary>
		/// <param name="assembly"></param>
		/// <param name="version"></param>
		/// <returns></returns>
		public static AutoUpdateProductDescriptor FromAssembly(Assembly assembly, Version version)
		{			
			// create a product descriptor
			AutoUpdateProductDescriptor productDescriptor = new AutoUpdateProductDescriptor();				
						
			// grab its assembly name
			AssemblyName assemblyName = assembly.GetName();

			// set the name of the product
			productDescriptor.Name = assemblyName.Name.Replace(".exe", null);
			
			// the version will be the starting folder name parsed to a version
			// this is based on the bootstrap's ability to look at folders and determine the appropriate
			// version. we view the versions of the product as a collection of assemblies as a whole, not 
			// just individual assemblies. we do this to keep compiliation references together, we compile 
			// for releases of versions, and therefore when one changes, we view that single changes as applying to a 
			// particular update, which taken in context represents a version of the product.
			productDescriptor.Version = version;
			
			// snag the product id attribute from the assembly
			ProductIdentifierAttribute pia = ProductIdentifierAttribute.FromAssembly(assembly);
			if (pia != null)
				productDescriptor.Id = pia.Id;

			// snag the product requires registration attribute from the assembly
			ProductRequiresRegistrationAttribute rra = ProductRequiresRegistrationAttribute.FromAssembly(assembly);
			if (rra != null)
				productDescriptor.RequiresRegistration = rra.RequiresRegistration;
		
			return productDescriptor;			
		}

		/// <summary>
		/// Gets or sets the name of this product
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the version for this product
		/// </summary>
		public Version Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates whether the product requires registration
		/// </summary>
		public bool RequiresRegistration
		{
			get
			{
				return _requiresRegistration;
			}
			set
			{
				_requiresRegistration = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the identifier for this product
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
	}
}
