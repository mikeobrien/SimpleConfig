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
            return Deserializer.Create(x => x.IgnoreCase()).Deserialize<T>(
                ((Section)ConfigurationManager.GetSection(sectionName ?? typeof(T).Name.ToCamelCase())).Element);
        }
    }
}
