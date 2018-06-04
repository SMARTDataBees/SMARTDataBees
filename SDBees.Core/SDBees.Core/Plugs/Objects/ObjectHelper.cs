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

namespace SDBees.Plugs.Objects
{
  public class ObjectHelper
  {
    private string _sTypeOf = null;
    private string _Name = null;
    private string _TabPageLabel = null;
    private object _Plugin = null;

    /// <summary>
    /// The Type of the HelperPlugin
    /// </summary>
    public string PluginType
    {
      get { return this._sTypeOf; }
      set { this._sTypeOf = value; }
    }

    /// <summary>
    /// The Name of the HelperPlugin
    /// </summary>
    public string PluginName
    {
      get { return this._Name; }
      set { this._Name = value; }
    }

    /// <summary>
    /// The Label of TabPage for the HelperPlugin, "" if not to be displayed
    /// </summary>
    public string PluginTabPageLabel
    {
        get { return this._TabPageLabel; }
        set { this._TabPageLabel = value; }
    }

    /// <summary>
    /// The Plugin instance for the HelperPlugin
    /// </summary>
    public object Plugin
    {
        get { return this._Plugin; }
        set { this._Plugin = value; }
    }

    /// <summary>
    /// The Constructor for the HelperPlugin
    /// </summary>
    public ObjectHelper()
    {
      _sTypeOf = "";
      _Name = "";
      _TabPageLabel = "";
      _Plugin = null;
  }
  }
}
