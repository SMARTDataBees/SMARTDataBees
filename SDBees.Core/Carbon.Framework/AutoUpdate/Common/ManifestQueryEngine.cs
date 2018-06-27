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
using System.IO;
using System.Xml;
using Carbon.Common;

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Provides utility methods for finding manifest files in the local file system.
	/// </summary>
	public static class ManifestQueryEngine
	{
		/// <summary>
		/// Queries the latest version of a particular product using the local file system as a backing store.
		/// </summary>
		/// <param name="updatesPath">The path to the updates</param>
		/// <param name="productName">The name of the product to update</param>
		/// <param name="currentVersion">The current version of the product that is checking for updates</param>
		/// <param name="productId">The id of the product that is checking for updates</param>
		/// <returns></returns>		
		public static XmlDocument QueryLatestVersion(string updatesPath, string productName, string currentVersion, string productId)
		{
			/*
			 * we really don't need all the information, but it would be nice to log who is trying to update
			 * */			
			try
			{				
				// log information about this event to the system's event log
//				Debug.WriteLine(string.Format("The product '{0}' version '{1}' with Id '{2}' checked for updates at {3}.", productName, currentVersion, productId, DateTime.Now.ToString()));
				
				// there must be a path to the updates folder
				if (updatesPath == null || updatesPath == string.Empty)
					return null;

				// append the product name to the updates path 
				var path = Path.Combine(updatesPath, productName);

				// if the directory doesn't exist, bail with null
				if (!Directory.Exists(path))
					return null;
				
				// find all of the manifest files in the path specified							
				var di = new DirectoryInfo(path);							
				var files = di.GetFiles("*.manifest", SearchOption.TopDirectoryOnly);

				// create versioned files from the results
				var versionedFiles = VersionedFile.CreateVersionedFiles($"{productName}-", files);

				// sort them
				versionedFiles = VersionedFile.Sort(versionedFiles);

				// grab the latest version
				var latestVersion = VersionedFile.GetLatestVersion(versionedFiles);
				
				// assuming there is a version available
				if (latestVersion != null)
				{					
					// create a new xml document to hold the response
					var doc = new XmlDocument();

					// load the document with the xml
					doc.Load(latestVersion.File.FullName);

					// return the doc, which will return the document element, 
					// which is the pure xml inside the soap headers of the web service response
					// skipping anymore encoding issues.
					return doc;				
				}				
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			return null;
		}		
	}
}
