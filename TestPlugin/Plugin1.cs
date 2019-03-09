using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFPluginFramework;

namespace TestPlugin
{
    public class Plugin1 :
        IPlugin
    {
        public PluginCapability GetCapabilities()
        {
            return PluginCapability.None;
        }

        public int HeartBeat(int data)
        {
            return data;
        }
    }
}
