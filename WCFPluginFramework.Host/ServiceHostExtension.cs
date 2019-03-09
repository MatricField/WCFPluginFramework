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

        public static void PrintBaseAddress(this ServiceHost host)
        {
            Console.WriteLine("Base addresses:");
            foreach (var address in host.BaseAddresses)
            {
                Console.WriteLine($"    {address}");
            }
        }

        public static void PrintEndPoints(this ServiceHost host)
        {
            Console.WriteLine("Endpoints:");
            foreach (var endpoint in host.Description.Endpoints)
            {
                Console.WriteLine($"    {endpoint.Address}");
            }
        }
    }
}
