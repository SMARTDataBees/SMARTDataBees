using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;
using Carbon.Plugins;
using SDBees.Core.Connectivity.SDBeesLink.UI;
using SDBees.Core.Main.Systemtray;
using SDBees.Core.Model;
using SDBees.Plugs.Explorer;
using Timer = System.Threading.Timer;

namespace SDBees.Core.Connectivity.SDBeesLink.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SDBeesExternalPluginService : ISDBeesExternalPluginService
    {
        Dispatcher m_dispatcher;
        SDBeesCallbackCommandQueue m_uiCommands;
        SDBeesCallbackCommandQueue m_returnCommands;
        string m_activeDocumentId;
        private bool m_isEditDataSet;

        iExplorer m_explorer;

        public bool IsEditDataSet
        {
            get
            {
                return m_isEditDataSet;
            }
        }

        private Control MainControl
        {
            get
            {
#if false
                return SDBees.Main.Window.MainWindowApplication.Current.MyMainWindow.TheDialog;
#else
                return ProcessIcon.Current.MyContextMenu;
#endif
            }
        }

        private bool InvokeRequired
        {
            get
            {
                return MainControl.InvokeRequired;
            }
        }

        private object BeginInvoke(Delegate method, object param1)
        {
            return MainControl.BeginInvoke(method, param1);
        }

        private object BeginInvoke(Delegate method, object param1, object param2)
        {
            return MainControl.BeginInvoke(method, param1, param2);
        }

        private object BeginInvoke(Delegate method, object param1, object param2, object param3)
        {
            return MainControl.BeginInvoke(method, param1, param2, param3);
        }

        private object Invoke(Delegate method)
        {
            return MainControl.Invoke(method);
        }
  
        private object Invoke(Delegate method, object param1)
        {
            return MainControl.Invoke(method, param1);
        }

        private object Invoke(Delegate method, object param1, object param2)
        {
            return MainControl.Invoke(method, param1, param2);
        }

        private object Invoke(Delegate method, object param1, object param2, object param3)
        {
            return MainControl.Invoke(method, param1, param2, param3);
        }

        private void ShowDialogInUIThread(iExplorer explorer, IWin32Window window, bool blockApplication)
        {
            if (InvokeRequired)
            {
                BeginInvoke(ShowDialogDelegate(), explorer, window, blockApplication);
            }
            else
            {
                ShowDialog(explorer, window, blockApplication);
            }
        }

        private void CloseDialogInUIThread(iExplorer explorer)
        {
            if (InvokeRequired)
            {
                BeginInvoke(CloseDialogDelegate(), explorer);
            }
            else
            {
                CloseDialog(explorer);
            }
        }

        public bool Connect(string name)
        {
            if (InvokeRequired)
            {
                return (bool)Invoke(Commands.ExternalClientConnectDelegate(), name, this);
            }
            return Commands.ExternalClientConnect(name, this);
        }

        public void Disconnect(string name)
        {
            if (InvokeRequired)
            {
                Invoke(Commands.ExternalClientDisconnectDelegate(), name, this);
            }
            else
            {
                Commands.ExternalClientDisconnect(name, this);
            }

            ConnectivityManager.Current.ExternalClientDisconnect(name, this);
        }

        public void Login(string user, string password)
        {
            // Please make sure to use InvokeRequired/Invoke as done for the other functions!

            throw new NotImplementedException();
        }

        public SDBeesProjectId ProjectSelectExisting()
        {
            if (InvokeRequired)
            {
                return (SDBeesProjectId)Invoke(Commands.ProjectSelectExistingDelegate());
            }
            return Commands.ProjectSelectExisting();
        }

        public bool ProjectOpenExisting(SDBeesProjectId projectid)
        {
            if (InvokeRequired)
            {
                return (bool)Invoke(Commands.ProjectOpenExistingDelegate(), projectid);
            }
            return Commands.ProjectOpenExisting(projectid);
        }

        public SDBeesProjectId ProjectCreateNew()
        {
            // Please make sure to use InvokeRequired/Invoke as done for the other functions!

            throw new NotImplementedException();
        }

        public bool ProjectOpen(string filenameDatabase, bool createIfNotFound)
        {
            if (InvokeRequired)
            {
                return (bool)Invoke(Commands.ProjectOpenDelegate(), filenameDatabase, createIfNotFound);
            }
            return Commands.ProjectOpen(filenameDatabase, createIfNotFound);
        }

        public SDBeesExternalDocument ExternalDocumentGet(string LocalDocumentId)
        {
            if (InvokeRequired)
            {
                return (SDBeesExternalDocument)Invoke(Commands.ExternalDocumentGetDelegate(), LocalDocumentId);
            }
            return Commands.ExternalDocumentGet(LocalDocumentId);
        }

        public SDBeesExternalDocument ExternalDocumentRegister(string LocalDocumentId, string pluginId, string roleId)
        {
            if (InvokeRequired)
            {
                return (SDBeesExternalDocument)Invoke(Commands.ExternalDocumentRegisterDelegate(), LocalDocumentId, pluginId, roleId);
            }
            return Commands.ExternalDocumentRegister(LocalDocumentId, pluginId, roleId);
        }

        public void ExternalDocumentUnregister(SDBeesDocumentId documentId)
        {
            // Please make sure to use InvokeRequired/Invoke as done for the other functions!

            throw new NotImplementedException();
        }

        public void ExternalDocumentSynchronize(SDBeesExternalDocument doc, SDBeesSyncMode mode, SDBeesDataSet data, IntPtr windowHandle, bool blockApplication)
        {
            try
            {
                m_dispatcher = Dispatcher.CurrentDispatcher;
                m_uiCommands = new SDBeesCallbackCommandQueue();
                m_activeDocumentId = doc.DocumentId.ToString();

                SDBeesDataSetTools.normalizePropertyValues(data);

                if (mode == SDBeesSyncMode.EditDataSet)
                {
                    m_isEditDataSet = true;

                    CommandEditDataSet(doc, data, new WindowHandle(windowHandle), blockApplication);
                }
                else if (mode == SDBeesSyncMode.UpdateClient)
                {
                    CommandUpdateClient(doc, data);
                }
                else if (mode == SDBeesSyncMode.UpdateServer)
                {
                    CommandUpdateServer(doc, data, mode);
                }
                else if (mode == SDBeesSyncMode.UpdateServerValidation)
                {
                    CommandUpdateServer(doc, data, mode);
                }
                else if (mode == SDBeesSyncMode.ShowServerDialog)
                {
                    CommandShowServerDialog(doc);
                }
                else if (mode == SDBeesSyncMode.SetUpBuildingStructure)
                {
                    CommandSetUpBuildingStructure(doc, data);
                }
                else if (mode == SDBeesSyncMode.ShowIssueBrowser)
                {
                    CommandShowIssueBrowser(doc, data, new WindowHandle(windowHandle), blockApplication);
                }
            }
            finally
            {
                m_dispatcher = null;
                m_uiCommands = null;
                m_activeDocumentId = null;
                m_isEditDataSet = false;
            }
        }

        public void CancelExternalDocumentSynchronize()
        {
            if (m_explorer != null)
            {
                CloseDialogInUIThread(m_explorer);
            }
            else
            {
                ReturnFromEdit(new SDBeesDataSet());
            }
        }

        private void CommandEditDataSet(SDBeesExternalDocument doc, SDBeesDataSet data, IWin32Window window, bool blockApplication)
        {
            var entDefs = (data.EntityDefinitions != null) ? data.EntityDefinitions : ConnectivityManager.Current.GetEntityDefinitions();

            iExplorer explorer = null;

            var pluginDescriptor = ConnectivityManager.Current.MyContext.PluginDescriptors[data.ExplorerPlugin];

            if (pluginDescriptor != null)
            {
                var explorerPlugin = (ExplorerPlugin)pluginDescriptor.PluginInstance;

                explorerPlugin.ExplorerMode = ExplorerMode.eQualification;

                explorerPlugin.EntityDefinitions = entDefs;

                explorer = explorerPlugin;
            }
            else
            {
                explorer = new SDBeesDataSetDLG(entDefs);
            }

            explorer.Context = ConnectivityManager.Current.MyContext;

            explorer.MyPluginService = this;

            explorer.MyDataSet = data;

            ShowDialogInUIThread(explorer, window, blockApplication);

            handleUICommands();
        }

        private void CommandUpdateClient(SDBeesExternalDocument doc, SDBeesDataSet data)
        {
            SDBeesDataSet m_dSet = null;

            if (InvokeRequired)
            {
                m_dSet = (SDBeesDataSet)Invoke(Commands.UpdateClientDelegate(), doc, data);
            }
            else
            {
                m_dSet = Commands.UpdateClient(doc, data);
            }

            ReturnFromEdit(m_dSet);
        }
        
        private void CommandUpdateServer(SDBeesExternalDocument doc, SDBeesDataSet data, SDBeesSyncMode mode)
        {
            if (InvokeRequired)
            {
                Invoke(Commands.UpdateServerDelegate(), doc, data, mode);
            }
            else
            {
                Commands.UpdateServer(doc, data, mode);
            }

            ReturnFromEdit(data);
        }

        private void CommandShowServerDialog(SDBeesExternalDocument doc)
        {
            if (InvokeRequired)
            {
                BeginInvoke(Commands.ShowServerDialogDelegate(), doc);
            }
            else
            {
                Commands.ShowServerDialog(doc);
            }

            handleUICommands();
        }

        private void CommandSetUpBuildingStructure(SDBeesExternalDocument doc, SDBeesDataSet data)
        {
            try
            {
                ConnectivityManager.Current.SetupBuildingSubLevels(doc, data);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                ReturnFromEdit(data);
            }
        }

        private void CommandShowIssueBrowser(SDBeesExternalDocument doc, SDBeesDataSet data, IWin32Window window, bool blockApplication)
        {
            var entDefs = (data.EntityDefinitions != null) ? data.EntityDefinitions : ConnectivityManager.Current.GetEntityDefinitions();

            iExplorer explorer = null;

            var pluginDescriptor = ConnectivityManager.Current.MyContext.PluginDescriptors[data.ExplorerPlugin];

            if (pluginDescriptor != null)
            {
                var explorerPlugin = (ExplorerPlugin)pluginDescriptor.PluginInstance;

                explorerPlugin.ExplorerMode = ExplorerMode.eIssues;

                explorerPlugin.EntityDefinitions = entDefs;

                explorer = explorerPlugin;
            }

            if (explorer != null)
            {
                explorer.Context = ConnectivityManager.Current.MyContext;

                explorer.MyPluginService = this;

                explorer.MyDataSet = data;

                ShowDialogInUIThread(explorer, window, blockApplication);

                handleUICommands();
            }
        }

        public void ReturnData(SDBeesDataSet data, int errno)
        {
            var command = new SDBeesCallbackCommand("Return", data, errno);

            if (m_returnCommands != null) m_returnCommands.push(command);
        }

        public string GetData(string name, string explorerPluginDesriptor)
        {
            string result = null;

            if (InvokeRequired)
            {
                var retval = Invoke(Commands.GetDataDelegate(), name, explorerPluginDesriptor);

                result = retval as string;
            }
            else
            {
                result =  Commands.GetData(name, explorerPluginDesriptor);
            }

            return result;
        }

        public SDBeesExternalMappings MappingsGet(SDBeesPluginId pluginId, SDBeesPluginRoleId roleId, SDBeesDocumentId documentId)
        {
            // Please make sure to use InvokeRequired/Invoke as done for the other functions!

            throw new NotImplementedException();
        }

        public void MappingsDefine(SDBeesExternalAvailableInfo pluginInfo, SDBeesPluginId pluginId, SDBeesPluginRoleId roleId)
        {
            // Please make sure to use InvokeRequired/Invoke as done for the other functions!

            throw new NotImplementedException();
        }

        public SDBeesPluginRoleId RoleDefineNew(SDBeesExternalDocument doc)
        {
            // Please make sure to use InvokeRequired/Invoke as done for the other functions!

            throw new NotImplementedException();
        }

        public SDBeesPluginRoleId RoleSelectCurrent()
        {
            return new SDBeesPluginRoleId(Guid.Empty.ToString());
        }

        private void handleUICommands()
        {
            var handleCommands = true;

            var keepAliveCommand = new SendKeepAliveCommand(this, SendKeepAliveCommand.Mode.OnRequest);

            try
            {
                while (handleCommands)
                {
                    var command = m_uiCommands.popFirst();
                    if (command != null)
                    {
                        switch (command.command)
                        {
                            case "RequestQualification":
                                pushClientCommand(command);
                                break;
                            case "ApplyMapping":
                                pushClientCommand(command);
                                break;
                            case "Highlight":
                                pushClientCommand(command);
                                break;
                            case "ReturnFromEdit":
                                pushClientCommand(command);
                                handleCommands = false;
                                break;
                        }
                    }

                    keepAliveCommand.keepAlive();

                    Application.DoEvents();

                    Thread.Yield();
                }
            }
            finally
            {
                keepAliveCommand.Dispose();

                keepAliveCommand = null;
            }
        }

        private SDBeesDataSet handleReturnCommands(out int errno)
        {
            SDBeesDataSet result = null;

            errno = 0;

            var handleCommands = true;

            var keepAliveCommand = new SendKeepAliveCommand(this, SendKeepAliveCommand.Mode.OnRequest);

            try
            {
                while (handleCommands)
                {
                    var command = m_returnCommands.popFirst();
                    if (command != null)
                    {
                        switch (command.command)
                        {
                            case "Return":
                                handleCommands = false;
                                result = command.dataSet;
                                errno = command.Errno;
                                break;
                        }
                    }

                    keepAliveCommand.keepAlive();

                    Application.DoEvents();

                    Thread.Yield();
                }
            }
            finally
            {
                keepAliveCommand.Dispose();

                keepAliveCommand = null;
            }

            return result;
        }

        public void pushClientCommand(SDBeesCallbackCommand command)
        {
            try
            {
                if (m_dispatcher.CheckAccess())
                {
                    OperationContext.Current.GetCallbackChannel<ISDBeesExternalPluginCallbackService>().PushCallbackCommand(command);
                }
                else
                {
                    if (command.command != "KeepAliveCommand")
                    {
                        m_uiCommands.push(command);
                    }
                    Thread.Yield();
                }
            }
            catch (Exception ex)
            {
                //Fails, if it takes to much time to push back to external application...
                MessageBox.Show(ex.Message);
            }
        }

        public void ShowEntity(string instanceId, HashSet<SDBeesAlienId> alienId)
        {
            var dataSet = new SDBeesDataSet();

            AddEntity(ref dataSet, instanceId, alienId);

            var command = new SDBeesCallbackCommand("Highlight", dataSet);

            pushClientCommand(command);
        }

        public void ShowEntities(List<SDBeesEntity> entities)
        {
            var dataSet = new SDBeesDataSet();

            foreach (var entity in entities)
            {
                AddEntity(ref dataSet, entity.InstanceId.ToString(), entity.AlienIds);
            }

            var command = new SDBeesCallbackCommand("Highlight", dataSet);

            pushClientCommand(command);
        }

        public SDBeesDataSet ApplyMapping(SDBeesExternalMappings mappings, out int errno)
        {
            var dataSet = new SDBeesDataSet();

            dataSet.Mappings = mappings;

            m_returnCommands = new SDBeesCallbackCommandQueue();

            var command = new SDBeesCallbackCommand("ApplyMapping", dataSet);

            pushClientCommand(command);

            var result = handleReturnCommands(out errno);

            m_returnCommands = null;

            return result;
        }

        public SDBeesDataSet RequestQualification(string explorerPluginDescriptor, string dataName, SDBeesExternalMappings mappings, out int errno)
        {
            var dataSet = new SDBeesDataSet();

            dataSet.Mappings = mappings;

            m_returnCommands = new SDBeesCallbackCommandQueue();

            var data = GetData(dataName, explorerPluginDescriptor);

            var command = new SDBeesCallbackCommand("RequestQualification", dataSet, data);

            pushClientCommand(command);

            var result = handleReturnCommands(out errno);

            m_returnCommands = null;

            return result;
        }

        private void AddEntity(ref SDBeesDataSet dataSet, string instanceId, HashSet<SDBeesAlienId> alienId)
        {
            var entity = new SDBeesEntity();
            entity.Id = new SDBeesEntityId(instanceId);
            entity.AlienIds = new HashSet<SDBeesAlienId>();
            foreach (var id in alienId)
            {
                if (id.DocumentId.Equals(new SDBeesDocumentId(m_activeDocumentId)))
                {
                    entity.AlienIds.Add(id);
                }
                else if (m_activeDocumentId == new SDBeesDocumentId(Guid.Empty.ToString()))
                {
                    entity.AlienIds.Add(id);
                }
            }
            dataSet.Entities.Add(entity);
        }

        public void ReturnFromEdit(SDBeesDataSet dataSet)
        {
            if (dataSet == null)
            {
                dataSet = new SDBeesDataSet();
            }
            var commandName = "ReturnFromEdit";
            var command = new SDBeesCallbackCommand(commandName, dataSet);
            pushClientCommand(command);
        }

        private delegate void ShowDialogFunction(iExplorer explorer, IWin32Window window, bool blockApplication);

        private Delegate ShowDialogDelegate()
        {
            return new ShowDialogFunction(ShowDialog);
        }

        private void ShowDialog(iExplorer explorer, IWin32Window window, bool blockApplication)
        {
            SDBeesDataSet dataSet = null;

            try
            {
                m_explorer = explorer;

                if (explorer.ShowDialog(window, blockApplication) == DialogResult.OK)
                {
                    dataSet = explorer.MyDataSet;

                    SDBeesDataSet.DebugDataSetWrite("EditDataSetFeedbackFromDB.xml", dataSet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                m_explorer = null;
            }

            ReturnFromEdit(dataSet);
        }

        private delegate void CloseDialogFunction(iExplorer explorer);

        private Delegate CloseDialogDelegate()
        {
            return new CloseDialogFunction(CloseDialog);
        }

        private void CloseDialog(iExplorer explorer)
        {
            explorer.CloseDialog();
        }

        public void UpdateServerDialog()
        {
            if (InvokeRequired)
            {
                Invoke(Commands.UpdateServerDialogDelegate());
            }
            else
            {
                Commands.UpdateServerDialog();
            }
        }

        private class WindowHandle : IWin32Window
        {
            IntPtr _hwnd;

            public WindowHandle(IntPtr h)
            {
                _hwnd = h;
            }

            public IntPtr Handle
            {
                get
                {
                    return _hwnd;
                }
            }
        }
    }

    public class Commands
    {
        // Connect ...

        private delegate bool ExternalClientConnectFunction(string name, SDBeesExternalPluginService externalPluginService);

        public static Delegate ExternalClientConnectDelegate()
        {
            return new ExternalClientConnectFunction(ExternalClientConnect);
        }

        public static bool ExternalClientConnect(string name, SDBeesExternalPluginService externalPluginService)
        {
            return ConnectivityManager.Current.ExternalClientConnect(name, externalPluginService);
        }

        // Disconnect ...

        private delegate void ExternalClientDisconnectFunction(string name, SDBeesExternalPluginService externalPluginService);

        public static Delegate ExternalClientDisconnectDelegate()
        {
            return new ExternalClientDisconnectFunction(ExternalClientDisconnect);
        }

        public static void ExternalClientDisconnect(string name, SDBeesExternalPluginService externalPluginService)
        {
            ConnectivityManager.Current.ExternalClientDisconnect(name, externalPluginService);
        }

        // ProjectGetCurrentId ...

        private delegate SDBeesProjectId ProjectSelectExistingFunction();

        public static Delegate ProjectSelectExistingDelegate()
        {
            return new ProjectSelectExistingFunction(ProjectSelectExisting);
        }

        public static SDBeesProjectId ProjectSelectExisting()
        {
            return ConnectivityManager.Current.ProjectGetCurrentId();
        }

        // ProjectOpenExisting ...

        private delegate bool ProjectOpenExistingFunction(SDBeesProjectId projectid);

        public static Delegate ProjectOpenExistingDelegate()
        {
            return new ProjectOpenExistingFunction(ProjectOpenExisting);
        }

        public static bool ProjectOpenExisting(SDBeesProjectId projectid)
        {
            return ConnectivityManager.Current.ProjectOpen(projectid);
        }

        // ProjectOpen ...

        private delegate bool ProjectOpenFunction(string filenameDatabase, bool createIfNotFound);

        public static Delegate ProjectOpenDelegate()
        {
            return new ProjectOpenFunction(ProjectOpen);
        }

        public static bool ProjectOpen(string filenameDatabase, bool createIfNotFound)
        {
            return ConnectivityManager.Current.ProjectOpen(filenameDatabase, createIfNotFound);
        }

        // ExternalDocumentGet ...

        private delegate SDBeesExternalDocument ExternalDocumentGetFunction(string LocalDocumentId);

        public static Delegate ExternalDocumentGetDelegate()
        {
            return new ExternalDocumentGetFunction(ExternalDocumentGet);
        }

        public static SDBeesExternalDocument ExternalDocumentGet(string LocalDocumentId)
        {
            var doc = new SDBeesExternalDocument();
            doc.DocOriginalName = Path.GetFileName(LocalDocumentId);

            return ConnectivityManager.Current.DocumentGet(doc);
        }

        // ExternalDocumentRegister ...

        private delegate SDBeesExternalDocument ExternalDocumentRegisterFunction(string LocalDocumentId, string pluginId, string roleId);

        public static Delegate ExternalDocumentRegisterDelegate()
        {
            return new ExternalDocumentRegisterFunction(ExternalDocumentRegister);
        }

        public static SDBeesExternalDocument ExternalDocumentRegister(string LocalDocumentId, string pluginId, string roleId)
        {
            var doc = new SDBeesExternalDocument();
            doc.DocOriginalName = Path.GetFileName(LocalDocumentId);

            return ConnectivityManager.Current.DocumentRegister(doc, pluginId, roleId);
        }

        // UpdateClient ...

        private delegate SDBeesDataSet UpdateClientFunction(SDBeesExternalDocument doc, SDBeesDataSet data);

        public static Delegate UpdateClientDelegate()
        {
            return new UpdateClientFunction(UpdateClient);
        }

        public static SDBeesDataSet UpdateClient(SDBeesExternalDocument doc, SDBeesDataSet data)
        {
            SDBeesDataSet m_dSet = null;

            try
            {
                m_dSet = ConnectivityManager.Current.SynchronizeClient(doc);

                SDBeesDataSet.DebugDataSetWrite("SyncClient.xml", m_dSet);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return m_dSet;
        }

        // UpdateServer ...

        private delegate void UpdateServerFunction(SDBeesExternalDocument doc, SDBeesDataSet data, SDBeesSyncMode mode);

        public static Delegate UpdateServerDelegate()
        {
            return new UpdateServerFunction(UpdateServer);
        }

        public static void UpdateServer(SDBeesExternalDocument doc, SDBeesDataSet data, SDBeesSyncMode mode)
        {
            try
            {
                ConnectivityManager.Current.SynchronizeServer(doc, data, mode);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // ShowServerDialog ...

        private delegate void ShowServerDialogFunction(SDBeesExternalDocument doc);

        public static Delegate ShowServerDialogDelegate()
        {
            return new ShowServerDialogFunction(ShowServerDialog);
        }

        public static void ShowServerDialog(SDBeesExternalDocument doc)
        {
            try
            {
                ConnectivityManager.Current.ShowServerDialog(doc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // UpdateServerDialog ...

        private delegate void UpdateServerDialogFunction();

        public static Delegate UpdateServerDialogDelegate()
        {
            return new UpdateServerDialogFunction(UpdateServerDialog);
        }

        public static void UpdateServerDialog()
        {
            try
            {
                ConnectivityManager.Current.UpdateServerDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // GetData ...

        private delegate string GetDataFunction(string name, string explorerPluginDesriptor);

        public static Delegate GetDataDelegate()
        {
            return new GetDataFunction(GetData);
        }

        public static string GetData(string name, string explorerPluginDesriptor)
        {
            string result = null;

            try
            {
                iExplorer explorer = null;

                var pluginDescriptor = ConnectivityManager.Current.MyContext.PluginDescriptors[explorerPluginDesriptor];

                if (pluginDescriptor != null)
                {
                    var explorerPlugin = (ExplorerPlugin)pluginDescriptor.PluginInstance;

                    explorer = explorerPlugin;
                }
                else
                {
                    explorer = new SDBeesDataSetDLG(null);
                }

                if (explorer != null)
                {
                    result = explorer.GetData(name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return result;
        }
    }

    public class SendKeepAliveCommand
    {
        public enum Mode { None, OnRequest, OnTimer }

        private Mode m_mode = Mode.None;

        private SDBeesExternalPluginService m_service;

        private Timer m_timer;

        private bool m_timerTicked;

        private long m_dueTime = 15000; // ms  

        private long m_period = 30000; // ms

        public SendKeepAliveCommand(SDBeesExternalPluginService service, Mode mode)
        {
            m_mode = mode;

            m_service = service;

            m_timer = new Timer(TimerTick, null, m_dueTime, m_period);

            m_timerTicked = false;
        }

        public void Dispose()
        {
            m_timerTicked = false;

            m_timer.Dispose();

            m_timer = null;

            m_service = null;

            m_mode = Mode.None;
        }

        public void keepAlive()
        {
            if (m_mode == Mode.OnRequest)
            {
                if (m_timerTicked)
                {
                    sendKeepAliveCommand();

                    m_timerTicked = false;
                }
            }
        }

        private void TimerTick(Object stateInfo)
        {
            if (m_mode == Mode.OnRequest)
            {
                m_timerTicked = true;
            }
            else if (m_mode == Mode.OnTimer)
            {
                sendKeepAliveCommand();
            }
        }

        private void sendKeepAliveCommand()
        {
            if (m_service != null)
            {
                var keepAliveCommand = new SDBeesCallbackCommand("KeepAliveCommand", null);

                m_service.pushClientCommand(keepAliveCommand);
            }
        }
    }
}
