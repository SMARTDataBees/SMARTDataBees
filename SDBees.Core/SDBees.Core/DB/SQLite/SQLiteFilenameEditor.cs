using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace SDBees.Core.DB.SQLite
{
    internal class SQLiteFilenameEditor : FileNameEditor
    {
        protected override void InitializeDialog(OpenFileDialog openFileDialog)
        {
            openFileDialog = new OpenFileDialog
            {
                Title = @"Select database file",
                AddExtension = true,
                AutoUpgradeEnabled = true,
                Filter = @"Database files (*.s3db)|*.s3db|All files (*.*)|*.* ",
                DefaultExt = "s3db",
                CheckPathExists = false,
                CheckFileExists = false,
                FileName = "*.s3db",
                Multiselect = false
            };


            base.InitializeDialog(openFileDialog);
        }

    }
}
