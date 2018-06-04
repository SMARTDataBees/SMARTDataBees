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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SDBees.GuiTools
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ChangePasswordDLG : Form
    {
        private string mOldPassword;
        private string mNewPassword;

        /// <summary>
        /// Returns the old password that the user typed in
        /// </summary>
        public string OldPassword
        {
            get { return mOldPassword; }
        }

        /// <summary>
        /// Returns the new password the user typed in and confirmed
        /// </summary>
        public string NewPassword
        {
            get { return mNewPassword; }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ChangePasswordDLG()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Triggered when OK is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bnOk_Click(object sender, EventArgs e)
        {
            if (ebNewPassword.Text == ebConfirmPassword.Text)
            {
                mOldPassword = ebOldPassword.Text;
                mNewPassword = ebNewPassword.Text;

                this.DialogResult = DialogResult.OK;

                this.Close();
            }
            else
            {
                MessageBox.Show("Das neue Passwort stimmt nicht mit der Bestätigung überein!");
            }
        }
    }
}
