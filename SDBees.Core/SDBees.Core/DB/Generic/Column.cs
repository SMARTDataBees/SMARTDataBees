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
using SDBees.Core.Model;

namespace SDBees.DB
{
    /// <summary>
    /// Flags used for columns (not all apply to all SQL vendors
    /// </summary>
    public enum DbFlags
    {
        /// <summary>
        /// Allow DbNull to be set as a value
        /// </summary>
        eAllowNull          = 0x0001,
        /// <summary>
        /// Marks the column as IDENTITY in the SQL database
        /// </summary>
        eIdentity           = 0x0002,
        /// <summary>
        /// Marks the column (must be of type guid) as a unique row guid. This is currently
        /// only supported in Microsoft SQL. MySQL does not support Guids.
        /// </summary>
        eIsRowGuid          = 0x0004,
        /// <summary>
        /// Automatically increment the value of this column. Only compatible with integer
        /// types and might not be available for all SQL vendors.
        /// </summary>
        eAutoIncrement      = 0x0008,
        /// <summary>
        /// Set if the column has a default
        /// </summary>
        eHasDefault         = 0x0010,
        /// <summary>
        /// Mark this column to not be replicated. Not supported by all SQL vendors
        /// </summary>
        eNotForReplication  = 0x0020,
        /// <summary>
        /// Sets the UNIQUE key for this column. SQL server will complain/fail if a
        /// duplicate value is inserted
        /// </summary>
        eUnique             = 0x0040,
        /// <summary>
        /// Automatically create a value. Use for integer types, guids or guid strings only
        /// </summary>
        eAutoCreate         = 0x0080
    }

    /// <summary>
    /// Class representing a column in a table of a database
    /// </summary>
    public class Column
    {
        #region Private Data Members

        private string mName;
        private string mDisplayName;
        private string mDescription;
        private string mCategory;
        private List<string> mSelectionList;
        private DbType mType;
        private int mSize;
        private string mDefault;
        private int mFlags;
        private int mSeed;
        private int mIncrement;
        private bool mEditable;
        private bool mBrowsable = true;

        private string m_UITypeEditor;
        private string m_UITypeConverter;

        #endregion

        #region Consts
        //public const string IDColumnName = "id";
        //public const string NameColumnName = "name";
        //public const string NameDisplayName = "Name";
        #endregion

        #region Public Properties

        /// <summary>
        /// Type for the UI displayment
        /// You have to provide the full classname
        /// </summary>
        public string UITypeConverter
        {
            get { return m_UITypeConverter; }
            set { m_UITypeConverter = value; }
        }

        /// <summary>
        /// UITypeEditor for UI displayment
        /// You have to provide the full classname
        /// </summary>
        public string UITypeEditor
        {
            get { return m_UITypeEditor; }
            set { m_UITypeEditor = value; }
        }

        /// <summary>
        /// Name of the column in the table (persistent, don't change after first save)
        /// </summary>
        public string Name 
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Name of the column as displayed to the user
        /// </summary>
        public string DisplayName
        {
            get { return mDisplayName; }
            set { mDisplayName = value; }
        }

        /// <summary>
        /// Description of the column for the user
        /// </summary>
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        /// <summary>
        /// Get or set the category.
        /// Category this column should be grouped when displayed to the user
        /// </summary>
        public string Category
        {
            get { return mCategory; }
            set { mCategory = value; }
        }

        /// <summary>
        /// Get or set the selection list.
        /// If the type is a sting, then only this selection can be set by the user. The
        /// UI will display a combo box to select from
        /// </summary>
        public List<string> SelectionList
        {
            get { return mSelectionList; }
            set { mSelectionList = value; }
        }

        /// <summary>
        /// Get or set the comma separated representation of the selection list
        /// </summary>
        public string SelectionListString
        {
            get
            {
                var strBuilder = new StringBuilder();
                if (mSelectionList != null)
                {
                    foreach (var strValue in mSelectionList)
                    {
                        if (strBuilder.Length > 0)
                        {
                            strBuilder.Append(",");
                        }
                        strBuilder.Append(strValue);
                    }
                }
                return strBuilder.ToString();
            }
            set
            {
                if ((value == null) || (value.Trim() == ""))
                {
                    mSelectionList = null;
                }
                else
                {
                    var v = value.Split(',');
                    mSelectionList = new List<string>();
                    foreach (var strValue in v)
                    {
                        mSelectionList.Add(strValue.Trim());
                    }
                }
            }
        }

        public Type GetTypeForColumn()
        {
            Type returnType;

            switch (Type)
            {
                case DbType.CrossSize:
                    returnType = typeof(SDBeesOpeningSize);
                    break;

                case DbType.String:
                case DbType.StringFixed:
                case DbType.GuidString:
                case DbType.Text:
                case DbType.LongText:
                case DbType.Binary:
                    returnType = typeof(string);
                    break;

                case DbType.Boolean:
                    returnType = typeof(Boolean);
                    break;

                case DbType.Byte:
                    returnType = typeof(Byte);
                    break;

                case DbType.Double:
                case DbType.Currency:
                    returnType = typeof(Double);
                    break;

                case DbType.Date:
                case DbType.DateTime:
                    returnType = typeof(DateTime);
                    break;

                case DbType.Int64:
                case DbType.Decimal:
                    returnType = typeof(Int64);
                    break;

                case DbType.Guid:
                    returnType = typeof(Guid);
                    break;

                case DbType.Int16:
                    returnType = typeof(Int16);
                    break;

                case DbType.Int32:
                    returnType = typeof(Int32);
                    break;

                case DbType.Single:
                    returnType = typeof(Single);
                    break;

                default:
                    returnType = typeof(string);
                    break;
            }

            return returnType;
        }

        /// <summary>
        /// Get or set the persistent SQL type of this column
        /// </summary>
        public DbType Type
        {
            get { return mType; }
            set
            {
                mType = value;

                // Now check if a type has a specific custom size...
                if (mType == DbType.GuidString)
                {
                    mSize = 40;
                }
            }
        }

        /// <summary>
        /// Get or set the size of the values stored. This is not used by all types.
        /// </summary>
        public int Size
        {
            get { return mSize; }
            set { mSize = value; }
        }

        /// <summary>
        /// Get or set the default
        /// </summary>
        public string Default
        {
            get { return mDefault; }
            set { mDefault = value; }
        }

        /// <summary>
        /// Get or set the flags for the SQL type (see Flags description)
        /// </summary>
        public int Flags
        {
            get { return mFlags; }
            set { mFlags = value; }
        }

        /// <summary>
        /// Get or set the seed for automatic created values. Only used when eAutoCreate flag is set.
        /// </summary>
        public int Seed
        {
            get { return mSeed; }
            set { mSeed = value; }
        }

        /// <summary>
        /// Get or set the increment for automatic created values. Only used when eAutoCreate flag is set.
        /// </summary>
        public int Increment
        {
            get { return mIncrement; }
            set { mIncrement = value; }
        }

        /// <summary>
        /// Get or set the editable property. If this is false the user cannot edit the value.
        /// </summary>
        public bool Editable
        {
            get { return mEditable; }
            set { mEditable = value; }
        }

        /// <summary>
        /// Get or set the browsable property. If this is false the user cannot see the value.
        /// </summary>
        public bool Browsable
        {
            get { return mBrowsable; }
            set { mBrowsable = value; }
        }

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public Column()
        {
            Init();
        }

        /// <summary>
        /// Standard constructor with parameters
        /// </summary>
        /// <param name="name">Name of the column in the table</param>
        /// <param name="type">Type of the column</param>
        /// <param name="displayName">Name of the column as it is displayed to the user</param>
        /// <param name="description">Description of the column</param>
        /// <param name="category">Category where this column is grouped in</param>
        /// <param name="size">Size of the stored data, not used for all types</param>
        /// <param name="defaultValue">Default value if not passed on row creation</param>
        /// <param name="flags">Flags for this column</param>
        public Column(string name, DbType type, string displayName, string description, string category, int size, string defaultValue, int flags)
        {
            Init();

            mName = name;
            mDisplayName = displayName;
            mDescription = description;
            mCategory = category;
            mSelectionList = null;
            mType = type;
            mSize = size;
            mDefault = defaultValue;
            mFlags = flags;
            mBrowsable = true;

            // We might have to modify the size, flags, etc... to be consistent
            FixValues();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="otherColumn">Column to copy the definition from</param>
        public Column(Column otherColumn)
        {
            mName = otherColumn.mName;
            mDisplayName = otherColumn.mDisplayName;
            mDescription = otherColumn.Description;
            mCategory = otherColumn.Category;
            mSelectionList = null;
            if (otherColumn.mSelectionList != null)
            {
                mSelectionList = new List<string>();
                foreach (var entry in otherColumn.mSelectionList)
                {
                    mSelectionList.Add(entry);
                }
            }
            mType = otherColumn.mType;
            mSize = otherColumn.mSize;
            mDefault = otherColumn.mDefault;
            mFlags = otherColumn.mFlags;
            mEditable = otherColumn.mEditable;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create a default value for this column
        /// </summary>
        /// <returns>A valid default for this column</returns>
        public object CreateDefaultValue()
        {
            object defaultValue = null;

            if (mType == DbType.CrossSize)
            {
                defaultValue = new SDBeesOpeningSize();
            }
            else if ((mType == DbType.String) || (mType == DbType.StringFixed) || (mType == DbType.Text) || (mType == DbType.LongText))
            {
                defaultValue = "";
            }
            else if (mType == DbType.GuidString)
            {
                defaultValue = Guid.Empty.ToString();
            }
            else if (mType == DbType.Boolean)
            {
                defaultValue = false;
            }
            else if ((mType == DbType.Binary) ||
                     (mType == DbType.Byte) ||
                     (mType == DbType.Currency) ||
                     (mType == DbType.Decimal) ||
                     (mType == DbType.Double) ||
                     (mType == DbType.Int16) ||
                     (mType == DbType.Int32) ||
                     (mType == DbType.Int64) ||
                     (mType == DbType.Single))
            {
                defaultValue = 0;
            }
            else if (mType == DbType.Date)
            {
                defaultValue = DateTime.Now;
            }
            else if (mType == DbType.DateTime)
            {
                defaultValue = DateTime.Now;
            }

            return defaultValue;
        }

        /// <summary>
        /// Convert a value from the database to a value for this column
        /// </summary>
        /// <param name="value">The value from the database</param>
        /// <returns>The converted value</returns>
        public object ConvertValueFromDataRow(object value)
        {
            object result = null;

            if (mType == DbType.CrossSize)
            {
                result = new SDBeesOpeningSize(value.ToString());
            }
            else if (mType == DbType.Boolean)
            {
                result = value.ToString() != "0" ? true : false;
            }
            else
            {
                result = value;
            }

            return result;
        }

        /// <summary>
        /// Check if this value is valid for this column (type)
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool IsValidValue(string defaultValue)
        {
            var isValid = false;

            try
            {
                if (mType == DbType.CrossSize)
                {
                    isValid = true; // TODO: RALF CHECK THIS!
                }
                else if ((mType == DbType.String) || (mType == DbType.StringFixed) || (mType == DbType.Text) || (mType == DbType.LongText))
                {
                    isValid = true;
                }
                else if (mType == DbType.GuidString)
                {
                    var myGuid = new Guid(defaultValue);
                    isValid = true;
                }
                else if (mType == DbType.Binary)
                {
                    isValid = true;
                }
                else if (mType == DbType.Boolean)
                {
                    defaultValue = defaultValue.Trim();
                    isValid = (defaultValue == "true") || (defaultValue == "false");
                }
                else if (mType == DbType.Byte)
                {
                    var value = (Byte)Convert.ChangeType(defaultValue, typeof(Byte));
                    isValid = true;
                }
                else if (mType == DbType.Int16)
                {
                    var value = (Int16)Convert.ChangeType(defaultValue, typeof(Int16));
                    isValid = true;
                }
                else if (mType == DbType.Int32)
                {
                    var value = (Int32)Convert.ChangeType(defaultValue, typeof(Int32));
                    isValid = true;
                }
                else if (mType == DbType.Int64)
                {
                    var value = (Int64)Convert.ChangeType(defaultValue, typeof(Int64));
                    isValid = true;
                }
                else if ((mType == DbType.Double) || (mType == DbType.Decimal) || (mType == DbType.Currency))
                {
                    var value = (Double)Convert.ChangeType(defaultValue, typeof(Double));
                    isValid = true;
                }
                else if (mType == DbType.Single)
                {
                    var value = (Single)Convert.ChangeType(defaultValue, typeof(Single));
                    isValid = true;
                }
                else if (mType == DbType.Date)
                {
                    // TBD: ...
                    isValid = true;
                }
                else if (mType == DbType.DateTime)
                {
                    // TBD: ...
                    isValid = true;
                }
            }
            catch(Exception)
            {
                isValid = false;
            }

            return isValid;
        }

        /// <summary>
        /// Check if this column has a custom size
        /// </summary>
        /// <returns>Returns true for binary, string fixed string and guid string types</returns>
        public bool HasCustomSize()
        {
            return (mType == DbType.CrossSize) || (mType == DbType.Binary) || (mType == DbType.String) || (mType == DbType.StringFixed) || (mType == DbType.GuidString);
        }

        /// <summary>
        /// Check if this column can have a selection list
        /// </summary>
        /// <returns>Returns true for string and fixed string types</returns>
        public bool IsSelectionListPossible()
        {
            return (mType == DbType.String) || (mType == DbType.StringFixed);
        }

        /// <summary>
        /// If some setting is wrong, this this method fixes the definition as required. This
        /// makes some implementation easier. The size, default value and the default flag may be
        /// modified.
        /// </summary>
        public void FixValues()
        {
            // Now check if a type has a specific custom size...
            if (mType == DbType.GuidString)
            {
                mSize = 40;
            }
            else if ((mSize < 1) && ((mType == DbType.String) || (mType == DbType.StringFixed)))
            {
                mSize = 50;
            }

            // Check if we need to force a default
            if (((mFlags & (int)DbFlags.eHasDefault) == 0) && ((mFlags & (int)DbFlags.eAllowNull) == 0) && ((mFlags & (int)DbFlags.eAutoCreate) == 0))
            {
                mFlags |= (int)DbFlags.eHasDefault;
                if (mDefault == "")
                {
                    mDefault = CreateDefaultValue().ToString();
                }
            }
            else if (((mFlags & (int)DbFlags.eHasDefault) != 0) && ((mFlags & (int)DbFlags.eAllowNull) != 0) && ((mFlags & (int)DbFlags.eAutoCreate) == 0))
            {
                if (mDefault == "")
                {
                    mFlags &= ~(int)DbFlags.eHasDefault;
                }
            }

            // Check if the default is valid...
            if ((mFlags & (int)DbFlags.eHasDefault) != 0)
            {
                if (!IsValidValue(mDefault))
                {
                    mDefault = CreateDefaultValue().ToString();
                }
            }
        }

        /// <summary>
        /// Get an XML description of this column definition
        /// </summary>
        /// <returns>XML description</returns>
        public string Xmlwrite()
        {
            var strXml = "";

            // create the XML structure
            var settings = new XmlWriterSettings();
            settings.Indent = false;
            settings.IndentChars = ("  ");
            var xmlStringBuilder = new StringBuilder();
            using (var writer = XmlWriter.Create(xmlStringBuilder, settings))
            {
                // Write XML data.
                writer.WriteStartElement("Column");
                writer.WriteAttributeString("Name", mName);
                writer.WriteAttributeString("DisplayName", mDisplayName);
                writer.WriteAttributeString("Description", mDescription);
                writer.WriteAttributeString("Category", mCategory);
                writer.WriteAttributeString("DBType", ((int)mType).ToString());

                if (mSelectionList != null)
                {
                    writer.WriteAttributeString("SelectionList", SelectionListString);
                }

                writer.WriteAttributeString("Size", mSize.ToString());
                writer.WriteAttributeString("Default", mDefault);
                writer.WriteAttributeString("Flags", mFlags.ToString());
                writer.WriteAttributeString("Seed", mSeed.ToString());
                writer.WriteAttributeString("Increment", mIncrement.ToString());
                writer.WriteAttributeString("Editable", mEditable.ToString());
                writer.WriteAttributeString("Browsable", mBrowsable.ToString());

                writer.WriteAttributeString("UIType", m_UITypeConverter);
                writer.WriteAttributeString("UITypeEditor", m_UITypeEditor);

                writer.WriteEndElement(); // Column
                writer.Flush();

                strXml = xmlStringBuilder.ToString();
            }

            return strXml;
        }

        /// <summary>
        /// Define this column based on the given XML description. This will reset all values first.
        /// </summary>
        /// <param name="xmlContent">XML description</param>
        public void Xmlread(string xmlContent)
        {
            // reset values...
            Init();

            // Interpret the xml
            // Interpret the xml and create the necessary columns
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlContent);

            var rootNode = xmlDoc.SelectSingleNode("Column");
            XmlReader reader = new XmlNodeReader(rootNode);

            reader.Read();
            mName = reader.GetAttribute("Name");
            mDisplayName = reader.GetAttribute("DisplayName");
            mDescription = reader.GetAttribute("Description");
            mCategory = reader.GetAttribute("Category");
            mType = (DbType) Convert.ToInt32(reader.GetAttribute("DBType"));
            mSize = Convert.ToInt32(reader.GetAttribute("Size"));
            mDefault = reader.GetAttribute("Default");
            mFlags = Convert.ToInt32(reader.GetAttribute("Flags"));
            mSeed = Convert.ToInt32(reader.GetAttribute("Seed"));
            mIncrement = Convert.ToInt32(reader.GetAttribute("Increment"));
            mEditable = Convert.ToBoolean(reader.GetAttribute("Editable"));
            mBrowsable = Convert.ToBoolean(reader.GetAttribute("Browsable"));
            try
            {
                m_UITypeConverter = reader.GetAttribute("UIType");
                m_UITypeEditor = reader.GetAttribute("UITypeEditor");
            }
            catch (Exception ex)
            {
            }
            var strTmp = reader.GetAttribute("SelectionList");
            SelectionListString = strTmp;
        }


        #endregion

        #region Protected Methods

        private void Init()
        {
            mName = "";
            mDisplayName = "";
            mDescription = "";
            mCategory = "";
            mSelectionList = null;
            mType = DbType.Unknown;
            mSize = 0;
            mDefault = "";
            mFlags = 0;
            mEditable = true;

            m_UITypeConverter = "";
            m_UITypeEditor = "";
        }

        #endregion
    }

    /// <summary>
    /// Collection of columns as a dictionary for quick access by the column name.
    /// </summary>
    public class Columns : Dictionary<string, Column>
    {
        /// <summary>
        /// Adds a column to the collection. The name must have been set.
        /// </summary>
        /// <param name="column"></param>
        public void Add(Column column)
        {
            Add(column.Name, column);
        }
    }
}
