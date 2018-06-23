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

namespace Carbon.Common
{
	/// <summary>
	/// Provides a class that combines a separate file and associated version that 
	/// may or may not be related to the file's actual timestamp.
	/// </summary>
	public sealed class VersionedFile
	{
	    /// <summary>
		/// Initializes a new instance of the VersionedFile class.
		/// </summary>
		/// <param name="version">A version to associate with this file.</param>
		/// <param name="file">File information that is the context of this object.</param>
		public VersionedFile(Version version, FileInfo file)
		{
			Version = version;
			File = file;			
		}

		/// <summary>
		/// Returns the version associated with the file.
		/// </summary>
		public Version Version { get; }

	    /// <summary>
		/// Returns the file that is ultimately the context of this entire class.
		/// </summary>
		public FileInfo File { get; }

	   

		/// <summary>
		/// Sorts an array of versioned file objects.
		/// </summary>
		/// <param name="files">The array of items to sort.</param>
		/// <returns></returns>
		public static VersionedFile[] Sort(VersionedFile[] files)
		{
			// we could swap this bubble sort out for an IComparer implementation.

			// front to back - 1 
			for(var i = 0; i < files.Length - 1; i++)
			{
				// front + 1 to back
				for(var j = i + 1; j < files.Length; j++)
				{			
					if (files[i].Version < files[j].Version)
					{											 
						// swap i with j, where i=1 and j=2
						var file = files[j];
						files[j] = files[i];
						files[i] = file;
					}													
				}
			}
			return files;
		}		

		/// <summary>
		/// Converts the array of files to an array of versioned files
		/// </summary>
		/// <param name="files">The files to work with.</param>
		/// <returns></returns>
		public static VersionedFile[] CreateVersionedFiles(FileInfo[] files)
		{			
			var array = new ArrayList();
			foreach(var file in files)
			{				
				try
				{
					var name = file.Name.Replace(file.Extension, null);
					var version = new Version(name);
					var vf = new VersionedFile(version, file);
					array.Add(vf);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
			return array.ToArray(typeof(VersionedFile)) as VersionedFile[];
		}

		/// <summary>
		/// Returns an array of versioned files, optionally allowing for some prepended text to remove.
		/// </summary>
		/// <param name="prependedTextToRemove">Optional text to remove from the file name.</param>
		/// <param name="files">An array of files to work with.</param>
		/// <returns></returns>
		public static VersionedFile[] CreateVersionedFiles(string prependedTextToRemove, FileInfo[] files)
		{
			var array = new ArrayList();
			foreach(var file in files)
			{				
				try
				{
					// strip the file extention
					var name = file.Name.Replace(file.Extension, null);
					
					// remove prepended text here
					name = name.Replace(prependedTextToRemove, null);
					
					// create a version from the file name that's remaining
					var version = new Version(name);
					var vf = new VersionedFile(version, file);
					array.Add(vf);
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex);
				}
			}
			return array.ToArray(typeof(VersionedFile)) as VersionedFile[];
		}

		/// <summary>
		/// Returns the latest version available in the array of files
		/// </summary>
		/// <param name="versionedFiles">Returns the latest file based on it's version.</param>
		/// <returns></returns>
		public static VersionedFile GetLatestVersion(VersionedFile[] versionedFiles)
		{
		    if (versionedFiles?.Length > 0)
		    {
		        return versionedFiles[0];
		    }
		    return null;
		}
	}
}
