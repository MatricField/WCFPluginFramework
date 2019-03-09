using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
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
