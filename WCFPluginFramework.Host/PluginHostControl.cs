using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using WCFPluginFramework.Metadata;
using static WCFPluginFramework.Common.ExceptionExtension;

namespace WCFPluginFramework.Host
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true, InstanceContextMode = InstanceContextMode.Single)]
    public class PluginHostControl : IPluginHostControl
    {
        private ConcurrentBag<PluginHost> PluginHosts = new ConcurrentBag<PluginHost>();

        private Uri PluginBaseAddress;

        private PluginHostControl(Uri pluginBaseAddress)
        {
            PluginBaseAddress = pluginBaseAddress;
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
                var host = new PluginHost(description, PluginBaseAddress);
                host.ServiceHost.Open();
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
                foreach(var endpoint in h.Endpoints)
                {
                    dict.Add(endpoint.Address.Uri, endpoint.Contract.ContractType.AssemblyQualifiedName);
                }
            }
            return dict;
        }

        public static ServiceHost MakePluginHostControlService()
        {
            var pluginBaseAddress = GetBaseAddress();
            var hostControl = new PluginHostControl(pluginBaseAddress);
            var serviceHost = new ServiceHost(hostControl, pluginBaseAddress);
            serviceHost.AddServiceEndpoint(
                typeof(IPluginHostControl),
                new NetNamedPipeBinding(),
                nameof(PluginHostControl)
                );
            serviceHost.PrintEndPoints();

            Console.WriteLine($"pluginBaseAddress:{pluginBaseAddress}");

            return serviceHost;
        }

        private static Uri GetBaseAddress()
        {
            var assemblyName =
                Path.GetFileName(typeof(PluginHostControl).Assembly.Location);
            var commandLine =
                Environment.CommandLine;

            var arg0 = Environment.GetCommandLineArgs()[0];
            if (arg0.Contains(assemblyName))
            {
                var index = commandLine.IndexOf(arg0);
                commandLine = commandLine
                    .Substring(index + arg0.Length)
                    .TrimStart(' ', '"')
                    .TrimEnd();
            }
            return new Uri(commandLine);
        }
    }
}
