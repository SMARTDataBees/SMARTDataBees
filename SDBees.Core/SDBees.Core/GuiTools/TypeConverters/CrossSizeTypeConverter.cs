using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SDBees.Core.GuiTools.TypeConverters
{
    /// <summary>
    /// Converts cross size (either diameter or width x height) from one data type to another. Access this class through the <see cref="TypeDescriptor"/>.
    /// </summary>
    public abstract class AbstractCrossSizeTypeConverter : TypeConverter
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
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            // string -> SDBeesOpeningSize

            return Convert(context, culture, value, null);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext" /> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo" />. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object" /> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type" /> to convert the <paramref name="value" /> parameter to.</param>
        /// <returns>An <see cref="T:System.Object" /> that represents the converted value.</returns>
        /// <exception cref="System.ArgumentNullException">destinationType</exception>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            // SDBeesOpeningSize -> string

            return Convert(context, culture, value, destinationType);
        }

        private object Convert(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            object result = null;

            string valueString = value.ToString();

            if (valueString != "")
            {
                string[] dimensions = valueString.Split(new Char[] { 'x' });

                if ((dimensions.Length == 1) && IsRound(context))
                {
                    LengthTypeConverter converter = new LengthTypeConverter();

                    double doubleValue0 = 0.0;

                    if (Double.TryParse(dimensions[0], out doubleValue0))
                    {
                        if (destinationType == null)
                        {
                            SDBees.Core.Model.SDBeesOpeningSize openingSize = new SDBees.Core.Model.SDBeesOpeningSize(converter.ConvertFrom(context, culture, doubleValue0).ToString());

                            if (Validate(context, openingSize))
                            {
                                result = openingSize;
                            }
                        }
                        else
                        {
                            result = converter.ConvertTo(context, culture, doubleValue0, destinationType);
                        }
                    }
                }
                else if ((dimensions.Length == 2) && IsRectangular(context))
                {
                    LengthTypeConverter converter = new LengthTypeConverter();

                    double doubleValue0 = 0.0;

                    double doubleValue1 = 0.0;

                    if (Double.TryParse(dimensions[0], out doubleValue0) && Double.TryParse(dimensions[1], out doubleValue1))
                    {
                        if (destinationType == null)
                        {
                            object result0 = converter.ConvertFrom(context, culture, doubleValue0);

                            object result1 = converter.ConvertFrom(context, culture, doubleValue1);

                            SDBees.Core.Model.SDBeesOpeningSize openingSize = new SDBees.Core.Model.SDBeesOpeningSize(result0.ToString() + "x" + result1.ToString());

                            if (Validate(context, openingSize))
                            {
                                result = openingSize;
                            }
                        }
                        else
                        {
                            object result0 = converter.ConvertTo(context, culture, doubleValue0, destinationType);

                            object result1 = converter.ConvertTo(context, culture, doubleValue1, destinationType);

                            result = result0.ToString() + "x" + result1.ToString();
                        }
                    }
                }
            }
            else
            {
                result = "";
            }

            if (result == null)
            {
                Exception exception = null;

                if (destinationType == null)
                {
                    exception = base.GetConvertFromException(value);
                }
                else
                {
                    exception = base.GetConvertToException(value, destinationType);
                }

                throw exception;
            }

            return result;
        }

        protected abstract bool IsRound(ITypeDescriptorContext context);
 
        protected abstract bool IsRectangular(ITypeDescriptorContext context);

        protected virtual bool Validate(ITypeDescriptorContext context, SDBees.Core.Model.SDBeesOpeningSize openingSize)
        {
            return true;
        }

        #endregion
    }
}
