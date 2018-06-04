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
using System.Text;
using System.Xml;
using System.Xml.XPath;

using Carbon.Common;
using Carbon.AutoUpdate;

namespace Carbon.AutoUpdate.Common.Xml
{
	/// <summary>
	/// Provides a class that is capable of writing an AutoUpdateManifest to an Xml file.
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	public sealed class XmlAutoUpdateManifestWriter : DisposableObject
	{
		private XmlTextWriter _writer;
		
		/// <summary>
		/// Initializes a new instance of the XmlTestWriter class
		/// </summary>
		/// <param name="writer">The text writer to write to</param>
		public XmlAutoUpdateManifestWriter(TextWriter writer) 
		{			
			_writer = new XmlTextWriter(writer);
			_writer.Formatting = Formatting.Indented;
		}

		/// <summary>
		/// Initializes a new instance of the XmlTestWriter class
		/// </summary>
		/// <param name="filename">The filename to write to. If the file exists, it will truncate it and overwrite the existing content.</param>
		/// <param name="encoding">The encoding to use while writing</param>
		public XmlAutoUpdateManifestWriter(string filename, Encoding encoding)
		{
			_writer = new XmlTextWriter(filename, encoding);
			_writer.Formatting = Formatting.Indented;
		}

		/// <summary>
		/// Initializes a new instance of the XmlTestWriter class
		/// </summary>
		/// <param name="stream">The stream to which you want to write</param>
		/// <param name="encoding">The encoding to use while writing</param>
		public XmlAutoUpdateManifestWriter(Stream stream, Encoding encoding)
		{
			_writer = new XmlTextWriter(stream, encoding);	
			_writer.Formatting = Formatting.Indented;
		}

		protected override void DisposeOfManagedResources()
		{
			if (_writer != null)
			{
				_writer.Close();
				_writer = null;
			}
			base.DisposeOfManagedResources();
		}

		#region My Public Methods

		/// <summary>
		/// Writes an Xml document containing the AutoUpdateManifest specified
		/// </summary>
		/// <param name="manifest">The manifest to write.</param>
		public void Write(AutoUpdateManifest manifest)
		{
			if (manifest == null)
			{
				throw new ArgumentNullException("manifest");
			}

			// start the document
			_writer.WriteStartDocument();

			// comment on the version, and some props to me
//			_writer.WriteComment(@"Object design by Mark Belles, Xml formatting by Mark Belles");

			// start the manifest element
			_writer.WriteStartElement(manifest.GetType().Name);

			// write the manifest id (which is the update id needed for registration hashing)
			XmlWriterUtils.WriteAttributes(_writer, 
				new XmlStringPair("Id", manifest.Id),
				new XmlStringPair("Url", manifest.UrlOfUpdate),
				new XmlStringPair("Size", manifest.SizeOfUpdate.ToString())
				);

				// write the product
				this.WriteProductDescriptor(manifest.Product);

				// write the more info href
				this.WriteHref(manifest.MoreInfo);

				// write the change summaries
				this.WriteChangeSummaries(manifest.ChangeSummaries);

			// end the manifest element
			_writer.WriteEndElement();

			// end the document
			_writer.WriteEndDocument();

			// NOTE: Always flush the DAMN writer
			_writer.Flush();
		}
		
		#endregion

		#region My Private Methods

		/// <summary>
		/// Writes an Xml element containing AutoUpdateProductDescriptor specified
		/// </summary>
		/// <param name="product"></param>
		private void WriteProductDescriptor(AutoUpdateProductDescriptor product)
		{
			if (product == null)
				throw new ArgumentNullException("product");

			// start the element
			_writer.WriteStartElement(product.GetType().Name);

			XmlWriterUtils.WriteAttributes(_writer,
				new XmlStringPair("Version", product.Version.ToString()),
				new XmlStringPair("RequiresRegistration", product.RequiresRegistration.ToString()),
				new XmlStringPair("Id", product.Id)
				);
			
			XmlWriterUtils.WriteCDataElement(_writer, "Name", product.Name, null);

			// end the element
			_writer.WriteEndElement();
		}

		/// <summary>
		/// Writes an Xml element containing AutoUpdateHref specified
		/// </summary>
		/// <param name="moreInfo"></param>
		private void WriteHref(AutoUpdateHref moreInfo)
		{
			if (moreInfo == null)
				throw new ArgumentNullException("moreInfo");

			// start the element
			_writer.WriteStartElement(moreInfo.GetType().Name);

			XmlWriterUtils.WriteAttributes(_writer,
				new XmlStringPair("Href", moreInfo.Href)
				);
			
			XmlWriterUtils.WriteCDataElement(_writer, "Text", moreInfo.Text, null);

			// end the element
			_writer.WriteEndElement();
		}

		/// <summary>
		/// Writes an Xml element containing the AutoUpdateChangeSummaryList specified
		/// </summary>
		/// <param name="changeSummaries"></param>
		private void WriteChangeSummaries(AutoUpdateChangeSummaryList changeSummaries)
		{
			if (changeSummaries == null)
				throw new ArgumentNullException("changeSummaries");
	
			// start the element
			_writer.WriteStartElement(changeSummaries.GetType().Name);

			foreach(AutoUpdateChangeSummary changeSummary in changeSummaries)
				this.WriteChangeSummary(changeSummary);

			// end the element
			_writer.WriteEndElement();
		}
		
		/// <summary>
		/// Writes an Xml element containing the AutoUpdateChangeSummary specified
		/// </summary>
		/// <param name="changeSummary"></param>
		private void WriteChangeSummary(AutoUpdateChangeSummary changeSummary)
		{
			if (changeSummary == null)
				throw new ArgumentNullException("changeSummary");

			// start the element
			_writer.WriteStartElement(changeSummary.GetType().Name);

			XmlWriterUtils.WriteAttributes(_writer,
				new XmlStringPair("Type", changeSummary.Type.ToString()),
				new XmlStringPair("PostedBy", changeSummary.PostedBy),
				new XmlStringPair("DatePosted", changeSummary.DatePosted.ToString()),
				new XmlStringPair("Id", changeSummary.Id)
				);
						
			XmlWriterUtils.WriteCDataElement(_writer, "Title", changeSummary.Title, null);
			
			XmlWriterUtils.WriteCDataElement(_writer, "Preview", changeSummary.Preview, null);

			// end the element
			_writer.WriteEndElement();
		}

		#endregion

		#region My Public Static Methods

		/// <summary>
		/// Writes the Test specified to the file specified using the specified encoding
		/// </summary>
		/// <param name="test">The test to write</param>
		/// <param name="path">The file to write to</param>
		/// <param name="encoding">The encoding to write with</param>
		public static void Write(AutoUpdateManifest manifest, string path, Encoding encoding)
		{
			if (manifest == null)
				throw new ArgumentNullException("manifest");

			if (encoding == null)
				throw new ArgumentNullException("encoding");
	
			// create a new manifest writer
			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				// create a writer to write the test
				XmlAutoUpdateManifestWriter writer = new XmlAutoUpdateManifestWriter(stream, encoding);

				// write the test
				writer.Write(manifest);
				
				stream.Close();
			}
		}

		public static string ToXml(AutoUpdateManifest manifest, Encoding encoding)
		{
			if (manifest == null)
				throw new ArgumentNullException("manifest");

			if (encoding == null)
				throw new ArgumentNullException("encoding");

			using (MemoryStream stream = new MemoryStream())
			{
				XmlAutoUpdateManifestWriter writer = new XmlAutoUpdateManifestWriter(stream, encoding);

				writer.Write(manifest);

				stream.Close();

				return encoding.GetString(stream.GetBuffer());
			}
		}

		#endregion
	}
}
