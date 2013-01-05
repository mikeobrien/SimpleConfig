using System;
using System.Collections.Generic;
using System.Linq;
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
            public List<string> Dependencies { get; set; }
        }

        [Test]
        public void should_load_config()
        {
            var config = new Configuration().LoadSection<Application>();
            config.Build.Date.ShouldEqual(DateTime.Parse("10/25/1985"));
            config.Build.DeployTarget.ShouldEqual(Target.Dev);
            config.Build.Version.ShouldEqual("0.0.0.0");
        }

        [Test]
        public void should_load_config_from_named_section()
        {
            var config = new Configuration().LoadSection<Application>("app");
            config.Build.Date.ShouldEqual(DateTime.Parse("10/25/1985"));
            config.Build.DeployTarget.ShouldEqual(Target.Dev);
            config.Build.Version.ShouldEqual("0.0.0.0");
        }

        [Test]
        public void should_load_config_with_convenience_method()
        {
            var config = Configuration.Load<Application>();
            config.Build.Date.ShouldEqual(DateTime.Parse("10/25/1985"));
            config.Build.DeployTarget.ShouldEqual(Target.Dev);
            config.Build.Version.ShouldEqual("0.0.0.0");
        }

        [Test]
        public void should_load_config_with_custom_options()
        {
            var config = new Configuration(x => x
            .AddReader<List<string>>((options, property, node) => node.Value.Split(',').ToList())).LoadSection<Application>("custom");
            var dependencies = config.Build.Dependencies;
            dependencies.Count.ShouldEqual(2);
            dependencies[0].ShouldEqual("this");
            dependencies[1].ShouldEqual("that");
        }
    }
}
