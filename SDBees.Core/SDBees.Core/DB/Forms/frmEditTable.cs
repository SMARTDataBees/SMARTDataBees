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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SDBees.Core.Utils;

namespace SDBees.DB
{
    public partial class frmEditTable : Form
    {
        private Table mOrigTable;
        private Table mTable;
        private SDBeesDBConnection m_dbManager;

        /// <summary>
        /// Table this Form edits
        /// </summary>
        public Table Table
        {
            get { return mOrigTable; }
            set { mOrigTable = value; }
        }

        /// <summary>
        /// Database this table is in
        /// </summary>
        public Database Database
        {
            get { return m_dbManager.Database; }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public frmEditTable(SDBeesDBConnection dbManager)
        {
            m_dbManager = dbManager;

            InitializeComponent();
        }

        private void frmEditTable_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                SetFromObject();

                updateProperties();
                EnableControls();
            }
        }

        private void SetFromObject()
        {
            var xml = Serializer.ToXml(mOrigTable);
            if (xml != null)
                mTable = Serializer.FromXml(xml);
            updateControls();
        }

        private void SetToObject()
        {
            // Update the original table...
            var xml = Serializer.ToXml(mTable);
            mOrigTable = Serializer.FromXml(xml);
        }

        private void EnableControls()
        {
            var bColumnSelected = lbColumns.SelectedIndex >= 0;
            bnDeleteColumn.Enabled = bColumnSelected;
        }

        private void lbColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
        	// Vor dem Wechseln noch die Standardwerte prüfen ...
        	
            updateProperties();
            EnableControls();
        }

        private void updateControls()
        {
            lbColumns.Items.Clear();
            foreach (var column in mTable.Columns)
            {
                lbColumns.Items.Add(column.DisplayName);
            }
        }

        private Column selectedColumn()
        {
            Column result = null;

            var index = lbColumns.SelectedIndex;
            if (index >= 0)
            {
                var columnDisplayName = lbColumns.Items[index].ToString();
                foreach (var column in mTable.Columns)
                {
                   
                    if (column.DisplayName == columnDisplayName)
                    {
                        result = column;
                        break;
                    }
                }
            }

            return result;
        }

        internal void updateProperties()
        {
            var column = selectedColumn();

            if (column != null)
            {
                var propertyTable = new ColumnPropertyRow(column, this);

                pgProperties.SelectedObject = propertyTable;
            }
            else
            {
                pgProperties.SelectedObject = null;
            }
        }

        internal void ColumnNameChanged(string oldName, string newName)
        {
            var index = lbColumns.Items.IndexOf(oldName);
            lbColumns.Items[index] = newName;
        }

        private void bnAddColumn_Click(object sender, EventArgs e)
        {
            string columnName;
            var displayName = "";
            bool nameInUse;
            var index = 1;
            do
            {
                columnName = "Eigenschaft" + index;
                var column = mTable.Columns.FirstOrDefault(clmn => clmn.Name.Equals(columnName));
                nameInUse = column != null;    
            } while (nameInUse);

            var location = MousePosition;

            do
            {
                var dlgres = DialogResult.Abort;
                columnName = InputBox.Show("Spaltenname", "Name für neue Spalte", columnName, location.X, location.Y, ref dlgres);
                columnName = columnName.Trim();
                if (columnName != "")
                {
                    displayName = columnName;
                    columnName = Database.MakeValidColumnName(columnName);

                    nameInUse = false;

                    foreach (var iterator in mTable.Columns)
                    {
                      
                        if (string.Compare(iterator.Name, columnName, true) == 0)
                        {
                            nameInUse = true;
                            break;
                        }
                    }

                    if (nameInUse)
                    {
                        MessageBox.Show("Die Tabelle besitzt bereits eine Spalte mit diesem Namen!", "Spaltenname nicht zulässig", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

            } while (nameInUse && (columnName != ""));

            if (columnName != "")
            {
                var column = new Column(columnName, DbType.String, displayName, "", "", 50, "", 0);
                mTable.Columns.Add(column);
                index = lbColumns.Items.Add(displayName);
                lbColumns.SelectedIndex = index;
            }
        }

        private void bnDeleteColumn_Click(object sender, EventArgs e)
        {
            var column = selectedColumn();

            if (column != null)
            {
                if (mTable.PrimaryKey == column.Name)
                {
                    MessageBox.Show("Primärspalte kann nicht gelöscht werden!");
                }
                else
                {
                    var index = lbColumns.SelectedIndex;

                    mTable.Columns.Remove(column);
                    lbColumns.Items.RemoveAt(lbColumns.SelectedIndex);

                    // Auswahl auf einen sinnvollen Eintrag...
                    if (index >= lbColumns.Items.Count)
                    {
                        index = lbColumns.Items.Count - 1;
                    }
                    lbColumns.SelectedIndex = index;
                }
            }
        }

        private void bnOk_Click(object sender, EventArgs e)
        {
            SetToObject();

            DialogResult = DialogResult.OK;

            Close();
        }

        private void bnExport_Click(object sender, EventArgs e)
        {
            var xml = Serializer.ToXml(mTable);

            var dlg = new SaveFileDialog();
            dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using (var sw = new StreamWriter(dlg.FileName))
                {
                    // write the XML to the file...
                    sw.Write(xml);
                }
            }
        }

        private void bnImport_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using (var sr = new StreamReader(dlg.FileName))
                {
                    // write the XML to the file...
                    var xmlSchema = sr.ReadToEnd();
                    mTable = Serializer.FromXml(xmlSchema);

                    updateControls();
                }
            }
        }
    }
}
