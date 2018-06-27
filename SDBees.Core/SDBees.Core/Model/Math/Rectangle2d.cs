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

using System.Text;

namespace SDBees.Core.Model.Math
{
    /// <summary>
    /// Rectangle in real units
    /// </summary>
    public sealed class Rectangle2d
    {
        private double x;
        private double y;
        private double w;
        private double h;
        private Unit unit;

        /// <summary>
        /// Create new vector from two distance values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="width">Width value</param>
        /// <param name="height">Height value</param>
        public Rectangle2d(DistanceD x, DistanceD y, DistanceD width, DistanceD height)
        {
            this.x = x.NativeValue;
            this.y = y.GetIn(x.NativeUnit);
            w = width.GetIn(x.NativeUnit);
            h = height.GetIn(x.NativeUnit);
            unit = x.NativeUnit;
        }

        /// <summary>
        /// Create new vector from two distance values and specified unit type
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="width">Width value</param>
        /// <param name="height">Height value</param>
        /// <param name="unit">Unit type value</param>
        public Rectangle2d(double x, double y, double width, double height, Unit unit)
        {
            this.unit = unit;
            this.x = x;
            this.y = y;
            w = width;
            h = height;
        }

        /// <summary>
        /// Create new vector from two distance values and specified unit types
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="width">Width value</param>
        /// <param name="height">Height value</param>
        /// <param name="unitX">Unit type for X</param>
        /// <param name="unitY">Unit type for Y</param>
        /// <param name="unitW">Unit type for width</param>
        /// <param name="unitH">Unit type for height</param>
        public Rectangle2d(double x, double y, double width, double height, Unit unitX, Unit unitY, Unit unitW, Unit unitH)
        {
            unit = unitX;
            this.x = x;
            this.y = unitY.Convert(y, unitX);
            w = unitW.Convert(width, unitX);
            h = unitH.Convert(height, unitX);
        }

        /// <summary>
        /// Returns top y coordinate of rectangle
        /// </summary>
        public DistanceD Top
        {
            get
            {
                return new DistanceD(y, unit);
            }
        }

        /// <summary>
        /// Returns left x coordinate of rectangle
        /// </summary>
        public DistanceD Left
        {
            get
            {
                return new DistanceD(x, unit);
            }
        }

        /// <summary>
        /// Returns right x coordinate of rectangle
        /// </summary>
        public DistanceD Right
        {
            get
            {
                return new DistanceD(x + w, unit);
            }
        }

        /// <summary>
        /// Returns bottom y coordinate of rectangle
        /// </summary>
        public DistanceD Bottom
        {
            get
            {
                return new DistanceD(y + h, unit);
            }
        }

        /// <summary>
        /// Top left corner position
        /// </summary>
        public Vector2d TopLeft
        {
            get
            {
                return new Vector2d(x, y, unit);
            }
        }

        /// <summary>
        /// Top right corner position
        /// </summary>
        public Vector2d TopRight
        {
            get
            {
                return new Vector2d(x + w, y, unit);
            }
        }

        /// <summary>
        /// Bottom right corner position
        /// </summary>
        public Vector2d BottomRight
        {
            get
            {
                return new Vector2d(x + w, y + h, unit);
            }
        }

        /// <summary>
        /// Bottom left corner position
        /// </summary>
        public Vector2d BottomLeft
        {
            get
            {
                return new Vector2d(x, y + h, unit);
            }
        }

        /// <summary>
        /// Gets or sets X distance
        /// </summary>
        public DistanceD X
        {
            get
            {
                return new DistanceD(x, unit);
            }
            set
            {
                x = value.GetIn(unit);
            }
        }

        /// <summary>
        /// Gets or sets Y distance
        /// </summary>
        public DistanceD Y
        {
            get
            {
                return new DistanceD(y, unit);
            }
            set
            {
                y = value.GetIn(unit);
            }
        }

        /// <summary>
        /// Gets or sets Width
        /// </summary>
        public DistanceD Width
        {
            get
            {
                return new DistanceD(w, unit);
            }
            set
            {
                w = value.GetIn(unit);
            }
        }

        /// <summary>
        /// Gets or sets Height
        /// </summary>
        public DistanceD Height
        {
            get
            {
                return new DistanceD(h, unit);
            }
            set
            {
                h = value.GetIn(unit);
            }
        }

        /// <summary>
        /// Get units used for this vector
        /// </summary>
        public Unit NativeUnit
        {
            get
            {
                return unit;
            }
        }

        /// <summary>
        /// Get stored X value
        /// </summary>
        public double NativeX
        {
            get
            {
                return x;
            }
        }

        /// <summary>
        /// Get stored Y value
        /// </summary>
        public double NativeY
        {
            get
            {
                return y;
            }
        }

        /// <summary>
        /// Get stored width value
        /// </summary>
        public double NativeWidth
        {
            get
            {
                return w;
            }
        }

        /// <summary>
        /// Get stored height value
        /// </summary>
        public double NativeHeight
        {
            get
            {
                return h;
            }
        }

        /// <summary>
        /// Returns new rectangle object in another units
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public Rectangle2d Converted(Unit unit)
        {
            return new Rectangle2d(this.unit.Convert(x, unit), this.unit.Convert(y, unit), this.unit.Convert(w, unit), this.unit.Convert(h, unit), unit);
        }

        /// <summary>
        /// Returns string representation of this rectangle object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(X);
            sb.Append(' ');
            sb.Append(Y);
            sb.Append(' ');
            sb.Append(Width);
            sb.Append(' ');
            sb.Append(Height);
            return sb.ToString();
        }

        /// <summary>
        /// Return cropped rectangle by specified amounts from all sides
        /// </summary>
        /// <param name="left">How much to crop from left</param>
        /// <param name="right">How much to crop from right</param>
        /// <param name="top">How much to crop from top</param>
        /// <param name="bottom">How much to crop from bottom</param>
        /// <returns>New cropped rectangle</returns>
        public Rectangle2d Cropped(DistanceD left, DistanceD right, DistanceD top, DistanceD bottom)
        {
            var l = left.GetIn(unit);
            var t = top.GetIn(unit);
            return new Rectangle2d(x + l, y + t, w - l - right.GetIn(unit), h - t - bottom.GetIn(unit), unit);
        }
    }
}