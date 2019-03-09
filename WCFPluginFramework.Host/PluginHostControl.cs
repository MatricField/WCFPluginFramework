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
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class PluginHostControl : IPluginHostControl
    {
        private ConcurrentDictionary<Guid, PluginHost> HostedPlugins =
            new ConcurrentDictionary<Guid, PluginHost>();

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
            try
            {
                var ret = new List<PluginDescription>();
                foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.GetCustomAttribute<PluginDescriptionAttribute>() != null && !type.IsGenericType)
                        {
                            Console.WriteLine($"Loading plugin info of {type.Name}");
                            var provider = Activator.CreateInstance(type) as IPluginDescriptionProvider;
                            if (provider != null)
                            {
                                ret.Add(provider.Description);
                            }
                        }
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                return Enumerable.Empty<PluginDescription>();
            }
        }

        public void LoadPluginAssembly(string path)
        {
            try
            {
                var asm = Assembly.LoadFile(path);
            }
            catch (Exception ex)
            {
                switch(ex)
                {
                    case ArgumentNullException _:
                    case ArgumentException _:
                    case FileLoadException _:
                    case FileNotFoundException _:
                    case BadImageFormatException _:
                        ex.ThrowFaultedExceptionFromThis();
                        break;
                    default:
                        throw;
                }
            }
        }

        public static ServiceHost MakePluginHostControlService()
        {
            string GetRawArg()
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
                return commandLine;
            }
            var pluginBaseAddress =
                new Uri(GetRawArg());
            Console.WriteLine($"pluginBaseAddress:{pluginBaseAddress}");
            var serviceHost = new ServiceHost(typeof(PluginHostControl), pluginBaseAddress);
            serviceHost.AddServiceEndpoint(
                typeof(IPluginHostControl),
                new NetNamedPipeBinding(),
                nameof(PluginHostControl)
                );
            serviceHost.AddMetaDataExchange();
            serviceHost.PrintEndPoints();
            return serviceHost;
        }
    }
}
