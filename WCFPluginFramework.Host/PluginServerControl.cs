using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using WCFPluginFramework.Metadata;

namespace WCFPluginFramework.Host
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class PluginServerControl : IPluginServerControl
    {
        private ConcurrentBag<ServiceHost> PluginHosts = new ConcurrentBag<ServiceHost>();

        private ConcurrentDictionary<Guid, PluginDescription> PluginDescriptions = 
            new ConcurrentDictionary<Guid, PluginDescription>();

        private Uri[] ServerBaseAddresses;

        private PluginServerControl(params Uri[] pluginBaseAddress)
        {
            ServerBaseAddresses = pluginBaseAddress;
        }

        public int HeartBeat(int data)
        {
            #if DEBUG
            Console.WriteLine($"Heart beat: {data}");
            #endif
            return data;
        }

        public void Shutdown()
        {
            #if DEBUG
            Console.WriteLine($"Shutting down"); 
            #endif
            Program.Shutdown();
        }

        public IEnumerable<PluginDescription> EnumerateAvailablePlugins()
        {
            var ret = Enumerable.Empty<PluginDescription>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                ret = Enumerable.Concat(ret, EnumerateAvailablePlugins(asm));
            }
            return ret;
        }

        public void LoadPluginAssembly(string path)
        {
            var asm = Assembly.LoadFile(path);
            foreach (var description in EnumerateAvailablePlugins(asm))
            {
                var host = ServiceHostFactory.CreatePlugin(description, ServerBaseAddresses);
                host.Open();
                PluginHosts.Add(host);
            }
        }

        private IEnumerable<PluginDescription> EnumerateAvailablePlugins(Assembly assembly)
        {
            foreach (var type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<PluginDescriptionAttribute>() != null && !type.IsGenericType)
                {
                    Console.WriteLine($"Loading plugin info of {type.Name}");
                    var provider = Activator.CreateInstance(type) as IPluginDescriptionProvider;
                    if (provider != null)
                    {
                        yield return provider.Description;
                    }
                }
            }
        }

        public IDictionary<Uri, string> EnumerateEndPoints()
        {
            var dict = new Dictionary<Uri, string>();
            foreach(var h in PluginHosts)
            {
                foreach(var endpoint in h.Description.Endpoints)
                {
                    dict.Add(endpoint.Address.Uri, endpoint.Contract.ContractType.AssemblyQualifiedName);
                }
            }
            return dict;
        }

        public static ServiceHost CreateService(params Uri[] pluginBaseAddress)
        {
            var hostControl = new PluginServerControl(pluginBaseAddress);
            var serviceHost = new ServiceHost(hostControl, pluginBaseAddress);
            serviceHost.AddServiceEndpoint(
                typeof(IPluginServerControl),
                new NetNamedPipeBinding(),
                nameof(PluginServerControl)
                );
            serviceHost.PrintEndPoints();

            Console.WriteLine($"pluginBaseAddress:{pluginBaseAddress}");

            return serviceHost;
        }
    }
}
