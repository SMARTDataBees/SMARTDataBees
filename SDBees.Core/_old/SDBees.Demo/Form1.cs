using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace SDBees.Demo
{
    public partial class Form1 : Form
    {
        bool _mySQLstarted = true;
        bool _sdBeesStarted = false;

        public Form1()
        {
            InitializeComponent();
        }

        System.Diagnostics.Process _pMySQL = null;

        private void _buttonMySQLStart_Click(object sender, EventArgs e)
        {
            System.Diagnostics.ProcessStartInfo pInf = new System.Diagnostics.ProcessStartInfo();
            System.IO.DirectoryInfo dirinf = new System.IO.DirectoryInfo(Path.GetDirectoryName(this.GetType().Assembly.Location));
            //pInf.Arguments = String.Format(" --defaults-file=\"{0}\"", Path.Combine(dirinf.FullName, "sdbees_mysqldemo\\my-small.ini"));
            pInf.FileName = Path.Combine(dirinf.FullName, "sdbees_mysqldemo\\bin\\mysqld.exe");

            _pMySQL = new System.Diagnostics.Process();
            _pMySQL.StartInfo = pInf;
            _pMySQL.Start();

            _mySQLstarted = true;
        }


        System.Diagnostics.Process _pSDBees = null;

        private void _buttonSDBeesStart_Click(object sender, EventArgs e)
        {
            if(_mySQLstarted)
            {
                System.Diagnostics.ProcessStartInfo pInf = new System.Diagnostics.ProcessStartInfo();
                System.IO.DirectoryInfo dirinf = new System.IO.DirectoryInfo(Path.GetDirectoryName(this.GetType().Assembly.Location));
                pInf.FileName = Path.Combine(dirinf.FullName, "SDBees.exe");
                _pSDBees = new System.Diagnostics.Process();
                _pSDBees.StartInfo = pInf;

                _pSDBees.Start();
                _pSDBees.WaitForExit();
            }
        }
    }
}
