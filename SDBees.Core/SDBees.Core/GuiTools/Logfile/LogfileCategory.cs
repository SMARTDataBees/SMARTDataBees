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

namespace SDBees.GuiTools
{
    /// <summary>
    /// Class for grouping, filtering log file messages and giving them consistent display
    /// </summary>
    public class LogfileCategory
    {
        #region Private Data Members

        private string mName;
        private LogfileWriter.Flags mFlags;
        private bool mEnabled;
        private byte mRed;
        private byte mGreen;
        private byte mBlue;

        #endregion

        #region Public Properties

        /// <summary>
        /// Name of this category, this will be used when referenced
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Flags for this category, see LogfileWriter for details
        /// </summary>
        public LogfileWriter.Flags Flags
        {
            get { return mFlags; }
            set { mFlags = value; }
        }

        /// <summary>
        /// When enabled log messages will be displayed
        /// </summary>
        public bool Enabled
        {
            get { return mEnabled; }
            set { mEnabled = value; }
        }

        /// <summary>
        /// Red portion of the color (0-255)
        /// </summary>
        public byte Red
        {
            get { return mRed; }
            set { mRed = value; }
        }

        /// <summary>
        /// Green portion of the color (0-255)
        /// </summary>
        public byte Green
        {
            get { return mGreen; }
            set { mGreen = value; }
        }

        /// <summary>
        /// Blue portion of the color (0-255)
        /// </summary>
        public byte Blue
        {
            get { return mBlue; }
            set { mBlue = value; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="name"></param>
        public LogfileCategory(string name)
        {
            mName = name;
            mFlags = 0;
            mEnabled = true;
            mRed = 0;
            mGreen = 0;
            mBlue = 0;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Events

        #endregion
    }
}
