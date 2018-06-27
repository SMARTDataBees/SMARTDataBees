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
using System.Drawing;
using System.Xml;

namespace Carbon.AutoUpdate.Common.Xml
{
	/// <summary>
	/// Provides utility methods for writing Xml data.
	/// </summary>
	internal static class XmlWriterUtils
	{
		/// <summary>
		/// Writes an XmlElement using non-cdata text and optional attributes
		/// </summary>
		/// <param name="writer">The writer to use</param>
		/// <param name="name">The name of the element</param>
		/// <param name="text">The text value of the element</param>
		/// <param name="attributes">The attributes for the element</param>
		public static void WriteElement(XmlWriter writer, string name, string text, params XmlStringPair[] attributes)
		{
			Debug.Assert(writer != null);

			// if the element will have data
			if (IsNeeded(text))
			{
				// start the element
				writer.WriteStartElement(name);

				// write the attributes
				WriteAttributes(writer, attributes);

				// write the text value
				writer.WriteString(text);

				// end the element
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Writes an XmlElement using cdata text and optional attributes
		/// </summary>
		/// <param name="writer">The writer to use</param>
		/// <param name="name">The name of the element</param>
		/// <param name="cdataText">The cdata text value of the element</param>
		/// <param name="attributes">The attributes for the element</param>
		public static void WriteCDataElement(XmlWriter writer, string name, string cdataText, params XmlStringPair[] attributes)
		{
			Debug.Assert(writer != null);
			
			// if the element will have data
			if (IsNeeded(cdataText))
			{
				// start the element
				writer.WriteStartElement(name);

				// write the attributes
				WriteAttributes(writer, attributes);

				// write the cdata text value
				writer.WriteCData(cdataText);

				// end the element
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Writes the string pairs as attributes of the writer's current element
		/// </summary>
		/// <param name="writer">The writer to use</param>
		/// <param name="attributes">The attributes to write</param>
		public static void WriteAttributes(XmlWriter writer, params XmlStringPair[] attributes)
		{
			// if we have any string pairs to write as attributes
			if (attributes != null)
				// enumerate them
				foreach(var attribute in attributes)
					// if the attribute value is needed
					if (IsNeeded(attribute.Value))
						// then write the attribute
						writer.WriteAttributeString(attribute.Name, attribute.Value);
		}

		public static void WriteElementList(XmlWriter writer, string listName, Type itemType, IList items)
		{
			Debug.Assert(writer != null);

			if (items != null)
			{
				writer.WriteStartElement(listName);
				
				// write the type of the items in the list
				writer.WriteAttributeString("Type", itemType.Name);

				foreach(var item in items)
				{
					writer.WriteStartElement(itemType.Name);
					writer.WriteAttributeString("Value", item.ToString());
					writer.WriteEndElement();
				}	

				writer.WriteEndElement();
			}
		}

		public static void WriteCDataElementList(XmlWriter writer, string listName, Type itemType, IList items)
		{
			Debug.Assert(writer != null);

			if (items != null)
			{
				// start the element
				writer.WriteStartElement(listName);

				// write the type of the items in the list
				writer.WriteAttributeString("Type", itemType.Name);
				
				foreach(var item in items)
				{
					// start the element
					writer.WriteStartElement(itemType.Name);

					// attribute the value of the item
					writer.WriteCData(item.ToString());

					// end the element
					writer.WriteEndElement();
				}	

				// end the element
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Determines if the string needs to be written. If it is null or empty, it is considered not needed.
		/// </summary>
		/// <param name="text">The text to check</param>
		/// <returns></returns>
		public static bool IsNeeded(string text)
		{
			return (text != null && text != string.Empty);			
		}

		/// <summary>
		/// Determines if the image needs to be written. If it is null, it is considered not needed.
		/// </summary>
		/// <param name="image">The image to check</param>
		/// <returns></returns>
		public static bool IsNeeded(Image image)
		{			
			return (image != null);
		}
	}
}
