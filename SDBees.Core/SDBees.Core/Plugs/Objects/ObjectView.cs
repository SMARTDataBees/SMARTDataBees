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
  public class ObjectView
  {
    private string _sViewName = null;
    private string _sViewDescription = null;
    private string _sViewGUID = null;
    
    public string ViewName
    {
      get { return this._sViewName; }
      set { this._sViewName = value; }
    }

    public string ViewDescription
    {
      get { return this._sViewDescription; }
      set { this._sViewDescription = value; }
    }

    public string ViewGUID
    {
      get { return this._sViewGUID; }
      set { this._sViewGUID = value; }
    }

    public ObjectView()
    {
      _sViewName = "";
      _sViewDescription = "";
      _sViewGUID = "";
    }
  }
}
