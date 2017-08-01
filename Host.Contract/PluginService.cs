using System;

namespace Host.Contract
{
    public abstract class PluginService : MarshalByRefObject
    {
        public abstract void OnStart(string[] args);
        public abstract void OnStop();
    }
}