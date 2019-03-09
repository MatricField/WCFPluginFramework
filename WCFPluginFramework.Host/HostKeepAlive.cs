using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFPluginFramework.Host
{
    public class HostKeepAlive:
        IDisposable
    {
        private ServiceHost Host;
        private SemaphoreSlim HostCloseSingal;

        public HostKeepAlive(ServiceHost host)
        {
            Host = host;
            HostCloseSingal = new SemaphoreSlim(1, 1);
        }

        public async Task Start()
        {
            if(await HostCloseSingal.WaitAsync(0))
            {
                Host.Closed += Host_Closed;
                Host.Open();
                foreach(var address in Host.BaseAddresses)
                {
                    Debug.WriteLine($"Host running on {address}");
                }
                await HostCloseSingal.WaitAsync();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            HostCloseSingal.Release();
            Host.Closed -= Host_Closed;
        }

        public void Dispose()
        {
            ((IDisposable)Host).Dispose();
            HostCloseSingal.Dispose();
        }
    }
}
