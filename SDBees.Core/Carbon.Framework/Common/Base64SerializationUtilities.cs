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
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Carbon.Common
{
	/// <summary>
	/// The EncodingEngine class provides methods for Base64 and UUEncoding encoding/decoding data.
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	public static class Base64SerializationUtilities
	{
		/// <summary>
		/// Determines if the specified Type implements the ISerializable interface
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool SupportsISerializableInterface(Type t)
		{
			return (t.GetInterface(typeof(System.Runtime.Serialization.ISerializable).FullName) != null);
		}

		/// <summary>
		/// Serializes the object graph using a Binary formatter, and then encodes the resulting bytes into a Base64 string that will be safe for POP attachments or Xml node data.
		/// </summary>
		/// <param name="value">The object graph to serialize. NOTE: The object must support serialization via the SerializableAttribute and optionally support the ISerializable interface.</param>
		/// <param name="t">The Type of the object, needed to ensure serialization is supported</param>
		/// <param name="base64String">A reference to a string which will receive the encoded result of the operation</param>
		/// <returns></returns>
		public static bool Serialize(object instance, Type t, out string base64String)
		{
			base64String = null;

			/// if the type supports serialization
			if (SupportsISerializableInterface(t) || t.IsSerializable)
			{								
				BinaryFormatter bf = new BinaryFormatter();
				using(MemoryStream ms = new MemoryStream())
				{
					/// serialize the object graph
					bf.Serialize(ms, instance);
					/// retrieve the raw bytes from the serialization
					byte[] bytes = ms.GetBuffer();
					/// encode them to a base64 string
					return ((base64String = System.Convert.ToBase64String(bytes)) != null);
				}
			}			
			return false;
		}

		/// <summary>
		/// Deserializes a Base64 encoded string representing a serialized object graph in binary format. Decodes the string and then deserializes the object.
		/// </summary>
		/// <param name="base64String">The encoded string containing the serialization graph</param>
		/// <param name="instance">The instance of the object created when the function returns</param>
		/// <returns></returns>
		public static bool Deserialize(string base64String, out object instance)
		{
			instance = null;
			BinaryFormatter bf = new BinaryFormatter();
			/// decode the string
			byte[] bytes = System.Convert.FromBase64String(base64String);
			using(MemoryStream ms = new MemoryStream(bytes))
			{
				/// deserialize the object graph
				return ((instance = bf.Deserialize(ms)) != null);
			}	
		}
	}
}
