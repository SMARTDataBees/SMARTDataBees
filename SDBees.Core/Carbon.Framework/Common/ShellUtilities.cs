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
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Carbon.Common
{
	/// <summary>
	/// Defines the predefined sizes of Icon's extracted using SHGetFileInfo
	/// </summary>
	[Flags]
	public enum IconSizes
	{
		/// <summary>
		/// Extract the Icon as a large Icon (Usually defined as 32x32)
		/// </summary>
		LargeIconSize = ShellUtilities.SHGFI_LARGEICON,

		/// <summary>
		/// Extract the Icon as a small Icon (Usually defined as 16x16)
		/// </summary>
		SmallIconSize = ShellUtilities.SHGFI_SMALLICON,

		/// <summary>
		/// Extract the Icon at the predefined Shell size
		/// </summary>
		ShellIconSize = 0x0004
	}
		
	/// <summary>
	/// Defines the predefined styles that can be applied to Icon's extracted using SHGetFileInfo
	/// </summary>
	[Flags]
	public enum IconStyles
	{
		/// <summary>
		/// Normal Icon
		/// </summary>
		NormalIconStyle	= ShellUtilities.SHGFI_ICON,

		/// <summary>
		/// Includes the shortcut overlay
		/// </summary>
		LinkOverlayIconStyle = ShellUtilities.SHGFI_LINKOVERLAY,

		/// <summary>
		/// Applies a color matrix to create a selected look
		/// </summary>
		SelectedIconStyle = ShellUtilities.SHGFI_SELECTED,

		/// <summary>
		/// Gets the open version of the Icon, usually applied to folders that can display open and closed versions of their icons
		/// </summary>
		OpenIconStyle = ShellUtilities.SHGFI_OPENICON
	}
		
	/// <summary>
	/// Provides utility methods for retrieving Windows shell information.
	/// </summary>
	[DebuggerStepThrough]
	public sealed class ShellUtilities
	{
		internal const int MAX_PATH = 256;
		internal const int SHGFI_ICON				= 0x000000100;     // get icon
		internal const int SHGFI_DISPLAYNAME		= 0x000000200;     // get display name
		internal const int SHGFI_TYPENAME          	= 0x000000400;     // get type name
		internal const int SHGFI_ATTRIBUTES        	= 0x000000800;     // get attributes
		internal const int SHGFI_ICONLOCATION      	= 0x000001000;     // get icon location
		internal const int SHGFI_EXETYPE           	= 0x000002000;     // return exe type
		internal const int SHGFI_SYSICONINDEX      	= 0x000004000;     // get system icon index
		internal const int SHGFI_LINKOVERLAY       	= 0x000008000;     // put a link overlay on icon
		internal const int SHGFI_SELECTED          	= 0x000010000;     // show icon in selected state
		internal const int SHGFI_ATTR_SPECIFIED    	= 0x000020000;     // get only specified attributes
		internal const int SHGFI_LARGEICON         	= 0x000000000;     // get large icon
		internal const int SHGFI_SMALLICON         	= 0x000000001;     // get small icon
		internal const int SHGFI_OPENICON          	= 0x000000002;     // get open icon
		internal const int SHGFI_SHELLICONSIZE     	= 0x000000004;     // get shell size icon
		internal const int SHGFI_PIDL              	= 0x000000008;     // pszPath is a pidl
		internal const int SHGFI_USEFILEATTRIBUTES 	= 0x000000010;     // use passed dwFileAttribute
		internal const int SHGFI_ADDOVERLAYS       	= 0x000000020;     // apply the appropriate overlays
		internal const int SHGFI_OVERLAYINDEX      	= 0x000000040;     // Get the index of the overlay
	
		[DllImport("Shell32.dll")]
		private static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);
		
		[DllImport("User32.dll")]
		private static extern int DestroyIcon(IntPtr hIcon);
		
		[DllImport("gdi32.dll")]
		private static extern int DeleteObject(IntPtr hObject);
		
		/// <summary>
		/// Provides information about a file system object in the Windows shell.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		private struct SHFILEINFO 
		{
			public IntPtr hIcon;
			public int iIcon;
			public int dwAttributes;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string szDisplayName;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		}

		/// <summary>
		/// Initializes a new instance of the ShellUtilities class.
		/// </summary>
		private ShellUtilities() {}

		#region My Static Methods

		/// <summary>
		/// Returns the associated Icon for the file or folder path specified.
		/// </summary>
		/// <param name="path">The path to the file or folder</param>
		/// <param name="size">The size of the Icon to return</param>
		/// <param name="style">The style of the Icon to return</param>
		/// <param name="attributes">The file attributes to use to find the file or folder</param>
		/// <returns></returns>
		public static Icon GetIconFromPath(string path, IconSizes size, IconStyles style, FileAttributes attributes)
		{
			var shfi = new SHFILEINFO();
			uint uFlags = SHGFI_ICON;
			
			uFlags |= (uint)size;
			uFlags |= (uint)style;
			if (attributes != 0) 
				uFlags |= SHGFI_USEFILEATTRIBUTES; 
			
			SHGetFileInfo(path,	(int)attributes, ref shfi, (uint)Marshal.SizeOf(shfi), uFlags);

			var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
			DeleteObject(shfi.hIcon);
			return icon;
		}

		/// <summary>
		/// Returns a Bitmap created from the associated Icon for the file or folder path specified.
		/// </summary>
		/// <param name="path">The path can be a fully qualified path name, non-existent file, or file extension</param>
		/// <param name="size">The size of the bitmap to return</param>
		/// <param name="style">The style of the bitmap to retrieve</param>
		/// <param name="attributes">The file attributes to use to find the file or folder</param>
		/// <returns></returns>
		public static Bitmap GetBitmapFromPath(string path,	IconSizes size,	IconStyles style, FileAttributes attributes)
		{
			var shfi = new SHFILEINFO();
			uint uFlags = SHGFI_ICON;
			
			uFlags |= (uint)size;
			uFlags |= (uint)style;
			if (attributes != 0)
				uFlags |= SHGFI_USEFILEATTRIBUTES; 
			
			SHGetFileInfo(path,	(int)attributes, ref shfi, (uint)Marshal.SizeOf(shfi), uFlags);
						
			var bitmap = (Bitmap)Bitmap.FromHicon(shfi.hIcon).Clone();			
			DeleteObject(shfi.hIcon);
			return bitmap;
		}

		/// <summary>
		/// Returns an Image created from the associated Icon for the file or folder path specified.
		/// </summary>
		/// <param name="path">The path can be a fully qualified path name, non-existent file, or file extension</param>
		/// <param name="size">The size of the bitmap to return</param>
		/// <param name="style">The style of the bitmap to retrieve</param>
		/// <param name="attributes">The file attributes to use to find the file or folder</param>
		/// <returns></returns>
		public static Image GetImageFromPath(string path, IconSizes size, IconStyles style, FileAttributes attributes)
		{
			var shfi = new SHFILEINFO();
			uint uFlags = SHGFI_ICON;
			
			uFlags |= (uint)size;
			uFlags |= (uint)style;
			if (attributes != 0)
				uFlags |= SHGFI_USEFILEATTRIBUTES; 
			
			SHGetFileInfo(path,	(int)attributes, ref shfi, (uint)Marshal.SizeOf(shfi), uFlags);
						
			var image = (Image)Bitmap.FromHicon(shfi.hIcon).Clone();			
			DeleteObject(shfi.hIcon);
			return image;
		}

		/// <summary>
		/// Returns the type name for the file or folder specified.
		/// </summary>
		/// <param name="path">The file or folder path to retrieve the type name for</param>
		/// <returns></returns>
		public static string GetTypeName(string path)
		{
			var shfi = new SHFILEINFO();
			var p = SHGetFileInfo(path, (int)FileAttributes.Normal, ref shfi, (uint)Marshal.SizeOf(shfi), SHGFI_TYPENAME | SHGFI_USEFILEATTRIBUTES);
			return shfi.szTypeName;
		}

		/// <summary>
		/// Returns the display name for the file or folder specified.
		/// </summary>
		/// <param name="path">The file or folder path to retrieve the display name for</param>
		/// <returns></returns>
		public static string GetDisplayName(string path)
		{
			var shfi = new SHFILEINFO();
			var p = SHGetFileInfo(path, (int)FileAttributes.Normal, ref shfi, (uint)Marshal.SizeOf(shfi), SHGFI_DISPLAYNAME | SHGFI_USEFILEATTRIBUTES);
			return shfi.szDisplayName;
		}

		/// <summary>
		/// Determines if the path specified is a file or folder.
		/// </summary>
		/// <param name="path">The path that may be a file or folder to check</param>
		/// <returns></returns>
		public static bool IsDirectory(string path)
		{
			try
			{
				var attributes = File.GetAttributes(path);				
				return ((attributes & FileAttributes.Directory) == FileAttributes.Directory);				
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
			return false;
		}

		/// <summary>
		/// Returns the last write time for the file or folder specified.
		/// </summary>
		/// <param name="path">The path of the file or folder to retrieve the last write time for</param>
		/// <returns></returns>
		public static string GetLastWriteTime(string path)
		{		
			try
			{
				var last = File.GetLastWriteTime(path);
				return last.ToString();
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
			return DateTime.MinValue.ToString();
		}

		#endregion
	}
}
