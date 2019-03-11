using System;

namespace WCFPluginFramework.Metadata
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PluginDescriptionAttribute:
        Attribute
    {
    }
}
