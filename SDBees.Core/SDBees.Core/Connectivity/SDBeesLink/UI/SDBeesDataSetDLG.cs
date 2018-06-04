using Carbon.Plugins;
using SDBees.Core.Connectivity.SDBeesLink.Instances;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

using SDBees.Core.Model;

namespace SDBees.Core.Connectivity.SDBeesLink.UI
{
    public partial class SDBeesDataSetDLG : Form, SDBees.Plugs.Explorer.iExplorer
    {
        PluginContext m_context;

        Dictionary<string, SDBeesEntityDefinition> m_entityDefinitions = null;

        Dictionary<string, SDBeesMappedEntityDefinition> m_mappedEntityDefinitions = null;

        public SDBeesDataSetDLG(Dictionary<string, SDBeesEntityDefinition> entityDefinitions)
        {
            InitializeComponent();

            this.m_context = ConnectivityManager.Current.MyContext;

            this.m_entityDefinitions = entityDefinitions;

            SetUpComponents();
        }

        private void SetUpComponents()
        {
            //this.m_dataGridViewSDBeesEntitys.Dock = DockStyle.Fill;

            try
            { 
                this.Text = System.Configuration.ConfigurationManager.AppSettings["MainWindowTitle"] + " DataSet-Manager";
            }
            catch (Exception ex)
            {
            }

            this.m_splitContainer.Dock = DockStyle.Fill;
            this.m_propertyGrid.Dock = DockStyle.Fill;
            this.m_dataGridViewSDBeesEntitys.Dock = DockStyle.Fill;
            this.m_dataGridViewSDBeesEntitys.SelectionChanged += m_dataGridViewSDBeesEntitys_SelectionChanged;
        }

        void m_dataGridViewSDBeesEntitys_SelectionChanged(object sender, EventArgs e)
        {
            if(sender is DataGridView)
            {
                if(this.m_dataGridViewSDBeesEntitys.SelectedRows.Count > 0)
                {
                    List<SDBeesDataSetEntityController> _controllers = new List<SDBeesDataSetEntityController>();
                    foreach (DataGridViewRow item in this.m_dataGridViewSDBeesEntitys.SelectedRows)
                    {
                        _controllers.Add(m_SDBeesDataSetEntControllers[item.Index]);
                    }

                    m_propertyGrid.SelectedObjects = _controllers.ToArray();
                }
            }
        }

        private SDBeesDataSet m_dataSet = null;
        public SDBeesDataSet MyDataSet
        {
            get { return m_dataSet; }
            set
            {
                m_dataSet = value;

                m_mappedEntityDefinitions = null;

                FillDataGridView();
            }
        }

        public Dictionary<string, SDBeesEntityDefinition> EntityDefinitions
        {
            get { return m_entityDefinitions; }
            set
            {
                m_entityDefinitions = value;

                FillDataGridView();
            }
        }

        SDBeesDataSetEntityControllerBindingList m_SDBeesDataSetEntControllers = new SDBeesDataSetEntityControllerBindingList();
        private void FillDataGridView()
        {
            if (m_dataSet != null)
            {
                foreach (SDBeesEntity entity in m_dataSet.Entities)
                {
                    SDBeesEntityDefinition entityDefinition = GetEntityDefinition(entity);

                    SDBeesMappedEntityDefinition mappedEntityDefinition = GetMappedEntityDefinition(entity);

                    SDBeesDataSetEntityController entControl = new SDBeesDataSetEntityController(entity, entityDefinition, mappedEntityDefinition);

                    m_SDBeesDataSetEntControllers.Add(entControl);
                }
                this.m_labelNumberOfItems.Text = String.Format("Number of items : {0}", m_dataSet.Entities.Count.ToString());
            }
        }

        private SDBeesEntityDefinition GetEntityDefinition(SDBeesEntity entity)
        {
            SDBeesEntityDefinition result = null;

            if (m_entityDefinitions != null)
            {
                if (m_entityDefinitions.ContainsKey(entity.DefinitionId.ToString()))
                {
                    result = m_entityDefinitions[entity.DefinitionId.ToString()];
                }
            }

            return result;
        }

        private SDBeesMappedEntityDefinition GetMappedEntityDefinition(SDBeesEntity entity)
        {
            SDBeesMappedEntityDefinition result = null;

            if (m_mappedEntityDefinitions == null)
            {
                m_mappedEntityDefinitions = new Dictionary<string, SDBeesMappedEntityDefinition>();

                foreach (SDBeesMappedEntityDefinition mappedEntityDefinition in m_dataSet.Mappings.EntityMappings)
                {
                    m_mappedEntityDefinitions[mappedEntityDefinition.MappedEntityDefinitionId.Id] = mappedEntityDefinition;
                }
            }

            if (m_mappedEntityDefinitions != null)
            {
                if (m_mappedEntityDefinitions.ContainsKey(entity.MappedDefinitionId.ToString()))
                {
                    result = m_mappedEntityDefinitions[entity.MappedDefinitionId.ToString()];
                }
            }

            return result;
        }

        private void SDBeesDataSetDLG_Load(object sender, EventArgs e)
        {
            this.m_dataGridViewSDBeesEntitys.AutoGenerateColumns = true;
            this.m_dataGridViewSDBeesEntitys.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.m_dataGridViewSDBeesEntitys.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.m_dataGridViewSDBeesEntitys.EditMode = DataGridViewEditMode.EditProgrammatically;
            this.m_dataGridViewSDBeesEntitys.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.m_dataGridViewSDBeesEntitys.MultiSelect = true;

            BindingSource bindingSource1 = new BindingSource();
            bindingSource1.DataSource = this.m_SDBeesDataSetEntControllers;
            //bindingSource1.DataMember = m_dataSet.entities;
            this.m_dataGridViewSDBeesEntitys.DataSource = bindingSource1;
        }

        private void SDBeesDataSetDLG_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        public Service.SDBeesExternalPluginService MyPluginService { get; set; }

        private void m_dataGridViewSDBeesEntitys_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (this.m_dataGridViewSDBeesEntitys != null)
            {
                Point point1 = this.m_dataGridViewSDBeesEntitys.CurrentCellAddress;
                if (point1.X-1 == e.ColumnIndex && point1.Y == e.RowIndex && e.Button == MouseButtons.Left && this.m_dataGridViewSDBeesEntitys.EditMode == DataGridViewEditMode.EditProgrammatically)
                {
                    SDBeesDataSetEntityController con = this.m_SDBeesDataSetEntControllers[e.RowIndex];

                    MyPluginService.ShowEntity(con.MyEntity.Id.ToString(), con.MyEntity.AlienIds);
                }
            }
        }


        private void SDBeesDataSetDLG_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        private void m_buttonOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void m_propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            this.m_dataGridViewSDBeesEntitys.Refresh();
        }

        static DataGridViewColumn newColumn = null;

        public static DataGridViewColumn SortColumn
        {
            get { return newColumn; }
        }

        public PluginContext Context
        {
            get
            {
                return m_context;
            }

            set
            {
                m_context = value;
            }
        }

        public string GetData(string name)
        {
            return null;
        }

        DataGridViewColumn oldColumn = null;
        ListSortDirection direction;

        private void m_dataGridViewSDBeesEntitys_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            newColumn = this.m_dataGridViewSDBeesEntitys.Columns[e.ColumnIndex];
            oldColumn = this.m_dataGridViewSDBeesEntitys.SortedColumn;
            // If oldColumn is null, then the DataGridView is not sorted.
            if (oldColumn != null)
            {
                // Sort the same column again, reversing the SortOrder.
                if (oldColumn == newColumn &&
                    this.m_dataGridViewSDBeesEntitys.SortOrder == SortOrder.Ascending)
                {
                    direction = ListSortDirection.Descending;
                }
                else
                {
                    // Sort a new column and remove the old SortGlyph.
                    direction = ListSortDirection.Ascending;
                    oldColumn.HeaderCell.SortGlyphDirection = SortOrder.None;
                }
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            // Sort the selected column.
            this.m_dataGridViewSDBeesEntitys.Sort(newColumn, direction);
            newColumn.HeaderCell.SortGlyphDirection =
                direction == ListSortDirection.Ascending ?
                SortOrder.Ascending : SortOrder.Descending;
        }

        private void m_dataGridViewSDBeesEntitys_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Put each of the columns into programmatic sort mode.
            foreach (DataGridViewColumn column in this.m_dataGridViewSDBeesEntitys.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Programmatic;
            }
        }

        public DialogResult ShowDialog(IWin32Window window, bool blockApplication)
        {
            return base.ShowDialog(window);
        }

        public void CloseDialog()
        {
            throw new NotImplementedException();
        }

        private class WindowHandle : IWin32Window
        {
            IntPtr _hwnd;

            public WindowHandle(IntPtr h)
            {
                _hwnd = h;
            }

            public IntPtr Handle
            {
                get
                {
                    return _hwnd;
                }
            }
        }
    }

    public class SDBeesDataSetEntityControllerBindingList : BindingList<SDBeesDataSetEntityController>
    {
        public SDBeesDataSetEntityControllerBindingList() : base()
        {
            
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        private bool m_isSortedCore = false;
        protected override bool IsSortedCore
        {
            get { return m_isSortedCore; }
        }

        private ListSortDirection m_sortDirectionCore = ListSortDirection.Ascending;
        protected override ListSortDirection SortDirectionCore
        {
            get { return m_sortDirectionCore; }
        }

        private PropertyDescriptor m_sortPropertyCore = null;
        protected override PropertyDescriptor SortPropertyCore
        {
            get { return m_sortPropertyCore; }
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            m_sortDirectionCore = direction;
            m_sortPropertyCore = prop;
            var listRef = this.Items as List<SDBeesDataSetEntityController>;
            if (listRef == null)
                return;
            var comparer = new SDBeesEntityComparer(prop, direction);

            listRef.Sort(comparer);

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }

    internal class SDBeesEntityComparer : IComparer<SDBeesDataSetEntityController>
    {
        private ListSortDirection m_direction = ListSortDirection.Ascending;

        public SDBeesEntityComparer(PropertyDescriptor propDesc, ListSortDirection direction)
        {
            m_direction = direction;
        }

        int IComparer<SDBeesDataSetEntityController>.Compare(SDBeesDataSetEntityController x, SDBeesDataSetEntityController y)
        {
            return x.CompareTo(y);
        }
    }
}
