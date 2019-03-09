using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace WCFPluginFramework.Host
{
    static class ServiceHostExtension
    {
        public static void AddMetaDataExchange(this ServiceHost host, Uri pluginBaseAddress)
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
                new Uri(pluginBaseAddress, "mex"));
        }
    }
}
