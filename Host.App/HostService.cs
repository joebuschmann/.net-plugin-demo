using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Host.Contract;
using Host.Contract.Log;

namespace Host.App
{
    internal class HostService
    {
        private readonly ILogger _logger;
        private readonly List<PluginContext> _applications = new List<PluginContext>();
        private readonly string _currentDirectory;

        public HostService(ILogger logger)
        {
            _logger = logger;
            var fileInfo = new FileInfo(Assembly.GetEntryAssembly().Location);
            _currentDirectory = fileInfo.DirectoryName;
        }

        public void OnStart(string[] args)
        {
            var configuration = PluginConfigurationSection.Instance;

            _logger.Write($"Loading plugins from the directory: {_currentDirectory}");
            _logger.Write($"Found {configuration.Plugins.Count} plugin(s).");

            foreach (PluginConfigurationElement plugin in configuration.Plugins)
            {
                LogPluginInformation(plugin);

                PluginContext pluginContext = LoadPlugin(plugin);

                _applications.Add(pluginContext);

                Task.Run(() => pluginContext.Service?.OnStart(_logger))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            var summary = $"An exception was thrown when starting the plugin {plugin.Description}.";
                            _logger.Write(summary, t.Exception);
                        }
                        else if (t.IsCompleted)
                        {
                            _logger.Write($"{plugin.Description} started successfully.");
                        }
                    });
            }
        }

        public void OnStop()
        {
            _logger.Write($"Stopping {_applications.Count} plugin(s).");

            foreach (var application in _applications)
            {
                var name = application.AppDomain.FriendlyName;

                _logger.Write($"Stopping {name}.");

                try
                {
                    application.Service?.OnStop();
                    AppDomain.Unload(application.AppDomain);

                    _logger.Write($"{name} stopped successfully.");
                }
                catch (Exception e)
                {
                    _logger.Write($"{name} failed to stop.", e);
                }
            }
        }

        private PluginContext LoadPlugin(PluginConfigurationElement pluginConfiguration)
        {
            try
            {
                string baseDirectory = pluginConfiguration.BaseDirectory;

                if (string.IsNullOrEmpty(Path.GetPathRoot(baseDirectory)) || Path.GetPathRoot(baseDirectory) == @"\")
                {
                    // Relative path. Append current directory.
                    baseDirectory = Path.Combine(_currentDirectory, baseDirectory);
                }

                if (!Directory.Exists(baseDirectory))
                {
                    throw new DirectoryNotFoundException(baseDirectory);
                }

                var appDomainSetup = new AppDomainSetup
                {
                    ApplicationBase = baseDirectory,
                    ApplicationName = pluginConfiguration.Description,
                    ConfigurationFile = pluginConfiguration.ConfigurationFile
                };

                var appDomain =
                    AppDomain.CreateDomain(pluginConfiguration.Description, new Evidence(), appDomainSetup);

                PluginService service = (PluginService)appDomain.CreateInstanceAndUnwrap(pluginConfiguration.Assembly,
                    pluginConfiguration.Type);

                return new PluginContext(appDomain, service);
            }
            catch (Exception e)
            {
                var summary = $"Failed to load the plugin {pluginConfiguration.Description}.";
                _logger.Write(summary, e);
                throw;
            }
        }

        private void LogPluginInformation(PluginConfigurationElement pluginConfiguration)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("=====================================");
            stringBuilder.AppendLine($"Description: {pluginConfiguration.Description}");
            stringBuilder.AppendLine($"Assembly: {pluginConfiguration.Assembly}");
            stringBuilder.AppendLine($"Type: {pluginConfiguration.Type}");
            stringBuilder.AppendLine($"Base Directory: {pluginConfiguration.BaseDirectory}");
            stringBuilder.AppendLine($"Configuration File: {pluginConfiguration.ConfigurationFile}");
            stringBuilder.AppendLine("=====================================");
            stringBuilder.AppendLine();

            _logger.Write(stringBuilder.ToString());
        }
    }
}
