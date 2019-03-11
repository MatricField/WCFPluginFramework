using System;
using System.Collections;
using System.Collections.Generic;

namespace WCFPluginFramework.Metadata
{
    public class ServiceContractDescriptionBuilder :
        IEnumerable
    {
        public string DefaultEndPoint =>
            ContractInterface.Name;

        public HashSet<string> EndPointSet { get; } = new HashSet<string>();

        public Type ContractInterface { get; }

        public ServiceContractDescriptionBuilder(Type contractInterface)
        {
            ContractInterface = contractInterface;
        }

        public void Add(string endpoint)
        {
            EndPointSet.Add(endpoint);
        }

        public static ServiceContractDescriptionBuilder Create<IServiceContract>()
        {
            return new ServiceContractDescriptionBuilder(typeof(IServiceContract));
        }

        public ServiceContractDescription ToDescription()
        {
            var endPoints = new List<string>(EndPointSet);
            if(endPoints.Count == 0)
            {
                endPoints.Add(DefaultEndPoint);
            }
            return new ServiceContractDescription()
            {
                ContractInterfaceName = ContractInterface.AssemblyQualifiedName,
                EndPoints = endPoints
            };
        }

        /// <summary>
        /// Dummy method added to support collection initialization
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }
    }
}
