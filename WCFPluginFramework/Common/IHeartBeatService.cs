using System.ServiceModel;

namespace WCFPluginFramework.Common
{
    [ServiceContract]
    public interface IHeartBeatService
    {
        [OperationContract]
        int HeartBeat(int data);
    }
}
