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
using System.Windows.Forms;
using SDBees.DB;

namespace SDBees.UserAdmin
{
    internal partial class SecurityRightDLG : Form
    {
        private Server mServer;
        private AccessRights mAccessRights;

        public AccessRights AccessRights
        {
            get { return mAccessRights; }
            set { mAccessRights = value; }
        }

        public SecurityRightDLG(Server server)
        {
            mServer = server;
            InitializeComponent();
            mAccessRights = null;
        }

        private void SetFromObject()
        {
            if (mAccessRights != null)
            {
                cbSystemAdministrator.Checked = false;
                cbDatabaseAdministrator.Checked = false;

                var typeIndex = (int)mAccessRights.Type;
                puSecurityType.SelectedIndex = typeIndex;

                if (mAccessRights.Type == AccessType.Database)
                {
                    cbSystemAdministrator.Checked = (mAccessRights.AllowedFlags == AccessFlags.All);
                }
                else if (mAccessRights.Type == AccessType.Database)
                {
                    puDatabase.SelectedIndex = puDatabase.FindStringExact(mAccessRights.Name);
                    cbDatabaseAdministrator.Checked = (mAccessRights.AllowedFlags == AccessFlags.All);
                }
            }
        }

        private void SetToObject()
        {
            if (mAccessRights != null)
            {
                var typeIndex = puSecurityType.SelectedIndex;
                var isAdministrator = false;
                if (typeIndex == 0)
                {
                    mAccessRights.Type = AccessType.Server;
                    isAdministrator = cbSystemAdministrator.Checked;
                    mAccessRights.Name = mServer.Name;
                }
                else if (typeIndex == 1)
                {
                    mAccessRights.Type = AccessType.Database;
                    isAdministrator = cbDatabaseAdministrator.Checked;
                    mAccessRights.Name = puDatabase.Items[puDatabase.SelectedIndex].ToString();
                }
                else if (typeIndex == 2)
                {
                    mAccessRights.Type = AccessType.Table;
                }
                else if (typeIndex == 3)
                {
                    mAccessRights.Type = AccessType.Table;
                }

                if (isAdministrator)
                {
                    mAccessRights.AllowedFlags = AccessFlags.All;
                    mAccessRights.DeniedFlags = 0;
                }
                else
                {
                    mAccessRights.AllowedFlags = 0;
                    mAccessRights.DeniedFlags = 0;
                }
            }
        }

        private void SecurityRightDLG_Load(object sender, EventArgs e)
        {
            FillControls();

            SetFromObject();

            EnableControls();
        }

        private void FillControls()
        {
            // Type Combobox...
            puSecurityType.Items.Clear();
            puSecurityType.Items.Add("System Rechte");
            puSecurityType.Items.Add("Datenbank Rechte");
            puSecurityType.Items.Add("Tabellen Rechte");
            puSecurityType.Items.Add("Spalten Rechte");

            puSecurityType.SelectedIndex = 1;

            // Database Combobox
            if (mServer != null)
            {
                puDatabase.Items.Clear();

                ArrayList databaseNames = null;
                Error error = null;
                var numDatabases = mServer.GetDatabases(ref databaseNames, true, ref error);

                Error.Display("Failed to get Databases from Server", error);

                for (var index = 0; index < numDatabases; index++)
                {
                    puDatabase.Items.Add((string)databaseNames[index]);
                }
            }
        }

        private void FillTableControl()
        {
            puTable.Items.Clear();

            if ((puSecurityType.SelectedIndex >= 1) && (puDatabase.SelectedIndex >= 0))
            {
                puTable.Items.Add("** Alle Tabellen **");

                var databaseName = puDatabase.Items[puDatabase.SelectedIndex].ToString();
                var database = mServer.GetDatabase(databaseName);

                Error error = null;
                ArrayList tableNames = null;
                if (database.TableNames(ref tableNames, ref error) > 0)
                {
                    foreach (string tableName in tableNames)
                    {
                        puTable.Items.Add(tableName);
                    }
                }

                Error.Display("Cannot get Tables for database!", error);
            }
        }

        private void FillColumnControl()
        {
            puColumn.Items.Clear();

            if ((puSecurityType.SelectedIndex >= 1) && (puDatabase.SelectedIndex >= 0) && (puTable.SelectedIndex >= 0))
            {
                var databaseName = puDatabase.Items[puDatabase.SelectedIndex].ToString();
                var tableName = puTable.Items[puTable.SelectedIndex].ToString();

                puColumn.Items.Add("** Alle Spalten **");

                var database = mServer.GetDatabase(databaseName);

                // TBD: ...
                Error error = null;

                Error.Display("Cannot get column for database table!", error);
            }
        }

        private void EnableControls()
        {
            var isSystemRight = (puSecurityType.SelectedIndex == 0);
            var isDatabaseRight = (puSecurityType.SelectedIndex == 1);
            var isTableRight = (puSecurityType.SelectedIndex == 2);
            var isColumnRight = (puSecurityType.SelectedIndex == 3);

            cbSystemAdministrator.Enabled = isSystemRight;

            puDatabase.Enabled = isDatabaseRight || isTableRight || isColumnRight;
            cbDatabaseAdministrator.Enabled = isDatabaseRight;

            puTable.Enabled = isTableRight || isColumnRight;
            cbTableAdministrator.Enabled = isTableRight;

            puColumn.Enabled = isColumnRight;
        }

        private void puSecurityType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((puSecurityType.SelectedIndex == 1) && (puDatabase.Items.Count > 0) && (puDatabase.SelectedIndex < 0))
            {
                puDatabase.SelectedIndex = 0;
            }
            EnableControls();
        }

        private void bnOk_Click(object sender, EventArgs e)
        {
            SetToObject();

            DialogResult = DialogResult.OK;

            Close();
        }

        private void puDatabase_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillTableControl();

            if ((puDatabase.SelectedIndex >= 0) && (puTable.Items.Count > 0) && (puTable.SelectedIndex < 0))
            {
                puTable.SelectedIndex = 0;
            }

            EnableControls();
        }

        private void puTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillColumnControl();

            if ((puTable.SelectedIndex >= 0) && (puColumn.Items.Count > 0) && (puColumn.SelectedIndex < 0))
            {
                puColumn.SelectedIndex = 0;
            }

            EnableControls();
        }
    }
}
