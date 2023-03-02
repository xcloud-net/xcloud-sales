using System.Threading.Tasks;
using Nest;
using XCloud.Elasticsearch.Core;

namespace XCloud.Elasticsearch.Extension;

public static class DeleteExtension
{
    /// <summary>
    /// 删除文档
    /// </summary>
    public static async Task<DeleteResponse> DeleteDocAsync_<T>(this IElasticClient client, string indexName, string uid) where T : class, IEsIndex
    {
        var response = await client.DeleteAsync(client.ID<T>(indexName, uid));

        return response;
    }

    /// <summary>
    /// 通过条件删除
    /// </summary>
    public static async Task<DeleteByQueryResponse> DeleteByQueryAsync_<T>(this IElasticClient client, string indexName, QueryContainer where) where T : class, IEsIndex
    {
        var query = new DeleteByQueryRequest<T>(indexName) { Query = where };

        var response = await client.DeleteByQueryAsync(query);

        return response;
    }
}