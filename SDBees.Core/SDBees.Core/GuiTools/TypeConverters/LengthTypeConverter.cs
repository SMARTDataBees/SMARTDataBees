using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using SDBees.Core.Global;
using SDBees.Core.Model.Math;

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
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        public static double ConvertUILengthUnitToDBLengthUnit(double doubleValue)
        {
            double result = 0;
            if (SDBeesGlobalVars.GetLengthUnits() == LengthUnits.Engineering)
            {
                var un = new UnitFoot();
                result = un.ToMillimeters(doubleValue);
            }
            else if (SDBeesGlobalVars.GetLengthUnits() == LengthUnits.Meters)
            {
                var un = new UnitMeter();
                result = un.ToMillimeters(doubleValue);
            }
            else if (SDBeesGlobalVars.GetLengthUnits() == LengthUnits.Millimeters)
            {
                var un = new UnitMillimeter();
                result = un.ToMillimeters(doubleValue);
            }
            else if (SDBeesGlobalVars.GetLengthUnits() == LengthUnits.Inches)
            {
                var un = new UnitInch();
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
                var decimalplaces = SDBeesGlobalVars.GetDecimalPlaces();

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
            var un = new UnitMillimeter();
            if (SDBeesGlobalVars.GetLengthUnits() == LengthUnits.Engineering)
                result = un.ToFeet(length).ToString(SDBeesGlobalVars.GetFormattingForDecimalPlaces(), culture);
            else if (SDBeesGlobalVars.GetLengthUnits() == LengthUnits.Meters)
                result = un.ToMeters(length).ToString(SDBeesGlobalVars.GetFormattingForDecimalPlaces(), culture);
            else if (SDBeesGlobalVars.GetLengthUnits() == LengthUnits.Millimeters)
                result = un.ToMillimeters(length).ToString(SDBeesGlobalVars.GetFormattingForDecimalPlaces(), culture);
            else if (SDBeesGlobalVars.GetLengthUnits() == LengthUnits.Inches)
                result = un.ToInches(length).ToString(SDBeesGlobalVars.GetFormattingForDecimalPlaces(), culture);
            return result;
        }

        #endregion
    }
}
