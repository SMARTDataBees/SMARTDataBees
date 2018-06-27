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
using System.Configuration;
using System.Reflection;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.DB;
using SDBees.Plugs.TemplateBase;

namespace SDBees.Core.Global
{
    [PluginName("GlobalManager Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the global management of SmartDataBees")]
    [PluginId("16A2DC1C-0516-4CF0-AE5A-796261A60B39")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]

    public class GlobalManager : TemplatePlugin
    {
        private static GlobalManager _instance;
        private Configuration _configuration;

        public GlobalManager()
        {
            _instance = this;
        }

        /// <summary>
        /// Returns the one and only DataBaseManagerPlugin instance.
        /// </summary>
        public static GlobalManager Current => _instance;

        public Configuration Configuration
        {
            get => _configuration;
            set => _configuration = value;
        }

        /// <inheritdoc />
        public override TemplateDBBaseData CreateDataObject() => null;

        /// <inheritdoc />
        public override TemplatePlugin GetPlugin() => _instance;

        /// <inheritdoc />
        public override Table MyTable() => null;

        /// <inheritdoc />
        protected override void OnDatabaseChanged(object sender, EventArgs eventArgs)
        {           
        }

        /// <inheritdoc />
        protected override void Start(PluginContext context, PluginDescriptorEventArgs eventArgs)
        {
            //
            try
            {
                var assem = Assembly.GetEntryAssembly();
                Configuration = ConfigurationManager.OpenExeConfiguration(assem.Location);
            }
            catch (Exception ex)
            {

            }
        }

        /// <inheritdoc />
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs eventArgs)
        {
        }
    }
}
