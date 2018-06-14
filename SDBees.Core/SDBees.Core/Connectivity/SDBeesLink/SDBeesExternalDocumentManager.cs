using System.IO;
using System.Text;
using SDBees.Utils.ObjectXmlSerializer;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    public static class SDBeesExternalDocumentManager
    {
        private const string Extension = ".sdbeesdoc";

        public static void SDBeesExternalDocumentSave(SDBeesExternalDocument doc, string filename, string pathname = null)
        {
            doc.DocOriginalName = Path.GetFileName(filename);
            ObjectXMLSerializer<SDBeesExternalDocument>.Save(doc, FileNameFinal(filename, pathname), Encoding.Unicode);
        }

        public static SDBeesExternalDocument SDBeesExternalDocumentLoad(string filename, string pathname = null)
        {
            SDBeesExternalDocument externalDoc = null;
            var filenamefinal = FileNameFinal(filename, pathname);
            if (File.Exists(filenamefinal))
            {
                externalDoc = ObjectXMLSerializer<SDBeesExternalDocument>.Load(filenamefinal);
                externalDoc.DocOriginalName = Path.GetFileName(filename);
            }

            return externalDoc;
        }

        public static string FileNameFinal(string filename, string pathname = null)
        {
            var path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path))
            {
                path = pathname;
            }
            var filenamecombined = Path.Combine(path, Path.GetFileName(filename) + Extension);
            return filenamecombined;
        }
    }
}