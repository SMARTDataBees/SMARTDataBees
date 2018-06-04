namespace SDBees.Demo
{
    partial class Form1
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
            this._buttonMySQL = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._buttonSDBeesStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _buttonMySQL
            // 
            this._buttonMySQL.Location = new System.Drawing.Point(117, 37);
            this._buttonMySQL.Name = "_buttonMySQL";
            this._buttonMySQL.Size = new System.Drawing.Size(75, 23);
            this._buttonMySQL.TabIndex = 0;
            this._buttonMySQL.Text = "Start";
            this._buttonMySQL.UseVisualStyleBackColor = true;
            this._buttonMySQL.Click += new System.EventHandler(this._buttonMySQLStart_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "MySQL";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "SmartDataBees";
            // 
            // _buttonSDBeesStart
            // 
            this._buttonSDBeesStart.Location = new System.Drawing.Point(117, 79);
            this._buttonSDBeesStart.Name = "_buttonSDBeesStart";
            this._buttonSDBeesStart.Size = new System.Drawing.Size(75, 23);
            this._buttonSDBeesStart.TabIndex = 2;
            this._buttonSDBeesStart.Text = "Start";
            this._buttonSDBeesStart.UseVisualStyleBackColor = true;
            this._buttonSDBeesStart.Click += new System.EventHandler(this._buttonSDBeesStart_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(367, 195);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._buttonSDBeesStart);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._buttonMySQL);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _buttonMySQL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _buttonSDBeesStart;
    }
}

