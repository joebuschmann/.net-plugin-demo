namespace Host.App.Configuration
{
    public interface IPluginConfigurationItem
    {
        string Assembly { get; }
        string BaseDirectory { get; }
        string ConfigurationFile { get; }
        string Description { get; }
        string Type { get; }
    }
}