using System;
using System.Runtime.Serialization;

namespace WCFPluginFramework.Common
{
    [DataContract]
    public class PluginInfo
    {
        [DataMember]
        public string DisplayName { get; set; }

        [DataMember]
        public PluginAuthorInfo Author { get; set; }

        [DataMember]
        public string Description { get; set; }

        public override string ToString()
        {
            return
                $"DisplayName: {DisplayName}\nAuthor:\n{Author}\nDescription:\n    {Description}";
        }
    }
}
