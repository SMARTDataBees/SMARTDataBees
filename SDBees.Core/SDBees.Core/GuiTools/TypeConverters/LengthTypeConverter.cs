using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SDBees.Core.GuiTools.TypeConverters
{
    /// <summary>
    /// Converts lengths from one data type to another. Access this class through the <see cref="TypeDescriptor"/>.
    /// </summary>
    public class LengthTypeConverter : TypeConverter
    {
        #region Overridden Members

        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type" /> that represents the type you want to convert from.</param>
        /// <returns>true if this converter can perform the conversion; otherwise, false.</returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// Called, when a property value is changed in UI.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            object result = null;

            try
            {
                double doubleValue;
                if (Double.TryParse(value.ToString(), NumberStyles.Any, culture.NumberFormat, out doubleValue))
                //if (Double.TryParse(value.ToString(), out doubleValue))
                {
                    result = ConvertUILengthUnitToDBLengthUnit(doubleValue);
                }
                result = result ?? base.ConvertFrom(context, culture, value);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }

            return result;
        }

        public static double ConvertUILengthUnitToDBLengthUnit(double doubleValue)
        {
            double result = 0;
            if (SDBees.Core.Global.SDBeesGlobalVars.GetLengthUnits() == Model.Math.LengthUnits.Engineering)
            {
                SDBees.Core.Model.Math.UnitFoot un = new Model.Math.UnitFoot();
                result = un.ToMillimeters(doubleValue);
            }
            else if (SDBees.Core.Global.SDBeesGlobalVars.GetLengthUnits() == Model.Math.LengthUnits.Meters)
            {
                SDBees.Core.Model.Math.UnitMeter un = new Model.Math.UnitMeter();
                result = un.ToMillimeters(doubleValue);
            }
            else if (SDBees.Core.Global.SDBeesGlobalVars.GetLengthUnits() == Model.Math.LengthUnits.Millimeters)
            {
                SDBees.Core.Model.Math.UnitMillimeter un = new Model.Math.UnitMillimeter();
                result = un.ToMillimeters(doubleValue);
            }
            else if (SDBees.Core.Global.SDBeesGlobalVars.GetLengthUnits() == Model.Math.LengthUnits.Inches)
            {
                SDBees.Core.Model.Math.UnitInch un = new Model.Math.UnitInch();
                result = un.ToMillimeters(doubleValue);
            }
            return result;
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// Called when a property value will be rendered for UI presentation
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <exception cref="System.ArgumentNullException">destinationType</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            double length;
            object result = null;

            if (Double.TryParse(value.ToString(), out length))
            {
                Int16 decimalplaces = SDBees.Core.Global.SDBeesGlobalVars.GetDecimalPlaces();

                if (length != null && destinationType == typeof(string))
                {
                    result = ConvertDBLengthUnitsToUILengthUnits(culture, length);
                }
                result = result ?? base.ConvertTo(context, culture, value, destinationType);
            }
            return result;
        }

        public static object ConvertDBLengthUnitsToUILengthUnits(CultureInfo culture, double length)
        {
            object result = 0;
            SDBees.Core.Model.Math.UnitMillimeter un = new Model.Math.UnitMillimeter();
            if (SDBees.Core.Global.SDBeesGlobalVars.GetLengthUnits() == Model.Math.LengthUnits.Engineering)
                result = un.ToFeet(length).ToString(SDBees.Core.Global.SDBeesGlobalVars.GetFormattingForDecimalPlaces(), culture);
            else if (SDBees.Core.Global.SDBeesGlobalVars.GetLengthUnits() == Model.Math.LengthUnits.Meters)
                result = un.ToMeters(length).ToString(SDBees.Core.Global.SDBeesGlobalVars.GetFormattingForDecimalPlaces(), culture);
            else if (SDBees.Core.Global.SDBeesGlobalVars.GetLengthUnits() == Model.Math.LengthUnits.Millimeters)
                result = un.ToMillimeters(length).ToString(SDBees.Core.Global.SDBeesGlobalVars.GetFormattingForDecimalPlaces(), culture);
            else if (SDBees.Core.Global.SDBeesGlobalVars.GetLengthUnits() == Model.Math.LengthUnits.Inches)
                result = un.ToInches(length).ToString(SDBees.Core.Global.SDBeesGlobalVars.GetFormattingForDecimalPlaces(), culture);
            return result;
        }

        #endregion
    }
}
