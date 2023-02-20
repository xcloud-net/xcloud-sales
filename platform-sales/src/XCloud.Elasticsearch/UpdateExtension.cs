using System.Threading.Tasks;
using Nest;

namespace XCloud.Elasticsearch;

public static class UpdateExtension
{
    /// <summary>
    /// 更新文档
    /// </summary>
    public static async Task<IUpdateResponse<T>> UpdateDocAsync_<T>(this IElasticClient client, string indexName, string uid, T doc) where T : class, IESIndex
    {
        var response = await client.UpdateAsync(client.ID<T>(indexName, uid), x => x.Doc(doc));

        return response;
    }
}