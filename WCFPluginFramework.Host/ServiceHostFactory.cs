using System;
using System.Linq;
using System.ServiceModel;
using WCFPluginFramework.Metadata;
using System.ServiceModel.Description;

namespace WCFPluginFramework.Host
{
    public static class ServiceHostFactory
    {
        public static void AddMetaDataExchange(this ServiceHost host)
        {
            var serviceMetadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (null == serviceMetadataBehavior)
            {
                serviceMetadataBehavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(serviceMetadataBehavior);
            }
            host.AddServiceEndpoint(
                typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                "mex");
        }

        public static ServiceHost CreatePlugin(PluginDescription description, params Uri[] serverBaseAddresses)
        {
            var impl = Type.GetType(description.ImplementationName);

            var baseAddresses =
                serverBaseAddresses
                .Select(addr => new Uri(addr, description.ServiceName))
                .ToArray();

            var serviceHost =
                new ServiceHost(
                    impl,
                    baseAddresses);
            foreach (var contract in description.Contracts)
            {
                var contractInterface = Type.GetType(contract.ContractInterfaceName);
                foreach (var endpoint in contract.EndPoints)
                {
                    serviceHost.AddServiceEndpoint(
                        contractInterface,
                        new NetNamedPipeBinding(),
                        endpoint);
                }
            }
            return serviceHost;
        }
    }
}
