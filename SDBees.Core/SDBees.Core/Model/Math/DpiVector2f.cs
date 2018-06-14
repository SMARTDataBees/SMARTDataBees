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
    /// Vector for device resolution
    /// </summary>
    public struct DpiVector2f
    {
        /// <summary>
        /// Create new device resolution vector
        /// </summary>
        /// <param name="dpiX">Dots per inch in X direction</param>
        /// <param name="dpiY">Dots per inch in Y direction</param>
        public DpiVector2f(float dpiX, float dpiY)
        {
            DpiX = dpiX;
            DpiY = dpiY;
        }

        /// <summary>
        /// Dots per inch in X direction
        /// </summary>
        public float DpiX;
        /// <summary>
        /// Dots per inch in Y direction
        /// </summary>
        public float DpiY;
    }
}
