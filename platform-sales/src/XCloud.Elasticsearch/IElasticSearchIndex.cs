using Nest;
using XCloud.Core.Helper;

namespace XCloud.Elasticsearch;

/// <summary>
/// 标记为索引model
/// </summary>
public interface IESIndex : IDbTableFinder { }

/// <summary>
/// 用于关键词补全
/// 分词默认是ik_max_word，可以override
/// </summary>
public abstract class CompletionSuggestIndexBase : IESIndex
{
    [Completion(Name = nameof(CompletionSearchTitle), Analyzer = "ik_max_word", SearchAnalyzer = "ik_max_word")]
    public virtual string CompletionSearchTitle { get; set; }
}