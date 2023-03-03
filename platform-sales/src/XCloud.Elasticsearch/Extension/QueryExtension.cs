using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentAssertions;
using Nest;
using XCloud.Core.Helper;
using XCloud.Elasticsearch.Core;

namespace XCloud.Elasticsearch.Extension;

public static class QueryExtension
{
    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    public static async Task<bool> DocExistAsync_<T>(this IElasticClient client, string indexName, string uid) where T : class, IEsIndex
    {
        var response = await client.DocumentExistsAsync(client.ID<T>(indexName, uid));
        return response.ThrowIfException().Exists;
    }

    /// <summary>
    /// 分页
    /// </summary>
    public static SearchDescriptor<T> QueryPage_<T>(this SearchDescriptor<T> sd, int page, int pagesize)
        where T : class, IEsIndex
    {
        var skip = PageHelper.GetPagedSkip(page, pagesize);
        return sd.Skip(skip).Take(page);
    }

    /// <summary>
    /// 给关键词添加高亮
    /// </summary>
    public static SearchDescriptor<T> AddHighlightWrapper<T>(this SearchDescriptor<T> sd,
        string preHtmlTag, string postHtmlTag, Func<HighlightFieldDescriptor<T>, IHighlightField>[] fieldHighlighters)
        where T : class, IEsIndex
    {
        fieldHighlighters.Should().NotBeNullOrEmpty();

        sd = sd.Highlight(x => x.PreTags(preHtmlTag).PostTags(postHtmlTag).Fields(fieldHighlighters));

        /*
        sd.Highlight(x => x.PreTags(pre).PostTags(after)
        .Fields(f => f
        .Field("comment")
        .HighlightQuery(q =>
        q.Match(m => m.Query("keywords")
        .Operator(Operator.Or).Analyzer("ik_smart").MinimumShouldMatch("100%")))));
        */

        return sd;
    }

    /// <summary>
    /// 根据距离排序
    /// </summary>
    public static SortDescriptor<T> SortByDistance<T>(this SortDescriptor<T> sort, Expression<Func<T, object>> field, GeoLocation point,
        bool desc = false) where T : class, IEsIndex
    {
        GeoDistanceSortDescriptor<T> BuildSort(GeoDistanceSortDescriptor<T> s)
        {
            s = s.Field(field).Points(point);
            s = desc ?
                s.Descending() :
                s.Ascending();
            return s;
        }

        return sort.GeoDistance(x => BuildSort(x));
    }
}