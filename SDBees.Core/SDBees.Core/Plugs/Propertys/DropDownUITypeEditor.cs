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
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel;

namespace SDBees.Plugs.Properties
{
    public class DropDownUITypeEditor : UITypeEditor
    {
        private IWindowsFormsEditorService mEditorService;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            ListBox listBox = new ListBox();

            PropertyBag propertyBag = (PropertyBag)context.Instance;
            int index = propertyBag.Properties.IndexOf(context.PropertyDescriptor.Name);
            PropertySpecListbox propertySpec = (PropertySpecListbox)propertyBag.Properties[index];

            if (propertySpec.SelectionList != null)
            {
                foreach (string strValue in propertySpec.SelectionList)
                {
                    listBox.Items.Add(strValue);
                }
            }

            listBox.SelectedIndexChanged += new EventHandler(listBox_SelectedIndexChanged);

            mEditorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            mEditorService.DropDownControl(listBox);

            return listBox.SelectedItem;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.mEditorService.CloseDropDown();
        }
    }
}
