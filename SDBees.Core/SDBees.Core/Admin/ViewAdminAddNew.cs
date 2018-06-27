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
using System.Windows.Forms;
using SDBees.Plugs.Objects;

namespace SDBees.Core.Admin
{
    public partial class ViewAdminAddNew : Form
    {
        private Hashtable _hashViewDefs;

        public ViewAdminAddNew(ref Hashtable _hashView)
        {
            _hashViewDefs = new Hashtable(); // _hashView;

            InitializeComponent();
        }

        public string ViewName
        {
            get { return textBoxViewName.Text; }
        }

        public string ViewDescription
        {
            get { return richTextBoxDescription.Text; }
        }

        public string ViewGuid { get; private set; }


        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (_hashViewDefs.ContainsKey(ViewName))
            {
                MessageBox.Show("Der Viewname ist bereits vorhanden!\nBitte einen neuen Namen wählen!");
            }
            else
            {
                if (ViewName != "")
                {
                    var _plgObjViews = new ObjectView();
                    _plgObjViews.ViewName = ViewName;
                    _plgObjViews.ViewGUID = Guid.NewGuid() + "";
                    ViewGuid = _plgObjViews.ViewGUID;
                    _plgObjViews.ViewDescription = ViewDescription;
                    _hashViewDefs.Add(ViewName, _plgObjViews);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show("Der Viewname darf nicht leer sein!");
                }
            }
        }

        private void buttonChancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
