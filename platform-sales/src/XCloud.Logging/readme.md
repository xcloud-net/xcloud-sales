# logging

https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?view=aspnetcore-3.1

*The Logging property can have LogLevel and log provider properties. The LogLevel specifies the minimum level to log for selected categories. In the preceding JSON, Information and Warning log levels are specified. LogLevel indicates the severity of the log and ranges from 0 to 6:*

`Trace = 0, Debug = 1, Information = 2, Warning = 3, Error = 4, Critical = 5, and None = 6.`

``` c#
    //
    // 摘要:
    //     Defines logging severity levels.
    public enum LogLevel
    {
        Trace,
        Debug,
        Information,
        Warning,
        Error,
        Critical,
        None
    }
```

``` json
{
  "Logging": {
    "LogLevel": { // No provider, LogLevel applies to all the enabled providers.
      "Default": "Error",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Warning"
    },
    "Debug": { // Debug provider.
      "LogLevel": {
        "Default": "Information" // Overrides preceding LogLevel:Default setting.
      }
    },
    "Console": {
      "IncludeScopes": true,
      "LogLevel": {
        "Microsoft.AspNetCore.Mvc.Razor.Internal": "Warning",
        "Microsoft.AspNetCore.Mvc.Razor.Razor": "Debug",
        "Microsoft.AspNetCore.Mvc.Razor": "Error",
        "Default": "Information"
      }
    },
    "EventSource": {
      "LogLevel": {
        "Microsoft": "Information"
      }
    },
    "EventLog": {
      "LogLevel": {
        "Microsoft": "Information"
      }
    },
    "AzureAppServicesFile": {
      "IncludeScopes": true,
      "LogLevel": {
        "Default": "Warning"
      }
    },
    "AzureAppServicesBlob": {
      "IncludeScopes": true,
      "LogLevel": {
        "Microsoft": "Information"
      }
    },
    "ApplicationInsights": {
      "LogLevel": {
        "Default": "Information"
      }
    }
  }
}
```

``` json
{
  "Logging": {
    "LogLevel": { // All providers, LogLevel applies to all the enabled providers.
      "Default": "Error", // Default logging, Error and higher.
      "Microsoft": "Warning" // All Microsoft* categories, Warning and higher.
    },
    "Debug": { // Debug provider.
      "LogLevel": {
        "Default": "Information", // Overrides preceding LogLevel:Default setting.
        "Microsoft.Hosting": "Trace " // Debug:Microsoft.Hosting category.
      },
      "EventSource": { // EventSource provider
        "LogLevel": {
          "Default": "Warning" // All categories of EventSource provider.
        }
      }
    }
  }
}
```

# nlog

> 归档日期：`2020-8-27`

## 代码配置
```csharp
        /// <summary>
        /// nlog
        /// </summary>
        public static ILoggingBuilder __add_nlog__(this ILoggingBuilder builder, IConfiguration config, string config_file)
        {
            File.Exists(config_file).Should().BeTrue($"nlog配置文件不存在:{config_file}");

            var xml = File.ReadAllText(config_file);

            var log_path = config["log_base_dir"];
            if (!string.IsNullOrWhiteSpace(log_path) && Directory.Exists(log_path))
            {
#if xx
                        var app_name = new[] {
                            config.GetAppName() ,
                            System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name,
                            "app"
                        }.FirstNotEmpty_();

                        log_path = Path.Combine(log_path, app_name);
#endif

                new DirectoryInfo(log_path).CreateIfNotExist();

                var nlog_config_data = NLog.Config.XmlLoggingConfiguration.CreateFromXmlString(xml);
                nlog_config_data.Variables["log_base_dir"] = log_path;

                builder.AddNLog(nlog_config_data);
            }
            return builder;
        }
```

## 配置文件
``` xml
<?xml version="1.0" encoding="utf-8"?>
<nlog xmlns="http://www.nlog-project.org/schemas/NLog.xsd"
      xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
      autoReload="true"
      throwExceptions="false"
      throwConfigExceptions="true">

  <variable name="app_name" value="im-server" />

  <variable name="log_base_dir" value="/api-logs" />

  <variable name="logDirectory" value="${var:log_base_dir}" />

  <targets>
    <!--
        需要在appsetting里配置log_base_dir才能开启日志
        
        https://github.com/NLog/NLog/wiki/File-target
        archiveAboveSize会影响性能
        
        size=20971520（大约20MB）
        -->
    <target name="logfile" xsi:type="File"
            fileName="${var:logDirectory}/app.${app_name}.${shortdate}.log"
            archiveFileName="${var:logDirectory}/.archives/app.${app_name}.${shortdate}.{#####}.txt"
            archiveAboveSize="20971520"
            archiveNumbering="Sequence"
            concurrentWrites="true">
      <layout xsi:type="JsonLayout">
        <attribute name="app" layout="${app_name}" />
        <attribute name="time" layout="${longdate}" />
        <attribute name="logger" layout="${logger}" />
        <attribute name="level" layout="${level:upperCase=true}"/>
        <attribute name="message" layout="${message}" escapeUnicode="false" />
        <attribute name="exception" layout="${exception:format=toString}" escapeUnicode="false" />
        <attribute name="stacktrace" layout="${stacktrace}" escapeUnicode="false" />
      </layout>
    </target>
  </targets>

  <rules>
    <logger name="*" minlevel="Info" writeTo="logfile" />
  </rules>

</nlog>
```