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
	/// Provides a class that is capable of reading an AutoUpdateManifest from an Xml file.
	/// </summary>
	[System.Diagnostics.DebuggerStepThrough()]
	public sealed class XmlAutoUpdateManifestReader : DisposableObject
	{
		private XmlDocument _document;
						
		/// <summary>
		/// Initializes a new instance of the XmlAutoUpdateManifestReader class.
		/// </summary>
		/// <param name="filename">The file to read from.</param>
		public XmlAutoUpdateManifestReader(string filename)
		{			
			_document = new XmlDocument();
			_document.Load(filename);
		}
		
		/// <summary>
		/// Initializes a new instance of the XmlAutoUpdateManifestReader class.
		/// </summary>
		/// <param name="stream">The stream to read from.</param>
		public XmlAutoUpdateManifestReader(Stream stream)
		{
			_document = new XmlDocument();
			_document.Load(stream);
		}

		/// <summary>
		/// Initializes a new instance of the XmlAutoUpdateManifestReader class.
		/// </summary>
		/// <param name="reader">The text reader to</param>
		public XmlAutoUpdateManifestReader(TextReader reader)
		{
			_document = new XmlDocument();
			_document.Load(reader);			
		}

		/// <summary>
		/// Initializes a new instance of the XmlAutoUpdateManifestReader class.
		/// </summary>
		/// <param name="node"></param>
		public XmlAutoUpdateManifestReader(XmlNode node)
		{
			_document = new XmlDocument();
			_document.LoadXml(node.OuterXml);
		}

		protected override void DisposeOfManagedResources()
		{
			_document = null;

			base.DisposeOfManagedResources();
		}

		/// <summary>
		/// Reads the file and returns an AutoUpdateManifest instance.
		/// </summary>
		/// <returns></returns>
		public AutoUpdateManifest Read()
		{
			// create an xpath navigator so that we can traverse the elements inside the xml
			XPathNavigator navigator = _document.CreateNavigator();

			// move to the version element
			navigator.MoveToFirstChild();

//			// move to the file format description element
//			navigator.MoveToNext();
//
//			// move to the shout outs element
//			navigator.MoveToNext();

			AutoUpdateManifest manifest = new AutoUpdateManifest();
            
			// read the manifest
			this.Read(navigator, manifest);

			return manifest;
		}
		
		private void Read(XPathNavigator navigator, AutoUpdateManifest manifest)
		{
			if (navigator == null)
			{
				throw new ArgumentNullException("navigator");
			}

			if (manifest == null)
			{
				throw new ArgumentNullException("manifest");
			}

			if (navigator.HasAttributes)
			{
				// clone the current navigator so we can save our position
				XPathNavigator attributesNavigator = navigator.Clone();

				// move to the first attribute
				if (attributesNavigator.MoveToFirstAttribute())
				{
					do
					{
						// parse the attributes from the test element
						switch(attributesNavigator.Name)
						{
							case @"Id":
								manifest.Id = attributesNavigator.Value;
								break;
						
							case @"Url":
								manifest.UrlOfUpdate = attributesNavigator.Value;
								break;

							case @"Size":
								manifest.SizeOfUpdate = long.Parse(attributesNavigator.Value);
								break;
						};
					}
					while(attributesNavigator.MoveToNextAttribute());
				}
			}

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
						case @"AutoUpdateProductDescriptor":						
						{
							AutoUpdateProductDescriptor product;
							this.ReadProductDescriptor(navigator, out product);
							manifest.Product = product;
							navigator.MoveToParent();
							break;
						}

						case @"AutoUpdateHref":											
						{
							AutoUpdateHref moreInfo;
							this.ReadHref(navigator, out moreInfo);
							manifest.MoreInfo = moreInfo;
							navigator.MoveToParent();
							break;
						}

						case @"AutoUpdateChangeSummaryList":												
						{
							AutoUpdateChangeSummaryList changeSummaryList;
							this.ReadChangeSummaries(navigator, out changeSummaryList);
							manifest.ChangeSummaries = changeSummaryList;
							navigator.MoveToParent();
							break;
						}
					};

				}
				while(navigator.MoveToNext());
			}				
		}	


		private void ReadProductDescriptor(XPathNavigator navigator, out AutoUpdateProductDescriptor product)
		{
			product = new AutoUpdateProductDescriptor();

			// if the element has attributs
			if (navigator.HasAttributes)
			{
				// clone the current navigator so we can save our position
				XPathNavigator attributesNavigator = navigator.Clone();

				// move to the first attribute
				if (attributesNavigator.MoveToFirstAttribute())
				{
					do
					{
						// parse the attributes from the test element
						switch(attributesNavigator.Name)
						{
						case @"Version":
							product.Version = new Version(attributesNavigator.Value);
							break;

						case @"RequiresRegistration":																
							product.RequiresRegistration = bool.Parse(attributesNavigator.Value);
							break;
						};
					}
					while(attributesNavigator.MoveToNextAttribute());
				}
			}

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
					case @"Name":						
						product.Name = navigator.Value;
						break;					
					};
				}
				while(navigator.MoveToNext());
			}				
		}

		private void ReadHref(XPathNavigator navigator, out AutoUpdateHref href)
		{
			href = new AutoUpdateHref();

			// if the element has attributs
			if (navigator.HasAttributes)
			{
				// clone the current navigator so we can save our position
				XPathNavigator attributesNavigator = navigator.Clone();

				// move to the first attribute
				if (attributesNavigator.MoveToFirstAttribute())
				{
					do
					{
						// parse the attributes from the test element
						switch(attributesNavigator.Name)
						{
						case @"Href":
							href.Href = attributesNavigator.Value;
							break;
						};
					}
					while(attributesNavigator.MoveToNextAttribute());
				}
			}

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
					case @"Text":						
						href.Text = navigator.Value;
						break;					
					};
				}
				while(navigator.MoveToNext());
			}
		}

		private void ReadChangeSummaries(XPathNavigator navigator, out AutoUpdateChangeSummaryList changeSummaryList)		
		{
			changeSummaryList = new AutoUpdateChangeSummaryList();

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
						case @"AutoUpdateChangeSummary":	
						{							
							AutoUpdateChangeSummary changeSummary;
							this.ReadChangeSummary(navigator, out changeSummary);
							changeSummaryList.Add(changeSummary);
							navigator.MoveToParent();
							break;
						}
					};

				}
				while(navigator.MoveToNext());
			}	
		}

		private void ReadChangeSummary(XPathNavigator navigator, out AutoUpdateChangeSummary changeSummary)
		{
			changeSummary = new AutoUpdateChangeSummary();

			// if the element has attributs
			if (navigator.HasAttributes)
			{
				// clone the current navigator so we can save our position
				XPathNavigator attributesNavigator = navigator.Clone();

				// move to the first attribute
				if (attributesNavigator.MoveToFirstAttribute())
				{
					do
					{
						// parse the attributes from the test element
						switch(attributesNavigator.Name)
						{
						case @"Type":
								
							changeSummary.Type = (AutoUpdateChangeTypes)Enum.Parse(typeof(AutoUpdateChangeTypes), attributesNavigator.Value, true);
							break;

						case @"PostedBy":
							changeSummary.PostedBy = attributesNavigator.Value;
							break;

						case @"DatePosted":
							changeSummary.DatePosted = DateTime.Parse(attributesNavigator.Value);
							break;

						case @"Id":
							changeSummary.Id = attributesNavigator.Value;
							break;						
						};
					}
					while(attributesNavigator.MoveToNextAttribute());
				}
			}

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
					case @"Title":						
						changeSummary.Title = navigator.Value;
						break;	
				
					case @"Preview":						
						changeSummary.Preview = navigator.Value;
						break;					
					};
				}
				while(navigator.MoveToNext());
			}
		}

		#region My Public Static Methods

		/// <summary>
		/// Writes the Test specified to the file specified using the specified encoding
		/// </summary>
		/// <param name="test">The test to write</param>
		/// <param name="path">The file to write to</param>
		/// <param name="encoding">The encoding to write with</param>
		public static AutoUpdateManifest Read(string path)
		{						
			AutoUpdateManifest manifest = null;

			// create a new test writer
			using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// create a reader to read the manifest
				XmlAutoUpdateManifestReader reader = new XmlAutoUpdateManifestReader(stream);

				// read the manifest
				manifest = reader.Read();
				
				stream.Close();
			}

			return manifest;
		}

		#endregion

	}
}
