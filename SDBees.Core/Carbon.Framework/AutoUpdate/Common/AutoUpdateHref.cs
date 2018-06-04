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

namespace Carbon.AutoUpdate.Common
{
	/// <summary>
	/// Provides a text and href combination used to provide a link to follow to get
	/// more information about an update.
	/// </summary>
	public sealed class AutoUpdateHref
	{
		private string _text;
		private string _href;		

		/// <summary>
		/// Initializes a new instance of the AutoUpdateHref class.
		/// </summary>
		public AutoUpdateHref()
		{
			
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateHref class.
		/// </summary>
		/// <param name="text">The text that will be displayed for the link.</param>
		/// <param name="href">The url that will be assigned to the href of the link.</param>
		public AutoUpdateHref(string text, string href)
		{
			_text = text;
			_href = href;
		}

		/// <summary>
		/// Gets or sets the text that will be displayed for the link.
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		/// <summary>
		/// Gets or sets the url that will be assigned to the href of the link.
		/// </summary>
		public string Href
		{
			get
			{
				return _href;
			}
			set
			{
				_href = value;
			}
		}
	}
}
