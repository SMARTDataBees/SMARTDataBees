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
using System.Collections;
using System.IO;
using System.Text;
using Carbon.Configuration;
using SDBees.Core.Global;

namespace SDBees.GuiTools
{
    /// <summary>
    /// Class wrapping writing a log file using HTML
    /// </summary>
    public class LogfileWriter
    {
        #region Private Data Members

        private Hashtable mCategories;
        private string mFilename;

        #endregion

        #region Public Types

        /// <summary>
        /// Flags that can be set in the categories
        /// </summary>
        [Flags]
        public enum Flags
        {
            /// <summary>
            /// Title of a message will be displayed bold
            /// </summary>
            eTitleBold     = 0x00000001,

            /// <summary>
            /// Title of a message will be displayed italic
            /// </summary>
            eTitleItalic   = 0x00000002,

            /// <summary>
            /// Body of a message will be displayed bold
            /// </summary>
            eMessageBold   = 0x00000004,

            /// <summary>
            /// Body of a message will be displayed italic
            /// </summary>
            eMessageItalic = 0x00000008,

            /// <summary>
            /// Display date and time at the beginning of the log message
            /// </summary>
            eShowDateTime  = 0x00000010
        }

        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="filename"></param>
        public LogfileWriter(string filename)
        {
            mFilename = filename;
            mCategories = new Hashtable();

            SetUpConfiguration();
        }

        internal static string m_LogSuccess = "LogSuccess";
        private static bool m_LogSuccessDefault = false;

        private void SetUpConfiguration()
        {
            var catLog = SDBeesLogLocalConfiguration();

            var opLogSuccess = new XmlConfigurationOption(m_LogSuccess, m_LogSuccessDefault);
            opLogSuccess.Description = "Log items with successfull opterations?";
            //opAnullarSpace.EditorAssemblyQualifiedName
            if (!catLog.Options.Contains(opLogSuccess))
                catLog.Options.Add(opLogSuccess);

        }

        internal static string m_ConfigurationSection = "Log";
        internal static XmlConfigurationCategory SDBeesLogLocalConfiguration()
        {
            return SDBeesGlobalVars.SDBeesLocalUsersConfiguration().Categories[m_ConfigurationSection, true];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get a category by the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public LogfileCategory GetCategory(string name)
        {
            LogfileCategory result = null;

            if (mCategories.ContainsKey(name))
            {
                result = (LogfileCategory)mCategories[name];
            }

            return result;
        }

        /// <summary>
        /// Add a category to the log file writer
        /// </summary>
        /// <param name="category"></param>
        /// <returns>true if successful, false if a category with this name already exists</returns>
        public bool AddCategory(LogfileCategory category)
        {
            var result = false;

            if (!mCategories.ContainsKey(category.Name))
            {
                mCategories[category.Name] = category;
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Write a message to the log file considering the category settings
        /// </summary>
        /// <param name="label"></param>
        /// <param name="message"></param>
        /// <param name="categoryName"></param>
        public void Writeline (string label, string message, string categoryName)
        {
            var category = GetCategory(categoryName);

            if ((category == null) || (category.Enabled))
            {
                var preGeneralSettings = "";
                var postGeneralSettings = "";
                var preLabelSettings = "";
                var postLabelSettings = "";
                var preMessageSettings = "";
                var postMessageSettings = "";

                if (category != null)
                {
                    var rgb = string.Format("{0,2:X2}{1,2:X2}{2,2:X2}", category.Red, category.Green, category.Blue);
                    preGeneralSettings += "<font color = \"#" + rgb + "\">";
                    postGeneralSettings += "</font>";

                    if ((category.Flags & Flags.eTitleBold) != 0)
                    {
                        preLabelSettings += "<B>";
                        postLabelSettings += "</B>";
                    }
                    if ((category.Flags & Flags.eMessageBold) != 0)
                    {
                        preMessageSettings += "<B>";
                        postMessageSettings += "</B>";
                    }
                    if ((category.Flags & Flags.eTitleItalic) != 0)
                    {
                        preLabelSettings += "<I>";
                        postLabelSettings += "</I>";
                    }
                    if ((category.Flags & Flags.eMessageItalic) != 0)
                    {
                        preMessageSettings += "<I>";
                        postMessageSettings += "</I>";
                    }
                    if ((category.Flags & Flags.eShowDateTime) != 0)
                    {
                        var currentTime = DateTime.Now;
                        var dateTime = currentTime.ToShortDateString() + ", " + currentTime.ToLongTimeString() + " - ";

                        preLabelSettings += dateTime;
                    }
                }
                var line = preGeneralSettings + preLabelSettings + label + ": " + postLabelSettings + preMessageSettings + message + postMessageSettings + postGeneralSettings + "<BR>";

                Write(line, true);
            }
        }

        #endregion

        #region Protected Methods

        private void Write(string line, bool withEOL)
        {
            var utf8 = new UTF8Encoding();

            var writer = new StreamWriter(mFilename, true, utf8);

            writer.Write(line);
            if (withEOL)
            {
                writer.WriteLine();
            }
            writer.Flush();
            writer.Close();
        }

        #endregion

        #region Events

        #endregion
    }
}
