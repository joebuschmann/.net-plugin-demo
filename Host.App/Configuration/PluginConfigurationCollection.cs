using System.Configuration;

namespace Host.App.Configuration
{
    [ConfigurationCollection(typeof(PluginConfigurationElement),
        CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class PluginConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PluginConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as PluginConfigurationElement).Description;
        }
    }
}