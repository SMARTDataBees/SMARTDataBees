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
using System.Collections;
using System.Collections.Generic;
using SDBees.DB;
using SDBees.Plugs.TemplateTreeNode;

namespace SDBees.ViewAdmin
{
    /// <summary>
    /// Class to delegate view specific requests to the view administrator
    /// without specifically linking to it.
    /// </summary>
    public class ViewAdminDelegator : ViewAdminDelegatorBase
    {
        #region Private Data Members

        SDBeesDBConnection m_dbManager;

        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ViewAdminDelegator(SDBeesDBConnection dbManager)
        {
            m_dbManager = dbManager;

            // Register this instance now...
            TemplateTreenode.ViewAdminDelegator = this;
        }

        #endregion

        #region Public Methods

        public Database Database
        {
            get
            {
                return m_dbManager.Database;
            }
        }

        /// <summary>
        /// Get all the defined child types for a given parentType and view
        /// </summary>
        /// <param name="viewId">The requested view</param>
        /// <param name="parentType">The requested parent type</param>
        /// <param name="childTypes">The returned childTypes that can be beneath a parentType in this view, can be empty</param>
        /// <returns>true if successful, false if input is not valid</returns>
        public override bool GetChildTypes(Guid viewId, string parentType, out List<string> childTypes)
        {
            var success = false;
            childTypes = new List<string>();

            ArrayList objectIds = null;
            Error error = null;
            if (ViewDefinition.FindViewDefinitionsByParentType(Database, ref objectIds, viewId, parentType, ref error) > 0)
            {
                foreach (var objectId in objectIds)
                {
                    var viewDef = new ViewDefinition();
                    if (viewDef.Load(Database, objectId, ref error))
                    {
                        childTypes.Add(viewDef.ChildType);
                    }
                }
            }

            success = (error == null);

            Error.Display("Konnte ViewDefinition nicht abfragen", error);

            return success;
        }

        /// <summary>
        /// Get the defined parent type for a given childType and view
        /// </summary>
        /// <param name="viewId">The requested view</param>
        /// <param name="childType">The requested child type</param>
        /// <param name="parentType">The returned parentType defined in this view, can be "" for root nodes</param>
        /// <returns></returns>
        public override bool GetParentType(Guid viewId, string childType, out string parentType)
        {
            var success = false;
            parentType = "";

            ArrayList objectIds = null;
            Error error = null;
            var viewDefCount = ViewDefinition.FindViewDefinitionByChildType(Database, ref objectIds, viewId, childType, ref error);
            if (viewDefCount == 1)
            {
                var viewDef = new ViewDefinition();
                if (viewDef.Load(Database, objectIds[0], ref error))
                {
                    parentType = viewDef.ParentType;
                }
                else
                {
                    error = new Error("Konnte ViewDefinition nicht laden", 9999, GetType(), error);
                }
            }

            success = (error == null);

            Error.Display("Konnte ViewDefinition nicht abfragen", error);

            return success;
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Events

        #endregion
    }
}
