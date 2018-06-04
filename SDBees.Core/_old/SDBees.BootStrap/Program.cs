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
using System.Reflection;

using Carbon;
//using Razor;

namespace SDBees.BootStrap
{

  public class Startup
  {
    [STAThread]
    static void Main(string[] args)
    {
      /*
      VersioningBootStrap bootStrap = new VersioningBootStrap();
      bootStrap.Run(args, System.Reflection.Assembly.GetExecutingAssembly());
       * */
      // use the carbon runtime to create a plugin host for this application
      CarbonRuntime.CreatePluginContext(Assembly.GetExecutingAssembly(), args);

    }
  }
}
