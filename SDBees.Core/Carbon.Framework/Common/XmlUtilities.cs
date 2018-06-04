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
using System.Xml;

namespace Carbon.Common
{
	/// <summary>
	/// Provides methods commonly used to manipulate Xml documents.
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	public static class XmlUtilities
	{
		#region XmlAttributeMissingException

		/// <summary>
		/// Defines an exception generated when a XmlNode is missing an attribute.
		/// </summary>
		public sealed class XmlAttributeMissingException : ApplicationException
		{
			private readonly XmlNode _node;
			private readonly string _attributeName;

			/// <summary>
			/// Initializes a new instance of the XmlAttributeMissingException class
			/// </summary>
			/// <param name="node">The XmlNode missing the attribute</param>
			/// <param name="attributeName">The name of the attribute missing</param>
			internal XmlAttributeMissingException(XmlNode node, string attributeName) : 
				base(string.Format("The XmlNode '{0}' does not have an attribute named '{1}'.", node.Name, attributeName))
			{
				_node = node;
				_attributeName = attributeName;
			}

			/// <summary>
			/// Returns the XmlNode that is missing the attribute
			/// </summary>
			public XmlNode Node
			{
				get
				{
					return _node;
				}
			}

			/// <summary>
			/// Returns the name of the attribute that is missing on the XmlNode
			/// </summary>
			public string AttributeName
			{
				get
				{
					return _attributeName;
				}
			}
		}

		#endregion        

		/// <summary>
		/// Returns the XmlAttribute from the XmlNode with the specified name
		/// </summary>
		/// <param name="node">The XmlNode to read the attribute from</param>
		/// <param name="attributeName">The name of the attribute to read</param>
		/// <param name="throwExceptionIfMissing">true to throw an exception if the attribute is not found</param>
		/// <returns></returns>
		public static XmlAttribute GetAttribute(XmlNode node, string attributeName, bool throwExceptionIfMissing)
		{
			if (node == null)
				throw new ArgumentNullException("node");

			if (attributeName == null || attributeName == string.Empty)
				throw new ArgumentNullException("attributeName");

			foreach (XmlAttribute attribute in node.Attributes)
				if (string.Compare(attribute.Name, attributeName, true) == 0)
					return attribute;

			if (throwExceptionIfMissing)
				throw new XmlAttributeMissingException(node, attributeName);

			return null;
		}
	}
}


