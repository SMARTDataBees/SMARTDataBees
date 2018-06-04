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
    /// Handles centimeter unit conversions
    /// </summary>
    public sealed class UnitCentimeter : Unit
    {
        /// <summary>
        /// Convert value to millimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToMillimeters(double value)
        {
            return value * 10.0;
        }

        /// <summary>
        /// Convert value to centimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToCentimeters(double value)
        {
            return value;
        }

        /// <summary>
        /// Convert value to decimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToDecimeters(double value)
        {
            return value * 0.1;
        }

        /// <summary>
        /// Convert value to meters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToMeters(double value)
        {
            return value * 0.01;
        }

        /// <summary>
        /// Convert value to inches
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToInches(double value)
        {
            return value * 0.393700787;
        }

        /// <summary>
        /// Convert value to feet
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override double ToFeet(double value)
        {
            return value * 0.032808399;
        }

        /// <summary>
        /// Convert value to millimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToMillimeters(float value)
        {
            return value * 10.0f;
        }

        /// <summary>
        /// Convert value to centimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToCentimeters(float value)
        {
            return value;
        }

        /// <summary>
        /// Convert value to decimeters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToDecimeters(float value)
        {
            return value * 0.1f;
        }

        /// <summary>
        /// Convert value to meters
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToMeters(float value)
        {
            return value * 0.01f;
        }

        /// <summary>
        /// Convert value to inches
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToInches(float value)
        {
            return value * 0.393700787f;
        }

        /// <summary>
        /// Convert value to feet
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Converted value</returns>
        public override float ToFeet(float value)
        {
            return value * 0.032808399f;
        }

        /// <summary>
        /// Get type identifier
        /// </summary>
        protected override Unit.UnitType Type
        {
            get { return UnitType.Centimeter; }
        }

        /// <summary>
        /// Get short name of this type
        /// </summary>
        public override string SymbolName
        {
            get { return "cm"; }
        }
    }
}
