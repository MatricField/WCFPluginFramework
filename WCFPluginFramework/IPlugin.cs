using System;
using System.ServiceModel;

namespace WCFPluginFramework
{
    [ServiceContract]
    public interface IPlugin
    {
        [OperationContract]
        PluginCapability GetCapabilities();

        [OperationContract]
        int HeartBeat(int data);
    }
}