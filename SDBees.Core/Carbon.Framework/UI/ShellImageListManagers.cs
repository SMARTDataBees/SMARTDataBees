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
using System.Drawing;
using System.Collections;
using System.Windows.Forms;

using Carbon.Common;

namespace Carbon.UI
{
	/// <summary>
	/// Provides a class for managing an ImageList that contains Images or Icons of shell objects.
	/// </summary>
	public abstract class ShellImageListManager
	{
		private Hashtable _extensions;
		private IconSizes _size;
		private IconStyles _style;

		/// <summary>
		/// Initializes a new instance of the ShellImageListManager class
		/// </summary>
		protected ShellImageListManager()
		{
			_extensions = new Hashtable();
			_size = IconSizes.ShellIconSize;
			_style = IconStyles.NormalIconStyle;
		}	

		#region My Public Methods

		/// <summary>
		/// Returns the index of the Image/Icon to use in the specified ImageList for the path of the file or folder specified.
		/// </summary>
		/// <param name="imageList">The ImageList where the Image is stored</param>
		/// <param name="path">The file or folder to retrieve the Image/Icon index for</param>
		/// <returns></returns>
		public int GetIconIndex(ImageList imageList, string path)
		{
			return this.GetIconIndex(imageList, path, false);
		}

		/// <summary>
		/// Returns the index of the Image/Icon to use in the specified ImageList for the path of the file or folder specified.
		/// </summary>
		/// <param name="imageList">The ImageList where the Image is stored</param>
		/// <param name="path">The file or folder to retrieve the Image/Icon index for</param>
		/// <param name="extractNew">A flag that determines if a cached Image/Icon is used or if the sub-system will extract a new Image/Icon instead</param>
		/// <returns></returns>
		public int GetIconIndex(ImageList imageList, string path, bool extractNew)
		{
			string extension;
			bool useNormalAttribs = false;
			System.IO.FileAttributes attributes = System.IO.FileAttributes.Normal;

			try
			{
				attributes = File.GetAttributes(path);

				if ((attributes & System.IO.FileAttributes.Directory) == System.IO.FileAttributes.Directory)
				{
					DirectoryInfo directory = new DirectoryInfo(path);
					extension = directory.Extension;
					if (extension == null || extension == string.Empty)
						extension = @"Folder";
				}
				else
				{
					FileInfo file = new FileInfo(path);

					// be carefull on these certain extensions! they will prolly contain different icons
					// if we wanted to be more elegant, these should be loaded from the config for extensibility reasons
					switch(file.Extension)
					{
						case ".exe": extension = file.FullName;	break;					
						case ".lnk": extension = file.FullName;	break;					
						case ".url": extension = file.FullName;	break;	
						case ".ico": extension = file.FullName;	break;	
						case ".cur": extension = file.FullName;	break;	
						case ".ani": extension = file.FullName;	break;	
						case ".msc": extension = file.FullName;	break;	
						default: extension = file.Extension; break;
					}
				}
			}
			catch
			{
				extension = path;
				useNormalAttribs = true;
			}

			if (!extractNew)
				if (_extensions.ContainsKey(extension))
					return (int)_extensions[extension];

			int index = imageList.Images.Count;

			FileAttributes attribs = FileAttributes.Normal;
			if (!useNormalAttribs)
				attribs = ((attributes & FileAttributes.Directory) == FileAttributes.Directory ? FileAttributes.Directory : FileAttributes.Normal);

			imageList.Images.Add(ShellUtilities.GetIconFromPath(path, _size, _style, attribs));

			if (!_extensions.ContainsKey(extension))
				_extensions.Add(extension, index);

			return index;
		}

		#endregion

		#region My Public Properties

		/// <summary>
		/// Gets or sets the size of the icon extracted for a given path
		/// </summary>
		public IconSizes Size
		{
			get
			{
				return _size;
			}
			set
			{
				_size = value;
			}
		}

		/// <summary>
		/// Gets or sets the style of the icon extracted for a give path
		/// </summary>
		public IconStyles Style
		{
			get
			{
				return _style;
			}
			set
			{
				_style = value;
			}
		}

		#endregion
	}

	/// <summary>
	/// Provides a class for managing an ImageList that contains the small Images or Icons of shell objects.
	/// </summary>
	public sealed class SmallShellImageListManager : ShellImageListManager
	{
		/// <summary>
		/// Initializes a new instance of the XXX class
		/// </summary>
		public SmallShellImageListManager() : base()
		{
			base.Size = IconSizes.SmallIconSize;
		}		
	}

	/// <summary>
	/// Provides a class for managing an ImageList that contains the large Images or Icons of shell objects.
	/// </summary>
	public sealed class LargeShellImageListManager : ShellImageListManager
	{
		/// <summary>
		/// Initializes a new instance of the XXX class
		/// </summary>
		public LargeShellImageListManager() : base()
		{
			base.Size = IconSizes.LargeIconSize;			
		}	
	}
}
