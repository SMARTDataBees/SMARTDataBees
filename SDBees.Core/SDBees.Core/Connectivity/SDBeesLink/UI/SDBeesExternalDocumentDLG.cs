using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Windows.Forms;
using SDBees.DB;
using SDBees.Plugs.TemplateBase;
using SDBees.Plugs.TemplateTreeNode;
using Object = SDBees.DB.Object;

namespace SDBees.Core.Connectivity.SDBeesLink.UI
{
    public partial class SDBeesExternalDocumentDLG : Form
    {
        BindingList<TreenodePropertyRow> m_List;
        BindingSource m_Source;

        public SDBeesExternalDocumentDLG()
        {
            InitializeComponent();

            try
            {
                Text = ConfigurationManager.AppSettings["MainWindowTitle"] + @" - External document manager";
            }
            catch (Exception ex)
            {
            }
        }

        internal void UpdateView()
        {
            SetupControls();
            _dataGridView.DataSource = null;

            m_List = TemplateDBBaseData.GetTreeNodePropertyRowBindingList(ConnectivityManagerDocumentBaseData.gTable, ConnectivityManager.Current);
            m_Source = new BindingSource(m_List, null);
            _dataGridView.DataSource = m_Source;

        }

        private void SetupControls()
        {
            _dataGridView.AutoGenerateColumns = true;
            _dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            _dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells;
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
            Close();
        }

        private void m_dataGridViewExternalDocuments_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (_dataGridView.SelectedRows != null)
            {
                var row = m_List[e.RowIndex];
                object val = null;
                if (row.PropertyValues.TryGetValue(Object.m_IdSDBeesColumnDisplayName, out val))
                {
                    Error _error = null;
                    ArrayList _ids = null;

                    if (ConnectivityManagerAlienBaseData.GetAlienIdsByDocumentSDBeesId(val.ToString(), ref _error, ref _ids))
                    {
                        m_textBox.Text = String.Format("{0} alienids found for this docid {1}", _ids.Count, val);
                    }
                }
            }
        }

        private void m_modifyParentRelationsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
