using Host.Contract;
using Host.Contract.Log;

namespace Host.Tests.Plugin
{
    public class WellBehavedPlugin : PluginService
    {
        public override void OnStart(ILogger logger)
        {
            logger.Write($"{this.GetType().Name} is starting.");
        }

        public override void OnStop(ILogger logger)
        {
            logger.Write($"{this.GetType().Name} is stopping.");
        }
    }
}