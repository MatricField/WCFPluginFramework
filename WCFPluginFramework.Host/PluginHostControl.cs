using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFPluginFramework.Host
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PluginHostControl" in both code and config file together.
    public class PluginHostControl : IPluginHostControl
    {
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
            Debug.WriteLine($"Shutting down"); 
            #endif
            Program.Shutdown();
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
            serviceHost.PrintEndPoints();
            serviceHost.AddServiceEndpoint(
                typeof(IPluginHostControl),
                new NetNamedPipeBinding(),
                nameof(PluginHostControl)
                );
            serviceHost.PrintEndPoints();
            serviceHost.AddMetaDataExchange(pluginBaseAddress);
            serviceHost.PrintEndPoints();
            return serviceHost;
        }
    }
}
