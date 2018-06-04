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
using System.Diagnostics;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Runtime.InteropServices;

/*
using Razor;
using Razor.Attributes;
using Razor.Configuration;
using Razor.Features;
using Razor.SnapIns;
using Razor.SnapIns.WindowPositioningEngine;
 * */
using Carbon.Common;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;

using SDBees.Plugs.Events;
using SDBees.GuiTools;

namespace SDBees.Main.Window
{
    [PluginName("Main Window Plugin")]
    [PluginAuthors("Tim Hoffeller")]
    [PluginDescription("Plugin for the main Window of SmartDataBees")]
    [PluginId("9054FC0C-9225-499A-8D5B-A98818EE21B1")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

    public sealed class MainWindowApplication : SDBees.Plugs.TemplateBase.TemplateBase
    {
        private static MainWindowApplication _theInstance;
        private MainWindowApplicationDLG m_window;
        private bool _windowClosed;
        private bool m_noPromptsOnClose;

        public PluginContext MyContext;

        #region Interop

        private const uint WM_CLOSE = 0x0010;
        private const int TRUE = 1;
        private const int FALSE = 0;

        [DllImport(@"User32.dll")]
        private static extern int PostMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(@"User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        [DllImport(@"User32.dll")]
        private static extern int IsWindowVisible(IntPtr hWnd);

        #endregion

        /// <summary>
        /// Returns the one and only SystemTrayApplicationPlugin instance.
        /// </summary>
        public static MainWindowApplication Current
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindowApplication()
            : base()
        {
            _theInstance = this;
            _windowClosed = true;
        }


        /// <summary>
        /// Occurs when the main window plugin starts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                this.StartMe(context, e);

                //Store the context for internal use
                MyContext = context;

                // create a hidden window to run the app's msg pump
                m_window = new MainWindowApplicationDLG(this);

                m_window.Closed += new EventHandler(OnWindowClosed);
                //context.RestartPending += new EventHandler<PluginContextEventArgs>(OnPluginContextRestartPending);

                MyContext.AfterPluginsStarted += new EventHandler<PluginContextEventArgs>(OnAllPluginsStarted);
                MyContext.BeforePluginsStopped += new EventHandler<PluginContextEventArgs>(OnAllPluginsStopp);

                //_window.Closed += new EventHandler(OnWindowClosed);

                //context.ApplicationContext.MainForm = _window;

                Console.WriteLine("Main Window starts\n");

                if (MyContext.SplashWindow != null)
                {
                    MyContext.SplashWindow.Hide();
                }

                #region Version mit Delegate
                /*
        //Das Hauptfester anzeigen
        // create a delegate of MethodInvoker poiting to
        // our Foo function.
        MethodInvoker simpleDelegate = new MethodInvoker(StartMainDialog);

        simpleDelegate.BeginInvoke(null, null);
         * */
                #endregion

                #region Version mit normalem ShowDialog()
                //_window.ShowDialog();
                #endregion

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            //this.CloseHiddenWindow();

            if (_windowClosed)
            {
                Console.WriteLine("Main Window stops\n");
            }
        }

        #region Properties

        /// <summary>
        /// Returns the Dialog
        /// </summary>
        public MainWindowApplicationDLG TheDialog
        {
            get { return this.m_window.MyForm; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occures when the context stops and alle plugins are forced to shutdown
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAllPluginsStopp(object sender, PluginContextEventArgs e)
        {
            //MyContext.SplashWindow.Show();
            //MyContext.SplashWindow.Focus();
            //MyContext.SplashWindow.BringToFront();

            //throw new Exception("The method or operation is not implemented.");
        }

        private void OnWindowClosed(object sender, EventArgs e)
        {
            _windowClosed = true;
            try
            {
                SDBees.Core.Connectivity.ConnectivityManager.Current.OnClose();
            }
            catch (Exception ex)
            {
            }

        }

        private void OnAllPluginsStarted(object sender, EventArgs e)
        {
            bool doStartMainDialog = true;

            foreach (string arg in MyContext.CommandLineArgs)
            {
                if (arg.CompareTo("-show-dialog=false") == 0)
                {
                    doStartMainDialog = false;

                    break;
                }
            }

            SDBees.Core.Connectivity.ConnectivityManager.Current.Ready = true;

            if (doStartMainDialog)
            {
                this.StartMainDialog();
            }
        }

        public void StartMainDialog()
        {
            try
            {
                //MyContext.ApplicationContext.AddTopLevelWindow(_window);
                m_window.ShowDialog();
                //MyContext.ApplicationContext.RemoveTopLevelWindow(_window);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            InitDatabase();
        }

        private void InitDatabase()
        {
            if(MyDBManager != null)
            { }
        }

        public Icon GetApplicationIcon()
        {
            Icon ic = null;

            string _iconPath = GetApplicationIconPath();
            if (!String.IsNullOrEmpty(_iconPath))
            {
                string path = Path.GetDirectoryName(this.GetType().Assembly.Location);
                string full = path + _iconPath;
                FileInfo fi = new FileInfo(full);
                if (fi.Exists)
                    ic = new Icon(fi.FullName);
            }

            return ic;
        }

        public string GetApplicationIconPath()
        {
            string path = System.Configuration.ConfigurationManager.AppSettings["MainWindowIcon"];
            return path;
        }

        public string GetApplicationTitle()
        {
            string _title = System.Configuration.ConfigurationManager.AppSettings["MainWindowTitle"];
            return _title;
        }

        public string GetLicenseFileLocation(string dirctory)
        {
            string _file = "";
            return _file;
        }

        public void ShowWebpage()
        {
            //Load expected window title
            string _webpage = System.Configuration.ConfigurationManager.AppSettings["Webpage"];
            if (!String.IsNullOrEmpty(_webpage))
            {
                Process.Start(_webpage);
            }
        }

        public void ShowInfo()
        {
            Assembly assembly1 = Assembly.GetExecutingAssembly();
            Assembly assembly2 = Assembly.GetEntryAssembly();

            AboutBox dlg = new AboutBox(assembly2);

            dlg.ShowDialog();
        }
    }
}
