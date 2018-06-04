using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SDBees.Utils
{
    public static class DirectoryTools
    {
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

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
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        public static string GetTempDir()
        {
            string temppath = System.IO.Path.GetTempPath();
            string path = System.IO.Path.Combine(temppath, "SDBees");
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            return path;
        }

        public static string GetRootedFileName(string fileName)
        {
            string result = null;

            string path = null;

            if (System.IO.Path.IsPathRooted(fileName))
            {
                path = System.IO.Path.GetDirectoryName(fileName);
            }
            else
            {
                path = Path.Combine(SDBees.Utils.DirectoryTools.GetTempDir(), System.IO.Path.GetDirectoryName(fileName));
            }

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            result = Path.Combine(path, Path.GetFileName(fileName));

            return result;
        }
    }
}
