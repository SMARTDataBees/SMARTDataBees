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
using View = SDBees.Plugs.Objects.View;

namespace SDBees.Core.Admin
{
    public partial class AddPluginConfigurationForm : Form
    {
        public View View { get; private set; }

        public AddPluginConfigurationForm()
        {
            View = null;
            InitializeComponent();
        }

        private void OnOkay(object sender, EventArgs eventArgs)
        {
            if (string.IsNullOrEmpty(textBoxViewName.Text))
            {
                MessageBox.Show(@"Enter a configuration name, please");
                return;
            }
            View = new View
            {
                Identification = Guid.NewGuid() + "",
                Name = textBoxViewName.Text,
                Description = richTextBoxDescription.Text
            };
            DialogResult = DialogResult.OK;
            Close();
        }



        private void OnCancel(object sender, EventArgs eventArgs)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
