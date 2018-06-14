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

using System.Drawing;

namespace Carbon.UI
{	
	/// <summary>
	/// Defines an interface that must be implemented by all ProgressViewers
	/// </summary>
	public interface IProgressViewer
	{
		/// <summary>
		/// Sets the progress viewer's text
		/// </summary>
		/// <param name="text">The text to display</param>
		void SetTitle(string text);

		/// <summary>
		/// Sets the progress viewer's heading
		/// </summary>
		/// <param name="text">The text to display</param>
		void SetHeading(string text);

		/// <summary>
		/// Sets the progress viewer's description
		/// </summary>
		/// <param name="text">The text to display</param>
		void SetDescription(string text);		

		/// <summary>
		/// Sets the progress viewer's extended description
		/// </summary>
		/// <param name="text">The text to display</param>
		void SetExtendedDescription(string text);					

		/// <summary>
		/// Sets the progress viewer's image
		/// </summary>
		/// <param name="image">The image to display</param>
		void SetImage(Image image);

		/// <summary>
		/// Sets the progress viewer's marquee state
		/// </summary>
		/// <param name="moving">A flag that determines if the marquee is moving or stopped</param>
		/// <param name="reset">A flag that determines if the marquee should be reset</param>
		void SetMarqueeMoving(bool moving, bool reset);
	}
}
