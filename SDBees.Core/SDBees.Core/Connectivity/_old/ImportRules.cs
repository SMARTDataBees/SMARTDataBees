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
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace SDBees.Connectivity
{
    /// <summary>
    /// Class with collection of import rules
    /// </summary>
    public class ImportRules
    {
        #region Private Data Members

        private List<ImportRule> mItems;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get the items in this rule set
        /// </summary>
        public List<ImportRule> Items
        {
            get { return mItems; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ImportRules()
        {
            mItems = new List<ImportRule>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Read the import rules from the given xml file...
        /// </summary>
        /// <param name="pathname"></param>
        /// <returns></returns>
        public bool ReadFromConfig(string pathname)
        {
            bool success = false;

            try
            {
                FileStream stream = new FileStream(pathname, FileMode.Open);

                success = ReadFromXMLFile(stream);

                stream.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Fehler in Regelkonfiguration (" + pathname + ")");
            }

            return success;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Read the import rules from an XML file
        /// </summary>
        /// <param name="stream">stream representing the xml file</param>
        /// <returns></returns>
        protected bool ReadFromXMLFile(Stream stream)
        {
            bool success = false;

            mItems.Clear();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;

            XmlReader reader = XmlReader.Create(stream, settings);

            if (reader.ReadToFollowing("ImportRules"))
            {
                bool isReading = reader.Read();

                while (isReading)
                {
                    if (reader.IsStartElement() && reader.Name == "ImportRule")
                    {
                        string content = reader.ReadOuterXml();

                        ImportRule newRule = new ImportRule();
                        if (newRule.setFromXML(content))
                        {
                            mItems.Add(newRule);
                        }
                    }
                    else if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "ImportRules"))
                    {
                        isReading = false;
                    }
                    else
                    {
                        isReading = reader.Read();
                    }
                }
            }

            return success;
        }

        #endregion
    }
}
