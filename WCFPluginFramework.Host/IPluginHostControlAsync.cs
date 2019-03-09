﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using WCFPluginFramework.Common;
using WCFPluginFramework.Metadata;

namespace WCFPluginFramework.Host
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPluginHostControl" in both code and config file together.
    [ServiceContract(Name = nameof(IPluginHostControl))]
    public interface IPluginHostControlAsync:
        IHeartBeatServiceAsync
    {
        [OperationContract]
        Task ShutdownAsync();

        [OperationContract]
        Task<IEnumerable<PluginDescription>> EnumerateAvailablePluginsAsync();

        [OperationContract]
        [FaultContract(typeof(SerializableException))]
        Task LoadPluginAssemblyAsync(string path);
    }
}
