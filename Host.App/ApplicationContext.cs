using System;
using Host.Contract;

namespace Host.App
{
    internal class ApplicationContext
    {
        private readonly System.AppDomain _appDomain;
        private readonly PluginService _service;

        public ApplicationContext(System.AppDomain appDomain, PluginService service)
        {
            _appDomain = appDomain;
            _service = service;
        }

        public AppDomain AppDomain
        {
            get { return _appDomain; }
        }

        public PluginService Service
        {
            get { return _service; }
        }
    }
}
