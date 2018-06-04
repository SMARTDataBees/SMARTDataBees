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
namespace SDBees.ViewAdmin
{
  partial class ViewAdminAddNew
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
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
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.label1 = new System.Windows.Forms.Label();
      this.textBoxViewName = new System.Windows.Forms.TextBox();
      this.buttonOK = new System.Windows.Forms.Button();
      this.buttonChancel = new System.Windows.Forms.Button();
      this.richTextBoxDescription = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(61, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "View Name";
      // 
      // textBoxViewName
      // 
      this.textBoxViewName.Location = new System.Drawing.Point(12, 25);
      this.textBoxViewName.Name = "textBoxViewName";
      this.textBoxViewName.Size = new System.Drawing.Size(304, 20);
      this.textBoxViewName.TabIndex = 1;
      // 
      // buttonOK
      // 
      this.buttonOK.Location = new System.Drawing.Point(12, 106);
      this.buttonOK.Name = "buttonOK";
      this.buttonOK.Size = new System.Drawing.Size(75, 23);
      this.buttonOK.TabIndex = 2;
      this.buttonOK.Text = "OK";
      this.buttonOK.UseVisualStyleBackColor = true;
      this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
      // 
      // buttonChancel
      // 
      this.buttonChancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.buttonChancel.Location = new System.Drawing.Point(241, 106);
      this.buttonChancel.Name = "buttonChancel";
      this.buttonChancel.Size = new System.Drawing.Size(75, 23);
      this.buttonChancel.TabIndex = 3;
      this.buttonChancel.Text = "Chancel";
      this.buttonChancel.UseVisualStyleBackColor = true;
      this.buttonChancel.Click += new System.EventHandler(this.buttonChancel_Click);
      // 
      // richTextBoxDescription
      // 
      this.richTextBoxDescription.Location = new System.Drawing.Point(12, 51);
      this.richTextBoxDescription.Name = "richTextBoxDescription";
      this.richTextBoxDescription.Size = new System.Drawing.Size(304, 49);
      this.richTextBoxDescription.TabIndex = 4;
      this.richTextBoxDescription.Text = "";
      // 
      // ViewAdminAddNew
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(328, 141);
      this.ControlBox = false;
      this.Controls.Add(this.richTextBoxDescription);
      this.Controls.Add(this.buttonChancel);
      this.Controls.Add(this.buttonOK);
      this.Controls.Add(this.textBoxViewName);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ViewAdminAddNew";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "ViewAdminAddNew";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBoxViewName;
    private System.Windows.Forms.Button buttonOK;
    private System.Windows.Forms.Button buttonChancel;
    private System.Windows.Forms.RichTextBox richTextBoxDescription;
  }
}
