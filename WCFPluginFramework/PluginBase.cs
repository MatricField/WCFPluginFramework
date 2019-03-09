using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFPluginFramework.Common;

namespace WCFPluginFramework
{
    /// <summary>
    /// Extend this class to create a plugin
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public abstract class PluginBase :
        IPlugin
    {
        protected abstract PluginInfo PluginInfo { get; }

        protected abstract PluginCapability PluginCapability { get; }

        public void Connect()
        {
            try
            {
                DoConnect();
            }
            catch(Exception ex)
            {
                ex.ThrowFaultedExceptionFromThis();
            }
        }

        /// <summary>
        /// Override to add custom connect logic.
        /// No operation by default
        /// </summary>
        protected virtual void DoConnect()
        {

        }

        public void Disconnect()
        {
            try
            {
                DoDisconnect();
            }
            catch (Exception ex)
            {
                ex.ThrowFaultedExceptionFromThis();
            }
        }

        /// <summary>
        /// Override to add custom disconnect logic.
        /// No operation by default
        /// </summary>
        protected virtual void DoDisconnect()
        {

        }

        public PluginCapability GetCapabilities() => PluginCapability;

        public PluginInfo GetPluginInfo() => PluginInfo;

        public int HeartBeat(int data) => data;
    }
}
