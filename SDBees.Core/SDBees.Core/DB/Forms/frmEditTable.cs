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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using SDBees.Plugs.Properties;

namespace SDBees.DB
{
    public partial class frmEditTable : Form
    {
        private Table mOrigTable;
        private Table mTable;
        private SDBees.DB.SDBeesDBConnection m_dbManager;

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
        public frmEditTable(SDBees.DB.SDBeesDBConnection dbManager)
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
            string xmlDefinition = mOrigTable.writeXml();
            mTable = new Table();
            mTable.readXml(xmlDefinition);

            updateControls();
        }

        private void SetToObject()
        {
            // Update the original table...
            string xmlDefinition = mTable.writeXml();
            mOrigTable.readXml(xmlDefinition);
        }

        private void EnableControls()
        {
            bool bColumnSelected = lbColumns.SelectedIndex >= 0;
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
            foreach (KeyValuePair<string, Column> iterator in mTable.Columns)
            {
                Column column = iterator.Value;
                lbColumns.Items.Add(column.DisplayName);
            }
        }

        private Column selectedColumn()
        {
            Column result = null;

            int index = lbColumns.SelectedIndex;
            if (index >= 0)
            {
                string columnDisplayName = lbColumns.Items[index].ToString();
                foreach (KeyValuePair<string, Column> iterator in mTable.Columns)
                {
                    Column column = iterator.Value;
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
            Column column = selectedColumn();

            if (column != null)
            {
                ColumnPropertyRow propertyTable = new ColumnPropertyRow(column, this);

                pgProperties.SelectedObject = propertyTable;
            }
            else
            {
                pgProperties.SelectedObject = null;
            }
        }

        internal void ColumnNameChanged(string oldName, string newName)
        {
            int index = lbColumns.Items.IndexOf(oldName);
            lbColumns.Items[index] = newName;
        }

        private void bnAddColumn_Click(object sender, EventArgs e)
        {
            string columnName = "";
            string displayName = "";
            bool nameInUse = false;
            int index = 1;
            do
            {
                columnName = "Eigenschaft" + index.ToString();
                nameInUse = mTable.Columns.ContainsKey(columnName);
                index++;
            } while (nameInUse);

            System.Drawing.Point location = Control.MousePosition;

            do
            {
                System.Windows.Forms.DialogResult dlgres = DialogResult.Abort;
                columnName = InputBox.Show("Spaltenname", "Name für neue Spalte", columnName, location.X, location.Y, ref dlgres);
                columnName = columnName.Trim();
                if (columnName != "")
                {
                    displayName = columnName;
                    columnName = Database.MakeValidColumnName(columnName);

                    nameInUse = false;

                    foreach (KeyValuePair<string, Column> iterator in mTable.Columns)
                    {
                        Column existingColumn = iterator.Value;
                        if (string.Compare(existingColumn.Name, columnName, true) == 0)
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
                Column column = new Column(columnName, DbType.eString, displayName, "", "", 50, "", 0);
                mTable.Columns.Add(column);
                index = lbColumns.Items.Add(displayName);
                lbColumns.SelectedIndex = index;
            }
        }

        private void bnDeleteColumn_Click(object sender, EventArgs e)
        {
            Column column = selectedColumn();

            if (column != null)
            {
                if (mTable.PrimaryKey == column.Name)
                {
                    MessageBox.Show("Primärspalte kann nicht gelöscht werden!");
                }
                else
                {
                    int index = lbColumns.SelectedIndex;

                    mTable.Columns.Remove(column.Name);
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
            string xmlSchema = mTable.writeXml();

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(dlg.FileName))
                {
                    // write the XML to the file...
                    sw.Write(xmlSchema);
                }
            }
        }

        private void bnImport_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(dlg.FileName))
                {
                    // write the XML to the file...
                    string xmlSchema = sr.ReadToEnd();
                    mTable.readXml(xmlSchema);

                    updateControls();
                }
            }
        }
    }
}
