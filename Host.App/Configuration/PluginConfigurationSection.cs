using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Host.App.Configuration
{
    public class PluginConfigurationSection : ConfigurationSection, IPluginConfiguration
    {
        private const string PluginsName = "plugins";

        private static PluginConfigurationSection _instance = null;

        public static IPluginConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = ConfigurationManager.GetSection("pluginConfiguration") as PluginConfigurationSection;
                }

                return _instance;
            }
        }

        [ConfigurationProperty(PluginsName)]
        private PluginConfigurationCollection _plugins
        {
            get { return (PluginConfigurationCollection)this[PluginsName]; }
        }

        public IEnumerable<IPluginConfigurationItem> Plugins
        {
            get { return _plugins.Cast<IPluginConfigurationItem>(); }
        }
    }
}
