using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XCloud.Elasticsearch;

public static class IndexExtension
{
    /// <summary>
    /// 设置shards和replicas和model搜索deep
    /// https://www.elastic.co/guide/en/elasticsearch/client/net-api/current/writing-analyzers.html
    /// 创建自定义分词
    /// </summary>
    public static CreateIndexDescriptor GetCreateIndexDescriptor<T>(this CreateIndexDescriptor create_index_descriptor,
        int? shards = null, int? replicas = null, int deep = 5)
        where T : class, IESIndex
    {
        //shards and replicas
        var indexDescriptor = create_index_descriptor.Settings(s => s.NumberOfShards(shards).NumberOfReplicas(replicas));
        //mapping option
        indexDescriptor = indexDescriptor.Map(x => x.AutoMap<T>(maxRecursion: deep));

        return indexDescriptor;
    }

    /// <summary>
    /// 获取所有索引名
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public static async Task<string[]> GetAllIndexs(this IElasticClient client)
    {
        var response = await client.Cat.IndicesAsync();
        response.ThrowIfException();
        return response.Records.Select(x => x.Index).ToArray();
    }

    /// <summary>
    /// 如果索引不存在就创建
    /// </summary>
    /// <param name="client"></param>
    /// <param name="indexName"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static async Task CreateIndexIfNotExistsAsync_(this IElasticClient client, string indexName, Func<CreateIndexDescriptor, ICreateIndexRequest> selector = null)
    {
        indexName = indexName.ToLower();
        var exist_response = await client.Indices.ExistsAsync(indexName);
        exist_response.ThrowIfException();

        if (exist_response.Exists)
            return;

        var response = await client.Indices.CreateAsync(indexName, selector);
        response.ThrowIfException();
    }

    /// <summary>
    /// 删除索引
    /// </summary>
    /// <param name="client"></param>
    /// <param name="indexName"></param>
    /// <returns></returns>
    public static async Task DeleteIndexIfExistsAsync_(this IElasticClient client, string indexName)
    {
        indexName = indexName.ToLower();
        var exist_response = await client.Indices.ExistsAsync(indexName);
        exist_response.ThrowIfException();

        if (!exist_response.Exists)
            return;

        var response = await client.Indices.DeleteAsync(indexName);
        response.ThrowIfException();
    }

    /// <summary>
    /// 分词
    /// </summary>
    public static async Task<List<string>> AnalyzeAsync_(this IElasticClient client, string keyword, string analyzer = null)
    {
        AnalyzeDescriptor BuildAnalyze(AnalyzeDescriptor a)
        {
            a = a.Text(keyword);
            if (!string.IsNullOrWhiteSpace(analyzer))
                a = a.Analyzer(analyzer);
            return a;
        }
        var response = await client.Indices.AnalyzeAsync(x => BuildAnalyze(x));
        response.ThrowIfException();
        return response.Tokens.Select(x => x.Token).ToList();
    }
}