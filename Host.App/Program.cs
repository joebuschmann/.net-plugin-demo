using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using Host.App.Configuration;
using Host.Contract.Log;

namespace Host.App
{
    public class Program
    {
        private readonly bool _isConsoleApplication;
        private readonly ILogger _logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            bool isConsoleApplication = (args.Length > 0 && args.Any(arg => arg == "/console"));

            Program program = new Program(isConsoleApplication);
            program.Run(args);
        }

        public Program(bool isConsoleApplication)
        {
            _isConsoleApplication = isConsoleApplication;
            _logger = CreateLogger();
        }

        private void Run(string[] args)
        {
            if (_isConsoleApplication)
            {
                _logger.Write("Launching host as a console application.");
                LaunchConsoleApplication(args);
            }
            else
            {
                _logger.Write("Launching host as a Windows Service.");
                LaunchWindowsService();
            }
        }

        private ILogger CreateLogger()
        {
            if (_isConsoleApplication)
            {
                return new ConsoleLogger();
            }

            string source = ConfigurationManager.AppSettings["EventLog.Source"] ?? "Plugin-Demo";
            int eventId;

            if (!int.TryParse(ConfigurationManager.AppSettings["EventLog.ID"], out eventId))
            {
                eventId = 25;
            }

            return new EventLogLogger(source, eventId);
        }

        private void LaunchWindowsService()
        {
            var servicesToRun = new ServiceBase[]
            {
                new WindowsHostService(_logger)
            };

            ServiceBase.Run(servicesToRun);
        }

        private void LaunchConsoleApplication(string[] args)
        {
            try
            {
                HostService hostService = new HostService(_logger, PluginConfigurationSection.Instance);
                hostService.OnStart(args);

                Console.WriteLine("Services running. Press enter to shutdown the services and exit.");
                Console.ReadLine();

                hostService.OnStop();
            }
            catch (Exception e)
            {
                Console.WriteLine("An exception was thrown by the application.");
                Console.WriteLine();
                Console.WriteLine("Exception:");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("Stack trace:");
                Console.WriteLine(e.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
