using System.Configuration;
using Bender;

namespace SimpleConfig
{
    public interface IConfiguration<T>
    {
        T Load(string sectionName = null);
    }

    public class Configuration<T> : IConfiguration<T>
    {
        public T Load(string sectionName = null)
        {
            return Deserializer.Create(x => x.IgnoreCase()).Deserialize<T>(
                ((Section)ConfigurationManager.GetSection(sectionName ?? typeof(T).Name.ToCamelCase())).Element);
        }
    }
}
