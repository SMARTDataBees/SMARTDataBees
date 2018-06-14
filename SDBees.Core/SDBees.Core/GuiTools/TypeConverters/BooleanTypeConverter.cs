using System;
using System.ComponentModel;
using System.Globalization;

namespace SDBees.Core.GuiTools.TypeConverters
{
    public class BooleanTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result = null; // base.ConvertFrom(context, culture, value);
            var boolValue = false;

            if(bool.TryParse(value.ToString(), out boolValue))
            {
                result = boolValue;
            }
            else
            {
                switch (value.ToString())
                {
                    case "1":
                        result = 1;
                        break;
                    case "0":
                        result = 0;
                        break;
                    default:
                        break;
                }
            }

            return result; 
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null; //= base.ConvertTo(context, culture, value, destinationType);
            bool mybool;

            mybool = false;
            if (bool.TryParse(value.ToString(), out mybool))
            {
                
            }
            else
            {
                if (value.ToString().ToLower() == "0")
                    mybool = false;
                else if (value.ToString().ToLower() == "1")
                    mybool = true;
            }

            if (mybool != null && destinationType == typeof(string))
            {
                if (mybool)
                    result = 1;
                else
                    result = 0;
            }

            return result;
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false; //base.GetStandardValuesExclusive(context);
        }

        //public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        //{
        //    List<int> m_BooleanValues = new List<int>();

        //    m_BooleanValues.Add(1);
        //    m_BooleanValues.Add(0);

        //    return new StandardValuesCollection(m_BooleanValues);
        //}
    }
}
