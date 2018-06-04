using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDBees.Core.Connectivity.SDBeesLink.UI
{
    public partial class SDBeesExternalDocumentDLG : Form
    {
        BindingList<TreenodePropertyRow> m_List = null;
        BindingSource m_Source = null;

        public SDBeesExternalDocumentDLG()
        {
            InitializeComponent();

            try
            {
                this.Text = System.Configuration.ConfigurationManager.AppSettings["MainWindowTitle"] + " - External file manager";
            }
            catch (Exception ex)
            {
            }
        }

        internal void UpdateView()
        {
            SetupControls();
            this.m_dataGridViewExternalDocuments.DataSource = null;

            m_List = ConnectivityManagerDocumentBaseData.GetTreeNodePropertyRowBindingList(ConnectivityManagerDocumentBaseData.gTable, ConnectivityManager.Current);
            m_Source = new BindingSource(m_List, null);
            this.m_dataGridViewExternalDocuments.DataSource = m_Source;

        }

        private void SetupControls()
        {
            this.m_dataGridViewExternalDocuments.AutoGenerateColumns = true;
            this.m_dataGridViewExternalDocuments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.m_dataGridViewExternalDocuments.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
        }

        private void SDBeesExternalDocumentDLG_Load(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void m_dataGridViewExternalDocuments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void m_dataGridViewExternalDocuments_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (m_dataGridViewExternalDocuments.SelectedRows != null)
            {
                TreenodePropertyRow row = m_List[e.RowIndex];
                object val = null;
                if (row.PropertyValues.TryGetValue(SDBees.DB.Object.m_IdSDBeesColumnDisplayName, out val))
                {
                    Error _error = null;
                    ArrayList _ids = null;

                    if (ConnectivityManagerAlienBaseData.GetAlienIdsByDocumentSDBeesId(val.ToString(), ref _error, ref _ids))
                    {
                        this.m_textBox.Text = String.Format("{0} alienids found for this docid {1}", _ids.Count, val);
                    }
                }
            }
        }

        private void m_modifyParentRelationsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
