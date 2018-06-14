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
using System.Windows.Forms;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.Plugs.TreenodeHelper
{
  /// <summary>
  /// Helpertemplate für die TemplateTreenode basierenden Klassen
  /// 
  /// </summary>
  public abstract class TemplateTreenodeHelper : TemplateBase.TemplateBase
  {

    private static Hashtable _hashPlugins;

    public TemplateTreenodeHelper()
    {
        if (_hashPlugins == null)
        {
            _hashPlugins = new Hashtable();
        }
        var treenodeType = GetType().ToString();
        _hashPlugins.Add(treenodeType, this);
    }

    /// <summary>
    /// Override this method if plugin should react to selection changes in the system tree view
    /// </summary>
    public virtual void UpdatePropertyPage(TabPage tabPage, Guid viewId, TemplateTreenodeTag selectedTag, TemplateTreenodeTag parentTag)
    {
        // Do nothing at this stage.
    }

    /// <summary>
    /// Diese Funktion überladen und einen "Eindeutigen" Tab-Name zurückgeben, damit eine TabPage
    /// für diesen Helper angelegt wird.
    /// </summary>
    public virtual string TabPageName()
    {
        return "";
    }

    public abstract UserControl MyUserControl();


    /// <summary>
    /// returns a list of menu items to add to the current popup Menu, called by the popup managing class
    /// </summary>
    /// <param name="nodes">the selected node</param>
    /// <param name="menuItemsToAdd">the menu items to add</param>
    /// <returns>true if menu has to be added</returns>
    public virtual bool GetPopupMenuItems(List<TemplateTreenodeTag> nodes, out List<MenuItem> menuItemsToAdd)
    {
        menuItemsToAdd = null;
        return false;
    }

    /// <summary>
    /// Returns a list of the loaded helper plugins
    /// </summary>
    /// <returns></returns>
    public static List<TemplateTreenodeHelper> GetAllHelperPlugins()
    {
        var result = new List<TemplateTreenodeHelper>();

        if (_hashPlugins != null)
        {
            foreach (DictionaryEntry plugin in _hashPlugins)
            {
                result.Add((TemplateTreenodeHelper)plugin.Value);
            }
        }

        return result;
    }

  }
}
