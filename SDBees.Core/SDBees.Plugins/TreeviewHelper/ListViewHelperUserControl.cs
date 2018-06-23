using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;
using System.Collections;
using SDBees.Core.Admin;

namespace SDBees.Plugins.TreeviewHelper
{
    public partial class ListViewHelperUserControl : UserControl
    {
        private readonly ListViewHelper _parent;

        public ListViewHelperUserControl(ListViewHelper parent)
        {
            _parent = parent;

            InitializeComponent();

            FormatElements();
        }

        public TemplateTreenodeTag ChildTemplateTreenodeTag { get; set; }

        public TemplateTreenodeTag ParentTemplateTreenodeTag { get; set; }

        public Database Database => _parent.MyDBManager.Database;

        public Guid ViewId { get; set; }

        private void FormatElements()
        {
            _dataGridView.AutoGenerateColumns = true;
            _dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            _dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            _dataGridView.EditMode = DataGridViewEditMode.EditOnEnter;

        }

        internal void UpdateView()
        {
            _dataGridView.DataSource = null;

            // Zunächst die PlugIn-Typen bestimmen, die unter dem aktuellen Knoten eingefügt werden können...
            Error error = null;
            var m_parentType = "parent";
            if (ParentTemplateTreenodeTag != null)
            {
                m_parentType = ParentTemplateTreenodeTag.NodeTypeOf;
            }
            else
            {
                return;
            }

            var childType = "child";
            if (ChildTemplateTreenodeTag != null)
            {
                childType = ChildTemplateTreenodeTag.NodeTypeOf;
            }

            var dataList = new List<TreenodePropertyRow>();
            if (!Guid.TryParse(ParentTemplateTreenodeTag.NodeGUID, out var guidParent))
                return;
            if (!Guid.TryParse(ChildTemplateTreenodeTag.NodeGUID, out _))
                return;

            var objectIds = new ArrayList();
            ViewDefinition.FindViewDefinitionsByTypes(Database, ref objectIds, ViewId, m_parentType, childType, ref error);

            foreach (var objectId in objectIds)
            {
                var viewDef = new ViewDefinition();
                if (viewDef.Load(Database, objectId, ref error))
                {
                    // Jetzt aus dem PlugIn alle persistenten Objekte bestimmen...
                    // Das Plugin anlegen
                    var treenodePlugin = TemplateTreenode.GetPluginForType(viewDef.ChildType);

                    // Alle Objekte in der Datenbank besorgen
                    // Wir bekommen alle, nicht nur die der aktuellen View!
                    var childIds = new ArrayList();
                    treenodePlugin.FindAllObjects(Database, ref childIds, ref error);

                    foreach (string item in childIds)
                    {
                        if (!Guid.TryParse(item, out var guidChild))
                            continue;

                        var viewRelIds = new ArrayList();
                        if (ViewRelation.FindViewRelationForView(Database, ViewId, guidParent, guidChild, ref viewRelIds, ref error) > 0)
                        {
                            var propTable = new TreenodePropertyRow(_parent.MyDBManager, viewDef.ChildType, item);
                            dataList.Add(propTable);
                        }
                    }
                }
                break;
            }

            _dataGridView.DataSource = dataList;
        }
    }
}
