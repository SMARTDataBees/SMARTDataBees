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

using System.Collections;
using System.Windows.Forms;

namespace SDBees.DB
{
    /// <summary>
    /// Base class of all database inspectors, that inspect, fix and cleanup a database
    /// </summary>
    public abstract class Inspector
    {
        #region Private Data Members

        private SDBeesDBConnection m_dbManager;
        private bool mAutomaticFix;
        private bool mDeleteUnreferenced;
        private ToolStripProgressBar mProgressBar;
        private TextBox mMessageBox;

        private static Hashtable gRegisteredInspectors;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/set the database to inspect
        /// </summary>
        public Database Database
        {
            get { return m_dbManager.Database; }
        }

        /// <summary>
        /// If true the error should be fixed during inspection
        /// </summary>
        public bool AutomaticFix
        {
            get { return mAutomaticFix; }
            set { mAutomaticFix = value; }
        }

        /// <summary>
        /// If true the unreferenced objects should be deleted
        /// </summary>
        public bool DeleteUnreferenced
        {
            get { return mDeleteUnreferenced; }
            set { mDeleteUnreferenced = value; }
        }

        public ToolStripProgressBar myProgressBar
        {
            get { return mProgressBar; }
        }

        public TextBox myMessageBox
        {
            get { return mMessageBox; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Inspector(SDBeesDBConnection dbManager)
        {
            m_dbManager = dbManager;

            RegisterInspector();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~Inspector()
        {
            UnregisterInspector();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Inspect the database and fix/delete depending on the settings
        /// </summary>
        public abstract void InspectDatabase();

        /// <summary>
        /// Static method that will iterate through all registered inspectors and inspect the database
        /// </summary>
        /// <param name="automaticFix"></param>
        /// <param name="deleteUnreferenced"></param>
        /// <param name="progressBar"></param>
        /// <param name="messageBox"></param>
        public static void InspectDatabase(bool automaticFix, bool deleteUnreferenced, ToolStripProgressBar progressBar, TextBox messageBox)
        {
            if (messageBox != null)
            {
                messageBox.Text = "";
                messageBox.Refresh();
            }

            foreach (DictionaryEntry registeredObject in gRegisteredInspectors)
            {
                var inspector = (Inspector)registeredObject.Value;
                inspector.AutomaticFix = automaticFix;
                inspector.DeleteUnreferenced = deleteUnreferenced;
                inspector.mProgressBar = progressBar;
                inspector.mMessageBox = messageBox;

                inspector.InspectDatabase();
            }
        }

        #endregion

        #region Protected Methods

        protected void WriteMessage(string message)
        {
            if (mMessageBox != null)
            {
                mMessageBox.Text += message;
                mMessageBox.Refresh();
            }
        }

        /// <summary>
        /// Register the inspector, called in constructor
        /// </summary>
        private void RegisterInspector()
        {
            if (gRegisteredInspectors == null)
            {
                gRegisteredInspectors = new Hashtable();
            }
            var key = GetType().ToString();

            if (!gRegisteredInspectors.ContainsKey(key))
            {
                gRegisteredInspectors.Add(key, this);
            }
        }

        /// <summary>
        /// Unregister the inspector, called in destructor
        /// </summary>
        private void UnregisterInspector()
        {
            var key = GetType().ToString();

            if (gRegisteredInspectors.ContainsKey(key))
            {
                gRegisteredInspectors.Remove(key);
            }
        }

        #endregion
    }
}
