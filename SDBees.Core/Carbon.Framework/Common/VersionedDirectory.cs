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

namespace Carbon.Common
{
	/// <summary>
	/// Provides a class that combines a separate directory and associated version that 
	/// may or may not be related to the file's actual timestamp.
	/// </summary>
	public sealed class VersionedDirectory
	{
	    /// <summary>
        /// Initializes a new instance of the VersionedDirectory class.
        /// </summary>
        /// <param name="version">A version to associate with this directory.</param>
        /// <param name="directory">Directory information that is the context of this object.</param>
        public VersionedDirectory(Version version, DirectoryInfo directory)
		{
			Version = version;
			Directory = directory;
		}

		/// <summary>
		/// Returns a version that is associated with this directory.
		/// </summary>
		public Version Version { get; }

	    /// <summary>
		/// Returns the directory 
		/// </summary>
		public DirectoryInfo Directory { get; }

	    #region static Mmthods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="directories"></param>
		/// <returns></returns>
		public static VersionedDirectory[] Sort(VersionedDirectory[] directories)
		{
			// front to back - 1 
			for(var i = 0; i < directories.Length - 1; i++)
			{
				// front + 1 to back
				for(var j = i + 1; j < directories.Length; j++)
				{			
					if (directories[i].Version < directories[j].Version)
					{											 
						// swap i with j, where i=1 and j=2
						var directory = directories[j];
						directories[j] = directories[i];
						directories[i] = directory;
					}													
				}
			}
			return directories;
		}

		#endregion
	}
}
