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

namespace Carbon.Configuration
{	
	/// <summary>
	/// EventArgs class for the XmlConfigurationCategoryEventHandler delegate
	/// </summary>
	public class XmlConfigurationCategoryEventArgs : XmlConfigurationElementEventArgs 
	{
		/// <summary>
		/// Initializes a new instance of the XmlConfigurationOptionEventArgs class
		/// </summary>
		/// <param name="option">The option being affected by this action</param>
		/// <param name="action">The action affecting this option</param>
		public XmlConfigurationCategoryEventArgs(XmlConfigurationCategory category, XmlConfigurationElementActions action) : base(category, action)
		{
			
		}

		/// <summary>
		/// Gets the option affected by this event
		/// </summary>
		public new XmlConfigurationCategory Element
		{
			get
			{
				return (XmlConfigurationCategory)base.Element;
			}
			set
			{
				base.Element = value;
			}
		}
	}

	/// <summary>
	/// Delegate for the XmlConfigurationCategoryEventArgs class
	/// </summary>
	public delegate void XmlConfigurationCategoryEventHandler(object sender, XmlConfigurationCategoryEventArgs e);
}
