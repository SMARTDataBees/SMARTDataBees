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
namespace SDBees.Connectivity
{
    partial class ConnectivityControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bnConnect = new System.Windows.Forms.Button();
            this.bnDisconnect = new System.Windows.Forms.Button();
            this.ebOutput = new System.Windows.Forms.TextBox();
            this.bnHighlight = new System.Windows.Forms.Button();
            this.bnSynchronize = new System.Windows.Forms.Button();
            this.bnImport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bnConnect
            // 
            this.bnConnect.Location = new System.Drawing.Point(3, 3);
            this.bnConnect.Name = "bnConnect";
            this.bnConnect.Size = new System.Drawing.Size(68, 25);
            this.bnConnect.TabIndex = 0;
            this.bnConnect.Text = "&Verbinden";
            this.bnConnect.UseVisualStyleBackColor = true;
            this.bnConnect.Click += new System.EventHandler(this.bnConnect_Click);
            // 
            // bnDisconnect
            // 
            this.bnDisconnect.Location = new System.Drawing.Point(77, 3);
            this.bnDisconnect.Name = "bnDisconnect";
            this.bnDisconnect.Size = new System.Drawing.Size(65, 25);
            this.bnDisconnect.TabIndex = 1;
            this.bnDisconnect.Text = "&Trennen";
            this.bnDisconnect.UseVisualStyleBackColor = true;
            this.bnDisconnect.Click += new System.EventHandler(this.bnDisconnect_Click);
            // 
            // ebOutput
            // 
            this.ebOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ebOutput.Location = new System.Drawing.Point(3, 34);
            this.ebOutput.Multiline = true;
            this.ebOutput.Name = "ebOutput";
            this.ebOutput.ReadOnly = true;
            this.ebOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ebOutput.Size = new System.Drawing.Size(384, 238);
            this.ebOutput.TabIndex = 2;
            // 
            // bnHighlight
            // 
            this.bnHighlight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnHighlight.Location = new System.Drawing.Point(318, 3);
            this.bnHighlight.Name = "bnHighlight";
            this.bnHighlight.Size = new System.Drawing.Size(69, 25);
            this.bnHighlight.TabIndex = 3;
            this.bnHighlight.Text = "&Anzeigen";
            this.bnHighlight.UseVisualStyleBackColor = true;
            this.bnHighlight.Click += new System.EventHandler(this.bnHighlight_Click);
            // 
            // bnSynchronize
            // 
            this.bnSynchronize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnSynchronize.Location = new System.Drawing.Point(245, 3);
            this.bnSynchronize.Name = "bnSynchronize";
            this.bnSynchronize.Size = new System.Drawing.Size(67, 25);
            this.bnSynchronize.TabIndex = 4;
            this.bnSynchronize.Text = "&Synch.";
            this.bnSynchronize.UseVisualStyleBackColor = true;
            this.bnSynchronize.Click += new System.EventHandler(this.bnSynchronize_Click);
            // 
            // bnImport
            // 
            this.bnImport.Location = new System.Drawing.Point(148, 3);
            this.bnImport.Name = "bnImport";
            this.bnImport.Size = new System.Drawing.Size(65, 25);
            this.bnImport.TabIndex = 5;
            this.bnImport.Text = "&Import";
            this.bnImport.UseVisualStyleBackColor = true;
            this.bnImport.Click += new System.EventHandler(this.bnImport_Click);
            // 
            // ConnectivityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bnImport);
            this.Controls.Add(this.bnSynchronize);
            this.Controls.Add(this.bnHighlight);
            this.Controls.Add(this.ebOutput);
            this.Controls.Add(this.bnDisconnect);
            this.Controls.Add(this.bnConnect);
            this.MinimumSize = new System.Drawing.Size(380, 100);
            this.Name = "ConnectivityControl";
            this.Size = new System.Drawing.Size(390, 275);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button bnConnect;
        private System.Windows.Forms.Button bnDisconnect;
        private System.Windows.Forms.TextBox ebOutput;
        private System.Windows.Forms.Button bnHighlight;
        private System.Windows.Forms.Button bnSynchronize;
        private System.Windows.Forms.Button bnImport;
    }
}
