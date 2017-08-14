using System;
using Host.Contract;

namespace Host.App
{
    internal class PluginContext
    {
        private readonly AppDomain _appDomain;
        private readonly PluginService _service;

        public PluginContext(AppDomain appDomain, PluginService service)
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
