using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Host.Contract;

namespace Host.App
{
    internal class HostService : PluginService
    {
        private readonly Action<string> _logger;
        private readonly List<PluginContext> _applications = new List<PluginContext>();
        private readonly string _currentDirectory;

        public HostService(Action<string> logger)
        {
            _logger = logger;
            var fileInfo = new FileInfo(Assembly.GetEntryAssembly().Location);
            _currentDirectory = fileInfo.DirectoryName;
        }

        public override void OnStart(string[] args)
        {
            var configuration = PluginConfigurationSection.Instance;

            _logger($"Loading plugins from the directory: {_currentDirectory}");
            _logger($"Found {configuration.Plugins.Count} plugin(s).");

            foreach (PluginConfigurationElement plugin in configuration.Plugins)
            {
                LogPluginInformation(plugin);

                PluginContext pluginContext = LoadPlugin(plugin);

                _applications.Add(pluginContext);

                Task.Run(() => pluginContext.Service?.OnStart(args))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            var summary = $"An exception was thrown when starting the plugin {plugin.Description}.";
                            LogException(summary, t.Exception?.InnerException);
                        }
                        else if (t.IsCompleted)
                        {
                            _logger($"{plugin.Description} started successfully.");
                        }
                    });
            }
        }

        public override void OnStop()
        {
            _logger($"Stopping {_applications.Count} plugin(s).");

            foreach (var application in _applications)
            {
                var name = application.AppDomain.FriendlyName;

                _logger($"Stopping {name}.");

                try
                {
                    application.Service?.OnStop();
                    AppDomain.Unload(application.AppDomain);

                    _logger($"{name} stopped successfully.");
                }
                catch (Exception e)
                {
                    LogException($"{name} failed to stop.", e);
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
                LogException(summary, e);
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

            _logger(stringBuilder.ToString());
        }

        private void LogException(string summary, Exception e)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.AppendLine(summary);
            stringBuilder.AppendLine();

            if (e == null)
            {
                stringBuilder.AppendLine("Unable to retrieve exception details.");
            }
            else
            {
                stringBuilder.AppendLine("Exception:");
                stringBuilder.AppendLine(e.Message);
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("Stack trace:");
                stringBuilder.AppendLine(e.StackTrace);
            }

            _logger(stringBuilder.ToString());
        }
    }
}
