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
/*
 * ajma.Utils.InputBox
 * Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
 *  
 * Andrew J. Ma
 * ajmaonline@hotmail.com
 */

using System;
using System.Drawing;

namespace SDBees.DB
{
	/// <summary>
	/// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
	/// </summary>
	public class InputBox
	{
		/// <summary>
		/// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
		/// </summary>
		/// <param name="Prompt">String expression displayed as the message in the dialog box.</param>
		/// <param name="Title">String expression displayed in the title bar of the dialog box.</param>
		/// <returns>The value in the textbox is returned if the user clicks OK or presses the ENTER key. If the user clicks Cancel, a zero-length string is returned.</returns>
		public static string Show(string Prompt, string Title, ref System.Windows.Forms.DialogResult dlgres)
		{
			return Show(Prompt, Title, "", -1, -1, ref dlgres);
		}

		/// <summary>
		/// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
		/// </summary>
		/// <param name="Prompt">String expression displayed as the message in the dialog box.</param>
		/// <param name="Title">String expression displayed in the title bar of the dialog box.</param>
		/// <param name="DefaultResponse">String expression displayed in the text box as the default response if no other input is provided. If you omit DefaultResponse, the displayed text box is empty.</param>
		/// <returns>The value in the textbox is returned if the user clicks OK or presses the ENTER key. If the user clicks Cancel, a zero-length string is returned.</returns>
		public static string Show(string Prompt, string Title, string DefaultResponse, ref System.Windows.Forms.DialogResult dlgres)
		{
			return Show(Prompt, Title, DefaultResponse, -1, -1, ref dlgres);
		}

		/// <summary>
		/// Displays a prompt in a dialog box, waits for the user to input text or click a button, and then returns a string containing the contents of the text box.
		/// </summary>
		/// <param name="Prompt">String expression displayed as the message in the dialog box.</param>
		/// <param name="Title">String expression displayed in the title bar of the dialog box.</param>
		/// <param name="DefaultResponse">String expression displayed in the text box as the default response if no other input is provided. If you omit DefaultResponse, the displayed text box is empty.</param>
		/// <param name="XPos">Integer expression that specifies, in pixels, the distance of the left edge of the dialog box from the left edge of the screen.</param>
		/// <param name="YPos">Integer expression that specifies, in pixels, the distance of the upper edge of the dialog box from the top of the screen.</param>
		/// <returns>The value in the textbox is returned if the user clicks OK or presses the ENTER key. If the user clicks Cancel, a zero-length string is returned.</returns>
		public static string Show(string Prompt, string Title, string DefaultResponse, int XPos, int YPos, ref System.Windows.Forms.DialogResult dlgres)
		{
			// Create a new input box dialog
			frmInputBox frmInputBox = new frmInputBox();
			frmInputBox.Title = Title;
			frmInputBox.Prompt = Prompt;
			frmInputBox.DefaultResponse = DefaultResponse;
			if (XPos >= 0 && YPos >= 0)
			{
				frmInputBox.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
				frmInputBox.Location = new Point(XPos, YPos);
			}
			dlgres = frmInputBox.ShowDialog();
			return frmInputBox.ReturnValue;
		}
	}
}
