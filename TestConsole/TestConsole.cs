using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using WCFPluginFramework.Host;

namespace TestConsole
{
    class TestConsole
    {
        const string NamedPipeBaseUri =
            "net.pipe://localhost/";
        static void Main(string[] args)
        {
            var pluginBaseAddress =
                new Uri(NamedPipeBaseUri + Guid.NewGuid() + "/");
            var hostProcess = RunHost(pluginBaseAddress);
            hostProcess.WaitForExit();
        }

        static Process RunHost(Uri pluginBaseAddress)
        {
            var info =
                new ProcessStartInfo()
                {
                    UseShellExecute = false,
                    FileName = "WCFPluginFramework.Host.exe",
                    Arguments = pluginBaseAddress.ToString(),
                    //WindowStyle = ProcessWindowStyle.Hidden
                };
            return Process.Start(info);
        }
    }
}
