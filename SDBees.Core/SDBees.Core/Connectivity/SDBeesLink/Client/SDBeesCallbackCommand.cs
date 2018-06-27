using System.Runtime.Serialization;
using SDBees.Core.Model;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    [DataContract(Name = "sdbeescallbackcommand", Namespace = "http://www.smartdatabees.de")]
    public class SDBeesCallbackCommand
    {
        private string m_command;
        [DataMember]
        public string command
        {
            get { return m_command; }
            set { m_command = value; }
        }

        private SDBeesDataSet m_dataSet;
        [DataMember]
        public SDBeesDataSet dataSet
        {
            get { return m_dataSet; }
            set { m_dataSet = value; }
        }

        private int m_errno;
        [DataMember]
        public int Errno
        {
            get { return m_errno; }
            set { m_errno = value; }
        }

        private string m_data;
        [DataMember]
        public string Data
        {
            get { return m_data; }
            set { m_data = value; }
        }

        public SDBeesCallbackCommand(string command, SDBeesDataSet dataSet) : this(command, dataSet, 0)
        {
            // empty
        }

        public SDBeesCallbackCommand(string command, SDBeesDataSet dataSet, int errno)
        {
            m_command = command;
            m_dataSet = dataSet;
            m_errno = errno;
            m_data = null;
        }

        public SDBeesCallbackCommand(string command, SDBeesDataSet dataSet, string data)
        {
            m_command = command;
            m_dataSet = dataSet;
            m_errno = 0;
            m_data = data;
        }
    }
}
