// #StartHeader# ==============================================================
//
// This file is A part of the SMARTDataBees open source project.
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
// You should have received A copy of the GNU General Public License
// along with SMARTDataBees.  If not, see <http://www.gnu.org/licenses/>.
//
// #EndHeader# ================================================================
/********************************************************************
 *
 *  PropertyBag.cs
 *  --------------
 *  Copyright (C) 2002  Tony Allowatt
 *  Last Update: 12/14/2002
 * 
 *  THE SOFTWARE IS PROVIDED BY THE AUTHOR "AS IS", WITHOUT WARRANTY
 *  OF ANY KIND, EXPRESS OR IMPLIED. IN NO EVENT SHALL THE AUTHOR BE
 *  LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OF THIS
 *  SOFTWARE.
 * 
 *  Public types defined in this file:
 *  ----------------------------------
 *  namespace Flobbster.Windows.Forms
 *     class PropertySpec
 *     class PropertySpecEventArgs
 *     delegate PropertySpecEventHandler
 *     class PropertyBag
 *        class PropertyBag.PropertySpecCollection
 *     class PropertyTable
 *
 ********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace SDBees.Plugs.Properties
{
    /// <summary>
    /// Represents A single property in A PropertySpec.
    /// </summary>
    public class PropertySpec
    {
        private IList<Attribute> attributes;
        private string category;
        private object defaultValue;
        private string description;
        private string editor;
        private string name;
        private string type;
        private string typeConverter;

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        public PropertySpec(string name, string type) : this(name, type, null, null, null) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        public PropertySpec(string name, Type type) :
            this(name, type.AssemblyQualifiedName, null, null, null) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        public PropertySpec(string name, string type, string category) : this(name, type, category, null, null) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category"></param>
        public PropertySpec(string name, Type type, string category) :
            this(name, type.AssemblyQualifiedName, category, null, null) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        public PropertySpec(string name, string type, string category, string description) :
            this(name, type, category, description, null) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        public PropertySpec(string name, Type type, string category, string description) :
            this(name, type.AssemblyQualifiedName, category, description, null) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        public PropertySpec(string name, string type, string category, string description, object defaultValue)
        {
            this.name = name;
            this.type = type;
            this.category = category;
            this.description = description;
            this.defaultValue = defaultValue;
            attributes = new List<Attribute>();
        }

        public PropertySpec()
        {
            attributes = new List<Attribute>();
        }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue) :
            this(name, type.AssemblyQualifiedName, category, description, defaultValue) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="typeConverter">The fully qualified name of the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue, string typeConverter)
            : this(name, type, category, description, defaultValue)
        {
            this.typeConverter = typeConverter;
        }        
        
        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="typeConverter">The fully qualified name of the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, string type, string category, string description, object defaultValue, Type typeConverter)
            : this(name, type, category, description, defaultValue)
        {
            this.typeConverter = typeConverter.AssemblyQualifiedName;
        }        

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The fully qualified name of the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The fully qualified name of the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, string type, string category, string description, object defaultValue,
            string editor, string typeConverter)
            : this(name, type, category, description, defaultValue)
        {
            this.editor = editor;
            this.typeConverter = typeConverter;
        }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The fully qualified name of the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The fully qualified name of the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue,
            string editor, string typeConverter) :
            this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor, typeConverter) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The Type that represents the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The fully qualified name of the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, string type, string category, string description, object defaultValue,
            Type editor, string typeConverter) :
            this(name, type, category, description, defaultValue, editor.AssemblyQualifiedName,
            typeConverter) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The Type that represents the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The fully qualified name of the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue,
            Type editor, string typeConverter) :
            this(name, type.AssemblyQualifiedName, category, description, defaultValue,
            editor.AssemblyQualifiedName, typeConverter) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The fully qualified name of the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The Type that represents the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, string type, string category, string description, object defaultValue,
            string editor, Type typeConverter) :
            this(name, type, category, description, defaultValue, editor, typeConverter.AssemblyQualifiedName) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The fully qualified name of the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The Type that represents the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue,
            string editor, Type typeConverter) :
            this(name, type.AssemblyQualifiedName, category, description, defaultValue, editor,
            typeConverter.AssemblyQualifiedName) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">The fully qualified name of the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The Type that represents the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The Type that represents the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, string type, string category, string description, object defaultValue,
            Type editor, Type typeConverter) :
            this(name, type, category, description, defaultValue, editor.AssemblyQualifiedName,
            typeConverter.AssemblyQualifiedName) { }

        /// <summary>
        /// Initializes A new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        /// <param name="category">The category under which the property is displayed in the
        /// property grid.</param>
        /// <param name="description">A string that is displayed in the help area of the
        /// property grid.</param>
        /// <param name="defaultValue">The default value of the property, or null if there is
        /// no default value.</param>
        /// <param name="editor">The Type that represents the type of the editor for this
        /// property.  This type must derive from UITypeEditor.</param>
        /// <param name="typeConverter">The Type that represents the type of the type
        /// converter for this property.  This type must derive from TypeConverter.</param>
        public PropertySpec(string name, Type type, string category, string description, object defaultValue,
            Type editor, Type typeConverter) :
            this(name, type.AssemblyQualifiedName, category, description, defaultValue,
            editor.AssemblyQualifiedName, typeConverter.AssemblyQualifiedName) { }

        /// <summary>
        /// Gets or sets A collection of additional Attributes for this property.  This can
        /// be used to specify attributes beyond those supported intrinsically by the
        /// PropertySpec class, such as ReadOnly and Browsable.
        /// </summary>
        public IList<Attribute> Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        /// <summary>
        /// Gets or sets the category name of this property.
        /// </summary>
        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        /// <summary>
        /// Gets or sets the fully qualified name of the type converter
        /// type for this property.
        /// </summary>
        public string ConverterTypeName
        {
            get { return typeConverter; }
            set { typeConverter = value; }
        }

        /// <summary>
        /// Gets or sets the default value of this property.
        /// </summary>
        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        /// <summary>
        /// Gets or sets the help text description of this property.
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        bool _readonly;
        /// <summary>
        /// Gets or sets the readonly Attribute for this property.
        /// </summary>
        public bool ReadOnlyProperty
        {
            get { return _readonly; }
            set { _readonly = value; }
        }

        bool _browsable = true;
        /// <summary>
        /// Gets or sets the browsable Attribute for this property.
        /// </summary>
        public bool BrowsableProperty
        {
            get { return _browsable; }
            set { _browsable = value; }
        }

        /// <summary>
        /// Gets or sets the fully qualified name of the editor type for
        /// this property.
        /// </summary>
        public string EditorTypeName
        {
            get { return editor; }
            set { editor = value; }
        }

        /// <summary>
        /// Gets or sets the name of this property.
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Gets or sets the fully qualfied name of the type of this
        /// property.
        /// </summary>
        public string TypeName
        {
            get { return type; }
            set { type = value; }
        }
    }

    /// <summary>
    /// Provides data for the GetValue and SetValue events of the PropertyBag class.
    /// </summary>
    public class PropertySpecEventArgs : EventArgs
    {
        private PropertySpec property;
        private object val;
        private bool updateProperties;

        /// <summary>
        /// Initializes A new instance of the PropertySpecEventArgs class.
        /// </summary>
        /// <param name="property">The PropertySpec that represents the property whose
        /// value is being requested or set.</param>
        /// <param name="val">The current value of the property.</param>
        public PropertySpecEventArgs(PropertySpec property, object val, bool updateProperties = true)
        {
            this.property = property;
            this.val = val;
            this.updateProperties = updateProperties;
        }

        /// <summary>
        /// Gets the PropertySpec that represents the property whose value is being
        /// requested or set.
        /// </summary>
        public PropertySpec Property
        {
            get { return property; }
        }

        /// <summary>
        /// Gets or sets the current value of the property.
        /// </summary>
        public object Value
        {
            get { return val; }
            set { val = value; }
        }

        public bool UpdateProperties
        {
            get { return updateProperties; }
            set { updateProperties = value; }
        }
    }

    /// <summary>
    /// Represents the method that will handle the GetValue and SetValue events of the
    /// PropertyBag class.
    /// </summary>
    public delegate void PropertySpecEventHandler(object sender, PropertySpecEventArgs e);

    #region PropertySpecCollection class definition
    /// <summary>
    /// Encapsulates A collection of PropertySpec objects.
    /// </summary>
    [Serializable]
    public class PropertySpecCollection : IList<PropertySpec>, IList
    {
        #region existing
        private IList<PropertySpec> innerList;

        public IList<PropertySpec> InnerList
        {
            get { return innerList; }
            set { innerList = value; }
        }

        /// <summary>
        /// Initializes A new instance of the PropertySpecCollection class.
        /// </summary>
        public PropertySpecCollection()
        {
            innerList = new List<PropertySpec>();
        }

        /// <summary>
        /// Gets the number of elements in the PropertySpecCollection.
        /// </summary>
        /// <value>
        /// The number of elements contained in the PropertySpecCollection.
        /// </value>
        public int Count
        {
            get { return innerList.Count; }
        }

        /// <summary>
        /// Gets A value indicating whether the PropertySpecCollection has A fixed size.
        /// </summary>
        /// <value>
        /// true if the PropertySpecCollection has A fixed size; otherwise, false.
        /// </value>
        public bool IsFixedSize
        {
            get { return false; }
        }

        /// <summary>
        /// Gets A value indicating whether the PropertySpecCollection is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets A value indicating whether access to the collection is synchronized (thread-safe).
        /// </summary>
        /// <value>
        /// true if access to the PropertySpecCollection is synchronized (thread-safe); otherwise, false.
        /// </value>
        public bool IsSynchronized
        {
            get { return false; }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the collection.
        /// </summary>
        /// <value>
        /// An object that can be used to synchronize access to the collection.
        /// </value>
        object ICollection.SyncRoot
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// In C#, this property is the indexer for the PropertySpecCollection class.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <value>
        /// The element at the specified index.
        /// </value>
        public PropertySpec this[int index]
        {
            get { return innerList[index]; }
            set { innerList[index] = value; }
        }

        /// <summary>
        /// Adds A PropertySpec to the end of the PropertySpecCollection.
        /// </summary>
        /// <param name="value">The PropertySpec to be added to the end of the PropertySpecCollection.</param>
        /// <returns>The PropertySpecCollection index at which the value has been added.</returns>
        public void Add(PropertySpec value)
        {
            innerList.Add(value);
        }

        /// <summary>
        /// Adds the elements of an array of PropertySpec objects to the end of the PropertySpecCollection.
        /// </summary>
        /// <param name="array">The PropertySpec array whose elements should be added to the end of the
        /// PropertySpecCollection.</param>
        public void AddRange(PropertySpec[] array)
        {
            foreach (var item in array)
            {
                innerList.Add(item);
            }
        }

        /// <summary>
        /// Removes all elements from the PropertySpecCollection.
        /// </summary>
        public void Clear()
        {
            innerList.Clear();
        }

        /// <summary>
        /// Determines whether A PropertySpec is in the PropertySpecCollection.
        /// </summary>
        /// <param name="item">The PropertySpec to locate in the PropertySpecCollection. The element to locate
        /// can be A null reference (Nothing in Visual Basic).</param>
        /// <returns>true if item is found in the PropertySpecCollection; otherwise, false.</returns>
        public bool Contains(PropertySpec item)
        {
            return innerList.Contains(item);
        }

        /// <summary>
        /// Determines whether A PropertySpec with the specified name is in the PropertySpecCollection.
        /// </summary>
        /// <param name="name">The name of the PropertySpec to locate in the PropertySpecCollection.</param>
        /// <returns>true if item is found in the PropertySpecCollection; otherwise, false.</returns>
        public bool Contains(string name)
        {
            foreach (var spec in innerList)
                if (spec.Name == name)
                    return true;

            return false;
        }

        /// <summary>
        /// Copies the entire PropertySpecCollection to A compatible one-dimensional Array, starting at the
        /// beginning of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied
        /// from PropertySpecCollection. The Array must have zero-based indexing.</param>
        public void CopyTo(PropertySpec[] array)
        {
            innerList.CopyTo(array, 0);
        }

        /// <summary>
        /// Copies the PropertySpecCollection or A portion of it to A one-dimensional array.
        /// </summary>
        /// <param name="array">The one-dimensional Array that is the destination of the elements copied
        /// from the collection.</param>
        /// <param name="index">The zero-based index in array at which copying begins.</param>
        public void CopyTo(PropertySpec[] array, int index)
        {
            innerList.CopyTo(array, index);
        }

        /// <summary>
        /// Returns an enumerator that can iterate through the PropertySpecCollection.
        /// </summary>
        /// <returns>An IEnumerator for the entire PropertySpecCollection.</returns>
        public IEnumerator GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        /// <summary>
        /// Searches for the specified PropertySpec and returns the zero-based index of the first
        /// occurrence within the entire PropertySpecCollection.
        /// </summary>
        /// <param name="value">The PropertySpec to locate in the PropertySpecCollection.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire PropertySpecCollection,
        /// if found; otherwise, -1.</returns>
        public int IndexOf(PropertySpec value)
        {
            return innerList.IndexOf(value);
        }

        /// <summary>
        /// Searches for the PropertySpec with the specified name and returns the zero-based index of
        /// the first occurrence within the entire PropertySpecCollection.
        /// </summary>
        /// <param name="name">The name of the PropertySpec to locate in the PropertySpecCollection.</param>
        /// <returns>The zero-based index of the first occurrence of value within the entire PropertySpecCollection,
        /// if found; otherwise, -1.</returns>
        public int IndexOf(string name)
        {
            var i = 0;

            foreach (var spec in innerList)
            {
                if (spec.Name == name)
                    return i;

                i++;
            }

            return -1;
        }

        /// <summary>
        /// Inserts A PropertySpec object into the PropertySpecCollection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which value should be inserted.</param>
        /// <param name="value">The PropertySpec to insert.</param>
        public void Insert(int index, PropertySpec value)
        {
            innerList.Insert(index, value);
        }

        /// <summary>
        /// Removes the first occurrence of A specific object from the PropertySpecCollection.
        /// </summary>
        /// <param name="obj">The PropertySpec to remove from the PropertySpecCollection.</param>
        public void Remove(PropertySpec obj)
        {
            innerList.Remove(obj);
        }

        /// <summary>
        /// Removes the property with the specified name from the PropertySpecCollection.
        /// </summary>
        /// <param name="name">The name of the PropertySpec to remove from the PropertySpecCollection.</param>
        public void Remove(string name)
        {
            var index = IndexOf(name);
            RemoveAt(index);
        }

        /// <summary>
        /// Removes the object at the specified index of the PropertySpecCollection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        /// <summary>
        /// Copies the elements of the PropertySpecCollection to A new PropertySpec array.
        /// </summary>
        /// <returns>A PropertySpec array containing copies of the elements of the PropertySpecCollection.</returns>
        public PropertySpec[] ToArray()
        {
            var ar = new ArrayList();
            foreach (var item in innerList)
            {
                ar.Add(item);
            }

            return (PropertySpec[])ar.ToArray(typeof(PropertySpec));
            //return (PropertySpec[])InnerList..ToArray(); //.ToArray(typeof(PropertySpec));
        }
        #endregion

        #region Explicit interface implementations for ICollection and IList
        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((PropertySpec[])array, index);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        int IList.Add(object value)
        {
            var result = 0;

            Add((PropertySpec)value);
            result = IndexOf((PropertySpec)value);

            return result;
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        bool IList.Contains(object obj)
        {
            return Contains((PropertySpec)obj);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (PropertySpec)value;
            }
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        int IList.IndexOf(object obj)
        {
            return IndexOf((PropertySpec)obj);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        void IList.Insert(int index, object value)
        {
            Insert(index, (PropertySpec)value);
        }

        /// <summary>
        /// This member supports the .NET Framework infrastructure and is not intended to be used directly from your code.
        /// </summary>
        void IList.Remove(object value)
        {
            Remove((PropertySpec)value);
        }
        #endregion

        int IList<PropertySpec>.IndexOf(PropertySpec item)
        {
            return IndexOf(item);
        }

        void IList<PropertySpec>.Insert(int index, PropertySpec item)
        {
            Insert(index, item);
        }

        void IList<PropertySpec>.RemoveAt(int index)
        {
            innerList.RemoveAt(index);
        }

        PropertySpec IList<PropertySpec>.this[int index]
        {
            get
            {
                return innerList[index];
            }
            set
            {
                innerList[index] = value;
            }
        }

        void ICollection<PropertySpec>.Add(PropertySpec item)
        {
            innerList.Add(item);
        }

        void ICollection<PropertySpec>.Clear()
        {
            innerList.Clear();
        }

        bool ICollection<PropertySpec>.Contains(PropertySpec item)
        {
            return innerList.Contains(item);
        }

        void ICollection<PropertySpec>.CopyTo(PropertySpec[] array, int arrayIndex)
        {
            innerList.CopyTo(array, arrayIndex);
        }

        int ICollection<PropertySpec>.Count
        {
            get { return innerList.Count; }
        }

        bool ICollection<PropertySpec>.IsReadOnly
        {
            get { return innerList.IsReadOnly; }
        }

        bool ICollection<PropertySpec>.Remove(PropertySpec item)
        {
            return innerList.Remove(item);
        }

        IEnumerator<PropertySpec> IEnumerable<PropertySpec>.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return innerList.GetEnumerator();
        }
    }
    #endregion

    //public class PropertySpecCollectionType : NHibernate.UserTypes.IUserCollectionType
    //{
    //    bool NHibernate.UserTypes.IUserCollectionType.Contains(object collection, object entity)
    //    {
    //        if (entity is PropertySpec)
    //            return ((IList<PropertySpec>)collection).Contains(entity as PropertySpec);
    //        else
    //            return false;
    //    }

    //    IEnumerable NHibernate.UserTypes.IUserCollectionType.GetElements(object collection)
    //    {
    //        return (IEnumerable)collection;
    //    }

    //    object NHibernate.UserTypes.IUserCollectionType.IndexOf(object collection, object entity)
    //    {
    //        return -1;
    //    }

    //    object NHibernate.UserTypes.IUserCollectionType.Instantiate(int anticipatedSize)
    //    {
    //        return new PropertySpecCollection();
    //    }

    //    NHibernate.Collection.IPersistentCollection NHibernate.UserTypes.IUserCollectionType.Instantiate(NHibernate.Engine.ISessionImplementor session, NHibernate.Persister.Collection.ICollectionPersister persister)
    //    {
    //        return new PersitentPropertySpecCollection(session);
    //    }

    //    object NHibernate.UserTypes.IUserCollectionType.ReplaceElements(object original, object target, NHibernate.Persister.Collection.ICollectionPersister persister, object owner, IDictionary copyCache, NHibernate.Engine.ISessionImplementor session)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    NHibernate.Collection.IPersistentCollection NHibernate.UserTypes.IUserCollectionType.Wrap(NHibernate.Engine.ISessionImplementor session, object collection)
    //    {
    //        PersitentPropertySpecCollection coll = new PersitentPropertySpecCollection(session, collection as PropertySpecCollection);
    //        return coll;
    //    }
    //}

    //public class PersitentPropertySpecCollection : NHibernate.Collection.PersistentList
    //{
    //    public PersitentPropertySpecCollection(NHibernate.Engine.ISessionImplementor session)
    //        : base(session)
    //    {
    //    }

    //    public PersitentPropertySpecCollection(NHibernate.Engine.ISessionImplementor session, PropertySpecCollection collection)
    //        : base(session)
    //    {
    //    }
    //}

    /// <summary>
    /// Represents A collection of custom properties that can be selected into A
    /// PropertyGrid to provide functionality beyond that of the simple reflection
    /// normally used to query an object's properties.
    /// </summary>
    public class PropertyBag : ICustomTypeDescriptor
    {

        #region PropertySpecDescriptor class definition
        private class PropertySpecDescriptor : PropertyDescriptor
        {
            private PropertyBag bag;
            private PropertySpec item;

            public PropertySpecDescriptor(PropertySpec item, PropertyBag bag, string name, Attribute[] attrs) :
                base(name, attrs)
            {
                this.bag = bag;
                this.item = item;
            }

            public override Type ComponentType
            {
                get { return item.GetType(); }
            }

            public override bool IsReadOnly
            {
                get { return (Attributes.Matches(ReadOnlyAttribute.Yes)); }
            }

            public override Type PropertyType
            {
                get { return Type.GetType(item.TypeName); }
            }

            public override bool CanResetValue(object component)
            {
                if (item.DefaultValue == null)
                    return false;
                return !GetValue(component).Equals(item.DefaultValue);
            }

            public override object GetValue(object component)
            {
                // Have the property bag raise an event to get the current value
                // of the property.

                var e = new PropertySpecEventArgs(item, null);
                var myBag = (PropertyBag)component;
                if(myBag != null)
                    myBag.OnGetValue(e);
                else
                    bag.OnGetValue(e);

                return e.Value;
            }

            public override void ResetValue(object component)
            {
                SetValue(component, item.DefaultValue);
            }

            public override void SetValue(object component, object value)
            {
                // Have the property bag raise an event to set the current value
                // of the property.

                var e = new PropertySpecEventArgs(item, value);

                var myBag = (PropertyBag)component;
                if (myBag != null)
                    myBag.OnSetValue(e);
                else
                    bag.OnSetValue(e);
            }

            public override bool ShouldSerializeValue(object component)
            {
                var val = GetValue(component);

                if (item.DefaultValue == null && val == null)
                    return false;
                return !val.Equals(item.DefaultValue);
            }
        }
        #endregion

        private string defaultProperty;
        private PropertySpecCollection properties;

        /// <summary>
        /// Initializes A new instance of the PropertyBag class.
        /// </summary>
        public PropertyBag()
        {
            defaultProperty = null;
            properties = new PropertySpecCollection();
        }

        /// <summary>
        /// Gets or sets the name of the default property in the collection.
        /// </summary>
        public string DefaultProperty
        {
            get { return defaultProperty; }
            set { defaultProperty = value; }
        }

        /// <summary>
        /// Gets the collection of properties contained within this PropertyBag.
        /// </summary>
        public PropertySpecCollection Properties
        {
            get { return properties; }
            set { properties = value; }
        }

        /// <summary>
        /// Occurs when A PropertyGrid requests the value of A property.
        /// </summary>
        public event PropertySpecEventHandler GetValue;

        /// <summary>
        /// Occurs when the user changes the value of A property in A PropertyGrid.
        /// </summary>
        public event PropertySpecEventHandler SetValue;

        /// <summary>
        /// Raises the GetValue event.
        /// </summary>
        /// <param name="e">A PropertySpecEventArgs that contains the event data.</param>
        protected virtual void OnGetValue(PropertySpecEventArgs e)
        {
            if (GetValue != null)
                GetValue(this, e);
        }

        /// <summary>
        /// Raises the SetValue event.
        /// </summary>
        /// <param name="e">A PropertySpecEventArgs that contains the event data.</param>
        protected virtual void OnSetValue(PropertySpecEventArgs e)
        {
            if (SetValue != null)
                SetValue(this, e);
        }

        #region ICustomTypeDescriptor explicit interface definitions
        // Most of the functions required by the ICustomTypeDescriptor are
        // merely passed on to the default TypeDescriptor for this type,
        // which will do something appropriate.  The exceptions are noted
        // below.
        AttributeCollection ICustomTypeDescriptor.GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }


        string _classname;
        public string ClassName
        {
            get { return _classname; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    _classname = value;
            }
        }

        string _modulename;
        public string ModuleName
        {
            get { return _modulename; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                    _modulename = value;
            }
        }

        public override string ToString()
        {
            if (!String.IsNullOrEmpty(_classname))
                return _classname;
            return base.ToString();
        }



        string ICustomTypeDescriptor.GetClassName()
        {
            string result = null;

            if (!String.IsNullOrEmpty(_classname))
                result = ClassName;
            else
                result = TypeDescriptor.GetClassName(this, true);

            return result;
        }

        string ICustomTypeDescriptor.GetComponentName()
        {
            string result = null;

            if (!String.IsNullOrEmpty(_modulename))
                result = ModuleName;
            else
                result = TypeDescriptor.GetClassName(this, true);

            return result;
        }


        TypeConverter ICustomTypeDescriptor.GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
        {
            // This function searches the property list for the property
            // with the same name as the DefaultProperty specified, and
            // returns A property descriptor for it.  If no property is
            // found that matches DefaultProperty, A null reference is
            // returned instead.

            PropertySpec propertySpec = null;
            if (defaultProperty != null)
            {
                var index = properties.IndexOf(defaultProperty);
                propertySpec = properties[index];
            }

            if (propertySpec != null)
                return new PropertySpecDescriptor(propertySpec, this, propertySpec.Name, null);
            return null;
        }

        object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
        {
            return ((ICustomTypeDescriptor)this).GetProperties(new Attribute[0]);
        }

        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
        {
            // Rather than passing this function on to the default TypeDescriptor,
            // which would return the actual properties of PropertyBag, I construct
            // A list here that contains property descriptors for the elements of the
            // Properties list in the bag.

            var props = new ArrayList();

            foreach (PropertySpec property in properties)
            {
                var attrs = new ArrayList();

                // If A category, description, editor, or type converter are specified
                // in the PropertySpec, create attributes to define that relationship.
                if (property.Category != null)
                    attrs.Add(new CategoryAttribute(property.Category));

                if (property.Description != null)
                    attrs.Add(new DescriptionAttribute(property.Description));

                if (property.EditorTypeName != null)
                    attrs.Add(new EditorAttribute(property.EditorTypeName, typeof(UITypeEditor)));

                if (property.ConverterTypeName != null)
                    attrs.Add(new TypeConverterAttribute(property.ConverterTypeName));

                // adding the readonly attribute
                attrs.Add(new ReadOnlyAttribute(property.ReadOnlyProperty));

                // adding the browsable attribute, to support hidden properties - TH 20140723
                attrs.Add(new BrowsableAttribute(property.BrowsableProperty));

                // Additionally, append the custom attributes associated with the
                // PropertySpec, if any.
                //if (property.Attributes != null)
                //    attrs.AddRange(property.Attributes);

                var attrArray = (Attribute[])attrs.ToArray(typeof(Attribute));

                // Create A new property descriptor for the property item, and add
                // it to the list.
                var pd = new PropertySpecDescriptor(property,
                    this, property.Name, attrArray);
                props.Add(pd);
            }

            // Convert the list of PropertyDescriptors to A collection that the
            // ICustomTypeDescriptor can use, and return it.
            var propArray = (PropertyDescriptor[])props.ToArray(
                typeof(PropertyDescriptor));
            return new PropertyDescriptorCollection(propArray);
        }

        object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion
    }

    /// <summary>
    /// An extension of PropertyBag that manages A table of property values, in
    /// addition to firing events when property values are requested or set.
    /// </summary>
    public class PropertyRow : PropertyBag
    {
        private IDictionary<string, object> m_propValues;

        public IDictionary<string, object> PropertyValues
        {
            get { return m_propValues; }
            set { m_propValues = value; }
        }

        public IList<PropertySpec> MyInnerPropertyList
        {
            get { return Properties.InnerList; }
            set { Properties.InnerList = value; }
        }

        /// <summary>
        /// Initializes A new instance of the PropertyTable class.
        /// </summary>
        public PropertyRow()
        {
            m_propValues = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets or sets the value of the property with the specified name.
        /// <p>In C#, this property is the indexer of the PropertyTable class.</p>
        /// </summary>
        public object this[string key]
        {
            get { return m_propValues[key]; }
            set { m_propValues[key] = value; }
        }

        /// <summary>
        /// This member overrides PropertyBag.OnGetValue.
        /// </summary>
        protected override void OnGetValue(PropertySpecEventArgs e)
        {
            e.Value = m_propValues[e.Property.Name];
            base.OnGetValue(e);
        }

        /// <summary>
        /// This member overrides PropertyBag.OnSetValue.
        /// </summary>
        protected override void OnSetValue(PropertySpecEventArgs e)
        {
            m_propValues[e.Property.Name] = e.Value;
            base.OnSetValue(e);
        }
    }
}
