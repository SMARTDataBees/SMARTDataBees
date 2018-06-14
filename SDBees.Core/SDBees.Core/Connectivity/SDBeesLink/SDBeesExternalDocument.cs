using System;
using System.Xml.Serialization;
using SDBees.Core.Model;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    [XmlRoot("sdbeesexternaldocument", Namespace = "", IsNullable = false)]
    public class SDBeesExternalDocument
    {
        string m_docOriginalName = "";
        [XmlAttribute("docname")]
        public string DocOriginalName
        {
            get { return m_docOriginalName; }
            set { m_docOriginalName = value; }
        }

        [XmlAttribute("projectid")]
        public string ProjectId { get; set; } = Guid.Empty.ToString();

        [XmlElement("documentid")]
        public SDBeesDocumentId DocumentId { get; set; } = new SDBeesDocumentId (Guid.Empty.ToString());
    }
}
