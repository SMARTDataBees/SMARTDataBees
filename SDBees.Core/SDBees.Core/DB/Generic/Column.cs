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
using System.Xml.Serialization;
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
        eAllowNull = 0x0001,
        /// <summary>
        /// Marks the column as IDENTITY in the SQL database
        /// </summary>
        eIdentity = 0x0002,
        /// <summary>
        /// Marks the column (must be of type guid) as a unique row guid. This is currently
        /// only supported in Microsoft SQL. MySQL does not support Guids.
        /// </summary>
        eIsRowGuid = 0x0004,
        /// <summary>
        /// Automatically increment the value of this column. Only compatible with integer
        /// types and might not be available for all SQL vendors.
        /// </summary>
        eAutoIncrement = 0x0008,
        /// <summary>
        /// Set if the column has a default
        /// </summary>
        eHasDefault = 0x0010,
        /// <summary>
        /// Mark this column to not be replicated. Not supported by all SQL vendors
        /// </summary>
        eNotForReplication = 0x0020,
        /// <summary>
        /// Sets the UNIQUE key for this column. SQL server will complain/fail if a
        /// duplicate value is inserted
        /// </summary>
        eUnique = 0x0040,
        /// <summary>
        /// Automatically create a value. Use for integer types, guids or guid strings only
        /// </summary>
        eAutoCreate = 0x0080
    }

    /// <summary>
    /// Class representing a column in a table of a database
    /// </summary>
    public class Column
    {
        #region Private Data Members

        private DbType mType;

        #endregion

 

        #region Public Properties

        /// <summary>
        /// Type for the UI displayment
        /// You have to provide the full classname
        /// </summary>
        [XmlAttribute("UIType")]
        public string UITypeConverter { get; set; }

        /// <summary>
        /// UITypeEditor for UI displayment
        /// You have to provide the full classname
        /// </summary>
        [XmlAttribute("UITypeEditor")]
        public string UITypeEditor { get; set; }

        /// <summary>
        /// Name of the column in the table (persistent, don't change after first save)
        /// </summary>
        [XmlAttribute("Name")]
        public string Name { get; set; }

        /// <summary>
        /// Name of the column as displayed to the user
        /// </summary>
        [XmlAttribute("DisplayName")]
        public string DisplayName { get; set; }

        /// <summary>
        /// Description of the column for the user
        /// </summary>
        [XmlAttribute("Description")]
        public string Description { get; set; }

        /// <summary>
        /// Get or set the category.
        /// Category this column should be grouped when displayed to the user
        /// </summary>
        [XmlAttribute("Category")]
        public string Category { get; set; }

        /// <summary>
        /// Get or set the selection list.
        /// If the type is a sting, then only this selection can be set by the user. The
        /// UI will display a combo box to select from
        /// </summary>
        [XmlIgnore]
        public List<string> SelectionList { get; set; }


        /// <summary>
        /// Xml helper for the property ({Propertyname}Specified)
        /// </summary>
        [XmlIgnore]
        public bool SelectionListStringSpecified => string.IsNullOrEmpty(SelectionListString) == false;

        /// <summary>
        /// Get or set the comma separated representation of the selection list
        /// </summary>
        [XmlAttribute("SelectionList")]
        public string SelectionListString
        {
            get
            {
                var strBuilder = new StringBuilder();
                if (SelectionList != null)
                {
                    foreach (var strValue in SelectionList)
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
                    SelectionList = null;
                }
                else
                {
                    var v = value.Split(',');
                    SelectionList = new List<string>();
                    foreach (var strValue in v)
                    {
                        SelectionList.Add(strValue.Trim());
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
                    returnType = typeof(bool);
                    break;

                case DbType.Byte:
                    returnType = typeof(byte);
                    break;

                case DbType.Double:
                case DbType.Currency:
                    returnType = typeof(double);
                    break;

                case DbType.Date:
                case DbType.DateTime:
                    returnType = typeof(DateTime);
                    break;

                case DbType.Int64:
                case DbType.Decimal:
                    returnType = typeof(long);
                    break;

                case DbType.Guid:
                    returnType = typeof(Guid);
                    break;

                case DbType.Int16:
                    returnType = typeof(short);
                    break;

                case DbType.Int32:
                    returnType = typeof(int);
                    break;

                case DbType.Single:
                    returnType = typeof(float);
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
        [XmlAttribute("DBType")]
        public DbType Type
        {
            get => mType;
            set
            {
                mType = value;

                // Now check if a type has a specific custom size...
                if (mType == DbType.GuidString)
                {
                    Size = 40;
                }
            }
        }

        /// <summary>
        /// Get or set the size of the values stored. This is not used by all types.
        /// </summary>
        [XmlAttribute("Size")]
        public int Size { get; set; }

        /// <summary>
        /// Get or set the default
        /// </summary>
        [XmlAttribute("Default")]
        public string Default { get; set; }

        /// <summary>
        /// Get or set the flags for the SQL type (see Flags description)
        /// </summary>
        [XmlAttribute("Flags")]
        public int Flags { get; set; }

        /// <summary>
        /// Get or set the seed for automatic created values. Only used when eAutoCreate flag is set.
        /// </summary>
        [XmlAttribute("Seed")]
        public int Seed { get; set; }

        /// <summary>
        /// Get or set the increment for automatic created values. Only used when eAutoCreate flag is set.
        /// </summary>
        [XmlAttribute("Increment")]
        public int Increment { get; set; }



        /// <summary>
        /// Get or set the editable property. If this is false the user cannot edit the value.
        /// </summary>
        [XmlIgnore]
        public bool IsEditable { get; set; }


        /// <summary>
        /// Xml helper value for the serializer
        /// </summary>
        [XmlAttribute("Editable ")]
        public string Editable
        {
            get => IsEditable ? "True" : "False";
            set => IsEditable = string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) != 0;
        }

        /// <summary>
        /// Get or set the browsable property. If this is false the user cannot see the value.
        /// </summary>
        [XmlIgnore]
        public bool IsBrowsable { get; set; } = true;

        /// <summary>
        /// Xml helper value for the serializer
        /// </summary>
        [XmlAttribute("Browsable ")]
        public string Browsable
        {
            get => IsBrowsable ? "True" : "False";
            set => IsBrowsable = string.Compare(value, "false", StringComparison.OrdinalIgnoreCase) != 0;
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

            Name = name;
            DisplayName = displayName;
            Description = description;
            Category = category;
            SelectionList = null;
            mType = type;
            Size = size;
            Default = defaultValue;
            Flags = flags;
            IsBrowsable = true;

            // We might have to modify the size, flags, etc... to be consistent
            FixValues();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="otherColumn">Column to copy the definition from</param>
        public Column(Column otherColumn)
        {
            Name = otherColumn.Name;
            DisplayName = otherColumn.DisplayName;
            Description = otherColumn.Description;
            Category = otherColumn.Category;
            SelectionList = null;
            if (otherColumn.SelectionList != null)
            {
                SelectionList = new List<string>();
                foreach (var entry in otherColumn.SelectionList)
                {
                    SelectionList.Add(entry);
                }
            }
            mType = otherColumn.mType;
            Size = otherColumn.Size;
            Default = otherColumn.Default;
            Flags = otherColumn.Flags;
            IsEditable = otherColumn.IsEditable;
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
            object result;

            if (mType == DbType.CrossSize)
            {
                result = new SDBeesOpeningSize(value.ToString());
            }
            else if (mType == DbType.Boolean)
            {
                result = value.ToString() != "0";
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
                    var value = (byte)Convert.ChangeType(defaultValue, typeof(byte));
                    isValid = true;
                }
                else if (mType == DbType.Int16)
                {
                    var value = (short)Convert.ChangeType(defaultValue, typeof(short));
                    isValid = true;
                }
                else if (mType == DbType.Int32)
                {
                    var value = (int)Convert.ChangeType(defaultValue, typeof(int));
                    isValid = true;
                }
                else if (mType == DbType.Int64)
                {
                    var value = (long)Convert.ChangeType(defaultValue, typeof(long));
                    isValid = true;
                }
                else if ((mType == DbType.Double) || (mType == DbType.Decimal) || (mType == DbType.Currency))
                {
                    var value = (double)Convert.ChangeType(defaultValue, typeof(double));
                    isValid = true;
                }
                else if (mType == DbType.Single)
                {
                    var value = (float)Convert.ChangeType(defaultValue, typeof(float));
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
            catch (Exception)
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
                Size = 40;
            }
            else if ((Size < 1) && ((mType == DbType.String) || (mType == DbType.StringFixed)))
            {
                Size = 50;
            }

            // Check if we need to force a default
            if (((Flags & (int)DbFlags.eHasDefault) == 0) && ((Flags & (int)DbFlags.eAllowNull) == 0) && ((Flags & (int)DbFlags.eAutoCreate) == 0))
            {
                Flags |= (int)DbFlags.eHasDefault;
                if (Default == "")
                {
                    Default = CreateDefaultValue().ToString();
                }
            }
            else if (((Flags & (int)DbFlags.eHasDefault) != 0) && ((Flags & (int)DbFlags.eAllowNull) != 0) && ((Flags & (int)DbFlags.eAutoCreate) == 0))
            {
                if (Default == "")
                {
                    Flags &= ~(int)DbFlags.eHasDefault;
                }
            }

            // Check if the default is valid...
            if ((Flags & (int)DbFlags.eHasDefault) != 0)
            {
                if (!IsValidValue(Default))
                {
                    Default = CreateDefaultValue().ToString();
                }
            }
        }


        #endregion

        #region Protected Methods

        private void Init()
        {
            Name = "";
            DisplayName = "";
            Description = "";
            Category = "";
            SelectionList = null;
            mType = DbType.Unknown;
            Size = 0;
            Default = "";
            Flags = 0;
            IsEditable = true;

            UITypeConverter = "";
            UITypeEditor = "";
        }

        #endregion
    }

}
