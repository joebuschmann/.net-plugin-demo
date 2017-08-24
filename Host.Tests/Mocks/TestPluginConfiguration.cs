using System.Collections.Generic;
using System.Linq;
using Host.App.Configuration;

namespace Host.Tests.Mocks
{
    internal class TestPluginConfiguration : IPluginConfiguration
    {
        private readonly List<IPluginConfigurationItem> _pluginConfigurationItems =
            new List<IPluginConfigurationItem>();

        public void AddConfigurationItem(IPluginConfigurationItem pluginConfigurationItem)
        {
            _pluginConfigurationItems.Add(pluginConfigurationItem);
        }

        public IEnumerable<IPluginConfigurationItem> Plugins
        {
            get { return _pluginConfigurationItems.ToList(); }
        }
    }
}
