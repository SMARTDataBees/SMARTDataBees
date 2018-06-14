/*
 *    Copyright 2009-2010 Nerijus Arlauskas and contributors
 * 
 *    This file is part of RealUnits.
 *
 *    RealUnits is free software: you can redistribute it and/or modify
 *    it under the terms of the GNU Lesser General Public License as 
 *    published by the Free Software Foundation, either version 3 of 
 *    the License, or (at your option) any later version.
 *
 *    RealUnits is distributed in the hope that it will be useful,
 *    but WITHOUT ANY WARRANTY; without even the implied warranty of
 *    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 *    GNU Lesser General Public License for more details.
 *
 *    You should have received A copy of the GNU Lesser General Public 
 *    License along with RealUnits. If not, see 
 *    <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Globalization;
using System.Text;

namespace SDBees.Core.Model.Math
{
    /// <summary>
    /// Distance in real units
    /// </summary>
    public class DistanceF
    {
        private const float smallVal = 0.000001f;

        /// <summary>
        /// Parse distance value from string representation
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DistanceF Parse(string s)
        {
            if (s == null)
                throw new FormatException("Can't parse null string.");
            float value;
            var ls = s.ToLowerInvariant();
            for (var nameLength = Unit.LongestUnitName; nameLength >= Unit.ShortestUnitName; nameLength--)
            {
                foreach (var unit in Unit.AllUnitTypes)
                {
                    if (unit.SymbolName.Length == nameLength)
                    {
                        if (ls.EndsWith(unit.SymbolName))
                        {
                            if (float.TryParse(s.Substring(0, s.Length - nameLength), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                                return new DistanceF(value, Unit.Millimeters);
                            throw new FormatException("Invalid " + unit.SymbolName + " unit value.");
                        }
                    }
                }
            }
            throw new FormatException("Failed to parse distance string.");
        }

        /// <summary>
        /// Create new distance from pixels and resolution
        /// </summary>
        /// <param name="pixels">Pixels on device</param>
        /// <param name="dpi">Device resolution</param>
        /// <returns>New distance object</returns>
        public static DistanceF FromPixels(int pixels, float dpi)
        {
            return new DistanceF(pixels / dpi, Unit.Inches);
        }

        /// <summary>
        /// Specifies value of distance which is equal to 0
        /// </summary>
        public static DistanceF Zero = new DistanceF(0, Unit.Millimeters);

        private Unit unit;
        private float value;

        /// <summary>
        /// Create new distance with specified unit type.
        /// </summary>
        /// <param name="value">Distance</param>
        /// <param name="unit">Unit type</param>
        public DistanceF(float value, Unit unit)
        {
            this.unit = unit;
            this.value = value;
        }

        /// <summary>
        /// Get distance as number of pixels in inch
        /// </summary>
        /// <param name="dotsPerInch">DPI value</param>
        /// <returns>Value in pixels</returns>
        public float Pixels(float dotsPerInch)
        {
            return Inches * dotsPerInch;
        }

        /// <summary>
        /// Get value in specified unit format
        /// </summary>
        /// <param name="unit">Unit format of requested value</param>
        /// <returns>Value in specified units</returns>
        public float GetIn(Unit unit)
        {
            return this.unit.Convert(value, unit);
        }

        /// <summary>
        /// Get distance in other units
        /// </summary>
        /// <param name="unit">Other unit type</param>
        /// <returns>New distance object</returns>
        public DistanceF Converted(Unit unit)
        {
            return new DistanceF(this.unit.Convert(value, unit), unit);
        }

        /// <summary>
        /// Get value in native format
        /// </summary>
        public float NativeValue { get { return value; } }

        /// <summary>
        /// Get units used for this distance
        /// </summary>
        public Unit NativeUnit { get { return unit; } }

        /// <summary>
        /// Get distance as meters
        /// </summary>
        public float Meters { get { return unit.ToMeters(value); } }
        /// <summary>
        /// Get distance as millimeters
        /// </summary>
        public float Millimeters { get { return unit.ToMillimeters(value); } }
        /// <summary>
        /// Get distance as centimeters
        /// </summary>
        public float Centimeters { get { return unit.ToCentimeters(value); } }
        /// <summary>
        /// Get distance as decimeters
        /// </summary>
        public float Decimeters { get { return unit.ToDecimeters(value); } }
        /// <summary>
        /// Get distance as inches
        /// </summary>
        public float Inches { get { return unit.ToInches(value); } }
        /// <summary>
        /// Get distance in hundreds of inches
        /// </summary>
        public float HInches { get { return unit.ToInches(value) * 100.0f; } }
        /// <summary>
        /// Get distance as number of feet
        /// </summary>
        public float Feet { get { return unit.ToFeet(value); } }

        /// <summary>
        /// Compares two distances
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>Comparision result</returns>
        public static bool operator <(DistanceF a, DistanceF b)
        {
            return a.value < b.GetIn(a.unit);
        }

        /// <summary>
        /// Compares two distances
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>Comparision result</returns>
        public static bool operator >(DistanceF a, DistanceF b)
        {
            return a.value > b.GetIn(a.unit);
        }

        /// <summary>
        /// Compares two distances
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>Comparision result</returns>
        public static bool operator <=(DistanceF a, DistanceF b)
        {
            return a.value <= b.GetIn(a.unit);
        }

        /// <summary>
        /// Compares two distances
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>Comparision result</returns>
        public static bool operator >=(DistanceF a, DistanceF b)
        {
            return a.value >= b.GetIn(a.unit);
        }

        /// <summary>
        /// Checks if distance is equal to another object
        /// </summary>
        /// <param name="obj">Another object</param>
        /// <returns>True if distance is equal to another object, false if not.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DistanceF)
            {
                return this == (DistanceF)obj;
            }
            return false;
        }

        /// <summary>
        /// Returns hash code for this distance
        /// </summary>
        /// <returns>Hach code</returns>
        public override int GetHashCode()
        {
            return unit.ToMillimeters(value).GetHashCode();
        }

        /// <summary>
        /// Checks if two distance instances are equal
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>True if distances are equal</returns>
        public static bool operator ==(DistanceF a, DistanceF b)
        {
            return a.value > b.GetIn(a.unit) - smallVal && a.value < b.GetIn(a.unit) + smallVal;
        }

        /// <summary>
        /// Checks if two distance instances are not equal
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>True if distances are not equal</returns>
        public static bool operator !=(DistanceF a, DistanceF b)
        {
            return !(a == b);
        }

        /// <summary>
        /// Performs addition of two distances
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>New distance</returns>
        public static DistanceF operator +(DistanceF a, DistanceF b)
        {
            return new DistanceF(a.value + b.GetIn(a.unit), a.unit);
        }

        /// <summary>
        /// Performs subtraction of two distances
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>New distance</returns>
        public static DistanceF operator -(DistanceF a, DistanceF b)
        {
            return new DistanceF(a.value - b.GetIn(a.unit), a.unit);
        }

        /// <summary>
        /// Performs multiplication of distance to scalar value
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="scalar">Value</param>
        /// <returns>New distance</returns>
        public static DistanceF operator *(DistanceF a, float scalar)
        {
            return new DistanceF(a.value * scalar, a.unit);
        }

        /// <summary>
        /// Performs multiplication of distance to scalar value
        /// </summary>
        /// <param name="scalar">Value</param>
        /// <param name="A">Distance A</param>
        /// <returns>New distance</returns>
        public static DistanceF operator *(float scalar, DistanceF a)
        {
            return new DistanceF(a.value * scalar, a.unit);
        }

        /// <summary>
        /// Performs multiplication of distance from scalar value
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="scalar">Value</param>
        /// <returns>New distance</returns>
        public static DistanceF operator /(DistanceF a, float scalar)
        {
            return new DistanceF(a.value / scalar, a.unit);
        }

        /// <summary>
        /// Gets ratio of two distances
        /// </summary>
        /// <param name="A">Distance A</param>
        /// <param name="b">Distance B</param>
        /// <returns>Scalar value</returns>
        public static float operator /(DistanceF a, DistanceF b)
        {
            return a.value / b.GetIn(a.unit);
        }

        /// <summary>
        /// Converts two distances into X and Y vector
        /// </summary>
        /// <param name="A">Distance A for X value</param>
        /// <param name="b">Distance B for Y value</param>
        /// <returns>Distance vector</returns>
        public static Vector2f operator ^(DistanceF a, DistanceF b)
        {
            return new Vector2f(a.NativeValue, b.GetIn(a.NativeUnit), a.NativeUnit);
        }

        /// <summary>
        /// Returns string representation of this distance
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(value.ToString(CultureInfo.InvariantCulture));
            sb.Append(unit);

            return sb.ToString();
        }
    }
}