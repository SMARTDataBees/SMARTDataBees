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
using System.Collections.Generic;

namespace SDBees.Core.Model.Math
{
    /// <summary>
    /// Collection of unit handling types
    /// </summary>
    public abstract class Unit
    {
        private static UnitMillimeter millimeters = new UnitMillimeter();
        /// <summary>
        /// Units in millimeters
        /// </summary>
        public static UnitMillimeter Millimeters { get { return millimeters; } }

        private static UnitCentimeter centimeters = new UnitCentimeter();
        /// <summary>
        /// Units in centimeters
        /// </summary>
        public static UnitCentimeter Centimeters { get { return centimeters; } }

        private static UnitDecimeter decimeters = new UnitDecimeter();
        /// <summary>
        /// Units in decimeters
        /// </summary>
        public static UnitDecimeter Decimeters { get { return decimeters; } }

        private static UnitMeter meters = new UnitMeter();
        /// <summary>
        /// Units in meters
        /// </summary>
        public static UnitMeter Meters { get { return meters; } }

        private static UnitInch inches = new UnitInch();
        /// <summary>
        /// Units in inches
        /// </summary>
        public static UnitInch Inches { get { return inches; } }

        private static UnitFoot feet = new UnitFoot();
        /// <summary>
        /// Units in feet
        /// </summary>
        public static UnitFoot Feet { get { return feet; } }

        /// <summary>
        /// Static list of all defined unit types
        /// </summary>
        public static List<Unit> AllUnitTypes
        {
            get 
            { 
                var _lst = new List<Unit>();
                _lst.Add(Centimeters);
                _lst.Add(Decimeters);
                _lst.Add(Feet);
                _lst.Add(Inches);
                _lst.Add(Meters);
                _lst.Add(Millimeters);
                _lst.Add(MillimeterSquare);
                return _lst; 
            }
        }

        private static UnitMilimeterSquare millimetersquare = new UnitMilimeterSquare();
        /// <summary>
        /// Units in millimeter square
        /// </summary>
        public static UnitMilimeterSquare MillimeterSquare
        {
            get { return millimetersquare; }
        }

        private static int longestUnitName = -1;
        /// <summary>
        /// Returns longest type name
        /// </summary>
        public static int LongestUnitName
        {
            get
            {
                if (longestUnitName == -1)
                {
                    foreach (var unit in AllUnitTypes)
                    {
                        if (unit.SymbolName.Length > longestUnitName)
                            longestUnitName = unit.SymbolName.Length;
                    }
                }
                return longestUnitName;
            }
        }

        private static int shortestUnitName = -1;
        /// <summary>
        /// Returns shortest type name
        /// </summary>
        public static int ShortestUnitName
        {
            get
            {
                if (shortestUnitName == -1)
                {
                    shortestUnitName = 10;
                    foreach (var unit in AllUnitTypes)
                    {
                        if (unit.SymbolName.Length < shortestUnitName)
                            shortestUnitName = unit.SymbolName.Length;
                    }
                }
                return shortestUnitName;
            }
        }

        /// <summary>
        /// Unit type identifiers
        /// </summary>
        public enum UnitType
        {
            /// <summary>
            /// mm
            /// </summary>
            Millimeter,
            /// <summary>
            /// cm
            /// </summary>
            Centimeter,
            /// <summary>
            /// dm
            /// </summary>
            Decimeter,
            /// <summary>
            /// m
            /// </summary>
            Meter,
            /// <summary>
            /// in
            /// </summary>
            Inch,
            /// <summary>
            /// ft
            /// </summary>
            Foot,
            /// <summary>
            /// mm²
            /// </summary>
            MillimeterSquare
        }

        /// <summary>
        /// Type identifier of this unit
        /// </summary>
        protected abstract UnitType Type { get; }

        /// <summary>
        /// Perform conversion from current type to millimeters on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract double ToMillimeters(double value);
        /// <summary>
        /// Perform conversion from current type to centimeters on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract double ToCentimeters(double value);
        /// <summary>
        /// Perform conversion from current type to decimeters on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract double ToDecimeters(double value);
        /// <summary>
        /// Perform conversion from current type to meters on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract double ToMeters(double value);
        /// <summary>
        /// Perform conversion from current type to inches on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract double ToInches(double value);
        /// <summary>
        /// Perform conversion from current type to feet on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract double ToFeet(double value);

        /// <summary>
        /// Perform conversion from current type to millimeters on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract float ToMillimeters(float value);
        /// <summary>
        /// Perform conversion from current type to centimeters on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract float ToCentimeters(float value);
        /// <summary>
        /// Perform conversion from current type to decimeters on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract float ToDecimeters(float value);
        /// <summary>
        /// Perform conversion from current type to meters on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract float ToMeters(float value);
        /// <summary>
        /// Perform conversion from current type to inches on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract float ToInches(float value);
        /// <summary>
        /// Perform conversion from current type to feet on specified value
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public abstract float ToFeet(float value);

        /// <summary>
        /// Convert value to another type
        /// </summary>
        /// <param name="value">Value in this type</param>
        /// <param name="unit">Units to convert to</param>
        /// <returns>Value in new units</returns>
        public float Convert(float value, Unit unit)
        {
            switch (unit.Type)
            {
                case UnitType.Millimeter:
                    return ToMillimeters(value);
                case UnitType.Inch:
                    return ToInches(value);
                case UnitType.Centimeter:
                    return ToCentimeters(value);
                case UnitType.Decimeter:
                    return ToDecimeters(value);
                case UnitType.Meter:
                    return ToMeters(value);
                case UnitType.Foot:
                    return ToFeet(value);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Convert value to another type
        /// </summary>
        /// <param name="value">Value in this type</param>
        /// <param name="unit">Units to convert to</param>
        /// <returns>Value in new units</returns>
        public double Convert(double value, Unit unit)
        {
            switch (unit.Type)
            {
                case UnitType.Millimeter:
                    return ToMillimeters(value);
                case UnitType.Inch:
                    return ToInches(value);
                case UnitType.Centimeter:
                    return ToCentimeters(value);
                case UnitType.Decimeter:
                    return ToDecimeters(value);
                case UnitType.Meter:
                    return ToMeters(value);
                case UnitType.Foot:
                    return ToFeet(value);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get short name of this type
        /// </summary>
        public abstract string SymbolName { get; }

        /// <summary>
        /// Get string representation of this type
        /// </summary>
        /// <returns>Type representation as string</returns>
        public override string ToString()
        {
            foreach (var type in AllUnitTypes)
            {
                if (Type == type.Type)
                    return type.SymbolName;
            }
            return "";
        }
    }
}
