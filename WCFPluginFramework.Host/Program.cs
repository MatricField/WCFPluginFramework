using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFPluginFramework.Host
{
    public class Program
    {
        static SemaphoreSlim KeepAlive;

        public static void Shutdown()
        {
            KeepAlive.Release();
        }

        static async Task Main(string[] args)
        {
            Console.WriteLine("Host started");
            Console.WriteLine("Command Line args:");
            foreach(var arg in args)
            {
                Console.WriteLine(arg);
            }
            KeepAlive = new SemaphoreSlim(0, 1);
            using (var controlService = PluginHostControl.MakePluginHostControlService())
            {
                controlService.Open();
                Console.WriteLine("Control Service started");
                await KeepAlive.WaitAsync();
                controlService.Close();
            }
        }
    }
}
