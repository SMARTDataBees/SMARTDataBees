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
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using SDBees.Main.Window;

namespace SDBees.GuiTools
{
    /// <summary>
    /// Form to display Application and license information
    /// </summary>
    public partial class AboutBox : Form
    {
        #region Private Data Members

        private Assembly m_assembly;

        #endregion

        #region Public Properties

        #endregion

        #region Constructor/Destructor

        /// <summary>
        /// Standard constructor
        /// </summary>
        public AboutBox(Assembly assembly)
        {
            m_assembly = assembly;

            InitializeComponent();

            Icon = MainWindowApplication.Current.GetApplicationIcon();

            SetUpDisplayment();
        }


        #endregion

        #region Public Methods

        #endregion

        #region private methods
        private void SetUpDisplayment()
        {
            _AssemblyInformation.Dock = DockStyle.Fill;

            _panelLinkLabels.Dock = DockStyle.Fill;
        }

        private void FillLicensesInformation()
        {
            var sourceFolder = new DirectoryInfo(Path.GetDirectoryName(GetType().Assembly.Location));
            if(sourceFolder != null)
            {
                var licensefiles = sourceFolder.GetFiles("License*.txt");
                if(licensefiles != null)
                {
                    var coll = new List<Control>();
                    var topmargin = 10;

                    foreach (var file in licensefiles)
                    {
                        var ll = CreateLinkLabel(file, topmargin);
                        ll.AutoSize = true;
                        coll.Add(ll);
                        topmargin += 30;
                    }
                    _panelLinkLabels.Controls.AddRange(coll.ToArray());
               }
            }
        }

        //private string AddLicenseInfo(FileInfo file)
        //{
        //    string name = Path.GetFileNameWithoutExtension(file.Name).Replace('-', ' ');
        //    string filename = file.FullName;
        //    //filename = filename.Replace("\", "/");
        //    string encoded = System.Net.WebUtility.UrlEncode(string.Format("file://{0}", filename));
        //    string temp = String.Format("{0}\n{1}\n\n", name, encoded);
        //    return temp;
        //}

        private LinkLabel CreateLinkLabel(FileInfo file, int toplocation)
        {
            var name = Path.GetFileNameWithoutExtension(file.Name).Replace('-', ' ');
            var newLink = new LinkLabel();
            newLink.Text = name;


            var encoded = WebUtility.UrlEncode(string.Format("file://{0}", file.FullName));
            var lk = new LinkLabel.Link();
            lk.LinkData = encoded;
            lk.Name = name;
            lk.Length = name.Length;

            newLink.Links.Add(lk);

            //position in parent control
            var p = newLink.Location;
            p.X = 15;
            p.Y = toplocation;
            newLink.Location = p;

            //assign to eventhandler
            newLink.LinkClicked += NewLink_LinkClicked;

            return newLink;
        }

        private void NewLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var link = e.Link.LinkData.ToString();
            link = WebUtility.UrlDecode(link);
            var temp = link.Replace("file://", "");
            var p = Process.Start("notepad.exe", temp);
        }

        #endregion

        #region Protected Methods

        protected void FillAssemblyInformation()
        {
            lbApplicationName.Text = m_assembly.FullName;
            lbApplicationLocation.Text = m_assembly.CodeBase;

            var assemblyInfo = m_assembly.FullName;

            assemblyInfo += "\r\nLoaded Assemblies:";

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                assemblyInfo += "\r\n" + assembly.FullName;
            }

            _AssemblyInformation.Text = assemblyInfo;
        }

        #endregion

        #region Events
        private void _licensesRichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            var p = Process.Start("notepad.exe", e.LinkText.Replace("file://", ""));
        }

        private void AboutBox_Load(object sender, EventArgs e)
        {
            FillLicensesInformation();
            FillAssemblyInformation();
        }

        private void bnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}
