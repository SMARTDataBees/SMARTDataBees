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

using System.ComponentModel;
using System.Windows.Forms;

namespace SDBees.DB.Forms
{
    /// <summary>
    /// Form to display errors
    /// </summary>
    partial class frmError
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ebDetails = new System.Windows.Forms.TextBox();
            this.lbMessage = new System.Windows.Forms.Label();
            this.bnOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ebDetails
            // 
            this.ebDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ebDetails.Location = new System.Drawing.Point(24, 146);
            this.ebDetails.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ebDetails.Multiline = true;
            this.ebDetails.Name = "ebDetails";
            this.ebDetails.ReadOnly = true;
            this.ebDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ebDetails.Size = new System.Drawing.Size(1160, 289);
            this.ebDetails.TabIndex = 2;
            // 
            // lbMessage
            // 
            this.lbMessage.Location = new System.Drawing.Point(24, 17);
            this.lbMessage.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbMessage.Name = "lbMessage";
            this.lbMessage.Size = new System.Drawing.Size(1164, 123);
            this.lbMessage.TabIndex = 1;
            // 
            // bnOk
            // 
            this.bnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnOk.Location = new System.Drawing.Point(1038, 450);
            this.bnOk.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.bnOk.Name = "bnOk";
            this.bnOk.Size = new System.Drawing.Size(150, 44);
            this.bnOk.TabIndex = 0;
            this.bnOk.Text = "OK";
            this.bnOk.UseVisualStyleBackColor = true;
            this.bnOk.Click += new System.EventHandler(this.bnOk_Click);
            // 
            // frmError
            // 
            this.AcceptButton = this.bnOk;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(1212, 517);
            this.Controls.Add(this.bnOk);
            this.Controls.Add(this.lbMessage);
            this.Controls.Add(this.ebDetails);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "frmError";
            this.ShowIcon = false;
            this.Text = "Error";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmError_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox ebDetails;
        private Label lbMessage;
        private Button bnOk;
    }
}
