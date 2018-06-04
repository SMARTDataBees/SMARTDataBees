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
    public sealed class Rectangle2f
    {
        private float x;
        private float y;
        private float w;
        private float h;
        private Unit unit;

        /// <summary>
        /// Create new vector from two distance values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="width">Width value</param>
        /// <param name="height">Height value</param>
        public Rectangle2f(DistanceF x, DistanceF y, DistanceF width, DistanceF height)
        {
            this.x = x.NativeValue;
            this.y = y.GetIn(x.NativeUnit);
            this.w = width.GetIn(x.NativeUnit);
            this.h = height.GetIn(x.NativeUnit);
            this.unit = x.NativeUnit;
        }

        /// <summary>
        /// Create new vector from two distance values and specified unit type
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="width">Width value</param>
        /// <param name="height">Height value</param>
        /// <param name="unit">Unit type value</param>
        public Rectangle2f(float x, float y, float width, float height, Unit unit)
        {
            this.unit = unit;
            this.x = x;
            this.y = y;
            this.w = width;
            this.h = height;
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
        public Rectangle2f(float x, float y, float width, float height, Unit unitX, Unit unitY, Unit unitW, Unit unitH)
        {
            this.unit = unitX;
            this.x = x;
            this.y = unitY.Convert(y, unitX);
            this.w = unitW.Convert(width, unitX);
            this.h = unitH.Convert(height, unitX);
        }

        /// <summary>
        /// Returns top y coordinate of rectangle
        /// </summary>
        public DistanceF Top
        {
            get
            {
                return new DistanceF(y, unit);
            }
        }

        /// <summary>
        /// Returns left x coordinate of rectangle
        /// </summary>
        public DistanceF Left
        {
            get
            {
                return new DistanceF(x, unit);
            }
        }

        /// <summary>
        /// Returns right x coordinate of rectangle
        /// </summary>
        public DistanceF Right
        {
            get
            {
                return new DistanceF(x + w, unit);
            }
        }

        /// <summary>
        /// Returns bottom y coordinate of rectangle
        /// </summary>
        public DistanceF Bottom
        {
            get
            {
                return new DistanceF(y + h, unit);
            }
        }

        /// <summary>
        /// Top left corner position
        /// </summary>
        public Vector2f TopLeft
        {
            get
            {
                return new Vector2f(x, y, unit);
            }
        }

        /// <summary>
        /// Top right corner position
        /// </summary>
        public Vector2f TopRight
        {
            get
            {
                return new Vector2f(x + w, y, unit);
            }
        }

        /// <summary>
        /// Bottom right corner position
        /// </summary>
        public Vector2f BottomRight
        {
            get
            {
                return new Vector2f(x + w, y + h, unit);
            }
        }

        /// <summary>
        /// Bottom left corner position
        /// </summary>
        public Vector2f BottomLeft
        {
            get
            {
                return new Vector2f(x, y + h, unit);
            }
        }

        /// <summary>
        /// Gets or sets X distance
        /// </summary>
        public DistanceF X
        {
            get
            {
                return new DistanceF(x, unit);
            }
            set
            {
                this.x = value.GetIn(unit);
            }
        }

        /// <summary>
        /// Gets or sets Y distance
        /// </summary>
        public DistanceF Y
        {
            get
            {
                return new DistanceF(y, unit);
            }
            set
            {
                this.y = value.GetIn(unit);
            }
        }

        /// <summary>
        /// Gets or sets Width
        /// </summary>
        public DistanceF Width
        {
            get
            {
                return new DistanceF(w, unit);
            }
            set
            {
                this.w = value.GetIn(unit);
            }
        }

        /// <summary>
        /// Gets or sets Height
        /// </summary>
        public DistanceF Height
        {
            get
            {
                return new DistanceF(h, unit);
            }
            set
            {
                this.h = value.GetIn(unit);
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
        public float NativeX
        {
            get
            {
                return x;
            }
        }

        /// <summary>
        /// Get stored Y value
        /// </summary>
        public float NativeY
        {
            get
            {
                return y;
            }
        }

        /// <summary>
        /// Get stored width value
        /// </summary>
        public float NativeWidth
        {
            get
            {
                return w;
            }
        }

        /// <summary>
        /// Get stored height value
        /// </summary>
        public float NativeHeight
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
        public Rectangle2f Converted(Unit unit)
        {
            return new Rectangle2f(this.unit.Convert(this.x, unit), this.unit.Convert(this.y, unit), this.unit.Convert(this.w, unit), this.unit.Convert(this.h, unit), unit);
        }

        /// <summary>
        /// Returns string representation of this rectangle object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(X.ToString());
            sb.Append(' ');
            sb.Append(Y.ToString());
            sb.Append(' ');
            sb.Append(Width.ToString());
            sb.Append(' ');
            sb.Append(Height.ToString());
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
        public Rectangle2f Cropped(DistanceF left, DistanceF right, DistanceF top, DistanceF bottom)
        {
            float l = left.GetIn(unit);
            float t = top.GetIn(unit);
            return new Rectangle2f(x + l, y + t, w - l - right.GetIn(unit), h - t - bottom.GetIn(unit), unit);
        }
    }
}
