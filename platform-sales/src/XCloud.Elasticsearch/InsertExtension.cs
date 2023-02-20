using FluentAssertions;
using Nest;
using System.Linq;
using System.Threading.Tasks;

namespace XCloud.Elasticsearch;

public static class InsertExtension
{
    /// <summary>
    /// 添加到索引
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="client"></param>
    /// <param name="indexName"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public static async Task<BulkResponse> BulkIndexAsync_<T>(this IElasticClient client,
        string indexName, T[] data, string routing = null) where T : class, IESIndex
    {
        data.Should().NotBeNullOrEmpty();

        var data_list = data.Select(x => new BulkIndexOperation<T>(x)).ToArray();
        var bulk = new BulkRequest(indexName)
        {
            Operations = new BulkOperationsCollection<IBulkOperation>(data_list)
        };
        if (!string.IsNullOrWhiteSpace(routing))
            bulk.Routing = new Routing(routing);

        var response = await client.BulkAsync(bulk);
        return response;
    }

    /// <summary>
    /// 单个索引
    /// </summary>
    public static async Task<IndexResponse> AddIndexAsync_<T>(this IElasticClient client,
        string indexName, T model, string routing = null) where T : class, IESIndex
    {
        IndexDescriptor<T> BuildIndex(IndexDescriptor<T> m)
        {
            m = m.Index(indexName);
            if (!string.IsNullOrWhiteSpace(routing))
                m = m.Routing(routing);
            /*
             * use routing to replace parent parameter
            if (!string.IsNullOrWhiteSpace(parent))
                m = m.Parent(parent);*/
            return m;
        }

        var response = await client.IndexAsync(model, x => BuildIndex(x));
        return response;
    }
}