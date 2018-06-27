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
using SDBees.DB.Forms;

namespace SDBees.DB
{
    /// <summary>
    /// Class used for error logging and displaying
    /// </summary>
    public class Error
    {
        #region Private Data Members

        private string mMessage;
        private int mNumber;
        private Error mInnerError;
        private Type mSource;

        #endregion

        #region Public Properties

        /// <summary>
        /// Message to be displayed for this error
        /// </summary>
        public string Message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }

        /// <summary>
        /// Error number
        /// </summary>
        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; }
        }

        /// <summary>
        /// Error this has occurred from (next in the error stack)
        /// </summary>
        public Error InnerError
        {
            get { return mInnerError; }
            set { mInnerError = value; }
        }

        /// <summary>
        /// Source of the error
        /// </summary>
        public Type Source
        {
            get { return mSource; }
            set { mSource = value; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Error()
        {
            mMessage = "";
            mNumber = 0;
            mInnerError = null;
            mSource = null;
        }

        /// <summary>
        /// Standard constructor with parameters
        /// </summary>
        /// <param name="message"></param>
        /// <param name="number"></param>
        /// <param name="source"></param>
        public Error(string message, int number, Type source)
        {
            mMessage = message;
            mNumber = number;
            mInnerError = null;
            mSource = source;
        }

        /// <summary>
        /// Standard constructor with parameters
        /// </summary>
        /// <param name="message"></param>
        /// <param name="number"></param>
        /// <param name="source"></param>
        /// <param name="innerError"></param>
        public Error(string message, int number, Type source, Error innerError)
        {
            mMessage = message;
            mNumber = number;
            mInnerError = innerError;
            mSource = source;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create the full description of all error in the stack
        /// </summary>
        /// <returns></returns>
        public string FullDescription()
        {
            var msg = "(E" + mNumber + ") - " + mMessage + " - in " + mSource.FullName + " (" + mSource.Module + ")";
            if (mInnerError != null)
            {
                msg += "\r\n" + mInnerError.FullDescription();
            }

            return msg;
        }

        /// <summary>
        /// Displays an error dialog if error is not null. If it is, nothing will be displayed
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="error"></param>
        public static void Display(string msg, Error error)
        {
            if (error != null)
            {
                var dlg = new frmError(msg, "Application error", error);
                dlg.ShowDialog();

                dlg.Dispose();
            }
        }

        #endregion
    }
}
