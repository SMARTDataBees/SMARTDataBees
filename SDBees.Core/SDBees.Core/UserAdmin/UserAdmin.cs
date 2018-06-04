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
/*
 * Created by SharpDevelop.
 * User: th
 * Date: 22.05.2006
 * Time: 09:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;

using Carbon;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;

using SDBees.DB;
using SDBees.Main.Window;
using SDBees.GuiTools;

namespace SDBees.UserAdmin
{
    [PluginName("UserAdmin Plugin")]
    [PluginAuthors("Gamal Kira")]
    [PluginDescription("Plugin for the user management of SmartDataBees")]
    [PluginId("BD505C5A-9F30-4205-8D3E-1D0CBFB14EA8")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBees.DB.SDBeesDBConnection))]
    [PluginDependency(typeof(SDBees.Main.Window.MainWindowApplication))]
    [PluginDependency(typeof(SDBees.Core.Global.GlobalManager))]

    /// <summary>
    /// Klasse für die Userverwaltung
    /// </summary>
    public class UserAdmin : SDBees.Plugs.TemplateMenue.TemplateMenue
    {
        private static UserAdmin _theInstance;
        private MenuItem _mnuItem;
        private PluginContext _context;


        /// <summary>
        /// Returns the one and only UserAdmin Plugin instance.
        /// </summary>
        public static UserAdmin Current
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        public UserAdmin()
            : base()
        {
            _theInstance = this;
            _mnuItem = new MenuItem("User Admin ...");
            this._mnuItem.Click += new EventHandler(_mnuItemUserManager_Click);
        }

        /// <summary>
        /// Occurs when the plugin starts.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Start(PluginContext context, PluginDescriptorEventArgs e)
        {
            try
            {
                Console.WriteLine("Useradmin starts\n");

                _context = context;

                StartMe(context, e);

                InitDatabase();

                //Setting up the menuitem
                if (MyMainWindow != null)
                {
                    MenuItem mnuChangePassword = new MenuItem("Change Password ...");
                    mnuChangePassword.Click += new EventHandler(mnuChangePassword_Click);

                    MyMainWindow.TheDialog.MenueAdmin().MenuItems.Add(mnuChangePassword);
                    MyMainWindow.TheDialog.MenueAdmin().MenuItems.Add(_mnuItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                //throw;
            }
        }

        /// <summary>
        /// Occurs when the plugin stops.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="e"></param>
        protected override void Stop(PluginContext context, PluginDescriptorEventArgs e)
        {
            Console.WriteLine("User Admin stops ...");
        }

        protected override void OnDatabaseChanged(object sender, EventArgs e)
        {
            InitDatabase();
        }

        private void InitDatabase()
        {
            // Das Databaseplugin besorgen
            if (MyDBManager != null)
            {
                // Create the required database tables...
                Database database = MyDBManager.Database;
                RoleDefinitions.InitTableSchema(database);
            }
        }

        #region MyEvents
        void _mnuItemUserManager_Click(object sender, EventArgs e)
        {
            // Prüfen, ob der Anwender genügend Rechte hat...
            Error error = null;
            string loginName = UserAdmin.Current.MyDBManager.Database.User;
            Server server = UserAdmin.Current.MyDBManager.Database.Server;
            bool hasGrantPrivilege = server.UserHasGrantPrivileges(loginName, ref error);

            if (!hasGrantPrivilege)
            {
                MessageBox.Show("Sie haben nicht genügend Rechte zur Benutzer Administration!");
            }
            else
            {
                UserAdminDLG dlg = new UserAdminDLG(this);
                dlg.ShowDialog();
            }

        }

        void mnuChangePassword_Click(object sender, EventArgs e)
        {
            ChangePasswordDLG dlg = new ChangePasswordDLG();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string oldPassword = dlg.OldPassword;
                string newPassword = dlg.NewPassword;

                if (oldPassword != newPassword)
                {
                    Database database = UserAdmin.Current.MyDBManager.Database;
                    Server server = database.Server;

                    Error error = null;
                    if (server.ChangePassword(database.User, oldPassword, newPassword, ref error))
                    {
                        database.Password = newPassword;
                        server.Password = newPassword;

                        MessageBox.Show("Das Passwort wurde erfolgreich geändert!", "Passwort ändern", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        Error.Display("Passwort konnte nicht geändert werden!", error);
                    }

                }
            }
        }

        #endregion
    }
}
