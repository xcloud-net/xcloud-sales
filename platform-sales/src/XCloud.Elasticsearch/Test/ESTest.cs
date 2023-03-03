using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Elasticsearch.Net;
using Nest;
using XCloud.Elasticsearch.Core;
using XCloud.Elasticsearch.Extension;

namespace XCloud.Elasticsearch.Test;

static class ESTest
{
    /// <summary>
    /// has parent/child。child and parent必须在一个索引
    /// 并且在一个分片(所以要指定routing[shard=hash(routing)%shard_num])
    /// nest在7.0以后取消了parent函数，直接用routing来指定parent and children的分片
    /// </summary>
    static void HasChild(IElasticClient client)
    {
        //https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/has-child-query-usage.html
        client.Search<EsIndexExample>(sd => sd.Query(q =>
            q.HasChild<EsIndexExample>(c => c
                .MinChildren(1).MaxChildren(10).ScoreMode(ChildScoreMode.Average)
                .Query(qq => qq.Term(t => t.Field(f => f.PIsRemove).Value(0))))));

        //https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/has-parent-query-usage.html
        client.Search<EsIndexExample>(sd => sd.Query(q =>
            q.HasParent<EsIndexExample>(c => c
                .Score(true)
                .Query(qq => qq.Term(t => t.Field(f => f.PIsRemove).Value(0))))));

        //add to index
        client.Index(new EsIndexExample(), x => x.Routing("parent ukey"));
    }

    static void GetCreateIndexDescriptor<T>(CreateIndexDescriptor create_index_descriptor,
        int? shards = null, int? replicas = null, int deep = 5)
        where T : class, IEsIndex
    {
        //shards and replicas
        var indexDescriptor = create_index_descriptor.Settings(s => s.NumberOfShards(shards).NumberOfReplicas(replicas));
        //mapping option
        indexDescriptor = indexDescriptor.Map(x => x.AutoMap<T>(maxRecursion: deep).SourceField(s => s.Enabled(false)));

    }

    static void ReScore(IElasticClient client)
    {
        var rescore = new RescoringDescriptor<EsIndexExample>();

        var query = new QueryContainer() && new MatchAllQuery();
        var function_query = new FunctionScoreQuery()
        {
            Query = new MatchAllQuery(),
            ScoreMode = FunctionScoreMode.Average,
            Functions = new List<IScoreFunction>()
            {
                new GaussDateDecayFunction(){ },
                new ScriptScoreFunction(){ }
            }
        };
        var script_query = new ScriptQuery() { };
        var script_query_ = new ScriptScoreFunction() { };

        rescore = rescore.Rescore(x =>
                x.RescoreQuery(d => d.Query(q => query)
                    .QueryWeight(0.5)
                    .RescoreQueryWeight(0.5)
                    .ScoreMode(ScoreMode.Average)).WindowSize(10))
            .Rescore(x =>
                x.RescoreQuery(d => d.Query(q => function_query)
                    .QueryWeight(0.5)
                    .RescoreQueryWeight(0.5)
                    .ScoreMode(ScoreMode.Average)).WindowSize(10));

        //use rescore
        var sd = new SearchDescriptor<EsIndexExample>();
        sd = sd.Rescore(x => rescore).Source(false);
    }

    /// <summary>
    /// 使用cursor遍历整个索引
    /// </summary>
    static void HowToScrollIndex(IElasticClient client)
    {
        var res = client.Search<EsIndexExample>(s => s
            .From(0)
            .Size(1)
            .MatchAll()
            .Scroll(new Time(TimeSpan.FromSeconds(4)))
        );
        res.ThrowIfException();
        if (string.IsNullOrWhiteSpace(res.ScrollId)) { throw new System.Exception("未能拿到游标地址"); }

        while (true)
        {
            res = client.Scroll<EsIndexExample>("4s", res.ScrollId);
            if (!res.Documents.Any())
            {
                break;
            }
            foreach (var doc in res.Documents)
            {
                //do something
                client.Delete(new DeleteRequest("index", "id"));
                client.Delete(DocumentPath<EsIndexExample>.Id("ukey").Index("index"));
            }
        }
    }

    /// <summary>
    /// 怎么通过距离筛选，请看源代码
    /// ES空间搜索
    /// </summary>
    /// <param name="qc"></param>
    static void HowToFilterByDistance(QueryContainer qc)
    {
        qc = qc && new GeoBoundingBoxQuery()
        {
            Field = "name",
            BoundingBox = new BoundingBox()
            {
                TopLeft = new GeoLocation(212, 32),
                BottomRight = new GeoLocation(43, 56)
            }
        };
        qc = qc && new GeoDistanceQuery()
        {
            Field = "Field Name",
            Location = new GeoLocation(32, 43),
        };
        qc = qc && new GeoShapeQuery()
        {
            Field = "name",
            Shape = new CircleGeoShape(new GeoCoordinate(324, 535), radius: ""),
            Relation = GeoShapeRelation.Within
        };
        qc = qc && new GeoShapeQuery()
        {
            Field = "xx",
            Shape = new PolygonGeoShape(new List<IEnumerable<GeoCoordinate>>()
            {
                new List<GeoCoordinate>() { }
            }),
            Relation = GeoShapeRelation.Within
        };
        qc = qc && new GeoDistanceQuery()
        {
            Field = "Location",
            Location = new GeoLocation(32, 43),
            Distance = Distance.Kilometers(1)
        };
        qc &= new GeoShapeQuery()
        {
            Field = "Location",
            Shape = new EnvelopeGeoShape(new List<GeoCoordinate>() { }),
            Relation = GeoShapeRelation.Intersects,
        };
        qc &= new GeoShapeQuery()
        {
            Field = "Location",
            Shape = new PointGeoShape(new GeoCoordinate(32, 32)),
            Relation = GeoShapeRelation.Intersects
        };
        qc &= new GeoShapeQuery()
        {
            Field = "location",
            Shape = new MultiPolygonGeoShape(new List<List<List<GeoCoordinate>>>() { }) { },
            Relation = GeoShapeRelation.Intersects
        };
        //使用场景：一个销售区域支持多个配送闭环范围，查询当前位置在不在配送范围内
        var model = new
        {
            nested_sales_area = new object[]
            {
                new
                {
                    cordinates=new List<GeoCoordinate>(){ }
                },
                new
                {
                    cordinates=new List<GeoCoordinate>(){ }
                },
            }
        };
        var nested_query = new QueryContainer();
        nested_query &= new GeoShapeQuery()
        {
            Field = "nested_sales_area.cordinates",
            Shape = new PointGeoShape(new GeoCoordinate(32, 32)),
            Relation = GeoShapeRelation.Intersects
        };
        qc &= new NestedQuery()
        {
            Path = "nested_sales_area",
            Query = nested_query
        };
    }

    static void HowToUseAggregationsInES(IElasticClient client,
        SearchDescriptor<EsIndexExample> sd)
    {
        var agg = new AggregationContainer();
        agg = new SumAggregation("", "") && new AverageAggregation("", "");

        //select x,count(1) from tb group by x
        sd = sd.Aggregations(a => a.Terms("terms", x => x.Field(m => m.IsGroup).Order(s => s.CountDescending()).Size(1000)));
        //select count(f) from tb where f is not null
        sd = sd.Aggregations(a => a.ValueCount("count", x => x.Field(m => m.IsGroup)));
        //最大值
        sd = sd.Aggregations(a => a.Max("max", x => x.Field(m => m.IsGroup)));
        //最大值，最小值，平均值等统计数据
        sd = sd.Aggregations(a => a.Stats("stats", x => x.Field(m => m.BrandId).Field(m => m.PIsRemove)));
        //直方图
        sd = sd.Aggregations(a => a.Histogram("price", x => x.Field("price").Interval(60)));
        //时间直方图
        sd = sd.Aggregations(a => a.DateHistogram("date", x => x.Field("date").FixedInterval(new Time(TimeSpan.FromHours(1)))));
        //数值区域
        sd = sd.Aggregations(a => a.Range("range", x => x.Field("price").Ranges(r => r.From(10).To(20), r => r.From(30).To(40))));
        //日期区域
        sd = sd.Aggregations(a => a.DateRange("date_range", x => x.Field("date").Ranges(r => r.From(DateTime.Now.AddDays(-1)).To(DateTime.Now))));
        //ip区域
        sd = sd.Aggregations(a => a.IpRange("ip_range", x => x.Field("ip").Ranges(r => r.From("192.168.0.1").To("192.168.0.10"))));


        //sd = sd.SearchType(SearchType.QueryThenFetch);
        var response = client.Search<EsIndexExample>(x => sd);
        response.ThrowIfException();

        var terms = response.Aggregations.Terms("group").Buckets;
        var count = response.Aggregations.ValueCount("count").Value;
        var stats = response.Aggregations.Stats("stats");
        var max = response.Aggregations.Max("max").Value;
        var histogram = response.Aggregations.Histogram("hist").Buckets.ToDictionary(x => x.Key, x => x.DocCount);
        var datehistogram = response.Aggregations.DateHistogram("date_hist").Buckets.ToDictionary(x => x.Date, x => x.DocCount);
        var range = response.Aggregations.Range("range").Buckets.Select(x => new { x.From, x.To, x.DocCount });
        var date_range = response.Aggregations.DateRange("date_range").Buckets.Select(x => new { x.From, x.To, x.DocCount });
        //etc
    }

    static void HowToSortWithScripts<T>(SortDescriptor<T> sort) where T : class, IEsIndex
    {
        var sd = new ScriptSortDescriptor<T>();

        sd = sd.Mode(SortMode.Sum).Type("number");
        var script = "doc['price'].value * params.factor";
        sd = sd.Script(x => x.Source(script).Lang("painless").Params(new Dictionary<string, object>()
        {
            ["factor"] = 1.1
        }));

        sort.Script(x => sd.Descending());
    }

    static void HowToUseNestedQuery(QueryContainer qc)
    {
        var attrList = new List<___AttrParam>();

        var attrQuery = new QueryContainer();
        foreach (var attr in attrList)
        {
            attrQuery = attrQuery || new TermQuery() { Field = $"ProductAttributes.{attr.Id}", Value = attr.value };
        }
        qc = qc && new NestedQuery()
        {
            Path = "ProductAttributes",
            Query = attrQuery
        };
    }

    static void HowToUseNestedSort(SortDescriptor<EsIndexExample> sort)
    {
        /*
        s => s
.Sort(ss => ss
 .Field(f => f
     .Field(p => p.Tags.First().Added)
     .Order(SortOrder.Descending)
     .MissingLast()
     .UnmappedType(FieldType.Date)
     .Mode(SortMode.Average)
     .Nested(n => n
         .Path(p => p.Tags)
         .Filter(ff => ff
             .MatchAll()
         )
     )
 )
)
          */

        sort = sort.Field(x =>
            x.Field(m => m.Area.First().Latitude).Mode(SortMode.Max).Nested(n => n.Path("").Filter(f => f.MatchAll())).Ascending());
    }

    /// <summary>
    /// query会计算匹配度（_score）
    /// filter不会计算匹配度，只会计算是否匹配，并且有查询缓存
    /// 不需要匹配度的查询使用filter效率更高
    /// </summary>
    static void DifferenceBetweenQueryAndFilter()
    {
        var sd = new SearchDescriptor<EsIndexExample>();

        //postfilter有坑，后续研究!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //sd = sd.PostFilter(x => new QueryContainer());

        //正确的filter应该这么做
        sd = sd.Query(x => new BoolQuery()
        {
            Filter = new List<QueryContainer>() { new QueryContainer() }
        });
        //query
        sd = sd.Query(x => new QueryContainer());
    }

    static void DifferentQuerysInEs(QueryContainer qc)
    {
        //匹配查询
        qc &= new MatchQuery()
        {
            Field = "analyized field name",
            Query = "关键词",
            Operator = Operator.Or,
            MinimumShouldMatch = "100%",
            Analyzer = "ik_smart"
        };

        //https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-query-string-query.html#query-string-syntax
        //https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-simple-query-string-query.html
        //query string自定义了一个查询语法
        var qsq = new QueryStringQuery() { };

        //https://www.elastic.co/guide/cn/elasticsearch/guide/current/_wildcard_and_regexp_queries.html
        //使用通配符查询，比如name.*
        var wcq = new WildcardQuery() { };

        //精准匹配，不分词
        var tq = new TermQuery() { };

        //字段存在且不为空
        var eq = new ExistsQuery() { };

        //https://www.elastic.co/guide/cn/elasticsearch/guide/current/fuzzy-query.html
        //模糊查询，它会计算关键词和目标字段的“距离”。如果在允许的距离范围内，计算拼写错误也可以匹配到
        var fq = new FuzzyQuery() { };

        //范围查询
        var drq = new DateRangeQuery() { };
        var nrq = new NumericRangeQuery() { };
    }

    /// <summary>
    /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/function-score-query-usage.html
    /// </summary>
    /// <param name="sd"></param>
    static void HowToUseFunctionQuery(SearchDescriptor<EsIndexExample> sd)
    {
        var qs = new FunctionScoreQuery()
        {
            Name = "named_query",
            Boost = 1.1,
            Query = new MatchAllQuery { },
            BoostMode = FunctionBoostMode.Multiply,
            ScoreMode = FunctionScoreMode.Sum,
            MaxBoost = 20.0,
            MinScore = 1.0,
            Functions = new List<IScoreFunction>
            {
                new ExponentialDecayFunction { Origin = 1.0, Decay =    0.5, Field = "", Scale = 0.1, Weight = 2.1 },
                new GaussDateDecayFunction { Origin = DateMath.Now, Field = "", Decay = 0.5, Scale = TimeSpan.FromDays(1) },
                new LinearGeoDecayFunction { Origin = new GeoLocation(70, -70), Field = "", Scale = Distance.Miles(1), MultiValueMode = MultiValueMode.Average },
                new FieldValueFactorFunction
                {
                    Field = "x", Factor = 1.1,    Missing = 0.1, Modifier = FieldValueFactorModifier.Ln
                },
                new RandomScoreFunction { Seed = 1337 },
                new GaussGeoDecayFunction() { Origin=new GeoLocation(32,4) },
                new RandomScoreFunction { Seed = "randomstring" },
                new WeightFunction { Weight = 1.0},
                new ScriptScoreFunction { Script = new InlineScript(script:"")}
            }
        };
        sd = sd.Query(x => qs);
        sd = sd.Sort(x => x.Descending(s => s.UpdatedDate));
        sd = sd.Skip(0).Take(10);
        new ElasticClient().Search<EsIndexExample>(_ => sd);
    }

    static void HowToUseInnerAgg()
    {
        var sd = new SearchDescriptor<EsIndexExample>();
        sd = sd.Aggregations(agg => agg
            .Terms("NAMEOF_ShowCatalogIdList", av => av.Field("NAMEOF_ShowCatalogIdList").Size(1000))
            .Terms("NAMEOF_BrandId", av => av.Field("NAMEOF_BrandId").Order(x => x.CountDescending()).Size(1000))
            .Terms("NAMEOF_ProductAttributes",
                //sub aggregation
                av => av.Field("NAMEOF_ProductAttributes")
                    .Aggregations(m => m.Average("", d => d.Field(""))).Order(xx => xx.Descending("")).Size(1000)));

        //nested agg
        var path = string.Empty;
        sd = sd.Aggregations(x => x
            .Nested("attr_nested", _ => _.Path(path).Aggregations(a => a.Terms("attr", f => f.Field($"{path}.Id").Size(1000)))));
    }

    /// <summary>
    /// https://stackoverflow.com/questions/42210930/nest-how-to-use-updatebyquery
    /// </summary>
    /// <param name="client"></param>
    static void HowToUpdateDocByScriptQuery(IElasticClient client)
    {
        var query = new QueryContainer();
        query &= new TermQuery() { Field = "name", Value = "wj" };

        client.UpdateByQuery<EsIndexExample>(q => q.Query(rq => query).Script(script => script
            .Source("ctx._source.name = newName;")
            .Params(new Dictionary<string, object>() { ["newName"] = "wj" })));

        //
        client.Update(DocumentPath<EsIndexExample>.Id(""),
            x => x.Index("").Doc(new EsIndexExample() { }));
    }

    /// <summary>
    /// 搜索建议
    /// https://elasticsearch.cn/article/142
    /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/suggest-usage.html
    /// https://www.elastic.co/guide/en/elasticsearch/reference/6.2/search-suggesters.html
    /// https://www.elastic.co/guide/en/elasticsearch/reference/6.2/search-suggesters-completion.html
    /// </summary>
    static ISuggestDictionary<T> SuggestSample<T>(IElasticClient client,
        string index,
        Expression<Func<T, object>> targetField, string text, string analyzer = null,
        string highlight_pre = "<em>", string hightlight_post = "</em>", int size = 20)
        where T : class, IEsIndex
    {
        var sd = new TermSuggesterDescriptor<T>();
        sd = sd.Field(targetField).Text(text);
        if (!string.IsNullOrWhiteSpace(analyzer))
        {
            sd = sd.Analyzer(analyzer);
        }
        sd = sd.Size(size);

        var csd = new CompletionSuggesterDescriptor<T>();
        var psd = new PhraseSuggesterDescriptor<T>();

        var response = client.Search<T>(s => s.Suggest(ss => ss
            .Term("my-term-suggest", t => t
                .MaxEdits(1)
                .MaxInspections(2)
                .MaxTermFrequency(3)
                .MinDocFrequency(4)
                .MinWordLength(5)
                .PrefixLength(6)
                .SuggestMode(SuggestMode.Always)
                .Analyzer("standard")
                .Field("")
                .ShardSize(7)
                .Size(8)
                .Text(text)
            )
            .Completion("my-completion-suggest", c => c
                .Contexts(ctxs => ctxs
                    .Context("color",
                        ctx => ctx.Context(text)
                    )
                )
                .Fuzzy(f => f
                    .Fuzziness(Fuzziness.Auto)
                    .MinLength(1)
                    .PrefixLength(2)
                    .Transpositions()
                    .UnicodeAware(false)
                )
                .Analyzer("simple")
                .Field("")
                .Size(8)
                .Prefix(text)
            )
            .Phrase("my-phrase-suggest", ph => ph
                .Collate(c => c
                    .Query(q => q
                        .Source("{ \"match\": { \"{{field_name}}\": \"{{suggestion}}\" }}")
                    )
                    .Params(p => p.Add("field_name", "title"))
                    .Prune()
                )
                .Confidence(10.1)
                .DirectGenerator(d => d
                    .Field("")
                )
                .GramSize(1)
                .Field("")
                .Text(text)
                .RealWordErrorLikelihood(0.5)
            )
        ));
        response.ThrowIfException();

        return response.Suggest;
    }
}

[ElasticsearchType(IdProperty = "UKey", RelationName = "ProductList")]
class EsIndexExample : IEsIndex
{
    [Text(Name = "UKey", Index = false)]
    public string UKey { get; set; }

    [Text(Name = "字段名", Index = false)]
    public string BrandId { get; set; }

    [Number(Name = "PIsRemove", Index = false)]
    public int PIsRemove { get; set; }

    [Number(Name = "IsGroup", Index = false)]
    public int IsGroup { get; set; }

    [Text(Name = "ShopName", Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
    public string ShopName { get; set; }

    [Text(Name = "SeachTitle", Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
    public string SeachTitle { get; set; }

    [Date(Name = "UpdatedDate")]
    public DateTime UpdatedDate { get; set; }

    [GeoPoint(Name = nameof(Location))]
    public GeoLocation Location { get; set; }

    [GeoShape(Name = nameof(Area))]
    public List<GeoLocation> Area { get; set; }
}

class ___AttrParam
{
    public virtual string Id { get; set; }

    public virtual string value { get; set; }
}