using System.Collections.Generic;

namespace Host.App.Configuration
{
    public interface IPluginConfiguration
    {
        IEnumerable<IPluginConfigurationItem> Plugins { get; }
    }
}
