using WCFPluginFramework.Metadata;

namespace WCFPluginFramework
{
    /// <summary>
    /// Create a derived class from this class
    /// to add a plugin description to assembly
    /// so that the plugin host can locate it
    /// </summary>
    /// <typeparam name="TPlugin">Class that implements a plugin</typeparam>
    [PluginDescription]
    public abstract class PluginDescriptionBase<TPlugin>:
        IPluginDescriptionProvider
    {
        public PluginDescription Description { get; }

        protected PluginDescriptionBase()
        {
            var pluginBuilder = PluginDescriptionBuilder.Create<TPlugin>();
            foreach (var contract in typeof(TPlugin).GetInterfaces())
            {
                var contractBuilder = new ServiceContractDescriptionBuilder(contract);
                pluginBuilder.ContractSet.Add(contractBuilder.ToDescription());
            }
            Description = pluginBuilder.ToPluginDescription();
        }
    }

}
