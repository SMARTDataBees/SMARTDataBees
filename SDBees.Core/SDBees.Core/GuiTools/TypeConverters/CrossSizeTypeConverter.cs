using System;
using System.ComponentModel;
using System.Globalization;
using SDBees.Core.Model;

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

            var valueString = value.ToString();

            if (valueString != "")
            {
                var dimensions = valueString.Split('x');

                if ((dimensions.Length == 1) && IsRound(context))
                {
                    var converter = new LengthTypeConverter();

                    var doubleValue0 = 0.0;

                    if (Double.TryParse(dimensions[0], out doubleValue0))
                    {
                        if (destinationType == null)
                        {
                            var openingSize = new SDBeesOpeningSize(converter.ConvertFrom(context, culture, doubleValue0).ToString());

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
                    var converter = new LengthTypeConverter();

                    var doubleValue0 = 0.0;

                    var doubleValue1 = 0.0;

                    if (Double.TryParse(dimensions[0], out doubleValue0) && Double.TryParse(dimensions[1], out doubleValue1))
                    {
                        if (destinationType == null)
                        {
                            var result0 = converter.ConvertFrom(context, culture, doubleValue0);

                            var result1 = converter.ConvertFrom(context, culture, doubleValue1);

                            var openingSize = new SDBeesOpeningSize(result0 + "x" + result1);

                            if (Validate(context, openingSize))
                            {
                                result = openingSize;
                            }
                        }
                        else
                        {
                            var result0 = converter.ConvertTo(context, culture, doubleValue0, destinationType);

                            var result1 = converter.ConvertTo(context, culture, doubleValue1, destinationType);

                            result = result0 + "x" + result1;
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
                    exception = GetConvertFromException(value);
                }
                else
                {
                    exception = GetConvertToException(value, destinationType);
                }

                throw exception;
            }

            return result;
        }

        protected abstract bool IsRound(ITypeDescriptorContext context);
 
        protected abstract bool IsRectangular(ITypeDescriptorContext context);

        protected virtual bool Validate(ITypeDescriptorContext context, SDBeesOpeningSize openingSize)
        {
            return true;
        }

        #endregion
    }
}
