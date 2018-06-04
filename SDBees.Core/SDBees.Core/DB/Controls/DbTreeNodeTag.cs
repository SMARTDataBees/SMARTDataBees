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

namespace SDBees.DB
{
    /// <summary>
    /// Baseclass for Tags that should be set to a TreeNode in a DbTreeView or derived
    /// class if all functionality should be used (Drag&Drop etc. all refer to this)
    /// </summary>
    public class DbTreeNodeTag
    {
        #region Private Data Members

        private object mNodeId;

        #endregion

        #region Public Properties

        /// <summary>
        /// This is the id of the DB object representing the node in the database
        /// </summary>
        public object NodeId
        {
            get { return mNodeId; }
            set { mNodeId = value; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="nodeId"></param>
        public DbTreeNodeTag(object nodeId)
        {
            mNodeId = nodeId;
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        #endregion
    }
}
