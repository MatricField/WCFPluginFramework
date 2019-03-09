using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using WCFPluginFramework.Host;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Threading;

namespace TestConsole
{
    class TestConsole
    {
        const string NamedPipeBaseUri =
            "net.pipe://localhost/";
        static async Task Main()
        {
            var pluginBaseAddress =
                new Uri(NamedPipeBaseUri + Guid.NewGuid() + "/");
            var hostProcess = RunHost(pluginBaseAddress);
            var pluginHostControl = ConnectToHostController(pluginBaseAddress);
            var cancellation = new CancellationTokenSource();
            var heartbeat = RunHeartBeatLoop(pluginHostControl, cancellation.Token);
            Console.WriteLine("Host running, press any key to shutdown");
            Console.ReadKey();
            cancellation.Cancel();
            try
            {
                await heartbeat;
            }
            catch (OperationCanceledException) { }
            await pluginHostControl.ShutdownAsync();
            hostProcess.WaitForExit();
        }

        static async Task RunHeartBeatLoop(IPluginHostControlAsync pluginHostControl, CancellationToken token)
        {
            var rand = new Random();
            for (; ; )
            {
                token.ThrowIfCancellationRequested();
                var delayPeriod = Task.Delay(TimeSpan.FromSeconds(1));
                var beat = rand.Next();
                var responseTask = pluginHostControl.HeartBeatAsync(beat);
                var responseOrTimedOut =
                    await Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromMilliseconds(700)));
                if (responseOrTimedOut == responseTask)
                {
                    var repeat = await responseTask;
                    if (repeat != beat)
                    {
                        Console.WriteLine($"Heart beat error: expected {beat}, but returned {repeat}");
                    }
                    else
                    {
                        Console.WriteLine($"Heart beat: {beat}");
                    }
                }
                else
                {
                    Console.WriteLine("Heart beat timed out");
                }
                await delayPeriod;
            }
        }

        static IPluginHostControlAsync ConnectToHostController(Uri pluginBaseAddress)
        {
            var binding = new NetNamedPipeBinding();
            var address = new EndpointAddress(new Uri(pluginBaseAddress, nameof(PluginHostControl)));
            var factory = new ChannelFactory<IPluginHostControlAsync>(binding, address);
            return factory.CreateChannel();
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
