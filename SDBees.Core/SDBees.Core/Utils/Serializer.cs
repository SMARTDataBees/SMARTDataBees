using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using SDBees.DB;

namespace SDBees.Core.Utils
{
    public class Serializer
    {
        /// <summary>
        /// Gets the table properties and columns from the given xml
        /// </summary>
        /// <param name="xml">Xml data</param>
        /// <returns>The definition of the table</returns>
        public static Table FromXml(string xml)
        {
            var serializer = new XmlSerializer(typeof(Table), new XmlRootAttribute("TableSchema"));
            var reader = new StringReader(xml);
            return (Table)serializer.Deserialize(reader);
        }

        /// <summary>
        /// Serialize table data to xml
        /// </summary>
        /// <param name="table">Table data</param>
        /// <returns></returns>
        public static string ToXml(Table table)
        {
            var serializer = new XmlSerializer(typeof(Table));
            using (var stringWriter = new StringWriter())
            {
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    serializer.Serialize(writer, table);
                    return stringWriter.ToString();
                }
            }
        }

    }
}
