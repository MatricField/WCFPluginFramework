using System;
using System.Collections.Generic;
using System.ServiceModel;
using WCFPluginFramework.Common;
using WCFPluginFramework.Metadata;

namespace WCFPluginFramework.Host
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPluginHostControl" in both code and config file together.
    [ServiceContract]
    public interface IPluginHostControl :
        IHeartBeatService
    {
        [OperationContract]
        void Shutdown();

        [OperationContract]
        IEnumerable<PluginDescription> EnumerateAvailablePlugins();

        [OperationContract]
        IDictionary<Uri, string> EnumerateEndPoints();

        [OperationContract]
        void LoadPluginAssembly(string path);
    }
}
