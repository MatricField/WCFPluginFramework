﻿using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace WCFPluginFramework.Common
{
    [ServiceContract(SessionMode = SessionMode.Required, Name = nameof(IPlugin))]
    public interface IPluginAsync:
        IHeartBeatServiceAsync
    {
        [OperationContract(IsInitiating = true)]
        [FaultContract(typeof(SerializableException))]
        Task ConnectAsync();

        [OperationContract(IsTerminating = true)]
        [FaultContract(typeof(SerializableException))]
        Task DisconnectAsync();

        [OperationContract]
        Task<PluginInfo> GetPluginInfoAsync();
    }
}