using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Json;
using WCFPluginFramework.Host;
using WCFPluginFramework.Common;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Threading;

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
                Console.WriteLine("Connected, Press Cancel key to continue");
                var cancellation = new CancellationTokenSource();
                var heartbeat = RunHeartBeatLoop(pluginHostControl, cancellation.Token);
                //await WaitForCancelKey();

                await LoadPluginAssemblyAndListPlugins(pluginHostControl);
                Console.WriteLine("Press Cancel key to continue");
                await WaitForCancelKey();

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

        static async Task LoadPluginAssemblyAndListPlugins(IPluginHostControlAsync pluginHost)
        {
            try
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
            catch(FaultException<SerializableException> ex)
            {
                var oex = Activator.CreateInstance(ex.Detail.OriginalException) as Exception;
                throw oex;
            }
        }

        static async Task WaitForCancelKey()
        {
            using (var singal = new SemaphoreSlim(0, 1))
            {
                void OnCancel(object o, ConsoleCancelEventArgs e)
                {
                    singal.Release();
                    e.Cancel = true;
                }
                Console.CancelKeyPress += OnCancel;
                await singal.WaitAsync();
                Console.CancelKeyPress -= OnCancel;
            }
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

        static async Task<IPluginHostControlAsync> ConnectToHostControllerAsync(Uri pluginBaseAddress)
        {
            var binding = new NetNamedPipeBinding();
            var address = new EndpointAddress(new Uri(pluginBaseAddress, nameof(PluginHostControl)));
            var factory = new ChannelFactory<IPluginHostControlAsync>(binding, address);
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
                    if(await proxy.HeartBeatAsync(beat) == beat)
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
