using System;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Carbon.Plugins;
using SDBees.Core.Connectivity;
using SDBees.Core.Properties;
using SDBees.GuiTools;
using SDBees.Main.Window;

namespace SDBees.Core.Main.Systemtray
{
	/// <summary>
	/// 
	/// </summary>
    public class ContextMenus : ContextMenuStrip
	{
        public ContextMenus(PluginContext context)
        {
            MyContext = context;
            Create();
        }

        public PluginContext MyContext;
		/// <summary>
		/// Is the About box displayed?
		/// </summary>
		bool isAboutLoaded;
        ToolStripMenuItem m_mainWindowItem;

        public ToolStripMenuItem MainWindowItem
        {
            get { return m_mainWindowItem; }
            set { m_mainWindowItem = value; }
        }

        MainWindowApplication m_MainWindow;
        ConnectivityManager m_ConnManager;

		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns>ContextMenuStrip</returns>
		internal void Create()
		{
            //MyContext = context;

            //Das MainWindowplugin besorgen
            if (MyContext.PluginDescriptors.Contains(new PluginDescriptor(typeof(MainWindowApplication))))
            {
                m_MainWindow = (MainWindowApplication)MyContext.PluginDescriptors[typeof(MainWindowApplication)].PluginInstance;
            }
            else
            {
                MessageBox.Show("Es konnte kein Hauptfenster gefunden werden!", ToString());
                m_MainWindow = null;
            }

            //Den ConnectivityManager besorgen
            if (MyContext.PluginDescriptors.Contains(new PluginDescriptor(typeof(ConnectivityManager))))
            {
                m_ConnManager = (ConnectivityManager)MyContext.PluginDescriptors[typeof(ConnectivityManager)].PluginInstance;
            }
            else
            {
                MessageBox.Show("Es konnte kein ConnectivityManager gefunden werden!", ToString());
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
            m_mainWindowItem.Click += MainWindow_Click;
            m_mainWindowItem.Image = Resources.SDBees;
            try
            {
                var _path = Path.GetDirectoryName(GetType().Assembly.Location) + ConfigurationManager.AppSettings["MainWindowIcon"];
                m_mainWindowItem.Image = Image.FromFile(_path);
            }
            catch (Exception ex)
            {
            }

            Items.Add(m_mainWindowItem);

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
			item.Click += Exit_Click;
			item.Image = Resources.Exit;
			Items.Add(item);
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
                    var myMainWindowHandle = MainWindowApplication.Current.TheDialog.Handle;

                    var caption = MainWindowApplication.Current.GetApplicationTitle();

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
                MainWindowApplication.Current.StartMainDialog();
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
				new AboutBox(GetType().Assembly).ShowDialog();
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
            var exitHandler = new ExitHandler(m_ConnManager);

            exitHandler.run();
		}

        private class ExitHandler
        {
            private ConnectivityManager m_connectionManager;

            private Timer m_timer;

            public ExitHandler(ConnectivityManager connectionManager)
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
                            var myMainWindowHandle = MainWindowApplication.Current.TheDialog.Handle;

                            var caption = MainWindowApplication.Current.GetApplicationTitle();

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
                m_timer = new Timer();

                m_timer.Tick += handleTimer;

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