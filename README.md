SimpleConfig
=============

SimpleConfig makes custom configuration in .NET much easier. Instead of wiring up and loading custom sections via the heavy and complicated API under the `System.Configuration` namespace, SimpleConfig allows you to load the section directly into a POCO graph via xml deserialization.

Install
------------

Bender can be found on nuget:

    PM> Install-Package SimpleConfig

Usage
------------

First create your configuration types:

```csharp
public class MyApplication
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
```

Next you need to register the SimpleConfig section handler in your `web/app.config` and create your configuration section as shown below. The default convention for the section name is the camel cased name of the root configuration type (Although you can override this as we'll see later). The section name under `configSections` must match the section element name. All other element and attribute names in the configuration section are case insensitive but must otherwise match the property names of your configuration types.

```xml
<configuration>
  <configSections>
    <section name="myApplication" type="SimpleConfig.Section, SimpleConfig"/>
  </configSections>
  <myApplication>
    <build version="0.0.0.0" date="10/25/1985" deployTarget="Dev"/>
  </myApplication>
</configuration>
```

Now you can load the section:

```csharp
var config = new SimpleConfig.Configuration().Load<Application>();
config.Build.Date.ShouldEqual(DateTime.Parse("10/25/1985"));
config.Build.DeployTarget.ShouldEqual(Target.Dev);
config.Build.Version.ShouldEqual("0.0.0.0");
```

If you want to override the default section name convention, you can pass a section name into the `Load()` method like so:

```csharp
var config = new SimpleConfig.Configuration().Load<Application>("myapp");
```

SimpleConfig also supports the `XmlElementAttribute` so you can also override the element name:

```csharp
...
public class Build
{
    ...
    [XmlElement("target")]
    public Target DeployTarget { get; set; }
}
```

```xml
<configuration>
  ...
  <myApplication>
    <build ... target="Dev"/>
  </myApplication>
</configuration>
```

Props
------------

Thanks to [JetBrains](http://www.jetbrains.com/) for providing OSS licenses! 