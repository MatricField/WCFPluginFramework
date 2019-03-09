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

        static async Task Main()
        {
            KeepAlive = new SemaphoreSlim(0, 1);
            using (var controlService = PluginHostControl.MakePluginHostControlService())
            {
                controlService.Open();
                await KeepAlive.WaitAsync();
                controlService.Close();
            }
        }
    }
}
