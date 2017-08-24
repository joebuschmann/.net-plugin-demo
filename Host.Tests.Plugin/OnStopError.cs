using System;
using Host.Contract;
using Host.Contract.Log;

namespace Host.Tests.Plugin
{
    public class OnStopError : PluginService
    {
        public override void OnStart(ILogger logger)
        {
        }

        public override void OnStop(ILogger logger)
        {
            throw new Exception("Error in OnStop");
        }
    }
}