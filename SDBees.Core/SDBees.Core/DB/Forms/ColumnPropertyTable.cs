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
using System.ComponentModel;
using System.Windows.Forms;
using SDBees.Plugs.Properties;

namespace SDBees.DB
{
    internal class ColumnPropertyRow : PropertyRow
    {
        private Column mColumn;
        private frmEditTable mDialog;

        public ColumnPropertyRow(Column column, frmEditTable dlg)
        {
            mColumn = column;
            mDialog = dlg;

            UpdateProperties();
        }

        protected void UpdateProperties()
        {
            PropertySpec ps = null;
            ps = new PropertySpec("Name", typeof(string), "General", "Name der Eigenschaft in der Datenbank (sprachunabhängug)", null);
            ps.Attributes = new System.Attribute[] { ReadOnlyAttribute.Yes };
            if (mColumn.IsSelectionListPossible())
            {
                Properties.Add(new PropertySpec("Liste", typeof(string), 
                    "General", 
                    "Mögliche Werte aus einer Liste, Werte durch Komma getrennt", 
                    null, 
                    "System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", 
                    typeof(StringConverter)));
                this["Liste"] = mColumn.SelectionListString;
            }

            Properties.Add(ps);
            this["Name"] = mColumn.Name;
            Properties.Add(new PropertySpec("Anzeigename", typeof(string), "General", "Name der Eigenschaft für den Anwender (sprachabhängug)", null));
            this["Anzeigename"] = mColumn.DisplayName;
            Properties.Add(new PropertySpec("Beschreibung", typeof(string), "General", "Beschreibung der Eigenschaft für den Anwender", null));
            this["Beschreibung"] = mColumn.Description;
            Properties.Add(new PropertySpec("Kategorie", typeof(string), "General", "Kategorie der Eigenschaft", null));
            this["Kategorie"] = mColumn.Category;
            Properties.Add(new PropertySpec("Typ", typeof(DbType), "Typ", "Typ der Eigenschaft", null));
            this["Typ"] = mColumn.Type;
            if (mColumn.HasCustomSize())
            {
                Properties.Add(new PropertySpec("Größe", typeof(Int32), "Typ", "Größe des Datensatzes der Eigenschaft", null));
                this["Größe"] = mColumn.Size;
            }
            Properties.Add(new PropertySpec("NULL Zulassen", typeof(bool), "Flags", "Eigenschaft lässt den Wert NULL zu", null));
            this["NULL Zulassen"] = ((mColumn.Flags & (int)DbFlags.eAllowNull) != 0);
            Properties.Add(new PropertySpec("Unique", typeof(bool), "Flags", "Wert der Eigenschaft ist in Spalte eindeutig", null));
            this["Unique"] = ((mColumn.Flags & (int)DbFlags.eUnique) != 0);
            Properties.Add(new PropertySpec("Auto Create", typeof(bool), "Flags", "Wert der Eigenschaft wird automatisch berechnet", null));
            this["Auto Create"] = ((mColumn.Flags & (int)DbFlags.eAutoCreate) != 0);
            Properties.Add(new PropertySpec("Auto Increment", typeof(bool), "Flags", "Wert der Eigenschaft wird automatisch hochgezählt", null));
            this["Auto Increment"] = ((mColumn.Flags & (int)DbFlags.eAutoIncrement) != 0);

            Properties.Add(new PropertySpec("Editierbar", typeof(bool), "UserInterface", "Schreibschutz für Eigenschaft", null));
            this["Editierbar"] = mColumn.Editable;
            Properties.Add(new PropertySpec("Sichtbar", typeof(bool), "UserInterface", "Sichtbare Eigenschaft?", null));
            this["Sichtbar"] = mColumn.Browsable;
            Properties.Add(new PropertySpec("UITypeEditor", typeof(string), "UserInterface", "Der Editor für das Userinterface", null));
            this["UITypeEditor"] = mColumn.UITypeEditor;

            Properties.Add(new PropertySpec("UITypeConverter", typeof(string), "UserInterface", "Der Typ für das Userinterface", null));
            this["UITypeConverter"] = mColumn.UITypeConverter;

            if ((mColumn.Flags & (int)DbFlags.eHasDefault) != 0)
            {
                Properties.Add(new PropertySpec("Standard", typeof(string), "Standard Wert", "Wert der Eigenschaft wenn kein Wert vorgegeben", null));
                this["Standard"] = mColumn.Default;
            }
        }

        protected bool ValueIsValid(string name, object value)
        {
            var isValid = true;

            switch (name)
            {
                case "Name":
                    {
                        var strValue = (string)value;
                        if (strValue.IndexOf(" ") >= 0)
                        {
                            isValid = false;
                        }
                    }
                    break;
            }

            return isValid;
        }
        /// <summary>
        /// This member overrides PropertyTable.OnSetValue.
        /// </summary>
        protected override void OnSetValue(PropertySpecEventArgs e)
        {
            // First check if this is allowed...
            if (!ValueIsValid(e.Property.Name, e.Value))
            {
                MessageBox.Show("Wert " + e.Value + " für " + e.Property.Name + " nicht zulässig!");

                // Refresh the property grid
                mDialog.updateProperties();

                return;
            }

            // Remember the old value...
            var oldValue = this[e.Property.Name];

            // Call base class first to update the value...
            base.OnSetValue(e);

            // Change the column value...
            switch (e.Property.Name)
            {
                case "Name":
                    mColumn.Name = (string)this[e.Property.Name];
                    break;

                case "Anzeigename":
                    mColumn.DisplayName = (string)this[e.Property.Name];

                    // We must notify the dialog to change the column name in the list box
                    mDialog.ColumnNameChanged((string)oldValue, mColumn.DisplayName);
                    break;

                case "Beschreibung":
                    mColumn.Description = (string)this[e.Property.Name];
                    break;

                case "Kategorie":
                    mColumn.Category = (string)this[e.Property.Name];
                    break;

                case "Standard":
                    mColumn.Default = (string)this[e.Property.Name];
                    break;

                case "Editierbar":
                    mColumn.Editable = (bool)this[e.Property.Name];
                    break;

                case "Sichtbar":
                    mColumn.Browsable = (bool)this[e.Property.Name];
                    break;

                case "Typ":
                    mColumn.Type = (DbType)this[e.Property.Name];
                    
                    // Den Standard prüfen und gegebenenfalls setzen
                    SetStandardValue((DbType)this[e.Property.Name], ref mColumn);
                    break;

                case "UITypeConverter":
                    mColumn.UITypeConverter = (string)this[e.Property.Name];
                    break;                
                
                case "UITypeEditor":
                    mColumn.UITypeEditor = (string)this[e.Property.Name];
                    break;

                case "Größe":
                    mColumn.Size = (Int32)this[e.Property.Name];
                    break;

                case "Liste":
                    mColumn.SelectionListString = (string)this[e.Property.Name];
                    break;

                case "NULL Zulassen":
                    {
                        var flag = (bool)this[e.Property.Name];
                        if (flag)
                        {
                            mColumn.Flags |= (int)DbFlags.eAllowNull;
                        }
                        else
                        {
                            mColumn.Flags &= ~(int)DbFlags.eAllowNull;
                        }
                    }
                    break;

                case "Unique":
                    {
                        var flag = (bool)this[e.Property.Name];
                        if (flag)
                        {
                            mColumn.Flags |= (int)DbFlags.eUnique;
                        }
                        else
                        {
                            mColumn.Flags &= ~(int)DbFlags.eUnique;
                        }
                    }
                    break;

                case "Auto Create":
                    {
                        var flag = (bool)this[e.Property.Name];
                        if (flag)
                        {
                            mColumn.Flags |= (int)DbFlags.eAutoCreate;
                        }
                        else
                        {
                            mColumn.Flags &= ~(int)DbFlags.eAutoCreate;
                        }
                    }
                    break;

                case "Auto Increment":
                    {
                        var flag = (bool)this[e.Property.Name];
                        if (flag)
                        {
                            mColumn.Flags |= (int)DbFlags.eAutoIncrement;
                        }
                        else
                        {
                            mColumn.Flags &= ~(int)DbFlags.eAutoIncrement;
                        }
                    }
                    break;
            }

            // Evtl. falsch eingestellte Werte jetzt korrigieren...
            mColumn.FixValues();

            // Refresh the property grid
            mDialog.updateProperties();
        }
        
        private void SetStandardValue(DbType dbTyp, ref Column MyColumn)
        {
        	if(MyColumn.Default == "")
        	{
        		switch(dbTyp.ToString())
	        	{
	        		case "eDate":
	        			MyColumn.Default = DateTime.Now.ToShortDateString();
	        			break;
	        			
	        		default:
	        			break;
	        	}
        	}
        }

    }

}
