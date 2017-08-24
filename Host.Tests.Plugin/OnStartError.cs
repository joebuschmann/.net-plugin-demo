using System;
using Host.Contract;
using Host.Contract.Log;

namespace Host.Tests.Plugin
{
    public class OnStartError : PluginService
    {
        public override void OnStart(ILogger logger)
        {
            throw new Exception("Error in OnStart");
        }

        public override void OnStop(ILogger logger)
        {
        }
    }
}