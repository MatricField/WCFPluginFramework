using System;
using System.IO;
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

        public static async Task Main(string[] args)
        {
            Console.WriteLine("Host started");
            Console.WriteLine("Command Line args:");
            foreach(var arg in args)
            {
                Console.WriteLine(arg);
            }
            KeepAlive = new SemaphoreSlim(0, 1);
            using (var controlService = PluginServerControl.CreateService(GetBaseAddress()))
            {
                controlService.Open();
                Console.WriteLine("Control Service started");
                await KeepAlive.WaitAsync();
                controlService.Close();
            }
        }

        private static Uri GetBaseAddress()
        {
            var assemblyName =
                Path.GetFileName(typeof(PluginServerControl).Assembly.Location);
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
