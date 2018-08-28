using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SDBees.Core.DB.SQLite
{
    internal class SQLiteFilenameEditor : FileNameEditor
    {
        protected override void InitializeDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog = new OpenFileDialog();

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
