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
        private readonly List<ApplicationContext> _applications = new List<ApplicationContext>();
        private readonly string _currentDirectory;

        public HostService()
        {
            var fileInfo = new FileInfo(Assembly.GetEntryAssembly().Location);
            _currentDirectory = fileInfo.DirectoryName;
        }

        public override void OnStart(string[] args)
        {
            // Filewatcher
            var applicationStartupDetails = new ApplicationStartupDetails()
            {
                Name = "File Watcher",
                BaseDirectory = _currentDirectory + @"\apps\Plugin.FileWatcher",
                ConfigurationFile = "Plugin.FileWatcher.dll.config",
                AssemblyName = "Plugin.FileWatcher",
                TypeName = "Plugin.FileWatcher.FileWatcherService"
            };

            ApplicationContext application = LoadApplication(applicationStartupDetails);

            _applications.Add(application);

            Task.Run(() => application.Service?.OnStart(args));

            // Http Server
            applicationStartupDetails = new ApplicationStartupDetails()
            {
                Name = "HTTP Server",
                BaseDirectory = _currentDirectory + @"\apps\Plugin.HttpServer",
                ConfigurationFile = "Plugin.HttpServer.dll.config",
                AssemblyName = "Plugin.HttpServer",
                TypeName = "Plugin.HttpServer.HttpService"
            };

            application = LoadApplication(applicationStartupDetails);

            _applications.Add(application);

            Task.Run(() => application.Service?.OnStart(args));
        }

        public override void OnStop()
        {
            foreach (var application in _applications)
            {
                application.Service?.OnStop();
                AppDomain.Unload(application.AppDomain);
            }
        }

        private ApplicationContext LoadApplication(ApplicationStartupDetails applicationStartupDetails)
        {
            var appDomainSetup = new AppDomainSetup
            {
                ApplicationBase = applicationStartupDetails.BaseDirectory,
                ApplicationName = applicationStartupDetails.Name,
                ConfigurationFile = applicationStartupDetails.ConfigurationFile
            };

            var appDomain =
                AppDomain.CreateDomain(applicationStartupDetails.Name, new Evidence(), appDomainSetup);

            PluginService service = (PluginService) appDomain.CreateInstanceAndUnwrap(applicationStartupDetails.AssemblyName,
                applicationStartupDetails.TypeName);

            return new ApplicationContext(appDomain, service);
        }
    }
}
