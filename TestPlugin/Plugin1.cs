using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WCFPluginFramework;
using WCFPluginFramework.Common;

namespace TestPlugin
{
    public class Plugin1 :
        PluginBase
    {
        private static readonly PluginInfo INFO =
            new PluginInfo()
            {
                Author = new PluginAuthorInfo()
                {
                    Name = "MatrixField",
                    Contact = "Go talk to god"
                },
                Description = "Aaaaaaaaaah",
                DisplayName = "Plugin1"
            };

        private const PluginCapability CAPABILITY =
            PluginCapability.None;

        public override PluginInfo GetPluginInfo() => INFO;

        public override PluginCapability GetCapabilities() => CAPABILITY;
    }
}
