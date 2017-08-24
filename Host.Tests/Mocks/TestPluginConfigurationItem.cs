using Host.App.Configuration;

namespace Host.Tests.Mocks
{
    internal class TestPluginConfigurationItem : IPluginConfigurationItem
    {
        public string Assembly { get; set; }
        public string BaseDirectory { get; set; }
        public string ConfigurationFile { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
