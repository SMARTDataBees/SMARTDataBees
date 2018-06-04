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
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Carbon.Common;
using Carbon.Plugins;

namespace Carbon
{
	/// <summary>
	/// Defines the top-most Carbon class used by executables to host plugins.
	/// This class should be the only referenced and used class in an application's Main
	/// method as far as Carbon calls are concerned.
	/// </summary>
	public static class CarbonRuntime
	{
		/// <summary>
		/// Starts and initializes the Carbon Runtime with a new PluginContext. 
		/// The new PluginContext will be executed in the current AppDomain, which
		/// will create and run a new ApplicationContext. This is the Carbon 
		/// entry point that should be called by the hosting executable.
		/// </summary>
		/// <param name="args">An array of command line arguments</param>
		[STAThread()]
		public static void CreatePluginContext(Assembly assembly, string[] args, bool silent, bool setApplicationDefaults = true)
		{            
			try
			{
                if (setApplicationDefaults)
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(true);
                }

				// create a new plugin context
				using (PluginContext context = new PluginContext())
				{
					// run the application in this context
					context.Run(assembly, args, silent);
				}
			}
			catch(Exception ex)
			{
				Log.WriteLine(ex);
				MessageBox.Show(null, string.Format("Additional Information: {0}", ex.Message) , "Critical Application Exception Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				LogFileManager.Shutdown();
			}
		}
	}
}
