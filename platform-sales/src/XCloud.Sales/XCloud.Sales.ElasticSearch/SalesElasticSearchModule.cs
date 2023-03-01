using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;
using XCloud.Elasticsearch;
using XCloud.Sales.ElasticSearch.Service.Goods;
using XCloud.Sales.Service.Search;

namespace XCloud.Sales.ElasticSearch;

[DependsOn(typeof(ElasticSearchModule), typeof(SalesModule))]
public class SalesElasticSearchModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.Configure<AbpAutoMapperOptions>(option =>
            option.AddMaps<SalesElasticSearchModule>(validate: false));
    }

    public override void PostConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.RemoveAll<IGoodsSearchService>();
        context.Services.AddTransient<IGoodsSearchService, EsGoodsSearchService>();
    }
}