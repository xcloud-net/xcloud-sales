using System;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Volo.Abp.Modularity;
using XCloud.Sales.ElasticSearch.Client;

namespace XCloud.Sales.ElasticSearch.Configuration;

public static class EsClientConfiguration
{
    public static void ConfigEsClient(this ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();

        var nodes = new[] { new Uri("http://localhost:9200") };

        var pool = new StaticConnectionPool(nodes);

        var settings = new ConnectionSettings(pool);

        var client = new ElasticClient(settings);

        context.Services.AddSingleton(new SalesEsClient(client));
        throw new NotImplementedException();
    }
}