// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2014 by
//        G.E.M. Team Solutions GbR
//        CAD-Development
//
// SMARTDataBees is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SMARTDataBees is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with SMARTDataBees.  If not, see <http://www.gnu.org/licenses/>.
//
// #EndHeader# ================================================================

using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    /// <summary>
    /// Language dependend naming
    /// </summary>
    [DataContract]
    public class SDBeesLabel
    {
        public SDBeesLabel()
        {
            m_text = "";
        }

        public SDBeesLabel(string text)
        {
            m_text = text;
        }

        public static implicit operator string(SDBeesLabel label)
        {
            return label.m_text;
        }

        public static implicit operator SDBeesLabel(string text)
        {
            var label = new SDBeesLabel(text);
            return label;
        }

        private string m_text;
        [DataMember]
        public string Text
        {
            get { return m_text; }
            set { m_text = value; }
        }
    }
}
