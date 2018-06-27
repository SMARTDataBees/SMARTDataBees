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
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Carbon.Common
{
	/*
	 * This class was ported originally from a work found on the Code Project, into the Razor Framework.
	 * It has now been migrated into the Carbon Framework source code from the Razor Framework.
	 * */

	/// <summary>
	/// Provides methods for parsing a command line argument set, into a collection of name/value pairs using a variety of switches and combinations
	/// </summary>
	public sealed class CommandLineParser : DisposableObject, IEnumerable
	{
		private StringDictionary _arguments;

		/// <summary>
		/// Initializes a new instance of the CommandLineParsingEngine class
		/// </summary>
		public CommandLineParser()
		{

		}

		/// <summary>
		/// Initializes a new instance of the CommandLineParsingEngine class
		/// </summary>
		/// <param name="args">A command line argument set to parse</param>
		public CommandLineParser(string[] args)
		{
			Parse(args);
		}

		#region Implementation of IEnumerable

		public IEnumerator GetEnumerator()
		{
			return _arguments.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Parses the command line argument set into a collection of name/value pairs
		/// </summary>
		/// <param name="args">An array of command line arguments to parse.</param>
		public void Parse(string[] args)
		{
			_arguments = new StringDictionary();

			var regexSplitter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.IgnoreCase | RegexOptions.Compiled);
			var regexRemover = new Regex(@"^['""]?(.*?)['""]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

			string param = null;
			string[] paramElements;

			// Valid parameters forms:
			// {-,/,--}param{ ,=,:}((",')value(",'))
			// Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
			foreach (var arg in args)
			{
				// Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
				paramElements = regexSplitter.Split(arg, 3);

				switch (paramElements.Length)
				{
					// Found a value (for the last parameter found (space separator))
					case 1:

						if (param != null)
						{
							if (!_arguments.ContainsKey(param))
							{
								paramElements[0] = regexRemover.Replace(paramElements[0], "$1");
								_arguments.Add(param, paramElements[0]);
							}
							param = null;
						}
						// else Error: no parameter waiting for a value (skipped)
						break;
					// Found just a parameter

					case 2:
						// The last parameter is still waiting. With no value, set it to true.
						if (param != null)
						{
							if (!_arguments.ContainsKey(param)) _arguments.Add(param, "true");
						}
						param = paramElements[1];
						break;
					// param with enclosed value

					case 3:
						// The last parameter is still waiting. With no value, set it to true.
						if (param != null)
						{
							if (!_arguments.ContainsKey(param)) _arguments.Add(param, "true");
						}
						param = paramElements[1];
						// Remove possible enclosing characters (",')
						if (!_arguments.ContainsKey(param))
						{
							paramElements[2] = regexRemover.Replace(paramElements[2], "$1");
							_arguments.Add(param, paramElements[2]);
						}
						param = null;
						break;
				}
			}
			// In case a parameter is still waiting
			if (param != null)
			{
				if (!_arguments.ContainsKey(param)) _arguments.Add(param, "true");
			}
		}

		/// <summary>
		/// Returns the specified named argument from the command line.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string this[string name]
		{
			get
			{
				if (_arguments.ContainsKey(name))
					return _arguments[name];
				return string.Empty;
			}
		}

		/// <summary>
		/// Determines if the specified named argument exists.
		/// </summary>
		/// <param name="name">The name of the argument to check for.</param>
		/// <returns></returns>
		public bool Exists(string name)
		{
			return _arguments.ContainsKey(name);
		}

		/// <summary>
		/// Returns the named argument as a boolean. Returns false if the argument does not exist.
		/// </summary>
		/// <param name="name">The name of the argument to check for.</param>
		/// <returns></returns>
		public bool ToBoolean(string name)
		{
			if (Exists(name))
				return Convert.ToBoolean(_arguments[name]);
			return false;
		}

		/// <summary>
		/// Returns the named argument as an Int16. Returns 0 if the argument does not exist.
		/// </summary>
		/// <param name="name">The name of the argument to check for.</param>
		/// <returns></returns>
		public short ToInt16(string name)
		{
			if (Exists(name))
				return Convert.ToInt16(_arguments[name]);
			return 0;
		}

		/// <summary>
		/// Returns the named argument as an Int32. Returns 0 if the argument does not exist.
		/// </summary>
		/// <param name="name">The name of the argument to check for.</param>
		/// <returns></returns>
		public int ToInt32(string name)
		{
			if (Exists(name))
				return Convert.ToInt32(_arguments[name]);
			return 0;
		}

		/// <summary>
		/// Returns the named argument as an Int64. Returns 0 if the argument does not exist.
		/// </summary>
		/// <param name="name">The name of the argument to check for.</param>
		/// <returns></returns>
		public long ToInt64(string name)
		{
			if (Exists(name))
				return Convert.ToInt64(_arguments[name]);
			return 0;
		}

		/// <summary>
		/// Returns the named argument as an Single. Returns 0 if the argument does not exist.
		/// </summary>
		/// <param name="name">The name of the argument to check for.</param>
		/// <returns></returns>
		public float ToSingle(string name)
		{			
			if (Exists(name))
				return Convert.ToSingle(_arguments[name]);
			return 0;
		}	
	}
}
