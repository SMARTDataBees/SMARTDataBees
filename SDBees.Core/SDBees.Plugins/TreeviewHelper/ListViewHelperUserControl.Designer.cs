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
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewChildElements)).BeginInit();
            this.SuspendLayout();
            // 
            // m_dataGridViewChildElements
            // 
            this.m_dataGridViewChildElements.AllowUserToAddRows = false;
            this.m_dataGridViewChildElements.AllowUserToDeleteRows = false;
            this.m_dataGridViewChildElements.AllowUserToResizeRows = false;
            this.m_dataGridViewChildElements.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_dataGridViewChildElements.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.m_dataGridViewChildElements.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridViewChildElements.Location = new System.Drawing.Point(0, 81);
            this.m_dataGridViewChildElements.Margin = new System.Windows.Forms.Padding(6);
            this.m_dataGridViewChildElements.MultiSelect = false;
            this.m_dataGridViewChildElements.Name = "m_dataGridViewChildElements";
            this.m_dataGridViewChildElements.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.m_dataGridViewChildElements.Size = new System.Drawing.Size(768, 502);
            this.m_dataGridViewChildElements.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(312, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Objectlist same type on same level";
            // 
            // ListViewHelperUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.m_dataGridViewChildElements);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ListViewHelperUserControl";
            this.Size = new System.Drawing.Size(774, 588);
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewChildElements)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView m_dataGridViewChildElements;
        private System.Windows.Forms.Label label1;
    }
}
