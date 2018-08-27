using System;
using System.ServiceModel;
using EXOE.CsharpHelper;
using SDBees.Core.Connectivity.SDBeesLink.Service;

namespace SDBees.Core.Connectivity
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectivityHost
    {
        private static ServiceHost _serviceHost;

        private ConnectivityHost()
        {
            _serviceHost = new ServiceHost(typeof(SDBeesExternalPluginService));
        }

        /// <summary>
        /// Opens the host connection.
        /// </summary>
        public void Open()
        {
            try
            {
                _serviceHost.Open();
            }
            catch (ObjectDisposedException)
            {

            }
            catch (CommunicationObjectFaultedException)
            {

            }
            catch (TimeoutException)
            {

            }
            catch (InvalidOperationException)
            {

            }
            catch (Exception)
            {
                
            }
        }


        /// <summary>
        /// Host state.
        /// </summary>
        /// <returns>returns the current state of the host</returns>
        public string HostState() => _serviceHost.State.ToString();

        /// <summary>
        /// Connectivity host
        /// </summary>
        /// <returns>Returns the host instance</returns>
        public static ConnectivityHost Instance() => Singleton<ConnectivityHost>.Instance;
    }
}
