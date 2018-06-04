using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SDBees.Core.DB.SQLite
{
    internal class SQLiteFilenameEditor : System.Windows.Forms.Design.FileNameEditor
    {
        public SQLiteFilenameEditor() : base()
        { }

        protected override void InitializeDialog(System.Windows.Forms.OpenFileDialog openFileDialog)
        {
            openFileDialog = new System.Windows.Forms.OpenFileDialog();

            openFileDialog.Title = "Select Databasefile";
            openFileDialog.AddExtension = true;
            openFileDialog.AutoUpgradeEnabled = true;
            openFileDialog.Filter = "Database files (*.s3db)|*.s3db|All files (*.*)|*.* ";
            openFileDialog.DefaultExt = "s3db";
            openFileDialog.CheckPathExists = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.FileName = "*.s3db";
            openFileDialog.Multiselect = false;

            base.InitializeDialog(openFileDialog);
        }

        // public override 
    }
}
