using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

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
