using System.Runtime.Serialization;

namespace WCFPluginFramework.Common
{
    [DataContract]
    public class PluginAuthorInfo
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Contact { get; set; }

        public override string ToString()
        {
            return
                $"Name: {Name}\nContact:{Contact}";
        }
    }
}
