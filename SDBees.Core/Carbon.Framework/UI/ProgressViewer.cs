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
using System.Windows.Forms;
using Carbon.Common;

namespace Carbon.UI
{
	/// <summary>
	/// Defines helper methods for working with any class or control that implements the IProgressViewer interface.
	/// Methods are ui-thread safe (ie. calls will be marshalled to the control's thread if the implementor inherits from control)
	/// </summary>
	[DebuggerStepThrough]
	public sealed class ProgressViewer : DisposableObject, IProgressViewer
	{
		private delegate void SetTextEventHandler(IProgressViewer viewer, string text);
		private delegate void SetImageEventHandler(IProgressViewer viewer, Image image);
		private delegate void SetMarqueeMovingEventHandler(IProgressViewer viewer, bool moving, bool reset);

		/// <summary>
		/// Initializes a new instance of the ProgressViewer class
		/// </summary>
		internal ProgressViewer() {}
		
		#region IProgressViewer Members

		void IProgressViewer.SetTitle(string text)
		{
			Log.WriteLine(text);
		}

		void IProgressViewer.SetHeading(string text)
		{
			Log.WriteLine(text);
		}

		void IProgressViewer.SetDescription(string text)
		{
			Log.WriteLine(text);
		}

		void IProgressViewer.SetExtendedDescription(string text)
		{
			Log.WriteLine(text);
		}

		void IProgressViewer.SetImage(Image image)
		{
			// TODO:  Add ProgressViewer.Carbon.UI.IProgressViewer.SetImage implementation
		}

		public void SetMarqueeMoving(bool moving, bool reset)
		{
			// TODO:  Add ProgressViewer.SetMarqueeMoving implementation
		}

		#endregion

		#region My Static Methods

		/// <summary>
		/// Sets the progress viewer's text
		/// </summary>
		/// <param name="viewer">The progress viewer to manipulate</param>
		/// <param name="text">The text to display</param>
		public static void SetTitle(IProgressViewer viewer, string text)
		{
			try
			{
				if (viewer != null)
				{
					var control = viewer as Control;
					if (control != null)
					{
						if (control.InvokeRequired)
						{
							control.Invoke(new SetTextEventHandler(SetTitle), viewer, text);
							return;
						}
					}
					viewer.SetTitle(text);				
				}
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		/// <summary>
		/// Sets the progress viewer's heading
		/// </summary>
		/// <param name="viewer">The progress viewer to manipulate</param>
		/// <param name="text">The text to display</param>
		public static void SetHeading(IProgressViewer viewer, string text)
		{
			try
			{
				if (viewer != null)
				{
					var control = viewer as Control;
					if (control != null)
					{
						if (control.InvokeRequired)
						{
							control.Invoke(new SetTextEventHandler(SetHeading), viewer, text);
							return;
						}
					}
					viewer.SetHeading(text);
				}
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		/// <summary>
		/// Sets the progress viewer's description
		/// </summary>
		/// <param name="viewer">The progress viewer to manipulate</param>
		/// <param name="text">The text to display</param>
		public static void SetDescription(IProgressViewer viewer, string text)
		{
			try
			{
				if (viewer != null)
				{
					var control = viewer as Control;
					if (control != null)
					{
						if (control.InvokeRequired)
						{
							control.Invoke(new SetTextEventHandler(SetDescription), viewer, text);
							return;
						}
					}
					viewer.SetDescription(text);
				}
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		/// <summary>
		/// Sets the progress viewer's extended description
		/// </summary>
		/// <param name="viewer">The progress viewer to manipulate</param>
		/// <param name="text">The text to display</param>
		public static void SetExtendedDescription(IProgressViewer viewer, string text)
		{
			try
			{
				if (viewer != null)
				{
					var control = viewer as Control;
					if (control != null)
					{
						if (control.InvokeRequired)
						{
							control.Invoke(new SetTextEventHandler(SetExtendedDescription), viewer, text);
							return;
						}
					}
					viewer.SetExtendedDescription(text);
				}
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		/// <summary>
		/// Sets the progress viewer's image
		/// </summary>
		/// <param name="viewer">The progress viewer to manipulate</param>
		/// <param name="image">The image to display</param>
		public static void SetImage(IProgressViewer viewer, Image image)
		{
			try
			{
				if (viewer != null)
				{
					var control = viewer as Control;
					if (control != null)
					{
						if (control.InvokeRequired)
						{
							control.Invoke(new SetImageEventHandler(SetImage), viewer, image);
							return;
						}
					}
					viewer.SetImage(image);
				}
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		/// <summary>
		/// Sets the progress viewer's marquee state
		/// </summary>
		/// <param name="viewer">The progress viewer to manipulate</param>
		/// <param name="moving">A flag that determines if the marquee is moving or stopped</param>
		/// <param name="reset">A flag that determines if the marquee should be reset</param>
		public static void SetMarqueeMoving(IProgressViewer viewer, bool moving, bool reset)
		{
			try
			{
				if (viewer != null)
				{
					var control = viewer as Control;
					if (control != null)
					{
						if (control.InvokeRequired)
						{
							control.Invoke(new SetMarqueeMovingEventHandler(SetMarqueeMoving), viewer, moving, reset);
							return;
						}
					}					
					viewer.SetMarqueeMoving(moving, reset);
				}
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
			}
		}

		#endregion
	}
}
