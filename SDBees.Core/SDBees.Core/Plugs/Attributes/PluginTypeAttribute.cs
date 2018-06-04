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

namespace SDBees.Plugs.Attributes
{
  /// <summary>
  /// Defines a plugin attribute for specifying the type of a plugin.
  /// Allowed types:
  /// MenueItem - A Plugin as menueitem
  /// TreeNode - 
  /// TreeNodeHelper - 
  /// MainWindow - The one and only main window ,-)
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public sealed class PluginTypeDefAttribute : Attribute
  {
    private readonly string _type;

    /// <summary>
    /// Initializes a new instance of the PluginTypeAttribute class.
    /// </summary>
    /// <param name="type">An string for the plugintype</param>
    public PluginTypeDefAttribute(string type)
    {
      _type = type;
    }

    /// <summary>
    /// Returns an array of strings the specify the names of the plugin's type.
    /// </summary>
    public string Types
    {
      get
      {
        return _type;
      }
    }
  }
}
