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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SDBees.ViewAdmin
{
  public partial class ViewAdminAddNew : Form
  {
    private Hashtable _hashViewDefs;

    public ViewAdminAddNew(ref Hashtable _hashView)
    {
      this._hashViewDefs = _hashView;
      InitializeComponent();
    }

    public string ViewName
    {
      get {return textBoxViewName.Text;}
    }

    public string ViewDescription
    {
      get { return this.richTextBoxDescription.Text; }
    }

    private void buttonOK_Click(object sender, EventArgs e)
    {
      if (this._hashViewDefs.ContainsKey(this.ViewName))
      {
        MessageBox.Show("Der Viewname ist bereits vorhanden!\nBitte einen neuen Namen wählen!");
      }
      else
      {
        if (this.ViewName != "")
        {
          SDBees.Plugs.Objects.ObjectView _plgObjViews = new SDBees.Plugs.Objects.ObjectView();
          _plgObjViews.ViewName = this.ViewName;
          _plgObjViews.ViewGUID = null;
          _plgObjViews.ViewDescription = this.ViewDescription;
          this._hashViewDefs.Add(this.ViewName, _plgObjViews);
          this.DialogResult = DialogResult.OK;
          this.Close();          
        }
        else
        {
          MessageBox.Show("Der Viewname darf nicht leer sein!");
        }
      }
    }

    private void buttonChancel_Click(object sender, EventArgs e)
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close();
    }
  }
}
