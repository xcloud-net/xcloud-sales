using Microsoft.Extensions.DependencyInjection;

namespace XCloud.Elasticsearch;

public static class ESBootstrap
{
    public static IServiceCollection UseElasticsearch(this IServiceCollection collection, 
        string servers, bool debug = false)
    {
        return collection;
    }
}