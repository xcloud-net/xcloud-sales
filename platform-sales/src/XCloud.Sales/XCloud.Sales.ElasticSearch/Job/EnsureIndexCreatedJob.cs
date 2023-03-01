using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using XCloud.Sales.Application;
using XCloud.Sales.ElasticSearch.Client;

namespace XCloud.Sales.ElasticSearch.Job;

[ExposeServices(typeof(EnsureIndexCreatedJob))]
public class EnsureIndexCreatedJob : SalesAppService, ITransientDependency
{
    private readonly SalesEsClient _salesEsClient;

    public EnsureIndexCreatedJob(SalesEsClient salesEsClient)
    {
        _salesEsClient = salesEsClient;
    }

    public virtual async Task EnsureIndexCreatedAsync()
    {
        await Task.CompletedTask;

        await this._salesEsClient.ElasticClient.Cat.IndicesAsync();

        throw new NotImplementedException();
    }
}