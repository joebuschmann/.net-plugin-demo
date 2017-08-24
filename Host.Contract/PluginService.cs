using System;
using Host.Contract.Log;

namespace Host.Contract
{
    public abstract class PluginService : MarshalByRefObject
    {
        public abstract void OnStart(ILogger logger);
        public abstract void OnStop(ILogger logger);
    }
}