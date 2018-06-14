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
using System.Windows.Forms;
using Carbon.Plugins;
using Carbon.Plugins.Attributes;
using SDBees.Core.Global;
using SDBees.DB;
using SDBees.GuiTools;
using SDBees.Main.Window;
using SDBees.Plugs.TemplateMenue;

namespace SDBees.UserAdmin
{
    [PluginName("UserAdmin Plugin")]
    [PluginAuthors("Gamal Kira")]
    [PluginDescription("Plugin for the user management of SmartDataBees")]
    [PluginId("BD505C5A-9F30-4205-8D3E-1D0CBFB14EA8")]
    [PluginManufacturer("CAD-Development")]
    [PluginVersion("1.0.0")]
    [PluginDependency(typeof(SDBeesDBConnection))]
    [PluginDependency(typeof(MainWindowApplication))]
    [PluginDependency(typeof(GlobalManager))]


    public class UserAdmin : TemplateMenue
    {
        private static UserAdmin _theInstance;
        private readonly MenuItem _mnuItem;
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
        {
            _theInstance = this;
            _mnuItem = new MenuItem("User Admin ...");
            _mnuItem.Click += _mnuItemUserManager_Click;
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
                    var mnuChangePassword = new MenuItem("Change Password ...");
                    mnuChangePassword.Click += mnuChangePassword_Click;

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
                var database = MyDBManager.Database;
                RoleDefinitions.InitTableSchema(database);
            }
        }

        #region MyEvents
        void _mnuItemUserManager_Click(object sender, EventArgs e)
        {
            // Prüfen, ob der Anwender genügend Rechte hat...
            Error error = null;
            var loginName = Current.MyDBManager.Database.User;
            var server = Current.MyDBManager.Database.Server;
            var hasGrantPrivilege = server.UserHasGrantPrivileges(loginName, ref error);

            if (!hasGrantPrivilege)
            {
                MessageBox.Show("Sie haben nicht genügend Rechte zur Benutzer Administration!");
            }
            else
            {
                var dlg = new UserAdminDLG(this);
                dlg.ShowDialog();
            }

        }

        void mnuChangePassword_Click(object sender, EventArgs e)
        {
            var dlg = new ChangePasswordDLG();

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var oldPassword = dlg.OldPassword;
                var newPassword = dlg.NewPassword;

                if (oldPassword != newPassword)
                {
                    var database = Current.MyDBManager.Database;
                    var server = database.Server;

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
