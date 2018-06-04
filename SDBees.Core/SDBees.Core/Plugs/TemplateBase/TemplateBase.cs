// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2007 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// SMARTDataBees is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SMARTDataBees is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SMARTDataBees.  If not, see <http://www.gnu.org/licenses/>.
//
// #EndHeader# ================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Carbon;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;

using SDBees.Main.Window;
using SDBees.DB;
using System.Collections;

namespace SDBees.Plugs.TemplateBase
{
    public abstract class TemplateBase : Carbon.Plugins.Plugin
    {
        private SDBees.DB.SDBeesDBConnection _DBManager = null;
        private MainWindowApplication _MainWindow = null;
        private PluginContext m_context = null;

        public PluginContext MyPluginContext
        {
            get { return m_context; }
            set { m_context = value; }
        }

        public TemplateBase()
            : base()
        {
        }

        public void ReStart(PluginContext context)
        {
            Start(context, null);
        }

        /// <summary>
        /// Your have to call this, to ensure that the DBManager and Mainwindow are set up correctly
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        public void StartMe(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                this.m_context = context;

                Console.WriteLine("TemplateBase starts\n");

                //Das Databaseplugin besorgen
                if (this.m_context.PluginDescriptors.Contains(new PluginDescriptor(typeof(SDBeesDBConnection))))
                {
                    _DBManager = (SDBeesDBConnection)this.m_context.PluginDescriptors[typeof(SDBees.DB.SDBeesDBConnection)].PluginInstance;
                    _DBManager.AddDatabaseChangedHandler(OnDatabaseChanged);
                    _DBManager.AddUpdateHandler(OnUpdate);
                }
                else
                {
                    MessageBox.Show("Es konnte kein Datenbankmanager gefunden werden!", this.ToString());
                    _DBManager = null;
                }

                //Das MainWindowplugin besorgen
                if (this.m_context.PluginDescriptors.Contains(new PluginDescriptor(typeof(SDBees.Main.Window.MainWindowApplication))))
                {
                    _MainWindow = (MainWindowApplication)this.m_context.PluginDescriptors[typeof(MainWindowApplication)].PluginInstance;
                }
                else
                {
                    MessageBox.Show("Es konnte kein Hauptfenster gefunden werden!", this.ToString());
                    _MainWindow = null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                //throw;
            }
        }

        public SDBees.DB.SDBeesDBConnection MyDBManager
        {
            get { return this._DBManager; }
        }

        public SDBees.Main.Window.MainWindowApplication MyMainWindow
        {
            get { return this._MainWindow; }
        }

        // Must be overridden by derived class!
        protected abstract void OnDatabaseChanged(object sender, EventArgs e);

        // Can be overridden by derived class!
        protected virtual void OnUpdate(object sender, EventArgs e) { /* empty */ }
    }
}
