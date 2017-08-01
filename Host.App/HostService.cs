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
            var appDomainSetup = new AppDomainSetup();
            appDomainSetup.ApplicationBase = _currentDirectory + @"\apps\Sample.AppDomain.App1";
            appDomainSetup.ApplicationName = "App1";

            AppDomain appDomain = AppDomain.CreateDomain("App1", new Evidence(), appDomainSetup);
            _applications.Add(new ApplicationContext(appDomain, null));

            Task.Run(() => appDomain.ExecuteAssembly(appDomainSetup.ApplicationBase + "\\Sample.AppDomain.App1.exe",
                new string[2] { "foo", "bar" }));

            // Filewatcher
            var applicationStartupDetails = new ApplicationStartupDetails()
            {
                Name = "File Watcher",
                BaseDirectory = _currentDirectory + @"\apps\Sample.App.FileWatcher",
                ConfigurationFile = "Sample.App.FileWatcher.dll.config",
                AssemblyName = "Sample.App.FileWatcher",
                TypeName = "Sample.App.FileWatcher.FileWatcherService"
            };

            ApplicationContext application = LoadApplication(applicationStartupDetails);

            _applications.Add(application);
            application.Service?.OnStart(args);
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
