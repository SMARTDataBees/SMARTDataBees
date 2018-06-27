using System.ServiceModel;

namespace SDBees.Core.Connectivity.SDBeesLink.Service
{
    /// <summary>
    /// External Client Interface
    /// </summary>
    [ServiceContract]
    public interface ISDBeesExternalPluginCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void PushCallbackCommand(SDBeesCallbackCommand command);
    }
}
