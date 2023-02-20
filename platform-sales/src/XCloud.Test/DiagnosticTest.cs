using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace XCloud.Test;

[TestClass]
public class DiagnosticTest
{
    class XxObserver : IObserver<KeyValuePair<string, object>>
    {
        public void OnCompleted()
        {
            //
        }

        public void OnError(Exception error)
        {
            //
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            //
        }
    }

    class HttpObserver : IObserver<KeyValuePair<string, object>>
    {
        public void OnCompleted()
        {
            //
        }

        public void OnError(Exception error)
        {
            //
        }

        public void OnNext(KeyValuePair<string, object> value)
        {
            //
        }
    }

    class DiagnosticListenerObserver : IObserver<DiagnosticListener>
    {
        public void OnCompleted()
        {
            //
        }

        public void OnError(Exception error)
        {
            //
        }

        public void OnNext(DiagnosticListener value)
        {
            if (value.Name == "xx")
            {
                value.Subscribe(new XxObserver());
            }
            if (value.Name == "HttpHandlerDiagnosticListener")
            {
                value.Subscribe(new HttpObserver());
            }
        }
    }

    [TestMethod]
    public async Task D()
    {
        var sub = DiagnosticListener.AllListeners.Subscribe(new DiagnosticListenerObserver());

        using (var source = new DiagnosticListener("xx"))
        {
            if (source.IsEnabled("dd"))
            {
                source.Write("dd", new { DateTime.UtcNow });
            }
        }

        var collection = new ServiceCollection();
        collection.AddHttpClient();

        var provider = collection.BuildServiceProvider();
        using var s = provider.CreateScope();

        var client = s.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("d");

        using var res = await client.GetAsync("https://www.qq.com");

        var html = await res.Content.ReadAsStringAsync();

        using (sub) { }
    }
}