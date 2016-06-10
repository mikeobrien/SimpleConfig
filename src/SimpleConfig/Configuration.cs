using System;
using System.Configuration;
using Bender;
using Bender.Configuration;

namespace SimpleConfig
{
    public interface IConfiguration
    {
        T LoadSection<T>(string sectionName = null, Action<DeserializerOptionsDsl> options = null);
    }

    public class Configuration : IConfiguration
    {
        private readonly string _configPath;
        private readonly Lazy<Deserializer> _deserializer;

        public Configuration(Action<OptionsDsl> options = null, string configPath = null)
        {
            _configPath = configPath;
            options = options ?? (x => { });
            _deserializer = new Lazy<Deserializer>(() => Deserializer.Create(x => options(x.IncludePublicFields()
                .Deserialization(y => y.IgnoreNameCase().IgnoreArrayItemNames().IgnoreRootName()))));
        }

        public static T Load<T>(Action<OptionsDsl> options = null, string sectionName = null, string configPath = null)
        {
            return new Configuration(options, configPath).LoadSection<T>(sectionName);
        }

        public T LoadSection<T>(string sectionName = null, Action<DeserializerOptionsDsl> options = null)
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
