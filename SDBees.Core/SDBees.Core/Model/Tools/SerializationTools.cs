using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

using System.Runtime.Serialization;



namespace SDBees.Core.Model
{
    public class SerializationTools
    {
        public static string Serialize(object t)
        {
            DataContractSerializer ser = new DataContractSerializer(t.GetType());
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter textWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.None })
                {
                    ser.WriteObject(textWriter, t);
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
        }

        public static T Deserialize<T>(string xml)
        {
            DataContractSerializer ser = new DataContractSerializer(typeof (T));
            using (StringReader stringReader = new StringReader (xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    return (T)ser.ReadObject(xmlReader);        
                }
            }
        }


    }
}
