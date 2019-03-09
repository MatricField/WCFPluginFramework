using System;
using System.Diagnostics;
using System.IO;
using WCFPluginFramework.Host;
using WCFPluginFramework.Common;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace TestConsole
{
    class TestProgram
    {
        const string NamedPipeBaseUri =
            "net.pipe://localhost/";
        static async Task Main()
        {
            using (var job = new Job())
            {
                var pluginBaseAddress =
                new Uri(NamedPipeBaseUri + Guid.NewGuid() + "/");
                var hostProcess = RunHost(pluginBaseAddress);
                job.AddProcess(hostProcess);
                Console.WriteLine($"Host process started PID={hostProcess.Id}");

                var pluginHostControl = await ConnectToHostControllerAsync(pluginBaseAddress);
                var cancellation = new CancellationTokenSource();
                var heartbeat = RunHeartBeatLoop(pluginHostControl, cancellation.Token);
                await WaitForAnyKey();

                await LoadPluginAssemblyAndListPlugins(pluginHostControl);
                await WaitForAnyKey();

                await QueryAndTestPlugins(pluginHostControl);
                await WaitForAnyKey();

                cancellation.Cancel();
                try
                {
                    await heartbeat;
                }
                catch (OperationCanceledException) { }
                await pluginHostControl.ShutdownAsync();
                Console.WriteLine("Shutting down");
                hostProcess.WaitForExit();
            }
        }

        static async Task QueryAndTestPlugins(IPluginHostControlAsync pluginHost)
        {
            var tasks = new List<Task>();
            foreach(var (uri, qualifiedName) in await pluginHost.EnumerateEndPointsAsync())
            {
                if(qualifiedName == typeof(IPlugin).AssemblyQualifiedName)
                {
                    var plugin = await ConnectAsync<IPluginAsync>(uri);
                    tasks.Add(TestPlugin(plugin));
                }
            }
            await Task.WhenAll(tasks);
        }

        static async Task TestPlugin(IPluginAsync plugin)
        {
            var cancellation = new CancellationTokenSource();
            var heartbeat = RunHeartBeatLoop(plugin, cancellation.Token);

            var info = await plugin.GetPluginInfoAsync();

            Console.WriteLine(info);
            await Task.Delay(TimeSpan.FromMinutes(1));

            cancellation.Cancel();
            try
            {
                await heartbeat;
            }
            catch (OperationCanceledException) { }
        }

        static async Task LoadPluginAssemblyAndListPlugins(IPluginHostControlAsync pluginHost)
        {
            await pluginHost.LoadPluginAssemblyAsync(typeof(TestPlugin.Plugin1).Assembly.Location);
            var availablePlugins = await pluginHost.EnumerateAvailablePluginsAsync();
            Console.WriteLine("Available plugins");
            foreach (var plugin in availablePlugins)
            {
                Console.WriteLine($"{plugin.ServiceName}:");
                Console.WriteLine($"Implementation: {plugin.ImplementationName}");
                Console.WriteLine("Contracts:");
                foreach (var contrct in plugin.Contracts)
                {
                    Console.WriteLine($"    {contrct.ContractInterfaceName}");
                    Console.WriteLine($"    Endpoints:");
                    foreach(var endpoint in contrct.EndPoints)
                    {
                        Console.WriteLine($"        {endpoint}");
                    }
                }
                Console.WriteLine();
            }
        }

        static async Task WaitForAnyKey()
        {
            Console.WriteLine("Press any key to continue...");
            while (!Console.KeyAvailable)
            {
                await Task.Delay(30);
            }
            Console.ReadKey();
        }

        static async Task RunHeartBeatLoop(IHeartBeatServiceAsync heartBeatService, CancellationToken token)
        {
            var rand = new Random();
            for (; ; )
            {
                token.ThrowIfCancellationRequested();
                var delayPeriod = Task.Delay(TimeSpan.FromSeconds(1));
                var beat = rand.Next();
                var responseTask = heartBeatService.HeartBeatAsync(beat);
                var any = await Task.WhenAny(responseTask, Task.Delay(TimeSpan.FromMilliseconds(100)));
                if (any == responseTask)
                {
                    var repeat = await responseTask;
                    if (repeat != beat)
                    {
                        Console.WriteLine($"Heart beat error: expected {beat}, but returned {repeat}");
                    }
                    //else
                    //{
                    //    Console.WriteLine($"Heart beat: {beat}");
                    //}
                }
                else
                {
                Console.WriteLine("Heart beat timed out");
                }
                await delayPeriod;
            }
        }

        static async Task<IContractT> ConnectAsync<IContractT>(Uri address)
            where IContractT : IHeartBeatServiceAsync
        {
            var factory = 
                new ChannelFactory<IContractT>(
                    new NetNamedPipeBinding(),
                    new EndpointAddress(address));
            const int MaxRetry = 5;
            const int RetryDelay = 1;
            var rand = new Random();
            for (int i = 0; i < MaxRetry; ++i)
            {
                var delay = Task.Delay(TimeSpan.FromSeconds(RetryDelay));
                var proxy = factory.CreateChannel();
                var beat = rand.Next();
                try
                {
                    if (await proxy.HeartBeatAsync(beat) == beat)
                    {
                        return proxy;
                    }
                }
                catch (EndpointNotFoundException)
                {
                    Console.WriteLine("Endpoint not available, retrying...");
                    await delay;
                }
            }
            throw new CommunicationException();
        }

        static Task<IPluginHostControlAsync> ConnectToHostControllerAsync(Uri pluginBaseAddress)
        {
            return ConnectAsync<IPluginHostControlAsync>(new Uri(pluginBaseAddress, nameof(PluginHostControl)));
            //var binding = new NetNamedPipeBinding();
            //var address = new EndpointAddress(new Uri(pluginBaseAddress, nameof(PluginHostControl)));
            //var factory = new ChannelFactory<IPluginHostControlAsync>(binding, address);
            //const int MaxRetry = 5;
            //const int RetryDelay = 1;
            //var rand = new Random();
            //for (int i = 0; i < MaxRetry; ++i)
            //{
            //    var delay = Task.Delay(TimeSpan.FromSeconds(RetryDelay));
            //    var proxy = factory.CreateChannel();
            //    var beat = rand.Next();
            //    try
            //    {
            //        if(await proxy.HeartBeatAsync(beat) == beat)
            //        {
            //            return proxy;
            //        }
            //    }
            //    catch (EndpointNotFoundException)
            //    {
            //        Console.WriteLine("Endpoint not available, retrying...");
            //        await delay;
            //    }
            //}
            //throw new CommunicationException();
        }

        static Process RunHost(Uri pluginBaseAddress)
        {
            var path = typeof(PluginHostControl).Assembly.Location;
            var info =
                new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = Path.GetFileName(path),
                    WorkingDirectory = Path.GetDirectoryName(path),
                    Arguments = pluginBaseAddress.ToString()
                };
            return Process.Start(info);
        }
    }
}
