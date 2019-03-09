using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using System.Threading.Tasks;

namespace WCFPluginFramework.Host
{
    class HostManager
    { 
        static async Task Main()
        {
            var pluginBaseAddress =
                new Uri(GetRawArg());
            var serviceHost = new ServiceHost(typeof(PluginHostControl), pluginBaseAddress);
            serviceHost.AddServiceEndpoint(
                    typeof(IPluginHostControl),
                    new NetNamedPipeBinding(),
                    nameof(PluginHostControl));
            AddMetaDataExchangeTo(serviceHost);
            using (var host = new HostKeepAlive(serviceHost))
            {
                await host.Start();
            }
        }
        static void AddMetaDataExchangeTo(ServiceHost host)
        {
            var serviceMetadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if(null == serviceMetadataBehavior)
            {
                serviceMetadataBehavior = new ServiceMetadataBehavior();
                host.Description.Behaviors.Add(serviceMetadataBehavior);
            }
            host.AddServiceEndpoint(
                typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                "mex");
        }

        static string GetRawArg()
        {
            var assemblyName =
                Path.GetFileName(typeof(HostManager).Assembly.Location);
            var commandLine =
                Environment.CommandLine;

            var arg0 = Environment.GetCommandLineArgs()[0];
            if(arg0.Contains(assemblyName))
            {
                var index = commandLine.IndexOf(arg0);
                commandLine = commandLine
                    .Substring(index + arg0.Length)
                    .TrimStart(' ', '"')
                    .TrimEnd();
            }
            return commandLine;
        }
    }
}
