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

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SDBees.Core.Model
{
    /// <summary>
    /// Boolean AND oder OR Verknüpfung aller Child Filter 
    /// </summary>
    [DataContract]
    public class SDBeesBooleanFilter : SDBeesEntityFilter
    {
        private bool m_inverted;
        /// <summary>
        /// invertiere die gefilterte Menge
        /// </summary>
        public bool inverted
        {
            get { return m_inverted; }
            set { m_inverted = value; }
        }

        public SDBeesFilterOperator m_filterOperator = SDBeesFilterOperator.OR;
        /// <summary>
        /// kombiniere die Child Filter 
        /// </summary>
        public SDBeesFilterOperator filterOperator
        {
            get { return m_filterOperator; }
            set { m_filterOperator = value; }
        }

        private HashSet<SDBeesFilter> m_children = new HashSet<SDBeesFilter>();
        /// <summary>
        /// Die Child Filter
        /// </summary>
        public HashSet<SDBeesFilter> children
        {
            get { return m_children; }
            set { m_children = value; }
        }
    }
}

