using System;
using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    [DataContract]
    public class SDBeesOpeningSize
    {
        public SDBeesOpeningSize()
        {
            m_isRound = false;

            m_isRectangle = false;

            m_diameter = 0.0;

            m_width = 0.0;
            
            m_height = 0.0;
        }

        public SDBeesOpeningSize(double diameter)
        {
            m_isRound = true;

            m_isRectangle = false;

            m_diameter = diameter;

            m_width = 0.0;
            
            m_height = 0.0;
        }

        public SDBeesOpeningSize(double width, double height)
        {
            m_isRound = false;

            m_isRectangle = true;

            m_diameter = 0.0;
 
            m_width = width;
            
            m_height = height;
        }

        public SDBeesOpeningSize(string encodedString)
        {
            GetValues(encodedString);
        }

        public bool IsValid
        {
            get { return IsRound || IsRectangle; }
        }

        public string getEncodedString()
        {
            if (IsRound)
            {
                return m_diameter.ToString();
            }
            if (IsRectangle)
            {
                return String.Format("{0}x{1}", m_width, m_height);
            }
            return "";
        }

        public override string ToString()
        {
            return getEncodedString();
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return base.Equals(obj);

            var a = this;

            if (a == null) return base.Equals(obj);

            var b = obj as SDBeesOpeningSize;

            if (b == null) return base.Equals(obj);

            var result = false;

            if (a.IsRound && b.IsRound)
            {
                result = (a.Diameter == b.Diameter);
            }
            else if (a.IsRound && b.IsRectangle)
            {
                result = (a.Diameter == b.Width) && (a.Diameter == b.Height);
            }
            else if (a.IsRectangle && b.IsRound)
            {
                result = (a.Width == b.Diameter) && (a.Height == b.Diameter);
            }
            else if (IsRectangle && b.IsRectangle)
            {
                result = (a.Width == b.Width) && (a.Height == b.Height);
            }

            return result;
        }

        public override int GetHashCode()
        {
            return getEncodedString().GetHashCode();
        }

        public static bool operator <=(SDBeesOpeningSize a, SDBeesOpeningSize b)
        {
            var result = false;

            if (a.IsRound && b.IsRound)
            {
                result = (a.Diameter <= b.Diameter);
            }
            else if (a.IsRound && b.IsRectangle)
            {
                result = (a.Diameter <= b.Width) && (a.Diameter <= b.Height);
            }
            else if (a.IsRectangle && b.IsRound)
            {
                result = (a.Width <= b.Diameter) && (a.Height <= b.Diameter);
            }
            else if (a.IsRectangle && b.IsRectangle)
            {
                result = (a.Width <= b.Width) && (a.Height <= b.Height);
            }

            return result;
        }

        public static bool operator >=(SDBeesOpeningSize a, SDBeesOpeningSize b)
        {
            var result = false;

            if (a.IsRound && b.IsRound)
            {
                result = (a.Diameter >= b.Diameter);
            }
            else if (a.IsRound && b.IsRectangle)
            {
                result = (a.Diameter >= b.Width) && (a.Diameter >= b.Height);
            }
            else if (a.IsRectangle && b.IsRound)
            {
                result = (a.Width >= b.Diameter) && (a.Height >= b.Diameter);
            }
            else if (a.IsRectangle && b.IsRectangle)
            {
                result = (a.Width >= b.Width) && (a.Height >= b.Height);
            }

            return result;
        }

        public int CompareTo(SDBeesOpeningSize other)
        {
            var result = 0;

            var a = this;

            var b = other;

            if (a.IsValid && b.IsValid)
            {
                if (a.IsRound && b.IsRound)
                {
                    result = (a.Diameter.CompareTo(b.Diameter));
                }
                else if (a.IsRound && b.IsRectangle)
                {
                    result = -1;
                }
                else if (a.IsRectangle && b.IsRound)
                {
                    result = +1;
                }
                else if (a.IsRectangle && b.IsRectangle)
                {
                    result = a.Width.CompareTo(b.Width);

                    if (result == 0) result = a.Height.CompareTo(b.Height);
                }
            }

            return result;
        }

        private bool m_isRound;
        [DataMember]
        public bool IsRound
        {
            get { return m_isRound; }
            set { m_isRound = value; }
        }

        private bool m_isRectangle;
        [DataMember]
        public bool IsRectangle
        {
            get { return m_isRectangle; }
            set { m_isRectangle = value; }
        }

        private double m_diameter;
        /// <summary>
        /// Der Durchmesser
        /// </summary>
        [DataMember]
        public double Diameter
		{
            get { return m_diameter; }
            set { m_diameter = value; }
		}

        private double m_width;
        /// <summary>
        /// Die Breite
        /// </summary>
        [DataMember]
        public double Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        private double m_height;
        /// <summary>
        /// Die Höhe
        /// </summary>
        [DataMember]
        public double Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        private bool GetValues(string encodedString)
        {
            var result = false;

            m_isRound = false;

            m_isRectangle = false;

            m_diameter = 0.0;

            m_width = 0.0;

            m_height = 0.0;

            if (!String.IsNullOrEmpty(encodedString))
            {
                var dimensions = encodedString.Split('x');

                if (dimensions.Length == 1)
                {
                    if (Double.TryParse(dimensions[0], out m_diameter))
                    {
                        m_isRound = true;

                        result = true;
                    }
                }
                else if (dimensions.Length == 2)
                {
                    if (Double.TryParse(dimensions[0], out m_width) && Double.TryParse(dimensions[1], out m_height))
                    {
                        m_isRectangle = true;

                        result = true;
                    }
                }
            }

            return result;
        }
    }
}
