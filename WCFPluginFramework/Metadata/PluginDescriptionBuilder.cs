using System;
using System.Collections.Generic;
using WCFPluginFramework.Common;

namespace WCFPluginFramework.Metadata
{
    public class PluginDescriptionBuilder
    {
        public Type Implementation { get; }

        public string ServiceName { get; set; }

        public HashSet<ServiceContractDescription> ContractSet { get; } =
            new HashSet<ServiceContractDescription>();

        public PluginDescriptionBuilder(Type implType)
        {
            Implementation = implType;
            ServiceName = implType.Name;
        }

        public PluginDescription ToPluginDescription()
        {
            var contracts = new List<ServiceContractDescription>(ContractSet);
            return new PluginDescription()
            {
                ImplementationName = Implementation.AssemblyQualifiedName,
                ServiceName = ServiceName,
                Contracts = contracts
            };
        }

        public static PluginDescriptionBuilder Create<TImpl>()
        {
            return new PluginDescriptionBuilder(typeof(TImpl));
        }
    }
}
