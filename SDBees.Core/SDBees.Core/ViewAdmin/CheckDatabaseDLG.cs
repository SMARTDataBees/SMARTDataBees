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
using SDBees.DB;

namespace SDBees.ViewAdmin
{
    public partial class CheckDatabaseDLG : Form
    {
        public CheckDatabaseDLG()
        {
            InitializeComponent();
        }

        private void bnStart_Click(object sender, EventArgs e)
        {
            // remember the old cursor and set the wait cursor
            var oldCursor = Cursor;
            Cursor = Cursors.WaitCursor;

            progressBar.Visible = true;

            Inspector.InspectDatabase(cbAutomaticFix.Checked, cbAutomaticDelete.Checked, progressBar, ebOutput);

            // reset the cursor
            Cursor = oldCursor;

            // reset the window's state...
            progressBar.Visible = false;
        }
    }
}
