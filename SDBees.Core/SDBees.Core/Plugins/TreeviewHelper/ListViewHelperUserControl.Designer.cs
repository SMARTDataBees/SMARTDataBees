namespace SDBees.Plugins.TreeviewHelper
{
    partial class ListViewHelperUserControl
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

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_dataGridViewChildElements = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewChildElements)).BeginInit();
            this.SuspendLayout();
            // 
            // m_dataGridViewChildElements
            // 
            this.m_dataGridViewChildElements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridViewChildElements.Location = new System.Drawing.Point(12, 27);
            this.m_dataGridViewChildElements.Name = "m_dataGridViewChildElements";
            this.m_dataGridViewChildElements.Size = new System.Drawing.Size(240, 150);
            this.m_dataGridViewChildElements.TabIndex = 0;
            // 
            // ListViewHelperUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.m_dataGridViewChildElements);
            this.Name = "ListViewHelperUserControl";
            this.Size = new System.Drawing.Size(422, 319);
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewChildElements)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView m_dataGridViewChildElements;

    }
}
