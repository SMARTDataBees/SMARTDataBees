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

using System.Windows.Forms;

namespace SDBees.Plugs.TemplateTreeNode
{
    public partial class UserControlTempTreenode : UserControl, IUserControlTempTreenode
    {
        public UserControlTempTreenode()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Returns the Propertygrid in our UserControl
        /// </summary>
        public PropertyGrid MyGrid()
        {
            return propertyGridUserControl;
        }

        void m_propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            var row = MyGrid().SelectedObject as TreenodePropertyRow;

            row.OnPropertyValueChanged(e);
        }
    }

    public interface IUserControlTempTreenode
    {
        PropertyGrid MyGrid();
    }
}
