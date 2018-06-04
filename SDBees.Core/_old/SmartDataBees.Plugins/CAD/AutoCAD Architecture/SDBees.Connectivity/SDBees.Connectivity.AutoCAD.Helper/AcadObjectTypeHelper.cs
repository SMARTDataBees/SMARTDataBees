// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
//
// Copyright (C) 2007 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
//
// #EndHeader# ================================================================
/*
 * Erstellt mit SharpDevelop.
 * Benutzer: th
 * Datum: 19.05.2007
 * Zeit: 13:36
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Windows.Forms;

using Autodesk;
using Autodesk.AutoCAD;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
//using Autodesk.AutoCAD.
using AutoCAD;


namespace SDBees.Connectivity.AutoCAD.Helper
{
	/// <summary>
	/// Description of AcadObjectTypeHelper.
	/// </summary>
	public class AcadObjectTypeHelper : IExtensionApplication
	{
		public AcadObjectTypeHelper()
		{
		}
		
		#region IExtensionApplication
		public void Initialize()
		{
			Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
			ed.WriteMessage("\n\nLoading SDBeesAcadTools...\n");
		}

		public void Terminate()
		{
			Editor ed = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
			ed.WriteMessage("Unloading SDBeesAcadTools...");
		}
		#endregion

		
		#region Commands
		[Autodesk.AutoCAD.Runtime.CommandMethod("SDBees", "SDBeesGetObjecttype", Autodesk.AutoCAD.Runtime.CommandFlags.Modal)]
		public void FindAcadObjectType()
		{
			try
			{
				AutoCAD.AcadApplication acadApp = (AcadApplication)Autodesk.AutoCAD.ApplicationServices.Application.AcadApplication;
				//this.mScheduleApp = (IAecScheduleApplication)mAcadApp.GetInterfaceObject("AecX.AecScheduleApplication.4.7");
	
				AcadDatabase db = acadApp.ActiveDocument.Database;
	
				MessageBox.Show("Testfenster");
			}
			catch(System.Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}			
		}
		#endregion
	
	}
}
