using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using SDBees.Core.Model;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    [XmlRootAttribute("sdbeesexternaldocument", Namespace = "", IsNullable = false)]
    public class SDBeesExternalDocument
    {
        string m_docOriginalName = "";
        [XmlAttribute("docname")]
        public string DocOriginalName
        {
            get { return m_docOriginalName; }
            set { m_docOriginalName = value; }
        }

        string m_ProjectId = Guid.Empty.ToString();
        [XmlAttribute("projectid")]
        public string ProjectId
        {
            get { return m_ProjectId; }
            set { m_ProjectId = value; }
        }

        SDBeesDocumentId m_DocumentId = new SDBeesDocumentId (Guid.Empty.ToString());
        [XmlElement("documentid")]
        public SDBeesDocumentId DocumentId
        {
            get { return m_DocumentId; }
            set { m_DocumentId = value; }
        }
    }

    public static class SDBeesExternalDocumentManager
    {
        static string m_Extension = ".sdbeesdoc";

        public static void SDBeesExternalDocumentSave(SDBeesExternalDocument doc, string filename, string pathname = null)
        {
            doc.DocOriginalName = Path.GetFileName(filename);
            SDBees.Utils.ObjectXmlSerializer.ObjectXMLSerializer<SDBeesExternalDocument>.Save(doc, FileNameFinal(filename, pathname), Encoding.Unicode);
        }

        public static SDBeesExternalDocument SDBeesExternalDocumentLoad(string filename, string pathname = null)
        {
            SDBeesExternalDocument m_ExternalDoc = null;
            string filenamefinal = FileNameFinal(filename, pathname);
            if (File.Exists(filenamefinal))
            {
                m_ExternalDoc = SDBees.Utils.ObjectXmlSerializer.ObjectXMLSerializer<SDBeesExternalDocument>.Load(filenamefinal);
                m_ExternalDoc.DocOriginalName = Path.GetFileName(filename);
            }

            return m_ExternalDoc;
        }

        public static string FileNameFinal(string filename, string pathname = null)
        {
            string path = Path.GetDirectoryName(filename);
            if (!Directory.Exists(path))
            {
                path = pathname;
            }
            string filenamecombined = Path.Combine(path, Path.GetFileName(filename) + m_Extension);
            return filenamecombined;
        }
    }
}
