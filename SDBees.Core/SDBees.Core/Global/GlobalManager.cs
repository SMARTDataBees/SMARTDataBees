// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2016 by
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.DB;
using SDBees.Plugs.TemplateBase;
using System.Configuration;
using System.Reflection;

namespace SDBees.Core.Global
{
    [PluginName("GlobalManager Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the global management of SmartDataBees")]
    [PluginId("16A2DC1C-0516-4CF0-AE5A-796261A60B39")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]

    public class GlobalManager : SDBees.Plugs.TemplateBase.TemplatePlugin
    {
        private static GlobalManager m_theInstance;
        private Configuration m_config = null;

        public GlobalManager():base()
        {
            m_theInstance = this;
        }

        /// <summary>
        /// Returns the one and only DataBaseManagerPlugin instance.
        /// </summary>
        public static GlobalManager Current
        {
            get
            {
                return m_theInstance;
            }
        }

        public Configuration Config
        {
            get
            {
                return m_config;
            }

            set
            {
                m_config = value;
            }
        }

        public override TemplateDBBaseData CreateDataObject()
        {
            return null;
        }

        public override TemplatePlugin GetPlugin()
        {
            return m_theInstance;
        }

        public override Table MyTable()
        {
            return null;
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            //
        }

        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            //
            try
            {
                Assembly assem = Assembly.GetEntryAssembly();
                Config = ConfigurationManager.OpenExeConfiguration(assem.Location);
            }
            catch (Exception ex)
            {

            }
        }

        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            //
        }
    }
}
