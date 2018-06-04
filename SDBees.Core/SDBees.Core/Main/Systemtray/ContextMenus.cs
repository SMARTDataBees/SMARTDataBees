using System;
using System.Diagnostics;
using System.Windows.Forms;
using SDBees.Core.Properties;
using System.Drawing;
using Carbon.Plugins;
using System.IO;

namespace SDBees.Core.Main.Systemtray
{
	/// <summary>
	/// 
	/// </summary>
    public class ContextMenus : ContextMenuStrip
	{
        public ContextMenus(PluginContext context) : base()
        {
            MyContext = context;
            Create();
        }

        public PluginContext MyContext;
		/// <summary>
		/// Is the About box displayed?
		/// </summary>
		bool isAboutLoaded = false;
        ToolStripMenuItem m_mainWindowItem = null;

        public ToolStripMenuItem MainWindowItem
        {
            get { return m_mainWindowItem; }
            set { m_mainWindowItem = value; }
        }

        SDBees.Main.Window.MainWindowApplication m_MainWindow;
        SDBees.Core.Connectivity.ConnectivityManager m_ConnManager;

		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns>ContextMenuStrip</returns>
		internal void Create()
		{
            //MyContext = context;

            //Das MainWindowplugin besorgen
            if (MyContext.PluginDescriptors.Contains(new PluginDescriptor(typeof(SDBees.Main.Window.MainWindowApplication))))
            {
                m_MainWindow = (SDBees.Main.Window.MainWindowApplication)MyContext.PluginDescriptors[typeof(SDBees.Main.Window.MainWindowApplication)].PluginInstance;
            }
            else
            {
                MessageBox.Show("Es konnte kein Hauptfenster gefunden werden!", this.ToString());
                m_MainWindow = null;
            }

            //Den ConnectivityManager besorgen
            if (MyContext.PluginDescriptors.Contains(new PluginDescriptor(typeof(Core.Connectivity.ConnectivityManager))))
            {
                m_ConnManager = (Core.Connectivity.ConnectivityManager)MyContext.PluginDescriptors[typeof(Core.Connectivity.ConnectivityManager)].PluginInstance;
            }
            else
            {
                MessageBox.Show("Es konnte kein ConnectivityManager gefunden werden!", this.ToString());
                m_ConnManager = null;
            }

			// Add the default menu options.
            //ContextMenuStrip menu = new ContextMenuStrip();
			ToolStripMenuItem item;
			ToolStripSeparator sep;

            //// Windows Explorer.
            //item = new ToolStripMenuItem();
            //item.Text = "Explorer";
            //item.Click += new EventHandler(Explorer_Click);
            //item.Image = Resources.Explorer;
            //menu.Items.Add(item);

            // Main window SDBees
            m_mainWindowItem = new ToolStripMenuItem();
            m_mainWindowItem.Text = "Show main window";
            m_mainWindowItem.Click += new EventHandler(MainWindow_Click);
            m_mainWindowItem.Image = Resources.SDBees;
            try
            {
                string _path = Path.GetDirectoryName(this.GetType().Assembly.Location) + System.Configuration.ConfigurationManager.AppSettings["MainWindowIcon"];
                m_mainWindowItem.Image = System.Drawing.Image.FromFile(_path);
            }
            catch (Exception ex)
            {
            }

            this.Items.Add(m_mainWindowItem);

#if false
			// About.
			item = new ToolStripMenuItem();
			item.Text = "About";
			item.Click += new EventHandler(About_Click);
			item.Image = Resources.About;
			this.Items.Add(item);

			// Separator.
			sep = new ToolStripSeparator();
			this.Items.Add(sep);
#endif

            // Exit.
			item = new ToolStripMenuItem();
			item.Text = "Exit";
			item.Click += new System.EventHandler(Exit_Click);
			item.Image = Resources.Exit;
			this.Items.Add(item);
		}

        /// <summary>
        /// Handles the Click event of the Explorer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void MainWindow_Click(object sender, EventArgs e)
        {
            if (m_ConnManager != null)
            {
                if (m_ConnManager.IsEditDataSet())
                {
                    IntPtr myMainWindowHandle = SDBees.Main.Window.MainWindowApplication.Current.TheDialog.Handle;

                    string caption = SDBees.Main.Window.MainWindowApplication.Current.GetApplicationTitle();

                    MessageBox.Show(new JtWindowHandle(myMainWindowHandle), "Cannot close application while connections are still open.", caption, MessageBoxButtons.OK, MessageBoxIcon.Information);

                    return;
                }
            }

            if(m_MainWindow != null && !m_MainWindow.TheDialog.Visible)
            { 
                //m_MainWindow.StartMainDialog(); 
                DoClickMainDialog();
            }
        }

        private void DoClickMainDialog()
        {
            try
            {
                SDBees.Main.Window.MainWindowApplication.Current.StartMainDialog();
            }
            catch (Exception ex)
            {
            }
        }

		/// <summary>
		/// Handles the Click event of the Explorer control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Explorer_Click(object sender, EventArgs e)
		{
			Process.Start("explorer", null);
		}

		/// <summary>
		/// Handles the Click event of the About control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void About_Click(object sender, EventArgs e)
		{
			if (!isAboutLoaded)
			{
				isAboutLoaded = true;
				new SDBees.GuiTools.AboutBox(this.GetType().Assembly).ShowDialog();
				isAboutLoaded = false;
			}
		}

		/// <summary>
		/// Processes a menu item.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void Exit_Click(object sender, EventArgs e)
		{
            ExitHandler exitHandler = new ExitHandler(m_ConnManager);

            exitHandler.run();
		}

        private class ExitHandler
        {
            private Connectivity.ConnectivityManager m_connectionManager = null;

            private System.Windows.Forms.Timer m_timer = null;

            public ExitHandler(Connectivity.ConnectivityManager connectionManager)
            {
                m_connectionManager = connectionManager;
            }

            public void run()
            {
                if (m_connectionManager != null)
                {
                    if (0 < m_connectionManager.ConnectedClients.Count)
                    {
                        if (!m_connectionManager.IsEditDataSet())
                        {
                            activateTimer();

                            m_connectionManager.OnClose();
                        }
                        else
                        {
                            IntPtr myMainWindowHandle = SDBees.Main.Window.MainWindowApplication.Current.TheDialog.Handle;

                            string caption = SDBees.Main.Window.MainWindowApplication.Current.GetApplicationTitle();

                            MessageBox.Show(new JtWindowHandle(myMainWindowHandle), "Cannot close application while connections are still open.", caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        exit();
                    }
                }
            }

            private void tryExit()
            {
                if (m_connectionManager.ConnectedClients.Count == 0)
                {
                    deactivateTimer();

                    exit();
                }
            }

            private void exit()
            {
                Application.Exit();
            }

            private void handleTimer(object sender, EventArgs e)
            {
                tryExit();
            }

            private void activateTimer()
            {
                m_timer = new System.Windows.Forms.Timer();

                m_timer.Tick += new EventHandler(handleTimer);

                m_timer.Interval = 200; // 200 ms

                m_timer.Enabled = true;
            }

            private void deactivateTimer()
            {
                m_timer.Enabled = false;
            }
        }

        private class JtWindowHandle : IWin32Window
        {
            IntPtr _hwnd;

            public JtWindowHandle(IntPtr h)
            {
                _hwnd = h;
            }

            public IntPtr Handle
            {
                get
                {
                    return _hwnd;
                }
            }
        }
    }
}