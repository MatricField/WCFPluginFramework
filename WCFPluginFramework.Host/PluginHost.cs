using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFPluginFramework.Metadata;

namespace WCFPluginFramework.Host
{
    public class PluginHost
    {
        public ServiceHost ServiceHost { get; }

        public IEnumerable<Uri> Endpoints =>
            ServiceHost.Description.Endpoints.Select(x => x.Address.Uri);

        public PluginHost(PluginDescription desc, Uri pluginBaseAddress)
        {
            var impl = Type.GetType(desc.ImplementationName);

            var serviceHost =
                new ServiceHost(
                    impl,
                    new Uri(pluginBaseAddress, desc.ServiceName));
            foreach (var contract in desc.Contracts)
            {
                var contractInterface = Type.GetType(contract.ContractInterfaceName);
                foreach (var endpoint in contract.EndPoints)
                {
                    serviceHost.AddServiceEndpoint(
                        contractInterface,
                        new NetNamedPipeBinding(),
                        endpoint
                        );
                }
            }
            serviceHost.AddMetaDataExchange();
            serviceHost.PrintEndPoints();
            serviceHost.Open();
            
        }
    }
}
