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

namespace SDBees.GuiTools
{
    partial class AboutBox
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
            this.components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
            this._AssemblyInformation = new System.Windows.Forms.TextBox();
            this.bnClose = new System.Windows.Forms.Button();
            this.lbApplicationName = new System.Windows.Forms.Label();
            this.lbApplicationLocation = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this._panelLinkLabels = new System.Windows.Forms.Panel();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _AssemblyInformation
            // 
            this._AssemblyInformation.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._AssemblyInformation.BackColor = System.Drawing.Color.White;
            this._AssemblyInformation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._AssemblyInformation.Location = new System.Drawing.Point(142, 103);
            this._AssemblyInformation.Multiline = true;
            this._AssemblyInformation.Name = "_AssemblyInformation";
            this._AssemblyInformation.ReadOnly = true;
            this._AssemblyInformation.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this._AssemblyInformation.Size = new System.Drawing.Size(222, 322);
            this._AssemblyInformation.TabIndex = 0;
            this._AssemblyInformation.TabStop = false;
            // 
            // bnClose
            // 
            this.bnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bnClose.Location = new System.Drawing.Point(537, 584);
            this.bnClose.Name = "bnClose";
            this.bnClose.Size = new System.Drawing.Size(75, 23);
            this.bnClose.TabIndex = 4;
            this.bnClose.Text = "Close";
            this.bnClose.UseVisualStyleBackColor = true;
            this.bnClose.Click += new System.EventHandler(this.bnClose_Click);
            // 
            // lbApplicationName
            // 
            this.lbApplicationName.AutoSize = true;
            this.lbApplicationName.Location = new System.Drawing.Point(12, 9);
            this.lbApplicationName.Name = "lbApplicationName";
            this.lbApplicationName.Size = new System.Drawing.Size(46, 13);
            this.lbApplicationName.TabIndex = 0;
            this.lbApplicationName.Text = "Program";
            // 
            // lbApplicationLocation
            // 
            this.lbApplicationLocation.AutoSize = true;
            this.lbApplicationLocation.Location = new System.Drawing.Point(12, 34);
            this.lbApplicationLocation.Name = "lbApplicationLocation";
            this.lbApplicationLocation.Size = new System.Drawing.Size(46, 13);
            this.lbApplicationLocation.TabIndex = 1;
            this.lbApplicationLocation.Text = "Program";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(15, 50);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(597, 528);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this._panelLinkLabels);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(589, 502);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Licenses";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // _panelLinkLabels
            // 
            this._panelLinkLabels.AutoScroll = true;
            this._panelLinkLabels.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._panelLinkLabels.Location = new System.Drawing.Point(126, 85);
            this._panelLinkLabels.Name = "_panelLinkLabels";
            this._panelLinkLabels.Size = new System.Drawing.Size(150, 150);
            this._panelLinkLabels.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this._AssemblyInformation);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(589, 502);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Components";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // AboutBox
            // 
            this.AcceptButton = this.bnClose;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(624, 619);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lbApplicationLocation);
            this.Controls.Add(this.lbApplicationName);
            this.Controls.Add(this.bnClose);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Program Info";
            this.Load += new System.EventHandler(this.AboutBox_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox _AssemblyInformation;
        private Button bnClose;
        private Label lbApplicationName;
        private Label lbApplicationLocation;
        private ToolTip toolTip1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Panel _panelLinkLabels;
    }
}
