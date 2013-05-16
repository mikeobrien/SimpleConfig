using System;
using System.Configuration;
using Bender;

namespace SimpleConfig
{
    public interface IConfiguration
    {
        T LoadSection<T>(string sectionName = null);
    }

    public class Configuration : IConfiguration
    {
        private readonly string _configPath;
        private readonly Lazy<Deserializer> _deserializer;

        public Configuration(Action<DeserializerOptions> options = null, string configPath = null)
        {
            _configPath = configPath;
            options = options ?? (x => { });
            _deserializer = new Lazy<Deserializer>(() => Deserializer.Create(x => options(x.IgnoreCase().IgnoreTypeXmlElementNames())));
        }

        public static T Load<T>(string sectionName = null, Action<DeserializerOptions> options = null, string configPath = null)
        {
            return new Configuration(options, configPath).LoadSection<T>(sectionName);
        }

        public T LoadSection<T>(string sectionName = null)
        {
            sectionName = sectionName ?? typeof(T).GetXmlTypeName() ?? typeof(T).Name.ToCamelCase();
            var section = GetSection(sectionName, _configPath);
            if (!(section is Section)) throw new ConfigurationErrorsException(
                "The configuration section '" + sectionName + 
                "' must have a section handler of type '" +
                typeof(Section).FullName + "'.");
            if (section == null) throw new ConfigurationErrorsException(
                "Could not find configuration section '" + sectionName + "'.");
            return _deserializer.Value.DeserializeXml<T>(((Section)section).Element);
        }

        private object GetSection(string sectionName, string configPath)
        {
            return configPath != null ? 
                ConfigurationManager.OpenMappedMachineConfiguration(new ConfigurationFileMap(configPath)).GetSection(sectionName) :
                ConfigurationManager.GetSection(sectionName);
        }
    }
}
