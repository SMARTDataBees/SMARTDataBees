using System.ComponentModel;
using System.Windows.Forms;

namespace SDBees.Core.Connectivity.SDBeesLink.UI
{
    partial class SDBeesExternalDocumentDLG
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
            this.m_dataGridViewExternalDocuments = new System.Windows.Forms.DataGridView();
            this.m_contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.m_modifyParentRelationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button1 = new System.Windows.Forms.Button();
            this.m_textBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewExternalDocuments)).BeginInit();
            this.m_contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_dataGridViewExternalDocuments
            // 
            this.m_dataGridViewExternalDocuments.AllowUserToAddRows = false;
            this.m_dataGridViewExternalDocuments.AllowUserToDeleteRows = false;
            this.m_dataGridViewExternalDocuments.AllowUserToResizeColumns = false;
            this.m_dataGridViewExternalDocuments.AllowUserToResizeRows = false;
            this.m_dataGridViewExternalDocuments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_dataGridViewExternalDocuments.BackgroundColor = System.Drawing.SystemColors.ControlLightLight;
            this.m_dataGridViewExternalDocuments.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.m_dataGridViewExternalDocuments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridViewExternalDocuments.ContextMenuStrip = this.m_contextMenuStrip;
            this.m_dataGridViewExternalDocuments.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.m_dataGridViewExternalDocuments.Location = new System.Drawing.Point(12, 12);
            this.m_dataGridViewExternalDocuments.MultiSelect = false;
            this.m_dataGridViewExternalDocuments.Name = "m_dataGridViewExternalDocuments";
            this.m_dataGridViewExternalDocuments.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.m_dataGridViewExternalDocuments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_dataGridViewExternalDocuments.Size = new System.Drawing.Size(470, 340);
            this.m_dataGridViewExternalDocuments.TabIndex = 0;
            this.m_dataGridViewExternalDocuments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.m_dataGridViewExternalDocuments_CellDoubleClick);
            this.m_dataGridViewExternalDocuments.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.m_dataGridViewExternalDocuments_RowHeaderMouseClick);
            // 
            // m_contextMenuStrip
            // 
            this.m_contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.m_modifyParentRelationsToolStripMenuItem});
            this.m_contextMenuStrip.Name = "m_contextMenuStrip";
            this.m_contextMenuStrip.Size = new System.Drawing.Size(210, 26);
            // 
            // m_modifyParentRelationsToolStripMenuItem
            // 
            this.m_modifyParentRelationsToolStripMenuItem.Name = "m_modifyParentRelationsToolStripMenuItem";
            this.m_modifyParentRelationsToolStripMenuItem.Size = new System.Drawing.Size(209, 22);
            this.m_modifyParentRelationsToolStripMenuItem.Text = "Modify parent relations ...";
            this.m_modifyParentRelationsToolStripMenuItem.Click += new System.EventHandler(this.m_modifyParentRelationsToolStripMenuItem_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(407, 364);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "OK";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // m_textBox
            // 
            this.m_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_textBox.Enabled = false;
            this.m_textBox.Location = new System.Drawing.Point(12, 365);
            this.m_textBox.Name = "m_textBox";
            this.m_textBox.ReadOnly = true;
            this.m_textBox.Size = new System.Drawing.Size(362, 20);
            this.m_textBox.TabIndex = 2;
            // 
            // SDBeesExternalDocumentDLG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(494, 399);
            this.Controls.Add(this.m_textBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.m_dataGridViewExternalDocuments);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "SDBeesExternalDocumentDLG";
            this.Text = "SDBeesExternalDocumentDLG";
            this.Load += new System.EventHandler(this.SDBeesExternalDocumentDLG_Load);
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewExternalDocuments)).EndInit();
            this.m_contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DataGridView m_dataGridViewExternalDocuments;
        private Button button1;
        private TextBox m_textBox;
        private ContextMenuStrip m_contextMenuStrip;
        private ToolStripMenuItem m_modifyParentRelationsToolStripMenuItem;
    }
}