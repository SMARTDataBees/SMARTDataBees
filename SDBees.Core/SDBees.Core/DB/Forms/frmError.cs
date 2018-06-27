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
using System.Windows.Forms;

namespace SDBees.DB.Forms
{
    public partial class frmError : Form
    {
        private Error mError;
        private string mTitle;
        private string mMessage;

        /// <summary>
        /// Error(stack) to display
        /// </summary>
        public Error Error
        {
            get { return mError; }
            set { mError = value; }
        }

        /// <summary>
        /// Title of the Error dialog
        /// </summary>
        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; }
        }

        /// <summary>
        /// Message to display
        /// </summary>
        public string Message
        {
            get { return mMessage; }
            set { mMessage = value; }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public frmError()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Standard constructor with parameters
        /// </summary>
        /// <param name="message">Message to display</param>
        /// <param name="title">Title of the form</param>
        /// <param name="error">error(stack) to display</param>
        public frmError(string message, string title, Error error)
        {
            mTitle = title;
            mMessage = message;
            mError = error;

            InitializeComponent();
        }

        private void frmError_Load(object sender, EventArgs e)
        {
            Text = mTitle;
            lbMessage.Text = mMessage;
            if (mError != null)
            {
                ebDetails.Text = mError.FullDescription();
            }
        }

        private void bnOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
