using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFPluginFramework.Metadata
{
    public interface IPluginDescriptionProvider
    {
        PluginDescription Description { get; }
    }
}
