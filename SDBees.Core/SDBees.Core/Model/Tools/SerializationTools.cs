using System.IO;
using System.Runtime.Serialization;
using System.Xml;

namespace SDBees.Core.Model
{
    public class SerializationTools
    {
        public static string Serialize(object t)
        {
            var ser = new DataContractSerializer(t.GetType());
            using (var stringWriter = new StringWriter())
            {
                using (var textWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.None })
                {
                    ser.WriteObject(textWriter, t);
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
        }

        public static T Deserialize<T>(string xml)
        {
            var ser = new DataContractSerializer(typeof (T));
            using (var stringReader = new StringReader (xml))
            {
                using (var xmlReader = XmlReader.Create(stringReader))
                {
                    return (T)ser.ReadObject(xmlReader);        
                }
            }
        }


    }
}
