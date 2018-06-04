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
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace Carbon.Common
{
	/// <summary>
	/// Provides access to the command line args passed to the application at startup.    
	/// </summary>
	/// <remarks>
	/// Supports the following switches: "/", "-", "--"
	/// Supports the following switch/value delimiters: "=", ":"
	/// Example: /noui /username=jdoe /password=secret /operation=export /outputfile=exportoutput.xml  
	/// </remarks>
	[System.Diagnostics.DebuggerStepThrough()]
	public sealed class CommandLineArgs 
	{
		private NameValueCollection _args;
        
		/// <summary>
		/// Initializes a new instance of the CommandLineArgs class
		/// </summary>
		public CommandLineArgs()
		{
			_args = new NameValueCollection();            
			this.Parse(System.Environment.GetCommandLineArgs());
		}

		/// <summary>
		/// Initializes a new instance of the CommandLineArgs class
		/// </summary>
		/// <param name="args">An array of string arguments to parse</param>
		public CommandLineArgs(string[] args) 
		{
			_args = new NameValueCollection();
			this.Parse(args);
		}

		/// <summary>
		/// Parses the argument list into a name/value collection
		/// </summary>
		/// <param name="args">The command line args to parse</param>
		private void Parse(string[] args)
		{   
			// <devnote>
			// The following regular expression is the pattern in use. It is here
			// in it's entirety so that it may be copied to a regular expression
			// testing tool such as Expresso for testing.
			// Expression: (^/|^-{1,2})(?<arg>(\w+))(?(=|:)(?<value>(.+)))
			// SampleData: Example: /noui /username=jdoe /password=secret /operation=export /outputfile=exportoutput.xml
			// </devnote>
			const string RegexArgGroupName = "arg"; // this value is in the pattern below as the name of a capture group
			const string RegexValueGroupName = "value"; // this value is in the pattern below as the name of a capture group
			const string RegexPattern = @"(^/|^-{1,2})(?<" + RegexArgGroupName + @">(\w+))(?(=|:)(?<" + RegexValueGroupName + @">(.+)))";                    
                                    
			Regex regex = new Regex(RegexPattern, RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);                            
			foreach (string arg in args)
			{
				// try and match each arg
				Match match = regex.Match(arg);
				while (match.Success)
				{
					// snag the arg group
					Group argGroup = match.Groups[RegexArgGroupName];
					if (argGroup != null)
					{
						// snag the value group
						Group valueGroup  = match.Groups[RegexValueGroupName];
						if (valueGroup != null)
							// finding both is an /arg=value
							_args.Add(argGroup.Value, valueGroup.Value);
						else
							// finding only the arg is something like /arg
							_args.Add(argGroup.Value, string.Empty);
					}
                    
					// go to the next match
					match = match.NextMatch();
				}
			}       
		}

		/// <summary>
		/// Determines if the specified arg exists by name
		/// </summary>
		/// <param name="name">The name of the arg to search for</param>
		/// <returns></returns>
		public bool Exists(string name)
		{
			foreach (string argName in _args.AllKeys)
				if (argName == name) // keep the case-sensitivity
					return true;
			return false;
		}

		/// <summary>
		/// Returns the number of args that were found and parsed from the command line
		/// </summary>
		public int ArgsCount
		{
			get
			{
				return _args.Count;
			}
		}

		/// <summary>
		/// Returns the value for the command line arg at the specified index
		/// </summary>
		public string this[int index]
		{
			get
			{                
				return _args[index];
			}
		}

		/// <summary>
		/// Returns the value for the command line arg with the specified name
		/// </summary>
		public string this[string name]
		{
			get
			{
				return _args[name];
			}
		} 
       
		/// <summary>
		/// Returns the value for the command line arg with the specified name as a string
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string GetArgValue(string name, string defaultValue)
		{
			if (this.Exists(name))
			{
				string value = this[name];
				
				// this is such a complete tarded up hack
				// i know you can replace with regex, just didn't take the time to look it up
				// we'll come back to this a little later and fix this gheyness
				value = value.Trim(new char[] {'"', '=', ':'});
				return value;
			}
			return defaultValue;
		}

		/// <summary>
		/// Returns the value for the command line arg with the specified name as a bool
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool GetArgValueAsBoolean(string name, bool defaultValue)
		{     
			if (this.Exists(name))
				return Convert.ToBoolean(this[name]);
			return defaultValue;
		}

		/// <summary>
		/// Returns the value for the command line arg with the specified name as an int
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public int GetArgValueAsInt32(string name, int defaultValue)
		{
			if (this.Exists(name))
				return Convert.ToInt32(this[name]);
			return defaultValue;
		}
	}
}
