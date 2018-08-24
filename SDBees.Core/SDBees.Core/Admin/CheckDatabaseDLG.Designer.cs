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

namespace SDBees.Core.Admin
{
    partial class CheckDatabaseDLG
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
            this.cbAutomaticFix = new System.Windows.Forms.CheckBox();
            this.cbAutomaticDelete = new System.Windows.Forms.CheckBox();
            this.bnStart = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.ebOutput = new System.Windows.Forms.TextBox();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbAutomaticFix
            // 
            this.cbAutomaticFix.AutoSize = true;
            this.cbAutomaticFix.Location = new System.Drawing.Point(24, 23);
            this.cbAutomaticFix.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbAutomaticFix.Name = "cbAutomaticFix";
            this.cbAutomaticFix.Size = new System.Drawing.Size(169, 29);
            this.cbAutomaticFix.TabIndex = 0;
            this.cbAutomaticFix.Text = "Repair &errors";
            this.cbAutomaticFix.UseVisualStyleBackColor = true;
            // 
            // cbAutomaticDelete
            // 
            this.cbAutomaticDelete.AutoSize = true;
            this.cbAutomaticDelete.Location = new System.Drawing.Point(24, 67);
            this.cbAutomaticDelete.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.cbAutomaticDelete.Name = "cbAutomaticDelete";
            this.cbAutomaticDelete.Size = new System.Drawing.Size(332, 29);
            this.cbAutomaticDelete.TabIndex = 1;
            this.cbAutomaticDelete.Text = "&Delete unreferenced elements";
            this.cbAutomaticDelete.UseVisualStyleBackColor = true;
            // 
            // bnStart
            // 
            this.bnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bnStart.Location = new System.Drawing.Point(860, 16);
            this.bnStart.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.bnStart.Name = "bnStart";
            this.bnStart.Size = new System.Drawing.Size(106, 44);
            this.bnStart.TabIndex = 3;
            this.bnStart.Text = "&Check";
            this.bnStart.UseVisualStyleBackColor = true;
            this.bnStart.Click += new System.EventHandler(this.bnStart_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 517);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.statusStrip1.Size = new System.Drawing.Size(990, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(400, 25);
            this.progressBar.Visible = false;
            // 
            // ebOutput
            // 
            this.ebOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ebOutput.Location = new System.Drawing.Point(24, 128);
            this.ebOutput.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.ebOutput.Multiline = true;
            this.ebOutput.Name = "ebOutput";
            this.ebOutput.ReadOnly = true;
            this.ebOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ebOutput.Size = new System.Drawing.Size(938, 357);
            this.ebOutput.TabIndex = 5;
            // 
            // CheckDatabaseDLG
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(990, 539);
            this.Controls.Add(this.ebOutput);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.bnStart);
            this.Controls.Add(this.cbAutomaticDelete);
            this.Controls.Add(this.cbAutomaticFix);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MinimumSize = new System.Drawing.Size(838, 482);
            this.Name = "CheckDatabaseDLG";
            this.Text = "Check database";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CheckBox cbAutomaticFix;
        private CheckBox cbAutomaticDelete;
        private Button bnStart;
        private StatusStrip statusStrip1;
        private ToolStripProgressBar progressBar;
        private TextBox ebOutput;
    }
}
