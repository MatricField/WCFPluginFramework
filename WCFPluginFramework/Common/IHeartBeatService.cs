using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFPluginFramework.Common
{
    [ServiceContract]
    public interface IHeartBeatService
    {
        [OperationContract]
        int HeartBeat(int data);
    }
}
