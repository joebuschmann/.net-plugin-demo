using System;
using System.Linq;
using System.ServiceProcess;

namespace Host.App
{
    public class Program
    {
        private readonly Action<string> _logger;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Program program = new Program();
            program.Run(args);
        }

        public Program()
        {
            _logger = Console.WriteLine;
        }

        private void Run(string[] args)
        {
            if (args.Length > 0 && args.Any(arg => arg == "/console"))
            {
                _logger("Launching host as a console application.");
                LaunchConsoleApplication(args);
            }
            else
            {
                _logger("Launching host as a Windows Service.");
                LaunchWindowsService();
            }
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
                HostService hostService = new HostService(_logger);
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
