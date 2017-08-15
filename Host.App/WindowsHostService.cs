using System;
using System.Diagnostics;
using System.ServiceProcess;
using Host.Contract.Log;

namespace Host.App
{
    public class WindowsHostService : ServiceBase
    {
        private readonly ILogger _logger;
        private HostService _hostService;

        public WindowsHostService(ILogger logger)
        {
            _logger = logger;
            this.ServiceName = "Host Service";
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif

            _hostService = new HostService(_logger);
            _hostService.OnStart(args);
        }

        protected override void OnStop()
        {
            _hostService.OnStop();
        }
    }
}
