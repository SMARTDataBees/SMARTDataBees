using System;
using System.Xml.Serialization;
using SDBees.Core.Model;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    [XmlRoot("sdbeesexternaldocument", Namespace = "", IsNullable = false)]
    public class SDBeesExternalDocument
    {
        /// <summary>
        /// Document name
        /// </summary>
        [XmlAttribute("docname")]
        public string DocOriginalName { get; set; } = "";


        /// <summary>
        /// Project identification
        /// </summary>
        [XmlAttribute("projectid")]
        public string ProjectId { get; set; } = Guid.Empty.ToString();


        /// <summary>
        /// Document identification
        /// </summary>
        [XmlElement("documentid")]
        public SDBeesDocumentId DocumentId { get; set; } = new SDBeesDocumentId (Guid.Empty.ToString());
    }
}
