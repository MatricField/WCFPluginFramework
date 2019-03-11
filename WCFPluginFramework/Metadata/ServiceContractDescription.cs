using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WCFPluginFramework.Metadata
{
    [DataContract]
    public sealed class ServiceContractDescription
    {
        /// <summary>
        /// Plugin host will listen for communication under this relative path
        /// </summary>
        [DataMember]
        public IEnumerable<string> EndPoints { get; set; }

        /// <summary>
        /// Assembly qualified name of the contract
        /// </summary>
        [DataMember]
        public string ContractInterfaceName { get; set; }
    }
}
