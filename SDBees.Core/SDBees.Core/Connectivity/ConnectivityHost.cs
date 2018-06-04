using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace SDBees.Core.Connectivity
{
    public class ConnectivityHost
    {
        static ServiceHost m_serviceHost;

        private ConnectivityHost()
        {
            m_serviceHost = new ServiceHost(typeof(SDBeesLink.Service.SDBeesExternalPluginService));
            m_serviceHost.Faulted += new EventHandler(m_serviceHost_Faulted);
            m_serviceHost.Opening += m_serviceHost_Opening;
        }

        void m_serviceHost_Opening(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        void m_serviceHost_Faulted(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void OpenHost()
        {
            try
            {
                m_serviceHost.Open();
                if (m_serviceHost.State == CommunicationState.Opened)
                {
                }
            }
            catch (System.ObjectDisposedException ex)
            { }
            catch (System.ServiceModel.CommunicationObjectFaultedException ex)
            { }
            catch (System.TimeoutException ex)
            { }
            catch (System.InvalidOperationException ex)
            { }
            catch (System.Exception ex)
            { }
        }

        public string HostState()
        {
            return m_serviceHost.State.ToString();
        }

        public static ConnectivityHost Instance()
        {
            return EXOE.CsharpHelper.Singleton<ConnectivityHost>.Instance;
        }
    }
}
