using System;
using System.Linq;
using System.Xml.Serialization;
using Bender.Collections;

namespace SimpleConfig
{
    public static class Extensions
    {
        public static string ToCamelCase(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return value.First().ToString().ToLower() + (value.Length > 1 ? value.Substring(1) : "");
        }

        public static string GetXmlTypeName(this Type type)
        {
            var xmlType = type.GetCustomAttributes(typeof(XmlTypeAttribute), true).FirstOrDefault();
            return xmlType != null ? xmlType.As<XmlTypeAttribute>().TypeName : null;
        }
    }
}