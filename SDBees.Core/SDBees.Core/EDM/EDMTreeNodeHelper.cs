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
using System.Windows.Forms;
using SDBees.Plugs.TemplateBase;

namespace SDBees.EDM
{
	public abstract class EDMTreeNodeHelper : TemplateBase
	{
		public abstract string PluginSectionText();
        private static Hashtable _hashPlugins;

        public EDMTreeNodeHelper()
        {
            try
            {
                if (_hashPlugins == null)
                {
                    _hashPlugins = new Hashtable();
                }
                var treenodeType = GetType().ToString();
                _hashPlugins.Add(treenodeType, this);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


		/// <summary>
		/// Returns the MenueItem for the New Item
		/// </summary>
		/// <returns>ToolStripMenuItem object</returns>
		public abstract ToolStripMenuItem NewItemsMenue();

		public abstract void ReactOnNew();

        /// <summary>
        /// Returns the MenueItem for the Execute / View Item
        /// </summary>
        /// <returns>ToolStripMenuItem object</returns>
		public abstract ToolStripMenuItem ExecuteItemsMenue();

		public abstract void ReactOnExecute();


		public abstract ToolStripMenuItem DeleteItemsMenue();

		public abstract void ReactOnDelete();

        internal static List<EDMTreeNodeHelper> GetAllPlugins()
        {
            var result = new List<EDMTreeNodeHelper>();

            foreach (DictionaryEntry plugin in _hashPlugins)
            {
                result.Add((EDMTreeNodeHelper)plugin.Value);
            }

            return result;
        }
    }
}
