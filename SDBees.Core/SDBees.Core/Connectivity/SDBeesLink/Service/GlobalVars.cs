using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace SDBees.Core.Connectivity.SDBeesLink.Service
{
    public static class GlobalVars
    {
        public static EndpointAddress getServiceEndPoint()
        {
            return new EndpointAddress("net.pipe://localhost/SDBees.Core/SDBeesExternalPluginService");
        }

        public static System.ServiceModel.Channels.Binding getServiceBinding()
        {
            NetNamedPipeBinding retval = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);

            retval.CloseTimeout = new TimeSpan(5, 55, 0); //TODO : TH Check close timeout
            retval.OpenTimeout = new TimeSpan(5, 55, 0);
            retval.ReceiveTimeout = new TimeSpan(5, 55, 0);
            retval.SendTimeout = new TimeSpan(5, 55, 0);
            retval.MaxBufferPoolSize = 50000000;
            retval.MaxBufferSize = 50000000;
            retval.MaxReceivedMessageSize = 50000000;
            retval.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            retval.MaxConnections = 100;
            retval.ReaderQuotas.MaxStringContentLength = 50000000;
            retval.ReaderQuotas.MaxDepth = 32;
            retval.ReaderQuotas.MaxArrayLength = 50000000;
            retval.ReaderQuotas.MaxBytesPerRead = 50000000;
            retval.ReaderQuotas.MaxNameTableCharCount = 50000000;
            retval.Security.Mode = NetNamedPipeSecurityMode.Transport;
            retval.Name = "ISDBeesExternalPluginService";
            //retval.TransactionProtocol = new OleTransactions

            return retval;
        }
    }
}
