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
  public class ObjectView
  {
    private string _sViewName;
    private string _sViewDescription;
    private string _sViewGUID;
        private string _idSdBees;

        public string ViewName
    {
      get { return _sViewName; }
      set { _sViewName = value; }
    }

    public string ViewDescription
    {
      get { return _sViewDescription; }
      set { _sViewDescription = value; }
    }

    public string ViewGUID
    {
      get { return _sViewGUID; }
      set { _sViewGUID = value; }
    }

        public string IdSdBees
        {
            get { return _idSdBees; }
            set { _idSdBees = value; }
        }


        public ObjectView()
    {
      _sViewName = "";
      _sViewDescription = "";
      _sViewGUID = "";
    }
  }
}
