using System.Runtime.Serialization;

namespace WCFPluginFramework.Common
{
    [DataContract]
    public class PluginAuthorInfo
    {
        public string Name { get; set; }

        public string Contact { get; set; }
    }
}
