using System.Diagnostics;
using System.ServiceProcess;

namespace Host.App
{
    public partial class WindowsHostService : ServiceBase
    {
        private HostService _hostService;

        public WindowsHostService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
#if DEBUG
            Debugger.Launch();
#endif

            _hostService = new HostService();
            _hostService.OnStart(args);
        }

        protected override void OnStop()
        {
            _hostService.OnStop();
        }
    }
}
