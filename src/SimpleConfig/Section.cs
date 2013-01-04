using System.Configuration;
using System.Xml;
using System.Xml.Linq;

namespace SimpleConfig
{
    public class Section : ConfigurationSection
    {
        protected override void DeserializeSection(XmlReader reader)
        {
            Element = XElement.Load(reader);
        }

        public XElement Element { get; set; }
    }
}
