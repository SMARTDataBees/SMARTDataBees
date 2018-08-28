using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;
using System.Collections;
using SDBees.ViewAdmin;

namespace SDBees.Plugins.TreeviewHelper
{
    public partial class ListViewHelperUserControl : UserControl
    {
        ListViewHelper m_parent;
        Plugs.TemplateTreeNode.TemplateTreenodeTag m_ChildTemplateTreenodeTag;
        Plugs.TemplateTreeNode.TemplateTreenodeTag m_ParentTemplateTreenodeTag;
        Guid m_ViewId;

        public ListViewHelperUserControl(ListViewHelper parent)
        {
            m_parent = parent;

            InitializeComponent();

            FormatElements();
        }

        public Plugs.TemplateTreeNode.TemplateTreenodeTag ChildTemplateTreenodeTag
        {
            get { return m_ChildTemplateTreenodeTag; }
            set { m_ChildTemplateTreenodeTag = value; }
        }

        public Plugs.TemplateTreeNode.TemplateTreenodeTag ParentTemplateTreenodeTag
        {
            get { return m_ParentTemplateTreenodeTag; }
            set { m_ParentTemplateTreenodeTag = value; }
        }

        public Database Database
        {
            get { return m_parent.MyDBManager.Database; }
        }

        public Guid ViewId
        {
            get { return m_ViewId; }
            set { m_ViewId = value; }
        }

        private void FormatElements()
        {
            //this.m_dataGridViewChildElements.Dock = DockStyle.Fill;

            this.m_dataGridViewChildElements.AutoGenerateColumns = true;
            this.m_dataGridViewChildElements.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.m_dataGridViewChildElements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            this.m_dataGridViewChildElements.EditMode = DataGridViewEditMode.EditOnEnter;

        }

        internal void UpdateView()
        {
            m_dataGridViewChildElements.DataSource = null;

            // Zunächst die PlugIn-Typen bestimmen, die unter dem aktuellen Knoten eingefügt werden können...
            Error error = null;
            var m_parentType = "parent";
            if (m_ParentTemplateTreenodeTag != null)
            {
                m_parentType = m_ParentTemplateTreenodeTag.NodeTypeOf;
            }
            else
            {
                return;
            }

            var m_childType = "child";
            if (m_ChildTemplateTreenodeTag != null)
            {
                m_childType = m_ChildTemplateTreenodeTag.NodeTypeOf;
            }

            var m_dataList = new List<SDBees.Plugs.TemplateTreeNode.TreenodePropertyRow>();
            Guid guidParent;
            Guid guidSelectedChild;
            if (!Guid.TryParse(m_ParentTemplateTreenodeTag.NodeGUID, out guidParent))
                return;
            if (!Guid.TryParse(m_ChildTemplateTreenodeTag.NodeGUID, out guidSelectedChild))
                return;

            var objectIds = new ArrayList();
            ViewDefinition.FindViewDefinitionsByTypes(Database, ref objectIds, m_ViewId, m_parentType, m_childType, ref error);

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

                    Guid guidChild;
                    foreach (string item in childIds)
                    {
                        if (!Guid.TryParse(item, out guidChild))
                            continue;

                        var viewRelIds = new ArrayList();
                        if (ViewRelation.FindViewRelationForView(Database, m_ViewId, guidParent, guidChild, ref viewRelIds, ref error) > 0)
                        {
                            var tempTag = new TemplateTreenodeTag();
                            tempTag.NodeGUID = guidChild.ToString(); ;
                            var propTable = new TreenodePropertyRow(m_parent.MyDBManager, viewDef.ChildType, item);
                            m_dataList.Add(propTable);
                        }
                    }
                    //List<TemplateTreenode> _lstTrnNodes = TemplateTreenode.GetAllPlugins();

                    //foreach (TemplateTreenode item in _lstTrnNodes)
                    //{
                    //    TemplateTreenodeTag _tag = item.MyTag();
                    //    Guid childId;
                    //    if(Guid.TryParse(_tag.NodeGUID, out childId))
                    //    {
                    //        if((item.MyTag().NodeTypeOf == m_childType) && 
                    //            (childIds.Contains(_tag.NodeGUID)) &&
                    //            ViewRelations.ChildReferencedByView(m_Database, m_ViewId, childId, ref error))
                    //        {
                    //            SDBees.Plugs.TemplateTreeNode.TreenodePropertyTable propTable = treenodePlugin.GetTreenodePropertyTable(m_ChildTemplateTreenodeTag);
                    //            
                    //        }
                    //    }
                    //}
                }
                break;
            }

            m_dataGridViewChildElements.DataSource = m_dataList;
        }
    }
}
