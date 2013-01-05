using System.Configuration;
using Bender;

namespace SimpleConfig
{
    public interface IConfiguration
    {
        T Load<T>(string sectionName = null);
    }

    public class Configuration : IConfiguration
    {
        public T Load<T>(string sectionName = null)
        {
            sectionName = sectionName ?? typeof (T).Name.ToCamelCase();
            var section = ConfigurationManager.GetSection(sectionName);
            if (!(section is Section)) throw new ConfigurationErrorsException(
                "The configuration section '" + sectionName + 
                "' must have a section handler of type '" +
                typeof(Section).FullName + "'.");
            if (section == null) throw new ConfigurationErrorsException(
                "Could not find configuration section '" + sectionName + "'.");
            return Deserializer.Create(x => x.IgnoreCase()).Deserialize<T>(((Section)section).Element);
        }
    }
}
