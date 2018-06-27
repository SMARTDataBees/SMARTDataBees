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

namespace SDBees.Core.Model.Math
{
    /// <summary>
    /// Handles millimeter unit conversions
    /// </summary>
    public sealed class UnitMillimeter : Unit
    {
        /// <summary>
        /// Convert value to millimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToMillimeters(double value)
        {
            return value;
        }

        /// <summary>
        /// Convert value to centimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToCentimeters(double value)
        {
            return value * 0.1;
        }

        /// <summary>
        /// Convert value to decimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToDecimeters(double value)
        {
            return value * 0.01;
        }

        /// <summary>
        /// Convert value to meters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToMeters(double value)
        {
            return value * 0.001;
        }

        /// <summary>
        /// Convert value to inches
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToInches(double value)
        {
            return value * 0.0393700787;
        }

        /// <summary>
        /// Convert value to feet
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToFeet(double value)
        {
            return value * 0.0032808399;
        }

        /// <summary>
        /// Convert value to millimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToMillimeters(float value)
        {
            return value;
        }

        /// <summary>
        /// Convert value to centimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToCentimeters(float value)
        {
            return value * 0.1f;
        }

        /// <summary>
        /// Convert value to decimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToDecimeters(float value)
        {
            return value * 0.01f;
        }

        /// <summary>
        /// Convert value to meters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToMeters(float value)
        {
            return value * 0.001f;
        }

        /// <summary>
        /// Convert value to inches
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToInches(float value)
        {
            return value * 0.0393700787f;
        }

        /// <summary>
        /// Convert value to feet
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToFeet(float value)
        {
            return value * 0.0032808399f;
        }

        /// <summary>
        /// Get type identifier
        /// </summary>
        protected override UnitType Type
        {
            get { return UnitType.Millimeter; }
        }

        /// <summary>
        /// Get short name of this type
        /// </summary>
        public override string SymbolName
        {
            get { return "mm"; }
        }
    }

    public sealed class UnitMilimeterSquare : Unit
    {
        /// <summary>
        /// Convert value to millimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToMillimeters(double value)
        {
            return value;
        }

        /// <summary>
        /// Convert value to centimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToCentimeters(double value)
        {
            return value * 0.01;
        }

        /// <summary>
        /// Convert value to decimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToDecimeters(double value)
        {
            return value * 0.0001;
        }

        /// <summary>
        /// Convert value to meters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToMeters(double value)
        {
            return  System.Math.Pow(value, -6);
        }

        /// <summary>
        /// Convert value to inches
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToInches(double value)
        {
            return value * 0.0015500031;
        }

        /// <summary>
        /// Convert value to feet
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToFeet(double value)
        {
            return value * System.Math.Pow(1.076391041671,-5);
        }

        /// <summary>
        /// Convert value to millimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToMillimeters(float value)
        {
            return value;
        }

        /// <summary>
        /// Convert value to centimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToCentimeters(float value)
        {
            return value * 0.01f;
        }

        /// <summary>
        /// Convert value to decimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToDecimeters(float value)
        {
            return value * 0.0001f;
        }

        /// <summary>
        /// Convert value to meters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToMeters(float value)
        {
            return System.Convert.ToSingle(System.Math.Pow(value, -6));
        }

        /// <summary>
        /// Convert value to inches
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToInches(float value)
        {
            return value * 0.0015500031f;
        }

        /// <summary>
        /// Convert value to feet
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToFeet(float value)
        {
            return System.Convert.ToSingle(value * System.Math.Pow(1.076391041671, -5));
        }
        protected override UnitType Type
        {
            get { return UnitType.MillimeterSquare; }
        }
        public override string SymbolName
        {
            get { return "mm²"; }
        }
    }

    public sealed class UnitMilimeterCubique : Unit
    {
        /// <summary>
        /// Convert value to millimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToMillimeters(double value)
        {
            return value;
        }

        /// <summary>
        /// Convert value to centimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToCentimeters(double value)
        {
            return value * 0.001;
        }

        /// <summary>
        /// Convert value to decimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToDecimeters(double value)
        {
            return System.Math.Pow(value, -6);
        }

        /// <summary>
        /// Convert value to meters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToMeters(double value)
        {
            return System.Math.Pow(value, -9);
        }

        /// <summary>
        /// Convert value to inches
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToInches(double value)
        {
            double result = 0;
            result = value * 0.000061023744; //System.Math.Pow(6.1023744094732, -5);

            return result;
        }

        public double ToOunce(double value)
        {
            return value * 0.0000351951; //System.Math.Pow(3.5195079727854, -5);
        }

        public double ToOunceUS(double value)
        {
            double result = 0;

            result = value * 0.000033814; // System.Math.Pow(3.3814022701843, -5);
            return result;
        }

        public double ToGallonUS(double value)
        {
            double result = 0;

            result = value * 0.0000002642; //System.Math.Pow(2.6417205235815, -7);
            return result;
        }

        /// <summary>
        /// Convert value to feet
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToFeet(double value)
        {
            return value * 0.0000000353; //System.Math.Pow(1.076391041671, -5);
        }

        /// <summary>
        /// Convert value to millimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToMillimeters(float value)
        {
            return value;
        }

        /// <summary>
        /// Convert value to centimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToCentimeters(float value)
        {
            return value * 0.01f;
        }

        /// <summary>
        /// Convert value to decimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToDecimeters(float value)
        {
            return value * 0.0001f;
        }

        /// <summary>
        /// Convert value to meters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToMeters(float value)
        {
            return System.Convert.ToSingle(System.Math.Pow(value, -6));
        }

        /// <summary>
        /// Convert value to inches
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToInches(float value)
        {
            return value * 0.0015500031f;
        }

        /// <summary>
        /// Convert value to feet
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToFeet(float value)
        {
            return System.Convert.ToSingle(value * System.Math.Pow(1.076391041671, -5));
        }
        protected override UnitType Type
        {
            get { return UnitType.MillimeterSquare; }
        }
        public override string SymbolName
        {
            get { return "mm³"; }
        }
    }

}