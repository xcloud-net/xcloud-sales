using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Elasticsearch;
using XCloud.Sales.ElasticSearch.Configuration;
using XCloud.Sales.ElasticSearch.Service.Goods;
using XCloud.Sales.Service.Search;

namespace XCloud.Sales.ElasticSearch;

[DependsOn(typeof(ElasticSearchModule), typeof(SalesModule))]
public class SalesElasticSearchModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.ConfigEsClient();
        context.ConfigEsHealthCheck();
        
        context.Services.Configure<AbpAutoMapperOptions>(option =>
            option.AddMaps<SalesElasticSearchModule>(validate: false));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.RemoveAll<IGoodsSearchService>();
        context.Services.AddTransient<IGoodsSearchService, EsGoodsSearchService>();
    }

    public override void OnPostApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        using var s = app.ApplicationServices.CreateScope();
        s.ServiceProvider.ConfigElasticSearchJobs();
    }
}