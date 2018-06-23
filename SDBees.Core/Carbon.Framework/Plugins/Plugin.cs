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
using System.Windows.Forms;
using Carbon.Common;

namespace Carbon.Plugins
{
    /// <summary>
    /// Defines the base class from which all Plugin classes must derive.
    /// </summary>
    public abstract class Plugin : DisposableObject
    {
        /// <summary>
        /// The abstract method that must be overriden by derived classes to start plugin functionality
        /// </summary>
        /// <param name="context">The PluginContext that is hosting this Plugin instance.</param>
        /// <param name="e">EventArgs that contain a PluginDescriptor with meta-data about the Plugin instance.</param>
        protected abstract void Start(PluginContext context, PluginDescriptorEventArgs e);

        /// <summary>
        /// The abstract method that must be overriden by derived classes to stop plugin functionality
        /// </summary>
        /// <param name="context">The PluginContext that is hosting this Plugin instance.</param>
        /// <param name="e">EventArgs that contain a PluginDescriptor with meta-data about the Plugin instance.</param>
        protected abstract void Stop(PluginContext context, PluginDescriptorEventArgs e);

        /// <summary>
        /// Calls the Start method of the Plugin class.
        /// </summary>
        /// <param name="context">The PluginContext that is hosting this Plugin instance.</param>
        /// <param name="e">EventArgs that contain a PluginDescriptor with meta-data about the Plugin instance.</param>
        internal void OnStart(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                Log.WriteLine("Starting Plugin, Plugin: '{0}'.", e.Descriptor.PluginName);

                // inform the plugin that it should start its services
                Start(context, e);

                Application.DoEvents();

                // fire the PluginStarted event of the PluginContext
                context.OnPluginStarted(e);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
            }
        }

        /// <summary>
        /// Calls the Stop method of the Plugin class.
        /// </summary>
        /// <param name="context">The PluginContext that is hosting this Plugin instance.</param>
        /// <param name="e">EventArgs that contain a PluginDescriptor with meta-data about the Plugin instance.</param>
        internal void OnStop(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                Log.WriteLine("Stopping Plugin, Plugin: '{0}'.", e.Descriptor.PluginName);

                // inform the plugin that it should stop its services
                Stop(context, e);

                // fire the PluginStopped event of the PluginContext
                context.OnPluginStopped(e);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex);
            }
        }
    }
}
