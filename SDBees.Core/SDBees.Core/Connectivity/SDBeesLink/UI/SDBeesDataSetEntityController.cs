using System;
using SDBees.Core.Connectivity.SDBeesLink.UI;
using SDBees.Core.Model;
using SDBees.Plugs.Properties;

namespace SDBees.Core.Connectivity.SDBeesLink.Instances
{
    public class SDBeesDataSetEntityController : PropertyBag , IComparable<SDBeesDataSetEntityController>
    {
        private SDBeesEntity m_ent;
        private SDBeesEntityDefinition m_entityDefinition;
        private SDBeesMappedEntityDefinition m_mappedEntityDefinition;

        public SDBeesEntity MyEntity
        {
            get { return m_ent; }
            set { m_ent = value; }
        }

        private Guid m_guid = Guid.NewGuid();

        public SDBeesDataSetEntityController(SDBeesEntity ent, SDBeesEntityDefinition entityDefinition, SDBeesMappedEntityDefinition mappedEntityDefinition)
        {
            m_ent = ent;

            m_entityDefinition = entityDefinition;

            m_mappedEntityDefinition = mappedEntityDefinition;

            SetValue += SDBeesDataSetEntityController_SetValue;
            GetValue += SDBeesDataSetEntityController_GetValue;

            FillPropertyRow();
        }

        void SDBeesDataSetEntityController_GetValue(object sender, PropertySpecEventArgs e)
        {
            foreach (var item in m_ent.Properties)
            {
                if (item.DefinitionId.ToString() == e.Property.Name)
                    e.Value = item.InstanceValue.ObjectValue;
            }
            //base.OnGetValue(e);
        }

        void SDBeesDataSetEntityController_SetValue(object sender, PropertySpecEventArgs e)
        {
            foreach (var item in m_ent.Properties)
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
                var _propFound = false;
                foreach (var prop in m_ent.Properties)
                {
                    _propFound = false;
                    foreach (var propdef in m_entityDefinition.Properties)
                    {
                        if (propdef.Id == prop.DefinitionId)
                        {
                            var mappedPropertyDefinitionEditable = true;

                            var mappedEntityDefinition = m_mappedEntityDefinition;
                            if (mappedEntityDefinition != null)
                            {
                                foreach (var mappedPropertyDefiniton in mappedEntityDefinition.PropertyMappings)
                                {
                                    if (mappedPropertyDefiniton.PropertyDefinitionId == prop.DefinitionId)
                                    {
                                        mappedPropertyDefinitionEditable = mappedPropertyDefiniton.Editable;
                                        break;
                                    }
                                }
                            }

                            var spec = new PropertySpec(prop.DefinitionId.ToString(), prop.InstanceValue.ObjectValue.GetType().ToString());
                            spec.ConverterTypeName = propdef.PropertyTypeConverter;
                            spec.EditorTypeName = propdef.PropertyUiTypeEditor;
                            spec.ReadOnlyProperty = mappedPropertyDefinitionEditable && propdef.Editable ? false : true;

                            Properties.Add(spec);

                            _propFound = true;
                            break;
                        }
                    }
                    if(!_propFound)
                    {
                        var spec = new PropertySpec(prop.DefinitionId.ToString(), prop.InstanceValue.ObjectValue.GetType().ToString());
                        spec.ReadOnlyProperty = true;
                        Properties.Add(spec);
                    }
                }
            }
        }

        internal string GetColumnValueByName()
        {
            var result = "";

            var colName = SDBeesDataSetDLG.SortColumn.Name;

            foreach (var item in m_ent.Properties)
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
            return GetColumnValueByName().CompareTo(other.GetColumnValueByName());
        }
    }
}
