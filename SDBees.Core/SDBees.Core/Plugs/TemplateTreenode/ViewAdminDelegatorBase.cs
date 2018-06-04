// #StartHeader# ==============================================================
//
// This file is a part of the SMARTDataBees open source project.
// 
// Copyright (C) 2007 by
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
using System;
using System.Collections.Generic;
using System.Text;

namespace SDBees.Plugs.TemplateTreeNode
{
    /// <summary>
    /// Base class to delegate view specific requests to the view administrator
    /// without specifically linking to it.
    /// </summary>
    public abstract class ViewAdminDelegatorBase
    {
        #region Private Data Members

        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ViewAdminDelegatorBase()
        {
            // Nothing to do at this level
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get all the defined child types for a given parentType and view
        /// </summary>
        /// <param name="viewId">The requested view</param>
        /// <param name="parentType">The requested parent type</param>
        /// <param name="childTypes">The returned childTypes that can be beneath a parentType in this view, can be empty</param>
        /// <returns>true if successful, false if input is not valid</returns>
        public abstract bool GetChildTypes(Guid viewId, string parentType, out List<string> childTypes);

        /// <summary>
        /// Get the defined parent type for a given childType and view
        /// </summary>
        /// <param name="viewId">The requested view</param>
        /// <param name="childType">The requested child type</param>
        /// <param name="parentType">The returned parentType defined in this view, can be "" for root nodes</param>
        /// <returns></returns>
        public abstract bool GetParentType(Guid viewId, string childType, out string parentType);

        #endregion

        #region Protected Methods

        #endregion

        #region Events

        #endregion
    }
}
