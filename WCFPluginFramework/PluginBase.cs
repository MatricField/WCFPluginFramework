using System;
using System.ServiceModel;
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

        public abstract PluginCapability GetCapabilities();

        public abstract PluginInfo GetPluginInfo();

        public int HeartBeat(int data) => data;
    }
}
