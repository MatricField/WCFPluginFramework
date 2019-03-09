using System;
using System.Runtime.Serialization;

namespace WCFPluginFramework.Common
{
    [DataContract]
    public class PluginInfo
    {
        public string DisplayName { get; set; }

        public PluginAuthorInfo Author { get; set; }

        public string Description { get; set; }
    }
}
