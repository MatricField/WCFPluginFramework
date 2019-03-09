using System;
using System.Collections.Generic;
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
            return data;
        }

        public void Shutdown()
        {
            Console.WriteLine("Shutting down");
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
            var serviceHost = new ServiceHost(typeof(PluginHostControl), pluginBaseAddress);
            serviceHost.AddServiceEndpoint(
                    typeof(IPluginHostControl),
                    new NetNamedPipeBinding(),
                    //new Uri(pluginBaseAddress, nameof(PluginHostControl)));
                    nameof(PluginHostControl));
            serviceHost.AddMetaDataExchange(pluginBaseAddress);
            return serviceHost;
        }
    }
}
