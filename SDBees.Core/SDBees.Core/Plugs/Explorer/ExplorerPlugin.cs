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

using SDBees.Core.Model;

namespace SDBees.Plugs.Explorer
{
    public enum ExplorerMode { eQualification, eSelection, eDefinition, eIssues };

    public interface iExplorer
    {
        PluginContext Context { get; set; }

        SDBees.Core.Connectivity.SDBeesLink.Service.SDBeesExternalPluginService MyPluginService { get; set; }

        SDBeesDataSet MyDataSet { get; set; }

        Dictionary<string, SDBeesEntityDefinition> EntityDefinitions { get; set; }

        System.Windows.Forms.DialogResult ShowDialog(IWin32Window window, bool blockApplication);

        void CloseDialog();

        string GetData(string name);
    }

    public abstract class ExplorerPlugin : TemplateBase.TemplateBase, iExplorer
    {
        public PluginContext Context { get; set; }

        public SDBees.Core.Connectivity.SDBeesLink.Service.SDBeesExternalPluginService MyPluginService { get; set; }

        public abstract ExplorerMode ExplorerMode { get; set; }

        public abstract SDBeesDataSet MyDataSet { get; set; }

        public abstract Dictionary<string, SDBeesEntityDefinition> EntityDefinitions { get; set; }

        public abstract DialogResult ShowDialog(IWin32Window window, bool blockApplication);

        public abstract void CloseDialog();

        public abstract string GetData(string name);

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
