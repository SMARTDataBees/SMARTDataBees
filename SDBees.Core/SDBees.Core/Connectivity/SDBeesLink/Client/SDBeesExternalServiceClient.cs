using System;
using System.Diagnostics;
using System.ServiceModel;
using EXOE.CsharpHelper;
using SDBees.Core.Connectivity.SDBeesLink.Service;
using SDBees.Core.Model;

namespace SDBees.Core.Connectivity.SDBeesLink
{
    [CallbackBehavior(UseSynchronizationContext = false)] 
    public class SDBeesExternalServiceClient : ISDBeesExternalPluginCallbackService
    {
        private DuplexChannelFactory<ISDBeesExternalPluginService> _namedPipeBindingFactory;
        private ISDBeesExternalPluginService _namedPipeBindingProxy;
        //private System.Threading.Timer _timer;
        private SDBeesCallbackCommandQueue m_stack;
        private Process m_process;
        
        public void PushCallbackCommand(SDBeesCallbackCommand command)
        {
            m_stack.push(command);
        }

        private SDBeesExternalServiceClient()
        {
            m_stack = new SDBeesCallbackCommandQueue();

            m_process = Process.GetCurrentProcess();
        }

        public SDBeesCallbackCommand getPendingCommand()
        {
            return m_stack.popFirst();
        }

        private string m_clientname;
        public bool Connect(string clientName)
        {
            m_clientname = UniqueClientName(clientName, m_process);

            var result = OpenDuplexChannel();

            if (result)
            {
                //_timer = new System.Threading.Timer(TimerTick, null, 0, 50000);
            }

            return result;
        }

        private bool OpenDuplexChannel()
        {
            var result = false;

            try
            {
                var context = new InstanceContext(Instance());
                var endPoint = GlobalVars.getServiceEndPoint();
                var binding = GlobalVars.getServiceBinding();
                _namedPipeBindingFactory = new DuplexChannelFactory<ISDBeesExternalPluginService>(context, binding, endPoint);
                //_namedPipeBindingFactory.

                _namedPipeBindingProxy = _namedPipeBindingFactory.CreateChannel();
                ((IClientChannel)_namedPipeBindingProxy).Faulted += Speaker_Faulted;
                ((IClientChannel)_namedPipeBindingProxy).Opened += Speaker_Opened;
                ((IClientChannel)_namedPipeBindingProxy).Open();

                //Connect to the SDBees project db
                result = _namedPipeBindingProxy.Connect(m_clientname);
            }
            catch (Exception ex)
            {
                //System.Windows.Forms.MessageBox.Show(ex.ToString());

                result = false;
            }

            return result;
        }

        /*
        void TimerTick(Object stateInfo)
        {
            try
            {
                ReConnect();
            }
            catch (Exception ex)
            {
            }
        }

        public bool ReConnect()
        {
            if (_namedPipeBindingProxy != null && (((IClientChannel)_namedPipeBindingProxy).State == CommunicationState.Opened ||
                ((IClientChannel)_namedPipeBindingProxy).State == CommunicationState.Opening))
                return true;

            bool result = OpenDuplexChannel();

            return result;
        }
        */

        public void DisConnect(string clientName)
        {
            m_clientname = UniqueClientName(clientName, m_process);
            try
            {
                _namedPipeBindingProxy.Disconnect(m_clientname);
                ((IClientChannel)_namedPipeBindingProxy).Close(new TimeSpan(0, 0, 5));
                //this._timer = null;
            }
            catch (Exception ex)
            {
            }
        }

        private static string UniqueClientName(string clientName, Process process)
        {
            return process.Id + ":" + clientName;
        }

        void Speaker_Opened(object sender, EventArgs e)
        {
        }

        void Speaker_Faulted(object sender, EventArgs e)
        {
        }

        public SDBeesExternalMappings GetMappingsFromServer(SDBeesPluginId pluginId, SDBeesPluginRoleId roleId, SDBeesDocumentId docId)
        {
            return _namedPipeBindingProxy.MappingsGet(pluginId, roleId, docId);
        }

        public void SendData(SDBeesExternalDocument doc, SDBeesDataSet dataset, SDBeesSyncMode mode, IntPtr windowHandle, bool blockApplication = true)
        {
            try
            {
                if (_namedPipeBindingProxy != null)
                {
                    _namedPipeBindingProxy.ExternalDocumentSynchronize(doc, mode, dataset, windowHandle, blockApplication);
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        public void CancelSendData()
        {
            _namedPipeBindingProxy.CancelExternalDocumentSynchronize();
        }

        public static SDBeesExternalServiceClient Instance()
        {
            return Singleton<SDBeesExternalServiceClient>.Instance;
        }

        public SDBeesExternalDocument DocGet(string m_DocName)
        {
            return _namedPipeBindingProxy.ExternalDocumentGet(m_DocName);
        }

        public SDBeesExternalDocument DocRegister(string m_DocName, string pluginId, string roleId)
        {
            return _namedPipeBindingProxy.ExternalDocumentRegister(m_DocName, pluginId, roleId);
            //SDBees.Core.Connectivity.SDBeesLink.SDBeesExternalDocumentManager.SDBeesExternalDocumentSave(_doc, this.GetType().Assembly.Location);
        }

        public bool OpenExistingProject(SDBeesExternalDocument _doc)
        {
            // = SDBees.Core.Connectivity.SDBeesLink.SDBeesExternalDocumentManager.SDBeesExternalDocumentLoad(this.GetType().Assembly.Location);
            return _namedPipeBindingProxy.ProjectOpenExisting(new SDBeesProjectId(_doc.ProjectId));
        }

        public SDBeesPluginRoleId SelectCurrentRole()
        {
            return _namedPipeBindingProxy.RoleSelectCurrent();
        }

        public bool OpenProject(string filenameDatabase, bool createIfNotFound)
        {
            return _namedPipeBindingProxy.ProjectOpen(filenameDatabase, createIfNotFound);
        }

        public void UpdateServerDialog()
        {
            _namedPipeBindingProxy.UpdateServerDialog();
        }

        public void ReturnData(SDBeesDataSet data, int errno)
        {
            _namedPipeBindingProxy.ReturnData(data, errno);
        }

        public string GetData(string name, string explorerPluginDesriptor = "")
        {
            return _namedPipeBindingProxy.GetData(name, explorerPluginDesriptor);
        }
    }
}
