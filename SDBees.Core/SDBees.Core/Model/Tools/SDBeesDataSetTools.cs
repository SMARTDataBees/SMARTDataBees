namespace SDBees.Core.Model
{
    public class SDBeesDataSetTools
    {
        public static void normalizePropertyValues (SDBeesDataSet data)
        {
            foreach (var entity in data.Entities)
            {
                normalizePropertyValues(entity);
            }
        }

        public static void normalizePropertyValues (SDBeesEntity entity)
        {
            foreach (var property in entity.Properties)
            {
                normalizePropertyValues (property);
            }
        }

        public static void normalizePropertyValues(SDBeesProperty property)
        {
            if (property.InstanceValue == null)
            {
                property.InstanceValue = new SDBeesPropertyValue();
            }
            if (property.InstanceValue.ObjectValue == null)
            {
                property.InstanceValue.SetObjectValue("");
            }
        }
    }
}
