using System;
using Nest;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using XCloud.Elasticsearch.Core;
using XCloud.Sales.ElasticSearch.Core;

namespace XCloud.Sales.ElasticSearch.Service.Goods;

[ElasticsearchType(RelationName = "goods_index", IdProperty = nameof(IEntityDto<string>.Id))]
public class GoodsIndex : IEntityDto<string>, IHasModificationTime, IEsIndex
{
    public GoodsIndex()
    {
        //
    }

    [Text(Name = nameof(Id))] public string Id { get; set; }

    [Text(Name = nameof(Name),
        Analyzer = SalesEsConst.IK_MAX_WORD,
        SearchAnalyzer = SalesEsConst.IK_MAX_WORD)]
    public string Name { get; set; }

    [Date(Name = nameof(LastModificationTime))]
    public DateTime? LastModificationTime { get; set; }
}