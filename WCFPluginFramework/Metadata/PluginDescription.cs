using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WCFPluginFramework.Metadata
{
    [DataContract]
    public sealed class PluginDescription
    {
        /// <summary>
        /// Assembly qualified name of implementation class
        /// </summary>
        [IgnoreDataMember]
        public string ImplementationName { get; set; }

        /// <summary>
        /// Plugin host will listen for communication under this relative path
        /// </summary>
        /// <example>
        /// baseUri = http://localhost/PluginHostID/
        /// EndPointRelativePath = ExamplePlugin
        /// host will listen for communication under
        /// http://localhost/PluginHostID/ExamplePlugin/
        /// </example>
        [DataMember]
        public string ServiceName { get; set; }

        [DataMember]
        public IEnumerable<ServiceContractDescription> Contracts { get; set; }
    }
}
