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
using System.Configuration;
using System.Xml;
using Carbon.Common;

namespace Carbon.Configuration.SectionHandlers
{
	/// <summary>
	/// Defines the base class all Carbon Provider ConfigurationSection handlers should derive.
	/// </summary>
	public abstract class ProviderConfigurationSectionHandler : IConfigurationSectionHandler
	{
	    #region IConfigurationSectionHandler Members

		/// <summary>
		/// Creates an object from the XmlNode passed to this ProviderConfigurationSectionHandler by the Configuration Runtime.
		/// This method is not intended for external use. It is only public due to the interface limitations implied by the C# 
		/// language specifications. We'll use interface hiding to hide it unless explicitly cast to the IConfigurationSectionHandler interface.
		/// </summary>
		/// <returns></returns>
		object IConfigurationSectionHandler.Create(object parent, object configContext, XmlNode section)
		{
			// create a collection for the providers defined in this config section
			var providers = GetProviderCollection();
						
			// grab all of the providers defined 
			var nodes = section.SelectNodes("Provider");
			
			foreach (XmlNode node in nodes)
			{
				// grab the required 'Type' attribute
				var typeAttribute = XmlUtilities.GetAttribute(node, "Type", true);
				
				// load the type from the attribute's value
				var type = TypeUtilities.GetTypeFromFullyQualifiedName(typeAttribute.Value, true, true);

				// create the provider from the type and any additional info stored in the node
				providers.Add(CreateProvider(node, type));
			}
			
			// return the collection of providers
			return providers;
		}

		#endregion

		/// <summary>
		/// Returns the appropriate provider collection into which created providers will be placed
		/// </summary>
		/// <returns></returns>
		protected abstract ProviderCollection GetProviderCollection();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="node"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		protected virtual Provider CreateProvider(XmlNode node, Type type)
		{
			// grab the required 'Name' attribute
			var nameAttribute = XmlUtilities.GetAttribute(node, "Name", true);
			
			// assert that the type derives from the required base class	
			TypeUtilities.AssertTypeIsSubclassOfBaseType(type, typeof(Provider));

			// create an instance of the specified type
			return (Provider)TypeUtilities.CreateInstanceOfType(type, new[] {typeof(string)}, new object[] {nameAttribute.Value});			
		}

	}
}
