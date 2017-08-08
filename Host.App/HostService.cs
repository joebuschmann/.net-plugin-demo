using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using Host.Contract;

namespace Host.App
{
    internal class HostService : PluginService
    {
        private readonly List<PluginContext> _applications = new List<PluginContext>();
        private readonly string _currentDirectory;

        public HostService()
        {
            var fileInfo = new FileInfo(Assembly.GetEntryAssembly().Location);
            _currentDirectory = fileInfo.DirectoryName;
        }

        public override void OnStart(string[] args)
        {
            var configuration = PluginConfigurationSection.Instance;

            foreach (PluginConfigurationElement plugin in configuration.Plugins)
            {
                PluginContext pluginContext = LoadPlugin(plugin);

                _applications.Add(pluginContext);

                Task.Run(() => pluginContext.Service?.OnStart(args));
            }
        }

        public override void OnStop()
        {
            foreach (var application in _applications)
            {
                application.Service?.OnStop();
                AppDomain.Unload(application.AppDomain);
            }
        }

        private PluginContext LoadPlugin(PluginConfigurationElement pluginConfiguration)
        {
            string baseDirectory = pluginConfiguration.BaseDirectory;
            
            if (string.IsNullOrEmpty(Path.GetPathRoot(baseDirectory)) || Path.GetPathRoot(baseDirectory) == @"\")
            {
                // Relative path. Append current directory.
                baseDirectory = Path.Combine(_currentDirectory, baseDirectory);
            }

            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = baseDirectory,
                ApplicationName = pluginConfiguration.Description,
                ConfigurationFile = pluginConfiguration.ConfigurationFile
            };

            var appDomain =
                AppDomain.CreateDomain(pluginConfiguration.Description, new Evidence(), appDomainSetup);

            PluginService service = (PluginService) appDomain.CreateInstanceAndUnwrap(pluginConfiguration.Assembly,
                pluginConfiguration.Type);

            return new PluginContext(appDomain, service);
        }
    }
}
