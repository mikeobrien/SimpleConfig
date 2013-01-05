using System;
using NUnit.Framework;
using Should;
using SimpleConfig;

namespace Tests
{
    [TestFixture]
    public class LoadConfigTests
    {
        public class Application
        {
            public Build Build { get; set; }
        }

        public enum Target { Dev, CI }

        public class Build
        {
            public string Version { get; set; }
            public DateTime Date { get; set; }
            public Target DeployTarget { get; set; }
        }

        [Test]
        public void should_load_config()
        {
            var config = new Configuration().Load<Application>();
            config.Build.Date.ShouldEqual(DateTime.Parse("10/25/1985"));
            config.Build.DeployTarget.ShouldEqual(Target.Dev);
            config.Build.Version.ShouldEqual("0.0.0.0");
        }
    }
}
