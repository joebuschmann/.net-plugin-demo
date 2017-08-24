using System;
using System.Linq;
using Host.App;
using Host.Tests.Contract;
using Host.Tests.Mocks;
using NUnit.Framework;

namespace Host.Tests
{
    [TestFixture]
    public class HostServiceFixture
    {
        private string _baseDirectory;

        [OneTimeSetUp]
        public void PluginDirectory()
        {
            // Use NUnit's test context to get the base directory in case the test runner shadow copies the assemblies.
            _baseDirectory = TestContext.CurrentContext.TestDirectory + "\\apps-test\\Host.Tests.Plugin";
        }

        [Test]
        public void ValidatePluginStartAndStop()
        {
            TestLogger logger = new TestLogger();
            TestPluginConfiguration pluginConfiguration = new TestPluginConfiguration();
            pluginConfiguration.AddConfigurationItem(new TestPluginConfigurationItem()
            {
                Description = "Well Behaved Plugin",
                Assembly = "Host.Tests.Plugin",
                Type = "Host.Tests.Plugin.WellBehavedPlugin",
                BaseDirectory = _baseDirectory
            });

            HostService hostService = new HostService(logger, pluginConfiguration);

            hostService.OnStart(new string[0]);
            hostService.OnStop();

            Assert.Contains("Well Behaved Plugin started successfully.", logger.Messages);
            Assert.Contains("Well Behaved Plugin stopped successfully.", logger.Messages);
        }

        [Test]
        public void ValidatePluginStartError()
        {
            TestLogger logger = new TestLogger();
            TestPluginConfiguration pluginConfiguration = new TestPluginConfiguration();
            pluginConfiguration.AddConfigurationItem(new TestPluginConfigurationItem()
            {
                Description = "Start Error",
                Assembly = "Host.Tests.Plugin",
                Type = "Host.Tests.Plugin.OnStartError",
                BaseDirectory = _baseDirectory
            });

            HostService hostService = new HostService(logger, pluginConfiguration);

            var exception = Assert.Catch<AggregateException>(() => hostService.OnStart(new string[0]));

            Assert.That(exception.Message,
                Is.EqualTo("One or more plugins failed to start. See inner exceptions for more details."));

            Assert.Contains("Error in OnStart", exception.InnerExceptions.Select(e => e.Message).ToList(),
                "The aggregate exception thrown from the service host should contain all exceptions thrown from plugins.");
        }

        [Test]
        public void ValidatePluginStopError()
        {
            TestLogger logger = new TestLogger();
            TestPluginConfiguration pluginConfiguration = new TestPluginConfiguration();
            pluginConfiguration.AddConfigurationItem(new TestPluginConfigurationItem()
            {
                Description = "Stop Error",
                Assembly = "Host.Tests.Plugin",
                Type = "Host.Tests.Plugin.OnStopError",
                BaseDirectory = _baseDirectory
            });

            HostService hostService = new HostService(logger, pluginConfiguration);

            hostService.OnStart(new string[0]);
            hostService.OnStop();

            Assert.Contains("Stopping Stop Error.", logger.Messages);
            Assert.IsTrue(logger.Messages.Count(m => m.StartsWith("Stop Error failed to stop.")) == 1);
        }
    }
}
