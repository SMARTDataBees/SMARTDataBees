namespace SDBees.Core.Connectivity.SDBeesLink.UI
{
    partial class SDBeesDataSetDLG
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.m_dataGridViewSDBeesEntitys = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.m_splitContainer = new System.Windows.Forms.SplitContainer();
            this.m_propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.m_buttonOK = new System.Windows.Forms.Button();
            this.m_labelNumberOfItems = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewSDBeesEntitys)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.m_splitContainer)).BeginInit();
            this.m_splitContainer.Panel1.SuspendLayout();
            this.m_splitContainer.Panel2.SuspendLayout();
            this.m_splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_dataGridViewSDBeesEntitys
            // 
            this.m_dataGridViewSDBeesEntitys.AllowUserToAddRows = false;
            this.m_dataGridViewSDBeesEntitys.AllowUserToDeleteRows = false;
            this.m_dataGridViewSDBeesEntitys.AllowUserToResizeRows = false;
            this.m_dataGridViewSDBeesEntitys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_dataGridViewSDBeesEntitys.BackgroundColor = System.Drawing.Color.White;
            this.m_dataGridViewSDBeesEntitys.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.m_dataGridViewSDBeesEntitys.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.m_dataGridViewSDBeesEntitys.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.m_dataGridViewSDBeesEntitys.Location = new System.Drawing.Point(30, 42);
            this.m_dataGridViewSDBeesEntitys.Name = "m_dataGridViewSDBeesEntitys";
            this.m_dataGridViewSDBeesEntitys.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.m_dataGridViewSDBeesEntitys.Size = new System.Drawing.Size(490, 364);
            this.m_dataGridViewSDBeesEntitys.TabIndex = 0;
            this.m_dataGridViewSDBeesEntitys.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.m_dataGridViewSDBeesEntitys_ColumnHeaderMouseClick);
            this.m_dataGridViewSDBeesEntitys.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.m_dataGridViewSDBeesEntitys_DataBindingComplete);
            this.m_dataGridViewSDBeesEntitys.RowHeaderMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.m_dataGridViewSDBeesEntitys_RowHeaderMouseDoubleClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.m_splitContainer);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(914, 481);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "DataSet Items";
            // 
            // m_splitContainer
            // 
            this.m_splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_splitContainer.Location = new System.Drawing.Point(3, 16);
            this.m_splitContainer.Name = "m_splitContainer";
            // 
            // m_splitContainer.Panel1
            // 
            this.m_splitContainer.Panel1.Controls.Add(this.m_dataGridViewSDBeesEntitys);
            // 
            // m_splitContainer.Panel2
            // 
            this.m_splitContainer.Panel2.Controls.Add(this.m_propertyGrid);
            this.m_splitContainer.Panel2MinSize = 250;
            this.m_splitContainer.Size = new System.Drawing.Size(908, 462);
            this.m_splitContainer.SplitterDistance = 568;
            this.m_splitContainer.TabIndex = 2;
            // 
            // m_propertyGrid
            // 
            this.m_propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.m_propertyGrid.CategoryForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.m_propertyGrid.Location = new System.Drawing.Point(89, 23);
            this.m_propertyGrid.Name = "m_propertyGrid";
            this.m_propertyGrid.Size = new System.Drawing.Size(181, 402);
            this.m_propertyGrid.TabIndex = 1;
            this.m_propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.m_propertyGrid_PropertyValueChanged);
            // 
            // m_buttonOK
            // 
            this.m_buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.m_buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.m_buttonOK.Location = new System.Drawing.Point(851, 499);
            this.m_buttonOK.Name = "m_buttonOK";
            this.m_buttonOK.Size = new System.Drawing.Size(75, 23);
            this.m_buttonOK.TabIndex = 2;
            this.m_buttonOK.Text = "OK";
            this.m_buttonOK.UseVisualStyleBackColor = true;
            this.m_buttonOK.Click += new System.EventHandler(this.m_buttonOK_Click);
            // 
            // m_labelNumberOfItems
            // 
            this.m_labelNumberOfItems.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.m_labelNumberOfItems.AutoSize = true;
            this.m_labelNumberOfItems.Location = new System.Drawing.Point(12, 512);
            this.m_labelNumberOfItems.Name = "m_labelNumberOfItems";
            this.m_labelNumberOfItems.Size = new System.Drawing.Size(92, 13);
            this.m_labelNumberOfItems.TabIndex = 3;
            this.m_labelNumberOfItems.Text = "Number of items : ";
            // 
            // SDBeesDataSetDLG
            // 
            this.AcceptButton = this.m_buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(938, 534);
            this.Controls.Add(this.m_labelNumberOfItems);
            this.Controls.Add(this.m_buttonOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SDBeesDataSetDLG";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SDBeesDataSetDLG";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SDBeesDataSetDLG_FormClosing);
            this.Load += new System.EventHandler(this.SDBeesDataSetDLG_Load);
            this.Shown += new System.EventHandler(this.SDBeesDataSetDLG_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.m_dataGridViewSDBeesEntitys)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.m_splitContainer.Panel1.ResumeLayout(false);
            this.m_splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.m_splitContainer)).EndInit();
            this.m_splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView m_dataGridViewSDBeesEntitys;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button m_buttonOK;
        private System.Windows.Forms.PropertyGrid m_propertyGrid;
        private System.Windows.Forms.SplitContainer m_splitContainer;
        private System.Windows.Forms.Label m_labelNumberOfItems;
    }
}