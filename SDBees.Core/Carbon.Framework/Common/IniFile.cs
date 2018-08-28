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
using System.Runtime.InteropServices;
using System.Text;

namespace Carbon.Common
{	
    /// <summary>
    /// Provides a class capable of manipulating Windows .Ini files.
	/// </summary>
	[DebuggerStepThrough]
	public sealed class IniFile 
	{		
		private string _filename;
		private const int MAX_ENTRY = 32768;

		#region Win32 APIs

		[DllImport("KERNEL32.DLL", EntryPoint="GetPrivateProfileIntA", CharSet=CharSet.Ansi)]
		private static extern int GetPrivateProfileInt(string lpApplicationName, string lpKeyName, int nDefault, string lpFileName);
		
		[DllImport("KERNEL32.DLL", EntryPoint="WritePrivateProfileStringA", CharSet=CharSet.Ansi)]
		private static extern int WritePrivateProfileString (string lpApplicationName, string lpKeyName, string lpString, string lpFileName);
		
		[DllImport("KERNEL32.DLL", EntryPoint="GetPrivateProfileStringA",  CharSet=CharSet.Ansi)]
		private static extern int GetPrivateProfileString (string lpApplicationName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
		
		[DllImport("KERNEL32.DLL", EntryPoint="GetPrivateProfileSectionNamesA", CharSet=CharSet.Ansi)]
		private static extern int GetPrivateProfileSectionNames (byte[] lpszReturnBuffer, int nSize, string lpFileName);
		
		[DllImport("KERNEL32.DLL", EntryPoint="WritePrivateProfileSectionA", CharSet=CharSet.Ansi)]
		private static extern int WritePrivateProfileSection (string lpAppName, string lpString, string lpFileName);
		
		#endregion

		/// <summary>
		/// Initializes a new instance of the IniFile class
		/// </summary>
		/// <param name="file">The path to the ini file (does not have to exist)</param>
		public IniFile(string file) 
		{
			Filename = file;
		}

		/// <summary>
		/// Gets or sets the path to the ini file
		/// </summary>
		public string Filename 
		{
			get 
			{
				return _filename;
			}
			set 
			{
				_filename = value;
			}
		}

		public int ReadInteger(string section, string key, int defVal) 
		{
			return GetPrivateProfileInt(section, key, defVal, Filename);
		}

		public int ReadInteger(string section, string key) 
		{
			return ReadInteger(section, key, 0);
		}

		public string ReadString(string section, string key, string defVal) 
		{
			var sb = new StringBuilder(MAX_ENTRY);
			var Ret = GetPrivateProfileString(section, key, defVal, sb, MAX_ENTRY, Filename);
			return sb.ToString();
		}

		public string ReadString(string section, string key) 
		{
			return ReadString(section, key, "");
		}

		public long ReadLong(string section, string key, long defVal) 
		{
			return long.Parse(ReadString(section, key, defVal.ToString()));
		}

		public long ReadLong(string section, string key) 
		{
			return ReadLong(section, key, 0);
		}

		public byte[] ReadByteArray(string section, string key) 
		{
			try 
			{
				return Convert.FromBase64String(ReadString(section, key));
			} 
			catch {}
			return null;
		}

		public bool ReadBoolean(string section, string key, bool defVal) 
		{
			return Boolean.Parse(ReadString(section, key, defVal.ToString()));
		}

		public bool ReadBoolean(string section, string key) 
		{
			return ReadBoolean(section, key, false);
		}

		public bool Write(string section, string key, int value) 
		{
			return Write(section, key, value.ToString());
		}
		
		public bool Write(string section, string key, string value) 
		{
			return (WritePrivateProfileString(section, key, value, Filename) != 0);
		}

		public bool Write(string section, string key, long value) 
		{
			return Write(section, key, value.ToString());
		}

		public bool Write(string section, string key, byte [] value)
		{
		    if (value == null)
				return Write(section, key, (string)null);
		    return Write(section, key, value, 0, value.Length);
		}

		public bool Write(string section, string key, byte [] value, int offset, int length)
		{
		    if (value == null)
				return Write(section, key, (string)null);
		    return Write(section, key, Convert.ToBase64String(value, offset, length));
		}

		public bool Write(string section, string key, bool value) 
		{
			return Write(section, key, value.ToString());
		}
		
		public bool DeleteKey(string section, string key) 
		{
			return (WritePrivateProfileString(section, key, null, Filename) != 0);
		}

		public bool DeleteSection(string section) 
		{
			return WritePrivateProfileSection(section, null, Filename) != 0;
		}

		public string[] GetSectionNames() 
		{
			try 
			{
				var buffer = new byte[MAX_ENTRY];
				GetPrivateProfileSectionNames(buffer, MAX_ENTRY, Filename);
				return Encoding.ASCII.GetString(buffer).Trim('\0').Split('\0');
			} 
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
			
			return null;
		}
	}
}
