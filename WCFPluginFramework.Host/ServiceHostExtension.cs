using System;
using System.ServiceModel;

namespace WCFPluginFramework.Host
{
    static class ServiceHostExtension
    { 
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
