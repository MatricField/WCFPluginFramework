using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFPluginFramework.Common
{
    [ServiceContract(Name = nameof(IHeartBeatService))]
    public interface IHeartBeatServiceAsync
    {
        [OperationContract]
        Task<int> HeartBeatAsync(int data);
    }
}
