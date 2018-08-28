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

namespace SDBees.ViewAdmin
{
  partial class ViewAdminAddNew
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gel�scht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode f�r die Designerunterst�tzung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor ge�ndert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.label1 = new Label();
      this.textBoxViewName = new TextBox();
      this.buttonOK = new Button();
      this.buttonChancel = new Button();
      this.richTextBoxDescription = new RichTextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new Size(61, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "View Name";
      // 
      // textBoxViewName
      // 
      this.textBoxViewName.Location = new Point(12, 25);
      this.textBoxViewName.Name = "textBoxViewName";
      this.textBoxViewName.Size = new Size(304, 20);
      this.textBoxViewName.TabIndex = 1;
      // 
      // buttonOK
      // 
      this.buttonOK.Location = new Point(12, 106);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new Size(75, 23);
      this.buttonOK.TabIndex = 2;
      this.buttonOK.Text = "OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      this.buttonOK.Click += new EventHandler(this.buttonOK_Click);
      // 
      // buttonChancel
      // 
      this.buttonChancel.DialogResult = DialogResult.Cancel;
      this.buttonChancel.Location = new Point(241, 106);
      this.buttonChancel.Name = "buttonChancel";
      this.buttonChancel.Size = new Size(75, 23);
      this.buttonChancel.TabIndex = 3;
      this.buttonChancel.Text = "Chancel";
      this.buttonChancel.UseVisualStyleBackColor = true;
      this.buttonChancel.Click += new EventHandler(this.buttonChancel_Click);
      // 
      // richTextBoxDescription
      // 
      this.richTextBoxDescription.Location = new Point(12, 51);
      this.richTextBoxDescription.Name = "richTextBoxDescription";
      this.richTextBoxDescription.Size = new Size(304, 49);
      this.richTextBoxDescription.TabIndex = 4;
      this.richTextBoxDescription.Text = "";
      // 
      // ViewAdminAddNew
      // 
      this.AutoScaleDimensions = new SizeF(6F, 13F);
      this.AutoScaleMode = AutoScaleMode.Font;
      this.ClientSize = new Size(328, 141);
      this.ControlBox = false;
      this.Controls.Add(this.richTextBoxDescription);
      this.Controls.Add(this.buttonChancel);
      this.Controls.Add(this.buttonOK);
      this.Controls.Add(this.textBoxViewName);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ViewAdminAddNew";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = FormStartPosition.CenterParent;
      this.Text = "ViewAdminAddNew";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Label label1;
    private TextBox textBoxViewName;
    private Button buttonOK;
    private Button buttonChancel;
    private RichTextBox richTextBoxDescription;
  }
}
