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

namespace SDBees.Main.Window
{
    /// <summary>
    /// This class helps showing progress when lengthy tasks are active. It works with the MainWindow Dialog.
    /// </summary>
    public class ProgressTool
    {
        #region Private Data Members

        private MainWindowApplicationDLG m_mainWindowDialog;
        private int m_activeProcessCount;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get the Statusbar
        /// </summary>
        public ToolStripProgressBar ProgressBar
        {
            get
            {
                if (m_mainWindowDialog != null)
                {
                    return m_mainWindowDialog.ProgressBar;
                }

                return null;
            }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ProgressTool()
        {
            m_activeProcessCount = 0;
            m_mainWindowDialog = MainWindowApplication.Current.TheDialog;
        }

        ~ProgressTool()
        {
            while (m_activeProcessCount > 0)
            {
                EndActiveProcess();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// A lengthy process will start, so setup the main window to display the wait cursor and progress bar
        /// </summary>
        /// <param name="displayWaitCursor">If true the wait cursor will be displayed</param>
        /// <param name="activateProgressbar">If true the progress bar will apear</param>
        public void StartActiveProcess(bool displayWaitCursor, bool activateProgressbar)
        {
            if (m_activeProcessCount == 0)
            {
                // Do we have to do something here???
            }

            if (activateProgressbar)
            {
                m_mainWindowDialog.ProgressBar.Value = m_mainWindowDialog.ProgressBar.Minimum;
                m_mainWindowDialog.ProgressBar.Visible = true;
            }

            m_activeProcessCount++;
        }

        /// <summary>
        /// The active process has been terminated so show this in the main window. Nested start/end are possible.
        /// </summary>
        public void EndActiveProcess()
        {
            if (m_activeProcessCount == 1)
            {
                m_mainWindowDialog.WriteStatus("Bereit.");
                m_mainWindowDialog.ProgressBar.Visible = false;

                m_activeProcessCount = 0;
            }
            else if (m_activeProcessCount > 1)
            {
                m_activeProcessCount--;
            }
            else
            {
                MessageBox.Show("EndActiveProcess invalid call!");

                m_activeProcessCount = 0;
            }
        }

        /// <summary>
        /// Write a message to the status bar of the main window.
        /// </summary>
        /// <param name="message"></param>
        public void WriteStatus(string message)
        {
            m_mainWindowDialog.WriteStatus(message);
        }

        #endregion

        #region Protected Methods

        #endregion
    }
}
