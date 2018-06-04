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
using System.Collections.Specialized;
using System.Text;
using System.Xml;
using System.IO;

namespace SDBees.Connectivity
{
    /// <summary>
    /// Class to configure an import rule
    /// </summary>
    public class ImportRule
    {
        #region Private Data Members

        private string mExternalObjectType;
        private string mInternalObjectType;
        private string mCriteria;
        private string mDefaultName;
        private Hashtable mIncludeCriteria;
        private Hashtable mExcludeCriteria;
        private Hashtable mAttributes;

        #endregion

        #region Public Properties

        /// <summary>
        /// Get/Set the external entity type (CAD Type)
        /// </summary>
        public string ExternalObjectType
        {
            get { return mExternalObjectType; }
            set { mExternalObjectType = value; }
        }

        /// <summary>
        /// Get/Set the internal entity type (SDBees Type)
        /// </summary>
        public string InternalObjectType
        {
            get { return mInternalObjectType; }
            set { mInternalObjectType = value; }
        }

        /// <summary>
        /// Get/Set the criteria for which an external type should imported into an internal type.
        /// This is an XML defined string that can vary depending on the underlying implementation.
        /// </summary>
        public string Criteria
        {
            get { return mCriteria; }
            set { mCriteria = value; }
        }

        /// <summary>
        /// The default label used for the node
        /// </summary>
        public string DefaultName
        {
            get { return mDefaultName; }
            set { mDefaultName = value; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public ImportRule()
            : this("ExternType", "InternType", "", "")
        {

        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="externType">Type in extern data</param>
        /// <param name="internType">Type in SDBees</param>
        /// <param name="criteria">criteria as XML description</param>
        /// <param name="defaultName">Default name for the object</param>
        public ImportRule(string externType, string internType, string criteria, string defaultName)
        {
            mExternalObjectType = externType;
            mInternalObjectType = internType;
            mCriteria = criteria;
            mDefaultName = defaultName;

            mIncludeCriteria = new Hashtable();
            mExcludeCriteria = new Hashtable();
            mAttributes = new Hashtable();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the rule from an XML definition.
        /// </summary>
        /// <param name="xmlString">complete XML definition including the main ImportRule tag</param>
        /// <returns></returns>
        public bool setFromXML(string xmlString)
        {
            bool success = false;

            mIncludeCriteria.Clear();
            mExcludeCriteria.Clear();
            mAttributes.Clear();

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;

            XmlReader reader = XmlReader.Create(new StringReader(xmlString), settings);

            if (reader.ReadToFollowing("ImportRule"))
            {
                mExternalObjectType = reader.GetAttribute("ExternType");
                mInternalObjectType = reader.GetAttribute("InternType");

                if ((mExternalObjectType != null) && (mInternalObjectType != null))
                {
                    success = true;
                    bool isReading = reader.Read();

                    while (isReading)
                    {
                        if (reader.IsStartElement() && reader.Name == "Criteria")
                        {
                            mCriteria = reader.ReadInnerXml();
                        }
                        else if (reader.IsStartElement() && reader.Name == "Defaults")
                        {
                            object attValue = reader.GetAttribute("name");
                            if (attValue != null)
                            {
                                mDefaultName = attValue.ToString();
                            }

                            isReading = reader.Read();
                        }
                        else if (reader.IsStartElement())
                        {
                            string attName = reader.Name;
                            object attValue = reader.GetAttribute("value");
                            if ((attValue != null) && (attName != ""))
                            {
                                mAttributes[attName] = attValue;
                            }

                            isReading = reader.Read();
                        }
                        else if ((reader.NodeType == XmlNodeType.EndElement) && (reader.Name == "ImportRule"))
                        {
                            isReading = false;
                        }
                        else
                        {
                            isReading = reader.Read();
                        }
                    }

                    if (success)
                    {
                        AnalyseCriteria();
                    }
                }
            }

            return success;
        }

        /// <summary>
        /// Checks if the properties match the including/excluding criterias
        /// </summary>
        /// <param name="properties">A collection of pairs of keys and values</param>
        /// <returns></returns>
        public bool Matches(NameValueCollection properties)
        {
            bool isMatching = true;

            foreach (string key in properties.AllKeys)
            {
                string value = properties[key];

                if (mIncludeCriteria.ContainsKey(key))
                {
                    List<string> values = (List<string>)mIncludeCriteria[key];
                    isMatching = isMatching && values.Contains(value);
                }

                if (mExcludeCriteria.ContainsKey(key))
                {
                    List<string> values = (List<string>)mExcludeCriteria[key];
                    isMatching = isMatching && !values.Contains(value);
                }

                if (!isMatching)
                {
                    break;
                }
            }

            return isMatching;
        }

        #endregion

        #region Protected Methods

        private void AnalyseCriteria()
        {
            mIncludeCriteria.Clear();
            mExcludeCriteria.Clear();

            if (mCriteria == "")
            {
                return;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;

            XmlReader reader = XmlReader.Create(new StringReader(mCriteria), settings);

            bool isReading = reader.Read();

            while (isReading)
            {
                if (reader.IsStartElement() && reader.Name == "Include")
                {
                    string content = reader.ReadInnerXml();

                    AnalyseCriteria(mIncludeCriteria, content, settings);
                }
                else if (reader.IsStartElement() && reader.Name == "Exclude")
                {
                    string content = reader.ReadInnerXml();

                    AnalyseCriteria(mExcludeCriteria, content, settings);
                }
                else
                {
                    isReading = reader.Read();
                }
            }
        }

        private void AnalyseCriteria(Hashtable hash, string xmlContent, XmlReaderSettings settings)
        {
            if (xmlContent == "")
            {
                return;
            }

            XmlReader reader = XmlReader.Create(new StringReader(xmlContent), settings);

            while (reader.Read())
            {
                if (reader.IsStartElement())
                {
                    string tag = reader.Name;
                    string value = reader.GetAttribute("value");
                    if ((tag != "") && (value != ""))
                    {
                        List<string> values = null;
                        if (!hash.ContainsKey(tag))
                        {
                            values = new List<string>();
                            hash.Add(tag, values);
                        }
                        else
                        {
                            values = (List<string>)hash[tag];
                        }

                        values.Add(value);
                    }
                }
            }
        }

        #endregion
    }
}
