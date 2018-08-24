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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SDBees.DB
{
	/// <summary>
	/// Summary description for InputBoxForm.
	/// </summary>
	internal class frmInputBox : Form
	{
		private Button btnOK;
		private Button btnCancel;
		private Label lblText;
		private TextBox txtResult;
		private string strReturnValue;
		private Point pntStartLocation;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public frmInputBox()
		{
			// Required for Windows Form Designer support
			InitializeComponent();
			strReturnValue = "";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnOK = new System.Windows.Forms.Button();
            this.txtResult = new System.Windows.Forms.TextBox();
            this.lblText = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(576, 15);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(128, 44);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // txtResult
            // 
            this.txtResult.Location = new System.Drawing.Point(16, 148);
            this.txtResult.Name = "txtResult";
            this.txtResult.Size = new System.Drawing.Size(688, 31);
            this.txtResult.TabIndex = 0;
            // 
            // lblText
            // 
            this.lblText.Location = new System.Drawing.Point(32, 15);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(528, 118);
            this.lblText.TabIndex = 0;
            this.lblText.Text = "InputBox";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(576, 74);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(128, 44);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmInputBox
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(10, 24);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(726, 229);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblText);
            this.Controls.Add(this.txtResult);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmInputBox";
            this.ShowIcon = false;
            this.Text = "InputBox";
            this.Load += new System.EventHandler(this.InputBoxForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void InputBoxForm_Load(object sender, EventArgs e)
		{
			if (!pntStartLocation.IsEmpty) 
			{
				Top = pntStartLocation.X;
				Left = pntStartLocation.Y;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			strReturnValue = txtResult.Text;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			Close();
		}

		public string Title
		{
			set
			{
				Text = value;
			}
		}

		public string Prompt
		{
			set
			{
				lblText.Text = value;
			}
		}

		public string ReturnValue
		{
			get
			{
				return strReturnValue;
			}
		}

		public string DefaultResponse
		{
			set
			{
				txtResult.Text = value;
				txtResult.SelectAll();
			}
		}

		public Point StartLocation
		{
			set
			{
				pntStartLocation = value;
			}
		}
	}
}
