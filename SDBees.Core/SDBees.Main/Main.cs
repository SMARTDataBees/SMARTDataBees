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
using System.Reflection;
using System.Windows.Forms;
using System.Threading;
//using Razor.SnapIns;
using Carbon;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using System.IO;

namespace SDBees.Main
{
    public class Startup
    {
        private static string appGuid = "24EC194D-C1F1-41BD-A2F9-1740B84E9293";
        [STAThread()]
        public static void Main(string[] args)
        {
            // prevent second start of exe, to avoid database problems
            //using (Mutex mutex = new Mutex(false, @"Global\"+ appGuid))
            using (Mutex mutex = new Mutex(false, appGuid))
            {
                if(!mutex.WaitOne(0, false))
                {
                    //MessageBox.Show("Test");
                    return;
                }

                //Copy the DemoData
                CopyDemoDataset();

                // use the carbon runtime to create a plugin host for this application
                CarbonRuntime.CreatePluginContext(Assembly.GetExecutingAssembly(), args, false);
            }
        }

        private static void CreateServerConfig()
        {
            SDBees.DB.Generic.ServerConfig conf = SDBees.DB.Generic.ServerConfigHandler.LoadConfig(false);
            if (conf != null && conf.ConfigItems.Count == 0)
            {
                DB.Generic.ServerConfigItem item = new DB.Generic.ServerConfigItem();

                item.ProjectName = m_DirDemo;
                item.ProjectDescription = "Some demofiles for SMARTDataBees tests";
                item.ServerDatabasePath = GetTargetDir() + "\\" + m_DirDemo + m_dbExtension;
                item.ServerTableCaching = false;
                item.ServerType = DB.Generic.Servers.SQLite;
                item.ConfigItemName = m_DirDemo;

                conf.ConfigItems.Add(item);

                SDBees.DB.Generic.ServerConfigHandler.SaveConfig(conf);
            }
        }

        static string m_DirDemo = "DemoDataSMARTDataBees"; //Don't change this, or modify also postbuild in SDBees.Main!!
        static string m_dbExtension = ".s3db";

        private static void CopyDemoDataset()
        {
#if false
            DirectoryInfo dirSource = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(typeof(SDBees.Main.Startup).Assembly.Location), "Content\\" + m_DirDemo));
            if (dirSource.Exists)
            {
                DirectoryInfo dirTarget = GetTargetDir();
                if (!dirTarget.Exists)
                {
                    dirTarget.Create();
                    SDBees.Utils.DirectoryTools.DirectoryCopy(dirSource.FullName, dirTarget.FullName, true);
                }
            }
#endif
        }

        private static DirectoryInfo GetTargetDir()
        {
            DirectoryInfo dirTarget = new DirectoryInfo(Path.Combine(DB.Generic.ServerConfigHandler.GetConfigStorageFolder(), m_DirDemo));
            return dirTarget;
        }
    }
}
