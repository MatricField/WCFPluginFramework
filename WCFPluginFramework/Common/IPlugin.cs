using System;
using System.ServiceModel;

namespace WCFPluginFramework.Common
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IPlugin:
        IHeartBeatService
    {
        [OperationContract(IsInitiating = true)]
        [FaultContract(typeof(SerializableException))]
        void Connect();

        [OperationContract(IsTerminating = true)]
        [FaultContract(typeof(SerializableException))]
        void Disconnect();

        [OperationContract]
        PluginInfo GetPluginInfo();
    }
}