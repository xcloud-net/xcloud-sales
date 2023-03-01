using Nest;

namespace XCloud.Sales.ElasticSearch.Client;

public class SalesEsClient
{
    public SalesEsClient(IElasticClient elasticClient)
    {
        ElasticClient = elasticClient;
    }

    public IElasticClient ElasticClient { get; }
}