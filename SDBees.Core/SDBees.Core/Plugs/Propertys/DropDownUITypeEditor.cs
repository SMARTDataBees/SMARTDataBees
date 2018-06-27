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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SDBees.Plugs.Properties
{
    public class DropDownUITypeEditor : UITypeEditor
    {
        private IWindowsFormsEditorService mEditorService;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {

            var listBox = new ListBox();

            var propertyBag = (PropertyBag)context.Instance;
            var index = propertyBag.Properties.IndexOf(context.PropertyDescriptor.Name);
            var propertySpec = (PropertySpecListbox)propertyBag.Properties[index];

            if (propertySpec.SelectionList != null)
            {
                foreach (var strValue in propertySpec.SelectionList)
                {
                    listBox.Items.Add(strValue);
                }
            }

            listBox.SelectedIndexChanged += listBox_SelectedIndexChanged;

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
            mEditorService.CloseDropDown();
        }
    }
}
