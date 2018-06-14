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

namespace SDBees.Plugs.Objects
{
  public class ObjectPlugin
  {
    private string _sTypeOf;
    private string _Name;
    private string _ViewID;
    private string _ID;
    private object _Plugin;

    public ObjectPlugin()
    {
      _sTypeOf = "";
      _Name = "";
      _ViewID = "";
      _ID = "";
      _Plugin = null;
    }

    public string ID
    {
      get { return _ID; }
      set { _ID = value; }
    }

    public string PluginType
    {
      get { return _sTypeOf; }
      set { _sTypeOf = value; }
    }

    public string PluginName
    {
      get { return _Name; }
      set { _Name = value; }
    }

    public object Plugin
    {
        get { return _Plugin; }
        set { _Plugin = value; }
    }

    /// <summary>
    /// Die ID der aktuellen View
    /// </summary>
    public string ViewID
    {
      get { return _ViewID; }
      set { _ViewID = value; }
    }
  }
}
