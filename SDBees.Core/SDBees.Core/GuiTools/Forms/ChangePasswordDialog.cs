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

namespace SDBees.GuiTools
{
    /// <inheritdoc />
    public partial class ChangePasswordDialog : Form
    {
        private string _currentPassword;
        private string _newPassword;

        /// <summary>
        /// Returns the current password
        /// </summary>
        public string CurrentPassword => _currentPassword;

        /// <summary>
        /// Returns the new password
        /// </summary>
        public string NewPassword => _newPassword;

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ChangePasswordDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Triggered when OK is clicked
        /// </summary>
        /// <param name="sender">Dialog</param>
        /// <param name="eventArgs">Standard event arguments</param>
        private void OnOkay(object sender, EventArgs eventArgs)
        {
            if (NewPasswordBox.Text == ConfirmationPasswordBox.Text)
            {
                _currentPassword = CurrentPasswordBox.Text;
                _newPassword = NewPasswordBox.Text;
                DialogResult = DialogResult.OK;

                Close();
            }
            else
            {
                MessageBox.Show(@"The password confirmation does not match.");
            }
        }
    }
}
