using System.IO;

namespace SDBees.Utils
{
    public static class DirectoryTools
    {
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            var dir = new DirectoryInfo(sourceDirName);
            var dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory doesn't exist, create it. 
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (var subdir in dirs)
                {
                    var temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static string GetTempDir()
        {
            var temppath = Path.GetTempPath();
            var path = Path.Combine(temppath, "SDBees");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public static string GetRootedFileName(string fileName)
        {
            string result = null;

            string path = null;

            path = Path.IsPathRooted(fileName) ? Path.GetDirectoryName(fileName) : Path.Combine(GetTempDir(), Path.GetDirectoryName(fileName));

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            result = Path.Combine(path, Path.GetFileName(fileName));

            return result;
        }
    }
}
