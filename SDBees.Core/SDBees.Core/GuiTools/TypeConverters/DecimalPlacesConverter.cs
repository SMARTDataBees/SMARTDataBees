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
	public class DecimalPlacesTypeConverter : TypeConverter
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

		static bool inConversionMode = false;
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
			Int16 places = 2;

			if (value != null)
			{
				if (Int16.TryParse(value.ToString(), out places))
				{
					if (!inConversionMode)
					{
						if (places < 0 || places > 9)
						{
							inConversionMode = true;
							places = 2;
							System.Windows.Forms.MessageBox.Show("No values lower than 0 and greater than 9 allowed!");
							inConversionMode = false;
						}
					}
				}
			}

			return places;
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if(destinationType == typeof(Int16))
			{ }
			return true;
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
			short places = 2;

			if (value != null)
			{
				if (!Int16.TryParse(value.ToString(), out places))
					places = 2;
			}

			return places.ToString();
		}
		
		#endregion
	}
}
