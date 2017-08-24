using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Host.App.Configuration;
using Host.Contract;
using Host.Contract.Log;

namespace Host.App
{
    internal class HostService
    {
        private readonly ILogger _logger;
        private readonly IPluginConfiguration _pluginConfiguration;
        private readonly List<PluginContext> _applications = new List<PluginContext>();
        private readonly string _currentDirectory;

        public HostService(ILogger logger, IPluginConfiguration pluginConfiguration)
        {
            _logger = logger;
            _pluginConfiguration = pluginConfiguration;
            var fileInfo = new FileInfo(typeof(HostService).Assembly.Location);
            _currentDirectory = fileInfo.DirectoryName;
        }

        public void OnStart(string[] args)
        {
            _logger.Write($"Loading plugins from the directory: {_currentDirectory}");
            _logger.Write($"Found {_pluginConfiguration.Plugins.Count()} plugin(s).");

            var tasks = RunPlugins();

            // Wait on handles instead of Tasks. Task.WaitAll throws an exception if any of the tasks are faulted.
            WaitHandle[] waitHandles = tasks.Select(t => ((IAsyncResult) t).AsyncWaitHandle).ToArray();
            WaitHandle.WaitAll(waitHandles);

            HandleExceptions(tasks);
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
                    application.Service?.OnStop(_logger);
                    AppDomain.Unload(application.AppDomain);

                    _logger.Write($"{name} stopped successfully.");
                }
                catch (Exception e)
                {
                    _logger.Write($"{name} failed to stop.", e);
                }
            }
        }

        private List<Task> RunPlugins()
        {
            List<Task> tasks = new List<Task>();

            foreach (IPluginConfigurationItem plugin in _pluginConfiguration.Plugins)
            {
                LogPluginInformation(plugin);

                PluginContext pluginContext = LoadPlugin(plugin);

                _applications.Add(pluginContext);

                Task task = Task.Run(() => pluginContext.Service?.OnStart(_logger));
                tasks.Add(task);
            }
            return tasks;
        }

        private void HandleExceptions(List<Task> tasks)
        {
            List<Exception> exceptions = new List<Exception>();

            for (int i = 0; i < tasks.Count; i++)
            {
                Task task = tasks[i];
                IPluginConfigurationItem plugin = _pluginConfiguration.Plugins.ElementAt(i);

                if (task.IsFaulted)
                {
                    var summary = $"An exception was thrown when starting the plugin {plugin.Description}.";
                    _logger.Write(summary, task.Exception);
                    exceptions.AddRange(task.Exception.InnerExceptions);
                }
                else if (task.IsCompleted)
                {
                    _logger.Write($"{plugin.Description} started successfully.");
                }
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("One or more plugins failed to start. See inner exceptions for more details.",
                    exceptions);
            }
        }

        private PluginContext LoadPlugin(IPluginConfigurationItem pluginConfiguration)
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

        private void LogPluginInformation(IPluginConfigurationItem pluginConfiguration)
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
