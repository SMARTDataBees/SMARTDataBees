using Carbon.Plugins;
using SDBees.Plugs.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SDBees.Core.Model;

namespace SDBees.Core.Connectivity.SDBeesLink.Instances
{
    public class SDBeesDataSetEntityController : SDBees.Plugs.Properties.PropertyBag , IComparable<SDBeesDataSetEntityController>
    {
        private PluginContext m_context;
        private SDBeesEntity m_ent;
        private SDBeesEntityDefinition m_entityDefinition = null;
        private SDBeesMappedEntityDefinition m_mappedEntityDefinition = null;

        public SDBeesEntity MyEntity
        {
            get { return m_ent; }
            set { m_ent = value; }
        }

        private Guid m_guid = Guid.NewGuid();

        public SDBeesDataSetEntityController(SDBeesEntity ent, SDBeesEntityDefinition entityDefinition, SDBeesMappedEntityDefinition mappedEntityDefinition)
        {
            m_context = ConnectivityManager.Current.MyContext;

            m_ent = ent;

            m_entityDefinition = entityDefinition;

            m_mappedEntityDefinition = mappedEntityDefinition;

            this.SetValue += SDBeesDataSetEntityController_SetValue;
            this.GetValue += SDBeesDataSetEntityController_GetValue;

            FillPropertyRow();
        }

        void SDBeesDataSetEntityController_GetValue(object sender, PropertySpecEventArgs e)
        {
            foreach (SDBeesProperty item in m_ent.Properties)
            {
                if (item.DefinitionId.ToString() == e.Property.Name)
                    e.Value = item.InstanceValue.ObjectValue;
            }
            //base.OnGetValue(e);
        }

        void SDBeesDataSetEntityController_SetValue(object sender, PropertySpecEventArgs e)
        {
            foreach (SDBeesProperty item in m_ent.Properties)
            {
                if (item.DefinitionId.ToString() == e.Property.Name)
                {
                    item.InstanceValue.SetObjectValueManual(e.Value);
                }
            }
            //base.OnSetValue(e);
        }

        //private SDBeesEntityDefinition m_entityDef = null;

        //public SDBeesEntityDefinition EntityDef
        //{
        //    get { return m_entityDef; }
        //    set { m_entityDef = value; }
        //}

        private void FillPropertyRow()
        {
            if (m_entityDefinition != null)
            {
                bool _propFound = false;
                foreach (SDBeesProperty prop in m_ent.Properties)
                {
                    _propFound = false;
                    foreach (SDBeesPropertyDefinition propdef in m_entityDefinition.Properties)
                    {
                        if (propdef.Id == prop.DefinitionId)
                        {
                            bool mappedPropertyDefinitionEditable = true;

                            SDBeesMappedEntityDefinition mappedEntityDefinition = m_mappedEntityDefinition;
                            if (mappedEntityDefinition != null)
                            {
                                foreach (SDBeesMappedPropertyDefinition mappedPropertyDefiniton in mappedEntityDefinition.PropertyMappings)
                                {
                                    if (mappedPropertyDefiniton.PropertyDefinitionId == prop.DefinitionId)
                                    {
                                        mappedPropertyDefinitionEditable = mappedPropertyDefiniton.Editable;
                                        break;
                                    }
                                }
                            }

                            PropertySpec spec = new PropertySpec(prop.DefinitionId.ToString(), prop.InstanceValue.ObjectValue.GetType().ToString());
                            spec.ConverterTypeName = propdef.PropertyTypeConverter;
                            spec.EditorTypeName = propdef.PropertyUiTypeEditor;
                            spec.ReadOnlyProperty = mappedPropertyDefinitionEditable && propdef.Editable ? false : true;

                            this.Properties.Add(spec);

                            _propFound = true;
                            break;
                        }
                    }
                    if(!_propFound)
                    {
                        PropertySpec spec = new PropertySpec(prop.DefinitionId.ToString(), prop.InstanceValue.ObjectValue.GetType().ToString());
                        spec.ReadOnlyProperty = true;
                        this.Properties.Add(spec);
                    }
                }
            }
        }

        internal string GetColumnValueByName()
        {
            string result = "";

            string colName = SDBees.Core.Connectivity.SDBeesLink.UI.SDBeesDataSetDLG.SortColumn.Name;

            foreach (SDBeesProperty item in m_ent.Properties)
            {
                if (item.DefinitionId == colName)
                {
                    result = item.InstanceValue.ObjectValue.ToString();
                    break;
                }
            }
            return result;
        }

        public int CompareTo(SDBeesDataSetEntityController other)
        {            
            return this.GetColumnValueByName().CompareTo(other.GetColumnValueByName());
        }
    }
}
